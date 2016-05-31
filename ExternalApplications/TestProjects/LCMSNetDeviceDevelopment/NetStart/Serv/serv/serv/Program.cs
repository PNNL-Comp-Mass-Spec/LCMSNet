using System;
using System.Net.Sockets;

namespace serv
{

    public class Server
    {
        public static void Main()
        {
            TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, 4871);
            tcpListener.Start();

            Socket socketForClient = tcpListener.AcceptSocket();

            if (socketForClient.Connected)
            {
                Console.WriteLine("Connection established");

                NetworkStream networkStream = new NetworkStream(socketForClient);

                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(networkStream);
                System.IO.StreamReader streamReader = new System.IO.StreamReader(networkStream);

                streamWriter.WriteLine("CONNECTED");
                Console.WriteLine("CONNECTED");
                streamWriter.Flush();

                string message = streamReader.ReadLine();
                Console.WriteLine(message);

                streamWriter.WriteLine("IDLE");
                Console.WriteLine("IDLE");
                streamWriter.Flush();

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                streamWriter.WriteLine("RECEIVED");
                Console.WriteLine("RECEIVED");
                streamWriter.Flush();

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                streamWriter.WriteLine("READY");
                Console.WriteLine("READY");
                streamWriter.Flush();

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                streamWriter.WriteLine("PREPARED");
                Console.WriteLine("PREPARED");
                streamWriter.Flush();

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                message = streamReader.ReadLine();
                Console.WriteLine(message);

                streamWriter.WriteLine("STARTED");
                Console.WriteLine("STARTED");
                streamWriter.Flush();           
 

                streamReader.Close();
                networkStream.Close();
                streamWriter.Close();
            }
            socketForClient.Close();
            Console.WriteLine("Exiting...");
        }
    }
}