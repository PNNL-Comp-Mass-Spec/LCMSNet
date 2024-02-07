using SerialPortDevices.PortDetails.SerialAdapterData;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace SerialPortDevices.PortDetails
{
    /// <summary>
    /// Class to contain information about a serial port and a static method to read/process the data
    /// </summary>
    public class SerialPortDetails : IComparable<SerialPortDetails>
    {
        /// <summary>
        /// Read the serial ports and any additional information we can find to help accurately identify which serial port name is associated with which physical port
        /// </summary>
        /// <param name="warningAction">Method to use for reporting warnings while reading data for the serial ports.</param>
        /// <returns></returns>
        public static List<SerialPortDetails> GetAllSerialPorts(Action<string, Exception> warningAction)
        {
            var mapping = new Dictionary<string, SerialPortDetails>();

            try
            {
                // SerialPort.GetPortNames() is the least descriptive - it gives us only the names
                foreach (var port in SerialPort.GetPortNames())
                {
                    mapping.Add(port.ToUpper(), new SerialPortDetails(port));
                }
            }
            catch (Exception ex)
            {
                warningAction("Unable to read serial port names from SerialPort.GetPortNames(). Serial Port listing may not be complete.", ex);
            }

            // WMI PnP information: Will give the most complete information, but may not be accurate (i.e., the COM ports for EdgePort devices may all be wrong)
            // WMI Serial Port information: Will give very accurate information (details that WMI PnP does not provide), but may not have all serial ports (i.e., Prolific USB-To-Serial devices)
            var wmiPnpSerialPorts = WmiSerialPortDetails.AddSerialPortWmiInfo(mapping, warningAction);

            // Overwrite the EdgePort information with data read from the registry, since it lets us associate port numbers with port names
            EdgePortSerialAdapters.UpdateEdgePortSerialInfo(mapping, warningAction);

            // Overwrite the Keyspan serial adapter information with data read from the registry, since it lets us associate port numbers with port names
            KeySpanMultiPortSerialAdapters.UpdateKeySpanMultiPortSerialInfo(mapping, wmiPnpSerialPorts, warningAction);

            // Overwrite certain FTDI serial adapter information with data read from the registry, since it lets us associate (kind of) port numbers with port names
            FtdiMultiPortSerialAdapters.UpdateFtdiMultiPortSerialInfo(mapping, wmiPnpSerialPorts, warningAction);

            var list = mapping.Values.ToList();
            list.Sort();
            return list;
        }

        /// <summary>
        /// Name of the port. Usually 'COMxx'
        /// </summary>
        public string PortName { get; }

        /// <summary>
        /// Descriptive information about the port
        /// </summary>
        public string PortDescription { get; }

        /// <summary>
        /// Port number on edgeport device; set to 0 for non-edgeport devices
        /// </summary>
        public int EdgePortPortNumber { get; }

        /// <summary>
        /// Serial Number of the EdgePort device
        /// </summary>
        public string EdgePortSerialNum { get; }

        /// <summary>
        /// Descriptive string to display in UI
        /// </summary>
        public string DisplayString { get; }

        private readonly string sortKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="portDescription"></param>
        /// <param name="edgePortSerialNum"></param>
        /// <param name="edgePortPortNumber"></param>
        internal SerialPortDetails(string portName, string portDescription = "", string edgePortSerialNum = "", int edgePortPortNumber = 0)
        {
            PortName = portName;
            PortDescription = portDescription;
            EdgePortPortNumber = edgePortPortNumber;
            EdgePortSerialNum = edgePortSerialNum;

            if (!PortName.ToLower().StartsWith("com"))
            {
                sortKey = PortName;
            }
            else
            {
                var sortKeyTemp = portName;
                // remove "COM" from the name
                var comNumber = PortName.Substring(3);
                if (int.TryParse(comNumber, out var comNumberInt))
                {
                    sortKeyTemp = $"COM{comNumberInt:D2}";
                }

                if (EdgePortPortNumber > 0)
                {
                    sortKeyTemp = $"E_{EdgePortSerialNum}_P{EdgePortPortNumber:D2}_{sortKeyTemp}";
                }

                sortKey = sortKeyTemp;
            }

            if (string.IsNullOrWhiteSpace(portDescription))
            {
                DisplayString = PortName;
            }
            else
            {
                DisplayString = $"{PortName}: {PortDescription}";
            }
        }

        /// <summary>
        /// IComparable implementation for sorting
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SerialPortDetails other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(sortKey, other.sortKey, StringComparison.Ordinal);
        }

        /// <summary>
        /// Overridden ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return PortName;
        }
    }
}
