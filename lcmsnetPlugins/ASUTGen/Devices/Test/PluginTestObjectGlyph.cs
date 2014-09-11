using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LCMSNetPlugins
{
    public partial class PluginTestObjectGlyph : UserControl, IDeviceGlyph
    {
        // LCMSNet plugin device being interacted with
        // TODO: Replace with device type being controlled
        // Use Refactor to rename member to 'real' name
        private PluginTestObjectDevice mTestObjectDevice;


        // Constructor
        public PluginTestObjectGlyph()
        {
            // Brian boilerplate
            InitializeComponent();
            BackColor = Color.Transparent;
        }


        // IDeviceGlyph interface implementation
        #region IDeviceGlyph Members

        // Registers the LCMSNet plugin device being interacted with
        public void RegisterDevice(IDevice device)
        {
            mTestObjectDevice = device as PluginTestObjectDevice;
        }

        // Deregisters the LCMSNet plugin device being interacted with
        public void DeRegisterDevice()
        {
            mTestObjectDevice = null;
        }

        // Returns a ref to this object
        public new UserControl UserControl
        {
            get { return this; }
        }

        // ?????
        public int ZOrder
        {
            get;
            set;
        }

        // ?????
        public controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }

        #endregion IDeviceGlyph Members



        #region UI Handlers

        // TODO: Replace with device methods to be controlled from glyph,
        // this is most likely, all that will required for the glyph
        /// <summary>
        /// Refreshes the device data being displayed by the glyph,
        /// i.e. the current name and the device's ToString() data
        /// </summary>       
        private void mRefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                mDeviceNameDisplay.Text = mTestObjectDevice.Name;
                mDeviceToStringDisplay.Text = mTestObjectDevice.DeviceToString();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

        } // End mMethod1Button_Click() method      

        #endregion UI Handlers



        /// <summary>
        /// Method to call if one of the glyph UI handlers has an error
        /// </summary>       
        void HandleError( Exception ex )
        {
            MessageBox.Show(ex.Message, "Error Occured");
        } // End HandleError() method


    } // End ItemGlyph class

} // End LCMSNetPlugins NS
