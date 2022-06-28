using System;

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

    /// <summary>
    /// Class for bridging the device manager to the user defined controls
    /// </summary>
    [Obsolete("Use classDeviceManager.Manager instead", true)]
    public class DeviceManagerBridge
    {
        private static IDeviceManager m_manager;

        public DeviceManagerBridge(IDeviceManager manager)
        {
            m_manager = manager;
            m_manager.DeviceAdded += m_manager_DeviceAdded;
            m_manager.DeviceRemoved += m_manager_DeviceRemoved;
        }

        public static event DelegateDeviceUpdated DeviceAdded;
        public static event DelegateDeviceUpdated DeviceRemoved;

        void m_manager_DeviceRemoved(object sender, IDevice device)
        {
            DeviceRemoved?.Invoke(this, device);
        }

        void m_manager_DeviceAdded(object sender, IDevice device)
        {
            DeviceAdded?.Invoke(this, device);
        }

        /// <summary>
        /// Renames the device to the name supplied.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="basename"></param>
        public static void RenameDevice(IDevice device, string basename)
        {
            m_manager.RenameDevice(device, basename);
        }
    }
}
