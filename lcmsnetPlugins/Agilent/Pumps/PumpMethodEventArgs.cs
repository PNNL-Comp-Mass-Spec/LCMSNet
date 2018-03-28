using System;

namespace LcmsNetPlugins.Agilent.Pumps
{

    /// <summary>
    /// Class that encapsulates method event arguments.
    /// </summary>
    public class classPumpMethodEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodname"></param>
        public classPumpMethodEventArgs(string methodname)
        {
            MethodName = methodname;
        }
        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string MethodName { get; private set; }
    }
}
