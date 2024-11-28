using System;
using System.Collections.Generic;

namespace SerialPortDevices.PortDetails.SerialAdapterData
{
    internal class WmiSerialPortDetails
    {
        public string ComPort { get; set; }
        public string DeviceId { get; set; }
        public string Caption { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Service { get; set; }

        public static Dictionary<string, WmiSerialPortDetails> AddSerialPortWmiInfo(Dictionary<string, SerialPortDetails> mapping, Action<string, Exception> warningAction)
        {
            // WMI PnP information: Will give the most complete information, but may not be accurate (i.e., the COM ports for EdgePort devices may all be wrong)
            // Don't let this data add new serial ports due to the possible inaccuracy
            var wmiPnpSerialPorts = WmiSerialPortDetails.ReadSerialPortWmiPnPInfo(warningAction);
            foreach (var port in wmiPnpSerialPorts.Values)
            {
                var portID = 0;

                var data = new SerialPortDetails(port.ComPort, port.Description, port.DeviceId);

                if (mapping.ContainsKey(port.ComPort.ToUpper()))
                {
                    mapping[port.ComPort.ToUpper()] = data;
                }
                //else
                //{
                //    mapping.Add(port.ComPort.ToUpper(), data);
                //}
            }

            // WMI Serial Port information: Will give very accurate information (details that WMI PnP does not provide), but may not have all serial ports (i.e., Prolific USB-To-Serial devices)
            var wmiSerialPorts = WmiSerialPortDetails.ReadSerialPortWmiInfo(warningAction);
            foreach (var port in wmiSerialPorts.Values)
            {
                var data = new SerialPortDetails(port.ComPort, port.Description, port.DeviceId);

                if (mapping.ContainsKey(port.ComPort.ToUpper()))
                {
                    mapping[port.ComPort.ToUpper()] = data;
                }
                else
                {
                    mapping.Add(port.ComPort.ToUpper(), data);
                }
            }

            return wmiPnpSerialPorts;
        }

        private static Dictionary<string, WmiSerialPortDetails> ReadSerialPortWmiPnPInfo(Action<string, Exception> warningAction)
        {
            // See WMI query "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'" for all com port friendly names/descriptions (but doesn't line up with EdgePort port numbers)
            // Also for WMI Win32_PnPEntity, if Service='Serial', it is a standard serial device, if Service='EdgeSer', it is an EdgePort device, if Service='Ser2ol', it is a prolific USB-to-Serial
            // Unfortunately, this listing can show incorrect COM ports for some devices (i.e., EdgePort USB-To-Serial devices)
            // A better method may be to use "SELECT * FROM Win32_SerialPort", and use 'DeviceID', since it is correct.

            // WMI gives us port descriptions, but the information for EdgePort devices is... somewhat randomized (port names are correct, but it can't give us the port number)
            // It can also give incorrect ports for devices (e.g., EdgePort), so don't use this function to add new ports.

            var data = new Dictionary<string, WmiSerialPortDetails>();

            try
            {
                using (var objSearcher = new System.Management.ManagementObjectSearcher(
                    @"SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%' OR Service = 'Serial'"))
                {
                    foreach (var match in objSearcher.Get())
                    {
                        var wmiData = new WmiSerialPortDetails
                        {
                            DeviceId = match["DeviceID"].ToString(),
                            Caption = match["Caption"].ToString(),
                            Name = match["Name"].ToString(),
                            Description = match["Description"].ToString(),
                            Service = match["Service"].ToString()
                        };

                        wmiData.ComPort = wmiData.Caption.Substring(wmiData.Caption.LastIndexOf("(COM", StringComparison.OrdinalIgnoreCase)).Trim('(', ')').ToUpper();
                        data.Add(wmiData.DeviceId, wmiData);
                    }
                }
            }
            catch (Exception ex)
            {
                warningAction("Unable to read serial port information from WMI (Win32_PnPEntity). Extra detail about COM ports might not be available.", ex);
            }

            return data;
        }

        private static Dictionary<string, WmiSerialPortDetails> ReadSerialPortWmiInfo(Action<string, Exception> warningAction)
        {
            // See WMI query "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'" for all com port friendly names/descriptions (but doesn't line up with EdgePort port numbers)
            // Also for WMI Win32_PnPEntity, if Service='Serial', it is a standard serial device, if Service='EdgeSer', it is an EdgePort device, if Service='Ser2ol', it is a prolific USB-to-Serial
            // Unfortunately, this listing can show incorrect COM ports for some devices (i.e., EdgePort USB-To-Serial devices)
            // A better method may be to use "SELECT * FROM Win32_SerialPort", and use 'DeviceID', since it is correct.

            var data = new Dictionary<string, WmiSerialPortDetails>();

            try
            {
                using (var objSearcher = new System.Management.ManagementObjectSearcher(
                    @"SELECT * FROM Win32_SerialPort"))
                {
                    foreach (var match in objSearcher.Get())
                    {
                        var wmiData = new WmiSerialPortDetails
                        {
                            ComPort = match["DeviceID"].ToString(),
                            DeviceId = match["PNPDeviceID"].ToString(),
                            Caption = match["Caption"].ToString(),
                            Name = match["Name"].ToString(),
                            Description = match["Description"].ToString(),
                            Service = match["ProviderType"]?.ToString() ?? ""
                        };

                        data.Add(wmiData.DeviceId, wmiData);
                    }
                }
            }
            catch (Exception ex)
            {
                warningAction("Unable to read serial port information from WMI (Win32_SerialPort). Extra detail about COM ports might not be displayed.", ex);
            }

            return data;
        }
    }
}
