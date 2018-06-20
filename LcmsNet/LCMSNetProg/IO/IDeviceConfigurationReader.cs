using LcmsNetSDK.Devices;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Interface for retrieving a device configuration from storage.
    /// </summary>
    interface IDeviceConfigurationReader
    {
        DeviceConfiguration ReadConfiguration(string path);
    }
}