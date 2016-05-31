using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace client
{
    public class Test
    {
        public static void Main()
        {
            classNetStartClient client = new classNetStartClient();
            client.Connect("192.168.30.138", 4771);
            //client.Connect("localhost", 10);
            List<string> methods = client.GetMethods();

            if (methods.Count > 1)
                client.StartAcquisition("erinisadork", methods[0]);

            client.Disconnect();
        }
    }
}

