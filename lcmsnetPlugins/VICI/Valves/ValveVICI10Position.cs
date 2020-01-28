using System;
using System.IO.Ports;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICIMultiPosViewModel),
                                 "10-Position",
                                 "Valves Multi-Position")
    ]
    public class ValveVICI10Position : ValveVICIMultiPos
    {
        private const int numPositions = 10;

        public ValveVICI10Position()
            : base(numPositions)
        {
        }

        public ValveVICI10Position(SerialPort port)
            : base(numPositions, port)
        {
        }

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
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
