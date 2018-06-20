using System;
using System.Collections.Generic;
using System.IO;
using LcmsNetSDK.Devices;

namespace LcmsNet.Devices
{
    public class DeviceConfigurationINIReader : IDeviceConfigurationReader
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
        /// Delimeter of file.
        /// </summary>
        private const string CONST_DELIMETER = " = ";

        #region IDeviceConfigurationReader Members

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
                var data = new Dictionary<string, object>();
                var delimeter = new[] {CONST_DELIMETER};
                for (var j = startIndex + 1; j < lastIndex; j++)
                {
                    var line = configLines[j];
                    var lineData = line.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
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
                        value = OldDeviceNameTranslator.TranslateOldDeviceFullName(value.ToString());
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

        #endregion
    }
}