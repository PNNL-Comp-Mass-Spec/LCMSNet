using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;

namespace LcmsNet.Devices.Valves
{
    public abstract class ValveVICIViewModelBase : BaseDeviceControlViewModel, IDeviceControlWpf
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

            SetupCommands();

            this.PropertyChanged += OnPropertyChanged;
        }

        protected abstract void RegisterDevice(IDevice device);

        #endregion

        #region Members

        //public ValveEventListener m_valveEventListener;

        protected readonly bool IsInDesignMode = false;

        private readonly ReactiveUI.ReactiveList<char> valveIdComboBoxOptions = new ReactiveUI.ReactiveList<char>();
        private char selectedValveId = ' ';
        private char currentValveId = ' ';
        private string currentValvePosition = "";
        private string valveVersionInfo = "";
        private bool valveControlTabSelected = false;
        private SerialPort comPort;

        #endregion

        #region Properties

        public ReactiveUI.IReadOnlyReactiveList<char> ValveIdComboBoxOptions => valveIdComboBoxOptions;

        public char SelectedValveId
        {
            get { return selectedValveId; }
            set { this.RaiseAndSetIfChanged(ref selectedValveId, value); }
        }

        public char CurrentValveId
        {
            get { return currentValveId; }
            protected set { this.RaiseAndSetIfChanged(ref currentValveId, value); }
        }

        public string CurrentValvePosition
        {
            get { return currentValvePosition; }
            protected set { this.RaiseAndSetIfChanged(ref currentValvePosition, value); }
        }

        public string ValveVersionInfo
        {
            get { return valveVersionInfo; }
            protected set { this.RaiseAndSetIfChanged(ref valveVersionInfo, value); }
        }

        public bool ValveControlTabSelected
        {
            get { return valveControlTabSelected; }
            set { this.RaiseAndSetIfChanged(ref valveControlTabSelected, value); }
        }

        /// <summary>
        /// The serial port used for communicating with the Valve
        /// </summary>
        public SerialPort ComPort
        {
            get { return comPort; }
            protected set { this.RaiseAndSetIfChanged(ref comPort, value); }
        }

        /// <summary>
        /// Gets or sets the associated device.
        /// </summary>
        public abstract IDevice Device { get; set; }

        public abstract UserControl GetDefaultView();

        #endregion

        #region Commands

        public ReactiveUI.ReactiveCommand<Unit, Unit> ClearValveIdCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshValveIdCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshValvePositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshValveVersionInfoCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> OpenPortCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ClosePortCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> InitializeDeviceCommand { get; private set; }

        private void SetupCommands()
        {
            ClearValveIdCommand = ReactiveUI.ReactiveCommand.Create(() => ClearValveId());
            RefreshValveIdCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshValveId());
            RefreshValvePositionCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshValvePosition());
            RefreshValveVersionInfoCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshValveVersion());
            OpenPortCommand = ReactiveUI.ReactiveCommand.Create(() => OpenPort());
            ClosePortCommand = ReactiveUI.ReactiveCommand.Create(() => ClosePort());
            InitializeDeviceCommand = ReactiveUI.ReactiveCommand.Create(() => InitializeDevice());
        }

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
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message);
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
