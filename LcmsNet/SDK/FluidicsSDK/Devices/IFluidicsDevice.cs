using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidicsSDK.Devices
{
    public interface IFluidicsDevice
    {
        /// <summary>
        /// Fired when the device data has changed and requires rendering.
        /// </summary>
        event EventHandler DeviceSaveRequired;
    }
}
