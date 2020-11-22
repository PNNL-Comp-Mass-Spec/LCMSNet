using System;
using System.Reactive.Concurrency;
using System.Collections.Generic;
using System.Reactive.Linq;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class AdvancedDeviceControlPanelViewModel : ReactiveObject, IDisposable
    {
        public AdvancedDeviceControlPanelViewModel()
        {
            nameToViewModelMap = new Dictionary<string, AdvancedDeviceGroupControlViewModel>();
            deviceToViewModelMap = new Dictionary<IDevice, AdvancedDeviceGroupControlViewModel>();

            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;

            // Once a group is added to the device controls, automatically make the first one the "Selected group" if one hasn't been previously set.
            this.WhenAnyValue(x => x.deviceGroups, x => x.SelectedGroup, x => x.deviceGroups.Count).Where(x => x.Item1.Count > 0 && x.Item2 == null)
                .Subscribe(x => SelectedGroup = x.Item1[0]);

            this.WhenAnyValue(x => x.SelectedGroup).Subscribe(x =>
            {
                foreach (var group in deviceGroups)
                {
                    group.GroupIsSelected = false;
                }

                if (SelectedGroup != null)
                {
                    SelectedGroup.GroupIsSelected = true;
                }
            });
        }

        ~AdvancedDeviceControlPanelViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var grp in DeviceGroups)
            {
                grp.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        private readonly ReactiveList<AdvancedDeviceGroupControlViewModel> deviceGroups = new ReactiveList<AdvancedDeviceGroupControlViewModel>();
        private AdvancedDeviceGroupControlViewModel selectedGroup = null;

        public IReadOnlyReactiveList<AdvancedDeviceGroupControlViewModel> DeviceGroups => deviceGroups;

        /// <summary>
        /// Selected device
        /// </summary>
        public AdvancedDeviceGroupControlViewModel SelectedGroup
        {
            get => selectedGroup;
            set => this.RaiseAndSetIfChanged(ref selectedGroup, value);
        }

        /// <summary>
        /// Maps a device to which advanced control panel it belongs to.
        /// </summary>
        private readonly Dictionary<IDevice, AdvancedDeviceGroupControlViewModel> deviceToViewModelMap;

        /// <summary>
        /// Maps a device group name to the advanced control panel.
        /// </summary>
        private readonly Dictionary<string, AdvancedDeviceGroupControlViewModel> nameToViewModelMap;

        /// <summary>
        /// Handles when a device is removed from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRemoved(object sender, IDevice device)
        {
            if (deviceToViewModelMap.ContainsKey(device))
            {
                var vm = deviceToViewModelMap[device];
                vm.RemoveDevice(device);

                deviceToViewModelMap.Remove(device);

                if (vm.IsDeviceGroupEmpty)
                {
                    if (deviceGroups.Contains(vm))
                    {
                        deviceGroups.Remove(vm);
                    }
                    if (nameToViewModelMap.ContainsKey(vm.Name))
                    {
                        nameToViewModelMap.Remove(vm.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Handles a new device being added to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceAdded(object sender, IDevice device)
        {
            var type = device.GetType();

            var attributes = type.GetCustomAttributes(typeof(DeviceControlAttribute), false);
            foreach (var o in attributes)
            {
                if (o is DeviceControlAttribute monitorAttribute)
                {
                    IDeviceControl control = null;
                    if (monitorAttribute.ControlType != null)
                    {
                        control = Activator.CreateInstance(monitorAttribute.ControlType) as IDeviceControl;
                    }

                    AddDeviceViewModel(monitorAttribute.Category, device, control);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds a new device to the user interface.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="device"></param>
        /// <param name="deviceVm"></param>
        private void AddDeviceViewModel(string groupName, IDevice device, IDeviceControl deviceVm)
        {
            // Ensure that this always happens on the main thread
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                // Make sure group control exists.
                AdvancedDeviceGroupControlViewModel vm = null;
                if (!nameToViewModelMap.ContainsKey(groupName))
                {
                    // Create the tab page
                    var page = new AdvancedDeviceGroupControlViewModel(groupName);
                    nameToViewModelMap.Add(groupName, page);
                    deviceGroups.Add(page);
                }

                vm = nameToViewModelMap[groupName];
                deviceToViewModelMap.Add(device, vm);

                if (deviceVm == null)
                {
                    deviceVm = new DefaultUserDeviceViewModel();
                }

                deviceVm.Device = device;

                // Adds the device control to the group control
                vm.AddDevice(device, deviceVm);
            });
        }
    }
}
