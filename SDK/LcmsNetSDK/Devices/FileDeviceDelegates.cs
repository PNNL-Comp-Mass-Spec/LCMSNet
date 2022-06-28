// This code is commented out because I couldn't get it to work, so I moved the delegate definitions into the IDeviceControl file - DAC

using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    public delegate void DelegateDeviceHasData(object sender, List<object> data);

    public delegate void DelegateNameListReceived(List<string> trayList);
}
