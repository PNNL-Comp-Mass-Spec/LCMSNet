﻿using System;
using System.IO.Ports;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICIMultiPosViewModel),
        "4-Position",
        "Valves Multi-Position")
    ]
    public class ValveVICI04Position : ValveVICIMultiPos
    {
        private const int numPositions = 4;
        public ValveVICI04Position()
            : base(numPositions)
        {
        }

        public ValveVICI04Position(SerialPort port)
            : base(numPositions, port)
        {
        }

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(FourPositionState position)
        {
            var err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(FourPositionState);
        }
    }
}