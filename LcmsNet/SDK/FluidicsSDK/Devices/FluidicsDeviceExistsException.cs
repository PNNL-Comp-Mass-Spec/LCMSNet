using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidicsSDK.Devices
{
    public class FluidicsDeviceExistsException: Exception
    {
        public FluidicsDeviceExistsException(string message)
            : base(message)
        {
        }
    }
}
