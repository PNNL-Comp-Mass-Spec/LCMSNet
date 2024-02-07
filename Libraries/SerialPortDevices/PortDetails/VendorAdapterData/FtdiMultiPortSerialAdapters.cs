using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Microsoft.Win32;

namespace SerialPortDevices.PortDetails.SerialAdapterData
{
    internal static class FtdiMultiPortSerialAdapters
    {
        public static void UpdateFtdiMultiPortSerialInfo(Dictionary<string, SerialPortDetails> mapping, Dictionary<string, WmiSerialPortDetails> wmiPnpSerialPorts, Action<string, Exception> warningAction)
        {
            // Overwrite certain FTDI serial adapter information with data read from the registry, since it lets us associate (kind of) port numbers with port names
            var regFtdiMultiportSerial = ReadFtdiUsbMultiPortSerialInfoFromRegistry(warningAction);
            foreach (var serPort in regFtdiMultiportSerial.Where(x => x.PortNumber >= 0))
            {
                if (wmiPnpSerialPorts.TryGetValue(serPort.SerialDeviceId, out var pnpPort))
                {
                    var name = pnpPort.ComPort.ToUpper();

                    var data = new SerialPortDetails(name, $"StarTech (FTDI) Multi Port {serPort.PortNumber}", serPort.ParentIdPrefix, serPort.PortNumber);

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

        private interface IFtdiUsbData
        {
            string UsbDevicePath { get; }
            int ParentPortId { get; set; }
            int ParentHubId { get; set; }
            string ParentIdPrefix { get; set; }
        }

        private class FtdiRegistryUsbEntry : IFtdiUsbData
        {
            public string SerialDeviceId { get; }
            public string UsbDevicePath { get; }
            public int ParentPortId { get; set; } = -1;
            public int ParentHubId { get; set; } = -1;
            public string ParentIdPrefix { get; set; } = "";
            public int PortNumber { get; set; } = -1;

            public FtdiRegistryUsbEntry(string serialDeviceId, string usbDevicePath)
            {
                SerialDeviceId = serialDeviceId;
                UsbDevicePath = usbDevicePath;
            }
        }

        private class FtdiRegistryTreeEntry : IFtdiUsbData
        {
            public string UsbDevicePath { get; }
            public int HubId { get; }
            public int ParentPortId { get; set; } = -1;
            public int ParentHubId { get; set; } = -1;
            public string ParentIdPrefix { get; set; } = "";
            public List<FtdiRegistryTreeEntry> ChildHubs { get; } = new List<FtdiRegistryTreeEntry>();
            public List<FtdiRegistryUsbEntry> ChildDevices { get; } = new List<FtdiRegistryUsbEntry>();
            public IEnumerable<IFtdiUsbData> Children => ChildHubs.Cast<IFtdiUsbData>().Concat(ChildDevices);

            public FtdiRegistryTreeEntry(int hubId, string usbDevicePath)
            {
                HubId = hubId;
                UsbDevicePath = usbDevicePath;
            }

            public FtdiRegistryTreeEntry FindHub(int hubId)
            {
                if (HubId == hubId)
                {
                    return this;
                }

                foreach (var hub in ChildHubs)
                {
                    var match = hub.FindHub(hubId);

                    if (match != null)
                    {
                        return match;
                    }
                }

                return null;
            }
        }

        private static List<FtdiRegistryUsbEntry> ReadFtdiUsbMultiPortSerialInfoFromRegistry(Action<string, Exception> warningAction)
        {
            // NOTE: This was designed for StarTech multi-port adapters, ICUSB2322I/ICUSB2324I/ICUSB2328I
            // The internal design is USB input->USB 7-port Hub->FTDI USB Serial Adapters; the 8-port adapter has a nested USB 7-port hub with 2 additional USB Serial Adapters.
            // The USB Serial Adapters in the tested device all had pretty similar serial numbers
            /*
             * NOTES:
             * For USB Hub->hub number mapping, see:
             * HKLM\SYSTEM\CurrentControlSet\Services\usbhub\Enum\(SubKeys: Name is hub index (0-based), Data (string) is path under HKLM\SYSTEM\CurrentControlSet\Enum\, also has SubKeys 'Count' and 'NextInstance')
             * HKLM\SYSTEM\CurrentControlSet\Services\USBHUB3\Enum\(SubKeys: Name is hub index (0-based), Data (string) is path under HKLM\SYSTEM\CurrentControlSet\Enum\, also has SubKeys 'Count' and 'NextInstance')
             *
             * To map USB Serial Port DeviceIDs to USB DeviceIDs:
             * USB Serial Port DeviceIDs are values in HKLM\SYSTEM\CurrentControlSet\Services\FTSER2K\Enum
             * USB DeviceIDs are values in HKLM\SYSTEM\CurrentControlSet\Services\FTDIBUS\Enum
             * The SubKey names match between them, and the values both map to key under HKLM\SYSTEM\CurrentControlSet\Enum\
             *
             * Then, to map the hub tree and USB DeviceIDs (with port numbers), read and process:
             * HKLM\SYSTEM\CurrentControlSet\Enum\ (USB DeviceID), SubKeys:
             *   - Address: Parent USB Hub port number
             *   - LocationInformation: string in format 'Port_#0001.Hub_#0005', which are the port number and parent USB Hub index (1-based)
             *   - ParentIdPrefix: string that also appears in the USB Hub DeviceID (followed by '&1', where 1 is the parent USB Hub port number)
             */

            var usbRoot = new FtdiRegistryTreeEntry(-1, "SystemRoot");
            var usbSer = new List<FtdiRegistryUsbEntry>();

            try
            {

                var regPermission = new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet");
                regPermission.Assert();

                const string servicesBasePath = @"SYSTEM\CurrentControlSet\Services\";

                using (var baseKey = Registry.LocalMachine)
                {
                    // Step 1: Get all of the USB Hubs in the system
                    var usbHubKeyNames = new string[] { "usbhub", "USBHUB3" };
                    var usbHubKeyPaths = usbHubKeyNames.Select(x => servicesBasePath + x + @"\Enum");

                    var hubIds = ReadRegistryIntStringValues(baseKey, usbHubKeyPaths);

                    if (hubIds.Count == 0)
                    {
                        return usbSer;
                    }

                    // Step 2: Get all of the FTSER2K and FTDIBUS service entries and map them
                    var ftdiBusServiceEntries = ReadRegistryIntStringValues(baseKey, servicesBasePath + @"FTDIBUS\Enum");
                    var ftSer2KServiceEntries = ReadRegistryIntStringValues(baseKey, servicesBasePath + @"FTSER2K\Enum");

                    if (ftSer2KServiceEntries.Count == 0)
                    {
                        return usbSer;
                    }

                    var ftSer2KtoFtdiBusMap = new List<KeyValuePair<string, string>>();
                    foreach (var entry in ftSer2KServiceEntries)
                    {
                        if (ftdiBusServiceEntries.TryGetValue(entry.Key, out var ftdiBus))
                        {
                            ftSer2KtoFtdiBusMap.Add(new KeyValuePair<string, string>(entry.Value, ftdiBus));
                            usbSer.Add(new FtdiRegistryUsbEntry(entry.Value, ftdiBus));
                        }
                    }

                    // Step 3: Get the information needed to create a tree of USB Hubs
                    // Hub IDs: need to add '1' to match with the 'LocationInformation' registry subkeys.
                    var hubIds2 = hubIds.OrderBy(x => x.Key).Select(x => new FtdiRegistryTreeEntry(x.Key + 1, x.Value)).ToList();

                    ReadRegistryUsbTree(baseKey, hubIds2);

                    // Build the tree
                    foreach (var entry in hubIds2)
                    {
                        var addPlace = usbRoot.FindHub(entry.ParentHubId);
                        if (addPlace == null)
                        {
                            addPlace = usbRoot;
                        }
                        addPlace.ChildHubs.Add(entry);
                    }

                    // Step 4: Get the information needed to add the serial port entries to the USB Hub tree
                    ReadRegistryUsbTree(baseKey, usbSer);

                    // Add the serial ports to the tree
                    foreach (var entry in usbSer)
                    {
                        var addPlace = usbRoot.FindHub(entry.ParentHubId);
                        addPlace.ChildDevices.Add(entry);
                    }

                    // Debugging: Output the assembled tree
                    //OutputTree(usbRoot);
                }

                // Step 5: Determine the hardware port IDs for the software COM port name
                // Step 5a: Determine common serial number prefixes
                var serialPrefixes = new Dictionary<string, int>();

                foreach (var entry in usbSer)
                {
                    var prefix = entry.UsbDevicePath.Substring(0, entry.UsbDevicePath.Length - 2);
                    if (!serialPrefixes.ContainsKey(prefix))
                    {
                        serialPrefixes.Add(prefix, 0);
                    }

                    serialPrefixes[prefix]++;
                }

                // Step 5b: Process the serial prefixes to group by parent hub device
                foreach (var prefix in serialPrefixes.Keys)
                {
                    if (serialPrefixes[prefix] == 1)
                    {
                        continue;
                    }

                    var matched = usbSer.Where(x => x.UsbDevicePath.StartsWith(prefix)).ToList();
                    foreach (var entry in matched)
                    {
                        entry.ParentIdPrefix = prefix;
                    }

                    var parentHubs = matched.Select(x => x.ParentHubId).Distinct().Select(x => usbRoot.FindHub(x));
                    var processedHubs = new List<int>();
                    foreach (var parentHub in parentHubs.OrderBy(x => x.HubId))
                    {
                        if (!processedHubs.Contains(parentHub.HubId))
                        {
                            processedHubs.AddRange(ProcessTree(parentHub));
                        }
                    }
                }

                //foreach (var entry in usbSer.OrderBy(x => x.ParentHubId).ThenBy(x => x.ParentPortId))
                //{
                //    Console.WriteLine("H{0}P{1}: {2},\t{3},\t{4}", entry.ParentHubId, entry.ParentPortId, entry.UsbDevicePath, entry.PortNumber, entry.SerialDeviceId);
                //}
            }
            catch (Exception ex)
            {
                warningAction("Unable to read USB multi-port configuration information from the registry. If no USB-to-multi-Serial adapter is connected, this warning can be ignored.", ex);
            }
            finally
            {
                RegistryPermission.RevertAssert();
            }


            return usbSer;
        }

        private static Dictionary<int, string> ReadRegistryIntStringValues(RegistryKey baseKey, string subKeyToRead)
        {
            return ReadRegistryIntStringValues(baseKey, new[] { subKeyToRead });
        }

        private static Dictionary<int, string> ReadRegistryIntStringValues(RegistryKey baseKey, IEnumerable<string> subKeysToRead)
        {
            var data = new Dictionary<int, string>();

            foreach (var subKeyPath in subKeysToRead)
            {
                // Returns 'null' (not an exception) for non-existent keys
                using (var subKey = baseKey.OpenSubKey(subKeyPath, false))
                {
                    if (subKey != null)
                    {
                        var valNames = subKey.GetValueNames();
                        foreach (var valName in valNames)
                        {
                            if (!int.TryParse(valName, out var id))
                            {
                                continue;
                            }

                            data[id] = subKey.GetValue(valName).ToString();
                        }
                    }
                }
            }

            return data;
        }

        private static void ReadRegistryUsbTree(RegistryKey baseKey, IEnumerable<IFtdiUsbData> dataToMap)
        {
            const string enumBasePath = @"SYSTEM\CurrentControlSet\Enum\";

            foreach (var entry in dataToMap)
            {
                // Returns 'null' (not an exception) for non-existent keys
                using (var subKey = baseKey.OpenSubKey(enumBasePath + entry.UsbDevicePath, false))
                {
                    if (subKey != null)
                    {
                        var valNames = subKey.GetValueNames();
                        foreach (var valName in valNames)
                        {
                            if (valName.Equals("Address", StringComparison.OrdinalIgnoreCase))
                            {
                                // read DWORD value
                                var value = subKey.GetValue(valName);
                                entry.ParentPortId = Convert.ToInt32(value);
                            }
                            else if (valName.Equals("LocationInformation", StringComparison.OrdinalIgnoreCase))
                            {
                                var value = subKey.GetValue(valName).ToString();
                                var parts = value.Split('.');
                                foreach (var part in parts)
                                {
                                    if (part.StartsWith("Hub_#", StringComparison.OrdinalIgnoreCase) && int.TryParse(part.Substring(part.Length - 4), out var hubId))
                                    {
                                        entry.ParentHubId = hubId;
                                    }
                                }
                            }
                            else if (valName.Equals("ParentIdPrefix", StringComparison.OrdinalIgnoreCase))
                            {
                                entry.ParentIdPrefix = subKey.GetValue(valName).ToString();
                            }
                        }
                    }
                }
            }
        }

        private static List<int> ProcessTree(FtdiRegistryTreeEntry hub)
        {
            var start = 1;
            return ProcessTree(hub, ref start);
        }

        private static List<int> ProcessTree(FtdiRegistryTreeEntry hub, ref int counter)
        {
            var processedIds = new List<int>(5) { hub.HubId };

            foreach (var child in hub.Children.OrderBy(x => x.ParentPortId))
            {
                if (child is FtdiRegistryUsbEntry device)
                {
                    device.PortNumber = counter++;
                }
                else if (child is FtdiRegistryTreeEntry childHub)
                {
                    processedIds.AddRange(ProcessTree(childHub, ref counter));
                }
            }

            return processedIds;
        }

        //private static void OutputTree(FtdiRegistryTreeEntry entry, string indent = "")
        //{
        //    var subIndent = indent + "  ";
        //
        //    Console.WriteLine("{0}Port {1}: Hub #{2} ({3}) Devices: ({4})", indent, entry.ParentPortId, entry.HubId, entry.UsbDevicePath, entry.ChildDevices.Count + entry.ChildHubs.Count);
        //    foreach (var item in entry.ChildDevices.OrderBy(x => x.ParentPortId))
        //    {
        //        Console.WriteLine("{0}Port {1}: {2} ({3})", subIndent, item.ParentPortId, item.SerialDeviceId, item.UsbDevicePath);
        //    }
        //
        //    foreach (var item in entry.ChildHubs.OrderBy(x => x.ParentPortId))
        //    {
        //        OutputTree(item, subIndent);
        //    }
        //}
    }
}
