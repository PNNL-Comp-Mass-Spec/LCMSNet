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
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Six-Port",
                                 "Valves Two-Position")
    ]
    public class ValveVICI2Pos06Port:ValveVICI2Pos, ISixPortValve
    {
        public ValveVICI2Pos06Port()
            : base()
        {
        }

        public ValveVICI2Pos06Port(SerialPort port)
            : base(port)
        {
        }
    }
}
