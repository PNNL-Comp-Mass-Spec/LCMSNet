using System;
using FluidicsSDK.Devices.Valves;

namespace FluidicsSDK.Devices
{
    public interface ISixPortInjectionValve : ITwoPositionValve
    {
        event EventHandler InjectionVolumeChanged;
        double InjectionVolume { get; set; }
    }
}
