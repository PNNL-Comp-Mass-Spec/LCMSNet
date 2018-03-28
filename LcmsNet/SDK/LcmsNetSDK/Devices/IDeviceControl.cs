namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Describes the interface for user control's that interface a given hardware device or object.
    /// </summary>
    public interface IDeviceControl
    {
        #region "Events"

        event DelegateNameChanged NameChanged;
        event DelegateSaveRequired SaveRequired;

        #endregion

        #region "Properties"

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

        global::System.Windows.Controls.UserControl GetDefaultView();

        #endregion
    }
}
