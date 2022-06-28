using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using LcmsNet.Data;
using LcmsNet.Devices;
using LcmsNet.Method;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.IO
{
    /// <summary>
    /// Class that can handle reading/writing a method from/to a file path.
    /// </summary>
    public class LCMethodXmlFile
    {
        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_IS_SPECIAL = "IsSpecial";

        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_ALLOW_PRE_OVERLAP = "AllowPreOverlap";

        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_ALLOW_POST_OVERLAP = "AllowPostOverlap";

        /// <summary>
        /// X-path string for a LC-Method Column list (integers).
        /// </summary>
        public const string CONST_XPATH_COLUMN_DATA = "Column";

        /// <summary>
        /// X-path string for a LC-Method Column list (integers).
        /// </summary>
        public const string CONST_XPATH_COLUMN_DATA_NUMBER = "Number";

        /// <summary>
        /// LC Methods folder name.
        /// </summary>
        public const string CONST_LC_METHOD_FOLDER = @"C:\LCMSNet\LCMethods";

        /// <summary>
        /// Deterministic or non-deterministic events.
        /// </summary>
        public const string CONST_IS_EVENT_INDETERMINANT = "Indeterminant";

        /// <summary>
        /// LC Methods folder name.
        /// </summary>
        public const string CONST_LC_METHOD_EXTENSION = ".xml";

        /// <summary>
        /// X-Path string for lc-events node.
        /// </summary>
        public const string CONST_XPATH_EVENTS = "LCEvent";

        /// <summary>
        /// X-Path string for lc-events node.
        /// </summary>
        public const string CONST_XPATH_ACTUAL_EVENTS = "LCEventActual";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_NAME = "name";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_DEVICE = "Device";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_DURATION = "Duration";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_HAS_DISCREET_STATES = "HasDiscreetStates";

        /// <summary>
        /// X-Path string for if a parameter is sample specific for an LC-Event XML Node.
        /// </summary>
        public const string CONST_IS_SAMPLE_SPECIFIC = "IsSampleSpecific";

        /// <summary>
        /// X-Path string for root lc-method node.
        /// </summary>
        public const string CONST_XPATH_METHOD = "LCMethod";

        /// <summary>
        /// X-Path string for root lc-method node.
        /// </summary>
        public const string CONST_XPATH_ACTUAL_METHOD = "MethodPerformanceData";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_METHOD_INFO = "MethodInfo";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_OPTIMIZE_WITH = "OptimizeWith";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_PARAMETERS = "Parameters";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_PARAMETER = "Parameter";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_START = "StartTime";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_TYPE = "type";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_VALUE = "value";

        private const string CONST_METHOD_EXTENSION = "*.xml";

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

            LCMethod method;
            try
            {
                method = ReadMethod(filePath, errors);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not load method from " + filePath, ex);
                throw;
            }

            // If the method failed to load...then...return false
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
            // Otherwise, add the method so it can be registered with appropriate objects
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

        /// <summary>
        /// Reads the method contained in the XML file path.
        /// </summary>
        /// <param name="filePath">Path of file that contains method.</param>
        /// <param name="errors"></param>
        /// <returns>Null if the path does not exist. New method object if successful.</returns>
        public static LCMethod ReadMethod(string filePath, List<Exception> errors)
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
        public static LCMethod ReadMethod(string filePath, bool readActuals, List<Exception> errors)
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
            var root = document.SelectSingleNode(CONST_XPATH_METHOD);

            return ReadEventList(root, false, errors);
        }

        /// <summary>
        /// Finds the type within the loaded assemblies.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Type FindType(string name)
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
        private static LCEvent ReadEventNode(XmlNode node)
        {
            var lcEvent = new LCEvent();

            // Read the name
            if (node.Attributes != null)
            {
                var nameAttribute = node.Attributes.GetNamedItem(CONST_XPATH_NAME);
                lcEvent.Name = nameAttribute.Value;
            }

            // Construct flags
            var value = node.Attributes.GetNamedItem(CONST_XPATH_OPTIMIZE_WITH);
            lcEvent.OptimizeWith = Convert.ToBoolean(value.Value);
            value = node.Attributes.GetNamedItem(CONST_XPATH_HAS_DISCREET_STATES);
            if (value != null)
            {
                lcEvent.HasDiscreteStates = Convert.ToBoolean(value.Value);
            }

            try
            {
                value = node.Attributes.GetNamedItem(CONST_IS_EVENT_INDETERMINANT);
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
            value = node.Attributes.GetNamedItem(CONST_XPATH_START);
            if (TimeSpan.TryParse(value.Value, out var startSpan))
            {
                lcEvent.Start = DateTime.MinValue.Add(startSpan);
            }
            else
            {
                lcEvent.Start = Convert.ToDateTime(value.Value);
            }

            // Duration
            value = node.Attributes.GetNamedItem(CONST_XPATH_DURATION);
            if (TimeSpan.TryParse(value.Value, out var span))
            {
                lcEvent.Duration = span;
            }
            else
            {
                throw new InvalidTimeSpanException("Could not read the duration of the method.");
            }

            // Read the parameters
            var parameters = node.SelectSingleNode(CONST_XPATH_PARAMETERS);
            var parameterList = parameters.SelectNodes(CONST_XPATH_PARAMETER);

            // Create an array of expected parameters.
            var parameterArray = new object[parameterList.Count];
            var parameterNameArray = new string[parameterList.Count];
            var typeArray = new Type[parameterList.Count];


            // Use this sequentially to avoid ambiguity with iterations or out of order keying.
            for (var i = 0; i < parameterList.Count; i++)
            {
                var parameterNode = parameterList[i];

                var parameterValue = parameterNode.Attributes.GetNamedItem(CONST_XPATH_VALUE);
                var parameterType = parameterNode.Attributes.GetNamedItem(CONST_XPATH_TYPE);
                var parameterName = parameterNode.Attributes.GetNamedItem(CONST_XPATH_NAME);

                // Create a parameter type, if it fails?! well...
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

            // Device Initialization
            value = node.SelectSingleNode(CONST_XPATH_DEVICE);
            var attribute = value.Attributes.GetNamedItem(CONST_XPATH_NAME);
            var deviceName = attribute.Value;
            attribute = value.Attributes.GetNamedItem(CONST_XPATH_TYPE);
            var deviceTypeName = attribute.Value;

            var devicetype = FindType(deviceTypeName);
            var device = DeviceManager.Manager.FindDevice(deviceName, devicetype);
            if (device == null)
            {
                throw new DeviceNotFoundException("Could not find the device " + deviceName + ".", deviceName);
            }
            lcEvent.Device = device;

            // Method Info for invoking the device's method.
            value = node.SelectSingleNode(CONST_XPATH_METHOD_INFO);
            attribute = value.Attributes.GetNamedItem(CONST_XPATH_NAME);
            var methodName = attribute.Value;
            try
            {
                // Load list of methods with matching names, then check each matching method for attribute match and parameter match (but with the attribute match, allow mismatched parameter upgrades)
                var methods = devicetype.GetMethods();
                var matchedMethods = methods.Where(x => x.Name == methodName).ToList();

                // Load all matching (by name) method attributes, and give them priority, also store all non-matching method attributes with the associated method, for backup behavior
                var matchedAttributes = new List<KeyValuePair<LCMethodEventAttribute, MethodInfo>>();
                var unmatchedAttributes = new List<KeyValuePair<LCMethodEventAttribute, MethodInfo>>();
                foreach (var info in matchedMethods)
                {
                    // Get the method attributes for this method as well.
                    // Otherwise this breaks specification, so it should be there. If the method was saved
                    // then it should have also been persisted into the XML.  Otherwise...not a chance?
                    LCMethodEventAttribute methodAttribute = null;
                    var matched = false;
                    try
                    {
                        var methodAttributes = info.GetCustomAttributes(false);
                        foreach (var attr in methodAttributes)
                        {
                            if (attr is LCMethodEventAttribute meth)
                            {
                                // Check all for a match
                                if (meth.Name.Equals(lcEvent.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    methodAttribute = meth;
                                    matched = true;
                                    break;
                                }

                                // Default (if no name match) is the first LCMethodEventAttribute
                                if (methodAttribute == null)
                                {
                                    methodAttribute = meth;
                                }
                            }
                        }
                    }
                    catch (Exception exOld)
                    {
                        var ex = new Exception("Could not read the LC-method event for device " + deviceName, exOld);
                        throw ex;
                    }

                    if (matched)
                    {
                        matchedAttributes.Add(new KeyValuePair<LCMethodEventAttribute, MethodInfo>(methodAttribute, info));
                    }
                    else
                    {
                        unmatchedAttributes.Add(new KeyValuePair<LCMethodEventAttribute, MethodInfo>(methodAttribute, info));
                    }
                }

                // If only one matched method event attribute, use it (regardless of parameters)
                // If more than one matched method event attribute, check parameters (this shouldn't happen, but there is nothing in the code enforcing unique method event attribute names per class)
                // If no matched method event attribute, then check all unmatched based on parameters.
                if (matchedAttributes.Count == 1)
                {
                    lcEvent.MethodAttribute = matchedAttributes[0].Key;
                    lcEvent.Method = matchedAttributes[0].Value;

                    // Update/fix parameters...
                    var parameterInfo = lcEvent.Method.GetParameters();
                    if (lcEvent.Parameters.Length != parameterInfo.Length)
                    {
                        // Add in newer default parameters, remove now-missing parameters
                        var newTypes = new Type[parameterInfo.Length];
                        var newParamNames = new string[parameterInfo.Length];
                        var newParams = new object[parameterInfo.Length];

                        for (var i = 0; i < parameterInfo.Length; i++)
                        {
                            var paramInfo = parameterInfo[i];
                            newTypes[i] = paramInfo.ParameterType;
                            newParamNames[i] = paramInfo.Name;
                            if (paramInfo.HasDefaultValue)
                            {
                                newParams[i] = paramInfo.DefaultValue;
                            }
                            else if (paramInfo.ParameterType.IsValueType)
                            {
                                newParams[i] = Activator.CreateInstance(paramInfo.ParameterType);
                            }
                            else
                            {
                                newParams[i] = null;
                            }

                            for (var j = 0; j < typeArray.Length; j++)
                            {
                                if (typeArray[j] == paramInfo.ParameterType && lcEvent.ParameterNames[j] == paramInfo.Name)
                                {
                                    newParams[i] = lcEvent.Parameters[j];
                                    break;
                                }
                            }
                        }

                        typeArray = newTypes;
                        lcEvent.ParameterNames = newParamNames;
                        lcEvent.Parameters = newParams;
                    }
                }
                else
                {
                    var methodList = matchedAttributes;
                    if (methodList.Count == 0)
                    {
                        methodList = unmatchedAttributes;
                    }

                    // There is no way to disambiguate overloaded methods...so we have to see if the parameters match of the methods that have the name provided.
                    foreach (var methodEvent in methodList)
                    {
                        var info = methodEvent.Value;
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

                                lcEvent.Method = info;
                                lcEvent.MethodAttribute = methodEvent.Key;
                                break;
                            }
                        }
                    }

                    if (lcEvent.Method == null)
                    {
                        throw new Exception($"Could not find a method with name '{methodName}' and {typeArray.Length} parameters of types '{string.Join(", ", typeArray.Select(x => x.FullName))}' (param names: '{string.Join(", ", lcEvent.ParameterNames)}').");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read the LC Method.  ", ex);
            }

            if (lcEvent.MethodAttribute != null)
            {
                // Set this value based on the method attribute
                lcEvent.HasDiscreteStates = lcEvent.MethodAttribute.HasDiscreteParameters;
            }

            return lcEvent;
        }

        /// <summary>
        /// Reads the event list from the document provided.
        /// </summary>
        /// <param name="root">Root item in the document.</param>
        /// <param name="readActual">Flag indicating whether to read the actual data instead of the proposed data.</param>
        /// <param name="errors"></param>
        /// <returns>LC-Method containing all event information.</returns>
        private static LCMethod ReadEventList(XmlNode root, bool readActual, List<Exception> errors)
        {
            LCMethod method = null;

            // Get the name of the lc-method

            try
            {
                var nameAttribute = root.Attributes.GetNamedItem(CONST_XPATH_NAME);
                method = new LCMethod
                {
                    Name = nameAttribute.Value
                };

                nameAttribute = root.Attributes.GetNamedItem(CONST_XPATH_IS_SPECIAL);
                if (nameAttribute != null)
                {
                    method.IsSpecialMethod = bool.Parse(nameAttribute.Value);
                    if (method.IsSpecialMethod)
                    {
                        method.Column = -1;
                    }
                    else
                    {
                        nameAttribute = root.Attributes.GetNamedItem(CONST_XPATH_COLUMN_DATA);
                        method.Column = int.Parse(nameAttribute.Value);
                    }
                    nameAttribute = root.Attributes.GetNamedItem(CONST_XPATH_ALLOW_POST_OVERLAP);
                    method.AllowPostOverlap = bool.Parse(nameAttribute.Value);

                    nameAttribute = root.Attributes.GetNamedItem(CONST_XPATH_ALLOW_PRE_OVERLAP);
                    method.AllowPreOverlap = bool.Parse(nameAttribute.Value);
                }
            }
            catch
            {
                return null;
            }

            // Now get the list and parse each item
            var eventListNode = root.SelectNodes(CONST_XPATH_EVENTS);

            // If the user really wanted to read the actual data, then read it instead
            // of reading the proposed method events. Actual data is stored after
            // a run in the XML file if requested.  This provides information about
            // the performance of the software and control.
            if (readActual)
            {
                //
                // Make sure we have some kind of events
                //
                eventListNode = root.SelectNodes(CONST_XPATH_ACTUAL_METHOD);
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

        /// <summary>
        /// Writes the LC-Event from the specified node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="eventRoot"></param>
        /// <param name="lcEvent"></param>
        /// <param name="isSampleReport">Supply true to write extra information that isn't needed for method loading</param>
        /// <param name="previousEventTimeTotal">Supply true to write the specific start time (when a sample ran) rather than just a cumulative start time</param>
        private static void WriteEventNode(XmlDocument document, XmlElement eventRoot, LCEvent lcEvent, bool isSampleReport, bool reportStartTime, TimeSpan previousEventTimeTotal)
        {
            // Set all of the event attributes
            eventRoot.SetAttribute(CONST_XPATH_NAME, lcEvent.Name);

            // write timespan for stored methods, write DateTime for actual/complete/incomplete methods
            var startTime = lcEvent.Start.ToString(CultureInfo.InvariantCulture);
            if (!reportStartTime)
            {
                startTime = previousEventTimeTotal.ToString();
            }
            eventRoot.SetAttribute(CONST_XPATH_START, startTime);

            eventRoot.SetAttribute(CONST_XPATH_DURATION, lcEvent.Duration.ToString());
            if (isSampleReport)
            {
                // This is set from event details, and can be re-set by "building" the method after loading
                eventRoot.SetAttribute(CONST_XPATH_HAS_DISCREET_STATES,
                    lcEvent.HasDiscreteStates.ToString());
            }

            eventRoot.SetAttribute(CONST_XPATH_OPTIMIZE_WITH, lcEvent.OptimizeWith.ToString());

            // Store the device data
            var device = document.CreateElement(CONST_XPATH_DEVICE);
            device.SetAttribute(CONST_XPATH_NAME, lcEvent.Device.Name);

            // Alternatively use .AssemblyQualifiedName;
            device.SetAttribute(CONST_XPATH_TYPE, lcEvent.Device.GetType().FullName);

            // Store the method name
            var methodInfo = document.CreateElement(CONST_XPATH_METHOD_INFO);
            methodInfo.SetAttribute(CONST_XPATH_NAME, lcEvent.Method.Name);

            // Store the parameter information
            var parameters = document.CreateElement(CONST_XPATH_PARAMETERS);
            for (var i = 0; i < lcEvent.Parameters.Length; i++)
            {
                var parameter = lcEvent.Parameters[i];
                var parameterName = lcEvent.ParameterNames[i];

                //TODO: Fix this sample thing, coupling
                if (parameter == null && lcEvent.MethodAttribute.SampleParameterIndex != i)
                {
                    throw new NullReferenceException(
                        string.Format("The parameter {0} was not set for LC Event {1}.  Device: {2}.",
                            parameterName,
                            lcEvent.Name,
                            lcEvent.Device.Name,
                            lcEvent.Method.Name));
                }

                var parameterInfo = document.CreateElement(CONST_XPATH_PARAMETER);
                parameterInfo.SetAttribute(CONST_XPATH_NAME, parameterName);

                var sampleSpecific = false;
                string value;
                string type;

                // Here we write if the parameter is a sample data object.
                if (lcEvent.MethodAttribute.SampleParameterIndex == i)
                {
                    sampleSpecific = true;

                    // Alternatively use .AssemblyQualifiedName;
                    type = typeof(SampleData).FullName;
                    value = "";
                }
                else
                {
                    value = Convert.ToString(parameter);

                    // Alternatively use .AssemblyQualifiedName;
                    if (parameter != null)
                        type = parameter.GetType().FullName;
                    else
                        type = "";
                }

                parameterInfo.SetAttribute(CONST_IS_EVENT_INDETERMINANT,
                    lcEvent.IsIndeterminant.ToString());
                parameterInfo.SetAttribute(CONST_XPATH_NAME, parameterName);
                parameterInfo.SetAttribute(CONST_XPATH_TYPE, type);
                parameterInfo.SetAttribute(CONST_XPATH_VALUE, value);
                parameterInfo.SetAttribute(CONST_IS_SAMPLE_SPECIFIC, sampleSpecific.ToString());

                // Add it to the list of parameters
                parameters.AppendChild(parameterInfo);
            }

            // Add all of the nodes
            eventRoot.AppendChild(device);
            eventRoot.AppendChild(methodInfo);
            eventRoot.AppendChild(parameters);

        }

        /// <summary>
        /// Writes the method to the XML file path.
        /// </summary>
        /// <param name="filePath">Path of file that contains method.</param>
        /// <param name="method"></param>
        /// <returns>Null if the path does not exist. New method object if successful.</returns>
        public static bool WriteMethod(string filePath, LCMethod method)
        {
            // Don't write if the method is null.
            if (method == null)
                return false;

            var document = new XmlDocument();
            var isSampleReport = method.ActualEvents.Count > 0;

            // Method Name and Flag if it is "special"
            var rootElement = document.CreateElement(CONST_XPATH_METHOD);
            rootElement.SetAttribute(CONST_XPATH_NAME, method.Name);
            rootElement.SetAttribute(CONST_XPATH_IS_SPECIAL, method.IsSpecialMethod.ToString());
            rootElement.SetAttribute(CONST_XPATH_ALLOW_POST_OVERLAP,
                method.AllowPostOverlap.ToString());
            rootElement.SetAttribute(CONST_XPATH_ALLOW_PRE_OVERLAP,
                method.AllowPreOverlap.ToString());
            rootElement.SetAttribute(CONST_XPATH_COLUMN_DATA, method.Column.ToString());

            var timeTotal = new TimeSpan();
            // Then construct the events.
            foreach (var lcEvent in method.Events)
            {
                var eventElement = document.CreateElement(CONST_XPATH_EVENTS);
                WriteEventNode(document, eventElement, lcEvent, isSampleReport, false, timeTotal);
                timeTotal = timeTotal.Add(lcEvent.Duration);
                rootElement.AppendChild(eventElement);
            }

            // Dump the actual events
            var rootActualElement = document.CreateElement(CONST_XPATH_ACTUAL_METHOD);
            rootActualElement.SetAttribute(CONST_XPATH_NAME, method.Name);

            if (method.ActualEvents.Count > 0)
            {
                timeTotal = new TimeSpan();
                // Then construct the events.
                foreach (var lcActualEvent in method.ActualEvents)
                {
                    var actualEventElement = document.CreateElement(CONST_XPATH_EVENTS);
                    WriteEventNode(document, actualEventElement, lcActualEvent, isSampleReport, true, timeTotal);
                    timeTotal = timeTotal.Add(lcActualEvent.Duration);
                    rootActualElement.AppendChild(actualEventElement);
                }
                rootElement.AppendChild(rootActualElement); // How the method actually ran.
            }

            // Finally, write the method out
            try
            {
                document.AppendChild(rootElement); // How the method is supposed to run.
                document.Save(filePath);
            }
            catch (XmlException ex)
            {
                throw new Exception("The configuration file was corrupt.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("You do not have authorization to open the method file.",
                    ex);
            }

            return true;
        }
    }
}
