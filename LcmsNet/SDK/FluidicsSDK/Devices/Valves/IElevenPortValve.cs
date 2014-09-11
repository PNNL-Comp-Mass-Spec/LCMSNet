using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    public interface IElevenPortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(TenPositionState s);
    }
}
