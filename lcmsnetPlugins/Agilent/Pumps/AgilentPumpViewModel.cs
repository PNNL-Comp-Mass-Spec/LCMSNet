using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetCommonControls.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetCommonControls.Devices.Pumps;
using LcmsNetCommonControls.ViewModels;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;
using Microsoft.Win32;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        private const string CONST_PUMP_METHOD_PATH = "pumpmethods";

        #region Constructors

        /// <summary>
        /// The default Constructor.
        /// </summary>
        public AgilentPumpViewModel()
        {
            pumpDisplay = new PumpDisplayViewModel("Unknown");
            pumpPopoutVm = new PopoutViewModel(pumpDisplay);
            SetupCommands();
            timer = new Timer(TimerTick, this, 1000, 1000);
            NewModuleName = "";
        }

        ~AgilentPumpViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            timer?.Dispose();
            purgeWindow?.Close();
            GC.SuppressFinalize(this);
        }

        private void RegisterDevice(IDevice device)
        {
            Pump = device as AgilentPump;
            this.RaisePropertyChanged(nameof(PumpInfo));
            this.RaisePropertyChanged(nameof(PumpStatus));

            // Initialize the underlying device class
            if (Pump != null)
            {
                Pump.MonitoringDataReceived += Pump_MonitoringDataReceived;
                Pump.PropertyChanged += PumpOnPropertyChanged;
                Pump.MethodNames += PumpOnMethodNames;
                using (modeComboBoxOptions.SuppressChangeNotifications())
                {
                    modeComboBoxOptions.Clear();
                    modeComboBoxOptions.AddRange(Enum.GetValues(typeof(AgilentPumpModes)).Cast<AgilentPumpModes>());
                }

                PumpDisplay.SetPumpName(Pump.Name);
                NewMethodAvailable += PumpAgilent_NewMethodAvailable;
            }

            // Add to the device manager.
            SetBaseDevice(Pump);

            // Make sure to select an option
            SelectedComPort = Pump.PortName;

            // Reads the pump method directory.
            try
            {
                ReadMethodDirectory();
            }
            catch
            {
                //TODO: Update errors!
            }

            if (!string.IsNullOrWhiteSpace(PumpInfo.ModuleName))
            {
                NewModuleName = PumpInfo.ModuleName;
            }
            else
            {
                NewModuleName = Pump.Name;
            }

            InitializePlots();
        }

        private void PumpOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Pump.Name)))
            {
                PumpDisplay.SetPumpName(Pump.Name);
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// A pump object to use.
        /// </summary>
        private AgilentPump pump;

        /// <summary>
        /// Fired when a new method is available from the pumps.
        /// </summary>
        public static event EventHandler NewMethodAvailable;

        private double flowRate;
        private double flowRateRead;
        //private double mixerVolume;
        //private double mixerVolumeRead;
        private double percentB;
        private double percentBRead;
        private readonly ReactiveList<AgilentPumpModes> modeComboBoxOptions = new ReactiveList<AgilentPumpModes>();
        private AgilentPumpModes selectedMode;
        private double pressure;
        private readonly ReactiveList<string> methodComboBoxOptions = new ReactiveList<string>();
        private string selectedMethod = "";
        private string selectedComPort = "";
        private string methodText = "";
        private readonly PumpDisplayViewModel pumpDisplay = null;
        private readonly PopoutViewModel pumpPopoutVm;
        private string newModuleName;
        private AgilentPumpPurgeWindow purgeWindow = null;

        #endregion

        #region Properties

        public IReadOnlyReactiveList<AgilentPumpModes> ModeComboBoxOptions => modeComboBoxOptions;
        public IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;
        public IReadOnlyReactiveList<SerialPortData> ComPortComboBoxOptions => SerialPortGenericData.SerialPorts;

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

        //public double MixerVolume
        //{
        //    get { return mixerVolume; }
        //    set { this.RaiseAndSetIfChanged(ref mixerVolume, value); }
        //}

        //public double MixerVolumeRead
        //{
        //    get { return mixerVolumeRead; }
        //    private set { this.RaiseAndSetIfChanged(ref mixerVolumeRead, value); }
        //}

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

        public AgilentPumpModes SelectedMode
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
        public AgilentPumpInfo PumpInfo => Pump?.PumpInfo;
        public AgilentPumpStatus PumpStatus => Pump?.PumpStatus;

        /// <summary>
        /// The associated device.
        /// </summary>
        public override IDevice Device
        {
            get { return Pump; }
            set { RegisterDevice(value); }
        }

        public AgilentPump Pump
        {
            get => pump;
            private set => this.RaiseAndSetIfChanged(ref pump, value);
        }

        /// <summary>
        /// Determines whether or not pump is in emulation mode
        /// </summary>
        public bool Emulation
        {
            get { return Pump.Emulation; }
            set { Pump.Emulation = value; }
        }

        public string NewModuleName
        {
            get => newModuleName;
            set => this.RaiseAndSetIfChanged(ref newModuleName, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SetFlowRateCommand { get; private set; }
        public ReactiveCommand<Unit, double> ReadFlowRateCommand { get; private set; }
        //public ReactiveCommand<Unit, Unit> SetMixerVolumeCommand { get; private set; }
        //public ReactiveCommand<Unit, double> ReadMixerVolumeCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetPercentBCommand { get; private set; }
        public ReactiveCommand<Unit, double> ReadPercentBCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetModeCommand { get; private set; }
        public ReactiveCommand<Unit, double> ReadPressureCommand { get; private set; }
        public CombinedReactiveCommand<Unit, double> ReadAllCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpOnCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpOffCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpStandbyCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PurgePumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StartPumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StopPumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetComPortCommand { get; private set; }
        public ReactiveCommand<Unit, string> ReadMethodFromPumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadMethodsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveMethodCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetModuleDateCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetModuleNameCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RefreshInfoCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> IdentifyCommand { get; private set; }

        private void SetupCommands()
        {
            SetFlowRateCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetFlowRate(FlowRate)));
            ReadFlowRateCommand = ReactiveCommand.Create(() => FlowRateRead = Pump.GetActualFlow());
            //SetMixerVolumeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetMixerVolume(MixerVolume)));
            //ReadMixerVolumeCommand = ReactiveCommand.Create(() => MixerVolumeRead = Pump.GetMixerVolume());
            SetPercentBCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPercentB(PercentB)));
            ReadPercentBCommand = ReactiveCommand.Create(() => PercentBRead = Pump.GetPercentB());
            SetModeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetMode(SelectedMode)));
            ReadPressureCommand = ReactiveCommand.Create(() => Pressure = Pump.GetPressure());
            ReadAllCommand = ReactiveCommand.CreateCombined(new [] {ReadFlowRateCommand, ReadPercentBCommand, ReadPressureCommand});
            PumpOnCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOn()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.On));
            PumpOffCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOff()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.Off));
            PumpStandbyCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpStandby()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.Standby));
            PurgePumpCommand = ReactiveCommand.Create(() => PurgePump());
            StartPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => StartPump()), this.WhenAnyValue(x => x.Pump.PumpState, x => x.PumpStatus.NotReadyState).Select(x => x.Item1 != PumpState.Off && x.Item1 != PumpState.Standby && x.Item2 == AgilentPumpStateNotReady.READY));
            StopPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.StopMethod()));
            SetComPortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetComPortName()));
            ReadMethodFromPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => MethodText = Pump.RetrieveMethod()));
            LoadMethodsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => LoadMethods()));
            SaveMethodCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SaveMethod()));
            SetModuleDateCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetModuleDateTime()));
            SetModuleNameCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetModuleName(NewModuleName)), this.WhenAnyValue(x => x.PumpInfo.ModuleName, x => x.NewModuleName).Select(x => !string.Equals(x.Item1, x.Item2) && x.Item2.Length <= 30));
            RefreshInfoCommand = ReactiveCommand.Create(() => Pump.GetPumpInformation());
            RefreshStatusCommand = ReactiveCommand.Create(() =>
            {
                Pump.GetPumpStatus();
                Pump.GetPumpState();
            });
            IdentifyCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.Identify()));
        }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new AgilentPumpView();
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
        /// Handles when new pump method names are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void PumpOnMethodNames(object sender, List<object> data)
        {
            using (methodComboBoxOptions.SuppressChangeNotifications())
            {
                methodComboBoxOptions.Clear();
                methodComboBoxOptions.AddRange(data.Select(x => x.ToString()));
            }

            // Make sure one method is selected.
            if (methodComboBoxOptions.Count > 0 && !methodComboBoxOptions.Contains(SelectedMethod))
            {
                SelectedMethod = methodComboBoxOptions[0];
            }
        }

        /// <summary>
        /// Reads the pump method directory and alerts the pumps of new methods to run.
        /// </summary>
        private void ReadMethodDirectory()
        {
            // The reason we don't just add stuff straight into the user interface here, is to maintain the
            // design pattern that things propagate events to us, since we are not in charge of managing the
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

            var methods = new Dictionary<string, string>();
            foreach (var filename in filenames)
            {
                var method = System.IO.File.ReadAllText(filename);
                methods[System.IO.Path.GetFileNameWithoutExtension(filename)] = method;
            }

            RxApp.MainThreadScheduler.Schedule(() => {
                // Clear any existing pump methods
                if (methods.Count > 0)
                {
                    Pump.ClearMethods();
                    using (methodComboBoxOptions.SuppressChangeNotifications())
                    {
                        methodComboBoxOptions.Clear();
                    }

                    Pump.AddMethods(methods);
                }
            });

            if (methodSelected != null)
            {
                // try to select the last selected method, if it has been loaded back in to the system.
                SelectedMethod = MethodComboBoxOptions.Contains(methodSelected) ? methodSelected : "";
            }
        }

        private void ReplacePumpMethods(Dictionary<string, string> methods)
        {
        }

        private readonly Random r = new Random();
        private readonly Timer timer;

        private void TimerTick(object sender)
        {
            //#if DEBUG
            if (Pump != null && Emulation)
            {
                timer.Change(Pump.TotalMonitoringSecondsElapsed * 1000, Pump.TotalMonitoringSecondsElapsed * 1000);
                Pump.PushData(r.NextDouble(), r.NextDouble(), r.NextDouble());
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
            Pump.AddMethod(System.IO.Path.GetFileNameWithoutExtension(dialog.FileName), MethodText);

            NewMethodAvailable?.Invoke(this, null);
        }

        /// <summary>
        /// Starts the pumps currently loaded time table.
        /// </summary>
        private void StartPump()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
                return;

            Pump.StartMethod(SelectedMethod);
        }

        private void LoadMethods()
        {
            try
            {
                ReadMethodDirectory();
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                ApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        private void PurgePump()
        {
            Pump.LoadPurgeData();
            if (purgeWindow == null)
            {
                var vm = new AgilentPumpPurgeViewModel(Pump);
                purgeWindow = new AgilentPumpPurgeWindow() {DataContext = vm};
                purgeWindow.Closed += PurgeWindowClosed;
                purgeWindow.Show();
            }
            else
            {
                purgeWindow.Activate();
            }
        }

        private void PurgeWindowClosed(object sender, EventArgs e)
        {
            purgeWindow.Closed -= PurgeWindowClosed;
            purgeWindow = null;
        }

        private void SetComPortName()
        {
            if (string.IsNullOrWhiteSpace(SelectedComPort))
            {
                return;
            }

            Pump.PortName = SelectedComPort;
        }

        #endregion
    }
}
