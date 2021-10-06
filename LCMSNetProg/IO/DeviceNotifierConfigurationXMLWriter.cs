using System;
using System.Xml;
using LcmsNet.Notification;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Concrete class that writes the notification system to XML.
    /// </summary>
    public class DeviceNotifierConfigurationXMLWriter
    {
        /// <summary>
        /// Writes the configuration to file.
        /// </summary>
        /// <param name="path">Path to write configuration to.</param>
        /// <param name="configuration"></param>
        public void WriteConfiguration(string path, NotificationConfiguration configuration)
        {
            var document = new XmlDocument();
            var rootElement = document.CreateElement("Devices");

            var notifierSetting = document.CreateElement("SystemSettings");
            notifierSetting.SetAttribute("Ignore", configuration.IgnoreNotifications.ToString());
            rootElement.AppendChild(notifierSetting);

            foreach (var device in configuration)
            {
                var deviceElement = document.CreateElement("Device");
                deviceElement.SetAttribute("name", device.Name);

                foreach (var setting in configuration.GetDeviceSettings(device))
                {
                    var notifyElement = document.CreateElement("Notification");
                    notifyElement.SetAttribute("name", setting.Name);
                    notifyElement.SetAttribute("action", setting.Action.ToString());

                    var conditions = setting.GetConditions();
                    notifyElement.SetAttribute("type", conditions.Name);

                    var conditionElement = document.CreateElement("Conditions");
                    foreach (var condition in conditions.Conditions.Keys)
                    {
                        conditionElement.SetAttribute(condition, conditions.Conditions[condition].ToString());
                    }
                    notifyElement.AppendChild(conditionElement);

                    var methodName = "";
                    if (setting.Method != null)
                    {
                        methodName = setting.Method.Name;
                    }
                    notifyElement.SetAttribute("method", methodName);
                    deviceElement.AppendChild(notifyElement);
                }
                rootElement.AppendChild(deviceElement);
            }

            try
            {
                document.AppendChild(rootElement);
                document.Save(path);
            }
            catch (XmlException ex)
            {
                throw new Exception("The configuration file was corrupt.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("You do not have authorization to save the notifications file.",
                    ex);
            }
        }
    }
}