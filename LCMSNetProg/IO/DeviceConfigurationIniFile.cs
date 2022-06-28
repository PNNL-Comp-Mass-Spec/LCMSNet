using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LcmsNet.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace LcmsNet.IO
{
    public class DeviceConfigurationIniFile : IDeviceConfigurationFile
    {
        /// <summary>
        /// Tag used above every device.
        /// </summary>
        private const string CONST_DEVICE_HEADER_TAG = "[Device]";

        /// <summary>
        /// tag used above the connections section
        /// </summary>
        private const string CONST_CONNECTIONS_HEADER_TAG = "[Connections]";

        /// <summary>
        /// Delimiter of file.
        /// </summary>
        private const string CONST_DELIMITER = " = ";

        public DeviceConfiguration ReadConfiguration(string path)
        {
            var configLines = File.ReadAllLines(path);

            // Find the device headers.
            var deviceHeaders = new List<int>();
            var i = 0;
            var connectionsIndex = configLines.Length;
                // if connection section doesn't exist, we should just go on, so set this to length of the array.
            foreach (var line in configLines)
            {
                var xline = line.Replace("\n", "");
                xline = xline.Replace("\r", "");

                if (xline == CONST_DEVICE_HEADER_TAG)
                {
                    deviceHeaders.Add(i);
                }
                if (xline == CONST_CONNECTIONS_HEADER_TAG)
                {
                    //point at the first connection after the connections header
                    connectionsIndex = i + 1;
                }
                i++;
            }

            var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var exeDir = Path.GetDirectoryName(Path.GetFullPath(exePath)) + "\\";
            var pathStartsToRemove = new string[]
            {
                exeDir,
                @"C:\Program Files (x86)\LCMSNet\",
                @"C:\Program Files\LCMSNet\",
            };

            var configuration = new DeviceConfiguration();
            for (i = 0; i < deviceHeaders.Count; i++)
            {
                var k = i + 1;
                var startIndex = deviceHeaders[i];
                var lastIndex = startIndex;
                if (k == deviceHeaders.Count)
                {
                    lastIndex = configLines.Length;
                }
                else
                {
                    lastIndex = deviceHeaders[k];
                }

                var deviceName = "";
                var data = new Dictionary<string, string>();
                var delimiter = new[] {CONST_DELIMITER};
                for (var j = startIndex + 1; j < lastIndex; j++)
                {
                    var line = configLines[j];
                    var lineData = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    if (lineData.Length == 2)
                    {
                        data.Add(lineData[0], lineData[1]);
                    }
                }

                deviceName = Convert.ToString(data["DeviceName"]);
                foreach (var key in data.Keys)
                {
                    var value = data[key];
                    if (key.Equals("DeviceType"))
                    {
                        // Update device names/full namespace from old versions to new
                        value = OldDeviceNameTranslator.TranslateOldDeviceFullName(value);
                    }

                    if (key.Equals("PluginPath"))
                    {
                        // Part 1: truncate the full path to be a relative path
                        foreach (var start in pathStartsToRemove)
                        {
                            if (value.StartsWith(start, StringComparison.OrdinalIgnoreCase))
                            {
                                //value = value.Replace(start, ""); // Not case-insensitive
                                value = value.Substring(start.Length); // Sufficient because we only care about removing the start of the string; if there are multiple occurrences, that's a problem anyway.
                                break;
                            }
                        }

                        // Part 2: update U12 contact closure DLL from PNNLDevices to LabJackU12
                        if (data.TryGetValue("DeviceType", out var fullName) && fullName.Contains("ContactClosure") &&
                            value.EndsWith("PNNLDevices.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            value = Regex.Replace(value, "PNNLDevices", "LabJackU12", RegexOptions.IgnoreCase);
                        }
                    }

                    configuration.AddSetting(deviceName, key, value);
                }
            }
            for (var j = connectionsIndex; j < configLines.Length; j++)
            {
                var line = configLines[j];
                var delimiter = new[] {","};
                var lineData = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (lineData.Length == 3)
                {
                    configuration.AddConnection("conn" + (j - connectionsIndex),
                        lineData[0] + "," + lineData[1] + "," + lineData[2]);
                }
            }
            return configuration;
        }

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
                        writer.WriteLine("{0}{1}{2}", setting, CONST_DELIMITER, value);
                    }
                }
                writer.WriteLine(CONST_CONNECTIONS_HEADER_TAG);
                var connections = configuration.GetConnections();
                foreach (var connID in connections.Keys)
                {
                    writer.WriteLine(connections[connID]);
                }
            }
        }
    }
}
