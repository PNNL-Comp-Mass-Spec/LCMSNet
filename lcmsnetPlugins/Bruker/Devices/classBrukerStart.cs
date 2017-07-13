//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/19/2010
//
// Updates:
// - 06/29/2010 (DAC) - Added details for Bruker operation
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Timers;
using FluidicsSDK.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;

namespace LcmsNet.Devices.BrukerStart
{
    /// <summary>
    /// Class for detector using Bruker network trigger
    /// </summary>
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(BrukerStartViewModel), "Bruker", "Detectors")]
    public class classBrukerStart : IDevice, IFluidicsClosure
    {
        #region "Constants"
        const double COMMAND_TIMEOUT_MSEC = 20000;
        #endregion

        #region "Class variables"
        /// <summary>
        /// Device name
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Device version
        /// </summary>
        private string m_Version;

        /// <summary>
        /// Indicates if device is currently running
        /// </summary>
        private bool m_Running;

        /// <summary>
        /// Determines if device is in emulation mode
        /// </summary>
        private bool m_Emulation;

        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private enumDeviceStatus m_status;

        readonly classBrukerMsgTools mobject_MsgTools;
        readonly string m_OuputFolderLocal;
        readonly string m_OutputFolderRemote;
        readonly string m_MethodFolderLocal;
        classBrukerComConstants.SxcReplies mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_NOMESSAGE;
        bool m_AcquisitionInProgress;
        readonly Timer mobject_CmdTimeoutTimer;
        bool m_DeviceError;
        //          string m_DeviceErrorMessage = "";
        bool m_CmdTimedOut;
        #endregion

        #region "Events"

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;

        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        #endregion

        #region "Constructors"
        public classBrukerStart()
        {
            m_Name = "Bruker Start";

            //TODO: Remove this kind of coupling to the outside...
            m_OutputFolderRemote = Bruker.Properties.Settings.Default.BrukerOutputFolderShareName;
            m_OuputFolderLocal = Bruker.Properties.Settings.Default.BrukerOutputFolderLocalName;
            m_MethodFolderLocal = Bruker.Properties.Settings.Default.BrukerMethodFolderLocalName;
            var brukerNetName = Bruker.Properties.Settings.Default.BrukerInstNetName;
            var brukerPort = Bruker.Properties.Settings.Default.BrukerInstPort;

            mobject_MsgTools = new classBrukerMsgTools(m_OuputFolderLocal, m_MethodFolderLocal, brukerNetName, brukerPort);
            mobject_MsgTools.BrukerMsgReceived += BrukerMsgReceived;

            mobject_CmdTimeoutTimer = new Timer();
            mobject_CmdTimeoutTimer.Elapsed += CmdTimeoutTimer_Elapsed;
            mobject_CmdTimeoutTimer.BeginInit();
            mobject_CmdTimeoutTimer.Interval = COMMAND_TIMEOUT_MSEC;
            mobject_CmdTimeoutTimer.Enabled = false;
            mobject_CmdTimeoutTimer.EndInit();
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Gets or sets emulation state
        /// </summary>
        //[classPersistenceAttribute("Emulated")]
        public bool Emulation
        {
            get
            {
                return m_Emulation;
            }
            set
            {
                m_Emulation = value;
            }
        }

        /// <summary>
        /// Gets or sets the device status
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "Status Changed", this));
                m_status = value;
            }
        }
        /// <summary>
        /// Gets or sets the Port number to connect to on the Bruker System.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public int Port
        {
            get
            {
                return mobject_MsgTools.Port;
            }
            set
            {
                mobject_MsgTools.Port = value;
            }
        }
        /// <summary>
        /// Gets or sets the IP address of the Bruker System.
        /// </summary>
        [classPersistenceAttribute("IPAddress")]
        public string IPAddress
        {
            get
            {
                return mobject_MsgTools.IPAddress;
            }
            set
            {
                mobject_MsgTools.IPAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the device Running status
        /// </summary>
        public bool Running
        {
            get
            {
                return m_Running;
            }
            set
            {
                m_Running = value;
            }
        }

        /// <summary>
        /// Gets or sets the device name
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_Name, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Gets or sets the device's version
        /// </summary>
        public string Version
        {
            get
            {
                return m_Version;
            }
            set
            {
                m_Version = value;
            }
        }
        #endregion

        #region "Methods"
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        public virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        /// <summary>
        /// De-registers event handlers for this object
        /// </summary>
        public void Dispose()
        {
            // Placeholder in case of future need
        }

        /// <summary>
        /// Initializes the device
        /// </summary>
        /// <returns>True for success; False for failure</returns>
        public bool Initialize(ref string errorMessage)
        {
            try
            {
                GetMethods();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Could not initialize. " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Closes the connection to the device
        /// </summary>
        /// <returns>True for success, False for failure</returns>
        public bool Shutdown()
        {
            if (m_Emulation)
            {
                return true;
            }
            //TODO: Put real code here
            return true;
        }

        /// <summary>
        /// Starts data acquisition
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="sample">Data object for sample to be run</param>
        [classLCMethodAttribute("Start Method", enumMethodOperationTime.Parameter, true, 1, "", -1, false)]
        public bool StartAcquisition(double timeout, classSampleData sample)
        {
            if (m_Emulation)
                return true;

            if (System.Threading.Thread.CurrentThread.Name == "")
                System.Threading.Thread.CurrentThread.Name = "StartAcquisition";

            m_DeviceError = false;
            string msg;
            var sampleName = sample.DmsData.DatasetName + ".d";
            var methodName = sample.InstrumentData.MethodName;

            // Check for acquistion already in progress
            if (m_AcquisitionInProgress)
            {
                msg = "StartAcquisition: Acquisition already in progress";
                classApplicationLogger.LogError(0, msg);
                //Error(this, new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));
                Error?.Invoke(this,
                    new classDeviceErrorEventArgs(msg, null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, "None"));
                return false;
            }

            // Make the output folder
            var result = MakeOutputFolder(sampleName);
            if (!result.Success)
            {
                // There was a problem making the folder. Details are in result
                classApplicationLogger.LogError(0, result.Message, result.CreationException, sample);
                Error?.Invoke(this, //new classDeviceErrorArgs(this, result.Message, enumDeviceErrorStatus.ErrorSampleOnly, result.CreationException));
new classDeviceErrorEventArgs(result.Message, result.CreationException, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, "None"));
                return false;
            }

            // Make the method output folder
            var datasetFolder = result.DirectoryName;
            result = MakeMethodOutputFolder(datasetFolder, methodName);
            if (!result.Success)
            {
                // There was a problem making the folder. Details are in result
                classApplicationLogger.LogError(0, result.Message, result.CreationException, sample);
                Error?.Invoke(this, //new classDeviceErrorArgs(this, result.Message, enumDeviceErrorStatus.ErrorSampleOnly, result.CreationException));
    new classDeviceErrorEventArgs(result.Message,
                         result.CreationException,
                         enumDeviceErrorStatus.ErrorSampleOnly,
                         this,
                         "None"));
                return false;
            }

            // Start the acquisition on sXc
            try
            {
                // Open the connection to sXc
                if (mobject_MsgTools.SocketConneted)
                {
                    msg = "StartAcquisiton: Socket already connected - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }
                if (!mobject_MsgTools.ConnectSxcSocket())
                {
                    msg = "StartAcquisiton: Problem connecting to sXc - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Start listening for messages from sXc
                if (!mobject_MsgTools.StartListeningToSXC())
                {
                    msg = "StartAcquisition: Problem starting message listen method - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    mobject_MsgTools.DisconnectSXC();
                    return false;
                }
                // Send INIT_FTMS
                mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_NOMESSAGE;
                if (!mobject_MsgTools.InitFTMS())
                {
                    msg = "StartAcquistion: Problem sending INIT_FTMS - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));
    new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Wait for a READY reply from sXc
                var waitResult = WaitForReady();
                if (!waitResult)
                {
                    msg = "StartAcquistion: Problem waiting for READY response to INIT_FTMS - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                classApplicationLogger.LogMessage(2, "StartAcuisition.WaitForReady completed OK");

                // Send the sample info
                mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_NOMESSAGE;
                var sampleInfoXml = classBrukerXmlBuilder.CreateXmlString(m_OuputFolderLocal, m_MethodFolderLocal, sampleName, methodName);
                if (!mobject_MsgTools.SendSampleInfo(sampleInfoXml))
                {
                    msg = "StartAcquistion: Problem sending sample info - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Wait for a READY reply from sXc
                waitResult = WaitForReady();
                if (!waitResult)
                {
                    msg = "StartAcquistion: Problem waiting for READY response to sample info - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                classApplicationLogger.LogMessage(2, "StartAcuisition.WaitForReady completed OK");

                // Send PREPARE_ACQUISITION
                mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_NOMESSAGE;
                if (!mobject_MsgTools.PrepareAcquisition())
                {
                    msg = "StartAcquistion: Problem sending PREPARE_ACQUISITION - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Wait for a READY reply from sXc
                waitResult = WaitForReady();
                if (!waitResult)
                {
                    msg = "StartAcquistion: Problem waiting for READY response to PREPARE_ACQUISITION - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                classApplicationLogger.LogMessage(2, "StartAcuisition.WaitForReady completed OK");

                // Send START_ACQUISITION
                if (!mobject_MsgTools.StartAcquisition())
                {
                    msg = "StartAcquistion: Problem sending START_ACQUISITION - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                //NOTE: START_ACQUISITION returns a SOCKET_REQUEST_FTMS_START, which we're going to try ignoring. Therefore we're done with this method
                m_AcquisitionInProgress = true;
                return true;
            }
            catch (Exception ex)
            {
                msg = "Exception starting acquisition";
                classApplicationLogger.LogError(0, msg, ex, sample);
                StopSxcCommunication();
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

            new classDeviceErrorEventArgs(msg,
                         null,
                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                         this,
                         "None"));
                return false;
            }
        }

        /// <summary>
        /// Stops data acquisition
        /// </summary>
        [classLCMethodAttribute("Stop Acquisition", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool StopAcquisition(double timeout)
        {
            if (m_Emulation)
                return true;

            string msg;

            if (!m_AcquisitionInProgress)
            {
                msg = "EndAcquisition: No acquisition in progress";
                classApplicationLogger.LogError(0, msg);
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

            new classDeviceErrorEventArgs(msg,
                         null,
                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                         this,
                         "None"));
                return false;
            }

            try
            {
                // Send message to stop running after current scan finishes
                mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_NOMESSAGE;
                if (!mobject_MsgTools.FinishAcq())
                {
                    msg = "EndAcquistion: Problem sending FINISHACQUISITION - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Wait for a READY reply from sXc
                if (!WaitForReady())
                {
                    msg = "EndAcquistion: Problem waiting for READY response to FINISHACQUISITION - " + mobject_MsgTools.Msg;
                    classApplicationLogger.LogError(0, msg);
                    StopSxcCommunication();
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                     null,
                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                     this,
                     "None"));
                    return false;
                }

                // Stop the rest of the sXc communication process
                var result = StopSxcCommunication();
                if (!result)
                    return false;

                m_AcquisitionInProgress = false;
                return true;
            }
            catch (Exception ex)
            {
                msg = "StopAcquisition: Exception stopping acquisition" + ex.Message;
                classApplicationLogger.LogError(0, msg, ex);
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

            new classDeviceErrorEventArgs(msg,
                         null,
                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                         this,
                         "None"));
                return false;
            }
        }

        /// <summary>
        /// Stops communication with sXc and closes network port
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        private bool StopSxcCommunication()
        {
            string msg;

            // Stop listening for messages from sXc
            //TODO: Figure out how to log this, if necessary
            //clsLogTools.LogDebugMsg("StopSxcCommunication: Stop listening");
            if (!mobject_MsgTools.StopListeningToSXC())
            {
                msg = "StopSxcCommunication: Problem stopping message listen method - " + mobject_MsgTools.Msg;
                classApplicationLogger.LogError(0, msg);
                mobject_MsgTools.ExitFTMS();
                mobject_MsgTools.DisconnectSXC();
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

            new classDeviceErrorEventArgs(msg,
                         null,
                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                         this,
                         "None"));
                return false;
            }

            // Send EXIT_FTMS
            //TODO: Figure out how to log this, if necessary
            //clsLogTools.LogDebugMsg("StopSxcCommunication: Send EXIT_FTMS");
            if (!mobject_MsgTools.ExitFTMS())
            {
                msg = "StopSxcCommunication: Problem sending EXIT_FTMS - " + mobject_MsgTools.Msg;
                classApplicationLogger.LogError(0, msg);
                mobject_MsgTools.DisconnectSXC();
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));
        new classDeviceErrorEventArgs(msg,
                                         null,
                                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                         this,
                                         "None"));
                return false;
            }

            // Disconnect from sXc listening port
            //TODO: Figure out how to log this, if necessary
            //clsLogTools.LogDebugMsg("StopSxcCommunication: Disconnect from port");
            if (!mobject_MsgTools.DisconnectSXC())
            {
                msg = "StopSxcCommunication: Problem disconnecting from sXc - " + mobject_MsgTools.Msg;
                classApplicationLogger.LogError(0, msg);
                Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));
        new classDeviceErrorEventArgs(msg,
                                         null,
                                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                         this,
                                         "None"));
                return false;
            }

            m_AcquisitionInProgress = false;
            return true;
        }

        /// <summary>
        /// Waits for a FTMS_READY response from sXc
        /// </summary>
        /// <returns>TRUE if FTMS_READY received; FALSE on error or not received within timeout</returns>
        private bool WaitForReady()
        {

            m_CmdTimedOut = false;

            mobject_CmdTimeoutTimer.Enabled = true;
            while ((mobject_sXcReply != classBrukerComConstants.SxcReplies.FTMS_READY) && (!m_DeviceError)
                        && (!m_CmdTimedOut) && (!m_DeviceError))
            {
                //TODO: Figure out how to log this, if necessary
                //clsLogTools.LogDebugMsg("WaitForReady: m_sXcReply = " + m_sXcReply.ToString());
                if (m_DeviceError)
                {
                    var msg = "clsBrukerStart.WaitForReady: Error received from Bruker";
                    classApplicationLogger.LogError(0, msg);
                    break;
                }
                System.Threading.Thread.Sleep(500); // Take a 500 msec nap
            }

            // Check to see if wait loop exited because FTMS_READY received, or because there was a problem.
            if ((mobject_sXcReply == classBrukerComConstants.SxcReplies.FTMS_READY) && (!m_DeviceError))
            {
                // FTMS_READY received
                mobject_CmdTimeoutTimer.Enabled = false;
                return true;
            }

            // There was a problem
            mobject_CmdTimeoutTimer.Enabled = false;
            return false;
        }

        /// <summary>
        /// Handles a message that has been received from Bruker sXc and processed
        /// </summary>
        /// <param name="sXcReply">Reply received from Bruker</param>
        private void HandleBrukerMsg(classBrukerComConstants.SxcReplies sXcReply)
        {
            classApplicationLogger.LogMessage(2, "classBrukerStart.HandleBrukerMsg: Starting method");

            string msg;
            mobject_sXcReply = sXcReply;

            switch (sXcReply)
            {
                case classBrukerComConstants.SxcReplies.FTMS_CRITICALERROR:
                    mobject_CmdTimeoutTimer.Enabled = false;
                    msg = "Critical error " + sXcReply.ToString() + " received from Bruker";
                    classApplicationLogger.LogError(0, msg);
                    m_DeviceError = true;
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));
        new classDeviceErrorEventArgs(msg,
                                     null,
                                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                     this,
                                     "None"));
                    break;
                case classBrukerComConstants.SxcReplies.FTMS_ERROR:
                    mobject_CmdTimeoutTimer.Enabled = false;
                    msg = "Error " + sXcReply.ToString() + " received from Bruker";
                    classApplicationLogger.LogError(0, msg);
                    m_DeviceError = true;
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, null));

        new classDeviceErrorEventArgs(msg,
                                     null,
                                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                     this,
                                     "None"));
                    break;
                case classBrukerComConstants.SxcReplies.SXC_INVALID:
                    classApplicationLogger.LogError(0, "Invalid message recevied from Bruker");
                    break;
                default:
                    //                      classApplicationLogger.LogMessage(0, "Message " + sXcReply.ToString() + " received from Bruker");
                    break;
            }

            classApplicationLogger.LogMessage(2, "classBrukerStart.HandleBrukerMsg: Exiting method");

        }

        /// <summary>
        /// Gets method folders from the instrument
        /// </summary>
        /// <returns>List of method names</returns>
        public List<string> GetMethods()
        {
            var methods = new List<string>();

            if (m_Emulation)
            {
                methods.Add("Dummy-Bruker-01");
                methods.Add("Dummy-Bruker-02");
                methods.Add("Dummy-Bruker-03");
                methods.Add("Dummy-Bruker-04");
            }
            else
            {
                var methodFolder = Bruker.Properties.Settings.Default.BrukerMethodFolderShareName;

                try
                {
                    var directoryList = Directory.GetDirectories(methodFolder, "*.m");
                    foreach (var tmpDirectory in directoryList)
                    {
                        var di = new DirectoryInfo(tmpDirectory);
                        methods.Add(di.Name);
                    }
                }
                catch (Exception ex)
                {
                    var msg = "Could not retrieve the methods from the Bruker instrument";
                    Error?.Invoke(this, //new classDeviceErrorArgs(this, msg, enumDeviceErrorStatus.ErrorAffectsAllColumns, ex));

        new classDeviceErrorEventArgs(msg,
                                     ex,
                                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                     this,
                                     "None"));
                }

            }

            // Alert listeners that new methods are available
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
        /// Builds the output folder for an acquisition run
        /// </summary>
        /// <param name="datasetName">Dataset name that will be used to create the folder name</param>
        /// <returns>Object describing results</returns>
        private classFolderCreateResults MakeOutputFolder(string datasetName)
        {
            var result = new classFolderCreateResults();
            var outFolderName = Path.Combine(m_OutputFolderRemote, datasetName);

            try
            {
                // If directory already exists, we have to skip this dataset
                if (Directory.Exists(outFolderName))
                {
                    result.Success = false;
                    result.Message = "classBrukerStart.MakeOutputFolder: Output folder " + outFolderName + " already exists";
                    classApplicationLogger.LogError(0, result.Message);
                    result.DirectoryName = outFolderName;
                    result.CreationException = null;
                }
                else
                {
                    // Atempt to create the directory
                    Directory.CreateDirectory(outFolderName);
                    classApplicationLogger.LogMessage(0, "classBrukerStart.MakeOutputFolder: Created output folder " + outFolderName);
                    result.DirectoryName = outFolderName;
                    result.Success = true;
                    result.CreationException = null;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "classBrukerStart.MakeOutputFolder: Exception creating output folder " + outFolderName;
                classApplicationLogger.LogError(0, result.Message);
                result.DirectoryName = outFolderName;
                result.CreationException = ex;
            }

            return result;
        }

        /// <summary>
        /// Creates a method folder within the dataset output folder
        /// </summary>
        /// <param name="datasetFolderName">Fully qualified path to dataset output folder</param>
        /// <param name="methodFolderName">Name of method folder to be created</param>
        /// <returns>Object describing results</returns>
        private classFolderCreateResults MakeMethodOutputFolder(string datasetFolderName, string methodFolderName)
        {
            var result = new classFolderCreateResults();
            var outFolderName = Path.Combine(datasetFolderName, methodFolderName);

            try
            {
                // If directory already exists, we have to skip this dataset
                if (Directory.Exists(outFolderName))
                {
                    result.Success = false;
                    result.Message = "classBrukerStart.MakeMethodOutputFolder: Output folder " + outFolderName + " already exists";
                    classApplicationLogger.LogError(0, result.Message);
                    result.DirectoryName = outFolderName;
                    result.CreationException = null;
                }
                else
                {
                    // Atempt to create the directory
                    Directory.CreateDirectory(outFolderName);
                    classApplicationLogger.LogMessage(0, "classBrukerStart.MakeMethodOutputFolder: Created output folder " + outFolderName);
                    result.DirectoryName = outFolderName;
                    result.CreationException = null;
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "classBrukerStart.MakeMethodOutputFolder: Exception creating output folder " + outFolderName;
                result.DirectoryName = outFolderName;
                result.CreationException = ex;
                classApplicationLogger.LogError(0, result.Message);
            }

            return result;
        }

        public override string ToString()
        {
            return m_Name;
        }

        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }
        #endregion

        #region "Event handlers"
        /// <summary>
        /// Handles an incoming message from sXc
        /// </summary>
        /// <param name="sXcReply">Message</param>
        void BrukerMsgReceived(classBrukerComConstants.SxcReplies sXcReply)
        {
            classApplicationLogger.LogMessage(2, "classBrukerStart.BrukerMsgReceived: Handling BrukerMsgReceived event");
            HandleBrukerMsg(sXcReply);
            classApplicationLogger.LogMessage(2, "classBrukerStart.BrukerMsgReceived: BrukerMsgReceived event handling complete");
        }

        /// <summary>
        /// Handles timeout caused by non-receipt of FTMS_READY message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CmdTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Name = "CmdTimeoutTimerElapsed";

            mobject_CmdTimeoutTimer.Enabled = false;
            m_DeviceError = true;
            mobject_sXcReply = classBrukerComConstants.SxcReplies.SXC_INVALID;
            m_CmdTimedOut = true;
            classApplicationLogger.LogError(1, "classBrukerStart: Command timeout");
        }
        #endregion

        #region IDevice Members
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status Changed" };
        }

        public List<string> GetErrorNotificationList()
        {

            return new List<string>();
        }
        #endregion

        #region IDevice Data Provider Methods
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {

        }
        #endregion

        #region IDevice Members
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        public enumDeviceType DeviceType => enumDeviceType.Component;

        #endregion

        #region IDevice Members
        /*public FinchComponentData GetData()
        {
            FinchComponentData component    = new FinchComponentData();
            component.Status                = Status.ToString();
            component.Name                  = Name;
            component.Type                  = "Network Start";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurementAddress = new FinchScalarSignal();
            measurementAddress.Name              = "Address";
            measurementAddress.Type              = FinchDataType.String;
            measurementAddress.Units             = "";
            measurementAddress.Value             = this.IPAddress;
            component.Signals.Add(measurementAddress);

            FinchScalarSignal port = new FinchScalarSignal();
            port.Name           = "Port";
            port.Type           = FinchDataType.String;
            port.Units          = "";
            port.Value          = this.Port.ToString();
            component.Signals.Add(port);

            return component;
        }*/

        #endregion

        #region IFluidicsCLosure Members
        public string GetClosureType()
        {
            return "Bruker";
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
