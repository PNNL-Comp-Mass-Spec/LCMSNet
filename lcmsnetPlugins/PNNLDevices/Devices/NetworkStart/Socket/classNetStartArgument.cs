namespace LcmsNet.Devices.NetworkStart.Socket
{
    public class classNetStartArgument
    {
        public classNetStartArgument()
        {
            Key = "";
            Value = "";
        }

        public classNetStartArgument(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
