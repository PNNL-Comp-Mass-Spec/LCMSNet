using System;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;


namespace LcmsNet.Devices.ContactClosure
{
    public partial class controlContactClosureGlyphU3 : UserControl, IDeviceGlyph
    {
        private classContactClosureU3 mobj_contactClosure;

        public controlContactClosureGlyphU3()
        {
            InitializeComponent();
        }


        #region IDeviceGlyph Methods

        public void RegisterDevice(IDevice device)
        {
            mobj_contactClosure = device as classContactClosureU3;
            mlabel_name.Text = device.Name;
            device.DeviceSaveRequired += new EventHandler(DeviceSaveRequired);
        }
        void DeviceSaveRequired(object sender, EventArgs e)
        {
            if (mobj_contactClosure != null)
            {
                mlabel_name.Text = mobj_contactClosure.Name;
            }
        }

        public void DeRegisterDevice()
        {            
            mobj_contactClosure = null;
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
        public controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }
    }
}
