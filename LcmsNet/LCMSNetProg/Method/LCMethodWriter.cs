using System;
using System.Globalization;
using System.Xml;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that can handle reading a method from a file path.
    /// </summary>
    public class LCMethodWriter
    {
        #region Methods

        /// <summary>
        /// Writes the LC-Event from the specified node.
        /// </summary>
        private void WriteEventNode(XmlDocument document, XmlElement eventRoot, LCEvent lcEvent)
        {

            //
            // Set all of the event attributes
            //
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, lcEvent.Name);
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_START, lcEvent.Start.ToString(CultureInfo.InvariantCulture));
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_HOLD_TIME, lcEvent.HoldTime.ToString());
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_DURATION, lcEvent.Duration.ToString());
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_HAS_DISCREET_STATES,
                lcEvent.HasDiscreteStates.ToString());
            eventRoot.SetAttribute(LCMethodFactory.CONST_XPATH_OPTIMIZE_WITH, lcEvent.OptimizeWith.ToString());

            //
            // Store the device data
            //
            var device = document.CreateElement(LCMethodFactory.CONST_XPATH_DEVICE);
            device.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, lcEvent.Device.Name);

            // Alternatively use .AssemblyQualifiedName;
            device.SetAttribute(LCMethodFactory.CONST_XPATH_TYPE, lcEvent.Device.GetType().FullName);

            //
            // Store the method name
            //
            var methodInfo = document.CreateElement(LCMethodFactory.CONST_XPATH_METHOD_INFO);
            methodInfo.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, lcEvent.Method.Name);

            //
            // Store the parameter information
            //
            var parameters = document.CreateElement(LCMethodFactory.CONST_XPATH_PARAMETERS);
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

                var parameterInfo = document.CreateElement(LCMethodFactory.CONST_XPATH_PARAMETER);
                parameterInfo.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, parameterName);

                var sampleSpecific = false;
                string value;
                string type;
                //
                // Here we write if the parameter is a sample data object.
                //
                if (lcEvent.MethodAttribute.SampleParameterIndex == i)
                {
                    sampleSpecific = true;

                    // Alternatively use .AssemblyQualifiedName;
                    type = typeof (SampleData).FullName;
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

                parameterInfo.SetAttribute(LCMethodFactory.CONST_IS_EVENT_INDETERMINANT,
                    lcEvent.IsIndeterminant.ToString());
                parameterInfo.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, parameterName);
                parameterInfo.SetAttribute(LCMethodFactory.CONST_XPATH_TYPE, type);
                parameterInfo.SetAttribute(LCMethodFactory.CONST_XPATH_VALUE, value);
                parameterInfo.SetAttribute(LCMethodFactory.CONST_IS_SAMPLE_SPECIFIC, sampleSpecific.ToString());
                //
                // Add it to the list of parameters
                //
                parameters.AppendChild(parameterInfo);
            }

            //
            // Add all of the nodes
            //
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
        public bool WriteMethod(string filePath, LCMethod method)
        {
            // Don't write if the method is null.
            if (method == null)
                return false;

            var document = new XmlDocument();

            // Method Name and Flag if it is "special"
            var rootElement = document.CreateElement(LCMethodFactory.CONST_XPATH_METHOD);
            rootElement.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, method.Name);
            rootElement.SetAttribute(LCMethodFactory.CONST_XPATH_IS_SPECIAL, method.IsSpecialMethod.ToString());
            rootElement.SetAttribute(LCMethodFactory.CONST_XPATH_ALLOW_POST_OVERLAP,
                method.AllowPostOverlap.ToString());
            rootElement.SetAttribute(LCMethodFactory.CONST_XPATH_ALLOW_PRE_OVERLAP,
                method.AllowPreOverlap.ToString());
            rootElement.SetAttribute(LCMethodFactory.CONST_XPATH_COLUMN_DATA, method.Column.ToString());


            //
            // Then construct the events.
            //
            foreach (var lcEvent in method.Events)
            {
                var eventElement = document.CreateElement(LCMethodFactory.CONST_XPATH_EVENTS);
                WriteEventNode(document, eventElement, lcEvent);
                rootElement.AppendChild(eventElement);
            }


            //
            // Dump the actual events
            //
            var rootActualElement = document.CreateElement(LCMethodFactory.CONST_XPATH_ACTUAL_METHOD);
            rootActualElement.SetAttribute(LCMethodFactory.CONST_XPATH_NAME, method.Name);

            if (method.ActualEvents.Count > 0)
            {
                //
                // Then construct the events.
                //
                foreach (var lcActualEvent in method.ActualEvents)
                {
                    var actualEventElement = document.CreateElement(LCMethodFactory.CONST_XPATH_EVENTS);
                    WriteEventNode(document, actualEventElement, lcActualEvent);
                    rootActualElement.AppendChild(actualEventElement);
                }
                rootElement.AppendChild(rootActualElement); // How the method actually ran.
            }

            //
            // Finally, write the method out
            //
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

        #endregion
    }
}