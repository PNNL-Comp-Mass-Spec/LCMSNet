using System;
using System.IO;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.Devices
{
    class classINIDeviceConfigurationWriter : IDeviceConfigurationWriter
    {
        /// <summary>
        /// Tag used above every device.
        /// </summary>
        private const string CONST_DEVICE_HEADER_TAG = "[Device]";

        private const string CONST_CONNECTION_HEADER_TAG = "[Connections]";

        /// <summary>
        /// Delimeter of file.
        /// </summary>
        private const string CONST_DELIMETER = " = ";

        /// <summary>
        /// Writes the configuration to file.
        /// </summary>
        /// <param name="path">Path to write configuration to.</param>
        /// <param name="configuration"></param>
        public void WriteConfiguration(string path, classDeviceConfiguration configuration)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                string systemInformation = SystemInformationReporter.BuildSystemInformation();
                writer.WriteLine(systemInformation);

                for (int i = 0; i < configuration.DeviceCount; i++)
                {
                    writer.WriteLine(CONST_DEVICE_HEADER_TAG);
                    string deviceName = configuration[i];
                    Dictionary<string, object> settings = configuration.GetDeviceSettings(deviceName);
                    foreach (string setting in settings.Keys)
                    {
                        object value = settings[setting];
                        writer.WriteLine("{0}{1}{2}", setting, CONST_DELIMETER, value);
                    }
                }
                writer.WriteLine(CONST_CONNECTION_HEADER_TAG);
                Dictionary<string, string> connections = configuration.GetConnections();
                foreach (string connID in connections.Keys)
                {
                    writer.WriteLine(connections[connID]);
                }
            }
        }
    }
}