namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Delegate definition when a device is added or removed.
    /// </summary>
    /// <param name="sender">Object who added the device.</param>
    /// <param name="device">Device that was added</param>
    public delegate void DelegateDeviceUpdated(object sender, IDevice device);

    /// <summary>
    /// Interface describing a device manager.
    /// </summary>
    public interface IDeviceManager
    {
        void RenameDevice(IDevice device, string basename);
        event DelegateDeviceUpdated DeviceAdded;
        event DelegateDeviceUpdated DeviceRemoved;
    }
}
