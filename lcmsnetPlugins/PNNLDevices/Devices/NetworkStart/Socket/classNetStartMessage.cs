using System.Collections.Generic;


namespace LcmsNet.Devices.NetworkStart.Socket
{
    class classNetStartMessage
    {
        public enumNetStartMessageTypes Type { get; set; }

        public int Sequence { get; set; }

        public string Descriptor { get; set; }

        public List<classNetStartArgument> ArgumentList { get; set; }
    }
}
