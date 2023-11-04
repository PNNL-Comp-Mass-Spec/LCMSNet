using System;
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

        [LCMethodEvent("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, HasDiscreteParameters = true)]
        public void SetPosition(FifteenPositionState position)
        {
            var err = base.SetPosition((int)position);
        }

        [LCMethodEvent("Set Position Timed", MethodOperationTimeoutType.Parameter, HasDiscreteParameters = true, EventDescription = "Move the valve to the specified position, and then wait for the timeout before moving to the next event. Make sure the timeout is >=5 seconds.")]
        public void SetPositionTimed(double timeout, FifteenPositionState position)
        {
            // timeout parameter is not used internally, but is instead used by the scheduler and runner (and handled there).
            var err = base.SetPosition((int)position);
        }

        [LCMethodEvent("Step To Position", MethodOperationTimeoutType.CallMethod, EventDescription = "Step the valve until it reaches the set position, with a set time in each position (includes move time)", IgnoreLeftoverTime = true, TimeoutCalculationMethod = nameof(CalculateStepToPositionTime))]
        public void StepToPosition(FifteenPositionState endPosition, int delayEachStepSeconds)
        {
            var err = base.StepToPosition((int)endPosition, delayEachStepSeconds);
        }

        public override Type GetStateType()
        {
            return typeof(FifteenPositionState);
        }
    }
}
