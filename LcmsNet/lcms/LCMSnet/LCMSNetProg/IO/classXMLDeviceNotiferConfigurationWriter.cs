using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using LcmsNet.Notification;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Concrete class that writes the notifcation system to XML.
    /// </summary>
    public class classXMLDeviceNotificationConfigurationWriter 
    {      
        /// <summary>
        /// Writes the configuration to file.
        /// </summary>
        /// <param name="path">Path to write configuration to.</param>
        /// <param name="configuration"></param>
        public void WriteConfiguration(string path, NotificationConfiguration configuration)
        {   
            XmlDocument document                = new XmlDocument();            
            XmlElement  rootElement             = document.CreateElement("Devices");

            XmlElement notiferSetting = document.CreateElement("SystemSettings");
            notiferSetting.SetAttribute("Ignore", configuration.IgnoreNotifications.ToString());
            rootElement.AppendChild(notiferSetting);

            foreach(INotifier device in configuration)
            {           
                XmlElement deviceElement            =  document.CreateElement("Device");
                deviceElement.SetAttribute("name", device.Name);

                foreach(NotificationSetting setting in configuration.GetDeviceSettings(device))
                {                
                    XmlElement notifyElement = document.CreateElement("Notification");
                    notifyElement.SetAttribute("name",      setting.Name);
                    notifyElement.SetAttribute("action",    setting.Action.ToString());

                    NotificationConditionNode conditions = setting.GetConditions();
                    notifyElement.SetAttribute("type", conditions.Name);

                    XmlElement conditionElement = document.CreateElement("Conditions");
                    foreach (string condition in conditions.Conditions.Keys)
                    {                        
                        conditionElement.SetAttribute(condition, conditions.Conditions[condition].ToString());                        
                    }
                    notifyElement.AppendChild(conditionElement);

                    string methodName       = "";
                    if (setting.Method != null)
                    {
                        methodName          = setting.Method.Name;
                    }                    
                    notifyElement.SetAttribute("method",    methodName);
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
                throw new Exception("You do not have authorization to save the notificationa file.",
                                    ex);
            }
        }
    }
}
