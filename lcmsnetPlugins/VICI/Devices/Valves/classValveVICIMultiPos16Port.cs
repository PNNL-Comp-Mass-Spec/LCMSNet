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
using FluidicsSDK.Devices;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControlAttribute(typeof(controlValveVICIMultiPos),
                                 "Sixteen-Port",
                                 "Valves Multi-Position")
    ]
    public class classValveVICIMultipos16Port : classValveVICIMultiPos, ISixteenPortValve
    {
        private const int numPositions = 15;
        public classValveVICIMultipos16Port()
            : base(numPositions)
        {
        }

        public classValveVICIMultipos16Port(SerialPort port)
            : base(numPositions, port)
        {
        }


        [classLCMethodAttribute("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(FifteenPositionState position)
        {
            var err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(FifteenPositionState);
        }
    }
}
