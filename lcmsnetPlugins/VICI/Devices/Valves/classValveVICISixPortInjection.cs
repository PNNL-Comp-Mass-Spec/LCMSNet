/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 * Last Modified 6/5/2014 By Christopher Walters
 *********************************************************************************************************/
using System;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Devices;
using System.IO.Ports;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(controlSixPortInjectionValve),
                                 "Six-port Injection",
                                 "Valves Two-Position")
    ]
    public class classValveVICISixPortInjection: classValveVICI2Pos, ISixPortInjectionValve
    {

        private double m_volume;

         public classValveVICISixPortInjection()
            : base()
        {
            m_volume = 0;
        }

        public classValveVICISixPortInjection(SerialPort port)
            : base(port)
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

        [classPersistenceAttribute("InjectionVolume")]
        public double InjectionVolume
        {
            get { return m_volume;  }
            set
            {
                m_volume = value;

                InjectionVolumeChanged?.Invoke(this, new EventArgs());
            }
        }   

        public event EventHandler InjectionVolumeChanged;
    }
}
