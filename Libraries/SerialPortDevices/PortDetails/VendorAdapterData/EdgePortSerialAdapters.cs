using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Microsoft.Win32;

namespace SerialPortDevices.PortDetails.SerialAdapterData
{
    internal static class EdgePortSerialAdapters
    {
        public static void UpdateEdgePortSerialInfo(Dictionary<string, SerialPortDetails> mapping, Action<string, Exception> warningAction)
        {
            // Overwrite the EdgePort information with data read from the registry, since it lets us associate port numbers with port names
            var regEdgePortSerial = ReadEdgeSerialPortDataFromRegistry(warningAction);
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
                        var data = new SerialPortDetails(name, $"EdgePort {descriptionBase} Port {thisPortNum}", serialNumber, thisPortNum);

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
        }

        private class EdgeSerialRegistryData
        {
            public string ComSetup { get; set; }
            public string SerialNumExt { get; set; }
            public string EasyName { get; set; }
            public string SerialNumBase { get; set; }
        }

        private static Dictionary<string, EdgeSerialRegistryData> ReadEdgeSerialPortDataFromRegistry(Action<string, Exception> warningAction)
        {
            // EdgePort configuration in registry: see HKLM\SYSTEM\CurrentControlSet\services\EdgeSer\Parameters for current configuration (but properly detecting quantity? I don't know yet.)
            // Example: 16 port edgeport unit:
            // Example: Parameters\V32409111-0\ComSetup (REG_SZ) has value "1,2,3,4,5,6,7,8", mapping ports 1-8 to COM1-8, in order
            // Example: Parameters\V32409111-1\ComSetup (REG_SZ) has value "9,10,11,12,13,14,15,16", mapping ports 9-16 to COM9-16, in order
            //                     ^ these keys change from computer to computer, and if 2 keys have the same base name, they are halves of the same unit, with '0' being the lower half, and '1' being the upper half

            var data = new Dictionary<string, EdgeSerialRegistryData>();

            try
            {
                var regPermission = new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\EdgeSer\Parameters");
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

                                var regData = new EdgeSerialRegistryData { SerialNumExt = subkeyName, SerialNumBase = subkeyName };
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
                warningAction("Unable to read EdgePort configuration information from the registry. If no EdgePort USB-to-Serial adapter is connected, this warning can be ignored.", ex);
            }
            finally
            {
                RegistryPermission.RevertAssert();
            }

            return data;
        }
    }
}
