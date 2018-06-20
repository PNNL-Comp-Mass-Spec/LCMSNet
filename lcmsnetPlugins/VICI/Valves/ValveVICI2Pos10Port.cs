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
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Ten-Port",
                                 "Valves Two-Position")
    ]
    public class ValveVICI2Pos10port : ValveVICI2Pos, ITenPortValve
    {
        public ValveVICI2Pos10port()
            : base()
        {
        }

        public ValveVICI2Pos10port(SerialPort port)
            : base(port)
        {
        }
    }
}
