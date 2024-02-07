using System;
using System.Threading.Tasks;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage.Devices
{
    [Serializable]
    [DeviceControl(typeof(TwoColumnTipPositionerViewModel),
        "2 Column Tip Positioner",
        "Stages")]
    public class TwoColumnTipPositioner : ZaberStageBase<XYZAxis3Stage>
    {
        public TwoColumnTipPositioner() : base(new XYZAxis3Stage())
        {
        }

        private double yAxisMoveOffsetMM = 0;
        private Position position1;
        private Position position2;

        internal StageControl XAxis => StageDevice.XAxis;
        internal StageControl YAxis => StageDevice.YAxis;
        internal StageControl ZAxis => StageDevice.ZAxis;

        [DeviceSavedSetting("XAxisConfig")]
        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value);
        }

        [DeviceSavedSetting("YAxisConfig")]
        public string YAxisConfig
        {
            get => YAxis.GetAxisConfigString();
            set => YAxis.ParseAxisConfigString(value);
        }

        [DeviceSavedSetting("ZAxisConfig")]
        public string ZAxisConfig
        {
            get => ZAxis.GetAxisConfigString();
            set => ZAxis.ParseAxisConfigString(value);
        }

        [DeviceSavedSetting("YAxisMoveOffsetMM")]
        public double YAxisMoveOffsetMM
        {
            get => yAxisMoveOffsetMM;
            set => this.RaiseAndSetIfChanged(ref yAxisMoveOffsetMM, value);
        }

        [DeviceSavedSetting("Position1")]
        public string Position1Config
        {
            get => Position1.GetPositionEncoded();
            set => Position1 = new Position(value);
        }

        [DeviceSavedSetting("Position2")]
        public string Position2Config
        {
            get => Position2.GetPositionEncoded();
            set => Position2 = new Position(value);
        }

        public Position Position1
        {
            get => position1;
            set => this.RaiseAndSetIfChanged(ref position1, value);
        }

        public Position Position2
        {
            get => position2;
            set => this.RaiseAndSetIfChanged(ref position2, value);
        }

        [LCMethodEvent("Move to Position 1", MethodOperationTimeoutType.Parameter, EventDescription = "Move the stages to position 1")]
        [LCMethodEvent("Move to Position 1 NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Move the stages to position 1\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public bool MoveToPosition1(double timeout = 1)
        {
            return MoveToPosition(Position1);
        }

        [LCMethodEvent("Move to Position 2", MethodOperationTimeoutType.Parameter, EventDescription = "Move the stages to position 2")]
        [LCMethodEvent("Move to Position 2 NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Move the stages to position 2\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public bool MoveToPosition2(double timeout = 1)
        {
            return MoveToPosition(Position2);
        }

        private bool MoveToPosition(Position pos)
        {
            var yCurrent = YAxis.GetPositionMM();
            var yTarget = pos.Y;
            double yMovePosition;
            if (!YAxis.IsInverted)
            {
                var yMin = Math.Min(yCurrent, yTarget);
                yMovePosition = Math.Max(yMin + YAxisMoveOffsetMM, 0);
            }
            else
            {
                var yMax = Math.Max(yCurrent, yTarget);
                yMovePosition = Math.Max(yMax - YAxisMoveOffsetMM, 0);
            }

            var xCurrent = XAxis.GetPositionMM();
            var zCurrent = ZAxis.GetPositionMM();
            var xVelocity = 20d;
            var zVelocity = 20d;

            xVelocity = Math.Abs(pos.X - xCurrent);
            zVelocity = Math.Abs(pos.Z - zCurrent);

            var result = YAxis.MoveAbsolute(yMovePosition);
            if (!result) return false;

            var t1 = XAxis.MoveAbsoluteAsync(pos.X, velocity: xVelocity);
            var t2 = ZAxis.MoveAbsoluteAsync(pos.Z, velocity: zVelocity);

            Task.WaitAll(t1, t2);

            return YAxis.MoveAbsolute(pos.Y);
        }
    }
}
