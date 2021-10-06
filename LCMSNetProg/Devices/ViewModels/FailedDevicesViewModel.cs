using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LcmsNetSDK.Devices;
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
        public FailedDevicesViewModel() : this(new List<DeviceErrorEventArgs>()
        {
            new DeviceErrorEventArgs("Test Failure message", new Exception("Test exception message"), DeviceErrorStatus.ErrorSampleOnly, new TimerDevice()),
            new DeviceErrorEventArgs("Test Failure message", null, DeviceErrorStatus.ErrorAffectsAllColumns, new TimerDevice()),
        })
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public FailedDevicesViewModel(List<DeviceErrorEventArgs> deviceErrors)
        {
            DeviceErrorList = deviceErrors.Select(x => new DeviceErrorData(x.Device.Name, x.Error + " " + x.Exception?.Message)).ToList().AsReadOnly();
        }

        public ReadOnlyCollection<DeviceErrorData> DeviceErrorList { get; }
    }

    public class DeviceErrorData : ReactiveObject
    {
        public string DeviceName { get; }
        public string Error { get; }

        public DeviceErrorData(string deviceName, string error)
        {
            DeviceName = deviceName;
            Error = error;
        }
    }
}
