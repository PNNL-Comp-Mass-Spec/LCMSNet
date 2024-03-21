using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK.Logging;
using ReactiveUI;
using SerialPortDevices.PortDetails;

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

        private static IEnumerable<SerialPortData> GetSerialPortInformation()
        {
            var ports = SerialPortDetails.GetAllSerialPorts((message, ex) => ApplicationLogger.LogMessage(LogLevel.Warning, $"{message}; Exception '{ex.GetType()}': {ex}"));

            return ports.Select(x => new SerialPortData(x.PortName, x.PortDescription, x.EdgePortSerialNum, x.EdgePortPortNumber));
        }
    }
}
