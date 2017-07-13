/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/
using System;
using FluidicsSDK.Devices;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(ValveVICI2PosViewModel),
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
