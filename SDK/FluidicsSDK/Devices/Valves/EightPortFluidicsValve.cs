using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class EightPortFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 8;

        public EightPortFluidicsValve() :
            base(NUMBER_OF_PORTS, false)
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 8-port Cheminert: A is 8-1, B is 1-2
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as IEightPortValve;
            SetBaseDevice(valve);
        }
    }
}
