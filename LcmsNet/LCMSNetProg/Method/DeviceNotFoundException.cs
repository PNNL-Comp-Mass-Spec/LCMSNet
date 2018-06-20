using System;

namespace LcmsNet.Method
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException(string message)
            : base(message)
        {
        }

        public DeviceNotFoundException(string message, string deviceName)
            : base(message)
        {
            DeviceName = deviceName;
        }

        public string DeviceName { get; private set; }
    }
}