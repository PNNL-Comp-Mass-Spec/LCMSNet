using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private readonly List<string> m_methodList;

        public classNetworkStartEventArgs(List<string> methodList)
        {
            m_methodList = methodList;
        }

        public List<string> MethodList
        {
            get { return m_methodList; }
        }
    }
}