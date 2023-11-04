namespace LcmsNetPlugins.VICI.Valves
{
    internal readonly struct ValveConnectionID
    {
        public string PortName { get; }
        public char ID { get; }

        public ValveConnectionID(string portName, char id)
        {
            PortName = portName;
            ID = id;
        }

        public override string ToString()
        {
            return $"{PortName}-{ID}";
        }
    }
}
