using System;

namespace LcmsNetPlugins.ZaberStage
{
    public readonly struct ConnectionStageID : IEquatable<ConnectionStageID>
    {
        public string PortName { get; }
        public long SerialNumber { get; }
        public string ModelName { get; }

        public ConnectionStageID(string portName, long serialNumber, string modelName)
        {
            PortName = portName;
            SerialNumber = serialNumber;
            ModelName = modelName;
        }

        public override string ToString()
        {
            return $"{SerialNumber} ({ModelName})";
        }

        public bool Equals(ConnectionStageID other)
        {
            return PortName == other.PortName && SerialNumber == other.SerialNumber;
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionStageID other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PortName != null ? PortName.GetHashCode() : 0) * 397) ^ SerialNumber.GetHashCode();
            }
        }
    }
}
