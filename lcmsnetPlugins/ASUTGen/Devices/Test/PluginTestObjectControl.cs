using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace LCMSNetPlugins
{
    public partial class PluginTestObjectControl : UserControl, IDeviceControl
    {       
        // LCMSNet plugin device being interacted with
        // TODO: Replace with device type being controlled
        // Use Refactor to rename member to 'real' name
        private PluginTestObjectDevice mTestObjectDevice;


        // Constructor
        public PluginTestObjectControl()
        {
            // Brian boiler plate
            InitializeComponent();
           
        }


        // IDeviceControl interface implementation
        #region IDeviceControl Members

        // Events used by LCMSNet main automation SW ???
        public event DelegateNameChanged  NameChanged;
        public event DelegateSaveRequired SaveRequired;

        // Registers the LCMSNet plugin device being interacted with
        public void RegisterDevice(IDevice device)
        {
            mTestObjectDevice = device as PluginTestObjectDevice;
            mNameTitleLabel.Text = mTestObjectDevice.Name;
            this.mDeviceNameInput.Text = mTestObjectDevice.Name;

        } // End RegisterDevice() method

        // ?????
        public bool Running
        {
            get;
            set;
        }

        // Exposes plugin device being interacted with
        public IDevice Device
        {
            get
            {
                return mTestObjectDevice;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        // ?????
        public void ShowProps()
        {
            
        }

        #endregion



        #region UI Handlers

        // TODO: Write methods that allow configuration and control of device from the dashboard

        /// <summary>
        /// Tests setting the test object's persistent Property property
        /// </summary> 
        private void mIntPropertySetButton_Click(object sender, EventArgs e)
        {
            try
            {
                mTestObjectDevice.IntTestProperty = (int)mTestPropertyInput.Value;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

        }  // End mIntPropertySetButton_Click() mehod

        /// <summary>
        /// Sets the name of the device, this code should be sufficient, and not require modification
        /// </summary> 
        private void mRenameButton_Click(object sender, EventArgs e)
        {
            if ( mDeviceNameInput.Text.Length > 0 )
            {
                try
                {
                    mTestObjectDevice.Name = mDeviceNameInput.Text;
                    mNameTitleLabel.Text = mTestObjectDevice.Name;
                    DeviceManagerBridge.RenameDevice(mTestObjectDevice, mDeviceNameInput.Text);
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            } // End if(mDeviceNameInput.Text.Length > 0)

        } // End mRenameButton_Click() mehod

        /// <summary>
        ///Tests the test plugin's SimpleShowTest() method 
        /// </summary> 
        private void mSimpleShowTestButton_Click(object sender, EventArgs e)
        {
            try
            {
                mTestObjectDevice.SimpleShowTest("");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        } // End mSimpleShowTestButton_Click() mehod

        /// <summary>
        /// Tests the test objects's FunctionTest() method
        /// </summary> 
        private void mFunctionTestButton_Click(object sender, EventArgs e)
        {
            try
            {
               mTestFunctionResultOutput.Value= mTestObjectDevice.FunctionTest( "", mTestFunctionInput.Value.ToString() );
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        } // End mFunctionTestButton_Click() mehod

        /// <summary>
        ///Tests the test objects's ShowTest() method 
        /// </summary> 
        private void mShowTestButton_Click(object sender, EventArgs e)
        {
            try
            {
               mTestObjectDevice.ShowTest( "", mShowTestInput.Text );
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        } // End mShowTestButton_Click() mehod

        /// <summary>
        ///Tests the test objects's WaitTest() method 
        /// </summary> 
        private void mWaitTestButton_Click(object sender, EventArgs e)
        {
            try
            {
               mTestObjectDevice.WaitTest( "", mWaitTestInput.Value.ToString() );
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        } // End mWaitTestButton_Click() mehod

        /// <summary>
        /// Tests the test object's querying fucntion
        /// </summary> 
        private void mTestQueryButton_Click(object sender, EventArgs e)
        {
            try
            {
                mTestQueryOutput.Value= (Decimal)mTestObjectDevice.TestQuery("");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

        } // End mTestQueryButton_Click() mehod

        #endregion UI Handlers



        /// <summary>
        /// Method to call if one of the control UI handlers has an error
        /// </summary>  
        void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error Occured");
        }

        private void DeviceControlOperationPage_Click(object sender, EventArgs e)
        {

        } // End HandleError() method

      


    } // End ItemControl class

} // End LCMSNetPlugins NS
