using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class SixPortFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;

        public SixPortFluidicsValve() :
            base(NUMBER_OF_PORTS, false)
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 6-port Cheminert: A is 6-1, B is 1-2
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as ISixPortValve;
            SetBaseDevice(valve);
        }
    }
}
