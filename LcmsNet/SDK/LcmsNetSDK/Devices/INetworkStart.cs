using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Interface for a network start.
    /// </summary>
    public interface INetworkStart
    {
        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        event EventHandler<classNetworkStartEventArgs> MethodNames;
    }

    public class classNetworkStartEventArgs : EventArgs
    {
        public classNetworkStartEventArgs(List<string> methodList)
        {
            MethodList = methodList;
        }

        public List<string> MethodList { get; }
    }
}