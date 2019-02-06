/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/

using System;
using System.IO.Ports;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICIMultiPosViewModel),
                                 "8-Position",
                                 "Valves Multi-Position")
    ]
    public class ValveVICI08Position:ValveVICIMultiPos, IEightPositionValve
    {
        private const int numPositions = 8;

        public ValveVICI08Position()
            : base(numPositions)
        {
        }

        public ValveVICI08Position(SerialPort port)
            : base(numPositions, port)
        {
        }

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(EightPositionState position)
        {
           var err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(EightPositionState);
        }
    }
}
