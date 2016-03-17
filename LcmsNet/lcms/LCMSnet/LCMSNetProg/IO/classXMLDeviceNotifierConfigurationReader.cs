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
    public class classXMLDeviceNotifierConfigurationReader
    {
        /// <summary>
        /// Writes the configuration to file.
        /// </summary>
        /// <param name="path">Path to write configuration to.</param>
        /// <param name="configuration"></param>
        public NotificationConfiguration ReadConfiguration(string path)
        {
            if (!File.Exists(path))
            {
                return new NotificationConfiguration();
            }

            NotificationConfiguration configuration = new NotificationConfiguration();
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode rootElement = document.SelectSingleNode("Devices");

            foreach (XmlNode node in rootElement.ChildNodes)
            {
                if (node.Name == "Device")
                {
                    ReadDevice(configuration, node);
                }
                else if (node.Name == "SystemSettings")
                {
                    ReadNotificationSetting(configuration, node);
                }
            }
            return configuration;
        }

        private static void ReadNotificationSetting(NotificationConfiguration configuration, XmlNode node)
        {
            XmlNode nameNode = node.Attributes.GetNamedItem("Ignore");
            if (nameNode != null)
            {
                configuration.IgnoreNotifications = Convert.ToBoolean(nameNode.Value);
            }
        }

        private static void ReadDevice(NotificationConfiguration configuration, XmlNode node)
        {
            string deviceName = node.Attributes.GetNamedItem("name").Value;
            IDevice device = classDeviceManager.Manager.FindDevice(deviceName);


            INotifier notifier = device;
            if (device == null)
            {
                foreach (INotifier simpleNotifier in Notification.NotificationBroadcaster.Manager.Notifiers)
                {
                    if (simpleNotifier.Name == deviceName)
                    {
                        notifier = simpleNotifier;
                        break;
                    }
                }
            }

            if (notifier == null)
            {
                return;
            }

            foreach (XmlNode notificationNode in node.ChildNodes)
            {
                if (notificationNode.Name == "Notification")
                {
                    string name = notificationNode.Attributes.GetNamedItem("name").Value;
                    string type = notificationNode.Attributes.GetNamedItem("type").Value;
                    string action = notificationNode.Attributes.GetNamedItem("action").Value;
                    string method = notificationNode.Attributes.GetNamedItem("method").Value;
                    XmlNode conditionNode = notificationNode.ChildNodes[0];

                    // Determine the action
                    enumDeviceNotificationAction actionEnum = enumDeviceNotificationAction.Ignore;
                    string[] names = Enum.GetNames(typeof (enumDeviceNotificationAction));

                    try
                    {
                        actionEnum =
                            (enumDeviceNotificationAction) Enum.Parse(typeof (enumDeviceNotificationAction), action);
                    }
                    catch
                    {
                        //TODO: need to write handling code.
                        throw;
                    }

                    // Create the setting.
                    NotificationSetting setting = null;
                    if (type.ToLower() == "number")
                    {
                        NotificationNumberSetting number = new NotificationNumberSetting();
                        number.Minimum = Convert.ToDouble(conditionNode.Attributes.GetNamedItem("minimum").Value);
                        number.Maximum = Convert.ToDouble(conditionNode.Attributes.GetNamedItem("maximum").Value);
                        setting = number;
                    }
                    else if (type.ToLower() == "text")
                    {
                        NotificationTextSetting text = new NotificationTextSetting();
                        text.Text = conditionNode.Attributes[0].Value;
                        setting = text;
                    }
                    else if (type.ToLower() == "always")
                    {
                        NotificationAlwaysSetting always = new NotificationAlwaysSetting();
                        setting = always;
                    }
                    setting.Name = name;
                    if (!string.IsNullOrEmpty(method))
                    {
                        if (LcmsNet.Method.classLCMethodManager.Manager.Methods.ContainsKey(method))
                        {
                            setting.Method = LcmsNet.Method.classLCMethodManager.Manager.Methods[method];
                        }
                    }
                    setting.Action = actionEnum;

                    configuration.AddSetting(notifier, setting);
                }
            }
        }
    }
}