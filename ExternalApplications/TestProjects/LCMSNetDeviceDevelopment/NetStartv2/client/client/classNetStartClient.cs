using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

namespace LCMSNet.Devices
{
    /// <summary>
    /// Network Start using old command packing messaging for communication with mass spectrometer.
    /// </summary>
    public class classNetStartClient
    {
        #region Members
        /// <summary>
        /// Minimum timeout for receiving data in seconds.
        /// </summary>
        private const int CONST_MIN_TIMEOUT_RECEIVE = 1;
        /// <summary>
        /// Query string for getting the method names.
        /// </summary>
        private const string CONST_QUERY_GETMETHODNAMES = "METHODNAMES";
        /// <summary>
        /// Minimum timeout for sending data in seconds.
        /// </summary>
        private const int CONST_MIN_TIMEOUT_SEND    = 1;
        /// <summary>
        /// Network socket / stream for connecting to instrument.
        /// </summary>
        private TcpClient       mobj_socketForServer;
        /// <summary>
        /// Stream reader to read underlying network stream data.
        /// </summary>
        private StreamReader    mobj_reader;
        /// <summary>
        /// Stream writer for writing to underlying network stream data.
        /// </summary>
        private StreamWriter    mobj_writer;
        /// <summary>
        /// Network stream for establishing connections.
        /// </summary>
        private NetworkStream   mobj_networkstream;
        #endregion


        
        #region Properties 
        /// <summary>
        /// Gets or sets the send timeout for the socket.
        /// </summary>
        public int SendTimeout { get; set; }
        /// <summary>
        /// Gets or sets the receive timeout for the socket.
        /// </summary>
        public int ReceiveTimeout { get; set; }
        #endregion

        #region Connection Methods
        /// <summary>
        /// Connects to the server (instrument).
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public bool Connect(string server, int port)
        {
            try
            {
                mobj_socketForServer = new TcpClient(server, port);

                mobj_socketForServer.SendTimeout = Math.Max(SendTimeout, CONST_MIN_TIMEOUT_SEND);
                mobj_socketForServer.ReceiveTimeout = Math.Max(ReceiveTimeout, CONST_MIN_TIMEOUT_RECEIVE);

                mobj_networkstream = mobj_socketForServer.GetStream();

                mobj_reader = new System.IO.StreamReader(mobj_networkstream);
                mobj_writer = new System.IO.StreamWriter(mobj_networkstream);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Disconnects from instrument if connected. 
        /// </summary>
        /// <returns>False if not connected previously or disconnect failed.</returns>
        public bool Disconnect()
        {
            try
            {
                if (mobj_networkstream != null)
                {
                    mobj_networkstream.Close();
                    return true;
                }
            }
            catch
            {
               
            }
            return false;
        }
        #endregion


        public List<string> GetMethods()
        {
            List<classNetStartArgument> arguments = new List<classNetStartArgument>();
            SendMessage(mobj_writer, enumNetStartMessageTypes.Query, 0, CONST_QUERY_GETMETHODNAMES, arguments);

            string rawMessage               = mobj_reader.ReadLine();
            classNetStartMessage message    = UnpackMessage(rawMessage);


            List<string> methods = new List<string>();
            foreach (classNetStartArgument argument in message.ArgumentList)
            {
                methods.Add(argument.Value);
            }
            return methods;
        }

        #region Method Packing / Sending and Unpacking
        /// <summary>
        /// Sends a message to the instrument.
        /// </summary>
        /// <param name="streamWriter">Stream to send message across.</param>
        /// <param name="type">Type of message to send (e.g. query or post.)</param>
        /// <param name="sequence">Message Sequence Number</param>
        /// <param name="descriptor">Message Description (e.g. ACQSTARTED...)</param>
        /// <param name="arglist">Arguments to send with message (e.g. sample name or method name).</param>
        private void SendMessage(System.IO.StreamWriter streamWriter, enumNetStartMessageTypes type, int sequence, string descriptor, List<classNetStartArgument> arglist)
        {
            //Console.WriteLine(descriptor);

            string message = PackMessage(type, sequence, descriptor, arglist);    

            streamWriter.WriteLine(message);
            Console.WriteLine("Send: " + message);
            streamWriter.Flush();
        }
        private string PackMessage(enumNetStartMessageTypes type, int sequence, string descriptor, List<classNetStartArgument> arglist)
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
        /// <summary>
        /// Unpacks the EOL packing string into an object that can be deciphered.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private classNetStartMessage UnpackMessage(string message)
        {
            //<type>:<sqnc>|<dscp>@<argS1>=<argV1>@<argS1>=<argV1>...@<argSn>=<argVn>

            classNetStartMessage msg = new classNetStartMessage();
            List<classNetStartArgument> args = new List<classNetStartArgument>();
            char[] tokens = {':','@','|','='};
            string[] messagepieces = message.Split(tokens);

            msg.Type = (enumNetStartMessageTypes)Enum.Parse(typeof(enumNetStartMessageTypes), messagepieces[0]);
            msg.Sequence = Int32.Parse(messagepieces[1]);
            msg.Descriptor = messagepieces[2];

            for (int i = 3; i < messagepieces.Count()-1; i+=2)
            {
                args.Add(new classNetStartArgument(messagepieces[i], messagepieces[i+1]));
            }

            msg.ArgumentList = args;

            return msg;
        }
        #endregion

        #region Acquisition Start/Stop methods.
        /// <summary>
        /// Starts instrument acquisition.
        /// </summary>
        /// <param name="sampleName">Name of sample to run.</param>
        /// <param name="methodName">Instrument method to use.</param>
        /// <returns>True if start successful.  False if start failed for any reason.</returns>
        public bool StartAcquisition(string sampleName, string methodName)
        {
            StreamReader streamReader               = mobj_reader;
            StreamWriter streamWriter               = mobj_writer;
            List<classNetStartArgument> arguments   = new List<classNetStartArgument>();
            classNetStartMessage receivedMessage    = new classNetStartMessage();
            
            bool success = false;
            
            try
            {
                string outputString;
                int i = 0;
                {
                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, i++, "ACQIDLE", arguments);

                    outputString = streamReader.ReadLine();     //IDLE
                    
                    if (UnpackMessage(outputString).Descriptor == "ACQIDLE")
                    {
                        arguments.Add(new classNetStartArgument("Method", methodName));
                        arguments.Add(new classNetStartArgument("SampleName", sampleName));
                        SendMessage(streamWriter, enumNetStartMessageTypes.Post, i++, "ACQPARAMS", arguments);
                        arguments.Clear();

                        outputString = streamReader.ReadLine();     //RECEIVED
                        
                        /// 
                        /// Now see if the system is ready
                        /// 
                        SendMessage(streamWriter, enumNetStartMessageTypes.Query, i++, "ACQREADY", arguments);
                        outputString = streamReader.ReadLine();     //READY
                                                
                        if (UnpackMessage(outputString).Descriptor == "ACQREADY")
                        {

                            ///
                            /// Tell the system to prepare for acquisition
                            /// 
                            SendMessage(streamWriter, enumNetStartMessageTypes.Post, i++, "ACQPREPARE", arguments);
                            outputString = streamReader.ReadLine();     // Read off auto-response 
                            
                            /// 
                            /// Then ask if it is prepared...this should be in some kind of loop
                            /// 
                            {
                                SendMessage(streamWriter, enumNetStartMessageTypes.Query, i++, "ACQPREPARED", arguments);

                                /// 
                                /// Check to see if it is prepared...
                                /// 
                                outputString = streamReader.ReadLine();     // Read off response for PREPARED
                                classNetStartMessage preparedMessage = UnpackMessage(outputString);
                                if (preparedMessage.ArgumentList.Count > 0 && preparedMessage.ArgumentList[0].Value.ToUpper() == "TRUE")
                                {
                                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, i++, "ACQSTART", arguments);
                                    string startResponse = mobj_reader.ReadLine();
                    
                                    SendMessage(streamWriter, enumNetStartMessageTypes.Query, i++, "ACQSTARTED", arguments);
                                    outputString = streamReader.ReadLine();     //STARTED

                                    classNetStartMessage startedMessage = UnpackMessage(outputString);
                                    if (startedMessage.Descriptor.ToUpper() == "ACQSTARTED" && startedMessage.ArgumentList.Count > 0 && startedMessage.ArgumentList[0].Value.ToUpper() == "TRUE")
                                        success = true;
                                }
                            }
                        }                        
                    }                    
                }
            }
            catch
            {
                Console.WriteLine(":(");
            }

            return success;
        }
        /// <summary>
        /// Stops instrument acquisition.
        /// </summary>
        public void StopAcquisition()
        {
            System.IO.StreamReader streamReader = mobj_reader;
            System.IO.StreamWriter streamWriter = mobj_writer;
            List<classNetStartArgument> arguments = new List<classNetStartArgument>();
            classNetStartMessage receivedMessage = new classNetStartMessage();

            try
            {
                string outputString;
                int i = 0;
                {
                    SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "STOPACQ", arguments);

                    outputString = streamReader.ReadLine();     //IDLE
                    Console.WriteLine("Rec: " + outputString);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        #endregion
    }
}
