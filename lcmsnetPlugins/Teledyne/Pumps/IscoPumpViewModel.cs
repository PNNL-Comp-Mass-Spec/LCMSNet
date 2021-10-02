using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetCommonControls.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    public class IscoPumpViewModel : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        #region "Constructors"

        public IscoPumpViewModel()
        {
            ControlModesComboBoxOptions = Enum.GetValues(typeof(IscoControlMode)).Cast<IscoControlMode>()
                .Where(x => x != IscoControlMode.External /* External is not a valid option... */).ToList().AsReadOnly();
            OperationModeComboBoxOptions = Enum.GetValues(typeof(IscoOperationMode)).Cast<IscoOperationMode>().ToList().AsReadOnly();
            PumpCountComboBoxOptions = new List<int>(Enumerable.Range(1, 3)).AsReadOnly();

            // Add a list of available unit addresses to the unit address combo box
            var unitAddressOptions = new List<int>();
            for (var indx = pump.UnitAddressMin; indx <= pump.UnitAddressMax; indx++)
            {
                unitAddressOptions.Add(indx);
            }
            UnitAddressComboBoxOptions = unitAddressOptions.AsReadOnly();

            // Initialize refill rate array
            // Initialize max refill rate array
            RefillRates = new List<RefillData>
            {
                new RefillData("Pump A", 0) {MaxRefillRate = 30D},
                new RefillData("Pump B", 1) {MaxRefillRate = 30D},
                new RefillData("Pump C", 2) {MaxRefillRate = 30D},
            }.AsReadOnly();

            LimitsList = new List<LimitData>
            {
                new LimitData("Flow Units"),
                new LimitData("Pressure Units"),
                new LimitData("Level Units"),
                new LimitData("Min Pressure SP"),
                new LimitData("Max Pressure SP (Note 1)"),
                new LimitData("Min Flow SP"),
                new LimitData("Max Flow SP"),
                new LimitData("Max Flow Value (Note 2)"),
                new LimitData("Min Refill Rate SP"),
                new LimitData("Max Refill Rate SP"),
            }.AsReadOnly();


            // Initialize the pump display controls
            PumpDisplays = new List<IscoPumpDisplayViewModel>
            {
                new IscoPumpDisplayViewModel(0),
                new IscoPumpDisplayViewModel(1),
                new IscoPumpDisplayViewModel(2),
            }.AsReadOnly();

            foreach (var pumpDisplay in PumpDisplays)
            {
                // Set event handlers
                pumpDisplay.SetpointChanged += PumpDisplays_SetpointChanged;
                pumpDisplay.StartRefill += PumpDisplays_StartRefill;
                pumpDisplay.StartPump += PumpDisplays_StartPump;
                pumpDisplay.StopPump += PumpDisplays_StopPump;
            }

            PropertyChanged += PumpIscoViewModel_PropertyChanged;

            SetControlModeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetControlMode));
            StartAllCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StartAllPumps));
            StopAllCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StopAllPumps));
            SetAllFlowCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetAllFlow));
            SetAllPressureCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetAllPressure));
            RefillAllCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefillAll));
            UpdateDisplaysCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshPumpDisplays));
            SetPortSettingsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetPortProperties));
            SetOperationModeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetOperationMode));
        }

        #endregion

        #region "Class variables"

        public class RefillData : ReactiveObject
        {
            public string PumpName { get; }
            public string PumpLabel => PumpName + ":";
            public int PumpIndex { get; }

            public double RefillRate
            {
                get => refillRate;
                set => this.RaiseAndSetIfChanged(ref refillRate, value);
            }

            public double MaxRefillRate
            {
                get => maxRefillRate;
                set => this.RaiseAndSetIfChanged(ref maxRefillRate, value);
            }

            private double refillRate;
            private double maxRefillRate;

            public RefillData(string pumpName, int pumpIndex)
            {
                PumpName = pumpName;
                PumpIndex = pumpIndex;
            }
        }

        public class LimitData : ReactiveObject
        {
            public string LimitName { get; }

            public string PumpA
            {
                get => pumpA;
                set => this.RaiseAndSetIfChanged(ref pumpA, value);
            }

            public string PumpB
            {
                get => pumpB;
                set => this.RaiseAndSetIfChanged(ref pumpB, value);
            }

            public string PumpC
            {
                get => pumpC;
                set => this.RaiseAndSetIfChanged(ref pumpC, value);
            }

            public void SetLimit(int pumpIndex, string limit)
            {
                if (pumpIndex == 0)
                {
                    PumpA = limit;
                }
                if (pumpIndex == 1)
                {
                    PumpB = limit;
                }
                if (pumpIndex == 2)
                {
                    PumpC = limit;
                }
            }

            private string pumpA = "";
            private string pumpB = "";
            private string pumpC = "";

            public LimitData(string limitName)
            {
                LimitName = limitName;
            }
        }

        private IscoPump pump = new IscoPump();
        private int pumpCount = 3;
        private string comPort = "";
        private int unitAddress = 6;
        private string notes = "";
        private IscoControlMode controlMode = IscoControlMode.Local;
        private IscoOperationMode operationMode = IscoOperationMode.ConstantPressure;
        private int portReadTimeout = 500;
        private int portWriteTimeout = 500;
        private int portBaudRate = 9600;

        #endregion

        #region "Properties"

        public ReadOnlyCollection<IscoPumpDisplayViewModel> PumpDisplays { get; }
        public ReadOnlyCollection<IscoControlMode> ControlModesComboBoxOptions { get; }
        public ReadOnlyCollection<int> PumpCountComboBoxOptions { get; }
        public ReadOnlyObservableCollection<SerialPortData> ComPortComboBoxOptions => SerialPortGenericData.SerialPorts;
        public ReadOnlyCollection<int> UnitAddressComboBoxOptions { get; }
        public ReadOnlyCollection<IscoOperationMode> OperationModeComboBoxOptions { get; }
        public ReadOnlyCollection<RefillData> RefillRates { get; }
        public ReadOnlyCollection<LimitData> LimitsList { get; }

        public string COMPort
        {
            get => comPort;
            set => this.RaiseAndSetIfChanged(ref comPort, value);
        }

        public int UnitAddress
        {
            get => unitAddress;
            set => this.RaiseAndSetIfChanged(ref unitAddress, value);
        }

        public string Notes
        {
            get => notes;
            set => this.RaiseAndSetIfChanged(ref notes, value);
        }

        public IscoControlMode ControlMode
        {
            get => controlMode;
            set => this.RaiseAndSetIfChanged(ref controlMode, value);
        }

        public IscoOperationMode OperationMode
        {
            get => operationMode;
            set => this.RaiseAndSetIfChanged(ref operationMode, value);
        }

        public int PumpCount
        {
            get => pumpCount;
            set => this.RaiseAndSetIfChanged(ref pumpCount, value);
        }

        public int PortReadTimeout
        {
            get => portReadTimeout;
            set => this.RaiseAndSetIfChanged(ref portReadTimeout, value);
        }

        public int PortWriteTimeout
        {
            get => portWriteTimeout;
            set => this.RaiseAndSetIfChanged(ref portWriteTimeout, value);
        }

        public int PortBaudRate
        {
            get => portBaudRate;
            set => this.RaiseAndSetIfChanged(ref portBaudRate, value);
        }

        public override IDevice Device
        {
            get => pump;
            set => RegisterDevice(value);
        }

        public bool Emulation
        {
            get => pump.Emulation;
            set => pump.Emulation = value;
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SetControlModeCommand { get; }
        public ReactiveCommand<Unit, Unit> StartAllCommand { get; }
        public ReactiveCommand<Unit, Unit> StopAllCommand { get; }
        public ReactiveCommand<Unit, Unit> SetAllFlowCommand { get; }
        public ReactiveCommand<Unit, Unit> SetAllPressureCommand { get; }
        public ReactiveCommand<Unit, Unit> RefillAllCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateDisplaysCommand { get; }
        public ReactiveCommand<Unit, Unit> SetPortSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> SetOperationModeCommand { get; }

        #endregion

        #region "Methods"

        public override UserControl GetDefaultView()
        {
            return new IscoPumpView();
        }

        private void PumpIscoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PumpCount)))
            {
                PumpCountChanged();
            }
        }

        /// <summary>
        /// Initializes the controls and displays
        /// </summary>
        private void InitControl()
        {
            // Set the default to 6, if allowed; otherwise use minimum value
            if (UnitAddressComboBoxOptions.Count > 0)
            {
                UnitAddress = UnitAddressComboBoxOptions[0];
                if (UnitAddressComboBoxOptions.Contains(6))
                {
                    UnitAddress = 6;
                }
            }

            // Fill in the Notes text box
            var noteStr = "1) Also max allowed pressure in Const Flow mode" + Environment.NewLine;
            noteStr += "2) Max allowed flow in Const Press mode";
            Notes = noteStr;

            // Assign pump class event handlers
            pump.RefreshComplete += Pump_RefreshComplete;
            pump.InitializationComplete += Pump_InitializationComplete;
            pump.ControlModeSet += Pump_ControlModeSet;
            pump.OperationModeSet += Pump_OperationModeSet;
            pump.Disconnected += Pump_Disconnected;

#if DACTEST
                pump.StatusUpdate += new EventHandler<DeviceStatusEventArgs>(Pump_StatusUpdate);
                pump.Error += new EventHandler<DeviceErrorEventArgs>(Pump_Error);
#endif
            // Initial control mode display
            ControlMode = IscoControlMode.Local;

            // Set initial number of pumps to max
            PumpCount = pump.PumpCount;

            // Set initial operation mode display
            OperationMode = IscoOperationMode.ConstantFlow;
            SetOperationMode();

            // Set/Reset max refill rates
            foreach (var rate in RefillRates)
            {
                rate.MaxRefillRate = 30D;
            }

            COMPort = pump.PortName;

            UpdateLimitDisplay();
        }

        private void RegisterDevice(IDevice device)
        {
            pump = device as IscoPump;
            InitControl();

            // Add to the device manager
            SetBaseDevice(pump);
        }

        //public bool RemoveDevice()
        //{
        //    try
        //    {
        //        // Shutdown the object's connections
        //        pump.Shutdown();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new IscoException("Exception shutting down ISCO pump", pump.GetBaseException(ex));
        //    }
        //    return classDeviceManager.Manager.RemoveDevice(this.Device);
        //}

#if DACTEST
        /// <summary>
        /// Logs an ISCO message to a file (testing only)
        /// </summary>
        /// <param name="msg"></param>
        public void LogDebugMessage(string msg)
        {
            var execPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(execPath), "MsgLog.txt");

            var logTxt = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).ToString("MM/dd/yyyy HH:mm:ss.ff") + ", " + msg;

            using (System.IO.StreamWriter w = System.IO.File.AppendText(outFile))
            {
                w.WriteLine(logTxt);
                w.Flush();
                w.Close();
            }
        }
#endif

        /// <summary>
        /// Updates the display of limits values
        /// </summary>
        private void UpdateLimitDisplay()
        {
            for (var indx = 0; indx < PumpDisplays.Count; indx++)
            {
                // Flow units
                LimitsList[0].SetLimit(indx, IscoConversions.GetFlowUnitsString());

                // Pressure units
                LimitsList[1].SetLimit(indx, IscoConversions.GetPressUnitsString());

                // Level units
                LimitsList[2].SetLimit(indx, "mL");

                // Min press SP
                LimitsList[3].SetLimit(indx, PumpDisplays[indx].MinPressSp.ToString("0.000"));

                // Max press SP
                LimitsList[4].SetLimit(indx, PumpDisplays[indx].MaxPressSp.ToString("0.000"));

                // Min flow SP
                LimitsList[5].SetLimit(indx, PumpDisplays[indx].MinFlowSp.ToString("0.000"));

                // Max flow SP
                LimitsList[6].SetLimit(indx, PumpDisplays[indx].MaxFlowSp.ToString("0.000"));

                // Max flow limit
                LimitsList[7].SetLimit(indx, PumpDisplays[indx].MaxFlowLimit.ToString("0.000"));

                // Min refill rate SP
                LimitsList[8].SetLimit(indx, "0.0");

                // Max refill rate SP
                LimitsList[9].SetLimit(indx, RefillRates[indx].MaxRefillRate.ToString("0.000"));
            }
        }

        #endregion

        #region "Form event handlers"

        /// <summary>
        /// Starts refill for a single pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pumpIndx"></param>
        private void PumpDisplays_StartRefill(object sender, int pumpIndx)
        {
            if (pump.StartRefill(pumpIndx, RefillRates[pumpIndx].RefillRate))
            {
                UpdateStatusDisplay("Refill started");
            }
            else
            {
                UpdateStatusDisplay("Refill error");
            }
        }

        /// <summary>
        /// Stops specified pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pumpIndx"></param>
        private void PumpDisplays_StopPump(object sender, int pumpIndx)
        {
            if (pump.StopPump(0, pumpIndx))
            {
                UpdateStatusDisplay("Pump stopped");
            }
            else
            {
                UpdateStatusDisplay("Problem stopping pump");
            }
        }

        /// <summary>
        /// Starts specified pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pumpIndx"></param>
        private void PumpDisplays_StartPump(object sender, int pumpIndx)
        {
            if (pump.StartPump(0, pumpIndx))
            {
                UpdateStatusDisplay("Pump started");
            }
            else
            {
                UpdateStatusDisplay("Problem starting pump");
            }
        }

        /// <summary>
        /// Changes the setpoint for one pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pumpIndex">Pump to change</param>
        /// <param name="newValue">new setpoint</param>
        private void PumpDisplays_SetpointChanged(object sender, int pumpIndex, double newValue)
        {
            bool success;

            if (pump.OperationMode == IscoOperationMode.ConstantFlow)
            {
                success = pump.SetFlow(pumpIndex, newValue);
            }
            else
            {
                success = pump.SetPressure(pumpIndex, newValue);
            }

            if (success)
            {
                UpdateStatusDisplay("Setpoint changed");
            }
            else
            {
                UpdateStatusDisplay("Problem making setpoint change");
            }
        }

        /// <summary>
        /// Refreshes the pump displays
        /// </summary>
        private void RefreshPumpDisplays()
        {
            if (!pump.Refresh())
            {
                UpdateStatusDisplay("Problem refreshing pump status");
            }
        }

        /// <summary>
        /// Starts all pumps
        /// </summary>
        private void StartAllPumps()
        {
            var success = true;

            for (var pumpIndx = 0; pumpIndx < pumpCount; pumpIndx++)
            {
                if (!pump.StartPump(0, pumpIndx))
                    success = false;
            }

            if (success)
            {
                UpdateStatusDisplay("All pumps started");
            }
            else
                UpdateStatusDisplay("Problem starting one or more pumps");
        }

        /// <summary>
        /// Stops all pumps
        /// </summary>
        private void StopAllPumps()
        {
            var success = true;

            for (var pumpIndx = 0; pumpIndx < pumpCount; pumpIndx++)
            {
                if (!pump.StopPump(0, pumpIndx))
                    success = false;
            }

            if (success)
            {
                UpdateStatusDisplay("All pumps stopped");
            }
            else
                UpdateStatusDisplay("Problem stopping one or more pumps");
        }

        /// <summary>
        /// Sets the flow setpoint for all pumps
        /// </summary>
        private void SetAllFlow()
        {
            if (pump.OperationMode != IscoOperationMode.ConstantFlow)
            {
                UpdateStatusDisplay("Pump must be in constant flow mode");
                return;
            }

            var success = true;
            for (var indx = 0; indx < pumpCount; indx++)
            {
                if (!pump.SetFlow(indx, PumpDisplays[indx].Setpoint))
                    success = false;
            }

            if (success)
            {
                UpdateStatusDisplay("Flow setpoints changed");
            }
            else
                UpdateStatusDisplay("Problem changing one or more flow setpoints");
        }

        /// <summary>
        /// Sets pressure setpoint for all pumps
        /// </summary>
        private void SetAllPressure()
        {
            if (pump.OperationMode != IscoOperationMode.ConstantPressure)
            {
                UpdateStatusDisplay("Pump must be in constant pressure mode");
                return;
            }

            var success = true;
            for (var indx = 0; indx < pumpCount; indx++)
            {
                if (!pump.SetPressure(indx, PumpDisplays[indx].Setpoint))
                    success = false;
            }

            if (success)
            {
                UpdateStatusDisplay("Pressure setpoints changed");
            }
            else
                UpdateStatusDisplay("Problem changing one or more pressure setpoints");
        }

        /// <summary>
        /// Starts a refill operation on all pumps
        /// </summary>
        private void RefillAll()
        {
            var success = true;
            for (var pumpIndx = 0; pumpIndx < pumpCount; pumpIndx++)
            {
                if (!pump.StartRefill(pumpIndx, RefillRates[pumpIndx].RefillRate))
                    success = false;
            }

            if (success)
            {
                UpdateStatusDisplay("Refill started all pumps");
            }
            else
                UpdateStatusDisplay("Problem starting refill on one or more pumps");
        }

        /// <summary>
        /// Sets the control mode
        /// </summary>
        private void SetControlMode()
        {
            var newMode = IscoControlMode.External;

            if (ControlMode == IscoControlMode.Local)
            {
                newMode = IscoControlMode.Local;
            }
            else
            {
                newMode = IscoControlMode.Remote;
            }

            if (pump.SetControlMode(newMode))
            {
                UpdateStatusDisplay("Control mode changed");
            }
            else
            {
                UpdateStatusDisplay("Problem setting control mode");
            }
        }

        /// <summary>
        /// Combo box for choosing number of pumps in use
        /// </summary>
        private void PumpCountChanged()
        {
            pump.PumpCount = PumpCount;
        }

        /// <summary>
        /// Sets the operation mode
        /// </summary>
        private void SetOperationMode()
        {
            var newMode = IscoOperationMode.ConstantPressure;

            if (OperationMode == IscoOperationMode.ConstantFlow)
            {
                newMode = IscoOperationMode.ConstantFlow;
            }
            else
                newMode = IscoOperationMode.ConstantPressure;

            if (pump.SetOperationMode(newMode))
            {
                foreach (var pd in PumpDisplays)
                {
                    pd.OperationMode = newMode;
                }
                UpdateStatusDisplay("Operation mode changed");
            }
            else
                UpdateStatusDisplay("Problem setting control mode");
        }

        /// <summary>
        /// Sets the serial port properties object contents
        /// </summary>
        private void SetPortProperties()
        {
            if (pump.IsOpen())
            {
            }
            pump.PortName = COMPort;
            pump.BaudRate = PortBaudRate;
            pump.ReadTimeout = PortReadTimeout;
            pump.WriteTimeout = PortWriteTimeout;
            pump.UnitAddress = UnitAddress;
        }

        /// <summary>
        /// Sets the refill rate SP's
        /// </summary>
        private void SetRefillRates()
        {
            // Warn if pump isn't initialized, which means range limits aren't available
            if (!pump.Initialized)
            {
                UpdateStatusDisplay("Pump not initialized");
                return;
            }
        }

        #endregion

        #region "Misc event handlers"

        /// <summary>
        /// Refresh complete handler
        /// </summary>
        private void Pump_RefreshComplete()
        {
            try
            {
                foreach (var pumpDisplay in PumpDisplays)
                {
                    var pumpData = pump.GetPumpData(pumpDisplay.PumpIndex);
                    if (pumpData != null)
                    {
                        pumpDisplay.FlowRate = pumpData.Flow;
                        pumpDisplay.Pressure = pumpData.Pressure;
                        pumpDisplay.Volume = pumpData.Volume;
                        //pumpDisplay.Setpoint = pumpData.SetPoint;
                        pumpDisplay.ProblemStatus = pumpData.ProblemStatus;
                    }

                    //var setpointLimits = pump.GetSetpointLimits(pumpDisplay.PumpIndex);
                    //if (setpointLimits != null)
                    //{
                    //   pumpDisplay.MinFlowSp = setpointLimits.MinFlowSp;
                    //   pumpDisplay.MaxFlowSp = setpointLimits.MaxFlowSp;
                    //   pumpDisplay.MinPressSp = setpointLimits.MinPressSp;
                    //   pumpDisplay.MaxPressSp = setpointLimits.MaxPressSp;
                    //   pumpDisplay.MaxFlowLimit = setpointLimits.MaxFlowLimit;
                    //}
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "Exception occurred trying to refresh pump data " + ex.StackTrace);
            }
            //mcontrol_IscoGraphs.UpdateAllPlots(m_Pump.PumpData);
        }

        /// <summary>
        /// Initialization complete handler
        /// </summary>
        private void Pump_InitializationComplete()
        {
            // Set the operation mode displays
            var currOpMode = pump.OperationMode;
            OperationMode = currOpMode;
            foreach (var pd in PumpDisplays)
            {
                pd.OperationMode = currOpMode;
            }

            // Get data for limits display
            for (var indx = 0; indx < pumpCount; indx++)
            {
                var rangeData = pump.GetPumpRanges(indx);
                var setpointLimits = pump.GetSetpointLimits(indx);

                if (rangeData == null)
                    rangeData = new IscoPumpRangeData(); // Use the defaults in the class
                if (setpointLimits == null)
                    setpointLimits = new IscoPumpSetpointLimits(); // Use the defaults in the class

                PumpDisplays[indx].MinFlowSp = setpointLimits.MinFlowSp;
                PumpDisplays[indx].MaxFlowSp = setpointLimits.MaxFlowSp;
                PumpDisplays[indx].MinPressSp = setpointLimits.MinPressSp;
                PumpDisplays[indx].MaxPressSp = setpointLimits.MaxPressSp;
                PumpDisplays[indx].MaxFlowLimit = setpointLimits.MaxFlowLimit;

                RefillRates[indx].MaxRefillRate = rangeData.MaxRefillRate;

                RefillRates[indx].RefillRate = pump.GetPumpData(indx).RefillRate;
            }

            // Fill in limits display
            UpdateLimitDisplay();

            //// Clear the pump displays
            //mcontrol_IscoGraphs.ClearGraphs();
        }

        /// <summary>
        /// OperationModeSet handler
        /// </summary>
        /// <param name="newMode"></param>
        private void Pump_OperationModeSet(IscoOperationMode newMode)
        {
            OperationMode = newMode;
        }

        /// <summary>
        /// ControlModeSet handler
        /// </summary>
        /// <param name="newMode"></param>
        private void Pump_ControlModeSet(IscoControlMode newMode)
        {
            if (newMode == IscoControlMode.Local)
            {
                ControlMode = IscoControlMode.Local;
            }
            else
            {
                ControlMode = IscoControlMode.Remote;
            }
        }

        /// <summary>
        /// Handles pump class disconnect
        /// </summary>
        private void Pump_Disconnected()
        {
            //Included for future use
        }

#if DACTEST
        private void Pump_Error(object sender, DeviceErrorEventArgs e)
        {
            string msg = "ERROR: " + e.Notification;
            LogDebugMessage(msg);
        }

        private void Pump_StatusUpdate(object sender, DeviceStatusEventArgs e)
        {
            string msg = "STATUS: " + e.Notification;
            LogDebugMessage(msg);
        }
#endif

        #endregion
    }
}
