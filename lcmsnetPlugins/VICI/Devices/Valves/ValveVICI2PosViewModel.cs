using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using FluidicsSDK.Base;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Valves
{
    public class ValveVICI2PosViewModel : ValveVICIViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICI2PosViewModel()
        {
            SetPositionACommand = ReactiveUI.ReactiveCommand.Create(() => SetPositionA());
            SetPositionBCommand = ReactiveUI.ReactiveCommand.Create(() => SetPositionB());
        }

        protected override void RegisterDevice(IDevice device)
        {
            m_valve = device as classValveVICI2Pos;

            //TODO: Throw error!
            if (m_valve == null)
                return;

            m_valve.PositionChanged += OnPosChanged;
            m_valve.DeviceSaveRequired += Valve_DeviceSaveRequired;

            SetBaseDevice(m_valve);

            ComPort = m_valve.Port;
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that the position of the valve has changed.
        /// </summary>
        public event DelegatePositionChanged PosChanged;

        #endregion

        #region Members

        ///// <summary>
        ///// The serial port used for communicating with the Valve
        ///// </summary>
        //public static SerialPort testPort = new SerialPort();

        /// <summary>
        /// The valve object
        /// </summary>
        protected classValveVICI2Pos m_valve;

        //public ValveEventListener m_valveEventListener;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the emulation state of the device.
        /// </summary>
        //public bool Emulation
        //{
        //    get
        //    {
        //        return m_valve.Emulation;
        //    }
        //    set
        //    {
        //        m_valve.Emulation = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the associated device.
        /// </summary>
        public override IDevice Device
        {
            get
            {
                return m_valve;
            }
            set
            {
                if (!IsInDesignMode)
                {
                    //m_valve = (classValveVICI2Pos)value;
                    RegisterDevice(value);
                    //try
                    //{
                    //    string errorMessage = "";
                    //    m_valve.Initialize(ref errorMessage);
                    //    mpropertyGrid_Serial.SelectedObject = m_valve.Port;
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
                //m_valveEventListener = new ValveEventListener(m_valve, this);
            }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetPositionACommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetPositionBCommand { get; private set; }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new ValveVICI2PosView();
        }

        public virtual void Valve_DeviceSaveRequired(object sender, EventArgs e)
        {
            OnSaveRequired();
        }

        /// <summary>
        /// Indicates that the valve's position has changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="newPosition">The new position</param>
        public virtual void OnPosChanged(object sender, ValvePositionEventArgs<TwoPositionState> newPosition)   // DAC changed
        {
            PosChanged?.Invoke(this, newPosition.Position.ToCustomString());
            UpdatePositionTextBox(newPosition.Position);
        }

        /// <summary>
        /// Updates the position textbox on the control.
        /// </summary>
        /// <param name="position"></param>
        private void UpdatePositionTextBox(TwoPositionState position)
        {
            CurrentValvePosition = position.ToCustomString();
        }

        protected override void ValveControlSelected()
        {
            UpdatePositionTextBox(m_valve.LastMeasuredPosition);
        }

        private void SetPositionA()
        {
            try
            {
                /*Thread p = new Thread(() => m_valve.SetPosition(TwoPositionState.PositionA));
                p.Start();
                p = null;*/
                m_valve.SetPosition(TwoPositionState.PositionA);
                //UpdatePositionTextBox(m_valve.LastMeasuredPosition);
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
                /*Thread p = new Thread(() => m_valve.SetPosition(TwoPositionState.PositionB));
                p.Start();
                p = null;*/
                m_valve.SetPosition(TwoPositionState.PositionB);
                //UpdatePositionTextBox(m_valve.LastMeasuredPosition);
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
                UpdatePositionTextBox((TwoPositionState)m_valve.GetPosition());
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
                ValveVersionInfo = m_valve.GetVersion();
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
                if (!m_valve.Port.IsOpen)
                {
                    m_valve.Port.Open();
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
                m_valve.Port.Close();
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
                m_valve.SetHardwareID(newID);
                CurrentValveId = m_valve.GetHardwareID();
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
                CurrentValveId = m_valve.GetHardwareID();
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
                m_valve.ClearHardwareID();
                CurrentValveId = m_valve.GetHardwareID();
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
                var success = m_valve.Initialize(ref errorMessage);

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

            ValveVersionInfo = m_valve.Version;
            CurrentValvePosition = m_valve.LastMeasuredPosition.ToString();
            CurrentValveId = m_valve.SoftwareID;
        }

        #endregion
    }
}
