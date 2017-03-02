using System;
using System.IO;
using System.Xml;
using System.Net.Sockets;
using System.Collections.Generic;


using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

using LcmsNet.Devices;

namespace LcmsNet.Devices.NetworkStart.Socket
{
    /// <summary> 
    /// Network Start using old command packing messaging for communication with mass spectrometer.
    /// </summary>
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(controlNetStart),
                                 
                                 "Network Start",
                                 "Detectors")
    ]
    public class classNetStartSocket: IDevice
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
        /// Port of the server instrument.
        /// </summary>
        private const int CONST_SERVER_PORT = 4771;
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
        /// <summary>
        /// The name of the port (e.g. "COM1") used for communication with the Agilent pumps
        /// </summary>
        private string mstring_address;
        /// <summary>
        /// Port of server instrument.
        /// </summary>
        private int mint_port;
        /// <summary>
        /// The device's name.
        /// </summary>
        private string mstring_name;
        /// <summary>
        /// The device's verion.
        /// </summary>
        private string mstring_version;
        /// <summary>
        /// Status of the device.
        /// </summary>
        private enumDeviceStatus menum_status;
        #endregion

        #region Events
        /// <summary>
        /// Fired when the status changes for the device.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;
        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classNetStartSocket()
        {
            mstring_address = "localhost";
            mstring_name = "network start"; // classDeviceManager.Manager.CreateUniqueDeviceName("networkStart");
            mstring_version = "May-2010";
            mint_port       = CONST_SERVER_PORT;
            menum_status    = enumDeviceStatus.NotInitialized;

            AbortEvent      = new System.Threading.ManualResetEvent(false);
            Emulation       = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the send timeout for the socket.
        /// </summary>
        [classPersistenceAttribute("SendTimeout")]
        public int SendTimeout { get; set; }
        /// <summary>
        /// Gets or sets the receive timeout for the socket.
        /// </summary>
        [classPersistenceAttribute("ReceiveTimeout")]
        public int ReceiveTimeout { get; set; }
        /// <summary>
        /// Gets or sets the IP address or DNS name of the server instrument.
        /// </summary>
        [classPersistenceAttribute("IPAddress")]
        public string Address
        {
            get
            {
                return mstring_address;
            }
            set
            {
                mstring_address = value;
            }
        }
        /// <summary>
        /// Gets or sets the port used to connect to the server.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public int Port
        {
            get
            {
                return mint_port;
            }
            set
            {
                mint_port = value;
            }
        }
        #endregion

        #region Connection Methods
        /// <summary>
        /// Connects to the server (instrument).
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public bool Connect(string server, int port)
        {
            bool connected = false;

            if (Emulation == true)
                return true;

            try
            {
                mobj_socketForServer = new TcpClient(server, port);

                mobj_socketForServer.SendTimeout = Math.Max(SendTimeout, CONST_MIN_TIMEOUT_SEND);
                mobj_socketForServer.ReceiveTimeout = Math.Max(ReceiveTimeout, CONST_MIN_TIMEOUT_RECEIVE);

                mobj_networkstream = mobj_socketForServer.GetStream();

                mobj_reader = new System.IO.StreamReader(mobj_networkstream);
                mobj_writer = new System.IO.StreamWriter(mobj_networkstream);
                connected = true;
            }
            catch (Exception ex)
            {
                HandleError("Could not connect to instrument.", ex);
            }
            return connected;
        }
        /// <summary>
        /// Disconnects from instrument if connected.
        /// </summary>
        /// <returns>False if not connected previously or disconnect failed.</returns>
        public bool Disconnect()
        {
            try
            {
                if (Emulation == true)
                    return true;

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

        #region Methods
        /// <summary>
        /// Returns a list of methods stored on the instrument.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMethods()
        {
            List<string> methods = new List<string>();
            if (Emulation == true)
            {
                methods.Add("Dummy-Instrument-01");
                methods.Add("Dummy-Instrument-02");
                methods.Add("Dummy-Instrument-03");
                methods.Add("Dummy-Instrument-04");
            }
            else
            {
                try
                {
                    List<classNetStartArgument> arguments = new List<classNetStartArgument>();
                    SendMessage(mobj_writer, enumNetStartMessageTypes.Query, 0, CONST_QUERY_GETMETHODNAMES, arguments);

                    string rawMessage = mobj_reader.ReadLine();
                    classNetStartMessage message = UnpackMessage(rawMessage);


                    foreach (classNetStartArgument argument in message.ArgumentList)
                    {
                        methods.Add(argument.Value);
                    }
                }
                catch(Exception ex)
                {
                    HandleError("Could not retrieve the methods from the instrument.", ex);
                }
            }
            
            /// 
            /// Alert listeners that we have new methods!
            /// 
            if (MethodNames != null)
            {
                List<object> methodObjects = new List<object>();
                foreach (string method in methods)
                    methodObjects.Add(method);

                MethodNames(this, methodObjects);
            }

            return methods;
        }
        #endregion

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

            for (int i = 3; i < messagepieces.Length - 1; i+=2)
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
        [classLCMethodAttribute("Start Acquisition", enumMethodOperationTime.Parameter, true, 1, "MethodNames", 2, false)]
        public bool StartAcquisition(double timeout, classSampleData sample)
        {
            /// 
            /// Emulate starting an acquisition
            /// 
            if (Emulation == true)
            {
                return true;
            }

            List<classNetStartArgument> arguments   = new List<classNetStartArgument>();
            classNetStartMessage receivedMessage    = new classNetStartMessage();
            
            bool success = false;

            if (sample == null)
                return false;

            string methodName = sample.InstrumentData.MethodName;
            string sampleName = sample.DmsData.DatasetName;

            try
            {
                string outputString;
                int i = 0;
                Connect(mstring_address, mint_port);
                StreamReader streamReader = mobj_reader;
                StreamWriter streamWriter = mobj_writer;
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
                Disconnect();
            }
            catch (Exception ex)
            {
                HandleError("Could not start the acquisition.", ex);
                try
                {
                    Disconnect();
                }
                catch (Exception exDisconnect)
                {
                    HandleError("Could not disconnect from the server.", exDisconnect);
                }
            }
            finally
            {
                //TODO: Clean-up code to use disposable's
            }

            return success;
        }
        /// <summary>
        /// Stops instrument acquisition.
        /// </summary>
        [classLCMethodAttribute("Stop Acquisition", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool StopAcquisition(double delayTime)
        {
            DateTime startTime = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            /// 
            /// Emulate stopping an acquisition
            /// 
            if (Emulation == true)
            {
                return true;
            }

            Connect(mstring_address, mint_port);

            StreamReader streamReader = mobj_reader;
            StreamWriter streamWriter = mobj_writer;
            List<classNetStartArgument> arguments = new List<classNetStartArgument>();
            classNetStartMessage receivedMessage = new classNetStartMessage();

            bool success = false;
            try
            {
                string outputString;
                     SendMessage(streamWriter, enumNetStartMessageTypes.Post, 0, "ACQSTOP", arguments);
                outputString = streamReader.ReadLine();
                success = true;

            }
            catch(Exception ex)
            {
                HandleError("Could not stop the acquisition.", ex);
                success = false;
            }
            return success;
        }
        #endregion

        #region IDevice Properties
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get
            {
                return mstring_name;
            }
            set
            {
                mstring_name = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string Version
        {
            get
            {
                return mstring_version;
            }
            set
            {
                mstring_version = value;
            }
        }
        /// <summary>
        /// Gets or sets the Emulation state.
        /// </summary>
        //[classPersistenceAttribute("Emulated")]
        public bool Emulation
        {
            get;set;
        }
        /// <summary>
        /// Gets or sets the device's status.
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return menum_status;
            }
            set
            {
                if (value != menum_status && StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "None", this));
                menum_status = value;
            }
        }
        /// <summary>
        /// Gets or sets the abort event used to abort an I/O operation.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        #endregion

        #region IDevice Methods
        /// <summary>
        /// Initializes the device connecting to the server to get the instrument methods.
        /// </summary>
        /// <returns>True if it could connect, false if error occured.</returns>
        public bool Initialize(ref string errorMessage)
        {
            bool success = false;
            try
            {
                bool connected = Connect(mstring_address, mint_port);
                if (connected == false)
                {
                    errorMessage = "Could not connect to the remote instrument.";
                    return false;
                }
                GetMethods();
                connected  = Disconnect();
                if (connected == false)
                {
                    errorMessage = "Could not disconnect from the remote instrument.";
                    return false;
                }
                success = true;
            }
            catch (Exception ex)
            {
                errorMessage = "Could not initialize. " + ex.Message;
            }
            return success;
        }
        /// <summary>
        /// Shuts the connection to the instrument down if it exists.
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {
            //TODO: Shutdown for network start socket.
            return true;
        }
        /// <summary>
        /// Writes performance data to the directory provided.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
            //pass
        }
          public List<string> GetStatusNotificationList()
        {
            return new List<string>();
          }

          public List<string> GetErrorNotificationList()
          {
              return new List<string>();
          }
          #endregion

        #region Settings and Saving Methods
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
        }

        #region IDevice Data Provider Methods
        /// <summary>
        /// Registers the method with a data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames += remoteMethod;
                    GetMethods();
                    break;
            }
        }
        /// <summary>
        /// Unregisters the method from the data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames -= remoteMethod;
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        /// <summary>
        /// Handles the error internally for propogating to external error handling objects.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private void HandleError(string message, Exception ex)
        {
            if (Error != null)
            {
                Error(this, new classDeviceErrorEventArgs(message,
                                                 ex,
                                                 enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                                 this,
                                                 "None"));
            }
        }

        #region IDevice Members
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }
        #endregion
        
        /*/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FinchComponentData GetData()
        {
            FinchComponentData component    = new FinchComponentData();
            component.Status                = Status.ToString();
            component.Name                  = Name;
            component.Type                  = "Network Start";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurementSendTimeout  = new FinchScalarSignal();
            measurementSendTimeout.Name             = "Send Timeout";
            measurementSendTimeout.Type             = FinchDataType.Integer;
            measurementSendTimeout.Units            = "";
            measurementSendTimeout.Value            = this.SendTimeout.ToString();
            component.Signals.Add(measurementSendTimeout);

            FinchScalarSignal measurementReceiveTimeout   = new FinchScalarSignal();
            measurementReceiveTimeout.Name              = "Receive Timeout";
            measurementReceiveTimeout.Type              = FinchDataType.Integer;
            measurementReceiveTimeout.Units             = "";
            measurementReceiveTimeout.Value             = this.ReceiveTimeout.ToString();
            component.Signals.Add(measurementReceiveTimeout);

            FinchScalarSignal measurementAddress   = new FinchScalarSignal();
            measurementAddress.Name              = "Address";
            measurementAddress.Type              =  FinchDataType.String;
            measurementAddress.Units             = "";
            measurementAddress.Value             = this.Address.ToString();
            component.Signals.Add(measurementAddress);

            FinchScalarSignal port = new FinchScalarSignal();
            port.Name           = "Port";
            port.Type           = FinchDataType.String;
            port.Units          = "";
            port.Value          = this.Port.ToString();
            component.Signals.Add(port);
            
            return component;
        }*/        
    }
}
