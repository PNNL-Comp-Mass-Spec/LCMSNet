using System;

namespace FluidicsSDK.Devices.Valves
{
    public interface ISixPortInjectionValve : ITwoPositionValve
    {
        event EventHandler InjectionVolumeChanged;
        double InjectionVolume { get; set; }
    }
}
