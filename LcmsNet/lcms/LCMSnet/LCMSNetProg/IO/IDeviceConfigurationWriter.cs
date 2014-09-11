using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
