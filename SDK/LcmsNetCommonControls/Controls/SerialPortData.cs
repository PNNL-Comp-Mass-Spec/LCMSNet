using System;

namespace LcmsNetCommonControls.Controls
{
    /// <summary>
    /// Class to contain information about a serial port
    /// </summary>
    public class SerialPortData : IComparable<SerialPortData>
    {
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
        public SerialPortData(string portName, string portDescription = "", string edgePortSerialNum = "", int edgePortPortNumber = 0)
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
        public int CompareTo(SerialPortData other)
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
