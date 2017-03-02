using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Pumps
{
    public partial class controlPumpIscoGlyph : UserControl, IDeviceGlyph
    {
        #region "Class variables"
            private classPumpIsco mobj_IscoPump;
        #endregion

        #region "Properties"
            public UserControl UserControl
            {
                get { return this; }
            }

            public int ZOrder { get; set; }
        #endregion

        #region "Constructors"
            public controlPumpIscoGlyph()
            {
                InitializeComponent();
            }
        #endregion

        #region "Methods"
        #endregion

        #region IDeviceGlyph Methods
            public void RegisterDevice(IDevice device)
            {
                mobj_IscoPump = device as classPumpIsco;
                mlabel_name.Text = device.Name;
                device.DeviceSaveRequired += new EventHandler(DeviceSaveRequired);
            }
            void DeviceSaveRequired(object sender, EventArgs e)
            {
                if (mobj_IscoPump != null)
                {
                    mlabel_name.Text = mobj_IscoPump.Name;
                }
            }

            public void DeRegisterDevice()
            {
                //TODO: Why is this here?  
                mobj_IscoPump.Disconnect();
                mobj_IscoPump = null;
            }
        #endregion

            /// <summary>
            /// Gets or sets the status display bar.
            /// </summary>
            public LcmsNetDataClasses.Devices.controlDeviceStatusDisplay StatusBar
            {
                get;
                set;
            }
    }   
}   // End namespace
