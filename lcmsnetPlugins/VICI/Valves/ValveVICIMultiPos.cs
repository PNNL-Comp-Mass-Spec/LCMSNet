using System;
using System.IO.Ports;
using System.Threading.Tasks;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

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
        // Settings for EMTCA-CE (Multi-pos actuator):
        //     Baud Rate   9600
        //     Parity      None
        //     Stop Bits   One
        //     Data Bits   8
        //     Handshake   None

        #region Members

        /// <summary>
        /// How long to tell LCMSNet the SetPosition method can take. it is 6  seconds instead of 4 because we verify that
        /// the the position has change, and that 1.5 seconds+ so 4 + 1.5 rounded up = 6.
        /// </summary>
        public const int LC_EVENT_SET_POSITION_TIME_SECONDS = 6;

        //Model EMTCA-CE can take up to 3150(1161+(999*2)ms to rotate if only 4 positions are set
        //More positions reduces time it takes to rotate, but we can't know how many positions there are
        //Also, as LCEvents are timed in seconds, we round up to 4000ms to ensure that the
        //method isn't killed over 150ms + concurrency delays.
        private static readonly int m_rotationDelayTimems = 4000;
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
        }

        #endregion

        #region Properties

        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        public int LastMeasuredPosition { get; private set; }

        /// <summary>
        /// The last position sent to the valve.
        /// </summary>
        public int LastSentPosition { get; private set; }

        /// <summary>
        ///
        /// </summary>
        [PersistenceData("NumberOfPositions")]
        public int NumberOfPositions { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Initialize(ref string errorMessage)
        {
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
            var newPosition = Convert.ToInt32(position);
            if (Emulation)
            {
                LastSentPosition = LastMeasuredPosition = newPosition;
                OnPosChanged(newPosition);
                return ValveErrors.Success;
            }
            if(position == LastMeasuredPosition)
            {
                return ValveErrors.Success;
            }
            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    return ValveErrors.UnauthorizedAccess;
                }
            }

            if (newPosition > 0 && newPosition <= NumberOfPositions)
            {
                try
                {
                    LastSentPosition = newPosition;
                    Port.WriteLine(SoftwareID + "GO" + newPosition);
                }

                catch (TimeoutException)
                {
                    //ApplicationLogger.LogError(0, "Could not set position.  Write timeout.");
                    return ValveErrors.TimeoutDuringWrite;
                }
                catch (UnauthorizedAccessException)
                {
                    //ApplicationLogger.LogError(0, "Could not set position.  Could not access serial port.");
                    return ValveErrors.UnauthorizedAccess;
                }

                //Wait m_rotationDelayTimems for valve to actually switch before proceeding
                //TODO: BLL test this instead using the abort event.
                if (AbortEvent == null)
                    AbortEvent = new System.Threading.ManualResetEvent(false);

                var waited = System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { AbortEvent }, m_rotationDelayTimems);
                if (waited)
                    return ValveErrors.BadArgument;

                //System.Threading.Thread.Sleep(m_rotationDelayTimems);

                //Doublecheck that the position change was correctly executed
                try
                {
                    GetPosition();
                }
                catch (ValveExceptionWriteTimeout)
                {
                    //ApplicationLogger.LogError(0, "Could not set position.  The write operation timed out to device.");
                    return ValveErrors.TimeoutDuringWrite;
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    //ApplicationLogger.LogError(0, "Could not set position. Could not access port.");
                    return ValveErrors.UnauthorizedAccess;
                }

                if (LastMeasuredPosition != LastSentPosition)
                {
                    //ApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position.");
                    return ValveErrors.ValvePositionMismatch;
                }

                OnPosChanged(LastMeasuredPosition);
                //ApplicationLogger.LogMessage(0, Name + " changed position to: " + m_lastMeasuredPosition);
                return ValveErrors.Success;
            }

            return ValveErrors.BadArgument;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public override int GetPosition()
        {
            if (Emulation)
            {
                return LastSentPosition;
            }

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                Port.DiscardInBuffer();
                Port.WriteLine(SoftwareID + "CP");
                System.Threading.Thread.Sleep(200);
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //Read in whatever is waiting in the buffer
            //This should look like
            //  Position is "B"
            // Universal actuator:
            // \0CP01 (no hardware ID) or 5CP01 (hardware ID 5)
            string tempBuffer;
            try
            {
                tempBuffer = Port.ReadExisting();

                var cpPos = tempBuffer.IndexOf("CP", StringComparison.OrdinalIgnoreCase);
                if (cpPos >= 0 && cpPos < 2)
                {
                    // Universal actuator
                    tempBuffer = "CP=" + tempBuffer.Substring(cpPos + 2).Trim('\r', '\n');
                }
                else
                {
                    var contains = tempBuffer.Contains("Position is =");

                    var data = tempBuffer.Split(new[] {"\r"}, StringSplitOptions.RemoveEmptyEntries);
                    tempBuffer = "";
                    for (var i = data.Length - 1; i >= 0; i--)
                    {
                        var x = data[i];
                        x = x.Replace(" ", "").ToLower();
                        if (x.Contains("positionis="))
                        {
                            tempBuffer = data[i];
                            break;
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionReadTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //
            // Grab the actual position from the above string
            //
            if (tempBuffer.Length > 1)
            {
                var positions = tempBuffer.Split('=');
                var tempPosition = positions[positions.Length - 1];

                int position;
                if (int.TryParse(tempPosition, out position))
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

            try
            {
                Port.WriteLine(SoftwareID + "NP" + numPositions);
                NumberOfPositions = numPositions;

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(IDChangeDelayTimems);
            }
            catch (TimeoutException)
            {
                return ValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return ValveErrors.UnauthorizedAccess;
            }

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

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                Port.WriteLine(SoftwareID + "NP");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            string tempBuffer;

            try
            {
                tempBuffer = Port.ReadLine();
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionReadTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //This should look something like
            //  NP = 5
            //Grab the actual #positions from the above string
            if (tempBuffer.Length > 1)  //Make sure we have content in the string
            {
                //Find the first =
                var tempCharIndex = tempBuffer.IndexOf("=", StringComparison.Ordinal);
                if (tempCharIndex >= 0)  //Make sure we found a =
                {
                    //Change the position to be the second character following the first =
                    //TODO: Do we ever have more than 9 positions? Do we need 2 digits?
                    tempNumPositions = Convert.ToInt32(tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0]);
                }
            }

            NumberOfPositions = tempNumPositions;
            return tempNumPositions;
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
            measurementSentPosition.Value       = this.m_lastSentPosition.ToString();
            component.Signals.Add(measurementSentPosition);

            FinchScalarSignal measurementMeasuredPosition = new FinchScalarSignal();
            measurementMeasuredPosition.Name        = "Measured Position";
            measurementMeasuredPosition.Type        = FinchDataType.Integer;
            measurementMeasuredPosition.Units       = "";
            measurementMeasuredPosition.Value       = this.m_lastMeasuredPosition.ToString();
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
