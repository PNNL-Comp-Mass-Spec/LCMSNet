using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LcmsNetCommonControls.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.PALAutoSampler.Pal
{
    public class PalViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        #region Constructors

        /// <summary>
        /// The main constructor. Creates the PAL class and initializes the port.
        /// </summary>
        public PalViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            vialRangeComboBoxOptions = new ReactiveList<VialRanges>(Enum.GetValues(typeof(VialRanges)).Cast<VialRanges>());

            this.PropertyChanged += PalViewModel_PropertyChanged;

            RefreshMethodListCommand = ReactiveCommand.CreateFromTask(RefreshMethods);
            RunMethodCommand = ReactiveCommand.CreateFromTask(RunMethod);
            StopMethodCommand = ReactiveCommand.CreateFromTask(StopMethod);
            RefreshStatusCommand = ReactiveCommand.CreateFromTask(RefreshStatus);
            ApplyPortNameCommand = ReactiveCommand.Create(ApplyPortName);
            SelectVialsCommand = ReactiveCommand.CreateFromTask(SelectVials);
            ResetPalCommand = ReactiveCommand.CreateFromTask(ResetPal);
            FullResetPalCommand = ReactiveCommand.CreateFromTask(FullyResetPal);
        }

        ~PalViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            TimeoutMonitor(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers the device events and user interface.
        /// </summary>
        /// <param name="device"></param>
        private void RegisterDevice(IDevice device)
        {
            Pal = device as Pal;

            if (Pal != null)
            {
                Pal.DeviceSaveRequired += Pal_DeviceSaveRequired;
                Pal.Free += OnFree;

                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    ProcessTrays(Pal.TrayNames);
                    ProcessMethods(Pal.MethodNames);
                    ProcessTraysAndMaxVials(Pal.TrayNamesAndMaxVials);
                });
            }

            SetBaseDevice(Pal);
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that the device is available to take commands
        /// </summary>
        public event Action Free;

        #endregion

        #region Members

        /// <summary>
        /// The class which controls the PAL itself.
        /// </summary>
        private Pal pal;

        private readonly ReactiveList<string> methodComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> trayComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<VialRanges> vialRangeComboBoxOptions;
        private readonly ReactiveList<string> trayNamesAndMaxVial = new ReactiveList<string>();
        private readonly bool isInDesignMode = false;
        private string selectedMethod = "";
        private string selectedTray = "";
        private int maxVialForTray = 1;
        private int vialNumber = 1;
        private int volume = 1;
        private string statusText = "";
        private string selectedPortName = "";
        private bool monitorStatus = false;
        private string trayNamesAndMaxVialFormatted = "";
        private string selectVialsInput;
        private string selectVialsTray;
        private string selectVialsOutput;
        private string timeReport = "";

        #endregion

        #region Properties

        public IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;
        public IReadOnlyReactiveList<string> TrayComboBoxOptions => trayComboBoxOptions;
        public IReadOnlyReactiveList<VialRanges> VialRangeComboBoxOptions => vialRangeComboBoxOptions;
        public IReadOnlyReactiveList<SerialPortData> PortNamesComboBoxOptions => SerialPortGenericData.SerialPorts;
        public IReadOnlyReactiveList<string> TrayNamesAndMaxVial => trayNamesAndMaxVial;

        public string SelectedMethod
        {
            get => selectedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
        }

        public string SelectedTray
        {
            get => selectedTray;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedTray, value) && Pal.TrayNamesAndMaxVials.TryGetValue(value, out var maxTrayVial))
                {
                    MaxVialForTray = maxTrayVial;
                }
                else
                {
                    MaxVialForTray = 1;
                }
            }
        }

        public int MaxVialForTray
        {
            get => maxVialForTray;
            set => this.RaiseAndSetIfChanged(ref maxVialForTray, value);
        }

        public int VialNumber
        {
            get => vialNumber;
            set => this.RaiseAndSetIfChanged(ref vialNumber, value);
        }

        public int Volume
        {
            get => volume;
            set => this.RaiseAndSetIfChanged(ref volume, value);
        }

        public string StatusText
        {
            get => statusText;
            set => this.RaiseAndSetIfChanged(ref statusText, value);
        }

        public string SelectedPortName
        {
            get => selectedPortName;
            set => this.RaiseAndSetIfChanged(ref selectedPortName, value);
        }

        public string TrayNamesAndMaxVialFormatted {
            get => trayNamesAndMaxVialFormatted;
            private set => this.RaiseAndSetIfChanged(ref trayNamesAndMaxVialFormatted, value);
        }

        public string SelectVialsInput
        {
            get => selectVialsInput;
            set => this.RaiseAndSetIfChanged(ref selectVialsInput, value);
        }

        public string SelectVialsTray
        {
            get => selectVialsTray;
            set => this.RaiseAndSetIfChanged(ref selectVialsTray, value);
        }

        public string SelectVialsOutput
        {
            get => selectVialsOutput;
            private set => this.RaiseAndSetIfChanged(ref selectVialsOutput, value);
        }

        public bool MonitorStatus
        {
            get => monitorStatus;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref monitorStatus, value))
                {
                    ControlMonitoring(value);
                }
            }
        }

        public string TimeReport
        {
            get => timeReport;
            set => this.RaiseAndSetIfChanged(ref timeReport, value);
        }

        public Pal Pal
        {
            get => pal;
            private set => this.RaiseAndSetIfChanged(ref pal, value);
        }

        public int MaxVial => (int) (Pal?.VialRange ?? VialRanges.Well96);

        //TODO: This. There are wait/free events, do I still need this?
        /// <summary>
        /// Keeps track of whether or not the PAL is occupied.
        /// </summary>
        public override bool Running
        {
            get => false;
            set { }
        }

        /// <summary>
        /// Decides whether or not the PAL is emulated.
        /// </summary>
        public bool Emulation
        {
            get => Pal.Emulation;
            set => Pal.Emulation = value;
        }

        /// <summary>
        /// The associated device (PAL).
        /// </summary>
        public override IDevice Device
        {
            get => Pal;
            set
            {
                Pal = value as Pal;
                if (Pal != null && !isInDesignMode)
                {
                    try
                    {
                        SelectedPortName = Pal.PortName;
                    }
                    catch
                    {
                        // ignored
                    }
                    RegisterDevice(value);
                }
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> RefreshMethodListCommand { get; }
        public ReactiveCommand<Unit, Unit> RunMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> StopMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyPortNameCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectVialsCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetPalCommand { get; }
        public ReactiveCommand<Unit, Unit> FullResetPalCommand { get; }

        private async Task SelectVials()
        {
            var result = await Task.Run(() => Pal.SelectVials(SelectVialsInput, SelectVialsTray, ""));

            SelectVialsOutput = result;
        }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new PalView();
        }

        private void PalViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Pal)))
            {
                Pal.PropertyChanged += PalOnPropertyChanged;
            }
        }

        private void PalOnPropertyChanged(object sendero, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Pal.VialRange)))
            {
                this.RaisePropertyChanged(nameof(MaxVial));
            }
        }

        /// <summary>
        /// Indicates that the device is available to take commands
        /// </summary>
        public virtual void OnFree()
        {
            Free?.Invoke();
            // m_runningMethodManually = false;
            //mButton_RunMethod.Text = "Run Method";
        }

        public virtual void Pal_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propagate this
            //TODO: Figure out if this actually worked or not
            //System.Windows.Forms.MessageBox.Show("OH SNAP WE NEED TO SAVE");
            OnSaveRequired();
        }

        /// <summary>
        /// Converts the raw method list string into a list of methods.
        /// </summary>
        /// <param name="rawMethodList">The string which the PAL class returns after GetMethodList()</param>
        public void ProcessMethods(List<string> rawMethodList)
        {
            //LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.ApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "PAL ADVANCED CONTROL PROCESS METHODS:" + rawMethodList.Count);
            if (rawMethodList != null)
            {
                using (methodComboBoxOptions.SuppressChangeNotifications())
                {
                    methodComboBoxOptions.Clear();
                    methodComboBoxOptions.AddRange(rawMethodList);
                }
            }
        }

        /// <summary>
        /// Converts the raw tray list string into a list of trays.
        /// </summary>
        /// <param name="trayList">The string which the PAL class returns after GetTrayList()</param>
        public void ProcessTrays(List<string> trayList)
        {
            /*LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(
                                                               LcmsNetDataClasses.Logging.ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                                                               "ADVANCED CONTROL PROCESS TRAYS:" + trayList.Count);*/
            if (trayList != null)
            {
                using (trayComboBoxOptions.SuppressChangeNotifications())
                {
                    trayComboBoxOptions.Clear();
                    trayComboBoxOptions.AddRange(trayList);
                }
            }
        }

        public void ProcessTraysAndMaxVials(Dictionary<string, int> traysAndMaxVials)
        {
            if (traysAndMaxVials != null)
            {
                using (TrayNamesAndMaxVial.SuppressChangeNotifications())
                {
                    trayNamesAndMaxVial.Clear();
                    trayNamesAndMaxVial.AddRange(traysAndMaxVials.Select(x => $"Tray: {x.Key}   Max Vial: {x.Value}"));
                }
            }

            TrayNamesAndMaxVialFormatted = string.Join("\n", TrayNamesAndMaxVial);
        }

        private async Task RefreshMethods()
        {
            var methods = await Task.Run(() => Pal.ListMethods());
            var trays = await Task.Run(() => Pal.ListTrays());
            await Task.Run(() => Pal.SetMaxVialsForTrays());

            ProcessMethods(methods);
            ProcessTrays(trays);
            ProcessTraysAndMaxVials(Pal.TrayNamesAndMaxVials);
        }

        private async Task RunMethod()
        {
            var taskType = "";
            var elapsedSecs = 0.0;
            await Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                if (Pal.GetStatus().Contains("READY"))
                {
                    if (string.IsNullOrWhiteSpace(SelectedTray))
                    {
                        MessageBox.Show("No tray selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(SelectedMethod))
                    {
                        MessageBox.Show("No method selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    taskType = "Run Method";
                    Pal.LoadMethod(SelectedMethod, SelectedTray, VialNumber, Convert.ToString(Volume, CultureInfo.InvariantCulture));
                    Pal.StartMethod(1000);
                }
                else
                {
                    taskType = "Continue Method";
                    Pal.ContinueMethod(1000, true);
                }
                sw.Stop();
                elapsedSecs = sw.Elapsed.TotalSeconds;
            });

            TimeReport = $"{taskType} took {elapsedSecs:F2} seconds.";
        }

        private async Task ResetPal()
        {
            var elapsedSecs = 0.0;
            await Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Pal.ResetPAL();
                sw.Stop();
                elapsedSecs = sw.Elapsed.TotalSeconds;
            });

            TimeReport = $"Reset PAL took {elapsedSecs:F2} seconds.";
        }

        private async Task FullyResetPal()
        {
            var elapsedSecs = 0.0;
            await Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Pal.StopMethod();
                Pal.ResetPAL();
                sw.Stop();
                elapsedSecs = sw.Elapsed.TotalSeconds;
            });

            TimeReport = $"Fully Reset PAL took {elapsedSecs:F2} seconds.";
        }

        private void Initialize()
        {
            try
            {
                var errorMessage = "";
                Pal.Initialize(ref errorMessage);
            }
            catch
            {
                StatusText = "Could not initialize.";
            }
        }

        private async Task RefreshStatus()
        {
            var status = await Task.Run(() => Pal.GetStatus());
            StatusText = status;
        }

        private async Task StopMethod()
        {
            var elapsedSecs = 0.0;
            await Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Pal.StopMethod();
                sw.Stop();
                elapsedSecs = sw.Elapsed.TotalSeconds;
            });

            TimeReport = $"Stop Method took {elapsedSecs:F2} seconds.";
        }

        private void ApplyPortName()
        {
            Pal.PortName = SelectedPortName;
            StatusText = "Port name changed to " + pal.PortName;
        }

        private void ControlMonitoring(bool doMonitor)
        {
            if (doMonitor)
            {
                if (monitorTimer == null)
                {
                    monitorTimer = new Timer(MonitorUpdateStatus, this, 1000, 1000);
                }
                else
                {
                    monitorTimer.Change(1000, 0);
                }
                if (monitorTimeout == null)
                {
                    monitorTimeout = new Timer(TimeoutMonitor, this, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
                }
            }
            else
            {
                TimeoutMonitor(this);
            }
        }

        private Timer monitorTimer = null;
        private Timer monitorTimeout = null;

        private async void MonitorUpdateStatus(object sender)
        {
            await RefreshStatus();
        }

        private void TimeoutMonitor(object sender)
        {
            if (monitorTimer != null)
            {
                monitorTimer.Dispose();
                monitorTimer = null;
            }
            if (monitorTimeout != null)
            {
                monitorTimeout.Dispose();
                monitorTimeout = null;
            }
            MonitorStatus = false;
        }

        #endregion
    }
}
