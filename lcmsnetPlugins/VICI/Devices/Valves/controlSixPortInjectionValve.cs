using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;

namespace LcmsNet.Devices.Valves
{
    public partial class controlSixPortInjectionValve : controlValveVICI2Pos, IDeviceControl
    {
        public controlSixPortInjectionValve()
        {
            InitializeComponent();
        }

        private void btnSetInjectionVolume_Click(object sender, EventArgs e)
        {
            ISixPortInjectionValve injector = Device as ISixPortInjectionValve;
            double volume = Convert.ToDouble(txtInjectionVolume.Text);
            injector.InjectionVolume = volume;
        }

        public override void RegisterDevice(IDevice device)
        {
            base.RegisterDevice(device);

            ISixPortInjectionValve injector = Device as ISixPortInjectionValve;
            txtInjectionVolume.Text = injector.InjectionVolume.ToString();
        }
    }
}
