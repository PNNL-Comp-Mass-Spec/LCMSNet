using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApplication2
{
    public partial class controlValveVICI2Pos : controlBaseDevice
    {
        public static SerialPort testPort = new SerialPort();
        private classValveVICI2Pos mobj_valve;
        //public EventListener mobj_eventlistener;

        public controlValveVICI2Pos()
        {
            
            InitializeComponent();

            /// Dummy
            //mobj_valve = new classValveVICI2Pos();
            //mobj_valve.Port = testPort;
        }

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

        public classValveVICI2Pos Valve
        {
            get
            {
                return mobj_valve;
            }
            set
            {
                if (!DesignMode)
                {
                    mobj_valve = value;
                    try
                    {
                        mobj_valve.InitializeValve();
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

                //mobj_eventlistener = new EventListener(mobj_valve);
                
            }
        }


        private void valvetest_Load(object sender, EventArgs e)
        {

            if (DesignMode)
                return;

            mpropertyGrid_Serial.SelectedObject = mobj_valve.Port; // testPort;

            try
            {
                mobj_valve.InitializeValve();
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

            mtextbox_VersionInfo.Text = mobj_valve.VersionInfo;
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
    }
}
