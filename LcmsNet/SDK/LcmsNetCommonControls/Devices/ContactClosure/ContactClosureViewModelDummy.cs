using System;
using LcmsNetDataClasses.Devices;

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
        public override double MinimumVoltage
        {
            get { return -5; }
        }

        public override double MaximumVoltage
        {
            get { return 5; }
        }

        public override int MinimumPulseLength
        {
            get { return 0; }
        }

        public override IDevice Device { get; set; }

        protected override void SendPulse()
        {
            Console.WriteLine("Sending pulse...");
        }
    }
}
