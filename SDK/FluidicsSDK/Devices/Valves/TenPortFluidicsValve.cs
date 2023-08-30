using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class TenPortFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 10;

        public TenPortFluidicsValve() :
            base(NUMBER_OF_PORTS, false)
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 10-port Cheminert: A is 10-1, B is 1-2
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as ITenPortValve;
            SetBaseDevice(valve);
        }
    }
}
