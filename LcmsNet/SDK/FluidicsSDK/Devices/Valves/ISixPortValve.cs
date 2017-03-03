using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public interface ISixPortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;
        int GetPosition();
        void SetPosition(TwoPositionState s);
    }
}
