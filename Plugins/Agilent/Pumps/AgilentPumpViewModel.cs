using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DynamicData;
using LcmsNetCommonControls.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetCommonControls.Devices.Pumps;
using LcmsNetCommonControls.ViewModels;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;
using Microsoft.Win32;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        private const string CONST_PUMP_METHOD_PATH = "PumpMethods";

        /// <summary>
        /// The default Constructor.
        /// </summary>
        public AgilentPumpViewModel()
        {
            ModeComboBoxOptions = Enum.GetValues(typeof(AgilentPumpModes)).Cast<AgilentPumpModes>().ToList().AsReadOnly();
            methodComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodComboBoxOptionsBound).Subscribe();
            MethodComboBoxOptions = methodComboBoxOptionsBound;

            pumpDisplay = new PumpDisplayViewModel("Unknown");
            pumpPopoutVm = new PopoutViewModel(pumpDisplay);

            pumpModel = this.WhenAnyValue(x => x.Pump, x => x.Pump.PumpModel).Select(x => x.Item2).ToProperty(this, nameof(PumpModel), "", true, RxApp.MainThreadScheduler);
            pumpSerial = this.WhenAnyValue(x => x.Pump, x => x.Pump.PumpSerial).Select(x => x.Item2).ToProperty(this, nameof(PumpSerial), "", true, RxApp.MainThreadScheduler);
            pumpFirmware = this.WhenAnyValue(x => x.Pump, x => x.Pump.PumpFirmware).Select(x => x.Item2).ToProperty(this, nameof(PumpFirmware), "", true, RxApp.MainThreadScheduler);
            pumpState = this.WhenAnyValue(x => x.Pump, x => x.Pump.PumpState).Select(x => x.Item2).ToProperty(this, nameof(PumpState), PumpState.Unknown, true, RxApp.MainThreadScheduler);

            SetFlowRateCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetFlowRate(FlowRate)));
            ReadFlowRateCommand = ReactiveCommand.Create(() => FlowRateRead = Pump.GetActualFlow());
            //SetMixerVolumeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetMixerVolume(MixerVolume)));
            //ReadMixerVolumeCommand = ReactiveCommand.Create(() => MixerVolumeRead = Pump.GetMixerVolume());
            SetPercentBCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPercentB(PercentB)));
            ReadPercentBCommand = ReactiveCommand.Create(() => PercentBRead = Pump.GetPercentB());
            SetModeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetMode(SelectedMode)));
            ReadPressureCommand = ReactiveCommand.Create(() => Pressure = Pump.GetPressure());
            ReadAllCommand = ReactiveCommand.CreateCombined(new[] { ReadFlowRateCommand, ReadPercentBCommand, ReadPressureCommand });
            PumpOnCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOn()), this.WhenAnyValue(x => x.PumpState).Select(x => x != PumpState.On));
            PumpOffCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOff()), this.WhenAnyValue(x => x.PumpState).Select(x => x != PumpState.Off));
            PumpStandbyCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpStandby()), this.WhenAnyValue(x => x.PumpState).Select(x => x != PumpState.Standby));
            PurgePumpCommand = ReactiveCommand.Create(PurgePump);
            StartPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StartPump), this.WhenAnyValue(x => x.PumpState, x => x.PumpStatus.NotReadyState).Select(x => x.Item1 != PumpState.Off && x.Item1 != PumpState.Standby && x.Item2 == AgilentPumpStateNotReady.READY));
            StopPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.StopMethod()));
            SetComPortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetComPortName));
            ReadMethodFromPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => MethodText = Pump.RetrieveMethod()));
            LoadMethodsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(LoadMethods));
            SaveMethodCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SaveMethod));
            SetModuleDateCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetModuleDateTime()));
            SetModuleNameCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetModuleName(NewModuleName)), this.WhenAnyValue(x => x.PumpInfo.ModuleName, x => x.NewModuleName).Select(x => !string.Equals(x.Item1, x.Item2) && x.Item2.Length <= 30));
            RefreshInfoCommand = ReactiveCommand.Create(() => Pump.GetPumpInformation());
            RefreshStatusCommand = ReactiveCommand.Create(() =>
            {
                Pump.GetPumpStatus();
                Pump.GetPumpState();
            });
            IdentifyCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.Identify()));

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
            PumpInfo.PumpInfo = Pump?.PumpInfo;
            PumpStatus.PumpStatus = Pump?.PumpStatus;

            // Initialize the underlying device class
            if (Pump != null)
            {
                Pump.MonitoringDataReceived += Pump_MonitoringDataReceived;
                Pump.PropertyChanged += PumpOnPropertyChanged;
                Pump.MethodNames += PumpOnMethodNames;

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
        private AgilentPumpModes selectedMode;
        private double pressure;
        private readonly SourceList<string> methodComboBoxOptions = new SourceList<string>();
        private string selectedMethod = "";
        private string selectedComPort = "";
        private string methodText = "";
        private readonly PumpDisplayViewModel pumpDisplay = null;
        private readonly PopoutViewModel pumpPopoutVm;
        private string newModuleName = "";
        private AgilentPumpPurgeWindow purgeWindow = null;
        private readonly ObservableAsPropertyHelper<string> pumpModel;
        private readonly ObservableAsPropertyHelper<string> pumpSerial;
        private readonly ObservableAsPropertyHelper<string> pumpFirmware;
        private readonly ObservableAsPropertyHelper<PumpState> pumpState;

        public ReadOnlyCollection<AgilentPumpModes> ModeComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> MethodComboBoxOptions { get; }
        public ReadOnlyObservableCollection<SerialPortData> ComPortComboBoxOptions => SerialPortGenericData.SerialPorts;

        public string PumpModel => pumpModel?.Value ?? "";
        public string PumpSerial => pumpSerial?.Value ?? "";
        public string PumpFirmware => pumpFirmware?.Value ?? "";
        public PumpState PumpState => pumpState?.Value ?? PumpState.Unknown;

        public double FlowRate
        {
            get => flowRate;
            set => this.RaiseAndSetIfChanged(ref flowRate, value);
        }

        public double FlowRateRead
        {
            get => flowRateRead;
            private set => this.RaiseAndSetIfChanged(ref flowRateRead, value);
        }

        //public double MixerVolume
        //{
        //    get => mixerVolume;
        //    set => this.RaiseAndSetIfChanged(ref mixerVolume, value);
        //}

        //public double MixerVolumeRead
        //{
        //    get => mixerVolumeRead;
        //    private set => this.RaiseAndSetIfChanged(ref mixerVolumeRead, value);
        //}

        public double PercentB
        {
            get => percentB;
            set => this.RaiseAndSetIfChanged(ref percentB, value);
        }

        public double PercentBRead
        {
            get => percentBRead;
            private set => this.RaiseAndSetIfChanged(ref percentBRead, value);
        }

        public AgilentPumpModes SelectedMode
        {
            get => selectedMode;
            set => this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        public double Pressure
        {
            get => pressure;
            private set => this.RaiseAndSetIfChanged(ref pressure, value);
        }

        public string SelectedMethod
        {
            get => selectedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
        }

        public string SelectedComPort
        {
            get => selectedComPort;
            set => this.RaiseAndSetIfChanged(ref selectedComPort, value);
        }

        public string MethodText
        {
            get => methodText;
            set => this.RaiseAndSetIfChanged(ref methodText, value);
        }

        public PumpDisplayViewModel PumpDisplay => pumpDisplay;
        public PopoutViewModel PumpPopoutVm => pumpPopoutVm;
        public AgilentPumpInfoViewModel PumpInfo { get; } = new AgilentPumpInfoViewModel();
        public AgilentPumpStatusViewModel PumpStatus { get; } = new AgilentPumpStatusViewModel();

        /// <summary>
        /// The associated device.
        /// </summary>
        public override IDevice Device
        {
            get => Pump;
            set => RegisterDevice(value);
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
            get => Pump.Emulation;
            set => Pump.Emulation = value;
        }

        public string NewModuleName
        {
            get => newModuleName;
            set => this.RaiseAndSetIfChanged(ref newModuleName, value);
        }

        public ReactiveCommand<Unit, Unit> SetFlowRateCommand { get; }
        public ReactiveCommand<Unit, double> ReadFlowRateCommand { get; }
        //public ReactiveCommand<Unit, Unit> SetMixerVolumeCommand { get; }
        //public ReactiveCommand<Unit, double> ReadMixerVolumeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetPercentBCommand { get; }
        public ReactiveCommand<Unit, double> ReadPercentBCommand { get; }
        public ReactiveCommand<Unit, Unit> SetModeCommand { get; }
        public ReactiveCommand<Unit, double> ReadPressureCommand { get; }
        public CombinedReactiveCommand<Unit, double> ReadAllCommand { get; }
        public ReactiveCommand<Unit, Unit> PumpOnCommand { get; }
        public ReactiveCommand<Unit, Unit> PumpOffCommand { get; }
        public ReactiveCommand<Unit, Unit> PumpStandbyCommand { get; }
        public ReactiveCommand<Unit, Unit> PurgePumpCommand { get; }
        public ReactiveCommand<Unit, Unit> StartPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> StopPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> SetComPortCommand { get; }
        public ReactiveCommand<Unit, string> ReadMethodFromPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadMethodsCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> SetModuleDateCommand { get; }
        public ReactiveCommand<Unit, Unit> SetModuleNameCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> IdentifyCommand { get; }

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
            methodComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(data.Select(x => x.ToString()));
            });

            // Make sure one method is selected.
            if (methodComboBoxOptions.Count > 0 && !methodComboBoxOptions.Items.Contains(SelectedMethod))
            {
                SelectedMethod = methodComboBoxOptions.Items.First();
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
            var path = PersistDataPaths.GetDirectoryLoadPathCheckFiles(CONST_PUMP_METHOD_PATH, "*.txt");
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
                var method = System.IO.File.ReadAllText(filename).Trim('\r', '\n', '\'', '"');
                methods[System.IO.Path.GetFileNameWithoutExtension(filename)] = method;
            }

            // Clear any existing pump methods
            if (methods.Count > 0)
            {
                methodComboBoxOptions.Clear();

                RxApp.MainThreadScheduler.Schedule(() => {
                    Pump.ClearMethods();
                    Pump.AddMethods(methods);
                });
            }

            if (methodSelected != null)
            {
                // try to select the last selected method, if it has been loaded back in to the system.
                SelectedMethod = MethodComboBoxOptions.Contains(methodSelected) ? methodSelected : "";
            }
        }

        private readonly Random r = new Random();
        private readonly Timer timer;

        private void TimerTick(object sender)
        {
            //#if DEBUG
            if (Pump != null && Emulation)
            {
                timer.Change(Pump.TotalMonitoringSecondsElapsed * 1000, Pump.TotalMonitoringSecondsElapsed * 1000);
                // Multiply the value by 100 to get the range of 0-100 instead of 0-1
                Pump.PushData(r.NextDouble() * 100, r.NextDouble() * 100, r.NextDouble() * 100);
            }
            //#endif
        }

        /// <summary>
        /// Saves the Pump method to file.
        /// </summary>
        private void SaveMethod()
        {
            if (string.IsNullOrEmpty(MethodText))
                return;

            var dialog = new SaveFileDialog
            {
                InitialDirectory = PersistDataPaths.GetDirectorySavePath(CONST_PUMP_METHOD_PATH),
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
    }
}
