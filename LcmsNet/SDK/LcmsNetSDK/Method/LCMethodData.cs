using System;
using System.Reflection;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    public class BreakEventArgs : EventArgs
    {
        public BreakEventArgs(bool stoppedHere)
        {
            IsStopped = stoppedHere;
        }

        public bool IsStopped { get; set; }
    }

    /// <summary>
    /// Class that holds the selected method and the value to pass for the parameters.
    /// </summary>
    public class LCMethodData
    {
        /// <summary>
        /// Constructor that takes a method, and the value to call it with.
        /// </summary>
        public LCMethodData(IDevice device,
            MethodInfo info,
            LCMethodEventAttribute attr,
            LCMethodEventParameter parameter)
        {
            Method = info;
            Parameters = parameter;
            MethodAttribute = attr;
            Device = device;
        }

        public event EventHandler<BreakEventArgs> BreakPointEvent;
        public event EventHandler Simulated;
        public event EventHandler SimulatingEvent;

        /// <summary>
        /// Builds the method by grabbing the values stored in the ILCEventParameter objects.
        /// </summary>
        public void BuildMethod()
        {
            for (var i = 0; i < Parameters.Controls.Count; i++)
            {
                var control = Parameters.Controls[i];

                // 
                // Grab the controls value to be used later on
                // 
                var parameterControl = control as ILCEventParameter;
                if (parameterControl != null)
                {
                    Parameters.Values[i] = parameterControl.ParameterValue;
                }
            }
        }

        public void Break()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(true));
        }

        public void PassBreakPoint()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(false));
        }

        public void IsDone()
        {
            Executing = false;
            Simulated?.Invoke(this, new EventArgs());
        }

        public void IsCurrent()
        {
            Executing = true;
            SimulatingEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Returns the name of the method in human readable form.
        /// </summary>
        /// <returns>Name of the method</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(MethodAttribute.Name) ? "Undefined method" : MethodAttribute.Name;
        }

        #region Properties

        public IDevice Device { get; set; }

        /// <summary>
        /// Method that holds the given parameters.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Gets or sets the method attribute information for this method.
        /// </summary>
        public LCMethodEventAttribute MethodAttribute { get; set; }

        /// <summary>
        /// Gets or sets the parameter values.
        /// </summary>
        public LCMethodEventParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets whether this event is part of the optimization step.
        /// </summary>
        public bool OptimizeWith { get; set; }

        public bool BreakPoint { get; set; }

        public bool Executing { get; set; }

        #endregion
    }
}