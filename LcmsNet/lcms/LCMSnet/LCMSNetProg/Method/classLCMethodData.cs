using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method
{
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
            Method              = info;
            Parameters          = parameter;
            MethodAttribute     = attr;
            Device              = device;
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
        #endregion

        /// <summary>
        /// Builds the method by grabbing the values stored in the ILCEventParameter objects.
        /// </summary>
        public void BuildMethod()
        {
            for(int i = 0; i < Parameters.Controls.Count; i++)
            {
                Control control = Parameters.Controls[i];
                
                if (control != null)
                {
                    /// 
                    /// Grab the controls value to be used later on
                    /// 
                    ILCEventParameter parameterControl = control as ILCEventParameter;
                    if (parameterControl != null)
                    {
                        Parameters.Values[i] = parameterControl.ParameterValue;
                    }
                }
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
    }
}
