/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 * Last Modified 6/5/2014 By Christopher Walters
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Devices;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(controlValveVICI2Pos),
                                 "Six-Port",
                                 "Valves Two-Position")
    ]
    public class classValveVICI2Pos6Port:classValveVICI2Pos, ISixPortValve
    {
        public classValveVICI2Pos6Port()
            : base()
        {
        }

        public classValveVICI2Pos6Port(SerialPort port)
            : base(port)
        { 
        }
    }
}
