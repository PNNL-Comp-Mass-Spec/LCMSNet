using System;

namespace FluidicsSDK.Managers
{
    public class ConnectionChangedEventArgs<T>:EventArgs
    {
        public T arg { get; set; }
    }
}
