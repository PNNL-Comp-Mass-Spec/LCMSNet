/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/
using System;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(controlValveVICIMultiPos),
                                 "Eleven-Port",
                                 "Valves Multi-Position")
    ]
    public class classValveVICIMultipos11Port : classValveVICIMultiPos, IElevenPortValve
    {
        private const int numPositions = 10;

        public classValveVICIMultipos11Port()
            : base(numPositions)
        {
        }

        public classValveVICIMultipos11Port(SerialPort port)
            : base(numPositions, port)
        {
        }


        [classLCMethodAttribute("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(TenPositionState position)
        {
            var err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(TenPositionState);
        }
    }
}
