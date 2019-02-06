using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    public interface IMultiPositionValve<in T> : IFluidicsDevice where T : Enum
    {
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(T s);
    }

    public interface IFourPositionValve : IMultiPositionValve<FourPositionState>
    { }

    public interface ISixteenPositionValve : IMultiPositionValve<SixteenPositionState>
    { }

    public interface ITenPositionValve : IMultiPositionValve<TenPositionState>
    { }

    public interface IEightPositionValve : IMultiPositionValve<EightPositionState>
    { }

    public interface IFifteenPositionValve : IMultiPositionValve<FifteenPositionState>
    { }
}
