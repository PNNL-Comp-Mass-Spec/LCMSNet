using System;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Event arguments for when a position is changed in a valve.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValvePositionEventArgs<T> : EventArgs
    {
        public ValvePositionEventArgs(T position)
        {
            Position = position;
        }
        public T Position { get; }
    }
}
