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
                valveIdComboBoxOptions.AddRange(new []
                {
                    // TODO: Universal Actuators also support A-Z IDs.
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' '
                });
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
            this.WhenAnyValue(x => x.SelectedValveId).Subscribe(x =>
            {
                valve?.SetHardwareID(x);
                CurrentValveId = valve?.SoftwareID ?? ' ';
            });
        }

        protected abstract void RegisterDevice(IDevice device);

        protected void RegisterBaseDevice(ValveVICIBase valveObj)
        {
            valve = valveObj;
            SetBaseDevice(valveObj);

            valve.DeviceSaveRequired += Valve_DeviceSaveRequired;

            ComPort = valve.Port;
        }

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
        private ValveVICIBase valve = null;

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
            private set => this.RaiseAndSetIfChanged(ref comPort, value);
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

        private void ValveControlSelected()
        {
            CurrentValvePosition = valve.LastMeasuredPositionDisplay;
        }

        /// <summary>
        /// Handles initializing the device (common operations).
        /// </summary>
        private void InitializeDevice()
        {
            if (IsInDesignMode)
                return;

            ComPort = valve.Port;

            try
            {
                var errorMessage = "";
                var success = valve.Initialize(ref errorMessage);

                if (success == false)
                    ShowError("Could not initialize the valve. " + errorMessage);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to initialize valve", ex);
            }

            ValveVersionInfo = valve.Version;
            CurrentValvePosition = valve.LastMeasuredPositionDisplay;
            CurrentValveId = valve.SoftwareID;
        }

        private void RefreshValvePosition()
        {
            try
            {
                CurrentValvePosition = valve.GetPositionDisplay();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve position", ex);
            }
        }

        private void RefreshValveVersion()
        {
            try
            {
                ValveVersionInfo = valve.GetVersion();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve version", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve version", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve version", ex);
            }
        }

        private void OpenPort()
        {
            try
            {
                if (!valve.Port.IsOpen)
                {
                    valve.Port.Open();
                }
            }
            catch (NullReferenceException ex)
            {
                ShowError("Null reference when attempting to open port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access exception when attempting to open port", ex);
            }
        }

        private void ClosePort()
        {
            try
            {
                valve.Port.Close();
            }
            catch (NullReferenceException ex)
            {
                ShowError("Null reference when attempting to close port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access exception when attempting to close port", ex);
            }
        }

        private void SelectedValveIdUpdated()
        {
            var newID = SelectedValveId;
            try
            {
                valve.SetHardwareID(newID);
                CurrentValveId = valve.GetHardwareID();
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve ID", ex);
            }
        }

        private void RefreshValveId()
        {
            try
            {
                CurrentValveId = valve.GetHardwareID();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve ID", ex);
            }
        }

        private void ClearValveId()
        {
            try
            {
                valve.ClearHardwareID();
                CurrentValveId = valve.GetHardwareID();
                SelectedValveId = ' ';
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to clear valve ID", ex);
            }
        }

        #endregion
    }
}
