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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using System.Xml;
using System.Xml.Linq;

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICIMultiPos : controlBaseDeviceControl, IDeviceControl
    {
        #region Members
        /// <summary>
        /// Class that interfaces the hardware.
        /// </summary>
        private classValveVICIMultiPos          mobj_valve;
        /// <summary>
        /// Event fired when the position of the valve changes.
        /// </summary>
        public  event DelegatePositionChanged   PositionChanged;
        #endregion

        #region Constructors
        public controlValveVICIMultiPos()
        {        
            InitializeComponent();

            //Populate the combobox
            Array enums = Enum.GetValues(typeof(enumValvePositionMultiPos));
            
            foreach(object o in enums)
            {
                enumValvePositionMultiPos pos = (enumValvePositionMultiPos) o;
                mcomboBox_Position.Items.Add(pos);
            }
        }
        private void RegisterDevice(IDevice device)
        {
            mobj_valve              = device as classValveVICIMultiPos;
            mobj_valve.PosChanged  += new DelegateDevicePositionChange(OnPosChanged);
            SetBaseDevice(mobj_valve);
            
            mpropertyGrid_Serial.SelectedObject     = mobj_valve.Port;
        }

        #endregion

        #region Events

        //Position change
        public virtual void OnPosChanged(object sender, string newPosition)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateDevicePositionChange(OnPosChanged), new object[] { sender, newPosition });
            }
            else
            {
                if (PositionChanged != null)
                {
                    PositionChanged(this, newPosition);
                }
                mtextbox_CurrentPos.Text = newPosition;
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
                return mobj_valve.Emulation;
            }
            set
            {
                mobj_valve.Emulation = value;
            }
        }
        /// <summary>
        /// Gets or sets the device associated with this control.
        /// </summary>
        public IDevice Device
        {
            get
            {
                return (classValveVICIMultiPos)mobj_valve;
            }
            set
            {
                
                if (!DesignMode)
                {
                    RegisterDevice(value);

                    /*
                    string errorMessage = "";
                    mobj_valve = (classValveVICIMultiPos)value;
                    try
                    {
                        mobj_valve.Initialize(ref errorMessage);
                        mpropertyGrid_Serial.SelectedObject = mobj_valve.Port;
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
            mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
        }

        private void btnRefreshPos_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_CurrentPos.Text = mobj_valve.GetPosition().ToString();
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
                mtextbox_VersionInfo.Text = mobj_valve.GetVersion();
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
                if (!mobj_valve.Port.IsOpen)
                {
                    mobj_valve.Port.Open();
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
                mobj_valve.Port.Close();
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
            char newID = mcomboBox_setID.SelectedItem.ToString()[0];
            try
            {
                mobj_valve.SetHardwareID(newID);
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
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
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
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
                mobj_valve.ClearHardwareID();
                mtextbox_currentID.Text = mobj_valve.GetHardwareID().ToString();
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
            mobj_valve.SetPosition( ( enumValvePositionMultiPos )mcomboBox_Position.SelectedItem);
        }
        private void mbutton_GetNumPos_Click(object sender, EventArgs e)
        {
            mtextBox_GetNumPos.Text = mobj_valve.GetNumberOfPositions().ToString();
        }
        private void mbutton_SetNumPos_Click(object sender, EventArgs e)
        {
            mobj_valve.SetNumberOfPositions(Convert.ToInt32(mtextBox_SetNumPos.Text));
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

            mpropertyGrid_Serial.SelectedObject = mobj_valve.Port;

            string errorMessage = "";
            try
            {
                mobj_valve.Initialize(ref errorMessage);
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

            mtextbox_VersionInfo.Text = mobj_valve.Version;
            mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
            mtextbox_currentID.Text = mobj_valve.SoftwareID.ToString();
        }
    }    
}
