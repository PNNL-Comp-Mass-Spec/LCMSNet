using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Base interface for autosamplers
    /// </summary>
    public interface IAutoSampler
    {
        /// <summary>
        /// Fired when new trays are available.
        /// </summary>
        event EventHandler<classAutoSampleEventArgs> TrayNamesRead;

        /// <summary>
        /// Fired when new methods are available.
        /// </summary>
        event EventHandler<classAutoSampleEventArgs> MethodNamesRead;

        /// <summary>
        /// List of tray names in the autosampler
        /// </summary>
        List<string> TrayNames { get; }

        /// <summary>
        /// List of methods in the autosampler
        /// </summary>
        List<string> MethodNames { get; }

        /// <summary>
        /// The max vial number for each tray in the autosampler (according to autosampler configuration). Does not imply actual presence of a tray or vial.
        /// </summary>
        Dictionary<string, int> TrayNamesAndMaxVials { get; }

        /// <summary>
        /// If the device is emulated
        /// </summary>
        bool Emulation { get; }
    }

    /// <summary>
    /// Event args passed when IAutoSampler events are fired
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

    public static class AutoSamplers
    {
        public static List<IAutoSampler> ConnectedAutoSamplers { get; } = new List<IAutoSampler>();
    }
}