using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace LcmsNet.Devices.Pal
{
    public class PalViewModel : BaseDeviceControlViewModel, IDeviceControlWpf
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
                Pal.TrayNames += m_Pal_PalTrayListReceived;
                Pal.MethodNames += (sender, e) => { ProcessMethods(e.MethodList); };
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
        private classPal m_Pal;

        ///// <summary>
        ///// An event listener to watch for events from the PAL class
        ///// </summary>
        //public PalEventListener m_PalEventListener;

        private readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private readonly ReactiveUI.ReactiveList<string> trayComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private readonly ReactiveUI.ReactiveList<enumVialRanges> vialRangeComboBoxOptions;
        private readonly ReactiveUI.ReactiveList<string> portNamesComboBoxOptions;
        private readonly bool isInDesignMode = false;
        private string selectedMethod = "";
        private string selectedTray = "";
        private int vialNumber = 0;
        private int volume = 0;
        private string statusText = "";
        private string selectedPortName = "";

        #endregion

        #region Properties

        public ReactiveUI.IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> TrayComboBoxOptions => trayComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<enumVialRanges> VialRangeComboBoxOptions => vialRangeComboBoxOptions;
        public ReactiveUI.IReadOnlyReactiveList<string> PortNamesComboBoxOptions => portNamesComboBoxOptions;

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public string SelectedTray
        {
            get { return selectedTray; }
            set { this.RaiseAndSetIfChanged(ref selectedTray, value); }
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

        public classPal Pal
        {
            get { return m_Pal; }
            private set { this.RaiseAndSetIfChanged(ref m_Pal, value); }
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

        private void SetupCommands()
        {
            RefreshMethodListCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshMethods());
            RunMethodCommand = ReactiveUI.ReactiveCommand.Create(() => RunMethod());
            StopMethodCommand = ReactiveUI.ReactiveCommand.Create(() => StopMethod());
            RefreshStatusCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshStatus());
            ApplyPortNameCommand = ReactiveUI.ReactiveCommand.Create(() => ApplyPortName());
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
            //Propogate this
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
        /// Handles when the PAL says it has tray data.
        /// </summary>
        void m_Pal_PalTrayListReceived(object sender, classAutoSampleEventArgs args)
        {
            ProcessTrays(args.TrayList);
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

        private void RefreshMethods()
        {
            Pal.ListMethods();
            Pal.ListTrays();
        }

        private void RunMethod()
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

            if (Pal.GetStatus().Contains("READY"))
            {
                Pal.LoadMethod(SelectedMethod, SelectedTray, VialNumber, Convert.ToString(Volume, CultureInfo.InvariantCulture));
                Pal.StartMethod(1000);
            }
            else
            {
                Pal.ContinueMethod(0);
            }
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

        private void RefreshStatus()
        {
            StatusText = Pal.GetStatus();
        }

        private void StopMethod()
        {
            Pal.StopMethod();
        }

        private void ApplyPortName()
        {
            Pal.PortName = SelectedPortName;
            StatusText = "Port name changed to " + m_Pal.PortName;
        }

        #endregion
    }
}
