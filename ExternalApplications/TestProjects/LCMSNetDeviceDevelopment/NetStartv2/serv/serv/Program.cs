using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;


namespace serv
{

    public class Server
    {
        private static void SendMessage(System.IO.StreamWriter streamWriter, enumNetStartMessageTypes type, int sequence, string descriptor, List<classNetStartArgument> arglist)
        {
            //Console.WriteLine(descriptor);

            string message = PackMessage(type, sequence, descriptor, arglist);

            streamWriter.WriteLine(message);
            Console.WriteLine("Send: " + message);
            streamWriter.Flush();
        }

        private static string PackMessage(enumNetStartMessageTypes type, int sequence, string descriptor, List<classNetStartArgument> arglist)
        {
            string message = type.GetHashCode().ToString() + ":" + sequence.ToString() + "|" + descriptor;
            if (type == enumNetStartMessageTypes.Post || type == enumNetStartMessageTypes.Response || type == enumNetStartMessageTypes.Execute)
            {
                foreach (classNetStartArgument arg in arglist)
                {
                    message += "@" + arg.Key + "=" + arg.Value;
                }
            }
            else if (type == enumNetStartMessageTypes.Query)
            {
                foreach (classNetStartArgument arg in arglist)
                {
                    message += "@" + arg.Key;
                }
            }

            return message;
        }

        private static classNetStartMessage UnpackMessage(string message)
        {
            //<type>:<sqnc>|<dscp>@<argS1>=<argV1>@<argS1>=<argV1>...@<argSn>=<argVn>

            classNetStartMessage msg = new classNetStartMessage();
            List<classNetStartArgument> args = new List<classNetStartArgument>();
            char[] tokens = { ':', '@', '|', '=' };
            string[] messagepieces = message.Split(tokens);

            msg.Type = (enumNetStartMessageTypes)Enum.Parse(typeof(enumNetStartMessageTypes), messagepieces[0]);
            msg.Sequence = Int32.Parse(messagepieces[1]);
            msg.Descriptor = messagepieces[2];

            for (int i = 3; i < messagepieces.Count() - 1; i += 2)
            {
                args.Add(new classNetStartArgument(messagepieces[i], messagepieces[i + 1]));
            }

            msg.ArgList = args;


            /*foreach (string piece in messagepieces)
            {
                Console.WriteLine(piece);
            }
            */


            return msg;
        }

        public static void Main()
        {
            TcpListener tcpListener = new TcpListener(10);
            tcpListener.Start();

            Socket socketForClient = tcpListener.AcceptSocket();

            if (socketForClient.Connected)
            {
                List<classNetStartArgument> arguments = new List<classNetStartArgument>();

                Console.WriteLine("Connection established");

                NetworkStream networkStream = new NetworkStream(socketForClient);

                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(networkStream);
                System.IO.StreamReader streamReader = new System.IO.StreamReader(networkStream);

                /*
                streamWriter.WriteLine("CONNECTED");
                Console.WriteLine("CONNECTED");
                streamWriter.Flush();
                */

                string message;
                enumNetStartMessageTypes type;
                string descriptor;
                bool send;

                while (2 + 2 != 5)
                {
                    message = streamReader.ReadLine();
                    Console.WriteLine("Rec : " + message);
                    send = true;

                    /*switch(UnpackMessage(message).Type)
                    {
                        case enumNetStartMessageTypes.Post:
                            send = false;
                            break;
                        case enumNetStartMessageTypes.Query:
                            send = true;
                            break;
                        default:
                            send = false;
                            break;
                    }*/

                    if (message == null)
                        break;

                    switch(UnpackMessage(message).Descriptor)
                    {
                        case "ACQIDLE":
                            descriptor = "IDLE";
                            break;
                        case "ACQPARAMS":
                            descriptor = "RECEIVED";
                            break;
                        case "ACQREADY":
                            descriptor = "READY";
                            break;
                        case "ACQPREPARED":
                            descriptor = "PREPARED";
                            break;
                        case "ACQSTARTED":
                            descriptor = "STARTED";
                            break;
                        default:
                            descriptor = "ERROR";
                            send = false;
                            break;                        
                    }
                    if(send)
                        SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, descriptor, arguments);
                }

                    /*SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "IDLE", arguments);
                    //streamWriter.WriteLine("IDLE");
                    //Console.WriteLine("IDLE");
                    //streamWriter.Flush();

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "RECEIVED", arguments);
                    //streamWriter.WriteLine("RECEIVED");
                    //Console.WriteLine("RECEIVED");
                    //streamWriter.Flush();

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "READY", arguments);
                    //streamWriter.WriteLine("READY");
                    //Console.WriteLine("READY");
                    //streamWriter.Flush();

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "PREPARED", arguments);
                    //streamWriter.WriteLine("PREPARED");
                    //Console.WriteLine("PREPARED");
                    //streamWriter.Flush();

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    message = streamReader.ReadLine();
                    Console.WriteLine(message);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "STARTED", arguments);
                    //streamWriter.WriteLine("STARTED");
                    //Console.WriteLine("STARTED");
                    //streamWriter.Flush();           

                */
                    streamReader.Close();
                    networkStream.Close();
                    streamWriter.Close();
            }
            socketForClient.Close();
            Console.WriteLine("Exiting...");
        }
    }
}