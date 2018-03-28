using System.IO;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

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
        public void WriteConfiguration(string path, DeviceConfiguration configuration)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                var systemInformation = SystemInformationReporter.BuildSystemInformation();
                writer.WriteLine(systemInformation);

                for (var i = 0; i < configuration.DeviceCount; i++)
                {
                    writer.WriteLine(CONST_DEVICE_HEADER_TAG);
                    var deviceName = configuration[i];
                    var settings = configuration.GetDeviceSettings(deviceName);
                    foreach (var setting in settings.Keys)
                    {
                        var value = settings[setting];
                        writer.WriteLine("{0}{1}{2}", setting, CONST_DELIMETER, value);
                    }
                }
                writer.WriteLine(CONST_CONNECTION_HEADER_TAG);
                var connections = configuration.GetConnections();
                foreach (var connID in connections.Keys)
                {
                    writer.WriteLine(connections[connID]);
                }
            }
        }
    }
}