using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;
using FluidicsSDK.Devices;

using LcmsNetDataClasses.Method;
using LcmsNetSDK.Data;

using System.ComponentModel;

namespace ASIpump
{

    [classDeviceControlAttribute(typeof(AsiUI),
                                 "ASi Pump",
                                 "Syringe Pumps")]
    public class AsiPump : SerialDevice, IDevice, IPump, IFluidicsPump
    {

        /// <summary>
        /// Dictionary that holds a method name, key, and the method time table, value.
        /// </summary>
        private readonly Dictionary<string, string> mdict_methods;

        #region Program parameters


        // constant values
        public double uStepPeruL = 15400;
        public double minFlowRate = .12;
        public int StepLimit = 740000;
        public double MaxVolume = 48.05;

        // entered values
        [Description("Total flow in uL/minute")]
        [Category("Program Parameters")]
        [DisplayName("Total Flow")]
        [classPersistenceAttribute("TotalFlow")]
        public double TotalFlow { get; set; }

        [Description("Start %, 0-100")]
        [Category("Program Parameters")]
        [DisplayName("Start % A")]
        [classPersistenceAttribute("StartPercentA")]
        public double StartPercentA { get; set; }

        [Description("Start %, 0-100")]
        [Category("Program Parameters")]
        [DisplayName("Start % B")]
        [classPersistenceAttribute("StartPercentB")]
        public double StartPercentB { get; set; }

        [Description("Gradient Time (seconds)")]
        [Category("Program Parameters")]
        [DisplayName("Gradient Time")]
        [classPersistenceAttribute("GradientTime")]
        public double GradientTime { get; set; }

        [Description("Initial Iso time")]
        [Category("Program Parameters")]
        [DisplayName("Initial Iso Time")]
        [classPersistenceAttribute("InitialIsoTime")]
        public double InitialIsoTime { get; set; }

        [Description("Final Iso time")]
        [Category("Program Parameters")]
        [DisplayName("Final Iso Time")]
        [classPersistenceAttribute("FinalIsoTime")]
        public double FinalIsoTime { get; set; }
       
        // calculated values

        [Category("Calculated Values")]
        public double TotalVolumePumpB
        {
            get
            {
                return  ((GradientTime/60.0)*TotalFlow/2.0) + (((StartPercentB/100.0)*TotalFlow)*(InitialIsoTime/60.0)) +
                        (((StartPercentA/100.0)*TotalFlow)*(FinalIsoTime/60.0));
            }
        }

        [Category("Calculated Values")]
        public double NumPlateaus
        {
            get
            {
                return (StartSpeedB - StartSpeedA);
            }
        }

        [Category("Calculated Values")]
        public double StartSpeedA
        {
            get
            {
                return ((((StartPercentA / 100.0) * TotalFlow) / 60.0) * uStepPeruL);
            }
        }

        [Category("Calculated Values")]
        public double StartSpeedB
        {
            get
            {
                return ((((TotalFlow / 100.0) * StartPercentB) / 60.0) * uStepPeruL);
            }
        }

        [Category("Calculated Values")]
        public double PlateauTime
        {
            get
            {
                return (1.0 / (NumPlateaus / GradientTime));
            }
        }

        [Category("Calculated Values")]
        public double FullPlateuTime
        {
            get
            {
                return (PlateauTime * 1000.0)-7.0;
            }
        }

        [Category("Calculated Values")]
        public double PartialPlateuTime
        {
            get
            {
                return (1000.0 * (NumPlateaus - (int)NumPlateaus) * PlateauTime);
            }
        }

        #endregion

        public AsiPump()
        {
            // test calculations
            TotalFlow = 20;
            StartPercentA = 5;
            StartPercentB = 95;
            GradientTime = 200;
            InitialIsoTime = 30;
            FinalIsoTime = 0;

            MobilePhases  = new List<MobilePhase>();
            mdict_methods = new Dictionary<string, string>();
            mdict_methods.Add("Method A", "Some Data");
            mdict_methods.Add("Method B", "Some Data Too");
            Name          = "AsiPump";
        }

        #region Program literals

        private const string PromptPlateus = "Enter Noumber of full Plateaus";
        private const string PromptStartRateA = "Enter Solution A's Start rate in steps/sec";
        private const string PromptRateB = "Enter Solution B's Start rate in steps/sec";
        private const string PromptFullPlateuTime = "Enter Full plateau Time in milliSeconds";
        private const string PromptPartialPlateuTime = "Enter partial plateau time in milliseconds";
        private const string PromptInitialIsoTime = "Enter Initial Iso Time in Seconds";
        private const string PromptFinalIsoTime = "Enter Final Iso Time in Seconds";
        private const string PromptRestart = "Repeat cycle without re-entering data?";

        public const string MotorAddress = "*";

        //note the partial string.  The embedded program implements this inconsistantly.  This partial is common to all.
        private const string PromptPrompt = "Ctrl+j>";

        #endregion

        

        public void AsiPump_MessageStreamed(string msg)
        {
            // note the round down vs. round
            if (msg.Contains(PromptPlateus)) SendData(Math.Floor(NumPlateaus));

            if (msg.Contains(PromptStartRateA)) SendData(StartSpeedA);
            if (msg.Contains(PromptRateB)) SendData(StartSpeedB);
            if (msg.Contains(PromptFullPlateuTime)) SendData(FullPlateuTime);
            if (msg.Contains(PromptPartialPlateuTime)) SendData(PartialPlateuTime);
            if (msg.Contains(PromptInitialIsoTime)) SendData(InitialIsoTime);
            if (msg.Contains(PromptFinalIsoTime)) SendData(FinalIsoTime);

            if (msg.Contains(PromptRestart)) SendData(0);
        }

        #region Events
        /// <summary>
        /// Fired when a method is added.
        /// </summary>
        public event EventHandler<EventArgs> MethodAdded;
        /// <summary>
        /// Fired when a method is added.
        /// </summary>
        public event EventHandler<EventArgs> MethodUpdated;
        /// <summary>
        /// Fired when monitoring data is received from the instrument.
        /// </summary>        
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived;
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;
        /// <summary>
        /// List of times monitoring data was received.
        /// </summary>
        public List<DateTime> m_times;
        /// <summary>
        /// List of pressures used throughout the run.
        /// </summary>
        public List<double> m_pressures;
        /// <summary>
        /// List of flowrates used throughout the run.
        /// </summary>
        public List<double> m_flowrates;
        /// <summary>
        /// List of %B compositions throughout the run.
        /// </summary>
        public List<double> m_percentB;
        #endregion

        #region Properties
        [classPersistenceAttribute("TotalMonitoringMinutes")]
        public int TotalMonitoringMinutesDataToKeep
        {
            get;
            set;
        }
        [classPersistenceAttribute("TotalMonitoringSecondsElapsed")]
        public int TotalMonitoringSecondElapsed
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Emulation state.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets the device's status
        /// </summary>
        public LcmsNetDataClasses.Devices.enumDeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets whether the device is running
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Gets or sets the device's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the device's version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the port name to use to communicate with the pumps.
        /// </summary>
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get { return base.PortName; }
            set { base.PortName = value; }
        }
        /// <summary>
        /// Gets or sets the error type of the last error reported.
        /// </summary>
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the system device type.
        /// </summary>
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        #endregion


        #region Methods
        
        
        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Initialize(ref string errorMessage)
        {
            if (!Port.IsOpen)
            {
                Port.Open();
            }

            if (!Port.IsOpen)
            {
                errorMessage = "Port is not opened";
                return false;
            }
            else
            {
                ReplyDelimeter = "\r\n"; //carriage return linefeed
                SendDelimeter = "\n"; //Carriage return


                // catch messages sent by the pump
                // add this here so it gets on the end of the event queue
                // otherwise the send/replies get printed out of order in the UI
                MessageStreamed += AsiPump_MessageStreamed;

                // initialize party mode
                Send("");

                errorMessage = "Port is opened";
                return true;
            }
        }

        public void SendData(double data)
        {
            // wait for the query to be written
            Thread.Sleep(500);

            var rounded = Math.Round(data);

            var intData = (int) rounded;

            Send(MotorAddress + intData.ToString());
        }
         



        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, string type)
        {
            HandleError(message, type, null);
        }
        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, string type, Exception ex)
        {
            
        }

        public void Escape()
        {
            Send(MotorAddress + (char)27);
        }

        public void StartProgram()
        {
            Send(MotorAddress +"ex 1000");
        }

        /// <summary>
        /// Closes the connection to the device
        /// </summary>
        /// <returns>True on success</returns>
        public bool Shutdown()
        {
            
            Escape();
            return true;
        }
       
        #endregion


        #region Pump Interface methods

        /// <summary>
        /// Gets the percent B.
        /// </summary>
        /// <returns></returns>
        public double GetPercentB()
        {
            return 0;
        }

        /// <summary>
        /// Gets the current loaded method in the pump module.
        /// </summary>
        /// <returns>Method string kept on the pump.</returns>
        public string RetrieveMethod()
        {
            var methodString = "";
            
            return methodString;
        }
        /// <summary>
        /// Gets the flow rate. Note that this is the ideal flow rate not the actual flow rate.
        /// </summary>
        /// <returns>The flow rate</returns>
        public double GetFlowRate()
        {
            return 0;
        }
        /// <summary>
        /// Gets the pressure
        /// </summary>
        /// <returns>The pressure</returns>
        public double GetPressure()
        {
            return 0;
        }
        /// <summary>
        /// Gets the current mixer volume.
        /// </summary>
        /// <returns>The current mixer volume</returns>
        public double GetMixerVolume()
        {
            return 0;
        }
        /// <summary>
        /// Gets the actual flow rate
        /// </summary>
        /// <returns>The actual measured current flow rate</returns>
        public double GetActualFlow()
        {
            return 0;
        }

        #endregion

        #region Settings and Saving Methods
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            
        }
        #endregion

        #region IDevice Data Provider Methods

        /// <summary>
        /// Lists the methods
        /// </summary>
        private void ListMethods()
        {
            if (MethodNames != null)
            {
                var keys = new string[mdict_methods.Keys.Count];
                mdict_methods.Keys.CopyTo(keys, 0);

                var data = new List<object>();
                data.AddRange(keys);

                MethodNames(this, data);
            }
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
                    ListMethods();
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

        #region Performance and Error Notifications
        /// <summary>
        /// Writes the pump method time-table to the directory provided.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="methodName"></param>
        /// <param name="methodData"></param>
        private void WriteMethod(string directoryPath, string methodName)
        {
            
        }
        /// <summary>
        /// Writes the required data to the directory path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to create files in.</param>
        /// <param name="name">Name of the method the user is requesting performance data about.</param>
        /// <param name="parameters">Parameters used to create the performance data.</param>
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
            
        }
        public List<string> GetStatusNotificationList()
        {
            var notifications = new List<string>() { "Status"
                                                            };

            
            return notifications;
        }
        public List<string> GetErrorNotificationList()
        {
            var notifications = new List<string>() {  "" };

            return notifications;
        }
        #endregion

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run stored on the pumps.</param>
        [classLCMethod("Start Method", enumMethodOperationTime.Parameter, "MethodNames", 2, true)]
        public void StartMethod(double timeout, double flowrate, string methodName)
        {
            var start = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            //Some method starting private function StartMethod(methodName);
        }
        
        [classLCMethodAttribute("Turn On", 1, false, "", -1, false)]
        public void TurnOn()
        {
            
        }

        #region IFinchComponent Members

        /*public FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            

            return component;
        }*/

        #endregion

        #region IPump Members


        public List<MobilePhase> MobilePhases
        {
            get;
            set;
        }

        #endregion

    }
}
