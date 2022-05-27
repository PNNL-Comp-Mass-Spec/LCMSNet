using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using LcmsNet.Data;
using LcmsNet.Devices;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that can handle reading a method from a file path.
    /// </summary>
    public class LCMethodReader
    {
        private const string CONST_METHOD_EXTENSION = "*.xml";

        #region Events and Delegates

        /// <summary>
        /// Definition for a method that
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate bool DelegateValidateDevice(object sender, LCMethodDeviceArgs args);

        #endregion

        private static readonly Dictionary<string, string> InterfaceToImplementationMapper =
            new Dictionary<string, string>
            {
                { "LcmsNetSDK.Data.ISampleInfo", "LcmsNet.Data.SampleData" }
            };

        /// <summary>
        /// Loads a method from the path provided.
        /// </summary>
        /// <param name="filePath">Path to load method from</param>
        /// <param name="errors"></param>
        private static void LoadMethod(string filePath, ref List<Exception> errors)
        {
            //bool retValue = false;

            var reader = new LCMethodReader();
            LCMethod method;
            try
            {
                method = reader.ReadMethod(filePath, errors);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not load method from " + filePath, ex);
                throw;
            }

            //
            // If the method failed to load...then...return false
            //
            if (method == null)
                throw new Exception("The method was not able to be read.");

            // Figure out if the method exists.
            if (LCMethodManager.Manager.MethodExists(method.Name))
            {
                //TODO: Figure out what to do if a duplicate method exists.
                var errorMessage = $"The user method name from {filePath} conflicts with another method.";
                ApplicationLogger.LogMessage(0, errorMessage);
                throw new Exception(errorMessage);
            }
            //
            // Otherwise, add the method so it can be registered with appropriate objects
            //
            LCMethodManager.Manager.AddOrUpdateMethod(method);
        }

        /// <summary>
        /// Loads methods stored in path.  Top-level directory only.
        /// </summary>
        /// <param name="path">Path to load methods from.</param>
        /// <returns>True if successful</returns>
        public static Dictionary<string, List<Exception>> LoadMethods(string path)
        {
            var errors = new Dictionary<string, List<Exception>>();

            //
            // Find each file in the directory
            //
            var filePaths = Directory.GetFiles(path, CONST_METHOD_EXTENSION, SearchOption.TopDirectoryOnly);
            foreach (var filePath in filePaths)
            {
                try
                {
                    var methodErrors = new List<Exception>();
                    LoadMethod(filePath, ref methodErrors);

                    if (methodErrors.Count > 0)
                    {
                        errors.Add(filePath, methodErrors);
                    }
                }
                catch (Exception exception)
                {
                    errors.Add(filePath, new List<Exception> { exception });

                    ApplicationLogger.LogError(0,
                        "An unhandled exception occurred when reading a user method.",
                        exception);
                }
            }
            return errors;
        }

        public LCMethodReader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        /// <summary>
        /// Finds the type within the loaded assemblies.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Type FindType(string name)
        {
            Type constructedParameterType = null;
            // This could be a bit slow.  It's intended to load the
            // assemblies and their types.  Since we are loading plugins from another directory.
            // Then the CLR does not know where to look necessarily.  Here we are
            // looking within the loaded assemblies to find the type.
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var assemblies = new List<Assembly>();
            assemblies.AddRange(loadedAssemblies);
            assemblies.Add(Assembly.GetExecutingAssembly()); // Make sure we check the LCMS Net ones as well.

            for (var i = 0; i < 2; i++)
            {
                foreach (var asm in assemblies)
                {
                    try
                    {
                        var types = asm.GetTypes();
                        foreach (var asmType in types)
                        {
                            // Alternatively use .AssemblyQualifiedName;
                            if (asmType.FullName == name)
                            {
                                constructedParameterType = asmType;
                                return constructedParameterType;
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Ignore...we don't need to worry about these problems.
                    }
                }

                // if it wasn't found, try matching it with an old device type name and translating it
                name = OldDeviceNameTranslator.TranslateOldDeviceFullName(name);
                if (InterfaceToImplementationMapper.TryGetValue(name, out var implementation))
                {
                    name = implementation;
                }
            }
            ApplicationLogger.LogError(0, $"Failed to find type match for type name '{name}'");

            return constructedParameterType;
        }

        /// <summary>
        /// Reads the LC-Event from the specified node.
        /// </summary>
        /// <param name="node">Node that contains the event definition</param>
        /// <returns>An LC-Event</returns>
        private LCEvent ReadEventNode(XmlNode node)
        {
            var lcEvent = new LCEvent();

            // Read the name
            if (node.Attributes != null)
            {
                var nameAttribute = node.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_NAME);
                lcEvent.Name = nameAttribute.Value;
            }

            // Construct flags
            var value = node.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_OPTIMIZE_WITH);
            lcEvent.OptimizeWith = Convert.ToBoolean(value.Value);
            value = node.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_HAS_DISCREET_STATES);
            if (value != null)
            {
                lcEvent.HasDiscreteStates = Convert.ToBoolean(value.Value);
            }

            try
            {
                value = node.Attributes.GetNamedItem(LCMethodFactory.CONST_IS_EVENT_INDETERMINANT);
                if (value != null)
                {
                    lcEvent.IsIndeterminant = Convert.ToBoolean(value.Value);
                }
            }
            catch
            {
                //Debug.WriteLine(
                //    "Null Reference Exception due to backwards compatibility check in LCMethodReader");
                // This is to be backwards compatible.
            }

            // Start Time
            value = node.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_START);
            if (TimeSpan.TryParse(value.Value, out var startSpan))
            {
                lcEvent.Start = DateTime.MinValue.Add(startSpan);
            }
            else
            {
                lcEvent.Start = Convert.ToDateTime(value.Value);
            }

            // Duration
            value = node.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_DURATION);
            if (TimeSpan.TryParse(value.Value, out var span))
            {
                lcEvent.Duration = span;
            }
            else
            {
                throw new InvalidTimeSpanException("Could not read the duration of the method.");
            }

            // Read the parameters
            var parameters = node.SelectSingleNode(LCMethodFactory.CONST_XPATH_PARAMETERS);
            var parameterList = parameters.SelectNodes(LCMethodFactory.CONST_XPATH_PARAMETER);

            // Create an array of expected parameters.
            var parameterArray = new object[parameterList.Count];
            var parameterNameArray = new string[parameterList.Count];
            var typeArray = new Type[parameterList.Count];


            // Use this sequentially to avoid ambiguity with iterations or out of order keying.
            for (var i = 0; i < parameterList.Count; i++)
            {
                var parameterNode = parameterList[i];

                var parameterValue = parameterNode.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_VALUE);
                var parameterType = parameterNode.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_TYPE);
                var parameterName = parameterNode.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_NAME);

                //
                // Create a parameter type, if it fails?! well...
                //
                Type constructedParameterType = null;
                try
                {
                    constructedParameterType = FindType(parameterType.Value);
                    //Type.GetType(parameterType.Value, true, true);
                }
                catch (FileNotFoundException fileEx)
                {
                    try
                    {
                        constructedParameterType = FindType(parameterType.Value);
                        if (constructedParameterType == null)
                        {
                            throw new Exception(
                                string.Format("Could not construct parameter for {0}.", parameterName.Value), fileEx);
                        }
                    }
                    catch (Exception newException)
                    {
                        constructedParameterType = null;
                        throw new Exception(
                            string.Format("Could not construct parameter for {0}.", parameterName.Value), newException);
                    }
                }
                catch (TypeLoadException ex)
                {
                    try
                    {
                        constructedParameterType = FindType(parameterType.Value);
                        if (constructedParameterType == null)
                        {
                            throw new Exception(
                                string.Format("Could not construct parameter for {0}.", parameterName.Value), ex);
                        }
                    }
                    catch (Exception newException)
                    {
                        constructedParameterType = null;
                        throw new Exception(
                            string.Format("Could not construct parameter for {0}.", parameterName.Value), newException);
                    }
                }
                catch (Exception ex)
                {
                    constructedParameterType = null;
                    throw new Exception(string.Format("Could not construct parameter for {0}.", parameterName.Value), ex);
                }

                // If it's an enumeration, then we'll have to treat it different from the rest of things
                if (constructedParameterType.IsEnum)
                {
                    parameterArray[i] = Enum.Parse(constructedParameterType, parameterValue.Value);
                    parameterNameArray[i] = parameterName.Value;
                    typeArray[i] = constructedParameterType;
                }
                else
                {
                    var typeFound = false;
                    // Advanced way of converting parameter arguments; silently swallowing exceptions
                    if (constructedParameterType.FullName != null)
                    {
                        try
                        {
                            var type = Type.GetType(constructedParameterType.FullName);
                            if (type == null)
                            {
                                throw new TypeLoadException();
                            }

                            if (!string.IsNullOrWhiteSpace(parameterValue.Value))
                            {
                                parameterArray[i] = Convert.ChangeType(parameterValue.Value, type);
                            }
                            else
                            {
                                if (type.IsValueType)
                                {
                                    parameterArray[i] = Activator.CreateInstance(type);
                                }
                                else if (type == typeof(string))
                                {
                                    parameterArray[i] = "";
                                }
                                else
                                {
                                    parameterArray[i] = null;
                                }
                            }

                            parameterNameArray[i] = parameterName.Value;
                            typeArray[i] = type;
                            typeFound = true;
                        }
                        catch
                        {
                            // Just swallow it
                        }
                    }

                    if (!typeFound)
                    {
                        // Poor mans way of converting these parameter arguments.  But it works, however, its
                        // constraining for us.
                        switch (constructedParameterType.FullName)
                        {
                            case "System.Double":
                                parameterArray[i] = Convert.ToDouble(parameterValue.Value);
                                parameterNameArray[i] = parameterName.Value;
                                typeArray[i] = typeof(double);
                                break;
                            case "System.String":
                                parameterArray[i] = parameterValue.Value;
                                parameterNameArray[i] = parameterName.Value;
                                typeArray[i] = typeof(string);
                                break;
                            default:
                                parameterArray[i] = null;
                                parameterNameArray[i] = parameterName.Value;
                                typeArray[i] = typeof(SampleData);
                                break;
                        }
                    }
                }
            }
            lcEvent.Parameters = parameterArray;
            lcEvent.ParameterNames = parameterNameArray;

            //
            // Device Initialization
            //
            value = node.SelectSingleNode(LCMethodFactory.CONST_XPATH_DEVICE);
            var attribute = value.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_NAME);
            var deviceName = attribute.Value;
            attribute = value.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_TYPE);
            var deviceTypeName = attribute.Value;

            var devicetype = FindType(deviceTypeName);
            var device = DeviceManager.Manager.FindDevice(deviceName, devicetype);
            if (device == null)
            {
                throw new DeviceNotFoundException("Could not find the device " + deviceName + ".", deviceName);
            }
            lcEvent.Device = device;

            //
            // Method Info for invoking the device's method.
            //
            value = node.SelectSingleNode(LCMethodFactory.CONST_XPATH_METHOD_INFO);
            attribute = value.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_NAME);
            var methodName = attribute.Value;
            MethodInfo method = null;
            try
            {
                //
                // There is no way to disambiguate overloaded methods...so we
                // have to see if the parameters match of the methods
                // that have the name provided.
                //
                var methods = devicetype.GetMethods();
                foreach (var info in methods)
                {
                    var parameterInfo = info.GetParameters();
                    var nonDefaultParamCount = parameterInfo.Count(x => !x.HasDefaultValue);
                    if (info.Name == methodName && (parameterInfo.Length == lcEvent.Parameters.Length || lcEvent.Parameters.Length == nonDefaultParamCount))
                    {
                        var i = 0;
                        var found = true;
                        foreach (var pinfo in parameterInfo)
                        {
                            //if (!pinfo.ParameterType.Equals(typeArray[i]))
                            //if (pinfo.ParameterType.Name != typeArray[i].Name && (!pinfo.HasDefaultValue || i >= nonDefaultParamCount))
                            if (!pinfo.ParameterType.IsAssignableFrom(typeArray[i]) && (!pinfo.HasDefaultValue || i >= nonDefaultParamCount))
                            {
                                found = false;
                                break;
                            }
                            i++;

                            if (i >= typeArray.Length)
                            {
                                break;
                            }
                        }

                        if (found)
                        {
                            if (lcEvent.Parameters.Length < parameterInfo.Length)
                            {
                                // Add in newer default parameters
                                var newTypes = new Type[parameterInfo.Length];
                                var newParamNames = new string[parameterInfo.Length];
                                var newParams = new object[parameterInfo.Length];
                                Array.Copy(typeArray, newTypes, typeArray.Length);
                                Array.Copy(lcEvent.ParameterNames, newParamNames, lcEvent.ParameterNames.Length);
                                Array.Copy(lcEvent.Parameters, newParams, lcEvent.Parameters.Length);

                                for (var j = typeArray.Length; j < parameterInfo.Length; j++)
                                {
                                    newTypes[j] = parameterInfo[j].ParameterType;
                                    newParamNames[j] = parameterInfo[j].Name;
                                    newParams[j] = parameterInfo[j].DefaultValue;
                                }

                                typeArray = newTypes;
                                lcEvent.ParameterNames = newParamNames;
                                lcEvent.Parameters = newParams;
                            }

                            method = info;
                            break;
                        }
                    }
                }

                if (method == null)
                {
                    throw new Exception($"Could not find a method with name '{methodName}' and {typeArray.Length} parameters of types '{string.Join(", ", typeArray.Select(x => x.FullName))}' (param names: '{string.Join(", ", lcEvent.ParameterNames)}').");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read the LC Method.  ", ex);
            }

            lcEvent.Method = method;

            // Get the method attributes for this method as well.
            // Otherwise this breaks specification, so it should be there. If the method was saved
            // then it should have also been persisted into the XML.  Otherwise...not a chance?
            try
            {
                var methodAttributes = method.GetCustomAttributes(false);
                foreach (var attr in methodAttributes)
                {
                    if (attr is LCMethodEventAttribute meth)
                    {
                        // Check all for a match
                        if (meth.Name.Equals(lcEvent.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            lcEvent.MethodAttribute = meth;
                            break;
                        }

                        // Default (if no name match) is the first LCMethodEventAttribute
                        if (lcEvent.MethodAttribute == null)
                        {
                            lcEvent.MethodAttribute = meth;
                        }
                    }
                }
            }
            catch (Exception exOld)
            {
                var ex = new Exception("Could not read the LC-method event for device " + deviceName, exOld);
                throw ex;
            }

            if (lcEvent.MethodAttribute != null)
            {
                // Set this value based on the method attribute
                lcEvent.HasDiscreteStates = lcEvent.MethodAttribute.HasDiscreteParameters;
            }

            return lcEvent;
        }

        /// <summary>
        /// Reads the method contained in the XML file path.
        /// </summary>
        /// <param name="filePath">Path of file that contains method.</param>
        /// <param name="errors"></param>
        /// <returns>Null if the path does not exist. New method object if successful.</returns>
        public LCMethod ReadMethod(string filePath, List<Exception> errors)
        {
            return ReadMethod(filePath, false, errors);
        }

        /// <summary>
        /// Reads the method from the filepath.
        /// </summary>
        /// <param name="filePath">Path of the method file to read.</param>
        /// <param name="readActuals">Flag indicating whether to read the actual event information (if it exists).</param>
        /// <param name="errors"></param>
        /// <returns>LC-Method read from the file.</returns>
        public LCMethod ReadMethod(string filePath, bool readActuals, List<Exception> errors)
        {
            if (File.Exists(filePath) == false)
                return null;

            //
            // Load the document,
            //     Catch XML errors and authorization errors.
            //     We have made sure the file exists.
            //
            var document = new XmlDocument();
            try
            {
                document.Load(filePath);
            }
            catch (XmlException ex)
            {
                throw new Exception("The LC-method file was corrupt.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("You do not have authorization to open the method file.",
                    ex);
            }
            var root = document.SelectSingleNode(LCMethodFactory.CONST_XPATH_METHOD);

            return ReadEventList(root, false, errors);
        }

        /// <summary>
        /// Reads the event list from the document provided.
        /// </summary>
        /// <param name="root">Root item in the document.</param>
        /// <param name="readActual">Flag indicating whether to read the actual data instead of the proposed data.</param>
        /// <param name="errors"></param>
        /// <returns>LC-Method containing all event information.</returns>
        private LCMethod ReadEventList(XmlNode root, bool readActual, List<Exception> errors)
        {
            LCMethod method = null;

            //
            // Get the name of the lc-method
            //

            try
            {
                var nameAttribute = root.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_NAME);
                method = new LCMethod {
                    Name = nameAttribute.Value
                };

                nameAttribute = root.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_IS_SPECIAL);
                if (nameAttribute != null)
                {
                    method.IsSpecialMethod = bool.Parse(nameAttribute.Value);
                    if (method.IsSpecialMethod)
                    {
                        method.Column = -1;
                    }
                    else
                    {
                        nameAttribute = root.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_COLUMN_DATA);
                        method.Column = int.Parse(nameAttribute.Value);
                    }
                    nameAttribute = root.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_ALLOW_POST_OVERLAP);
                    method.AllowPostOverlap = bool.Parse(nameAttribute.Value);

                    nameAttribute = root.Attributes.GetNamedItem(LCMethodFactory.CONST_XPATH_ALLOW_PRE_OVERLAP);
                    method.AllowPreOverlap = bool.Parse(nameAttribute.Value);
                }
            }
            catch
            {
                return null;
            }
            //
            // Now get the list and parse each item
            //
            var eventListNode = root.SelectNodes(LCMethodFactory.CONST_XPATH_EVENTS);

            //
            // If the user really wanted to read the actual data, then read it instead
            // of reading the proposed method events. Actual data is stored after
            // a run in the XML file if requested.  This provides information about
            // the performance of the software and control.
            //
            if (readActual)
            {
                //
                // Make sure we have some kind of events
                //
                eventListNode = root.SelectNodes(LCMethodFactory.CONST_XPATH_ACTUAL_METHOD);
                if (eventListNode == null || eventListNode.Count < 1)
                    return null;
            }

            if (eventListNode == null)
                return method;

            var i = 0;
            foreach (XmlNode node in eventListNode)
            {
                i++;
                try
                {
                    var lcEvent = ReadEventNode(node);
                    method.Events.Add(lcEvent);
                }
                catch (DeviceNotFoundException ex)
                {
                    var error =
                        string.Format("The Device \"{0}\" was missing from the hardware manager at event {1}.",
                                      ex.DeviceName,
                                      i);
                    var newException = new Exception(error, ex);
                    ApplicationLogger.LogError(1, error, ex);
                    errors.Add(newException);
                }
            }

            return method;
        }
    }
}