/*********************************************************************************************************
 * Written by Brian LaMarche, Dave Clark, John Ryan for the US Department of Energy 
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2009, Battelle Memorial Institute
 * Created 06/19/2009
 * 
 *  Last modified 06/19/2009
 *      Created class and added static methods with static device manager object that registers itself with 
 *      the static method property.
 *      
 *  12-12-2009: BLL
 *      Added a method, FindDevice, to search for a device given its name (key) and it's type (found via GetType()).
 *  12-10-2009: BLL
 *      Created plug-ins, and loading of a new configuration pattern.
 * 
/*********************************************************************************************************/

using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Holds the configuration of a collection of devices for persistence.
    /// </summary>
    [Serializable]
    public class classDeviceConfiguration
    {
        /// <summary>
        /// Maps devices to their settings.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, object>> m_settings;

        /// <summary>
        /// holds a list of connections and the ports that they connection
        /// unique ID of connection is the key, the ports are a comma separated string that make up the value
        /// </summary>
        private readonly Dictionary<string, string> m_connections;

        /// <summary>
        /// Holds a list of devices that can be enumerated through.
        /// </summary>
        private readonly List<string> m_devices;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classDeviceConfiguration()
        {
            m_settings = new Dictionary<string, Dictionary<string, object>>();
            m_devices = new List<string>();
            m_connections = new Dictionary<string, string>();
        }

        /// <summary>
        /// Cart name
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Gets the number of devices stored here.
        /// </summary>
        public int DeviceCount
        {
            get { return m_devices.Count; }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">Index of IDevice.</param>
        /// <returns>Device in collection at index.</returns>
        public string this[int index]
        {
            get { return m_devices[index]; }
        }

        /// <summary>
        /// Adds a setting for the given device.
        /// </summary>
        /// <param name="deviceName">Device to persist.  If not added before will be saved</param>
        /// <param name="settingsName">Name of setting.</param>
        /// <param name="value">Value to assign.</param>
        public void AddSetting(string deviceName, string settingsName, object value)
        {
            if (!m_settings.ContainsKey(deviceName))
            {
                m_devices.Add(deviceName);
                m_settings.Add(deviceName, new Dictionary<string, object>());
            }
            m_settings[deviceName].Add(settingsName, value);
        }

        /// <summary>
        /// Retrieves the device settings for the specified device.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDeviceSettings(string deviceName)
        {
            return m_settings[deviceName];
        }

        public void AddConnection(string connId, string ports)
        {
            m_connections[connId] = ports;
        }

        public Dictionary<string, string> GetConnections()
        {
            return m_connections;
        }
    }
}