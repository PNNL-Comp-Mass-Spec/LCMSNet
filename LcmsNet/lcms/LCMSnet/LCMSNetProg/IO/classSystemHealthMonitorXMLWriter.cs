//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using LcmsNetDataClasses.Logging;
//using LcmsNetDataClasses.Devices;
//using LcmsNetDataClasses;
//using System.Xml;
//using System.IO;

//namespace LcmsNet.IO
//{
//    public class classSystemHealthMonitorXMLWriter: IDeviceHealthWriter
//    {
//        /// <summary>
//        /// Values for a given device.
//        /// </summary>
//        private Dictionary<string, string> mdict_values;

//        public classSystemHealthMonitorXMLWriter()
//        {
//            mdict_values = new Dictionary<string, string>();
//        }

//        #region IDeviceHealthWriter Members
//        /// <summary>
//        /// Writes a value from the device into the internal dictionary.
//        /// </summary>
//        /// <param name="deviceName">Device that is writing the value.</param>
//        /// <param name="setting">Setting to persist</param>
//        /// <param name="value">Value to persist for that setting.</param>
//        public void WriteValue(string deviceName, string setting, string value)
//        {
//            if (!mdict_values.ContainsKey(setting))
//            {
//                mdict_values.Add(setting, value);
//            }
//            mdict_values[setting] = value;
//        }
//        #endregion

//        /// <summary>
//        /// writes a full
//        /// </summary>
//        /// <param name="document"></param>
//        /// <param name="root"></param>
//        private void WriteProperties(XmlDocument document, XmlElement root)
//        {
//            XmlElement propertiesElement = document.CreateElement("assembly");
//            propertiesElement.SetAttribute("name", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME));
//            propertiesElement.SetAttribute("class", "LC-Cart");

//            XmlElement statusProperties = document.CreateElement("property");
//            statusProperties.SetAttribute("lastUpdate", DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).ToString("MM-dd-yyyy hh:mm:ss"));
//            propertiesElement.AppendChild(statusProperties);

//            root.AppendChild(propertiesElement);
//        }
//        /// <summary>
//        /// Writes a single device's status.
//        /// </summary>
//        /// <param name="document"></param>
//        /// <param name="root"></param>
//        /// <param name="device"></param>
//        private void WriteDeviceProperties(XmlDocument document, XmlElement root, IDevice device)
//        {

//            classMonitoringComponent deviceAsComponent = device.GetData();

//            if (deviceAsComponent == null)
//                return;

//            XmlElement component            = document.CreateElement("component");
//            component.SetAttribute("type", deviceAsComponent.Type);
//            component.SetAttribute("name", deviceAsComponent.Name);
//            component.SetAttribute("model", deviceAsComponent.Model);
//            XmlElement status               = document.CreateElement("status");
//            status.InnerText                = deviceAsComponent.Status;
//            XmlElement message              = document.CreateElement("message");
//            message.InnerText               = deviceAsComponent.Message;
//            XmlElement error                = document.CreateElement("hasError");
//            error.InnerText                 = "";
//            XmlElement diagnostics          = document.CreateElement("diagnosticData");
//            XmlElement measurements         = document.CreateElement("measurements");
//            XmlElement plots                = document.CreateElement("plots");

//            foreach (classMonitoringMeasurementScalar measurement in deviceAsComponent.MeasurementData)
//            {
//                XmlElement measurementNode = document.CreateElement("measurement");
//                measurementNode.SetAttribute("type", measurement.Type);
//                measurementNode.SetAttribute("name", measurement.Name);
//                measurementNode.SetAttribute("units", measurement.Units);
//                measurementNode.SetAttribute("description", measurement.Description);
//                measurementNode.InnerText = measurement.Value;
//                measurements.AppendChild(measurementNode);
//            }

//            foreach (classMonitoringMeasurementPlot plot in deviceAsComponent.PlotData)
//            {
//                XmlElement plotNode = document.CreateElement("plot");
//                plotNode.SetAttribute("type",  plot.Type);
//                plotNode.SetAttribute("name", plot.Name);
//                plotNode.SetAttribute("xUnits", plot.XUnits);
//                plotNode.SetAttribute("yUnits", plot.YUnits);
//                plotNode.SetAttribute("description", plot.Description);
//                int N = Math.Max(0, Math.Min(plot.YValues.Count, plot.XValues.Count));

//                StringBuilder builder = new StringBuilder();
//                for (int i = 0; i < N; i++)
//                {
//                    builder.Append(string.Format("{0},{1}:", plot.XValues[i], plot.YValues[i]));
//                }
//                plotNode.InnerText = builder.ToString();
//                measurements.AppendChild(plotNode);
//            }

//            diagnostics.AppendChild(measurements);
//            diagnostics.AppendChild(plots);
//            component.AppendChild(status);
//            component.AppendChild(message);
//            component.AppendChild(error);
//            component.AppendChild(diagnostics);
//            root.AppendChild(component);
//        }
//        /// <summary>
//        /// Writes the devices health status to the file path provided.
//        /// </summary>
//        /// <param name="devices"></param>
//        /// <param name="path"></param>
//        public void WriteDevices(List<IDevice> devices, string path)
//        {
//            using (TextWriter textWriter = File.CreateText(path))
//            {
//                using (XmlTextWriter writer = new XmlTextWriter(textWriter))
//                {
//                    writer.Formatting       = Formatting.Indented;
//                    XmlDocument document    = new XmlDocument();
//                    XmlElement  root        = document.CreateElement("assemblies");

//                    // writes the header properties.
//                    WriteProperties(document, root);


//                    XmlElement propertiesElement = document.CreateElement("components");
//                    // writes each device.
//                    foreach (IDevice device in devices)
//                    {
//                        if (device.DeviceType == enumDeviceType.Component)
//                        {
//                            WriteDeviceProperties(document, propertiesElement, device);
//                        }
//                    }
//                    root.AppendChild(propertiesElement);
//                    document.AppendChild(root);
//                    document.Save(writer);
//                }
//            }
//        }
//    }
//}

