using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public interface ISixteenPortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(FifteenPositionState s);
    }
}