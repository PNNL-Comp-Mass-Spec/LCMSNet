using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method
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
    public class classLCMethodData
    {
        /// <summary>
        /// Constructor that takes a method, and the value to call it with.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="val"></param>
        public classLCMethodData(IDevice device,
            MethodInfo info,
            classLCMethodAttribute attr,
            classLCMethodEventParameter parameter)
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

                if (control != null)
                {
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
        }

        public void Break()
        {
            if (BreakPointEvent != null)
            {
                BreakPointEvent(this, new BreakEventArgs(true));
            }
        }

        public void PassBreakPoint()
        {
            if (BreakPointEvent != null)
            {
                BreakPointEvent(this, new BreakEventArgs(false));
            }
        }

        public void IsDone()
        {
            Executing = false;
            if (Simulated != null)
            {
                Simulated(this, new EventArgs());
            }
        }

        public void IsCurrent()
        {
            Executing = true;
            if (SimulatingEvent != null)
            {
                SimulatingEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Returns the name of the method in human readable form.
        /// </summary>
        /// <returns>Name of the method</returns>
        public override string ToString()
        {
            return MethodAttribute.Name;
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
        public classLCMethodAttribute MethodAttribute { get; set; }

        /// <summary>
        /// Gets or sets the parameter values.
        /// </summary>
        public classLCMethodEventParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets whether this event is part of the optimization step.
        /// </summary>
        public bool OptimizeWith { get; set; }

        public bool BreakPoint { get; set; }

        public bool Executing { get; set; }

        #endregion
    }
}