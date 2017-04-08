using System;

namespace LcmsNet.Method
{
    public class classDeviceNotFoundException : Exception
    {
        public classDeviceNotFoundException(string message)
            : base(message)
        {
        }

        public classDeviceNotFoundException(string message, string deviceName)
            : base(message)
        {
            DeviceName = deviceName;
        }

        public string DeviceName { get; private set; }
    }
}