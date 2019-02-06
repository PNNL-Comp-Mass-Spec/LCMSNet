using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class SixteenPositionFluidicsValve : MultiPositionValve<SixteenPositionState, ISixteenPositionValve>
    {
        private const int NUMBER_OF_POSITIONS = 16;

        public SixteenPositionFluidicsValve() :
            base(NUMBER_OF_POSITIONS)
        {
            CurrentState = (int)SixteenPositionState.P1;
            base.ActivateState(_states[CurrentState]);
        }

        protected override int LowestPosition => (int) SixteenPositionState.P1;
        protected override int HighestPosition => (int) SixteenPositionState.P16;
        protected override int UnknownPosition => (int) SixteenPositionState.Unknown;
        protected override SixteenPositionState CurrentPosition => (SixteenPositionState) CurrentState;

        protected override SixteenPositionState GetPositionForState(int state)
        {
            return (SixteenPositionState) state;
        }

        protected override ISixteenPositionValve GetCastDevice(IDevice device)
        {
            return device as ISixteenPositionValve;
        }
    }
}
