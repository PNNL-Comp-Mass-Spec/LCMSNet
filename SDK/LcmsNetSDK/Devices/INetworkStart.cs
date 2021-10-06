using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Interface for a network start.
    /// </summary>
    public interface INetworkStart
    {
        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        event EventHandler<NetworkStartEventArgs> MethodNames;
    }

    public class NetworkStartEventArgs : EventArgs
    {
        public NetworkStartEventArgs(List<string> methodList)
        {
            MethodList = methodList;
        }

        public List<string> MethodList { get; }
    }
}