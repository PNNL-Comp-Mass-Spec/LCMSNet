using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Zaber.Motion;
using Zaber.Motion.Ascii;

namespace ZaberStageControl
{
    public class StageControl : INotifyPropertyChangedExtZaber
    {
        private string stageDisplayName = "";
        private long serialNumber;
        private int deviceModelID;
        private string deviceModelName;
        private int deviceAddress;
        private bool driverDisabled;
        private KnobMode knobMode;
        private int knobDisplacementDistance;
        private int resolution;
        private string firmwareVersion;
        private bool stageConnectionError;
        private bool isInverted;
        private int initOrder;
        private readonly double[] jogSpeeds = new double[] { 0.2, 1, 3 };
        private double maxMoveSpeed = 1000000; // default is no movement speed limit, beyond the hardware limits!
        private Zaber.Motion.Ascii.Device deviceRef = null;
        private int axisNumber = 1;
        private double maxMoveSpeedMMperSecondSetting = 100;

        // TODO: Stage display: have one tab that is details/configuration that is common for all stages; can be a ItemsControl or ListView

        public string DeviceStageName { get; }

        // TODO: If short enough, use axis.SetLabel()?
        public string StageDisplayName { get => stageDisplayName; set => this.RaiseAndSetIfChanged(ref stageDisplayName, value); }

        public long SerialNumber
        {
            get => serialNumber;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref serialNumber, value) && value != AttachedSerialNumber)
                {
                    DeviceModelID = 0;
                    DeviceModelName = "";
                    FirmwareVersion = "";
                }
            }
        }

        /*
         * settings to care about:
         * motion type:
         * range (mm):
         * controller (COM Port)
         * Type? (does this matter?)
         * "direction" (does this only affect the knob?): 'knob.dir'
         * Microstep (mm) 'deg':
         * resolution, usteps/step: 'resolution'
         * min position (mm) - settable: 'limit.min'
         * max position (mm) - settable: 'limit.max'
         * min speed (mm/s): '0'
         * max speed (mm/s): 'maxspeed'
         * min acceleration (mm/s/s): 'accel' or 'motion.accelonly', 'motion.decelonly'
         * max acceleration (mm/s/s)
         *
         */

        public int DeviceModelID { get => deviceModelID; set => this.RaiseAndSetIfChanged(ref deviceModelID, value); }
        public string DeviceModelName { get => deviceModelName; set => this.RaiseAndSetIfChanged(ref deviceModelName, value); }
        public int DeviceAddress { get => deviceAddress; set => this.RaiseAndSetIfChanged(ref deviceAddress, value); }
        public bool DriverDisabled { get => driverDisabled; set => this.RaiseAndSetIfChanged(ref driverDisabled, value); } // 'driver.enabled'
        public KnobMode KnobMode { get => knobMode; set => this.RaiseAndSetIfChanged(ref knobMode, value); } // 'knob.dir'
        public int KnobDisplacementDistance { get => knobDisplacementDistance; set => this.RaiseAndSetIfChanged(ref knobDisplacementDistance, value); } // 'knob.distance'
        public int Resolution { get => resolution; set => this.RaiseAndSetIfChanged(ref resolution, value); }
        public string FirmwareVersion { get => firmwareVersion; set => this.RaiseAndSetIfChanged(ref firmwareVersion, value); }
        public bool StageConnectionError { get => stageConnectionError; set => this.RaiseAndSetIfChanged(ref stageConnectionError, value); }
        public bool IsInverted { get => isInverted; set => this.RaiseAndSetIfChanged(ref isInverted, value); }
        public int InitOrder { get => initOrder; set => this.RaiseAndSetIfChanged(ref initOrder, value); }
        public double JogSpeedLow { get => jogSpeeds[0]; set => this.RaiseAndSetIfChanged(ref jogSpeeds[0], value); }
        public double JogSpeedMedium { get => jogSpeeds[1]; set => this.RaiseAndSetIfChanged(ref jogSpeeds[1], value); }
        public double JogSpeedHigh { get => jogSpeeds[2]; set => this.RaiseAndSetIfChanged(ref jogSpeeds[2], value); }

        /// <summary>
        /// Maximum movement speed (when provided to commands), in mm/s
        /// </summary>
        public double MaxMoveSpeed { get => maxMoveSpeed; set => this.RaiseAndSetIfChanged(ref maxMoveSpeed, value); }

        public double MaxPositionMM { get; set; }

        public Zaber.Motion.Ascii.Device DeviceRef
        {
            get => deviceRef;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref deviceRef, value) && value != null)
                {
                    // TODO: Set local values...
                    FirmwareVersion = $"{value.FirmwareVersion.Major}.{value.FirmwareVersion.Minor}";
                    DeviceAddress = value.DeviceAddress;

                    UpdateDeviceInfo();
                }

                if (value == null)
                {
                    AttachedSerialNumber = -1;
                }
            }
        }

        public int AxisNumber
        {
            get => axisNumber;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref axisNumber, value))
                {
                    UpdateDeviceInfo();
                }
            }
        }

        public long AttachedSerialNumber { get; private set; } = -1;

        public StageControl(string deviceStageName)
        {
            DeviceStageName = deviceStageName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateDeviceInfo()
        {
            if (DeviceRef == null)
            {
                return;
            }

            AxisSettings settings;
            if (DeviceRef.IsIntegrated || AxisNumber <= 0 || AxisNumber > DeviceRef.AxisCount)
            {
                DeviceModelID = DeviceRef.DeviceId;
                DeviceModelName = DeviceRef.Name;
                AttachedSerialNumber = DeviceRef.SerialNumber;
                settings = DeviceRef.GetAxis(1).Settings;
            }
            else
            {
                var axis = DeviceRef.GetAxis(AxisNumber);
                DeviceModelID = axis.PeripheralId;
                DeviceModelName = axis.PeripheralName;
                AttachedSerialNumber = axis.PeripheralSerialNumber;
                settings = axis.Settings;
            }

            MaxPositionMM = settings.Get("limit.max", Units.Length_Millimetres);
            Resolution = (int)settings.Get("resolution", Units.Native);
            maxMoveSpeedMMperSecondSetting = settings.Get("maxspeed", Units.Velocity_MillimetresPerSecond);
        }

        //public double GetMaxMoveSpeedMMperS()
        //{
        //    return DeviceRef.GetAxis(AxisNumber).Settings.Get("maxspeed", Units.Velocity_MillimetresPerSecond);
        //    // Set: DeviceRef.GetAxis(AxisNumber).Settings.Set("maxspeed", [speed], Units.Velocity_MillimetresPerSecond);
        //}

        private double MaxMoveSpeedActual => Math.Min(maxMoveSpeedMMperSecondSetting, MaxMoveSpeed);

        public string GetAxisConfigString()
        {
            return $"DN={StageDisplayName}&;SN={SerialNumber}&;Inv={IsInverted}&;Init#={InitOrder}&;JogSpeeds={string.Join(",", jogSpeeds.Select(x => x.ToString(CultureInfo.InvariantCulture)))}&;MaxSpeed={MaxMoveSpeed}";
        }

        public void ParseAxisConfigString(string config, Action<string, Exception> errorAction = null)
        {
            var parts = config.Trim().Split(new string[] { "&;" }, StringSplitOptions.None);
            try
            {
                foreach (var part in parts)
                {
                    if (part.StartsWith("DN=", StringComparison.OrdinalIgnoreCase))
                    {
                        StageDisplayName = part.Substring(3);
                    }

                    if (part.StartsWith("SN=", StringComparison.OrdinalIgnoreCase) &&
                        long.TryParse(part.Substring(3), out var sn))
                    {
                        SerialNumber = sn;
                    }

                    if (part.StartsWith("Inv=", StringComparison.OrdinalIgnoreCase) &&
                        bool.TryParse(part.Substring(4), out var inverted))
                    {
                        IsInverted = inverted;
                    }

                    if (part.StartsWith("Init#=", StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(part.Substring(6), out var priority))
                    {
                        InitOrder = priority;
                    }

                    if (part.StartsWith("JogSpeeds=", StringComparison.OrdinalIgnoreCase))
                    {
                        var split = part.Substring(10).Split(',');
                        for (var i = 0; i < jogSpeeds.Length && i < split.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(split[i]) && int.TryParse(split[i], out var speed))
                            {
                                jogSpeeds[i] = speed;
                            }
                        }

                        OnPropertyChanged(nameof(JogSpeedLow));
                        OnPropertyChanged(nameof(JogSpeedMedium));
                        OnPropertyChanged(nameof(JogSpeedHigh));
                    }

                    if (part.StartsWith("MaxSpeed=", StringComparison.OrdinalIgnoreCase) &&
                        double.TryParse(part.Substring(9), out var maxSpeed))
                    {
                        MaxMoveSpeed = maxSpeed;
                    }
                }
            }
            catch (Exception ex)
            {
                errorAction.Invoke($"Failed to parse AxisConfig \"{config}\"", ex);
            }
        }

        /// <summary>
        /// Handle inversion and limits for an absolute move (as needed)
        /// </summary>
        /// <returns></returns>
        public double FixPosition(double targetPosition)
        {
            // invert position if needed
            var inverted = IsInverted ? MaxPositionMM - targetPosition : targetPosition;

            // ensure position is below the max position
            var belowMax = Math.Min(MaxPositionMM, inverted);

            // ensure position is above the min position (0)
            var aboveMin = Math.Max(belowMax, 0);

            return aboveMin;
        }

        /// <summary>
        /// Handle limits for a relative move (as needed)
        /// </summary>
        /// <returns></returns>
        public double FixPositionRelative(double relativeMove)
        {
            // invert current position if needed
            var currentPosition = GetPositionMMFixed();

            var maxPosMove = MaxPositionMM - currentPosition;
            var maxNegMove = currentPosition;

            var correctedPos = Math.Min(relativeMove, maxPosMove);
            var correctedNeg = Math.Max(correctedPos, maxNegMove);

            return correctedNeg;
        }

        /// <summary>
        /// Gets the current position of the stage (never inverted)
        /// </summary>
        /// <returns></returns>
        public double GetPositionMM()
        {
            return DeviceRef.GetAxis(AxisNumber).GetPosition(Units.Length_Millimetres);
        }

        /// <summary>
        /// Gets the current position of the stage (inverted if stage is inverted)
        /// </summary>
        /// <returns></returns>
        public double GetPositionMMFixed()
        {
            var pos = GetPositionMM();
            return IsInverted ? MaxPositionMM - pos : pos;
        }

        /// <summary>
        /// Move the stage axis relative to the current position
        /// </summary>
        /// <param name="distance">distance in mm</param>
        /// <param name="waitUntilIdle"></param>
        /// <param name="velocity">velocity in mm/s; '0' for device-programmed 'max speed'</param>
        /// <returns></returns>
        public bool MoveRelative(double distance, bool waitUntilIdle = true, double velocity = 0)
        {
            return MoveRelative2(distance, Units.Length_Millimetres, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), Units.Velocity_MillimetresPerSecond);
        }

        public bool MoveRelative2(double distance, Units units, bool waitUntilIdle = true, double velocity = 0, Units velocityUnits = Units.Native)
        {
            var dist = distance;
            if (IsInverted)
            {
                dist = -dist;
            }

            try
            {
                DeviceRef.GetAxis(AxisNumber).MoveRelative(dist, units, waitUntilIdle, velocity, velocityUnits);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move the stage axis relative to the current position
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance">distance in mm</param>
        /// <param name="waitUntilIdle"></param>
        /// <param name="velocity">velocity in mm/s; '0' for device-programmed 'max speed'</param>
        /// <returns></returns>
        public bool MoveRelative(MoveDirection direction, double distance, bool waitUntilIdle = true, double velocity = 0)
        {
            return MoveRelative2(direction, distance, Units.Length_Millimetres, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), Units.Velocity_MillimetresPerSecond);
        }

        public bool MoveRelative2(MoveDirection direction, double distance, Units units, bool waitUntilIdle = true, double velocity = 0, Units velocityUnits = Units.Native)
        {
            try
            {
                DeviceRef.GetAxis(AxisNumber).MoveRelative(direction.Convert(distance, IsInverted), units, waitUntilIdle, velocity, velocityUnits);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Start moving the axis (jogging)
        /// </summary>
        /// <param name="velocity">velocity in mm/s</param>
        /// <returns></returns>
        public bool StartMove(double velocity)
        {
            return StartMove2(velocity, Units.Length_Millimetres);
        }

        public bool StartMove2(double velocity, Units units)
        {
            var vel = velocity;
            if (IsInverted)
            {
                vel = -vel;
            }

            try
            {
                DeviceRef.GetAxis(AxisNumber).MoveVelocity(vel, units);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StartJog(MoveDirection direction, JogSpeed speed)
        {
            try
            {
                DeviceRef.GetAxis(AxisNumber).MoveVelocity(Math.Min(direction.Convert(speed, jogSpeeds, IsInverted), MaxMoveSpeedActual), Units.Velocity_MillimetresPerSecond);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop(bool waitUntilIdle = true)
        {
            try
            {
                DeviceRef.GetAxis(AxisNumber).Stop(waitUntilIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move the stage axis to the specified position
        /// </summary>
        /// <param name="position">position in mm</param>
        /// <param name="waitUntilIdle"></param>
        /// <param name="velocity">velocity in mm/s; '0' for device-programmed 'max speed'</param>
        /// <returns></returns>
        public bool MoveAbsolute(double position, bool waitUntilIdle = true, double velocity = 0)
        {
            return MoveAbsolute2(position, Units.Length_Millimetres, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), Units.Velocity_MillimetresPerSecond);
        }

        public bool MoveAbsolute2(double position, Units units, bool waitUntilIdle = true, double velocity = 0, Units velocityUnits = Units.Native)
        {
            try
            {
                DeviceRef.GetAxis(AxisNumber).MoveAbsolute(position, units, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), velocityUnits);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move the stage axis to the specified position
        /// </summary>
        /// <param name="position">position in mm</param>
        /// <param name="waitUntilIdle"></param>
        /// <param name="velocity">velocity in mm/s; '0' for device-programmed 'max speed'</param>
        /// <returns></returns>
        public Task MoveAbsoluteAsync(double position, bool waitUntilIdle = true, double velocity = 0)
        {
            return MoveAbsoluteAsync2(position, Units.Length_Millimetres, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), Units.Velocity_MillimetresPerSecond);
        }

        public Task MoveAbsoluteAsync2(double position, Units units, bool waitUntilIdle = true, double velocity = 0, Units velocityUnits = Units.Native)
        {
            return DeviceRef.GetAxis(AxisNumber).MoveAbsoluteAsync(position, units, waitUntilIdle, Math.Min(velocity, MaxMoveSpeedActual), velocityUnits);
        }

        public bool WaitUntilIdle()
        {
            try
            {
                DeviceRef.GetAxis(AxisNumber).WaitUntilIdle(true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task WaitUntilIdleAsync()
        {
            return DeviceRef.GetAxis(AxisNumber).WaitUntilIdleAsync(true);
        }

        public void Home()
        {
            DeviceRef.GetAxis(AxisNumber).Home();
        }
    }

    public enum KnobMode
    {
        Velocity = 0,
        Displacement = 1
    }
}
