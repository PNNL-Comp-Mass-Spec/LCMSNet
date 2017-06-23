using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetDataClasses.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class FailedDevicesViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the failed devices view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public FailedDevicesViewModel()
        {
            var list = new List<classDeviceErrorEventArgs>();
            list.Add(new classDeviceErrorEventArgs("Test Failure message", new Exception("Test exception message"), enumDeviceErrorStatus.ErrorSampleOnly, new classTimerDevice()));
            list.Add(new classDeviceErrorEventArgs("Test Failure message", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, new classTimerDevice()));
            UpdateDeviceList(list);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public FailedDevicesViewModel(List<classDeviceErrorEventArgs> deviceErrors)
        {
            UpdateDeviceList(deviceErrors);
        }

        private readonly ReactiveList<DeviceErrorData> deviceErrorList = new ReactiveList<DeviceErrorData>();

        public IReadOnlyReactiveList<DeviceErrorData> DeviceErrorList => deviceErrorList;

        /// <summary>
        /// Updates the listview with the error device messages.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public void UpdateDeviceList(List<classDeviceErrorEventArgs> deviceErrors)
        {
            using (deviceErrorList.SuppressChangeNotifications())
            {
                deviceErrorList.Clear();
                deviceErrorList.AddRange(deviceErrors.Select(x => new DeviceErrorData(x.Device.Name, x.Error + " " + x.Exception?.Message)));
            }
        }
    }

    public class DeviceErrorData : ReactiveObject
    {
        public string DeviceName { get; private set; }
        public string Error { get; private set; }

        public DeviceErrorData(string deviceName, string error)
        {
            DeviceName = deviceName;
            Error = error;
        }
    }
}
