using System.Collections.Generic;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Writes data to a stream for health monitoring reasons.
    /// </summary>
    public interface IDeviceHealthWriter
    {
        void WriteDevices(List<IDevice> devices, string path);
    }
}