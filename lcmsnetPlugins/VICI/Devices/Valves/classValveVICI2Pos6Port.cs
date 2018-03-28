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

namespace LcmsNetPlugins.VICI.Devices.Valves
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
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
