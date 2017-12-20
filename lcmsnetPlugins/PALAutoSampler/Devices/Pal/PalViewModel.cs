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
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace LcmsNet.Devices.Pal
{
    public class PalViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        #region Constructors

        /// <summary>
        /// The main constructor. Creates the PAL class and initializes the port.
        /// </summary>
        public PalViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            portNamesComboBoxOptions = new ReactiveUI.ReactiveList<string>(System.IO.Ports.SerialPort.GetPortNames());
            vialRangeComboBoxOptions = new ReactiveUI.ReactiveList<enumVialRanges>(Enum.GetValues(typeof(enumVialRanges)).Cast<enumVialRanges>());

            SetupCommands();

            this.PropertyChanged += PalViewModel_PropertyChanged;
        }

        /// <summary>
        /// Registers the device events and user interface.
        /// </summary>
        /// <param name="device"></param>
        private void RegisterDevice(IDevice device)
        {
            Pal = device as classPal;

            if (Pal != null)
            {
                Pal.DeviceSaveRequired += Pal_DeviceSaveRequired;
                Pal.Free += OnFree;

                ReactiveUI.RxApp.MainThreadScheduler.Schedule(() =>
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
        public event DelegateFree Free;

        #endregion

        #region Members

        /// <summary>
        /// The class which controls the PAL itself.
        /// </summary>
        private classPal pal;

        private readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private readonly ReactiveUI.ReactiveList<string> trayComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private readonly ReactiveUI.ReactiveList<enumVialRanges> vialRangeComboBoxOptions;
        private readonly ReactiveUI.ReactiveList<string> portNamesComboBoxOptions;
        private readonly ReactiveUI.ReactiveList<string> trayNamesAndMaxVial = new ReactiveUI.ReactiveList<string>();
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

        #endregion

        #region Properties

        public ReactiveUI.IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> TrayComboBoxOptions => trayComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<enumVialRanges> VialRangeComboBoxOptions => vialRangeComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> PortNamesComboBoxOptions => portNamesComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> TrayNamesAndMaxVial => trayNamesAndMaxVial;

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public string SelectedTray
        {
            get { return selectedTray; }
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
            get { return maxVialForTray; }
            set { this.RaiseAndSetIfChanged(ref maxVialForTray, value); }
        }

        public int VialNumber
        {
            get { return vialNumber; }
            set { this.RaiseAndSetIfChanged(ref vialNumber, value); }
        }

        public int Volume
        {
            get { return volume; }
            set { this.RaiseAndSetIfChanged(ref volume, value); }
        }

        public string StatusText
        {
            get { return statusText; }
            set { this.RaiseAndSetIfChanged(ref statusText, value); }
        }

        public string SelectedPortName
        {
            get { return selectedPortName; }
            set { this.RaiseAndSetIfChanged(ref selectedPortName, value); }
        }

        public string TrayNamesAndMaxVialFormatted {
            get { return trayNamesAndMaxVialFormatted; }
            private set { this.RaiseAndSetIfChanged(ref trayNamesAndMaxVialFormatted, value); }
        }

        public string SelectVialsInput
        {
            get { return selectVialsInput; }
            set { this.RaiseAndSetIfChanged(ref selectVialsInput, value); }
        }

        public string SelectVialsTray
        {
            get { return selectVialsTray; }
            set { this.RaiseAndSetIfChanged(ref selectVialsTray, value); }
        }

        public string SelectVialsOutput
        {
            get { return selectVialsOutput; }
            private set { this.RaiseAndSetIfChanged(ref selectVialsOutput, value); }
        }

        public bool MonitorStatus
        {
            get { return monitorStatus; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref monitorStatus, value))
                {
                    ControlMonitoring(value);
                }
            }
        }

        public classPal Pal
        {
            get { return pal; }
            private set { this.RaiseAndSetIfChanged(ref pal, value); }
        }

        public int MaxVial => (int) (Pal?.VialRange ?? enumVialRanges.Well96);

        //TODO: This. There are wait/free events, do I still need this?
        /// <summary>
        /// Keeps track of whether or not the PAL is occupied.
        /// </summary>
        public override bool Running
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Decides whether or not the PAL is emulated.
        /// </summary>
        public bool Emulation
        {
            get { return Pal.Emulation; }
            set { Pal.Emulation = value; }
        }

        /// <summary>
        /// The associated device (PAL).
        /// </summary>
        public IDevice Device
        {
            get { return Pal; }
            set
            {
                Pal = value as classPal;
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

        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshMethodListCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RunMethodCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopMethodCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ApplyPortNameCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SelectVialsCommand { get; private set; }

        private void SetupCommands()
        {
            RefreshMethodListCommand = ReactiveUI.ReactiveCommand.CreateFromTask(() => RefreshMethods());
            RunMethodCommand = ReactiveUI.ReactiveCommand.CreateFromTask(() => RunMethod());
            StopMethodCommand = ReactiveUI.ReactiveCommand.CreateFromTask(() => StopMethod());
            RefreshStatusCommand = ReactiveUI.ReactiveCommand.CreateFromTask(() => RefreshStatus());
            ApplyPortNameCommand = ReactiveUI.ReactiveCommand.Create(() => ApplyPortName());
            SelectVialsCommand = ReactiveUI.ReactiveCommand.CreateFromTask(() => SelectVials());
        }

        private async Task SelectVials()
        {
            var result = await Task.Run(() => Pal.SelectVials(SelectVialsInput, SelectVialsTray, ""));

            SelectVialsOutput = result;
        }

        #endregion

        #region Methods

        public UserControl GetDefaultView()
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
                OnPropertyChanged(nameof(MaxVial));
            }
        }

        /// <summary>
        /// Indicates that the device is available to take commands
        /// </summary>
        public virtual void OnFree(object sender)
        {
            Free?.Invoke(this);
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
            //LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "PAL ADVANCED CONTROL PROCESS METHODS:" + rawMethodList.Count);
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
            /*LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(
                                                               LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
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

            await Task.Run(() =>
            {
                if (Pal.GetStatus().Contains("READY"))
                {
                    Pal.LoadMethod(SelectedMethod, SelectedTray, VialNumber, Convert.ToString(Volume, CultureInfo.InvariantCulture));
                    Pal.StartMethod(1000);
                }
                else
                {
                    Pal.ContinueMethod(0);
                }
            });
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
            await Task.Run(() => Pal.StopMethod());
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
