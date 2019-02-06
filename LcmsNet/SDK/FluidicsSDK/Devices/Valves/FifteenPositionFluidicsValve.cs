using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class FifteenPositionFluidicsValve : MultiPositionValve<FifteenPositionState, IFifteenPositionValve>
    {
        private const int NUMBER_OF_POSITIONS = 15;

        public FifteenPositionFluidicsValve() :
            base(NUMBER_OF_POSITIONS)
        {
            CurrentState = (int)FifteenPositionState.P1;
            base.ActivateState(_states[CurrentState]);
        }

        protected override int LowestPosition => (int)FifteenPositionState.P1;
        protected override int HighestPosition => (int)FifteenPositionState.P15;
        protected override int UnknownPosition => (int)FifteenPositionState.Unknown;
        protected override FifteenPositionState CurrentPosition => (FifteenPositionState)CurrentState;

        protected override FifteenPositionState GetPositionForState(int state)
        {
            return (FifteenPositionState)state;
        }

        protected override IFifteenPositionValve GetCastDevice(IDevice device)
        {
            return device as IFifteenPositionValve;
        }
    }
}
