using System;

namespace LcmsNetSDK.Devices
{
    public class DeviceControlAttribute : Attribute
    {
        public DeviceControlAttribute(Type deviceControlType, string name, string category) :
            this(deviceControlType, null, name, category)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="deviceControlType"></param>
        /// <param name="fluidicsDeviceType"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        public DeviceControlAttribute(Type deviceControlType, Type fluidicsDeviceType, string name, string category)
        {
            ControlType = deviceControlType;
            Category = category;
            Name = name;
            FluidicsDeviceType = fluidicsDeviceType;
        }

        /// <summary>
        /// Gets or sets the category name of the device.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the device name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the fluidics device type to be constructed for custom fluidics devices.
        /// </summary>
        public Type FluidicsDeviceType { get; set; }

        /// <summary>
        /// Gets or sets the advanced user control for displaying from system dashboard or the Fluidics Device
        /// </summary>
        public Type ControlType { get; set; }
    }
}