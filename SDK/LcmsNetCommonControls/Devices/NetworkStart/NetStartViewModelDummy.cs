using System;
using LcmsNetSDK.Devices;

namespace LcmsNetCommonControls.Devices.NetworkStart
{
    internal class NetStartViewModelDummy : NetStartViewModelBase
    {
        public override IDevice Device { get; set; }
        protected override void StartAcquisition()
        {
            Console.WriteLine("Start Acquisition");
        }

        protected override void StopAcquisition()
        {
            Console.WriteLine("Stop Acquisition");
        }

        protected override void RefreshMethods()
        {
            Console.WriteLine("Reload methods");
        }

        protected override void IPAddressUpdated()
        {
            Console.WriteLine("Update IP Address");
        }

        protected override void PortUpdated()
        {
            Console.WriteLine("Update Port");
        }
    }
}
