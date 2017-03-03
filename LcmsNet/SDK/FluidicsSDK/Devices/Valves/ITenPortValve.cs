using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public interface ITenPortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;
        int GetPosition();
        void SetPosition(TwoPositionState s);
    }
}
