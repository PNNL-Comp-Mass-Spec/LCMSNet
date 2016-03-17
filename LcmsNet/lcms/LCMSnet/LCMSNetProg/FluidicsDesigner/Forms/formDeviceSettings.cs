using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.FluidicsDesigner.Forms
{
    public partial class formDeviceSettings : Form
    {
        private const int WidthAdder = 5;
        private const int HeightAdder = 40;


        public formDeviceSettings()
        {
            InitializeComponent();
        }


        public IDeviceControl DeviceControl
        {
            set
            {
                Control tempControl = (Control)value;
                this.Width          = tempControl.Width + WidthAdder;
                this.Height         = tempControl.Height + HeightAdder;
                tempControl.Dock    = DockStyle.Fill;
                this.Controls.Add((Control)value);
            }
        }
    }
}
