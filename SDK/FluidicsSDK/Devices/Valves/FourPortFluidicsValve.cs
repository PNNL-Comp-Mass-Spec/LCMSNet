using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class FourPortFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 4;

        public FourPortFluidicsValve() :
            base(NUMBER_OF_PORTS, false)
        {
            // https://www.vici.com/support/app/2p_japp.php
            // 4-port Cheminert: A is 4-1, B is 1-2
        }

        protected override void SetDevice(IDevice device)
        {
            var valve = device as IFourPortValve;
            SetBaseDevice(valve);
        }
    }
}
