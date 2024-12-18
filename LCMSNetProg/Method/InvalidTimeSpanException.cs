﻿using System;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class thrown if the duration of an LC Event was invalid
    /// </summary>
    public class InvalidTimeSpanException : Exception
    {
        /// <summary>
        /// Constructor for a new invalid time span exception
        /// </summary>
        /// <param name="message"></param>
        public InvalidTimeSpanException(string message) : base(message)
        {
        }
    }
}
