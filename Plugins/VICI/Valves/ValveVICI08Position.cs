using System;
using System.IO.Ports;
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
    public class ValveVICI08Position:ValveVICIMultiPos
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

        [LCMethodEvent("Set Position Timed", MethodOperationTimeoutType.Parameter, "", -1, false, HasDiscreteParameters = true, EventDescription = "Move the valve to the specified position, and then wait for the timeout before moving to the next event. Make sure the timeout is >=5 seconds.")]
        public void SetPositionTimed(double timeout, EightPositionState position)
        {
            // timeout parameter is not used internally, but is instead used by the scheduler and runner (and handled there).
            var err = base.SetPosition((int)position);
        }

        [LCMethodEvent("Step To Position", MethodOperationTimeoutType.CallMethod, "", -1, false, EventDescription = "Step the valve until it reaches the set position, with a set time in each position (includes move time)", IgnoreLeftoverTime = true, TimeoutCalculationMethod = nameof(CalculateStepToPositionTime))]
        public void StepToPosition(EightPositionState endPosition, int delayEachStepSeconds)
        {
            var err = base.StepToPosition((int)endPosition, delayEachStepSeconds);
        }

        public override Type GetStateType()
        {
            return typeof(EightPositionState);
        }
    }
}
