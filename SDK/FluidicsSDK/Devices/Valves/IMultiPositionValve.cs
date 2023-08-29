using System;

namespace FluidicsSDK.Devices.Valves
{
    public interface IMultiPositionValve : IFluidicsDevice
    {
        int NumberOfPositions { get; }
        bool PortNumberingIsClockwise { get; }
        event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        int GetPosition();
        void SetPosition(int s);
    }
}
