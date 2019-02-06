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
        "16-Position",
        "Valves Multi-Position")
    ]
    public class ValveVICI16Position : ValveVICIMultiPos, ISixteenPositionValve
    {
        private const int numPositions = 16;
        public ValveVICI16Position()
            : base(numPositions)
        {
        }

        public ValveVICI16Position(SerialPort port)
            : base(numPositions, port)
        {
        }

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(SixteenPositionState position)
        {
            var err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(SixteenPositionState);
        }
    }
}
