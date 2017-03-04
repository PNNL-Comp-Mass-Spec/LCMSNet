using System;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;

namespace LcmsNet.Devices.Valves
{
    public partial class controlSixPortInjectionValve : controlValveVICI2Pos
    {
        public controlSixPortInjectionValve()
        {
            InitializeComponent();
        }

        private void btnSetInjectionVolume_Click(object sender, EventArgs e)
        {
            var injector = Device as ISixPortInjectionValve;
            var volume = Convert.ToDouble(txtInjectionVolume.Text);
            if (injector != null)
                injector.InjectionVolume = volume;
        }

        public override void RegisterDevice(IDevice device)
        {
            base.RegisterDevice(device);

            var injector = Device as ISixPortInjectionValve;
            if (injector != null)
                txtInjectionVolume.Text = injector.InjectionVolume.ToString("0.000");
        }
    }
}
