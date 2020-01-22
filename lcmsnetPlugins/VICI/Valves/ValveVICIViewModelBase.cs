using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using LcmsNetCommonControls.Devices;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public abstract class ValveVICIViewModelBase : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICIViewModelBase()
        {
            IsInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            using (valveIdComboBoxOptions.SuppressChangeNotifications())
            {
                valveIdComboBoxOptions.AddRange(new [] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' });
            }
            ValveControlTabSelected = true; // Default selected tab

            ClearValveIdCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => ClearValveId()));
            RefreshValveIdCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => RefreshValveId()));
            RefreshValvePositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => RefreshValvePosition()));
            RefreshValveVersionInfoCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => RefreshValveVersion()));
            OpenPortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => OpenPort()));
            ClosePortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => ClosePort()));
            InitializeDeviceCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => InitializeDevice()));

            this.PropertyChanged += OnPropertyChanged;
        }

        protected abstract void RegisterDevice(IDevice device);

        #endregion

        #region Members

        //public ValveEventListener ValveEventListener;

        protected readonly bool IsInDesignMode = false;

        private readonly ReactiveList<char> valveIdComboBoxOptions = new ReactiveList<char>();
        private char selectedValveId = ' ';
        private char currentValveId = ' ';
        private string currentValvePosition = "";
        private string valveVersionInfo = "";
        private bool valveControlTabSelected = false;
        private SerialPort comPort;

        #endregion

        #region Properties

        public IReadOnlyReactiveList<char> ValveIdComboBoxOptions => valveIdComboBoxOptions;

        public char SelectedValveId
        {
            get => selectedValveId;
            set => this.RaiseAndSetIfChanged(ref selectedValveId, value);
        }

        public char CurrentValveId
        {
            get => currentValveId;
            protected set => this.RaiseAndSetIfChanged(ref currentValveId, value);
        }

        public string CurrentValvePosition
        {
            get => currentValvePosition;
            protected set => this.RaiseAndSetIfChanged(ref currentValvePosition, value);
        }

        public string ValveVersionInfo
        {
            get => valveVersionInfo;
            protected set => this.RaiseAndSetIfChanged(ref valveVersionInfo, value);
        }

        public bool ValveControlTabSelected
        {
            get => valveControlTabSelected;
            set => this.RaiseAndSetIfChanged(ref valveControlTabSelected, value);
        }

        /// <summary>
        /// The serial port used for communicating with the Valve
        /// </summary>
        public SerialPort ComPort
        {
            get => comPort;
            protected set => this.RaiseAndSetIfChanged(ref comPort, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> ClearValveIdCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValveIdCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValvePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValveVersionInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenPortCommand { get; }
        public ReactiveCommand<Unit, Unit> ClosePortCommand { get; }
        public ReactiveCommand<Unit, Unit> InitializeDeviceCommand { get; }

        #endregion

        #region Methods

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(SelectedValveId)))
            {
                SelectedValveIdUpdated();
            }
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(ValveControlTabSelected)) && ValveControlTabSelected)
            {
                ValveControlSelected();
            }
        }

        public virtual void Valve_DeviceSaveRequired(object sender, EventArgs e)
        {
            OnSaveRequired();
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        protected void ShowError(string message)
        {
            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message);
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="ex">The exception to describe</param>
        protected void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + "\r\n" + ex.Message, Device.Name);
        }

        protected abstract void ValveControlSelected();

        protected abstract void RefreshValvePosition();

        protected abstract void RefreshValveVersion();

        protected abstract void OpenPort();

        protected abstract void ClosePort();

        protected abstract void InitializeDevice();

        protected abstract void SelectedValveIdUpdated();

        protected abstract void RefreshValveId();

        protected abstract void ClearValveId();

        #endregion
    }
}
