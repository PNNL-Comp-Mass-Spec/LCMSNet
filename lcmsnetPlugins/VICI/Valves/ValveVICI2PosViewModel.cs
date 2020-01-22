using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Controls;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetData;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public class ValveVICI2PosViewModel : ValveVICIViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICI2PosViewModel()
        {
            SetPositionACommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetPositionA()));
            SetPositionBCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetPositionB()));
        }

        protected override void RegisterDevice(IDevice device)
        {
            valve = device as ValveVICI2Pos;

            //TODO: Throw error!
            if (valve == null)
                return;

            valve.PositionChanged += OnPosChanged;
            valve.DeviceSaveRequired += Valve_DeviceSaveRequired;

            SetBaseDevice(valve);

            ComPort = valve.Port;
        }

        #endregion

        #region Members

        ///// <summary>
        ///// The serial port used for communicating with the Valve
        ///// </summary>
        //public static SerialPort testPort = new SerialPort();

        /// <summary>
        /// The valve object
        /// </summary>
        protected ValveVICI2Pos valve;

        //public ValveEventListener ValveEventListener;

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets or sets the emulation state of the device.
        ///// </summary>
        //public bool Emulation
        //{
        //    get
        //    {
        //        return valve.Emulation;
        //    }
        //    set
        //    {
        //        valve.Emulation = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the associated device.
        /// </summary>
        public override IDevice Device
        {
            get => valve;
            set
            {
                if (!IsInDesignMode)
                {
                    //valve = (classValveVICI2Pos)value;
                    RegisterDevice(value);
                    //try
                    //{
                    //    string errorMessage = "";
                    //    valve.Initialize(ref errorMessage);
                    //    mpropertyGrid_Serial.SelectedObject = valve.Port;
                    //}
                    //catch (ValveExceptionReadTimeout ex)
                    //{
                    //    ShowError("Timeout (read) when attempting to initialize valve", ex);
                    //}
                    //catch (ValveExceptionWriteTimeout ex)
                    //{
                    //    ShowError("Timeout (write) when attempting to initialize valve", ex);
                    //}
                    //catch (ValveExceptionUnauthorizedAccess ex)
                    //{
                    //    ShowError("Unauthorized access when attempting to initialize valve", ex);
                    //}
                }
                //ValveEventListener = new ValveEventListener(valve, this);
            }
        }

        public ReactiveCommand<Unit, Unit> SetPositionACommand { get; }
        public ReactiveCommand<Unit, Unit> SetPositionBCommand { get; }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new ValveVICI2PosView();
        }

        /// <summary>
        /// Indicates that the valve's position has changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="newPosition">The new position</param>
        public virtual void OnPosChanged(object sender, ValvePositionEventArgs<TwoPositionState> newPosition)   // DAC changed
        {
            UpdatePositionTextBox(newPosition.Position);
        }

        /// <summary>
        /// Updates the position textbox on the control.
        /// </summary>
        /// <param name="position"></param>
        private void UpdatePositionTextBox(TwoPositionState position)
        {
            RxApp.MainThreadScheduler.Schedule(() => CurrentValvePosition = position.GetEnumDescription());
        }

        protected override void ValveControlSelected()
        {
            UpdatePositionTextBox(valve.LastMeasuredPosition);
        }

        private void SetPositionA()
        {
            try
            {
                /*Thread p = new Thread(() => valve.SetPosition(TwoPositionState.PositionA));
                p.Start();
                p = null;*/
                valve.SetPosition(TwoPositionState.PositionA);
                //UpdatePositionTextBox(valve.LastMeasuredPosition);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                ShowError("Valve position mismatch", ex);
            }
            catch (Exception Ex)
            {
                ShowError("Exception in valve control", Ex);
            }
        }

        private void SetPositionB()
        {
            try
            {
                /*Thread p = new Thread(() => valve.SetPosition(TwoPositionState.PositionB));
                p.Start();
                p = null;*/
                valve.SetPosition(TwoPositionState.PositionB);
                //UpdatePositionTextBox(valve.LastMeasuredPosition);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                ShowError("Valve position mismatch", ex);
            }
        }

        protected override void RefreshValvePosition()
        {
            try
            {
                UpdatePositionTextBox((TwoPositionState)valve.GetPosition());
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

        protected override void RefreshValveVersion()
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

        protected override void OpenPort()
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

        protected override void ClosePort()
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

        protected override void SelectedValveIdUpdated()
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

        protected override void RefreshValveId()
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

        protected override void ClearValveId()
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

        protected override void InitializeDevice()
        {
            if (IsInDesignMode)
                return;

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
            CurrentValvePosition = valve.LastMeasuredPosition.ToString();
            CurrentValveId = valve.SoftwareID;
        }

        #endregion
    }
}
