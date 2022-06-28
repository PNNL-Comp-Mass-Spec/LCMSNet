using System;
using System.Collections.Generic;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;

namespace LcmsNet.Devices
{
    public class DeviceAddController
    {
        /// <summary>
        /// Gets a list of available plugins.
        /// </summary>
        /// <returns></returns>
        public List<DevicePluginInformation> GetAvailablePlugins()
        {
            var manager = DeviceManager.Manager;
            var availablePlugins = new List<DevicePluginInformation>();
            foreach (var plugin in manager.AvailablePlugins)
            {
                availablePlugins.Add(plugin);
            }

            return availablePlugins;
        }

        public List<DeviceErrorEventArgs> AddDevices(List<DevicePluginInformation> plugins,
            bool initializeOnAdd)
        {
            var failedDevices = new List<DeviceErrorEventArgs>();

            foreach (var plugin in plugins)
            {
                if (plugin != null)
                {
                    try
                    {
                        var wasAdded = DeviceManager.Manager.AddDevice(plugin, initializeOnAdd);
                        if (!wasAdded)
                        {
                            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                                "Could not create the selected device: " + plugin.DeviceAttribute.Name);
                        }
                        else
                        {
                            ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                                "A " + plugin.DeviceAttribute.Name + " device was added.");
                        }
                    }
                    catch (DeviceInitializationException ex)
                    {
                        failedDevices.Add(ex.ErrorDetails);
                        ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                            string.Format(
                                "The device {0} was added but was not initialized properly. Error Message: {1}",
                                ex.ErrorDetails.Device.Name, ex.ErrorDetails.Error));
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                            "Could not create the device " + plugin.DeviceAttribute.Name + " " + ex.Message, ex);
                    }
                }
            }

            return failedDevices;
        }
    }
}
