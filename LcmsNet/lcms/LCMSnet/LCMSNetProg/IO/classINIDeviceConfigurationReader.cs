using System;
using System.IO;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{
    public class classINIDeviceConfigurationReader : IDeviceConfigurationReader
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

        public classDeviceConfiguration ReadConfiguration(string path)
        {
            string[] configLines = File.ReadAllLines(path);

            // Find the device headers.
            List<int> deviceHeaders = new List<int>();
            int i = 0;
            int connectionsIndex = configLines.Length;
                // if connection section doesn't exist, we should just go on, so set this to length of the array.
            foreach (string line in configLines)
            {
                string xline = line.Replace("\n", "");
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

            classDeviceConfiguration configuration = new classDeviceConfiguration();
            for (i = 0; i < deviceHeaders.Count; i++)
            {
                int k = i + 1;
                int startIndex = deviceHeaders[i];
                int lastIndex = startIndex;
                if (k == deviceHeaders.Count)
                {
                    lastIndex = configLines.Length;
                }
                else
                {
                    lastIndex = deviceHeaders[k];
                }

                string deviceName = "";
                Dictionary<string, object> data = new Dictionary<string, object>();
                string[] delimeter = new string[] {CONST_DELIMETER};
                for (int j = startIndex + 1; j < lastIndex; j++)
                {
                    string line = configLines[j];
                    string[] lineData = line.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                    if (lineData.Length == 2)
                    {
                        data.Add(lineData[0], lineData[1]);
                    }
                }

                deviceName = Convert.ToString(data["DeviceName"]);
                foreach (string key in data.Keys)
                {
                    configuration.AddSetting(deviceName, key, data[key]);
                }
            }
            for (int j = connectionsIndex; j < configLines.Length; j++)
            {
                string line = configLines[j];
                string[] delimiter = new string[] {","};
                string[] lineData = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (lineData.Length == 3)
                {
                    configuration.AddConnection("conn" + (j - connectionsIndex).ToString(),
                        lineData[0] + "," + lineData[1] + "," + lineData[2]);
                }
            }
            return configuration;
        }

        #endregion
    }
}