using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using LcmsNet.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

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