using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// Class for specifying a range of a paramter
    /// </summary>
    public class classLCMethodParameterNumericRange : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public classLCMethodParameterNumericRange(string parameterName, double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            Name = parameterName;
        }

        public double Minimum { get; private set; }

        public double Maximum { get; private set; }

        public string Name { get; private set; }
    }
}