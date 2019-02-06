using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class FourPositionFluidicsValve : MultiPositionValve<FourPositionState, IFourPositionValve>
    {
        private const int NUMBER_OF_POSITIONS = 4;

        public FourPositionFluidicsValve() :
            base(NUMBER_OF_POSITIONS)
        {
            CurrentState = (int)FourPositionState.P1;
            base.ActivateState(_states[CurrentState]);
        }

        protected override int LowestPosition => (int)FourPositionState.P1;
        protected override int HighestPosition => (int)FourPositionState.P4;
        protected override int UnknownPosition => (int)FourPositionState.Unknown;
        protected override FourPositionState CurrentPosition => (FourPositionState)CurrentState;

        protected override FourPositionState GetPositionForState(int state)
        {
            return (FourPositionState)state;
        }

        protected override IFourPositionValve GetCastDevice(IDevice device)
        {
            return device as IFourPositionValve;
        }
    }
}
