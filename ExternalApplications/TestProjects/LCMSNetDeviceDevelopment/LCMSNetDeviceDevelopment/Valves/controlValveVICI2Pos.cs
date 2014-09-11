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

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICI2Pos : controlBaseDeviceControl
    {
        #region Constructors

        public controlValveVICI2Pos()
        {

            InitializeComponent();

            /// Dummy
            //mobj_valve = new classValveVICI2Pos();
            //mobj_valve.Port = testPort;
        }

        #endregion

        #region Events

        //Position change
        public event DelegatePositionChanged PosChanged;
        public virtual void OnPosChanged(LcmsNetDataClasses.Devices.enumValvePosition2Pos newPosition)
        {
            if (PosChanged != null)
            {
                PosChanged(this, newPosition);
            }
        }

        #endregion

        #region Members

        public static SerialPort testPort = new SerialPort();

        private classValveVICI2Pos mobj_valve;

        public ValveEventListener mobj_valveEventListener;

        #endregion

        #region Properties

        public IDevice Device
        {
            get
            {
                return (IDevice)mobj_valve;
            }
            set
            {
                if (!DesignMode)
                {
                    mobj_valve = (classValveVICI2Pos)value;
                    try
                    {
                        mobj_valve.Initialize();
                        mpropertyGrid_Serial.SelectedObject = mobj_valve.Port;
                    }
                    catch (ValveExceptionReadTimeout ex)
                    {
                        showError("Timeout (read) when attempting to intialize valve", ex);
                    }
                    catch (ValveExceptionWriteTimeout ex)
                    {
                        showError("Timeout (write) when attempting to intialize valve", ex);
                    }
                    catch (ValveExceptionUnauthorizedAccess ex)
                    {
                        showError("Unauthorized access when attempting to intialize valve", ex);
                    }

                }

                mobj_valveEventListener = new ValveEventListener(mobj_valve, (IDeviceControl)this);

            }
        }

        #endregion

        #region Methods

        private void showError(string message)
        {
            MessageBox.Show(message);
        }

        private void showError(string message, Exception ex)
        {
            MessageBox.Show(message + "\r\n" + ex.Message);// + ex.InnerException.Message);
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void valvetest_Load(object sender, EventArgs e)
        {

            if (DesignMode)
                return;

            mpropertyGrid_Serial.SelectedObject = mobj_valve.Port; // testPort;

            try
            {
                mobj_valve.Initialize();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to intialize valve", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to intialize valve", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to intialize valve", ex);
            }

            mtextbox_VersionInfo.Text = mobj_valve.Version;
            mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
            mtextbox_currentID.Text = mobj_valve.SoftwareID.ToString();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_Validated(object sender, EventArgs e)
        {
            mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
        }

        private void btnA_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.SetPosition(enumValvePosition2Pos.A);
                mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                showError("Valve position mismatch", ex);
            }
        }

        private void btnB_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.SetPosition(enumValvePosition2Pos.B);
                mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                showError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                showError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                showError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                showError("Valve position mismatch", ex);
            }
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

        #endregion
    }

    #region I Don't Understand This Code

    //This is kind of f'd up, but if it works...
    public class ValveEventListener
    {
        private classValveVICI2Pos Valve;
        private controlValveVICI2Pos Control;
        private controlBaseDeviceControl BaseControl;
        

        public ValveEventListener(classValveVICI2Pos valve, IDeviceControl control)
        {
            Valve = valve;
            Control = (controlValveVICI2Pos)control;
            BaseControl = (controlBaseDeviceControl)control;
            
            Valve.PosChanged += new DelegateDevicePositionChange(Valve_PosChanged);
            Valve.DeviceSaveRequired += new DelegateDeviceSaveRequired(Valve_DeviceSaveRequired);
        }

        void Valve_DeviceSaveRequired(object sender)
        {
            //Propogate this
            //TODO: Figure out if this actually worked or not
            //System.Windows.Forms.MessageBox.Show("OH SNAP WE NEED TO SAVE");
        }

        void Valve_PosChanged(object sender, LcmsNetDataClasses.Devices.enumValvePosition2Pos p)
        {
            //Propogate this
            //TODO: Figure out if this actually worked or not
            //System.Windows.Forms.MessageBox.Show("OH SNAP THE POSITION CHANGED TO " + p.ToString());
            Control.OnPosChanged(p);
        }
    }

    #endregion
}