﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Concurrency;
using System.Security.Permissions;
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
            UpdateSerialPorts();
        }

        /// <summary>
        /// Updates the serial port name list
        /// </summary>
        public static void UpdateSerialPorts()
        {
            RxApp.MainThreadScheduler.Schedule(() =>
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
            });
        }

        private static List<SerialPortData> GetSerialPortInformation()
        {
            var sysSerialPorts = SerialPort.GetPortNames();
            var wmiSerialPorts = ReadSerialPortWmiInfo();
            var regEdgePortSerial = ReadEdgeSerialPortDataFromRegistry();

            var mapping = new Dictionary<string, SerialPortData>();

            // SerialPort.GetPortNames() is the least descriptive - it gives us only the names
            foreach (var port in sysSerialPorts)
            {
                mapping.Add(port.ToUpper(), new SerialPortData(port));
            }

            // WMI gives use port descriptions, but the information for EdgePort devices is... somewhat randomized (port names are correct, but it can't give use the port number)
            // It can also give false devices (like 2 non-existent COM ports on EdgePort devices), so don't add new ports here.
            foreach (var port in wmiSerialPorts.Values)
            {
                var name = port.Caption.Substring(port.Caption.LastIndexOf("(COM", StringComparison.OrdinalIgnoreCase)).Trim('(', ')');
                var data = new SerialPortData(name, port.Description, port.DeviceId);

                if (mapping.ContainsKey(name.ToUpper()))
                {
                    mapping[name.ToUpper()] = data;
                }
                //else
                //{
                //    mapping.Add(name.ToUpper(), data);
                //}
            }

            // Overwrite the EdgePort information with data read from the registry, since it lets us associate port numbers with port names
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

            var regPermission = new RegistryPermission(RegistryPermissionAccess.Read,
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\EdgeSer\Parameters");
            regPermission.Assert();

            try
            {
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

                                var regData = new EdgeSerialRegistryData {SerialNumExt = subkeyName, SerialNumBase = subkeyName};
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

                                data.Add(regData.SerialNumExt, regData);
                            }
                        }
                    }
                }
            }
            finally
            {
                RegistryPermission.RevertAssert();
            }

            return data;
        }

        private class WmiSerialData
        {
            public string DeviceId { get; set; }
            public string Caption { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Service { get; set; }
        }

        private static Dictionary<string, WmiSerialData> ReadSerialPortWmiInfo()
        {
            // See WMI query "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%' for all com port friendly names/descriptions (but doesn't line up with EdgePort port numbers)
            // Also for WMI Win32_PnPEntity, if Service='Serial', it is a standard serial device, if Service='EdgeSer', it is an EdgePort device

            var data = new Dictionary<string, WmiSerialData>();

            using (var objSearcher = new System.Management.ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%' OR Service = 'Serial'"))
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

                    data.Add(wmiData.DeviceId, wmiData);
                }
            }

            return data;
        }
    }
}