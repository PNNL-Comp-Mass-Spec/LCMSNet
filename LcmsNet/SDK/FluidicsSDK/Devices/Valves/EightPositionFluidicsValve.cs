using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    sealed class EightPositionFluidicsValve : MultiPositionValve<EightPositionState, IEightPositionValve>
    {
        private const int NUMBER_OF_POSITIONS = 8;

        public EightPositionFluidicsValve() :
            base(NUMBER_OF_POSITIONS)
        {
            CurrentState = (int)EightPositionState.P1;
            base.ActivateState(_states[CurrentState]);
        }

        protected override int LowestPosition => (int)EightPositionState.P1;
        protected override int HighestPosition => (int)EightPositionState.P8;
        protected override int UnknownPosition => (int)EightPositionState.Unknown;
        protected override EightPositionState CurrentPosition => (EightPositionState)CurrentState;

        protected override EightPositionState GetPositionForState(int state)
        {
            return (EightPositionState)state;
        }

        protected override IEightPositionValve GetCastDevice(IDevice device)
        {
            return device as IEightPositionValve;
        }
    }
}
