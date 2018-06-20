﻿using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    public interface ISixteenPortValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(FifteenPositionState s);
    }
}