// This code is commented out because I couldn't get it to work, so I moved the delegate definitions into the IDeviceControl file - DAC

using System.Collections.Generic;

namespace LcmsNetSDK.Devices
{
    //All these need to be propogated up through the class
    public delegate void DelegateDevicePositionChange(object sender, string p);

    public delegate void DelegateDeviceHasData(object sender, List<object> data);

    /// <summary>
    /// Delegate defining the status of a device.
    /// </summary>
    /// <param name="sender">Device called.</param>
    /// <param name="status">Status of device</param>
    public delegate void DelegateDeviceStatusUpdate(object sender, DeviceStatus status);

    public delegate void DelegateDeviceWait(object sender);

    public delegate void DelegateDeviceFree(object sender);

    public delegate void DelegateNameListReceived(List<string> trayList);
}