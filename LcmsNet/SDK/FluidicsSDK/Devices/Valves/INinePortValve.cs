using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    public interface INinePortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(EightPositionState s);
    }
}
