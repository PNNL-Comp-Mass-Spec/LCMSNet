﻿using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using LcmsNetCommonControls.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public abstract class ValveVICIViewModelBase : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected ValveVICIViewModelBase()
        {
            IsInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            valveIdComboBoxOptionsLegacy = new ReadOnlyObservableCollection<char>(new ObservableCollection<char>
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ',
            });
            valveIdComboBoxOptionsUniversalActuator = new ReadOnlyObservableCollection<char>(new ObservableCollection<char>
            {
                // Universal Actuators also support A-Z IDs.
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
            });

            ValveIdComboBoxOptions = valveIdComboBoxOptionsLegacy;

            ValveControlTabSelected = true; // Default selected tab

            UpdatePortNameCommand = ReactiveCommand.Create(UpdatePortName);
            SetValveIdCommand = ReactiveCommand.Create(SetValveId);
            ClearValveIdCommand = ReactiveCommand.Create(ClearValveId);
            SetValveHardwareIdCommand = ReactiveCommand.Create(SetValveHardwareId);
            ClearValveHardwareIdCommand = ReactiveCommand.Create(ClearValveHardwareId);
            RefreshValveIdCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshValveId));
            GetValveIdBroadcastCommand = ReactiveCommand.Create(GetValveIdBroadcast);
            RefreshValvePositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshValvePosition));
            RefreshValveVersionInfoCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshValveVersion));
            OpenPortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(OpenPort));
            ClosePortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ClosePort));
            InitializeDeviceCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(InitializeDevice));

            //this.WhenAnyValue(x => x.valve.SoftwareID).ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => CurrentValveId = x);
            //this.WhenAnyValue(x => x.valve.Version).ObserveOn(RxApp.MainThreadScheduler).Subscribe(x =>
            //{
            //    ValveVersionInfo = x;
            //    UpdateAvailableIDList();
            //});

            this.WhenAnyValue(x => x.ValveControlTabSelected).ObserveOn(RxApp.MainThreadScheduler).Subscribe(x =>
            {
                if (x)
                {
                    ValveControlSelected();
                }
                else
                {
                    UpdateAvailableIDList();
                }
            });
        }

        protected abstract void RegisterDevice(IDevice device);

        protected void RegisterBaseDevice(ValveVICIBase valveObj)
        {
            valve = valveObj;
            SetBaseDevice(valveObj);

            valve.DeviceSaveRequired += Valve_DeviceSaveRequired;

            ComPort = valve.PortName;
            NewComPort = ComPort;
            ValveVersionInfo = valve.Version;
            CurrentValvePosition = valve.LastMeasuredPositionDisplay;
            CurrentValveId = valve.SoftwareID;
            UpdateAvailableIDList();
        }

        //public ValveEventListener ValveEventListener;

        protected readonly bool IsInDesignMode = false;

        private readonly ReadOnlyObservableCollection<char> valveIdComboBoxOptionsLegacy;
        private readonly ReadOnlyObservableCollection<char> valveIdComboBoxOptionsUniversalActuator;
        private ReadOnlyObservableCollection<char> valveIdComboBoxOptions;
        private char selectedValveId = ' ';
        private char currentValveId = ' ';
        private string currentValvePosition = "";
        private string valveVersionInfo = "";
        private string valveBroadcastId = "";
        private bool valveControlTabSelected = false;
        private string comPort;
        private string newComPort;
        private ValveVICIBase valve = null;

        public ReadOnlyObservableCollection<SerialPortData> PortNamesComboBoxOptions => SerialPortGenericData.SerialPorts;

        public ReadOnlyObservableCollection<char> ValveIdComboBoxOptions
        {
            get => valveIdComboBoxOptions;
            private set => this.RaiseAndSetIfChanged(ref valveIdComboBoxOptions, value);
        }

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

        public string ValveBroadcastId
        {
            get => valveBroadcastId;
            protected set => this.RaiseAndSetIfChanged(ref valveBroadcastId, value);
        }

        public bool ValveControlTabSelected
        {
            get => valveControlTabSelected;
            set => this.RaiseAndSetIfChanged(ref valveControlTabSelected, value);
        }

        /// <summary>
        /// The serial port used for communicating with the Valve
        /// </summary>
        public string ComPort
        {
            get => comPort;
            private set => this.RaiseAndSetIfChanged(ref comPort, value);
        }

        public string NewComPort
        {
            get => newComPort;
            set => this.RaiseAndSetIfChanged(ref newComPort, value);
        }

        public ReactiveCommand<Unit, Unit> UpdatePortNameCommand { get; }
        public ReactiveCommand<Unit, Unit> SetValveIdCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearValveIdCommand { get; }
        public ReactiveCommand<Unit, Unit> SetValveHardwareIdCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearValveHardwareIdCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValveIdCommand { get; }
        public ReactiveCommand<Unit, Unit> GetValveIdBroadcastCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValvePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshValveVersionInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenPortCommand { get; }
        public ReactiveCommand<Unit, Unit> ClosePortCommand { get; }
        public ReactiveCommand<Unit, Unit> InitializeDeviceCommand { get; }

        public virtual void Valve_DeviceSaveRequired(object sender, EventArgs e)
        {
            OnSaveRequired();
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="ex">The exception to describe</param>
        protected void ShowError(string message, Exception ex = null)
        {
            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message, ex);
            // MessageBox.Show(message + "\r\n" + ex.Message, Device.Name);
        }

        private void ValveControlSelected()
        {
            CurrentValvePosition = valve?.LastMeasuredPositionDisplay;
        }

        /// <summary>
        /// Handles initializing the device (common operations).
        /// </summary>
        private void InitializeDevice()
        {
            if (IsInDesignMode)
                return;

            ComPort = valve.PortName;
            NewComPort = ComPort;

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
            UpdateAvailableIDList();
        }

        private void UpdatePortName()
        {
            valve.PortName = NewComPort;
            ComPort = valve.PortName;
        }

        private void UpdateAvailableIDList()
        {
            if (valve != null && (valve.IsUniversalActuator || string.IsNullOrWhiteSpace(valve.Version)))
            {
                ValveIdComboBoxOptions = valveIdComboBoxOptionsUniversalActuator;
            }
            else
            {
                ValveIdComboBoxOptions = valveIdComboBoxOptionsLegacy;
            }
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
                valve.Connection.OpenTest();
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
                valve.Connection.CloseTest();
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

        private void GetValveIdBroadcast()
        {
            ValveBroadcastId = valve.GetHardwareIDFromAny();
        }

        private void SetValveId()
        {
            if (valve != null)
            {
                valve.SoftwareID = SelectedValveId;
            }

            CurrentValveId = valve?.SoftwareID ?? ' ';
            OnSaveRequired();
        }

        private void ClearValveId()
        {
            if (valve != null)
            {
                valve.SoftwareID = ' ';
            }

            SelectedValveId = ' ';
            OnSaveRequired();
        }

        private void SetValveHardwareId()
        {
            try
            {
                valve?.SetHardwareID(SelectedValveId);
                CurrentValveId = valve?.SoftwareID ?? ' ';
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

        private void ClearValveHardwareId()
        {
            try
            {
                valve?.ClearHardwareID();
                CurrentValveId = valve?.GetHardwareID() ?? ' ';
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
    }
}
