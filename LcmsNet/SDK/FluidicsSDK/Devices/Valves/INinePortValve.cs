using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public interface INinePortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(EightPositionState s);
    }
}
