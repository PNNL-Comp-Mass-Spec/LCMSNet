using System;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    /// <summary>
    /// Event arguments for when a device changes.
    /// </summary>
    public class FluidicsDeviceChangeEventArgs: EventArgs
    {
        public FluidicsDeviceChangeEventArgs(FluidicsDevice device)
        {
            Device = device;
        }
        /// <summary>
        /// Gets the device that was added.
        /// </summary>
        public FluidicsDevice Device { get; private set; }
    }

    /// <summary>
    /// Event arguments for when a device changes.
    /// </summary>
    public class FluidicsDeviceChangeEventArgsWpf : EventArgs
    {
        public FluidicsDeviceChangeEventArgsWpf(FluidicsDeviceWpf device)
        {
            Device = device;
        }
        /// <summary>
        /// Gets the device that was added.
        /// </summary>
        public FluidicsDeviceWpf Device { get; private set; }
    }
}
