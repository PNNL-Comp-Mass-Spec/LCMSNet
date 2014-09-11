using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace AmpsBox
{
    public partial class AmpsBoxGlyph : UserControl, IDeviceGlyph
    {
        private AmpsBoxDevicePlugin m_box;

        public AmpsBoxGlyph()
        {
            InitializeComponent();
        }


        #region IDeviceGlyph Members

        public void RegisterDevice(IDevice device)
        {
            m_box               = device as AmpsBoxDevicePlugin;
            mlabel_name.Text    = device.Name; 

            device.DeviceSaveRequired += new EventHandler(device_DeviceSaveRequired);
        }

        void device_DeviceSaveRequired(object sender, EventArgs e)
        {
            if (m_box != null)
            {
                mlabel_name.Text = m_box.Name;
            }
        }

        public void DeRegisterDevice()
        {
            if (m_box != null)
            {
                m_box.DeviceSaveRequired -= device_DeviceSaveRequired;
                m_box = null;
            }
        }

        public new UserControl UserControl
        {
            get { return this; }
        }

        public int ZOrder
        {
            get;
            set;
        }

        public controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }
        #endregion

        private void AmpsBoxGlyph_Load(object sender, EventArgs e)
        {

        }
    }
}
