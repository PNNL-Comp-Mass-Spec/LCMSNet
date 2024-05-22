using System;

namespace ZaberStageControl
{
    public readonly struct ConnectionStageID : IEquatable<ConnectionStageID>
    {
        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName { get; }

        /// <summary>
        /// Stage serial number
        /// </summary>
        public long SerialNumber { get; }

        /// <summary>
        /// Stage model name
        /// </summary>
        public string ModelName { get; }

        /// <summary>
        /// Stage axis number
        /// </summary>
        public int AxisNumber { get; }

        /// <summary>
        /// For non-'X-' stage models, the controller serial number
        /// </summary>
        public long ControllerSerialNumber { get; }

        /// <summary>
        /// Should be 'true' for non-'X-' stage models
        /// </summary>
        public bool NonIntegratedController { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Serial port name</param>
        /// <param name="serialNumber">Stage serial number</param>
        /// <param name="modelName">Stage model name</param>
        /// <param name="axisNumber">Stage axis number</param>
        /// <param name="controllerSerialNumber">For non-'X-' stage models, the controller serial number</param>
        /// <param name="nonIntegratedController">Should be 'true' for non-'X-' stage models</param>
        public ConnectionStageID(string portName, long serialNumber, string modelName, int axisNumber = 1, long controllerSerialNumber = -1, bool nonIntegratedController = false)
        {
            PortName = portName;
            SerialNumber = serialNumber;
            ModelName = modelName;
            AxisNumber = axisNumber;
            ControllerSerialNumber = controllerSerialNumber;
            NonIntegratedController = nonIntegratedController;
        }

        public override string ToString()
        {
            if (NonIntegratedController)
            {
                return $"{ControllerSerialNumber}->{SerialNumber} ({ModelName})";
            }

            return $"{SerialNumber} ({ModelName})";
        }

        public bool Equals(ConnectionStageID other)
        {
            return NonIntegratedController == other.NonIntegratedController && PortName == other.PortName && SerialNumber == other.SerialNumber && AxisNumber == other.AxisNumber && ControllerSerialNumber == other.ControllerSerialNumber;
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionStageID other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = NonIntegratedController.GetHashCode();
                hashCode = (hashCode * 397) ^ (PortName != null ? PortName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SerialNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ AxisNumber;
                hashCode = (hashCode * 397) ^ ControllerSerialNumber.GetHashCode();
                return hashCode;
            }
        }
    }
}
