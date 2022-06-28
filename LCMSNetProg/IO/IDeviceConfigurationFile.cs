using LcmsNetSDK.Devices;

namespace LcmsNet.IO
{
    /// <summary>
    /// Interface for retrieving/persisting a device configuration list from/to storage.
    /// </summary>
    interface IDeviceConfigurationFile
    {
        DeviceConfiguration ReadConfiguration(string path);
        void WriteConfiguration(string path, DeviceConfiguration configuration);
    }
}
