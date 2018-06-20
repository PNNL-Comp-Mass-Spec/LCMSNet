namespace LcmsNetPlugins.PNNLDevices.NetworkStart.Socket
{
    public class NetStartArgument
    {
        public NetStartArgument()
        {
            Key = "";
            Value = "";
        }

        public NetStartArgument(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
