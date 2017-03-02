using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Devices.Pumps
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

        public IPump Pump { get; private set; }

        public List<DateTime> Time { get; private set; }

        public List<double> Pressure { get; private set; }

        public List<double> Flowrate { get; private set; }

        public List<double> PercentB { get; private set; }
    }
}