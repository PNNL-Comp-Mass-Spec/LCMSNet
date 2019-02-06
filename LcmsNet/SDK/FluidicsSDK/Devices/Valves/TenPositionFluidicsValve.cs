using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class TenPositionFluidicsValve : MultiPositionValve<TenPositionState, ITenPositionValve>
    {
        private const int NUMBER_OF_POSITIONS = 10;

        public TenPositionFluidicsValve() :
            base(NUMBER_OF_POSITIONS)
        {
            CurrentState = (int)TenPositionState.P1;
            base.ActivateState(_states[CurrentState]);
        }

        protected override int LowestPosition => (int)TenPositionState.P1;
        protected override int HighestPosition => (int)TenPositionState.P10;
        protected override int UnknownPosition => (int)TenPositionState.Unknown;
        protected override TenPositionState CurrentPosition => (TenPositionState)CurrentState;

        protected override TenPositionState GetPositionForState(int state)
        {
            return (TenPositionState)state;
        }

        protected override ITenPositionValve GetCastDevice(IDevice device)
        {
            return device as ITenPositionValve;
        }
    }
}
