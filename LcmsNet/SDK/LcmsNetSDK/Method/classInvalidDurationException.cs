using System;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// Class thrown if the duration of an LC Event was invalid
    /// </summary>
    public class classInvalidTimeSpanException : Exception
    {
        /// <summary>
        /// Constructor for a new invalid time span exception
        /// </summary>
        /// <param name="message"></param>
        public classInvalidTimeSpanException(string message) : base(message)
        {
        }
    }
}