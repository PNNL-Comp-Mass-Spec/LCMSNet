using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Class for event handler arguments.
    /// </summary>
    public class PumpDataEventArgs : EventArgs
    {
        [Obsolete("old", true)]
        public PumpDataEventArgs(IPump pump,
            List<DateTime> time,
            List<double> pressure,
            List<double> flowrate,
            List<double> percentB)
        {
            Pump = pump;
            Time = time;
            Pressure = pressure;
            Flowrate = flowrate;
            PercentB = percentB;
        }

        public PumpDataEventArgs(IPump pump, IReadOnlyList<PumpDataPoint> dataPoints)
        {
            Pump = pump;
            DataPoints = dataPoints;
        }

        public IPump Pump { get; }

        public IReadOnlyList<PumpDataPoint> DataPoints { get; }

        [Obsolete("old", true)]
        public List<DateTime> Time { get; }

        [Obsolete("old", true)]
        public List<double> Pressure { get; }

        [Obsolete("old", true)]
        public List<double> Flowrate { get; }

        [Obsolete("old", true)]
        public List<double> PercentB { get; }
    }
}
