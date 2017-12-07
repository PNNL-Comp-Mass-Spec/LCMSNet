using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices.Pumps;
using LcmsNetCommonControls.ViewModels;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;
using Microsoft.Win32;

namespace Agilent.Devices.Pumps
{
    public class PumpAgilentViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        private const string CONST_PUMP_METHOD_PATH = "pumpmethods";

        #region Constructors

        /// <summary>
        /// The default Constructor.
        /// </summary>
        public PumpAgilentViewModel()
        {
            pumpDisplay = new PumpDisplayViewModel("Unknown");
            pumpPopoutVm = new PopoutViewModel(pumpDisplay);
            SetupCommands();
            timer = new Timer(TimerTick, this, 1000, 1000);
        }

        private void RegisterDevice(IDevice device)
        {
            m_pump = device as classPumpAgilent;

            // Initialize the underlying device class
            if (m_pump != null)
            {
                m_pump.MethodAdded += Pump_MethodAdded;
                m_pump.MethodUpdated += Pump_MethodUpdated;
                m_pump.MonitoringDataReceived += Pump_MonitoringDataReceived;
                m_pump.PropertyChanged += PumpOnPropertyChanged;
                using (modeComboBoxOptions.SuppressChangeNotifications())
                {
                    modeComboBoxOptions.Clear();
                    modeComboBoxOptions.AddRange(Enum.GetValues(typeof(enumPumpAgilentModes)).Cast<enumPumpAgilentModes>());
                }

                PumpDisplay.SetPumpName(m_pump.Name);
                NewMethodAvailable += PumpAgilent_NewMethodAvailable;
            }

            // Add to the device manager.
            SetBaseDevice(m_pump);

            // Add a list of available serial port names to the combo box.
            using (comPortComboBoxOptions.SuppressChangeNotifications())
            {
                comPortComboBoxOptions.Clear();
                comPortComboBoxOptions.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            }

            if (ComPortComboBoxOptions.Count > 0)
            {
                SelectedComPort = ComPortComboBoxOptions[0];
            }
            if (ComPortComboBoxOptions.Contains(m_pump.PortName))
            {
                SelectedComPort = m_pump.PortName;
            }

            // Reads the pump method directory.
            try
            {
                ReadMethodDirectory();
            }
            catch
            {
                //TODO: Update errors!
            }

            InitializePlots();
        }

        private void PumpOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(m_pump.Name)))
            {
                PumpDisplay.SetPumpName(m_pump.Name);
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// A pump object to use.
        /// </summary>
        private classPumpAgilent m_pump;

        /// <summary>
        /// Fired when a new method is available from the pumps.
        /// </summary>
        public static event EventHandler NewMethodAvailable;

        private double flowRate;
        private double flowRateRead;
        private double mixerVolume;
        private double mixerVolumeRead;
        private double percentB;
        private double percentBRead;
        private readonly ReactiveUI.ReactiveList<enumPumpAgilentModes> modeComboBoxOptions = new ReactiveUI.ReactiveList<enumPumpAgilentModes>();
        private enumPumpAgilentModes selectedMode;
        private double pressure;
        private readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private string selectedMethod = "";
        private readonly ReactiveUI.ReactiveList<string> comPortComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private string selectedComPort = "";
        private string methodText = "";
        private readonly PumpDisplayViewModel pumpDisplay = null;
        private readonly PopoutViewModel pumpPopoutVm;

        #endregion

        #region Properties

        public ReactiveUI.IReadOnlyReactiveList<enumPumpAgilentModes> ModeComboBoxOptions => modeComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> ComPortComboBoxOptions => comPortComboBoxOptions;

        public double FlowRate
        {
            get { return flowRate; }
            set { this.RaiseAndSetIfChanged(ref flowRate, value); }
        }

        public double FlowRateRead
        {
            get { return flowRateRead; }
            private set { this.RaiseAndSetIfChanged(ref flowRateRead, value); }
        }

        public double MixerVolume
        {
            get { return mixerVolume; }
            set { this.RaiseAndSetIfChanged(ref mixerVolume, value); }
        }

        public double MixerVolumeRead
        {
            get { return mixerVolumeRead; }
            private set { this.RaiseAndSetIfChanged(ref mixerVolumeRead, value); }
        }

        public double PercentB
        {
            get { return percentB; }
            set { this.RaiseAndSetIfChanged(ref percentB, value); }
        }

        public double PercentBRead
        {
            get { return percentBRead; }
            private set { this.RaiseAndSetIfChanged(ref percentBRead, value); }
        }

        public enumPumpAgilentModes SelectedMode
        {
            get { return selectedMode; }
            set { this.RaiseAndSetIfChanged(ref selectedMode, value); }
        }

        public double Pressure
        {
            get { return pressure; }
            private set { this.RaiseAndSetIfChanged(ref pressure, value); }
        }

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public string SelectedComPort
        {
            get { return selectedComPort; }
            set { this.RaiseAndSetIfChanged(ref selectedComPort, value); }
        }

        public string MethodText
        {
            get { return methodText; }
            set { this.RaiseAndSetIfChanged(ref methodText, value); }
        }

        public PumpDisplayViewModel PumpDisplay => pumpDisplay;
        public PopoutViewModel PumpPopoutVm => pumpPopoutVm;

        /// <summary>
        /// The associated device.
        /// </summary>
        public IDevice Device
        {
            get { return m_pump; }
            set { RegisterDevice(value); }
        }

        /// <summary>
        /// Determines whether or not pump is in emulation mode
        /// </summary>
        public bool Emulation
        {
            get { return m_pump.Emulation; }
            set { m_pump.Emulation = value; }
        }

        #endregion

        #region Commands

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetFlowRateCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, double> ReadFlowRateCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetMixerVolumeCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, double> ReadMixerVolumeCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetPercentBCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, double> ReadPercentBCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetModeCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, double> ReadPressureCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> PumpOnCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> PumpOffCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> PurgePumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StartPumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopPumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetComPortCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, string> ReadMethodFromPumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> LoadMethodsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SaveMethodCommand { get; private set; }

        private void SetupCommands()
        {
            SetFlowRateCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.SetFlowRate(FlowRate));
            ReadFlowRateCommand = ReactiveUI.ReactiveCommand.Create(() => FlowRateRead = m_pump.GetActualFlow());
            SetMixerVolumeCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.SetMixerVolume(MixerVolume));
            ReadMixerVolumeCommand = ReactiveUI.ReactiveCommand.Create(() => MixerVolumeRead = m_pump.GetMixerVolume());
            SetPercentBCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.SetPercentB(PercentB));
            ReadPercentBCommand = ReactiveUI.ReactiveCommand.Create(() => PercentBRead = m_pump.GetPercentB());
            SetModeCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.SetMode(SelectedMode));
            ReadPressureCommand = ReactiveUI.ReactiveCommand.Create(() => Pressure = m_pump.GetPressure());
            PumpOnCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.PumpOn());
            PumpOffCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.PumpOff());
            PurgePumpCommand = ReactiveUI.ReactiveCommand.Create(() => PurgePump());
            StartPumpCommand = ReactiveUI.ReactiveCommand.Create(() => StartPump());
            StopPumpCommand = ReactiveUI.ReactiveCommand.Create(() => m_pump.StopMethod());
            SetComPortCommand = ReactiveUI.ReactiveCommand.Create(() => SetComPortName());
            ReadMethodFromPumpCommand = ReactiveUI.ReactiveCommand.Create(() => MethodText = m_pump.RetrieveMethod());
            LoadMethodsCommand = ReactiveUI.ReactiveCommand.Create(() => LoadMethods());
            SaveMethodCommand = ReactiveUI.ReactiveCommand.Create(() => SaveMethod());
        }

        #endregion

        #region Methods

        public UserControl GetDefaultView()
        {
            return new PumpAgilentView();
        }

        ///// <summary>
        ///// Removes the device from the device manager.
        ///// </summary>
        ///// <returns></returns>
        //public override bool RemoveDevice()
        //{
        //    return classDeviceManager.Manager.RemoveDevice(this.Device);
        //}

        #endregion

        #region Plotting and Monitoring Data Handling

        /// <summary>
        /// Initialize the plots for monitoring data.
        /// </summary>
        public void InitializePlots()
        {
        }

        /// <summary>
        /// Handles the event when data is received from the pumps.
        /// </summary>
        private void Pump_MonitoringDataReceived(object sender, PumpDataEventArgs args)
        {
            PumpDisplay.DisplayMonitoringData(sender, args);
        }

        #endregion

        #region Pump Event Handlers and methods

        private void PumpAgilent_NewMethodAvailable(object sender, EventArgs e)
        {
            if (sender != null && sender != this)
            {
                //TODO: Error handling
                ReadMethodDirectory();
            }
        }

        /// <summary>
        /// Handles when a pump method is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pump_MethodUpdated(object sender, classPumpMethodEventArgs e)
        {
            methodComboBoxOptions.Add(e.MethodName);
        }

        /// <summary>
        /// Handles when a pump method is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pump_MethodAdded(object sender, classPumpMethodEventArgs e)
        {
            methodComboBoxOptions.Add(e.MethodName);

            // Make sure one method is selected.
            if (MethodComboBoxOptions.Count == 1)
                SelectedMethod = MethodComboBoxOptions[0];
        }

        /// <summary>
        /// Reads the pump method directory and alerts the pumps of new methods to run.
        /// </summary>
        private void ReadMethodDirectory()
        {
            // The reason we don't just add stuff straight into the user interface here, is to maintain the
            // design pattern that things propogate events to us, since we are not in charge of managing the
            // data.  We will catch an event from adding a method that one was added...and thus update
            // the user interface intrinsically.
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                CONST_PUMP_METHOD_PATH);
            if (!System.IO.Directory.Exists(path))
            {
                throw new System.IO.DirectoryNotFoundException("The directory " + path + " does not exist.");
            }

            var filenames = System.IO.Directory.GetFiles(path, "*.txt");

            string methodSelected = null;
            if (MethodComboBoxOptions.Count > 0)
            {
                methodSelected = SelectedMethod;
            }

            // Clear any existing pump methods
            if (filenames.Length > 0)
                m_pump.ClearMethods();
            var methods = new Dictionary<string, string>();
            foreach (var filename in filenames)
            {
                var method = System.IO.File.ReadAllText(filename);
                methods[System.IO.Path.GetFileNameWithoutExtension(filename)] = method;
                m_pump.AddMethod(System.IO.Path.GetFileNameWithoutExtension(filename), method);
            }
            //m_pump.AddMethods(methods);
            if (methodSelected != null)
            {
                // try to select the last selected method, if it has been loaded back in to the system.
                SelectedMethod = ComPortComboBoxOptions.Contains(methodSelected) ? methodSelected : "";
            }
        }

        private readonly Random r = new Random();
        private readonly Timer timer;

        private void TimerTick(object sender)
        {
            //#if DEBUG
            if (m_pump != null && Emulation)
            {
                timer.Change(m_pump.TotalMonitoringSecondElapsed * 1000, m_pump.TotalMonitoringSecondElapsed * 1000);
                m_pump.PushData(r.NextDouble(), r.NextDouble(), r.NextDouble());
            }
            //#endif
        }

        #endregion

        #region Control Event Handlers


        /// <summary>
        /// Saves the Pump method to file.
        /// </summary>
        private void SaveMethod()
        {
            if (string.IsNullOrEmpty(MethodText))
                return;

            var dialog = new SaveFileDialog
            {
                InitialDirectory = CONST_PUMP_METHOD_PATH,
                FileName = "pumpMethod",
                DefaultExt = ".txt"
            };
            var result = dialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            System.IO.TextWriter writer = System.IO.File.CreateText(dialog.FileName);
            writer.Write(MethodText);
            writer.Close();

            // Make sure we add it to the list of methods as well.
            m_pump.AddMethod(System.IO.Path.GetFileNameWithoutExtension(dialog.FileName), MethodText);

            NewMethodAvailable?.Invoke(this, null);
        }

        /// <summary>
        /// Starts the pumps currently loaded time table.
        /// </summary>
        private void StartPump()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
                return;

            m_pump.StartMethod(SelectedMethod);
        }

        private void LoadMethods()
        {
            try
            {
                ReadMethodDirectory();
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                classApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        private void PurgePump()
        {
            var vm = new AgilentPumpPurgeViewModel(m_pump);
            var v = new AgilentPumpPurgeWindow() {DataContext = vm};
            v.ShowDialog();
        }

        private void SetComPortName()
        {
            if (string.IsNullOrWhiteSpace(SelectedComPort))
            {
                return;
            }

            m_pump.PortName = SelectedComPort;
        }

        #endregion
    }
}
