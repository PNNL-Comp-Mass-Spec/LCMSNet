//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LcmsNetSDK.Logging;

namespace LcmsNetPlugins.Bruker
{
    /// <summary>
    /// Tools for handling Bruker sXc messages
    /// </summary>
    class classBrukerMsgTools : IDisposable
    {
        #region "Constants"
        const int DEFAULT_TIME_OFFSET = 0;
        const int MAX_ZERO_BYTE_RESPONSE_COUNT = 4;
        #endregion

        #region "Class variables"
        Socket mobject_Socket;
        readonly byte[] mbyte_DataBuffer = new byte[128];
        IAsyncResult mobject_AsyncResult;
        AsyncCallback mobjecct_RcxCallback;
        readonly Queue<byte> mobject_IncomingBytes = new Queue<byte>();
        classFtmsResponse mobject_ResponseData;
        bool m_Connected;
        string m_OutputFolderLocal;
        string m_MethodFolderLocal;
        string m_InstAddress;
        int m_InstPort;
        string m_Msg;
        bool m_ListenToSxc = true;
        int m_ZeroByteMsgCount;
        #endregion

        #region "Delegats"
        private delegate void delegateOnDataReceived();
        #endregion

        #region "Events"
        private event delegateOnDataReceived DataReceived;
        public event delegateBrukerMsgReceived BrukerMsgReceived;
        #endregion

        #region "Properties"
        /// <summary>
        /// 
        /// </summary>
        public bool SocketConneted => m_Connected;

        /// <summary>
        /// 
        /// </summary>
        public string Msg => m_Msg;

        /// <summary>
        /// Gets or sets the IP Address
        /// </summary>
        public string IPAddress
        {
            get
            {
                return m_InstAddress;
            }
            set
            {
                m_InstAddress = value;
            }
        }
        /// <summary>
        /// Gets or sets the Port
        /// </summary>
        public int Port
        {
            get
            {
                return m_InstPort;
            }
            set
            {
                m_InstPort = value;
            }
        }
        #endregion

        #region "Constructors"
        public classBrukerMsgTools(string outFolderLocal, string methodFolderLocal, string instAddress, int instPort)
        {
            m_OutputFolderLocal = outFolderLocal;
            m_MethodFolderLocal = methodFolderLocal;
            m_InstAddress = instAddress;
            m_InstPort = instPort;

            DataReceived += sXcDataReceived;
        }
        #endregion

        #region "Methods"
        /// <summary>
        /// Connects to the socket used by sXc on the Bruker instrument
        /// </summary>
        /// <returns>TRUE for success, FALSE otherwise</returns>
        public bool ConnectSxcSocket()
        {
            // Check to see if socket is already connected
            if ((mobject_Socket != null) || (m_Connected))
            {
                m_Msg = "classBrukerMsgTools.ConnectSXC: sXc socket already exists or in use";
                return false;
            }

            try
            {
                mobject_Socket = ConnectSocket(m_InstAddress, m_InstPort);
                if (mobject_Socket != null)
                {
                    m_Connected = true;
                    m_ListenToSxc = true;
                    return true;
                }

                m_Msg = "classBrukerMsgTools: Socket not connected";
                m_Connected = false;
                mobject_Socket = null;
                return false;
            }
            catch (Exception ex)
            {
                m_Msg = "classBrukerMsgTools: Exception making socket connection: " + ex.Message;
                mobject_Socket = null;
                m_Connected = false;
                return false;
            }
        }

        /// <summary>
        /// Sends INIT_FTMS command to sXc
        /// </summary>
        /// <returns>TRUE for success, FALSE otherwise</returns>
        public bool InitFTMS()
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.InitFTMS: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_INITFTMS;
            if (!SendShortInt(codeToSend))
                return false;
            // Send parameter
            codeToSend = classBrukerComConstants.PARAM_INITFTMS_ESI;
            if (!SendShortInt(codeToSend))
                return false;

            return true;
        }

        /// <summary>
        /// Sends the sampele info string to Bruker sXc
        /// </summary>
        /// <param name="sampleXML">XML string to send</param>
        /// <returns>TRUE for success; otherwise FALSE</returns>
        public bool SendSampleInfo(string sampleXML)
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.SendSampleInfo: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_PROCESSLCMSSAMPLEINFORMATION;
            if (!SendShortInt(codeToSend))
                return false;
            // Send XML data
            if (!SendString(sampleXML))
                return false;

            return true;
        }

        /// <summary>
        /// Sends PREPARE_ACQUISITION command to Bruker sXc
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool PrepareAcquisition()
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.PrepareAcquisition: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_STARTSTOPABORTFTMS;
            if (!SendShortInt(codeToSend))
                return false;
            // Send parameter
            codeToSend = classBrukerComConstants.PARAM_STARTSTOP_PREPARE;
            if (!SendShortInt(codeToSend))
                return false;

            return true;
        }

        /// <summary>
        /// Sends START_ACQUISITION command to Bruker sXc
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StartAcquisition()
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.StartAcquisition: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_STARTACQUISITION;
            if (!SendShortInt(codeToSend))
                return false;
            // Send parameter
            var intCodeToSend = DEFAULT_TIME_OFFSET;
            if (!SendInt(intCodeToSend))
                return false;

            return true;
        }

        /// <summary>
        /// Sends FINISH_ACQUISITION command to Bruker sXc
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool FinishAcq()
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.FinishAcq: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_STARTSTOPABORTFTMS;
            if (!SendShortInt(codeToSend))
                return false;
            // Send parameter
            codeToSend = classBrukerComConstants.PARAM_STARTSTOP_FINISH;
            if (!SendShortInt(codeToSend))
                return false;

            return true;
        }

        /// <summary>
        /// Sends ABORT_ACQUISTION command to Bruker sXc (not implemented yet)
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool AbortAcq()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends EXIT_FTMS command to Bruker sXc
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool ExitFTMS()
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.ExitFTMS: Socket not connected";
                return false;
            }

            // Send command
            var codeToSend = classBrukerComConstants.SOCKET_EXITFTMS;
            if (!SendShortInt(codeToSend))
                return false;
            // Send parameter
            codeToSend = 0;
            if (!SendShortInt(codeToSend))
                return false;

            ApplicationLogger.LogMessage(2, "ExitFTMS(): Sent EXIT_FTMS command");

            return true;
        }

        /// <summary>
        /// Disconnect the sXc socket connection
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool DisconnectSXC()
        {
            ApplicationLogger.LogMessage(2, "DisconnectSXC() starting");

            // Verify socket isn't already disconnected
            if ((mobject_Socket == null) || (!m_Connected))
            {
                m_Msg = "classBrukerMsgTools.DisconnectSXC: Socket already disconnected";
                mobject_Socket = null;
                m_Connected = false;
                return false;
            }

            mobject_Socket.Disconnect(false);
            mobject_Socket = null;
            m_Connected = false;

            ApplicationLogger.LogMessage(2, "DisconnectSXC() socket disconnected");
            return true;
        }

        /// <summary>
        /// Starts listening for incoming data on the network port
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StartListeningToSXC()
        {
            // Log program location for debugging
            ApplicationLogger.LogMessage(2, "StartListeningToSXC()");

            if (!m_ListenToSxc)
            {
                ApplicationLogger.LogMessage(2, "sXc listening disabled");
                return false;
            }

            ApplicationLogger.LogMessage(2, "sXc listening enabled");

            if (mobjecct_RcxCallback == null)
                mobjecct_RcxCallback = OnDataReceived;
            mobject_AsyncResult = mobject_Socket.BeginReceive(mbyte_DataBuffer, 0, mbyte_DataBuffer.Length, SocketFlags.None, mobjecct_RcxCallback, null);
            return true;
        }

        /// <summary>
        /// Stop listening for incoming data on the network port
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StopListeningToSXC()
        {
            ApplicationLogger.LogMessage(2, "StopListeningToSXC()");
            m_ListenToSxc = false;

            //TODO: Does something else need to be done here?
            return true;
        }

        /// <summary>
        /// Creates a network socket connection
        /// </summary>
        /// <param name="server">Name of machine to connect to (aka Bruker instrument) in DNS</param>
        /// <param name="port">Port number</param>
        /// <returns>Socket object if successful, otherwise NULL</returns>
        private Socket ConnectSocket(string server, int port)
        {
            // Code for this method derived from "C# / CSharp Tutorial" sample code at
            // http://www.java2s.com/Tutorial/CSharp/0300__File-Directory-Stream/CopyDirectory.htm

            Socket s = null;

            // Get host related information.
            var hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (var address in hostEntry.AddressList)
            {
                var ipe = new IPEndPoint(address, port);
                var tempSocket =
                     new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    mobject_ResponseData = new classFtmsResponse();
                    break;
                }
            }

            return s;
        }

        /// <summary>
        /// Sends a short integer to Bruker sXc
        /// </summary>
        /// <param name="shortData">Short integer to send</param>
        /// <returns>TRUE for success, FALSE otherwise</returns>
        private bool SendShortInt(short shortData)
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.SendShortInt: Socket not connected";
                return false;
            }

            var bytesToSend = BitConverter.GetBytes(shortData);

            // Log outgoing bytes for debugging purposes
            LogOutgoingBytes(bytesToSend, bytesToSend.Length);

            try
            {
                mobject_Socket.Send(bytesToSend, bytesToSend.Length, 0);
                return true;
            }
            catch (Exception ex)
            {
                m_Msg = "classBrukerMsgTools.SendShortInt: Exception sending command " + shortData.ToString() + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Sends a string to Bruker sXc
        /// </summary>
        /// <param name="msgString">String to send</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        private bool SendString(string msgString)
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.SendString: Socket not connected";
                return false;
            }

            var strBytesToSend = Encoding.Unicode.GetBytes(msgString);
            var byteCount = strBytesToSend.Length;
            var byteCountToSend = BitConverter.GetBytes(byteCount);

            // Log outgoing bytes
            LogOutgoingBytes(byteCountToSend, byteCountToSend.Length);
            LogOutgoingBytes(strBytesToSend, strBytesToSend.Length);

            try
            {
                mobject_Socket.Send(byteCountToSend, byteCountToSend.Length, 0);
                mobject_Socket.Send(strBytesToSend, strBytesToSend.Length, 0);
                return true;
            }
            catch (Exception ex)
            {
                m_Msg = "classBrukerMsgTools.SendString: Exception sending command " + msgString + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Sends an integer to Bruker sXc
        /// </summary>
        /// <param name="intData">Data to send</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        private bool SendInt(int intData)
        {
            // Verify socket is connected
            if (!m_Connected)
            {
                m_Msg = "classBrukerMsgTools.SendInt: Socket not connected";
                return false;
            }

            var bytesToSend = BitConverter.GetBytes(intData);

            // Log outgoing bytes for debug purposes
            LogOutgoingBytes(bytesToSend, bytesToSend.Length);

            try
            {
                mobject_Socket.Send(bytesToSend, bytesToSend.Length, 0);
                return true;
            }
            catch (Exception ex)
            {
                m_Msg = "classBrukerMsgTools.SendInt: Exception sending command " + intData.ToString() + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Retrieves a short integer from the queue of received bytes
        /// </summary>
        /// <returns></returns>
        private short GetShortIntFromRcvQueue()
        {
            // Pop the first two bytes off the queue and convert to a short int
            var tmpByte1 = mobject_IncomingBytes.Dequeue();
            var tmpByte2 = mobject_IncomingBytes.Dequeue();
            byte[] tmpByteArray = { tmpByte1, tmpByte2 };
            return BitConverter.ToInt16(tmpByteArray, 0);
        }

        /// <summary>
        /// Processes data recevied from Bruker sXc
        /// </summary>
        /// <param name="responseData">Class holding sXc data</param>
        private void ProcessResponse(ref classFtmsResponse responseData)
        {
            // Log program location for debugging purposes
            ApplicationLogger.LogMessage(2, "ProcessResponse: m_IncomingBytes.Count = " + mobject_IncomingBytes.Count.ToString());

            if (mobject_IncomingBytes.Count > 1)
            {
                //TODO: Figure out how to log this, if necessary
                //clsLogTools.LogDebugMsg("Label 1");
                //clsLogTools.LogBytes(m_IncomingBytes.ToArray(), m_IncomingBytes.Count, "ProcessResponse: m_IncomingBytes content");
                var tmpRcvdValue = GetShortIntFromRcvQueue();

                // If command code already received, this is the parm code; otherwise it's the command code
                ApplicationLogger.LogMessage(2, "ProcessResponse: tmpRcvdValue = " + tmpRcvdValue.ToString());
                ApplicationLogger.LogMessage(2, "ProcessResponse: responseData.WaitingForParam = " + responseData.WaitingForParam.ToString());

                short tmpResp;
                classBrukerComConstants.SxcReplies tmpReply;
                if (responseData.WaitingForParam)
                {
                    //TODO: Figure out how to log this, if necessary
                    //clsLogTools.LogDebugMsg("Label 2");
                    responseData.ParamCode = tmpRcvdValue;
                    responseData.WaitingForParam = false;
                    // Since this was the param code, notify the world that a value was received
                    tmpResp = (short)(responseData.ParamCode + 20);
                    tmpReply = classBrukerComConstants.ConvertShortToSxcReply(tmpResp);

                    ApplicationLogger.LogMessage(2, "ProcessResponse: Message with param = " + tmpReply.ToString());

                    ApplicationLogger.LogMessage(2, "ProcessResponse: Firing BrukerMsgReceived event");
                    BrukerMsgReceived?.Invoke(tmpReply);
                    ApplicationLogger.LogMessage(2, "ProcessResponse: BrukerMsgReceived event handling complete");
                    return;
                }
                
                //TODO: Figure out how to log this, if necessary
                //clsLogTools.LogDebugMsg("Label 3");
                responseData.CommandCode = tmpRcvdValue;

                // Handle the incoming code if it's not the parameter for an earlier status message
                ApplicationLogger.LogMessage(2, "ProcessResponse: responseData.CommandCode = " + responseData.CommandCode.ToString());
                if (responseData.CommandCode == classBrukerComConstants.SOCKET_REQUEST_CHECKMSSTATUS)
                {
                    //TODO: Figure out how to log this, if necessary
                    //clsLogTools.LogDebugMsg("Label 4");
                    if (mobject_IncomingBytes.Count > 1)
                    {
                        //TODO: Figure out how to log this, if necessary
                        //clsLogTools.LogDebugMsg("Label 5");
                        tmpRcvdValue = GetShortIntFromRcvQueue();
                        responseData.ParamCode = tmpRcvdValue;
                        responseData.WaitingForParam = false;
                        // We've received the whole message, so notify the world
                        tmpResp = (short)(responseData.ParamCode + 20);
                        tmpReply = classBrukerComConstants.ConvertShortToSxcReply(tmpResp);
                        ApplicationLogger.LogMessage(2, "ProcessResponse: Message with param = " + tmpReply.ToString());
                        BrukerMsgReceived?.Invoke(tmpReply);
                        return;
                    }
                    
                    // There weren't enough bytes left in the queue, so wait for more incoming data
                    // This may be dangerous, since it relies on WaitForData to exit before OnDataReceived is triggered
                    //TODO: Figure out how to log this, if necessary
                    //clsLogTools.LogDebugMsg("Label 6");
                    responseData.WaitingForParam = true;
                    return;
                }
                // This was a parameterless response, so just send out a notification
                //TODO: Figure out how to log this, if necessary
                //clsLogTools.LogDebugMsg("Label 7");
                tmpResp = (short)(responseData.ParamCode + 20);
                tmpReply = classBrukerComConstants.ConvertShortToSxcReply(tmpResp);
                ApplicationLogger.LogMessage(2, "ProcessResponse: Message with param = " + tmpReply.ToString());
                BrukerMsgReceived?.Invoke(tmpReply);
                return;
            }
            
            // This shouldn't ever happen (and pigs can fly!)
            //TODO: Figure out how to log this, if necessary
            //clsLogTools.LogDebugMsg("Label 8");
            //System.Windows.Forms.MessageBox.Show("ProcessResponse Outer Loop: Invalid byte count received: " +
            //  m_IncomingBytes.Count.ToString());
            
        }

        /// <summary>
        /// Logs raw data bytes received from Bruker
        /// </summary>
        /// <param name="dataBuffer">Incoming data byte array</param>
        /// <param name="byteCount">Number of bytes received</param>
        private void LogIncomingBytes(byte[] dataBuffer, int byteCount)
        {
            var outStrBld = new StringBuilder();
            outStrBld.Append("Data bytes received from Bruker: ");
            for (var byteIndx = 0; byteIndx < byteCount; byteIndx++)
            {
                outStrBld.Append(dataBuffer[byteIndx].ToString() + ",");
            }

            ApplicationLogger.LogMessage(2, outStrBld.ToString());
        }

        /// <summary>
        /// Logs raw bytes being sent to Bruker
        /// </summary>
        /// <param name="dataBuffer">Outgoing data byte array</param>
        /// <param name="byteCount">Number of bytes being sent</param>
        private void LogOutgoingBytes(byte[] dataBuffer, int byteCount)
        {
            var outStrBld = new StringBuilder();
            outStrBld.Append("Data bytes sent to Bruker: ");
            for (var byteIndx = 0; byteIndx < byteCount; byteIndx++)
            {
                outStrBld.Append(dataBuffer[byteIndx].ToString() + ",");
            }

            ApplicationLogger.LogMessage(2, outStrBld.ToString());
        }

        #endregion

        #region "Event handlers"
        /// <summary>
        /// Processes bytes held in the incoming message queue
        /// </summary>
        private void sXcDataReceived()
        {
            ApplicationLogger.LogMessage(2, "classBrukerMsgTools.sXcDataReceived: Handling DataReceived event");
            while (mobject_IncomingBytes.Count > 0)
            {
                ProcessResponse(ref mobject_ResponseData);
            }
        }

        /// <summary>
        /// Processes bytes received at the network port
        /// </summary>
        /// <param name="syncResult"></param>
        private void OnDataReceived(IAsyncResult syncResult)
        {
            System.Threading.Thread.CurrentThread.Name = "OnDataReceived";

            // Log location in program for debugging
            ApplicationLogger.LogMessage(2, "classBrukerMsgTools.OnDataReceived()");

            SocketError myError;

            // If we're not listening anymore, just exit
            if (!m_ListenToSxc)
                return;

            var bytesReceived = mobject_Socket.EndReceive(syncResult, out myError);

            if (bytesReceived > 0)
            {
                // Log the incoming data
                ApplicationLogger.LogMessage(2, "classBrukerMsgTools.OnDataReceived: Byte count = " + bytesReceived.ToString());
                LogIncomingBytes(mbyte_DataBuffer, bytesReceived);

                // Add incoming data to byte queue
                for (var byteIndx = 0; byteIndx < bytesReceived; byteIndx++)
                {
                    mobject_IncomingBytes.Enqueue(mbyte_DataBuffer[byteIndx]);
                }
                m_ZeroByteMsgCount = 0;
            }
            else
            {
                m_ZeroByteMsgCount++;
                ApplicationLogger.LogMessage(2, "classBrukerMsgTools.OnDataReceived: 0-byte message received from sXc");
            }

            // Check to see if multiple consecutive 0-byte responses have been received (indicates sXc probably disconnected)
            if (m_ZeroByteMsgCount > MAX_ZERO_BYTE_RESPONSE_COUNT)
            {
                m_Msg = "classBrukerMsgTools.OnDataReceived: Excessive consecutive 0-byte messages. Disabling receive";
                ApplicationLogger.LogMessage(2, m_Msg);
                m_ListenToSxc = false;
            }

            ApplicationLogger.LogMessage(2, "classBrukerMsgTools.OnDataReceived: Firing DataReceived event");
            DataReceived?.Invoke();
            ApplicationLogger.LogMessage(2, "classBrukerMsgTools.OnDataReceived: DataReceived event handling complete");

            System.Threading.Thread.Sleep(20);

            StartListeningToSXC();
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
        }
        #endregion
    }
}
