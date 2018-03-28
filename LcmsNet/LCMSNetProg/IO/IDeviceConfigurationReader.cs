using LcmsNetSDK.Devices;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Interface for retrieving a device configuration from storage.
    /// </summary>
    interface IDeviceConfigurationReader
    {
        classDeviceConfiguration ReadConfiguration(string path);
    }
}