using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;


namespace client
{
    public class clsNetStartClient
    {
        private TcpClient mobj_socketForServer;
        private System.IO.StreamReader mobj_reader;
        private System.IO.StreamWriter mobj_writer;
        private NetworkStream mobj_networkstream;

        public void Connect(string server, int port)
        {
            try
            {                
                mobj_socketForServer = new TcpClient(server, port);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to connect. " + ex.Message);
                return;
            }

            mobj_networkstream = mobj_socketForServer.GetStream();
            mobj_reader = new System.IO.StreamReader(mobj_networkstream);
            mobj_writer = new System.IO.StreamWriter(mobj_networkstream);
        }

        public void Disconnect()
        {
            mobj_networkstream.Close();
        }


        private void SendMessage(System.IO.StreamWriter streamWriter, enumNetStartMessageTypes type, string sequence, string descriptor, clsNetStartArgument[] argtable)
        {
            Console.WriteLine(descriptor);

            string message = type.GetHashCode().ToString() + ":" + sequence + "|" + descriptor;
            if (type == enumNetStartMessageTypes.Post || type == enumNetStartMessageTypes.Response || type == enumNetStartMessageTypes.Execute)
            {
                foreach (clsNetStartArgument arg in argtable)
                {
                    message += "@" + arg.Key + "=" + arg.Value;
                }
            }
            else if (type == enumNetStartMessageTypes.Query)
            {
                foreach (clsNetStartArgument arg in argtable)
                {
                    message += "@" + arg.Key;
                }
            }

            streamWriter.WriteLine(message);
            streamWriter.Flush();
        }

        public void GetMethods()
        {
            clsNetStartArgument [] arguments = new clsNetStartArgument[0];
            SendMessage(mobj_writer, enumNetStartMessageTypes.Query, "0", "METHODNAMES", arguments);

            string response = mobj_reader.ReadLine();
            Console.WriteLine(response);

            int x = 9;
        }

        public void Handshake()
        {
            System.IO.StreamReader streamReader = mobj_reader;
            System.IO.StreamWriter streamWriter = mobj_writer;
            clsNetStartArgument[] arguments = new clsNetStartArgument[0];
            try
            {
                string outputString;
                {
                    outputString = streamReader.ReadLine();     //CONNECTED
                    Console.WriteLine(outputString);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, "", "ACQIDLE", arguments);

                    outputString = streamReader.ReadLine();     //IDLE
                    Console.WriteLine(outputString);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, "", "ACQPARAMS", arguments);

                    outputString = streamReader.ReadLine();     //RECEIVED
                    Console.WriteLine(outputString);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, "", "ACQREADY", arguments);

                    outputString = streamReader.ReadLine();     //READY
                    Console.WriteLine(outputString);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, "", "ACQPREPARE", arguments);

                    System.Threading.Thread.Sleep(100);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, "", "ACQPREPARED", arguments);

                    outputString = streamReader.ReadLine();     //PREPARED
                    Console.WriteLine(outputString);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, "", "ACQSTART", arguments);

                    System.Threading.Thread.Sleep(100);

                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, "", "ACQSTARTED", arguments);

                    outputString = streamReader.ReadLine();     //STARTED
                    Console.WriteLine(outputString);

                }
            }
            catch
            {
                Console.WriteLine(":(");
            }

        }
    }
}
