using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using System.Windows.Forms;

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