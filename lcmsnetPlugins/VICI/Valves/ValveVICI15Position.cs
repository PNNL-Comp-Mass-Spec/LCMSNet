﻿using System;
using System.IO.Ports;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICIMultiPosViewModel),
                                 "15-Position",
                                 "Valves Multi-Position")
    ]
    public class ValveVICI15Position : ValveVICIMultiPos
    {
        private const int numPositions = 15;
        public ValveVICI15Position()
            : base(numPositions)
        {
        }

        public ValveVICI15Position(SerialPort port)
            : base(numPositions, port)
        {
        }

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
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
