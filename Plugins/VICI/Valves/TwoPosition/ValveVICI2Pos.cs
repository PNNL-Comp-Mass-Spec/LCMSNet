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
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
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
    public class ValveVICI2Pos : ValveVICIBase, IDevice, ITwoPositionValve
    {
        private static readonly int RotationDelayTimeMsec = 145;  //milliseconds
        private const int CONST_READTIMEOUT = 500;          //milliseconds
        private const int CONST_WRITETIMEOUT = 500;         //milliseconds

        /// <summary>
        /// Indicates that the valve position has changed
        /// </summary>
        public event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICI2Pos() : base(CONST_READTIMEOUT, CONST_WRITETIMEOUT, "valve")
        {
            //Set positions to unknown
            LastMeasuredPosition = TwoPositionState.Unknown;
            LastSentPosition = TwoPositionState.Unknown;
        }

        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="port">The serial port object to use.</param>
        public ValveVICI2Pos(SerialPort port) : base(port, "valve")
        {
            //Set positions to unknown
            LastMeasuredPosition = TwoPositionState.Unknown;
            LastSentPosition = TwoPositionState.Unknown;
        }

        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        public TwoPositionState LastMeasuredPosition { get; private set; }

        /// <summary>
        /// The last position sent to the valve.
        /// </summary>
        public TwoPositionState LastSentPosition { get; private set; }

        /// <summary>
        /// Display string for the Last Measured Position
        /// </summary>
        public override string LastMeasuredPositionDisplay => LastMeasuredPosition.GetEnumDescription();

        /// <summary>
        /// Indicates that the device's position has changed.
        /// </summary>
        /// <param name="position">The new position</param>
        protected virtual void OnPositionChanged(TwoPositionState position)
        {
            Task.Run(() => PositionChanged?.Invoke(this, new ValvePositionEventArgs<TwoPositionState>(position)));
        }

        /// <summary>
        /// Valve initialization calls that are specific to the valve mode - for example, checking the valve control mode for universal actuators
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public override bool InitializeModeSpecific(out string errorMessage)
        {
            errorMessage = "";

            if (Emulation)
            {
                //Fill in fake position
                LastMeasuredPosition = TwoPositionState.PositionA;
                return true;
            }

            try
            {
                if (!CheckIsTwoPosition())
                {
                    ApplicationLogger.LogError(1, $"ERROR: Valve reports that it is a multi-position valve, software configured for 2 positions! {Name}, COM port {PortName}");
                }
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                errorMessage = "Could not get the valve mode. " + ex.Message;
                return false;
            }
            catch (ValveExceptionReadTimeout ex)
            {
                errorMessage = "Reading the valve mode timed out. " + ex.Message;
                return false;
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                errorMessage = "Sending a command to get the valve mode timed out. " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as a display string.</returns>
        public override string GetPositionDisplay()
        {
            return ((TwoPositionState)GetPosition()).GetEnumDescription();
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public override int GetPosition()
        {
            if (Emulation)
            {
                return (int)LastSentPosition;
            }

            // Data returned from hardware should look like
            //  Position is "B"
            // GetHardwarePosition should return "B" (no quotes)
            var tempPosition = GetHardwarePosition();

            switch (tempPosition.ToUpper())
            {
                case "A":
                    LastMeasuredPosition = TwoPositionState.PositionA;
                    return (int)TwoPositionState.PositionA;
                case "B":
                    LastMeasuredPosition = TwoPositionState.PositionB;
                    return (int)TwoPositionState.PositionB;
                default:
                    LastMeasuredPosition = TwoPositionState.Unknown;
                    return (int)TwoPositionState.Unknown;
            }
        }

        void ITwoPositionValve.SetPosition(TwoPositionState newPosition)
        {
            SetPosition(newPosition);
        }

        /// <summary>
        /// Sets the position of the valve (A or B).
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        [LCMethodEvent("Set Position", 1, HasDiscreteParameters = true)]
        public ValveErrors SetPosition(TwoPositionState newPosition)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("\tSetting Position" + newPosition.ToString());
#endif

            if (Emulation)
            {
                LastSentPosition = LastMeasuredPosition = newPosition;
                OnPositionChanged(LastMeasuredPosition);
                return ValveErrors.Success;
            }

            /*
            // NOTE: This short-circuit is potentially dangerous - it relies on the last-read hardware position
            //       If someone has changed the valve position using the hardware buttons, the last-read hardware position is no longer valid
            //       Instead, it's safe enough to send a command to switch to the same position, and it ensures consistency;
            //          the valve firmware will not move the valve if it is told to move to the position it's already in. (for 2-position valves)
            //       The other option is to re-read the hardware position before performing this check, but that adds 200ms (read delay) to this method
            // short circuit..if we're already in that position, why bother trying to move to it?
            if (newPosition == LastMeasuredPosition)
            {
                return ValveErrors.Success;
            }
            */

            if (newPosition == TwoPositionState.PositionA || newPosition == TwoPositionState.PositionB)
            {
                //Wait 145ms for valve to actually switch before proceeding
                //NOTE: This can be shortened if there are more than 4 ports but still
                //      2 positions; see manual page 1 for switching times
                var sendError = SetHardwarePosition(newPosition.GetEnumDescription(), RotationDelayTimeMsec);

                if (sendError != ValveErrors.Success)
                {
                    return sendError;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid Position to set" + newPosition.GetEnumDescription());
            }

            //Doublecheck that the position change was correctly executed
            if (LastMeasuredPosition != newPosition)
            {
                //ApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position.");
                return ValveErrors.ValvePositionMismatch;
            }

            OnPositionChanged(LastMeasuredPosition);
            return ValveErrors.Success;
        }
    }
}
