using System;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices.Valves
{
    public partial class controlValveVICIMultiPosGlyph : UserControl, IDeviceGlyph
    {
        /// <summary>
        /// 
        /// </summary>
        private classValveVICIMultiPos mobj_multiPositionValve;

        /// <summary>
        /// 
        /// </summary>
        public controlValveVICIMultiPosGlyph()
        {
            InitializeComponent();

            //Populate the combobox
            Array enums = Enum.GetValues(typeof(enumValvePositionMultiPos));

            foreach (object o in enums)
            {
                enumValvePositionMultiPos pos = (enumValvePositionMultiPos)o;
                mcomboBox_Position.Items.Add(pos);
            }
        }

        #region IDeviceGlyph Methods

        public void RegisterDevice(IDevice device)
        {
            mobj_multiPositionValve             = device as classValveVICIMultiPos;
            mlabel_name.Text                    = device.Name;
            mobj_multiPositionValve.PosChanged += new DelegateDevicePositionChange(mobj_multiPositionValve_PosChanged);
            device.DeviceSaveRequired += new EventHandler(DeviceSaveRequired);
        }
        void DeviceSaveRequired(object sender, EventArgs e)
        {
            if (mobj_multiPositionValve != null)
            {
                mlabel_name.Text = mobj_multiPositionValve.Name;
            }
        }

        void mobj_multiPositionValve_PosChanged(object sender, string p)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateDevicePositionChange(mobj_multiPositionValve_PosChanged), new object[] { sender, p });
            }
            else
            {              
                mtextbox_CurrentPos.Text = p;
            }
        }

        public void DeRegisterDevice()
        {
            mobj_multiPositionValve = null;
        }
        #endregion

        #region Properties
        public UserControl UserControl
        {
            get
            {
                return this;
            }
        }
        public int ZOrder
        {
            get;set;
        }
        #endregion        

        private void mbutton_SetPosition_Click(object sender, EventArgs e)
        {

            try
            {
                mobj_multiPositionValve.SetPosition((enumValvePositionMultiPos)mcomboBox_Position.SelectedItem);
                mtextbox_CurrentPos.Text = mobj_multiPositionValve.LastMeasuredPosition.ToString();
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
            catch (NullReferenceException ex)
            {
                if (mobj_multiPositionValve != null)
                {
                    ShowError("A valve position selection should be made.", ex);
                }
                else
                {
                    ShowError("The valve was not initialized properly.", ex);
                }
            }
        }

        private void mbutton_refreshPos_Click(object sender, EventArgs e)
        {

            try
            {
                mtextbox_CurrentPos.Text = mobj_multiPositionValve.GetPosition().ToString();
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
        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        private void ShowError(string message, Exception ex)
        {
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message, ex);
        }


        /// <summary>
        /// Gets or sets the status display bar.
        /// </summary>
        public controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }
    }
}
