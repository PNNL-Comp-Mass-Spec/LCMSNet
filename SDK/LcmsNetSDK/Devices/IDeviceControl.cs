using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Describes the interface for user control's that interface a given hardware device or object.
    /// </summary>
    public interface IDeviceControl
    {
        /// <summary>
        ///  An event that indicates the name has changed. The parameter is the new name
        /// </summary>
        event EventHandler<string> NameChanged;

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        event Action SaveRequired;

        /// <summary>
        /// Indicates control state
        /// </summary>
        bool Running { get; set; }

        /// <summary>
        /// Gets the device to be controlled that's associated with this interface
        /// </summary>
        IDevice Device { get; set; }

        /// <summary>
        /// Gets or sets the name of the device control.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Status of device, updated using UpdateStatusDisplay
        /// </summary>
        string DeviceStatus { get; }

        /// <summary>
        /// Used for conditional control (like key bindings) when the device control is visible
        /// </summary>
        bool DeviceTabSelected { get; set; }

        /// <summary>
        /// Used to disable conditional control without clearing selection states
        /// </summary>
        void OutOfView();

        /// <summary>
        /// Gets an instance of the default view for this view model
        /// </summary>
        /// <returns></returns>
        global::System.Windows.Controls.UserControl GetDefaultView();
    }
}
