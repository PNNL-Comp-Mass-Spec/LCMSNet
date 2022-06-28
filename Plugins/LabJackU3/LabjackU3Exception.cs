﻿using System;

namespace LcmsNetPlugins.LabJackU3
{
    /// <summary>
    /// A class containing exceptions generated by the Labjack U3
    /// </summary>
    public class LabjackU3Exception : Exception
    {
        public LabjackU3Exception()
            : base()
        {
        }

        public LabjackU3Exception(string message)
            : base(message)
        {
        }

        public LabjackU3Exception(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
