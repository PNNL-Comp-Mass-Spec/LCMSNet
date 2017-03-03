using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public interface ISixPortInjectionValve : IFluidicsDevice
    {
        event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;
        event EventHandler InjectionVolumeChanged;
        int GetPosition();
        
        double InjectionVolume { get; set; }

        void SetPosition(TwoPositionState s);
    }
}
