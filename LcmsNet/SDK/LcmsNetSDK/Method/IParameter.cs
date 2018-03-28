using System;

namespace LcmsNetSDK.Method
{
    /// <summary>
    /// Abstract Control class that allows the user to define how the value is grabbed
    /// from the user interface for setting a parameter value in the method editor.
    /// </summary>
    public interface ILCEventParameter
    {
        /// <summary>
        /// Gets the value set by the user.
        /// </summary>
        object ParameterValue { get; set; }

        /// <summary>
        /// Fired when a parameter changes.
        /// </summary>
        event EventHandler EventChanged;

        //bool IsTime { get; set; }
    }
}