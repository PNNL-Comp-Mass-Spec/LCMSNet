using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Data points from pump readbacks
    /// </summary>
    public class PumpDataPoint
    {
        /// <summary>
        /// Time point
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Pressure at time point
        /// </summary>
        public double Pressure { get; }

        /// <summary>
        /// Flow rate at time point
        /// </summary>
        public double FlowRate { get; }

        /// <summary>
        /// %B at time point
        /// </summary>
        public double PercentB { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pressure"></param>
        /// <param name="flowRate"></param>
        /// <param name="percentB"></param>
        public PumpDataPoint(DateTime time, double pressure, double flowRate, double percentB)
        {
            Time = time;
            Pressure = pressure;
            FlowRate = flowRate;
            PercentB = percentB;
        }
    }
}
