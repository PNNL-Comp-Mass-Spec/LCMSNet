using LcmsNetSDK.Devices;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Used for persisting a device list to storage.
    /// </summary>
    interface IDeviceConfigurationWriter
    {
        void WriteConfiguration(string path, DeviceConfiguration configuration);
    }
}