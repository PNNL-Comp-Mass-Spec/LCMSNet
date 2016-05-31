using System;
using System.Net.Sockets;

namespace client
{
    public class Test
    {
        public static void Main()
        {
            clsNetStartClient client = new clsNetStartClient();
            client.Connect("192.168.30.138", 4871);
            client.GetMethods();

            //client.Handshake();
            //client.Disconnect();
            client.Disconnect();
        }
    }
}

