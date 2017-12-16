using System;
using LcmsNetDataClasses.Devices;

namespace LcmsNetCommonControls.Devices.ContactClosureRead
{
    internal enum DummyInputPorts
    {
        A0,
        A1,
        A2,
        A3,
    }

    internal class ContactClosureReadViewModelDummy : ContactClosureReadViewModelBase<DummyInputPorts>
    {
        public ContactClosureReadViewModelDummy()
        {
            IsAnalog = true;
        }

        public override IDevice Device { get; set; }

        protected override void ReadStatus()
        {
            if (Status == ContactClosureState.Closed)
            {
                Status = ContactClosureState.Open;
            }
            else
            {
                Status = ContactClosureState.Closed;
            }
        }
    }
}
