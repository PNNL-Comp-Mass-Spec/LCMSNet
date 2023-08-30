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

    public interface IFourPortValve : ITwoPositionValve
    { }

    public interface ISixPortValve : ITwoPositionValve
    { }

    public interface IEightPortValve : ITwoPositionValve
    { }

    public interface ITenPortValve : ITwoPositionValve
    { }

    public interface ISolidPhaseExtractor : ITwoPositionValve
    { }
}
