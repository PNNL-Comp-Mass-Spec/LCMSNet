using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Concurrency;
using System.Security.Permissions;
using LcmsNetData.Logging;
using Microsoft.Win32;
using ReactiveUI;

namespace LcmsNetCommonControls.Controls
{
    /// <summary>
    /// Static class to hold system-wide serial port information for UI uses
    /// </summary>
    public static class SerialPortGenericData
    {
        private static readonly ReactiveList<string> SerialPortNamesList;
        private static readonly ReactiveList<SerialPortData> SerialPortsList;

        /// <summary>
        /// List of serial port names
        /// </summary>
        public static IReadOnlyReactiveList<string> SerialPortNames => SerialPortNamesList;

        /// <summary>
        /// List of serial ports
        /// </summary>
        public static IReadOnlyReactiveList<SerialPortData> SerialPorts => SerialPortsList;

        static SerialPortGenericData()
        {
            SerialPortNamesList = new ReactiveList<string>();
            SerialPortsList = new ReactiveList<SerialPortData>();
            ReadAndStoreSerialPorts();
        }

        /// <summary>
        /// Updates the serial port name list
        /// </summary>
        public static void UpdateSerialPorts()
        {
            RxApp.MainThreadScheduler.Schedule(ReadAndStoreSerialPorts);
        }

        private static void ReadAndStoreSerialPorts()
        {
            using (SerialPortsList.SuppressChangeNotifications())
            {
                SerialPortsList.Clear();
                SerialPortsList.AddRange(GetSerialPortInformation());
            }

            using (SerialPortNamesList.SuppressChangeNotifications())
            {
                SerialPortNamesList.Clear();
                SerialPortNamesList.AddRange(SerialPortsList.Select(x => x.PortName));
            }
        }

        private static List<SerialPortData> GetSerialPortInformation()
        {
            var mapping = new Dictionary<string, SerialPortData>();

            try
            {
                // SerialPort.GetPortNames() is the least descriptive - it gives us only the names
                foreach (var port in SerialPort.GetPortNames())
                {
                    mapping.Add(port.ToUpper(), new SerialPortData(port));
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, "Unable to read serial port names from SerialPort.GetPortNames(). Serial Port listing may not be complete.", ex);
            }

            // WMI PnP information: Will give the most complete information, but may not be accurate (i.e., the COM ports for EdgePort devices may all be wrong)
            // Don't let this data add new serial ports due to the possible inaccuracy
            var wmiPnpSerialPorts = ReadSerialPortWmiPnPInfo();
            foreach (var port in wmiPnpSerialPorts.Values)
            {
                var data = new SerialPortData(port.ComPort, port.Description, port.DeviceId);

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
            var wmiSerialPorts = ReadSerialPortWmiInfo();
            foreach (var port in wmiSerialPorts.Values)
            {
                var data = new SerialPortData(port.ComPort, port.Description, port.DeviceId);

                if (mapping.ContainsKey(port.ComPort.ToUpper()))
                {
                    mapping[port.ComPort.ToUpper()] = data;
                }
                else
                {
                    mapping.Add(port.ComPort.ToUpper(), data);
                }
            }

            // Overwrite the EdgePort information with data read from the registry, since it lets us associate port numbers with port names
            var regEdgePortSerial = ReadEdgeSerialPortDataFromRegistry();
            foreach (var serialNumGp in regEdgePortSerial.Values.GroupBy(x => x.SerialNumBase))
            {
                var serialNumber = serialNumGp.Key;
                var portNum = 1;
                foreach (var serialNum in serialNumGp.OrderBy(x => x.SerialNumExt))
                {
                    var descriptionBase = serialNumber;
                    if (!string.IsNullOrWhiteSpace(serialNum.EasyName) && !serialNum.EasyName.Contains(serialNumber))
                    {
                        descriptionBase = serialNum.EasyName + $" ({descriptionBase})";
                    }

                    foreach (var num in serialNum.ComSetup.Split(','))
                    {
                        var thisPortNum = portNum++;
                        if (string.IsNullOrWhiteSpace(num))
                        {
                            continue;
                        }

                        var name = $"COM{num}";
                        var data = new SerialPortData(name, $"EdgePort {descriptionBase} Port {thisPortNum}", serialNumber, thisPortNum);

                        if (mapping.ContainsKey(name.ToUpper()))
                        {
                            mapping[name.ToUpper()] = data;
                        }
                        else
                        {
                            mapping.Add(name.ToUpper(), data);
                        }
                    }
                }
            }

            var list = mapping.Values.ToList();
            list.Sort();
            return list;
        }

        private class EdgeSerialRegistryData
        {
            public string ComSetup { get; set; }
            public string SerialNumExt { get; set; }
            public string EasyName { get; set; }
            public string SerialNumBase { get; set; }
        }

        private static Dictionary<string, EdgeSerialRegistryData> ReadEdgeSerialPortDataFromRegistry()
        {
            // EdgePort configuration in registry: see HKLM\SYSTEM\CurrentControlSet\services\EdgeSer\Parameters for current configuration (but properly detecting quantity? I don't know yet.)
            // Example: 16 port edgeport unit:
            // Example: Parameters\V32409111-0\ComSetup (REG_SZ) has value "1,2,3,4,5,6,7,8", mapping ports 1-8 to COM1-8, in order
            // Example: Parameters\V32409111-1\ComSetup (REG_SZ) has value "9,10,11,12,13,14,15,16", mapping ports 9-16 to COM9-16, in order
            //                     ^ these keys change from computer to computer, and if 2 keys have the same base name, they are halves of the same unit, with '0' being the lower half, and '1' being the upper half

            var data = new Dictionary<string, EdgeSerialRegistryData>();

            try
            {
                var regPermission = new RegistryPermission(RegistryPermissionAccess.Read,
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\EdgeSer\Parameters");
                regPermission.Assert();

                using (var baseKey = Registry.LocalMachine)
                using (var edgeSerKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\services\EdgeSer\Parameters", false))
                {
                    if (edgeSerKey != null)
                    {
                        var subkeyNames = edgeSerKey.GetSubKeyNames();
                        foreach (var subkeyName in subkeyNames)
                        {
                            using (var subkey = edgeSerKey.OpenSubKey(subkeyName, false))
                            {
                                if (subkey == null)
                                {
                                    continue;
                                }

                                var regData = new EdgeSerialRegistryData
                                    {SerialNumExt = subkeyName, SerialNumBase = subkeyName};
                                if (subkeyName[subkeyName.Length - 2] == '-')
                                {
                                    regData.SerialNumBase = subkeyName.Substring(0, subkeyName.Length - 2);
                                }

                                var valNames = subkey.GetValueNames();
                                foreach (var valName in valNames)
                                {
                                    var value = subkey.GetValue(valName).ToString();
                                    if (valName.Equals("ComSetup", StringComparison.OrdinalIgnoreCase))
                                    {
                                        regData.ComSetup = value;
                                    }
                                    else if (valName.Equals("EasyName", StringComparison.OrdinalIgnoreCase))
                                    {
                                        regData.EasyName = value;
                                    }
                                }

                                // Prevent null exceptions, make sure the EasyName is set.
                                if (string.IsNullOrWhiteSpace(regData.EasyName))
                                {
                                    regData.EasyName = regData.SerialNumBase;
                                }

                                // Entries are not removed for non-present devices, but the com port configuration might be
                                if (!string.IsNullOrWhiteSpace(regData.ComSetup))
                                {
                                    data.Add(regData.SerialNumExt, regData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, "Unable to read EdgePort configuration information from the registry. If no EdgePort USB-to-Serial adapter is connected, this warning can be ignored.", ex);
            }
            finally
            {
                RegistryPermission.RevertAssert();
            }

            return data;
        }

        private class WmiSerialData
        {
            public string ComPort { get; set; }
            public string DeviceId { get; set; }
            public string Caption { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Service { get; set; }
        }

        private static Dictionary<string, WmiSerialData> ReadSerialPortWmiPnPInfo()
        {
            // See WMI query "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'" for all com port friendly names/descriptions (but doesn't line up with EdgePort port numbers)
            // Also for WMI Win32_PnPEntity, if Service='Serial', it is a standard serial device, if Service='EdgeSer', it is an EdgePort device, if Service='Ser2ol', it is a prolific USB-to-Serial
            // Unfortunately, this listing can show incorrect COM ports for some devices (i.e., EdgePort USB-To-Serial devices)
            // A better method may be to use "SELECT * FROM Win32_SerialPort", and use 'DeviceID', since it is correct.

            // WMI gives us port descriptions, but the information for EdgePort devices is... somewhat randomized (port names are correct, but it can't give us the port number)
            // It can also give incorrect ports for devices (e.g., EdgePort), so don't use this function to add new ports.
            var data = new Dictionary<string, WmiSerialData>();

            try
            {
                using (var objSearcher = new System.Management.ManagementObjectSearcher(
                    @"SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%' OR Service = 'Serial'"))
                {
                    foreach (var match in objSearcher.Get())
                    {
                        var wmiData = new WmiSerialData
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
                ApplicationLogger.LogError(LogLevel.Warning, "Unable to read serial port information from WMI. Extra detail about COM ports will not be displayed.", ex);
            }

            return data;
        }

        private static Dictionary<string, WmiSerialData> ReadSerialPortWmiInfo()
        {
            // See WMI query "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'" for all com port friendly names/descriptions (but doesn't line up with EdgePort port numbers)
            // Also for WMI Win32_PnPEntity, if Service='Serial', it is a standard serial device, if Service='EdgeSer', it is an EdgePort device, if Service='Ser2ol', it is a prolific USB-to-Serial
            // Unfortunately, this listing can show incorrect COM ports for some devices (i.e., EdgePort USB-To-Serial devices)
            // A better method may be to use "SELECT * FROM Win32_SerialPort", and use 'DeviceID', since it is correct.

            var data = new Dictionary<string, WmiSerialData>();

            try
            {
                using (var objSearcher = new System.Management.ManagementObjectSearcher(
                    @"SELECT * FROM Win32_SerialPort"))
                {
                    foreach (var match in objSearcher.Get())
                    {
                        var wmiData = new WmiSerialData
                        {
                            ComPort = match["DeviceID"].ToString(),
                            DeviceId = match["PNPDeviceID"].ToString(),
                            Caption = match["Caption"].ToString(),
                            Name = match["Name"].ToString(),
                            Description = match["Description"].ToString(),
                            Service = match["ProviderType"].ToString()
                        };

                        data.Add(wmiData.DeviceId, wmiData);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, "Unable to read serial port information from WMI. Extra detail about COM ports will not be displayed.", ex);
            }

            return data;
        }
    }
}
