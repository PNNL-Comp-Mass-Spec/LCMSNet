﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.PNNLDevices.NetworkStart.Socket
{
    /// <summary>
    /// Network Start using old command packing messaging for communication with mass spectrometer.
    /// </summary>
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(NetStartViewModel),

                                 "Network Start",
                                 "Detectors")
    ]
    public class NetStartSocket: IDevice, IFluidicsClosure, IHasDataProvider
    {
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
        private TcpClient       m_socketForServer;
        /// <summary>
        /// Stream reader to read underlying network stream data.
        /// </summary>
        private StreamReader    m_reader;
        /// <summary>
        /// Stream writer for writing to underlying network stream data.
        /// </summary>
        private StreamWriter    m_writer;
        /// <summary>
        /// Network stream for establishing connections.
        /// </summary>
        private NetworkStream   m_networkstream;
        /// <summary>
        /// The name of the port (e.g. "COM1") used for communication with the Agilent pumps
        /// </summary>
        private string m_address;
        /// <summary>
        /// Port of server instrument.
        /// </summary>
        private int m_port;
        /// <summary>
        /// The device's name.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the device.
        /// </summary>
        private DeviceStatus m_status;

        /// <summary>
        /// Fired when the status changes for the device.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;
        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NetStartSocket()
        {
            m_address = "localhost";
            m_name = "network start"; // classDeviceManager.Manager.CreateUniqueDeviceName("networkStart");
            m_port       = CONST_SERVER_PORT;
            m_status    = DeviceStatus.NotInitialized;

            AbortEvent      = new System.Threading.ManualResetEvent(false);
            Emulation       = true;
        }

        /// <summary>
        /// Gets or sets the send timeout for the socket.
        /// </summary>
        [DeviceSavedSetting("SendTimeout")]
        public int SendTimeout { get; set; }

        /// <summary>
        /// Gets or sets the receive timeout for the socket.
        /// </summary>
        [DeviceSavedSetting("ReceiveTimeout")]
        public int ReceiveTimeout { get; set; }

        /// <summary>
        /// Gets or sets the IP address or DNS name of the server instrument.
        /// </summary>
        [DeviceSavedSetting("IPAddress")]
        public string Address
        {
            get => m_address;
            set => m_address = value;
        }

        /// <summary>
        /// Gets or sets the port used to connect to the server.
        /// </summary>
        [DeviceSavedSetting("Port")]
        public int Port
        {
            get => m_port;
            set => m_port = value;
        }

        /// <summary>
        /// Connects to the server (instrument).
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public bool Connect(string server, int port)
        {
            var connected = false;

            if (Emulation)
                return true;

            try
            {
                m_socketForServer = new TcpClient(server, port)
                {
                    SendTimeout = Math.Max(SendTimeout, CONST_MIN_TIMEOUT_SEND),
                    ReceiveTimeout = Math.Max(ReceiveTimeout, CONST_MIN_TIMEOUT_RECEIVE)
                };

                m_networkstream = m_socketForServer.GetStream();

                m_reader = new StreamReader(m_networkstream);
                m_writer = new StreamWriter(m_networkstream);
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
                if (Emulation)
                    return true;

                if (m_networkstream != null)
                {
                    m_networkstream.Close();
                    return true;
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        /// <summary>
        /// Returns a list of methods stored on the instrument.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMethods()
        {
            var methods = new List<string>();
            if (Emulation)
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
                    var arguments = new List<NetStartArgument>();
                    SendMessage(m_writer, NetStartMessageTypes.Query, 0, CONST_QUERY_GETMETHODNAMES, arguments);

                    var rawMessage = m_reader.ReadLine();
                    var message = UnpackMessage(rawMessage);

                    foreach (var argument in message.ArgumentList)
                    {
                        methods.Add(argument.Value);
                    }
                }
                catch(Exception ex)
                {
                    HandleError("Could not retrieve the methods from the instrument.", ex);
                }
            }

            // Alert listeners that we have new methods!
            if (MethodNames != null)
            {
                var methodObjects = new List<object>();
                foreach (var method in methods)
                    methodObjects.Add(method);

                MethodNames(this, methodObjects);
            }

            return methods;
        }

        /// <summary>
        /// Sends a message to the instrument.
        /// </summary>
        /// <param name="streamWriter">Stream to send message across.</param>
        /// <param name="type">Type of message to send (e.g. query or post.)</param>
        /// <param name="sequence">Message Sequence Number</param>
        /// <param name="descriptor">Message Description (e.g. ACQSTARTED...)</param>
        /// <param name="arglist">Arguments to send with message (e.g. sample name or method name).</param>
        private void SendMessage(TextWriter streamWriter, NetStartMessageTypes type, int sequence, string descriptor, List<NetStartArgument> arglist)
        {
            //Console.WriteLine(descriptor);

            var message = PackMessage(type, sequence, descriptor, arglist);

            streamWriter.WriteLine(message);
            Console.WriteLine("Send: " + message);
            streamWriter.Flush();
        }

        private string PackMessage(NetStartMessageTypes type, int sequence, string descriptor, List<NetStartArgument> arglist)
        {
            var message = type.GetHashCode().ToString() + ":" + sequence.ToString() + "|" + descriptor;
            if (type == NetStartMessageTypes.Post || type == NetStartMessageTypes.Response || type == NetStartMessageTypes.Execute)
            {
                foreach (var arg in arglist)
                {
                    message += "@" + arg.Key + "=" + arg.Value;
                }
            }
            else if (type == NetStartMessageTypes.Query)
            {
                foreach (var arg in arglist)
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
        private NetStartMessage UnpackMessage(string message)
        {
            //<type>:<sqnc>|<dscp>@<argS1>=<argV1>@<argS1>=<argV1>...@<argSn>=<argVn>

            var msg = new NetStartMessage();
            var args = new List<NetStartArgument>();
            char[] tokens = {':','@','|','='};
            var messagepieces = message.Split(tokens);

            msg.Type = (NetStartMessageTypes)Enum.Parse(typeof(NetStartMessageTypes), messagepieces[0]);
            msg.Sequence = Int32.Parse(messagepieces[1]);
            msg.Descriptor = messagepieces[2];

            for (var i = 3; i < messagepieces.Length - 1; i+=2)
            {
                args.Add(new NetStartArgument(messagepieces[i], messagepieces[i+1]));
            }

            msg.ArgumentList = args;

            return msg;
        }

        /// <summary>
        /// Starts instrument acquisition.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="sample">Name of sample to run.</param>
        /// <returns>True if start successful.  False if start failed for any reason.</returns>
        [LCMethodEvent("Start Acquisition", MethodOperationTimeoutType.Parameter, 1, "MethodNames", 2)]
        public bool StartAcquisition(double timeout, ISampleInfo sample)
        {
            if (Emulation)
            {
                // Emulate starting an acquisition
                return true;
            }

            var arguments   = new List<NetStartArgument>();
            var receivedMessage    = new NetStartMessage();

            var success = false;

            if (sample == null)
                return false;

            var methodName = sample.InstrumentMethod;
            var sampleName = sample.Name;

            try
            {
                var i = 0;
                Connect(m_address, m_port);
                var streamReader = m_reader;
                var streamWriter = m_writer;
                SendMessage(streamWriter, NetStartMessageTypes.Query, i++, "ACQIDLE", arguments);

                var outputString = streamReader.ReadLine();
                if (UnpackMessage(outputString).Descriptor == "ACQIDLE")
                {
                    arguments.Add(new NetStartArgument("Method", methodName));
                    arguments.Add(new NetStartArgument("SampleName", sampleName));
                    SendMessage(streamWriter, NetStartMessageTypes.Post, i++, "ACQPARAMS", arguments);
                    arguments.Clear();

                    outputString = streamReader.ReadLine();     //RECEIVED

                    // Now see if the system is ready
                    SendMessage(streamWriter, NetStartMessageTypes.Query, i++, "ACQREADY", arguments);
                    outputString = streamReader.ReadLine();     //READY

                    if (UnpackMessage(outputString).Descriptor == "ACQREADY")
                    {
                        // Tell the system to prepare for acquisition
                        SendMessage(streamWriter, NetStartMessageTypes.Post, i++, "ACQPREPARE", arguments);
                        outputString = streamReader.ReadLine();     // Read off auto-response

                        // Then ask if it is prepared...this should be in some kind of loop
                        {
                            SendMessage(streamWriter, NetStartMessageTypes.Query, i++, "ACQPREPARED", arguments);

                            // Check to see if it is prepared...
                            outputString = streamReader.ReadLine();     // Read off response for PREPARED
                            var preparedMessage = UnpackMessage(outputString);
                            if (preparedMessage.ArgumentList.Count > 0 && preparedMessage.ArgumentList[0].Value.ToUpper() == "TRUE")
                            {
                                SendMessage(streamWriter, NetStartMessageTypes.Post, i++, "ACQSTART", arguments);
                                var startResponse = m_reader.ReadLine();

                                SendMessage(streamWriter, NetStartMessageTypes.Query, i++, "ACQSTARTED", arguments);
                                outputString = streamReader.ReadLine();     //STARTED

                                var startedMessage = UnpackMessage(outputString);
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

            return success;
        }

        /// <summary>
        /// Stops instrument acquisition.
        /// </summary>
        [LCMethodEvent("Stop Acquisition", MethodOperationTimeoutType.Parameter)]
        public bool StopAcquisition(double delayTime)
        {
            var startTime = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            if (Emulation)
            {
                // Emulate stopping an acquisition
                return true;
            }

            Connect(m_address, m_port);

            var streamReader = m_reader;
            var streamWriter = m_writer;
            var arguments = new List<NetStartArgument>();
            var receivedMessage = new NetStartMessage();

            bool success;
            try
            {
                string outputString;
                     SendMessage(streamWriter, NetStartMessageTypes.Post, 0, "ACQSTOP", arguments);
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

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get => m_name;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Emulation state.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the device's status.
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "None", this));
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets the abort event used to abort an I/O operation.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Initializes the device connecting to the server to get the instrument methods.
        /// </summary>
        /// <returns>True if it could connect, false if error occurred.</returns>
        public bool Initialize(ref string errorMessage)
        {
            var success = false;
            try
            {
                var connected = Connect(m_address, m_port);
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

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public string GetClosureType()
        {
            return "Network Start";
        }

        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

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

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Handles the error internally for propogating to external error handling objects.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private void HandleError(string message, Exception ex)
        {
            Error?.Invoke(this, new DeviceErrorEventArgs(message,
                                 ex,
                                 DeviceErrorStatus.ErrorAffectsAllColumns,
                                 this,
                                 "None"));
        }

        public DeviceErrorStatus ErrorType { get; set; }
        public DeviceType DeviceType => DeviceType.Component;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
