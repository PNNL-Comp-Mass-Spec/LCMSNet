using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;

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
            m_supportedInterfaces = new Dictionary<Type, Type>();
            m_supportedInterfaces.Add(typeof(IFourPortValve), typeof(FourPortFluidicsValve));
            m_supportedInterfaces.Add(typeof(IFluidicsPump), typeof(FluidicsPump));
            m_supportedInterfaces.Add(typeof(ISixPortValve), typeof(SixPortFluidicsValve));
            m_supportedInterfaces.Add(typeof(ITenPortValve), typeof(TenPortFluidicsValve));
            m_supportedInterfaces.Add(typeof(INinePortValve), typeof(NinePortFluidicsValve));
            m_supportedInterfaces.Add(typeof(IFluidicsSampler), typeof(FluidicsSampler));
            m_supportedInterfaces.Add(typeof(ISixPortInjectionValve), typeof(SixPortInjectionFluidicsValve));
            m_supportedInterfaces.Add(typeof(IFluidicsClosure), typeof(FluidicsDetector));
            m_supportedInterfaces.Add(typeof(ISolidPhaseExtractor), typeof(SolidPhaseExtractor));
            m_supportedInterfaces.Add(typeof(IElevenPortValve), typeof(ElevenPortFluidicsValve));
            m_supportedInterfaces.Add(typeof(ISixteenPortValve), typeof(SixteenPortFluidicsValve));
            m_supportedInterfaces.Add(typeof(IContactClosure), typeof(ContactClosure));
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
            if (fluidicsDeviceType.BaseType.Equals(typeof(FluidicsDevice)))
            {
                throw new InvalidCustomFluidicsDeviceException("The supported type cannot be added. It is not a valid fluidics device.");
            }
            m_supportedInterfaces.Add(deviceInterfaceType, fluidicsDeviceType);
        }
        /// <summary>
        /// Creates a device from the device type checking to see if it supports standard fluidics device interfaces or valid custom interface types.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static FluidicsDevice CreateDevice(Type deviceType)
        {
            FluidicsDevice device  = null;
            var attributes         = deviceType.GetCustomAttributes(typeof(LcmsNetDataClasses.Devices.classDeviceControlAttribute), false);
            foreach (var o in attributes)
            {
                var monitorAttribute = o as classDeviceControlAttribute;
                if (monitorAttribute != null)
                {
                    var fluidicsDeviceType = monitorAttribute.FluidicsDeviceType;
                    /// Create a provided instance.
                    if (fluidicsDeviceType == null)
                    {
                        foreach(var interfaceType in m_supportedInterfaces.Keys)
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
                        if (!fluidicsDeviceType.BaseType.Equals(typeof(FluidicsDevice)))
                        {
                            throw new InvalidCustomFluidicsDeviceException(
                                string.Format("Could not create a fluidics device for {0}.", fluidicsDeviceType));
                        }                                                
                    }
                       
                    // 
                    if (fluidicsDeviceType != null)
                    {
                        device = Activator.CreateInstance(fluidicsDeviceType) as FluidicsDevice;
                    }
                }
            }

            return device;
        }
    }
}