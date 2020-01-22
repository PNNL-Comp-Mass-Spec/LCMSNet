using System;

namespace LcmsNetSDK.Method
{
    /// <summary>
    /// Class for specifying a range of a paramter
    /// </summary>
    public class LCMethodParameterNumericRange : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public LCMethodParameterNumericRange(string parameterName, double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            Name = parameterName;
        }

        public double Minimum { get; }

        public double Maximum { get; }

        public string Name { get; }
    }
}