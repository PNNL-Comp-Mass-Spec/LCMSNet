using System;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Dashboard
{
    public partial class formResolveDeviceStatus : Form
    {
        public formResolveDeviceStatus()
        {
            InitializeComponent();
        }

        public enumDeviceStatus Status { get; set; }

        private void mbutton_leaveError_Click(object sender, EventArgs e)
        {
            Status = enumDeviceStatus.Error;
        }

        private void mbutton_clearErrors_Click(object sender, EventArgs e)
        {
            Status = enumDeviceStatus.Initialized;
        }

        private void mbutton_notInitialized_Click(object sender, EventArgs e)
        {
            Status = enumDeviceStatus.NotInitialized;
        }
    }
}