//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//*********************************************************************************************************

using FluidicsSDK.Base;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICI2Pos : controlBaseDeviceControl, IDeviceControl
    {
        #region Members

        /// <summary>
        /// The serial port used for communicating with the Valve
        /// </summary>
        public static SerialPort testPort = new SerialPort();

        /// <summary>
        /// The valve object
        /// </summary>
        protected classValveVICI2Pos mobj_valve;

        //public ValveEventListener mobj_valveEventListener;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public controlValveVICI2Pos()
        {
            InitializeComponent();
        }

        public virtual void RegisterDevice(IDevice device)
        {
            mobj_valve = device as classValveVICI2Pos;
            
            //TODO: Throw error!
            if (mobj_valve == null)
                return;
    
            
            
            mobj_valve.PositionChanged    += new EventHandler<ValvePositionEventArgs<TwoPositionState>>(OnPosChanged);
            mobj_valve.DeviceSaveRequired += new EventHandler(Valve_DeviceSaveRequired);
            
            SetBaseDevice(mobj_valve);

            mpropertyGrid_Serial.SelectedObject = mobj_valve.Port;
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that the position of the valve has changed.
        /// </summary>
        public event DelegatePositionChanged PosChanged;

        
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the emulation state of the device.
        /// </summary>
        //public bool Emulation
        //{
        //    get
        //    {
        //        return mobj_valve.Emulation;
        //    }
        //    set
        //    {
        //        mobj_valve.Emulation = value;
        //    }
        //}
        /// <summary>
        /// Gets or sets the associated device.
        /// </summary>
        public IDevice Device
        {
            get
            {
                return (classValveVICI2Pos)mobj_valve;
            }
            set
            {
                if (!DesignMode)
                {
                    //mobj_valve = (classValveVICI2Pos)value;
                    RegisterDevice(value);
                    //try
                    //{
                    //    string errorMessage = "";
                    //    mobj_valve.Initialize(ref errorMessage);
                    //    mpropertyGrid_Serial.SelectedObject = mobj_valve.Port;
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
                //mobj_valveEventListener = new ValveEventListener(mobj_valve, this);
            }
        }
        #endregion

        #region Methods
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
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ValvePositionEventArgs<TwoPositionState>>(OnPosChanged), new object[] { sender, newPosition });
            }
            else
            {
                if (PosChanged != null)
                {
                    PosChanged(this, newPosition.Position.ToCustomString());
                }
                UpdatePositionTextBox(newPosition.Position);
            }
        }

        /// <summary>
        /// Updates the position textbox on the control.
        /// </summary>
        /// <param name="position"></param>
        private void UpdatePositionTextBox(TwoPositionState position)
        {
            mtextbox_CurrentPos.Text = position.ToCustomString();
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        private void ShowError(string message)
        {
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message);
        }
        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="ex">The exception to describe</param>
        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + "\r\n" + ex.Message, Device.Name);
        }
        private void tabControl1_Validated(object sender, EventArgs e)
        {
            UpdatePositionTextBox(mobj_valve.LastMeasuredPosition);
        }   

        private void btnA_Click(object sender, EventArgs e)
        {
              try
              {
                  /*Thread p = new Thread(() => mobj_valve.SetPosition(TwoPositionState.PositionA));
                  p.Start();
                  p = null;*/
                  mobj_valve.SetPosition(TwoPositionState.PositionA);
                  //UpdatePositionTextBox(mobj_valve.LastMeasuredPosition);
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

        private void btnB_Click(object sender, EventArgs e)
        {
            try
            {
                /*Thread p = new Thread(() => mobj_valve.SetPosition(TwoPositionState.PositionB));
                p.Start();
                p = null;*/
                mobj_valve.SetPosition(TwoPositionState.PositionB);
                //UpdatePositionTextBox(mobj_valve.LastMeasuredPosition);
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

        private void btnRefreshPos_Click(object sender, EventArgs e)
        {
            try
            {
                UpdatePositionTextBox((TwoPositionState)mobj_valve.GetPosition());
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

        private void btnRefreshVer_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_VersionInfo.Text = mobj_valve.GetVersion();
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

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!mobj_valve.Port.IsOpen)
                {
                    mobj_valve.Port.Open();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.Port.Close();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            char newID = mcomboBox_setID.SelectedItem.ToString()[0];
            try
            {
                mobj_valve.SetHardwareID(newID);
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
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

        private void btnRefreshID_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
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

        private void btnClearID_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.ClearHardwareID();
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
                mcomboBox_setID.SelectedIndex = 10;
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
                
        private void mpropertyGrid_Serial_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            OnSaveRequired();
        }
        
        #endregion

        private void mbutton_initialize_Click(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            try
            {
                string errorMessage = "";
                bool success = mobj_valve.Initialize(ref errorMessage);
                
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

            mtextbox_VersionInfo.Text = mobj_valve.Version;
            mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
            mtextbox_currentID.Text = mobj_valve.SoftwareID.ToString();
        }
    }   
}
