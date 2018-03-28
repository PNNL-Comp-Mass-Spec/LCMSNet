/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/
using System;
using FluidicsSDK.Devices;
using System.IO.Ports;
using LcmsNetSDK.Devices;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Four-Port",
                                 "Valves Two-Position")
    ]
    public class classValveVICI2pos4port: classValveVICI2Pos, IFourPortValve
    {

        public classValveVICI2pos4port()
            : base()
        {
        }

        public classValveVICI2pos4port(SerialPort port)
            : base(port)
        {

        }
    }
}
