using System;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// Enumeration indicating what type of timeout is available.
    /// </summary>
    public enum enumMethodOperationTime
    {
        /// <summary>
        /// Timeout is specified in the attribute.
        /// </summary>
        Attribute,

        /// <summary>
        /// Timeout is specified by the first parameter.
        /// </summary>
        Parameter,

        /// <summary>
        /// Timeout is unknown at runtime.
        /// </summary>
        Indeterminate
    }
}