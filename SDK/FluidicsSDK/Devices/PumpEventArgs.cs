using System;

namespace FluidicsSDK.Devices
{
    public class PumpEventArgs<T>:EventArgs
    {
        public PumpEventArgs( T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}