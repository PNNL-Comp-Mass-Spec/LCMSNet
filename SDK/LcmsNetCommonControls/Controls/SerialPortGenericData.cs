﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using LcmsNetCommonControls.Controls.SerialAdapterData;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNetCommonControls.Controls
{
    /// <summary>
    /// Static class to hold system-wide serial port information for UI uses
    /// </summary>
    public static class SerialPortGenericData
    {
        private static readonly ObservableCollectionExtended<string> SerialPortNamesList;
        private static readonly ObservableCollectionExtended<SerialPortData> SerialPortsList;

        /// <summary>
        /// List of serial port names
        /// </summary>
        public static ReadOnlyObservableCollection<string> SerialPortNames {get; }

        /// <summary>
        /// List of serial ports
        /// </summary>
        public static ReadOnlyObservableCollection<SerialPortData> SerialPorts { get; }

        static SerialPortGenericData()
        {
            SerialPortNamesList = new ObservableCollectionExtended<string>();
            SerialPortsList = new ObservableCollectionExtended<SerialPortData>();

            SerialPortNamesList.ToObservableChangeSet().Bind(out var serialPortNamesBound).Subscribe();
            SerialPortsList.ToObservableChangeSet().Bind(out var serialPortsBound).Subscribe();
            SerialPortNames = serialPortNamesBound;
            SerialPorts = serialPortsBound;

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
            using (SerialPortsList.SuspendNotifications())
            using (SerialPortNamesList.SuspendNotifications())
            {
                SerialPortsList.Clear();
                SerialPortNamesList.Clear();
                SerialPortsList.AddRange(GetSerialPortInformation());
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
            // WMI Serial Port information: Will give very accurate information (details that WMI PnP does not provide), but may not have all serial ports (i.e., Prolific USB-To-Serial devices)
            var wmiPnpSerialPorts = WmiSerialData.AddSerialPortWmiInfo(mapping);

            // Overwrite the EdgePort information with data read from the registry, since it lets us associate port numbers with port names
            EdgePortSerialData.UpdateEdgePortSerialInfo(mapping);

            // Overwrite the Keyspan serial adapter information with data read from the registry, since it lets us associate port numbers with port names
            KeySpanMultiPortSerialData.UpdateKeySpanMultiPortSerialInfo(mapping, wmiPnpSerialPorts);

            // Overwrite certain FTDI serial adapter information with data read from the registry, since it lets us associate (kind of) port numbers with port names
            FtdiMultiPortSerialData.UpdateFtdiMultiPortSerialInfo(mapping, wmiPnpSerialPorts);

            var list = mapping.Values.ToList();
            list.Sort();
            return list;
        }

    }
}
