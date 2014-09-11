using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Enumeration of possible status.
    /// </summary>
    public enum enumDeviceStatus
    {
        
		NotInitialized,
		Initialized,
        Error,
		InUseByMethod,        
    }
}
