//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Updates:
// - 9/8/2009 (BLL)
//    - Removed initialization from second constructor that takes a serial port.
//    - Added comments on properties missing them.  (e.g. describing Gets or sets properties)
//    - Additional cleanup in code making a new region for method editor enabled methods.
/*********************************************************************************************************/

using System;
using System.IO.Ports;
using System.Threading.Tasks;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves
{
    /// <summary>
    /// Class used for interacting with the VICI 2-position valve
    /// </summary>
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    /*[DeviceControlAttribute(typeof(controlValveVICI2Pos),
                                 typeof(controlValveVICI2PosGlyph),
                                 "Valve 2-Position",
                                 "Valves")
    ]*/
    public class ValveVICI2Pos : ValveVICIBase, IDevice
    {
        // Serial port Settings for EHCA-CE (2-pos actuator):
        //     Baud Rate   9600
        //     Parity      None
        //     Stop Bits   One
        //     Data Bits   8
        //     Handshake   None

        #region Members
        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        private TwoPositionState m_lastMeasuredPosition;
        /// <summary>
        /// The last sent position to the valve.
        /// </summary>
        private TwoPositionState m_lastSentPosition;

        private static readonly int m_rotationDelayTimems = 145;  //milliseconds
        private const int CONST_READTIMEOUT = 500;          //milliseconds
        private const int CONST_WRITETIMEOUT = 500;         //milliseconds
        #endregion

        #region Events

        /// <summary>
        /// Indicates that the valve position has changed
        /// </summary>
        public event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICI2Pos() : base(CONST_READTIMEOUT, CONST_WRITETIMEOUT, "valve")
        {
            //Set positions to unknown
            m_lastMeasuredPosition = TwoPositionState.Unknown;
            m_lastSentPosition       = TwoPositionState.Unknown;
        }

        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="port">The serial port object to use.</param>
        public ValveVICI2Pos(SerialPort port) : base(port, "valve")
        {
            //Set positions to unknown
            m_lastMeasuredPosition   = TwoPositionState.Unknown;
            m_lastSentPosition       = TwoPositionState.Unknown;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last measured position of the valve.
        /// </summary>
        public TwoPositionState LastMeasuredPosition => m_lastMeasuredPosition;

        /// <summary>
        /// Gets the last position sent to the valve.
        /// </summary>
        public TwoPositionState LastSentPosition => m_lastSentPosition;

        #endregion

        #region Methods

        /// <summary>
        /// Indicates that the device's position has changed.
        /// </summary>
        /// <param name="position">The new position</param>
        protected virtual void OnPositionChanged(TwoPositionState position)
        {
            Task.Run(() => PositionChanged?.Invoke(this, new ValvePositionEventArgs<TwoPositionState>(position)));
        }

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
                m_lastMeasuredPosition = TwoPositionState.PositionA;
            }

            return result;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public override int GetPosition()
        {
            if (Emulation)
            {
                return (int)m_lastSentPosition;
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
                Port.WriteLine(SoftwareID + "CP");
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

            //Make a string containing the position
            var tempPosition = "Unknown";        //Default to unknown

            //Grab the actual position from the above string
            if (tempBuffer.Length > 1)  //Make sure we have content in the string
            {
                //Find the "
                var tempCharIndex = tempBuffer.IndexOf("Position is \"", StringComparison.Ordinal);
                if (tempCharIndex >= 0)  //Make sure we found it
                {
                    //Change the position to be the character following the "
                    tempPosition = tempBuffer.Substring(tempCharIndex + 13, 1);
                }
            }

            if (tempPosition == "A")
            {
                m_lastMeasuredPosition = TwoPositionState.PositionA;
                return (int)TwoPositionState.PositionA;
            }

            if (tempPosition == "B")
            {
                m_lastMeasuredPosition = TwoPositionState.PositionB;
                return (int)TwoPositionState.PositionB;
            }

            m_lastMeasuredPosition = TwoPositionState.Unknown;
            return (int)TwoPositionState.Unknown;
        }
        #endregion

        #region Method Editor Enabled Methods
//        /// <summary>
//        /// Sets the position of the valve (A or B).
//        /// </summary>
//        /// <param name="newPosition">The new position.</param>
//        [LCMethodEventAttribute("Set Position", 1, true, "", -1, false)]
//        public FluidicsSDK.Base.ValveErrors SetPosition(TwoPositionState newPosition)
//        {
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine("\tSetting Position" + newPosition.ToString());
//#endif

//            if (m_emulation == true)
//            {
//                m_lastSentPosition = m_lastMeasuredPosition = newPosition;
//                OnPositionChanged(m_lastMeasuredPosition);
//                return ValveErrors.Success;
//            }
//            // short circuit..if we're already in that position, why bother trying to move to it?
//            if (newPosition == m_lastMeasuredPosition)
//            {
//                return ValveErrors.Success;
//            }
//            //If the serial port is not open, open it
//            if (!m_serialPort.IsOpen)
//            {
//                try
//                {
//                    m_serialPort.Open();
//                }
//                catch (UnauthorizedAccessException)
//                {
//                    return ValveErrors.UnauthorizedAccess;
//                }
//            }

//            string cmd = null;
//            if (newPosition == TwoPositionState.PositionA)
//            {
//                m_lastSentPosition = TwoPositionState.PositionA;
//                cmd = m_valveID.ToString() + "GOA";
//            }

//            else if (newPosition == TwoPositionState.PositionB)
//            {
//                m_lastSentPosition = TwoPositionState.PositionB;
//                cmd = m_valveID.ToString() + "GOB";
//            }
//            else
//            {
//                return ValveErrors.BadArgument;
//            }

//            try
//            {
//                m_serialPort.WriteLine(cmd);
//            }
//            catch (TimeoutException)
//            {
//                return ValveErrors.TimeoutDuringWrite;
//            }
//            catch (UnauthorizedAccessException)
//            {
//                return ValveErrors.UnauthorizedAccess;
//            }

//            //Wait 145ms for valve to actually switch before proceeding
//            //NOTE: This can be shortened if there are more than 4 ports but still
//            //      2 positions; see manual page 1 for switching times

//            System.Threading.Thread.Sleep(m_rotationDelayTimems);

//            //Doublecheck that the position change was correctly executed
//            try
//            {
//                GetPosition();
//            }
//            catch (ValveExceptionWriteTimeout)
//            {
//                return ValveErrors.TimeoutDuringWrite;
//            }
//            catch (ValveExceptionUnauthorizedAccess)
//            {
//                return ValveErrors.UnauthorizedAccess;
//            }

//            if (m_lastMeasuredPosition != m_lastSentPosition)
//            {
//                return ValveErrors.ValvePositionMismatch;
//            }
//            else
//            {
//                OnPositionChanged(m_lastMeasuredPosition);
//                return ValveErrors.Success;
//            }
//        }

        /// <summary>
        /// Sets the position of the valve (A or B).
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        [LCMethodEvent("Set Position", 1, true, "", -1, false)]
        public void SetPosition(TwoPositionState newPosition)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("\tSetting Position" + newPosition.ToString());
#endif

            if (Emulation)
            {
                m_lastSentPosition = m_lastMeasuredPosition = newPosition;
                OnPositionChanged(m_lastMeasuredPosition);
                return;
            }
            // short circuit..if we're already in that position, why bother trying to move to it?
            if (newPosition == m_lastMeasuredPosition)
            {
                return;
            }
            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                Port.Open();
            }

            string cmd;
            if (newPosition == TwoPositionState.PositionA)
            {
                m_lastSentPosition = TwoPositionState.PositionA;
                cmd = SoftwareID + "GOA";
            }

            else if (newPosition == TwoPositionState.PositionB)
            {
                m_lastSentPosition = TwoPositionState.PositionB;
                cmd = SoftwareID + "GOB";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid Position to set" + newPosition.ToCustomString());
            }

            Port.WriteLine(cmd);

            //Wait 145ms for valve to actually switch before proceeding
            //NOTE: This can be shortened if there are more than 4 ports but still
            //      2 positions; see manual page 1 for switching times

            System.Threading.Thread.Sleep(m_rotationDelayTimems);

            //Doublecheck that the position change was correctly executed
            GetPosition();
            OnPositionChanged(m_lastMeasuredPosition);
        }
        #endregion

        /*public override FinchComponentData GetData()
          {
              FinchComponentData component = new FinchComponentData();
              component.Status = Status.ToString();
              component.Name = Name;
              component.Type = "Valve";
              component.LastUpdate = DateTime.Now;

              FinchScalarSignal measurementSentPosition = new FinchScalarSignal();
              measurementSentPosition.Name = "Set Position";
              measurementSentPosition.Type = FinchDataType.Integer;
              measurementSentPosition.Units = "";
              measurementSentPosition.Value = this.m_lastSentPosition.ToString();
              component.Signals.Add(measurementSentPosition);

              FinchScalarSignal measurementMeasuredPosition = new FinchScalarSignal();
              measurementMeasuredPosition.Name = "Measured Position";
              measurementMeasuredPosition.Type = FinchDataType.Integer;
              measurementMeasuredPosition.Units = "";
              measurementMeasuredPosition.Value = this.m_lastMeasuredPosition.ToString();
              component.Signals.Add(measurementMeasuredPosition);

              FinchScalarSignal port = new FinchScalarSignal();
              port.Name = "Port";
              port.Type = FinchDataType.String;
              port.Units = "";
              port.Value = this.PortName.ToString();
              component.Signals.Add(port);

              return component;
          }*/
    }
}
