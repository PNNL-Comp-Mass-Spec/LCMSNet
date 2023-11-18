using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using Zaber.Motion;

namespace LcmsNetPlugins.ZaberStage.Devices
{
    [Serializable]
    [DeviceControl(typeof(TwoColumnTipPositionerViewModel),
        "2 Column Tip Positioner",
        "Stages")]
    public class TwoColumnTipPositioner : StageBase
    {
        public TwoColumnTipPositioner() : base(new string[] { "X", "Y", "Z" }, "XYZ_Stage")
        {
        }

        private double yAxisMoveOffsetMM = 0;
        private Position position1;
        private Position position2;

        internal StageControl XAxis => StagesUsed[0];
        internal StageControl YAxis => StagesUsed[1];
        internal StageControl ZAxis => StagesUsed[2];

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

        // operation notes:
        // Option to set an axis for first/last retraction/position and retraction distance
        // Have a hard-defined list of 2 positions
        // set max movement speed

        // TODO:
        // Add methods to move to position 1 or 2

        public bool MoveToPosition1()
        {
            return MoveToPosition(Position1);
        }

        public bool MoveToPosition2()
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

            var result = YAxis.MoveAbsolute(yMovePosition, Units.Length_Millimetres);
            if (!result) return false;

            var t1 = XAxis.DeviceRef.GetAxis(1).MoveAbsoluteAsync(pos.X, Units.Length_Millimetres, velocity: xVelocity, velocityUnit: Units.Velocity_MillimetresPerSecond);
            var t2 = ZAxis.DeviceRef.GetAxis(1).MoveAbsoluteAsync(pos.Z, Units.Length_Millimetres, velocity: zVelocity, velocityUnit: Units.Velocity_MillimetresPerSecond);

            Task.WaitAll(t1, t2);

            return YAxis.MoveAbsolute(pos.Y, Units.Length_Millimetres);
        }
    }
}
