﻿using System;
using System.Collections.Generic;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices
{
    /// <summary>
    /// Creates Fluidics Devices based on Types
    /// </summary>
    public static class FluidicsDeviceTypeFactory
    {
        /// <summary>
        /// Map of internally support device types.
        /// </summary>
        private static readonly Dictionary<Type, Type> m_supportedInterfaces;

        /// <summary>
        /// Default constructor.
        /// </summary>
        static FluidicsDeviceTypeFactory()
        {
            m_supportedInterfaces = new Dictionary<Type, Type>
            {
                {typeof(IFourPortValve), typeof(FourPortFluidicsValve)},
                {typeof(IFluidicsPump), typeof(FluidicsPump)},
                {typeof(ISixPortValve), typeof(SixPortFluidicsValve)},
                {typeof(IEightPortValve), typeof(EightPortFluidicsValve)},
                {typeof(ITenPortValve), typeof(TenPortFluidicsValve)},
                {typeof(IFluidicsSampler), typeof(FluidicsSampler)},
                {typeof(ISixPortInjectionValve), typeof(SixPortInjectionFluidicsValve)},
                {typeof(IFluidicsClosure), typeof(FluidicsDetector)},
                {typeof(ISolidPhaseExtractor), typeof(SolidPhaseExtractor)},
                {typeof(IMultiPositionValve), typeof(MultiPositionValve)},
                {typeof(IContactClosure), typeof(ContactClosure)}
            };
        }

        /// <summary>
        /// Adds the device type to the fluidics device type.
        /// </summary>
        /// <param name="deviceInterfaceType">Device type that controls state</param>
        /// <param name="fluidicsDeviceType">Device type that is rendered by the system.</param>
        public static void AddInterface(Type deviceInterfaceType, Type fluidicsDeviceType)
        {
            //TODO: Write unit tests to see for two levels of inheritance
            // use *.issubclass(typeof(FluidicsDevice) instead??
            if (fluidicsDeviceType.BaseType == null)
                return;

            if (fluidicsDeviceType.BaseType == typeof(FluidicsDevice))
            {
                throw new InvalidCustomFluidicsDeviceException("The supported type cannot be added. It is not a valid fluidics device.");
            }
            m_supportedInterfaces.Add(deviceInterfaceType, fluidicsDeviceType);
        }

        /// <summary>
        /// Creates a device from the device type checking to see if it supports standard fluidics device interfaces or valid custom interface types.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static FluidicsDevice CreateDevice(Type deviceType, IDevice device = null)
        {
            FluidicsDevice fluidicsDevice = null;
            var attributes = deviceType.GetCustomAttributes(typeof(DeviceControlAttribute), false);
            foreach (var o in attributes)
            {
                if (o is DeviceControlAttribute monitorAttribute)
                {
                    var fluidicsDeviceType = monitorAttribute.FluidicsDeviceType;
                    // Create a provider instance.
                    if (fluidicsDeviceType == null)
                    {
                        foreach (var interfaceType in m_supportedInterfaces.Keys)
                        {
                            if (interfaceType.IsAssignableFrom(deviceType))
                            {
                                fluidicsDeviceType = m_supportedInterfaces[interfaceType];
                            }
                        }
                    }
                    else
                    {
                        // Only test to see if the device type is a fluidicsdevice.
                        if (fluidicsDeviceType.BaseType != null && !(fluidicsDeviceType.BaseType == typeof(FluidicsDevice)))
                        {
                            throw new InvalidCustomFluidicsDeviceException(
                                string.Format("Could not create a fluidics device for {0}.", fluidicsDeviceType));
                        }
                    }

                    if (fluidicsDeviceType == typeof(MultiPositionValve) && device is IMultiPositionValve mpv)
                    {
                        fluidicsDevice = new MultiPositionValve(mpv.NumberOfPositions, portNumberingClockwise: mpv.PortNumberingIsClockwise);
                    }
                    else if (fluidicsDeviceType != null)
                    {
                        fluidicsDevice = Activator.CreateInstance(fluidicsDeviceType) as FluidicsDevice;
                    }
                }
            }

            return fluidicsDevice;
        }
    }
}
