using System;
using System.Drawing;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.BrukerStart
{
    public partial class controlBrukerStartGlyph : UserControl, IDeviceGlyph
    {
        private classBrukerStart mobj_brukerStart;

        public controlBrukerStartGlyph()
        {
            InitializeComponent();
            BackColor = Color.Transparent;
        }

        #region IDeviceGlyph Methods
        public void RegisterDevice(IDevice device)
        {
            mobj_brukerStart = device as classBrukerStart;
            mlabel_name.Text = device.Name;
            device.DeviceSaveRequired       += new EventHandler(DeviceSaveRequired);
        }
        void DeviceSaveRequired(object sender, EventArgs e)
        {
            if (mobj_brukerStart != null)
            {
                mlabel_name.Text = mobj_brukerStart.Name;
            }
        }
        public void DeRegisterDevice()
        {
            mobj_brukerStart = null;
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
            get;
            set;
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
}
