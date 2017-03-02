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
        public classAutoSampleEventArgs(List<string> trayList, List<string> methodList)
        {
            TrayList = trayList;
            MethodList = methodList;
        }

        public List<string> TrayList { get; }

        public List<string> MethodList { get; }
    }
}