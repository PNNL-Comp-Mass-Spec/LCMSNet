using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    public interface IAutoSampler
    {
        /// <summary>
        /// Fired when new trays are available.
        /// </summary>
        event EventHandler<classAutoSampleEventArgs> TrayNames;

        /// <summary>
        /// Fired when new methods are available.
        /// </summary>
        event EventHandler<classAutoSampleEventArgs> MethodNames;
    }

    /// <summary>
    /// 
    /// </summary>
    public class classAutoSampleEventArgs : EventArgs
    {
        private readonly List<string> m_methodList;
        private readonly List<string> m_trayList;

        public classAutoSampleEventArgs(List<string> trayList, List<string> methodList)
        {
            m_trayList = trayList;
            m_methodList = methodList;
        }

        public List<string> TrayList => m_trayList;

        public List<string> MethodList => m_methodList;
    }
}