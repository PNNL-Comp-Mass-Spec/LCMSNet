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
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Six-port SPE",
                                 "Valves Two-Position")
    ]
    class classValveVICISPE:classValveVICI2Pos, ISolidPhaseExtractor
    {
        public classValveVICISPE()
            : base()
        {
        }

        public classValveVICISPE(SerialPort port)
            : base(port)
        {
        }
    }
}
