using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Class for event handler arguments.
    /// </summary>
    public class PumpDataEventArgs : EventArgs
    {
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

        public IPump Pump { get; }

        public List<DateTime> Time { get; }

        public List<double> Pressure { get; }

        public List<double> Flowrate { get; }

        public List<double> PercentB { get; }
    }
}