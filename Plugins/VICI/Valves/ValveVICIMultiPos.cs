using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    /// <summary>
    /// Class used for interacting with the VICI multiposition valves
    /// </summary>
    //[Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    //[DeviceControl(typeof(ValveVICIMultiPosViewModel),
    //    "MultiPosition",
    //    "Valves Multi-Position")
    //]
    public class ValveVICIMultiPos : ValveVICIBase, IDevice, IMultiPositionValve
    {
        // TODO: Thought: Create a separate class for universal actuator valves
        // TODO: Reason: More extensive command set, ability to dynamically read and adjust the number of ports
        // Settings for EMTCA-CE (Multi-pos actuator):
        //     Baud Rate   9600
        //     Parity      None
        //     Stop Bits   One
        //     Data Bits   8
        //     Handshake   None

        #region Members

        private int lastMeasuredPosition;
        private int numberOfPositions;
        private int hardwarePositions;

        /// <summary>
        /// How long to tell LCMSNet the SetPosition method can take. it is 6  seconds instead of 4 because we verify that
        /// the the position has change, and that 1.5 seconds+ so 4 + 1.5 rounded up = 6.
        /// </summary>
        public const int LC_EVENT_SET_POSITION_TIME_SECONDS = 6;

        //Model EMTCA-CE can take up to 3150(1161+(999*2)ms to rotate if only 4 positions are set (and set to only rotate in one direction)
        //More positions reduces time it takes to rotate, but we can't know how many positions there are
        //Also, as LCEvents are timed in seconds, we round up to 4000ms to ensure that the
        //method isn't killed over 150ms + concurrency delays.
        // TODO: Universal actuator has "TM" command to notify how long the previous move took - should use it to calculate this delay time.
        private static readonly int RotationDelayTimeMsec = 4000;
        private static readonly int StepRotationDelayTimeMsec = 1170; // max rotation time for a single step for a 4-port valve.
        private const int CONST_DEFAULT_TIMEOUT = 1500;

        #endregion

        #region Events

        //Position change
        public event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        protected virtual void OnPosChanged(int position)
        {
            Task.Run(() => PosChanged?.Invoke(this, new ValvePositionEventArgs<int>(position)));
        }

        public virtual Type GetStateType()
        {
            return typeof(FifteenPositionState);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICIMultiPos(int numPositions) : base(CONST_DEFAULT_TIMEOUT, CONST_DEFAULT_TIMEOUT, "MPValve" + numPositions)
        {
            //Set positions to unknown
            LastMeasuredPosition   = -1;
            LastSentPosition       = -1;

            NumberOfPositions      = numPositions;
            HardwarePositions = numPositions;
        }

        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="numPositions"></param>
        /// <param name="port">The serial port object to use.</param>
        public ValveVICIMultiPos(int numPositions, SerialPort port) : base(port, "MPValve" + numPositions)
        {
            //Set positions to unknown
            LastMeasuredPosition   = -1;
            LastSentPosition       = -1;

            NumberOfPositions  = numPositions;
            HardwarePositions = numPositions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        public int LastMeasuredPosition
        {
            get => lastMeasuredPosition;
            private set => this.RaiseAndSetIfChanged(ref lastMeasuredPosition, value);
        }

        /// <summary>
        /// The last position sent to the valve.
        /// </summary>
        public int LastSentPosition { get; private set; }

        /// <summary>
        ///
        /// </summary>
        [PersistenceData("NumberOfPositions")]
        public int NumberOfPositions
        {
            get => numberOfPositions;
            set => this.RaiseAndSetIfChanged(ref numberOfPositions, value);
        }

        /// <summary>
        /// Number of positions reported by the hardware
        /// </summary>
        public int HardwarePositions
        {
            get => hardwarePositions;
            private set => this.RaiseAndSetIfChanged(ref hardwarePositions, value);
        }

        /// <summary>
        /// Display string for the Last Measured Position
        /// </summary>
        public override string LastMeasuredPositionDisplay =>
            LastMeasuredPosition > 0 ? LastMeasuredPosition.ToString() : "Unknown";

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Initialize(ref string errorMessage)
        {
            if (!Emulation)
            {
                // Check version first
                try
                {
                    GetVersion();

                    if (!CheckIsMultiPosition())
                    {
                        ApplicationLogger.LogError(1, $"ERROR: Valve reports that it is not a multi-position valve, software configured for {NumberOfPositions} positions! {Name}, COM port {PortName}");
                    }
                }
                catch
                {
                    // Suppressing errors here
                }

                var numPositions = GetNumberOfPositions();
                if (numPositions > 1 && numPositions != NumberOfPositions)
                {
                    ApplicationLogger.LogError(1, $"ERROR: Valve reports {numPositions} positions, software configured for {NumberOfPositions} positions! {Name}, COM port {PortName}");
                }
            }

            var result = base.Initialize(ref errorMessage);

            if (Emulation)
            {
                //Fill in fake position
                LastMeasuredPosition = 0;
            }

            return result;
        }

        /// <summary>
        /// Explicit implementation of SetPosition(int) to wrap the version that returns error info.
        /// </summary>
        /// <param name="s">The new position</param>
        void IMultiPositionValve.SetPosition(int s)
        {
            SetPosition(s);
        }

        /// <summary>
        /// Sets the position of the valve.
        /// </summary>
        /// <param name="position">The new position.</param>
        public ValveErrors SetPosition(int position)
        {
            if (position == LastMeasuredPosition)
            {
                return ValveErrors.Success;
            }

            var newPosition = position;
            if (Emulation)
            {
                LastSentPosition = LastMeasuredPosition = newPosition;
                OnPosChanged(newPosition);
                return ValveErrors.Success;
            }

            // TODO: Set the rotationalDelayTimeMs based on the number of positions, and then wait positions changed + 1 for it to change?
            // TODO: Hard challenge is dealing with valves that are configured with unidirectional movement
            // TODO: Math for setting the delay for bidirectional movement: Math.Min(Math.Abs(oldPos - newPos), (oldPos + newPos) % numPositions) * rotationalDelayTimeMs
            // TODO: Read the rotation mode;
            // TODO: If Auto, use the above math; if not:
            // TODO: clockwise/Forward: ((newPos - oldPos + numPositions) % numPositions) * rotationalDelayTimeMs
            // TODO: counter-clockwise/Reverse: ((oldPos - newPos + numPositions) % numPositions) * rotationalDelayTimeMs

            if (newPosition > 0 && newPosition <= NumberOfPositions)
            {
                LastSentPosition = newPosition;
                var sendError = SetHardwarePosition(newPosition.ToString(), RotationDelayTimeMsec);
                if (sendError != ValveErrors.Success)
                {
                    return sendError;
                }

                if (LastMeasuredPosition != LastSentPosition)
                {
                    //ApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position.");
                    return ValveErrors.ValvePositionMismatch;
                }

                OnPosChanged(LastMeasuredPosition);
                //ApplicationLogger.LogMessage(0, Name + " changed position to: " + LastMeasuredPosition);
                return ValveErrors.Success;
            }

            return ValveErrors.BadArgument;
        }

        [LCMethodEvent("Step Full Cycles", MethodOperationTimeoutType.CallMethod, EventDescription = "Cycle the valve through cycleCount full rotations, with a set time in each position (includes move time)", IgnoreLeftoverTime = true, TimeoutCalculationMethod = nameof(CalculateFullCycleSteppedTime))]
        public ValveErrors FullCycleStepped(int cycleCount, int delayEachStepSeconds)
        {
            if (cycleCount == 0)
            {
                return ValveErrors.Success;
            }

            SendStatusMessage(StatusReportType.CycleCount, $"Starting {cycleCount} valve cycles, waiting {delayEachStepSeconds} in each position.");

            var endPosition = LastMeasuredPosition;

            var result = ValveErrors.BadArgument;
            for (var i = 0; i < cycleCount; i++)
            {
                SendStatusMessage(StatusReportType.CycleCount, $"Starting valve cycle {i + 1} of {cycleCount}, moving from position {endPosition}...");
                for (var j = 0; j < NumberOfPositions; j++)
                {
                    result = StepPosition(delayEachStepSeconds);
                    if (result != ValveErrors.Success)
                    {
                        break;
                    }
                }

                if (result != ValveErrors.Success)
                {
                    break;
                }
            }
            SendStatusMessage(StatusReportType.CycleCount, $"Completed {cycleCount} valve cycles.");

            if (LastMeasuredPosition != endPosition && result == ValveErrors.Success)
            {
                result = ValveErrors.ValvePositionMismatch;
            }

            return result;
        }

        public int CalculateFullCycleSteppedTime(int cycleCount, int delayEachStepSeconds)
        {
            var msDelay = Math.Max(delayEachStepSeconds * 1000, StepRotationDelayTimeMsec);
            // Tolerance: add in an extra step
            var msTotal = msDelay * NumberOfPositions * cycleCount + msDelay;
            return (int)Math.Ceiling(msTotal / 1000.0);
        }

        public ValveErrors StepToPosition(int endPosition, int delayEachStepSeconds)
        {
            // TODO: should this instead allow a full cycle?
            if (endPosition == LastMeasuredPosition)
            {
                return ValveErrors.Success;
            }

            var result = ValveErrors.BadArgument;
            while (LastMeasuredPosition != endPosition)
            {
                result = StepPosition(delayEachStepSeconds);
                if (result != ValveErrors.Success)
                {
                    break;
                }
            }

            return result;
        }

        public int CalculateStepToPositionTime(int endPosition, int delayEachStepSeconds)
        {
            var msDelay = Math.Max(delayEachStepSeconds * 1000, StepRotationDelayTimeMsec);
            // Tolerance: add in an extra step
            var msTotal = msDelay * (NumberOfPositions + 1);
            return (int)Math.Ceiling(msTotal / 1000.0);
        }

        [LCMethodEvent("Step N Positions", MethodOperationTimeoutType.CallMethod, EventDescription = "Step the valve the specified number of times, with a set time in each position (includes move time)", IgnoreLeftoverTime = true, TimeoutCalculationMethod = nameof(CalculateStepNPositionsTime))]
        public ValveErrors StepNPositions(int stepCount, int delayEachStepSeconds)
        {
            if (stepCount <= 0)
            {
                return ValveErrors.Success;
            }

            SendStatusMessage(StatusReportType.CycleCount, $"Stepping {stepCount} valve positions, waiting {delayEachStepSeconds} in each position.");
            var result = ValveErrors.BadArgument;
            for (var i = 0; i < stepCount; i++)
            {
                result = StepPosition(delayEachStepSeconds);
                if (result != ValveErrors.Success)
                {
                    break;
                }
            }

            return result;
        }

        public int CalculateStepNPositionsTime(int stepCount, int delayEachStepSeconds)
        {
            var msDelay = Math.Max(delayEachStepSeconds * 1000, StepRotationDelayTimeMsec);
            // Tolerance: add in an extra step
            var msTotal = msDelay * (stepCount + 1);
            return (int) Math.Ceiling(msTotal / 1000.0);
        }

        private ValveErrors StepPosition(int delaySeconds)
        {
            var newPosition = LastMeasuredPosition + 1;
            if (newPosition > NumberOfPositions)
            {
                newPosition = 1;
            }

            if (Emulation)
            {
                LastSentPosition = LastMeasuredPosition = newPosition;
                OnPosChanged(newPosition);
                Thread.Sleep(delaySeconds * 1000);
                return ValveErrors.Success;
            }

            // TODO: Set the rotationalDelayTimeMs based on the number of positions, and then wait positions changed + 1 for it to change?
            // TODO: Hard challenge is dealing with valves that are configured with unidirectional movement
            // TODO: Math for setting the delay for bidirectional movement: Math.Min(Math.Abs(oldPos - newPos), (oldPos + newPos) % numPositions) * rotationalDelayTimeMs
            // TODO: Read the rotation mode;
            // TODO: If Auto, use the above math; if not:
            // TODO: clockwise/Forward: ((newPos - oldPos + numPositions) % numPositions) * rotationalDelayTimeMs
            // TODO: counter-clockwise/Reverse: ((oldPos - newPos + numPositions) % numPositions) * rotationalDelayTimeMs

            var sw = System.Diagnostics.Stopwatch.StartNew();

            LastSentPosition = newPosition;
            //var sendError = SetHardwarePosition(newPosition.ToString(), RotationDelayTimeMsec);
            // 'GO' by itself, on universal actuator, will change position (2-position) or advance to the next position (multiposition)
            // Also, using 'GO' it may allow the user to step the valve in reverse direction by setting the move direction via the SM command or manual remote menu
            var sendError = SetHardwarePosition("", StepRotationDelayTimeMsec);
            if (sendError != ValveErrors.Success)
            {
                return sendError;
            }

            if (LastMeasuredPosition != LastSentPosition)
            {
                //ApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position.");
                return ValveErrors.ValvePositionMismatch;
            }

            OnPosChanged(LastMeasuredPosition);

            sw.Stop();
            var remainingMs = delaySeconds * 1000 - (int)sw.ElapsedMilliseconds;
            if (remainingMs > 0)
            {
                System.Threading.Thread.Sleep(remainingMs);
            }

            //ApplicationLogger.LogMessage(0, Name + " changed position to: " + LastMeasuredPosition);
            return ValveErrors.Success;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as a display string.</returns>
        public override string GetPositionDisplay()
        {
            var pos = GetPosition();
            return pos > 0 ? pos.ToString() : "Unknown";
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an int.</returns>
        public override int GetPosition()
        {
            if (Emulation)
            {
                return LastSentPosition;
            }

            // Data returned from hardware should look like
            //  Position is = 1
            // Universal actuator:
            // \0CP01 (no hardware ID) or 5CP01 (hardware ID 5)
            // GetHardwarePosition should return "1" or "01"
            var tempPosition = GetHardwarePosition();

            // Grab the actual position from the above string
            if (!string.IsNullOrWhiteSpace(tempPosition))
            {
                if (int.TryParse(tempPosition, out var position))
                {
                    if (position >= 0 && position <= NumberOfPositions)
                    {
                        LastMeasuredPosition = position;
                        return position;
                    }
                }
            }

            LastMeasuredPosition = -1;
            return -1;
        }

        /// <summary>
        /// Sets the number of positions the device should use.
        /// </summary>
        /// <param name="numPositions"></param>
        /// <returns></returns>
        public ValveErrors SetNumberOfPositions(int numPositions)
        {
            if (Emulation)
            {
                NumberOfPositions = numPositions;
                return ValveErrors.Success;
            }

            // TODO: Test this; will there be a line-ending character problem?
            var sendError = SendCommand("NP" + numPositions);
            if (sendError != ValveErrors.Success)
            {
                switch (sendError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                }
            }

            //Wait 325ms for the command to go through
            System.Threading.Thread.Sleep(IDChangeDelayTimeMsec);

            return ValveErrors.Success;
        }
        /// <summary>
        /// Gets the number of positions from the device.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfPositions()
        {
            if (Emulation)
            {
                return NumberOfPositions;
            }

            var tempNumPositions = -1;
            // 'NP' command misbehaves when the port NewLine is just the default "\n" - for some reason it was setting the number of positions to 2 when just trying to get the number of positions.
            // Appending "\r" to the command resolves this.
            var readError = ReadCommand("NP\r", out var numPositions, 100);
            if (readError != ValveErrors.Success)
            {
                switch (readError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            //This should look something like
            //  NP = 5
            //  NP5 (for universal actuator
            //Grab the actual #positions from the above string
            if (numPositions.Length > 1)  //Make sure we have content in the string
            {
                //Find the first =
                var tempCharIndex = numPositions.IndexOf("=", StringComparison.Ordinal);
                if (tempCharIndex >= 0)  //Make sure we found a =
                {
                    // Take everything after the '=', trim any whitespace, and split off any extra lines
                    if (!int.TryParse(numPositions.Substring(tempCharIndex + 1).Trim().Split('\n')[0].Trim(), out tempNumPositions))
                    {
                        tempNumPositions = -1;
                    }
                }
                else
                {
                    // Universal actuator
                    var np = numPositions.IndexOf("NP", StringComparison.OrdinalIgnoreCase);
                    if (np >= 0 && !int.TryParse(numPositions.Substring(np + 2).Split('\n')[0].Trim(), out tempNumPositions))
                    {
                        tempNumPositions = -1;
                    }
                }
            }

            if (tempNumPositions > 1)
            {
                // TODO: I would like to do this, but other code need to be changed first so that this would have desired results.
                //NumberOfPositions = tempNumPositions;
                HardwarePositions = tempNumPositions;
            }

            return tempNumPositions;
        }

        public RotationMode GetRotationMode()
        {
            if (Emulation)
            {
                return RotationMode.A;
            }

            var readError = ReadCommand("SM", out var modeString);
            if (readError != ValveErrors.Success)
            {
                switch (readError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            // SM = A
            // SMA (universal actuator)

            modeString = modeString.Replace(" ", "").Replace("=", "");
            var pos = modeString.IndexOf("SM", StringComparison.OrdinalIgnoreCase);
            if (pos >= 0)
            {
                var modeS = modeString.Substring(pos + 2, 1);
                if (Enum.TryParse<RotationMode>(modeS, true, out var mode))
                {
                    return mode;
                }
            }

            return RotationMode.A;
        }

        public void SetRotationMode(RotationMode newRotationMode)
        {
            if (Emulation)
            {
                return;
            }

            var sendError = SendCommand("SM" + newRotationMode.ToString());
            if (sendError != ValveErrors.Success)
            {
                switch (sendError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                }
            }

            // TODO: Confirm mode change?
        }

        public bool CheckIsMultiPosition()
        {
            if (Emulation || !IsUniversalActuator)
            {
                return true;
            }

            var readError = ReadCommand("AM", out var modeString);
            if (readError != ValveErrors.Success)
            {
                switch (readError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            if (string.IsNullOrWhiteSpace(modeString))
            {
                return true;
            }

            // AM3 is desired; AM1 or AM2 are 2-position
            // The output is the same with legacy mode on the universal actuator.
            var pos = modeString.IndexOf("AM", StringComparison.OrdinalIgnoreCase);
            if (pos >= 0 && int.TryParse(modeString.Substring(pos + 2, 1), out var mode))
            {
                if (mode == 3)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Method Editor Enabled Methods
        #endregion

        /*
        /// <summary>
        /// Writes health information to the data file.
        /// </summary>
        /// <param name="writer"></param>
        public override FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            component.Status    = Status.ToString();
            component.Name      = Name;
            component.Type      = "Multi-Position Valve";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurementSentPosition = new FinchScalarSignal();
            measurementSentPosition.Name        = "Set Position";
            measurementSentPosition.Type        = FinchDataType.Integer;
            measurementSentPosition.Units       = "";
            measurementSentPosition.Value       = this.LastSentPosition.ToString();
            component.Signals.Add(measurementSentPosition);

            FinchScalarSignal measurementMeasuredPosition = new FinchScalarSignal();
            measurementMeasuredPosition.Name        = "Measured Position";
            measurementMeasuredPosition.Type        = FinchDataType.Integer;
            measurementMeasuredPosition.Units       = "";
            measurementMeasuredPosition.Value       = this.LastMeasuredPosition.ToString();
            component.Signals.Add(measurementMeasuredPosition);

            FinchScalarSignal port = new FinchScalarSignal();
            port.Name           = "Port";
            port.Type           =  FinchDataType.String;
            port.Units          = "";
            port.Value          = this.PortName.ToString();
            component.Signals.Add(port);

            return component;
        }*/
    }
}
