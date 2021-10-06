using System.Collections.Generic;

namespace LcmsNetPlugins.PNNLDevices.NetworkStart.Socket
{
    class NetStartMessage
    {
        public NetStartMessageTypes Type { get; set; }

        public int Sequence { get; set; }

        public string Descriptor { get; set; }

        public List<NetStartArgument> ArgumentList { get; set; }
    }
}
