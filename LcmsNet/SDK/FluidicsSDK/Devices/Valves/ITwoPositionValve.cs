using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Interface to control a valve with 2 positions
    /// </summary>
    public interface ITwoPositionValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;
        int GetPosition();
        void SetPosition(TwoPositionState s);
    }
}
