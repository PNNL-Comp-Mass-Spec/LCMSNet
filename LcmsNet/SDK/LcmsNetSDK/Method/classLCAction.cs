using System;
using System.Reflection;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// An atomic object operation used by stages in a method.
    /// </summary>
    public class classLCAction
    {
        /// <summary>
        /// Gets or sets the method used to perform the deterministic action required.
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// Gets or sets the start time of this action.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Gets or sets the duration for this action.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }   
}
