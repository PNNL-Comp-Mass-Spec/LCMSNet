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
        public List<classDevicePluginInformation> GetAvailablePlugins()
        {
            var manager = classDeviceManager.Manager;
            var availablePlugins = new List<classDevicePluginInformation>();
            foreach (var plugin in manager.AvailablePlugins)
            {
                availablePlugins.Add(plugin);
            }

            return availablePlugins;
        }

        public List<classDeviceErrorEventArgs> AddDevices(List<classDevicePluginInformation> plugins,
            bool initializeOnAdd)
        {
            var failedDevices = new List<classDeviceErrorEventArgs>();

            foreach (var plugin in plugins)
            {
                if (plugin != null)
                {
                    try
                    {
                        var wasAdded = classDeviceManager.Manager.AddDevice(plugin, initializeOnAdd);
                        if (!wasAdded)
                        {
                            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                                "Could not create the selected device: " + plugin.DeviceAttribute.Name);
                        }
                        else
                        {
                            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                                "A " + plugin.DeviceAttribute.Name + " device was added.");
                        }
                    }
                    catch (classDeviceInitializationException ex)
                    {
                        failedDevices.Add(ex.ErrorDetails);
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                            string.Format(
                                "The device {0} was added but was not initialized properly. Error Message: {1}",
                                ex.ErrorDetails.Device.Name, ex.ErrorDetails.Error));
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                            "Could not create the device " + plugin.DeviceAttribute.Name + " " + ex.Message, ex);
                    }
                }
            }

            return failedDevices;
        }
    }
}