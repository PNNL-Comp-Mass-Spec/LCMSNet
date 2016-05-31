using System;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICI2PosGlyph : UserControl, IDeviceGlyph
    {
        private classValveVICI2Pos mobj_valve;

        public controlValveVICI2PosGlyph()
        {
            InitializeComponent();
        }

        #region IDeviceGlyph Members

        public void RegisterDevice(IDevice device)
        {
            mobj_valve                  = device as classValveVICI2Pos;
            mlabel_name.Text            = device.Name;
            mobj_valve.PositionChanged      += new DelegateDevicePositionChange(mobj_valve_PositionChanged);
            device.DeviceSaveRequired       += new EventHandler(DeviceSaveRequired);
        }
        void DeviceSaveRequired(object sender, EventArgs e)
        {
            if (mobj_valve != null)
            {
                mlabel_name.Text = mobj_valve.Name;
            }
        }

        void mobj_valve_PositionChanged(object sender, string p)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateDevicePositionChange(mobj_valve_PositionChanged), new object[] { sender, p });
            }
            else
            {
                mtextbox_CurrentPos.Text = p;
            }
        }

        public void DeRegisterDevice()
        {
            mobj_valve = null;
        }

        public UserControl UserControl
        {
            get { return this; }
        }

        public int ZOrder
        {
            get;set;
        }

        #endregion

        private void mbutton_setPosition_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.SetPosition(enumValvePosition2Pos.A);
                mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
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

        private void mbutton_setPositionB_Click(object sender, EventArgs e)
        {
            try
            {
                mobj_valve.SetPosition(enumValvePosition2Pos.B);
                mtextbox_CurrentPos.Text = mobj_valve.LastMeasuredPosition.ToString();
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

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        private void ShowError(string message, Exception ex)
        {
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message, ex);
        }

        private void mbutton_refreshPos_Click(object sender, EventArgs e)
        {
            try
            {
                mtextbox_CurrentPos.Text = mobj_valve.GetPosition().ToString();
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
    }
}
