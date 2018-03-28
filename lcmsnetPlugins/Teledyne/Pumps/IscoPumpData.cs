//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/03/2011
//
//*********************************************************************************************************

using System;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Holds status data for one ISCO pump
    /// </summary>
    public class IscoPumpData
    {
        #region "Member variables"

        IscoOperationMode m_OpMode = IscoOperationMode.ConstantPressure;

        #endregion

        #region "Properties"

        /// <summary>
        /// Pump pressure
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        ///  Pump flow rate
        /// </summary>
        public double Flow { get; set; }

        /// <summary>
        /// Pump volume
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Pump operation status
        /// </summary>
        public IscoOperationStatus OperationStatus { get; set; }

        /// <summary>
        /// Pump control method (Local/Remote)
        /// </summary>
        public IscoControlMode ControlMode { get; set; }

        /// <summary>
        /// Pump problem status
        /// </summary>
        public IscoProblemStatus ProblemStatus { get; set; }

        /// <summary>
        /// Pump operation mode
        /// </summary>
        public IscoOperationMode OperationMode
        {
            get { return m_OpMode; }
            set { m_OpMode = value; }
        }

        /// <summary>
        /// Current setpoint value
        /// </summary>
        public double SetPoint { get; set; }

        /// <summary>
        /// Current time
        /// </summary>
        public DateTime PointTime { get; set; }

        /// <summary>
        /// Refill rate
        /// </summary>
        public double RefillRate { get; set; }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructor overload requiring no parameters
        /// </summary>
        public IscoPumpData()
        {
            Pressure = 0;
            Flow = 0;
            Volume = 0;
            OperationStatus = IscoOperationStatus.Stopped;
            ControlMode = IscoControlMode.Local;
            ProblemStatus = IscoProblemStatus.None;
            PointTime = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
        }

        /// <summary>
        /// Constructor overload allowing parameter specification
        /// </summary>
        /// <param name="press">Pump pressure</param>
        /// <param name="flo">Pump flow rate</param>
        /// <param name="vol">Pump volume</param>
        /// <param name="opStatus">Pump operation status</param>
        /// <param name="ctrlType">Pump control type (Local/Remote)</param>
        /// <param name="probStatus">Pump error status</param>
        /// <param name="currTime"></param>
        public IscoPumpData(double press, double flo, double vol,
            IscoOperationStatus opStatus, IscoControlMode ctrlType,
            IscoProblemStatus probStatus, DateTime currTime)
        {
            Pressure = press;
            Flow = flo;
            Volume = vol;
            OperationStatus = opStatus;
            ControlMode = ctrlType;
            ProblemStatus = probStatus;
            PointTime = currTime;
        }

        #endregion
    } //End class
}
