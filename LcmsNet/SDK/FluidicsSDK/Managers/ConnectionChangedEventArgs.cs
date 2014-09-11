using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidicsSDK.Managers
{
    public class ConnectionChangedEventArgs<T>:EventArgs
    {
        public T arg { get; set; }
    }
}
