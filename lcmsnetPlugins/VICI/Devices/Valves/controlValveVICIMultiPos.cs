/*********************************************************************************************************
// Written by Dave Clark, John Ryan, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 06/30/2009
//
// Last modified 06/30/2009
//                      11/03/2009: Removed objects that perform no function.
 *                          Removed the multiport listener class.
*********************************************************************************************************/

using System;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using System.Threading;

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICIMultiPos : controlBaseDeviceControl, IDeviceControl
    {
        #region Members
        /// <summary>
        /// Class that interfaces the hardware.
        /// </summary>
        private classValveVICIMultiPos m_valve;
        /// <summary>
        /// Event fired when the position of the valve changes.
        /// </summary>
        public event EventHandler<ValvePositionEventArgs<int>> PositionChanged;
        #endregion

        #region Constructors
        public controlValveVICIMultiPos()
        {
            InitializeComponent();

            //Populate the combobox
            PopulateComboBox();

        }
        private void RegisterDevice(IDevice device)
        {
            m_valve = device as classValveVICIMultiPos;
            if (m_valve != null)
            {
                m_valve.PosChanged += OnPosChanged;
            }
            SetBaseDevice(m_valve);

            mpropertyGrid_Serial.SelectedObject = m_valve.Port;

            PopulateComboBox();
        }
        private void PopulateComboBox()
        {
            if (m_valve != null)
            {
                var enums = Enum.GetValues(m_valve.GetStateType());

                foreach (var o in enums)
                {
                    mcomboBox_Position.Items.Add(o);
                }
            }
        }
        #endregion

        #region Events

        //Position change
        public virtual void OnPosChanged(object sender, ValvePositionEventArgs<int> newPosition)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ValvePositionEventArgs<int>>(OnPosChanged), sender, newPosition);
            }
            else
            {
                PositionChanged?.Invoke(this, new ValvePositionEventArgs<int>(newPosition.Position));
                mtextbox_CurrentPos.Text = newPosition.Position.ToString();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or sets the flag determining if the system is in emulation mode.
        /// </summary>
        public bool Emulation
        {
            get
            {
                return m_valve.Emulation;
            }
            set
            {
                m_valve.Emulation = value;
            }
        }
        /// <summary>
        /// Gets or sets the device associated with this control.
        /// </summary>
        public IDevice Device
        {
            get
            {
                return m_valve;
            }
            set
            {

                if (!DesignMode)
                {
                    RegisterDevice(value);

                    /*
                    string errorMessage = "";
                    m_valve = (classValveVICIMultiPos)value;
                    try
                    {
                        m_valve.Initialize(ref errorMessage);
                        mpropertyGrid_Serial.SelectedObject = m_valve.Port;
                    }
                    catch (ValveExceptionReadTimeout ex)
                    {
                        showError("Timeout (read) when attempting to initialize valve", ex);
                    }
                    catch (ValveExceptionWriteTimeout ex)
                    {
                        showError("Timeout (write) when attempting to initialize valve", ex);
                    }
                    catch (ValveExceptionUnauthorizedAccess ex)
                    {
                        showError("Unauthorized access when attempting to initialize valve", ex);
                    }*/
                }
            }
        }

        #endregion

        #region Methods

        private void showError(string message)
        {
            UpdateStatusDisplay(message);
        }

        private void showError(string message, Exception ex)
        {
            UpdateStatusDisplay(message + " " + ex.Message);// + ex.InnerException.Message);
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void valvetest_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_Validated(object sender, EventArgs e)
        {
            mtextbox_CurrentPos.Text = m_valve.LastMeasuredPosition.ToString();
        }

        private void btnRefreshPos_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_CurrentPos.Text = m_valve.GetPosition().ToString();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to get valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to get valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to get valve position", ex);
            }
        }

        private void btnRefreshVer_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_VersionInfo.Text = m_valve.GetVersion();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to get valve version", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to get valve version", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to get valve version", ex);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
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
                showError("Null reference when attempting to open port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access exception when attempting to open port", ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                m_valve.Port.Close();
            }
            catch (NullReferenceException ex)
            {
                showError("Null reference when attempting to close port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access exception when attempting to close port", ex);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newID = mcomboBox_setID.SelectedItem.ToString()[0];
            try
            {
                m_valve.SetHardwareID(newID);
                mtextbox_currentID.Text = m_valve.GetHardwareID().ToString();
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to set valve ID", ex);
            }
        }

        private void btnRefreshID_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_currentID.Text = m_valve.GetHardwareID().ToString();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to get valve ID", ex);
            }
        }

        private void btnClearID_Click(object sender, EventArgs e)
        {
            try
            {
                m_valve.ClearHardwareID();
                mtextbox_currentID.Text = m_valve.GetHardwareID().ToString();
                mcomboBox_setID.SelectedIndex = 10;
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to clear valve ID", ex);
            }
        }

        private void mpropertyGrid_Serial_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            OnSaveRequired();
        }

        #endregion

        private void mbutton_SetPosition_Click(object sender, EventArgs e)
        {
            if (mcomboBox_Position.SelectedItem == null)
            {
                LcmsNetDataClasses.Logging.classApplicationLogger.LogError(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_USER, "A valve position selection should be made.");
                return;
            }
            var pos = (int)mcomboBox_Position.SelectedItem;
            var p = new Thread(() => SetPosition(pos));
            p.Start();
            p = null;
        }

        private void SetPosition(int pos)
        {
            m_valve.SetPosition(pos);
        }

        private void mbutton_GetNumPos_Click(object sender, EventArgs e)
        {
            mtextBox_GetNumPos.Text = m_valve.GetNumberOfPositions().ToString();
        }
        private void mbutton_SetNumPos_Click(object sender, EventArgs e)
        {
            m_valve.SetNumberOfPositions(Convert.ToInt32(mtextBox_SetNumPos.Text));
        }

        /// <summary>
        /// Handles initializing the device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_initialize_Click(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            mpropertyGrid_Serial.SelectedObject = m_valve.Port;

            var errorMessage = "";
            try
            {
                m_valve.Initialize(ref errorMessage);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to initialize valve", ex);
            }

            mtextbox_VersionInfo.Text = m_valve.Version;
            mtextbox_CurrentPos.Text = m_valve.LastMeasuredPosition.ToString();
            mtextbox_currentID.Text = m_valve.SoftwareID.ToString();
        }
    }
}
