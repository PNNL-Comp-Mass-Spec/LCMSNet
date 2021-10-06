using System;
using LcmsNetSDK.Devices;

namespace LcmsNetCommonControls.Devices.ContactClosure
{
    internal enum DummyOutputPorts
    {
        A0,
        A1,
        A2,
        A3,
    }

    internal class ContactClosureViewModelDummy : ContactClosureViewModelBase<DummyOutputPorts>
    {
        public override double MinimumVoltage => -5;

        public override double MaximumVoltage => 5;

        public override int MinimumPulseLength => 0;

        public override IDevice Device { get; set; }

        protected override void SendPulse()
        {
            Console.WriteLine("Sending pulse...");
        }
    }
}
