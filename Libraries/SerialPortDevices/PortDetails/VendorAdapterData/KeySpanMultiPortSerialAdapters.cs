using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.Win32;

namespace SerialPortDevices.PortDetails.SerialAdapterData
{
    internal static class KeySpanMultiPortSerialAdapters
    {
        public static void UpdateKeySpanMultiPortSerialInfo(Dictionary<string, SerialPortDetails> mapping, Dictionary<string, WmiSerialPortDetails> wmiPnpSerialPorts, Action<string, Exception> warningAction)
        {
            // Overwrite the Keyspan serial adapter information with data read from the registry, since it lets us associate port numbers with port names
            var regKeyspanSerial = ReadKeyspanSerialPortDataFromRegistry(warningAction);
            foreach (var serPort in regKeyspanSerial.Values)
            {
                if (wmiPnpSerialPorts.TryGetValue(serPort.DeviceId, out var pnpPort))
                {
                    var name = pnpPort.ComPort.ToUpper();

                    var data = new SerialPortDetails(name, $"Keyspan Port {serPort.PortNumber}", serPort.DeviceId, serPort.PortNumber);

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

        private class KeyspanSerialRegistryData
        {
            public string PortIndex { get; set; }
            public int PortNumber { get; set; }
            public string DeviceId { get; set; }
        }

        private static Dictionary<string, KeyspanSerialRegistryData> ReadKeyspanSerialPortDataFromRegistry(Action<string, Exception> warningAction)
        {
            // EdgePort configuration in registry: see HKLM\SYSTEM\CurrentControlSet\Services\USA49WGP\Enum for current configuration (but properly detecting quantity? I don't know yet.)
            // Example: 4 port Keyspan unit:
            // Example: "0"="KEYSPAN\\*USA49WGMAP\\00_00" is a mapping from port index to system device ID

            var data = new Dictionary<string, KeyspanSerialRegistryData>();

            try
            {
                var regPermission = new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\USA49WGP\Enum");
                regPermission.Assert();

                using (var baseKey = Registry.LocalMachine)
                using (var keyspanSerKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USA49WGP\Enum", false))
                {
                    if (keyspanSerKey != null)
                    {
                        var valNames = keyspanSerKey.GetValueNames();
                        foreach (var valName in valNames)
                        {
                            if (!int.TryParse(valName, out var portIndex))
                            {
                                continue;
                            }

                            var regData = new KeyspanSerialRegistryData
                            { PortIndex = valName, PortNumber = portIndex + 1, DeviceId = keyspanSerKey.GetValue(valName).ToString() };

                            // Entries are not removed for non-present devices, but the com port configuration might be
                            if (!string.IsNullOrWhiteSpace(regData.DeviceId))
                            {
                                data.Add(regData.DeviceId, regData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                warningAction("Unable to read Keyspan multi-port configuration information from the registry. If no Keyspan USB-to-multi-Serial adapter is connected, this warning can be ignored.", ex);
            }
            finally
            {
                RegistryPermission.RevertAssert();
            }

            return data;
        }
    }
}
