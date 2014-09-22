using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Used for persisting a device list to storage.
    /// </summary>
    interface IDeviceConfigurationWriter
    {        
        void WriteConfiguration(string path, classDeviceConfiguration configuration);        
    }
}
