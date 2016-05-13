using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class controlBaseDevice : UserControl
    {
        //include a member which is the device class itself?

        public controlBaseDevice()
        {
            InitializeComponent();
        }

        public void AddToDeviceManager()
        {
            //Add this device to the device manager
            //classDeviceManager.AddDevice(this);
        }

        public void RemoveFromDeviceManager()
        {
            //Remove this device from the device manager
            //classDeviceManager.RemoveDevice(this);
        }

        public void RenameDevice(string s)
        {
            //Rename this device in the device manager
            //Validate the string?
            //if(classDeviceManager.RenameDevice(this,string))
            //{
            //  //Rename the name inside of the class
            //}
        }

        public enumInitializationStatus Initialize()
        {   
            //Call the associated class's Initialize function
            return enumInitializationStatus.Error;
        }

        private void mtextBox_NewDeviceName_TextChanged(object sender, EventArgs e)
        {

        }

        private void mbutton_RenameDevice_Click(object sender, EventArgs e)
        {
            RenameDevice(mtextBox_NewDeviceName.Text);
            OnNameChanged(mtextBox_NewDeviceName.Text);
            OnSaveRequired();
        }

        #region Events

        //Name change
        public delegate void DelegateNameChangeEventHandler(object sender, string newname);
        public event DelegateNameChangeEventHandler NameChanged;
        protected virtual void OnNameChanged(string newname)
        {
            if (NameChanged != null)
            {
                NameChanged(this, newname);
            }
        }

        //Save required
        public delegate void DelegateSaveRequiredEventHandler(object sender);
        public event DelegateSaveRequiredEventHandler SaveRequired;
        protected virtual void OnSaveRequired()
        {
            if (SaveRequired != null)
            {
                SaveRequired(this);
            }
        }

        #endregion
    }
}
