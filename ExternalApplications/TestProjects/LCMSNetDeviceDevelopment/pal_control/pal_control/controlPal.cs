using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//TODO: No validation for text boxes (should be numbers, positive, etc...)
//TODO: Exceptions. Heh.

namespace pal_control
{
    public partial class controlPal : controlBaseDeviceControl
    {
        #region Constructors 

        public controlPal()
        {
            InitializeComponent();
            Device = new classPal();
            //TODO: Change this! Debug stuff on next two lines.
            //Device.Initialize();
            Device.MethodsFolder = @"C:\Program Files\PAL\Cycle Composer\HTS PAL";
            mstr_RawMethodList = Device.ListMethods();
            ProcessMethods(mstr_RawMethodList);
        }

        #endregion

        #region Members

        private classPal mobj_Pal;
        private string mstr_RawMethodList;
        private List<String> mobj_MethodList;

        #endregion

        #region Properties

        public classPal Device
        {
            get
            {
                return mobj_Pal;
            }
            set
            {
                mobj_Pal = value;
            }
        }

        public System.IO.Ports.SerialPort Port
        {
            get
            {
                return mobj_Pal.Port;
            }
            set
            {
                mobj_Pal.Port = value;
            }
        }

        public string MethodsFolder
        {
            get
            {
                return mobj_Pal.MethodsFolder;
            }
            set
            {
                mobj_Pal.MethodsFolder = value;
            }
        }

        #endregion

        #region Methods

        public void ProcessMethods(string rawMethodList)
        {
            //Convert the methods into a list of strings
            //We're going to get something that looks like:
            //  Multi Step LC Injection;Single Step LC Injection;Dance;
            //So separate it by semicolons

            mobj_MethodList = new List<String>(mstr_RawMethodList.Split(';'));
            mobj_MethodList.Remove(""); //Remove trailing empty string, as list ends in ;

            //Clear out the previous list
            mcomboBox_MethodList.Items.Clear();

            //That was cool, so now fill up the combobox
            foreach (string s in mobj_MethodList)
            {
                mcomboBox_MethodList.Items.Add(s);
            }
        }

        private void mButton_RefreshMethods_Click(object sender, EventArgs e)
        {
            mstr_RawMethodList = Device.ListMethods();
            ProcessMethods(mstr_RawMethodList);
        }

        private void mButton_RunMethod_Click(object sender, EventArgs e)
        {
            Device.LoadMethod(mcomboBox_MethodList.SelectedText, mTextBox_Tray.Text, mTextBox_Vial.Text, mTextBox_Volume.Text);
            Device.StartMethod(60000);
        }

        private void mButton_Initialize_Click(object sender, EventArgs e)
        {
            Device.Initialize();
        }

        private void mButton_StatusRefresh_Click(object sender, EventArgs e)
        {
            mTextBox_Status.Text = Device.GetStatus();
        }

        #endregion
    }
}
