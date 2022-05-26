using System;
using System.Collections.Generic;

namespace LcmsNet.Method
{
    /// <summary>
    /// Abstract Control class that allows the user to define how the value is grabbed
    /// from the user interface for setting a parameter value in the method editor.
    /// </summary>
    public interface ILCEventParameter
    {
        string ParameterLabel { get; set; }

        /// <summary>
        /// Gets the value set by the user.
        /// </summary>
        object ParameterValue { get; set; }

        /// <summary>
        /// Fired when a parameter changes.
        /// </summary>
        event EventHandler EventChanged;

        //bool IsTime { get; set; }

        /// <summary>
        /// Method to create a basic duplicate of the event parameter.
        /// </summary>
        /// <returns></returns>
        ILCEventParameter CreateDuplicate();
    }

    /// <summary>
    /// Abstract Control class that allows the user to define how the value is grabbed
    /// from the user interface for setting a parameter value in the method editor.
    /// This interface is used for classes that can use a data provider to supply options to the user.
    /// </summary>
    public interface ILCEventParameterWithDataProvider : ILCEventParameter
    {
        /// <summary>
        /// Method for storing objects in the list view.
        /// </summary>
        /// <param name="sender">This parameter is needed for 'IDevice.RegisterDataProvider' support</param>
        /// <param name="data"></param>
        void FillData(object sender, IReadOnlyList<object> data);
    }
}