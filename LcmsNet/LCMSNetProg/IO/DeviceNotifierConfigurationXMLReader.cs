﻿using System;
using System.IO;
using System.Xml;
using LcmsNet.Notification;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Concrete class that reads the notification system from XML.
    /// </summary>
    public class DeviceNotifierConfigurationXMLReader
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

            var configuration = new NotificationConfiguration();
            var document = new XmlDocument();
            document.Load(path);
            var rootElement = document.SelectSingleNode("Devices");

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
            var nameNode = node.Attributes.GetNamedItem("Ignore");
            if (nameNode != null)
            {
                configuration.IgnoreNotifications = Convert.ToBoolean(nameNode.Value);
            }
        }

        private static void ReadDevice(NotificationConfiguration configuration, XmlNode node)
        {
            var deviceName = node.Attributes.GetNamedItem("name").Value;
            var device = DeviceManager.Manager.FindDevice(deviceName);


            INotifier notifier = device;
            if (device == null)
            {
                foreach (var simpleNotifier in NotificationBroadcaster.Manager.Notifiers)
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
                    var name = notificationNode.Attributes.GetNamedItem("name").Value;
                    var type = notificationNode.Attributes.GetNamedItem("type").Value;
                    var action = notificationNode.Attributes.GetNamedItem("action").Value;
                    var method = notificationNode.Attributes.GetNamedItem("method").Value;
                    var conditionNode = notificationNode.ChildNodes[0];

                    // Determine the action
                    var actionEnum = DeviceNotificationAction.Ignore;
                    var names = Enum.GetNames(typeof (DeviceNotificationAction));

                    try
                    {
                        actionEnum =
                            (DeviceNotificationAction) Enum.Parse(typeof (DeviceNotificationAction), action);
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
                        var number = new NotificationNumberSetting();
                        number.Minimum = Convert.ToDouble(conditionNode.Attributes.GetNamedItem("minimum").Value);
                        number.Maximum = Convert.ToDouble(conditionNode.Attributes.GetNamedItem("maximum").Value);
                        setting = number;
                    }
                    else if (type.ToLower() == "text")
                    {
                        var text = new NotificationTextSetting();
                        text.Text = conditionNode.Attributes[0].Value;
                        setting = text;
                    }
                    else if (type.ToLower() == "always")
                    {
                        var always = new NotificationAlwaysSetting();
                        setting = always;
                    }
                    setting.Name = name;
                    if (!string.IsNullOrEmpty(method))
                    {
                        if (LCMethodManager.Manager.TryGetLCMethod(method, out var lcMethod))
                        {
                            setting.Method = lcMethod;
                        }
                    }
                    setting.Action = actionEnum;

                    configuration.AddSetting(notifier, setting);
                }
            }
        }
    }
}