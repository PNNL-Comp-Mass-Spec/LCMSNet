using System;

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
