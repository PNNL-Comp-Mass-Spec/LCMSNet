using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using Zaber.Motion;

namespace LcmsNetPlugins.ZaberStage
{
    public class StageControl : INotifyPropertyChangedExt
    {
        private string stageDisplayName;
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
        private Zaber.Motion.Ascii.Device deviceRef = null;

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
                    DeviceModelID = value.DeviceId;
                    DeviceModelName = value.Name;
                    AttachedSerialNumber = value.SerialNumber;
                }

                if (value == null)
                {
                    AttachedSerialNumber = -1;
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

        public string GetAxisConfigString()
        {
            return $"DN={StageDisplayName}&;SN={SerialNumber}&;Inv={IsInverted}&;Init#={InitOrder}&;JogSpeeds={string.Join(",", jogSpeeds.Select(x => x.ToString(CultureInfo.InvariantCulture)))}";
        }

        public void ParseAxisConfigString(string config)
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
                }
            }
            catch
            {
                ApplicationLogger.LogError(LogLevel.Error, $"Failed to parse AxisConfig \"{config}\"");
            }
        }

        public double GetPositionMM()
        {
            return DeviceRef.GetAxis(1).GetPosition(Units.Length_Millimetres);
        }

        public bool MoveRelative(double distance, Units units, bool waitUntilIdle = true)
        {
            var dist = distance;
            if (IsInverted)
            {
                dist = -dist;
            }

            try
            {
                DeviceRef.GetAxis(1).MoveRelative(dist, units, waitUntilIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MoveRelative(MoveDirection direction, double distance, Units units, bool waitUntilIdle = true)
        {
            try
            {
                DeviceRef.GetAxis(1).MoveRelative(direction.Convert(distance, IsInverted), units, waitUntilIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StartMove(double velocity, Units units)
        {
            var vel = velocity;
            if (IsInverted)
            {
                vel = -vel;
            }

            try
            {
                DeviceRef.GetAxis(1).MoveVelocity(vel, units);
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
                DeviceRef.GetAxis(1).MoveVelocity(direction.Convert(speed, jogSpeeds, IsInverted), Units.Velocity_MillimetresPerSecond);
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
                DeviceRef.GetAxis(1).Stop(waitUntilIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MoveAbsolute(double position, Units units, bool waitUntilIdle = true, double velocity = 0, Units velocityUnits = Units.Native)
        {
            try
            {
                DeviceRef.GetAxis(1).MoveAbsolute(position, units, waitUntilIdle, velocity, velocityUnits);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WaitUntilIdle()
        {
            try
            {
                DeviceRef.GetAxis(1).WaitUntilIdle(true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Home()
        {
            DeviceRef.GetAxis(1).Home();
        }
    }

    public enum KnobMode
    {
        Velocity = 0,
        Displacement = 1
    }
}
