/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/

using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(SixPortInjectionValveViewModel), "Six-port Injection", "Valves Two-Position")]
    public class ValveVICISixPortInjection: ValveVICI2Pos, ISixPortInjectionValve
    {
        private double m_volume;

         public ValveVICISixPortInjection() : base()
        {
            m_volume = 0;
        }

        public ValveVICISixPortInjection(SerialPort port) : base(port)
        {
            m_volume = 0;
        }

        public double GetVolume()
        {
            return m_volume;
        }

        public void SetVolume(double volume)
        {
            InjectionVolume = volume;
        }

        [PersistenceData("InjectionVolume")]
        public double InjectionVolume
        {
            get => m_volume;
            set
            {
                m_volume = value;

                InjectionVolumeChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler InjectionVolumeChanged;
    }
}
