namespace LcmsNetDataClasses.Devices
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
    public class DeviceManagerBridge
    {
        private static IDeviceManager m_manager;

        public DeviceManagerBridge(IDeviceManager manager)
        {
            m_manager = manager;
            m_manager.DeviceAdded += new DelegateDeviceUpdated(m_manager_DeviceAdded);
            m_manager.DeviceRemoved += new DelegateDeviceUpdated(m_manager_DeviceRemoved);
        }

        public static event DelegateDeviceUpdated DeviceAdded;
        public static event DelegateDeviceUpdated DeviceRemoved;

        void m_manager_DeviceRemoved(object sender, IDevice device)
        {
            if (DeviceRemoved != null)
            {
                DeviceRemoved(this, device);
            }
        }

        void m_manager_DeviceAdded(object sender, IDevice device)
        {
            if (DeviceAdded != null)
            {
                DeviceAdded(this, device);
            }
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