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
            m_controlToPageMap = new Dictionary<AdvancedDeviceGroupControlViewModel, DeviceGroup>();
            m_nameToControlMap = new Dictionary<string, AdvancedDeviceGroupControlViewModel>();
            m_deviceToControlMap = new Dictionary<IDevice, AdvancedDeviceGroupControlViewModel>();

            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;

            // Once a group is added to the device controls, automatically make the first one the "Selected group" if one hasn't been previously set.
            this.WhenAnyValue(x => x.deviceGroups, x => x.SelectedGroup, x => x.deviceGroups.Count).Where(x => x.Item1.Count > 0 && x.Item2 == null)
                .Subscribe(x => SelectedGroup = x.Item1[0]);
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

        public class DeviceGroup : IEquatable<DeviceGroup>, IDisposable
        {
            public string Name { get; }
            public AdvancedDeviceGroupControlViewModel Content { get; }

            public DeviceGroup(string name)
            {
                Name = name;
                Content = new AdvancedDeviceGroupControlViewModel();
            }

            ~DeviceGroup()
            {
                Dispose();
            }

            public void Dispose()
            {
                Content.Dispose();
                GC.SuppressFinalize(this);
            }

            public bool Equals(DeviceGroup other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DeviceGroup) obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }
        }

        private readonly ReactiveList<DeviceGroup> deviceGroups = new ReactiveList<DeviceGroup>();
        private DeviceGroup selectedGroup = null;

        public IReadOnlyReactiveList<DeviceGroup> DeviceGroups => deviceGroups;

        /// <summary>
        /// Selected device
        /// </summary>
        public DeviceGroup SelectedGroup
        {
            get => selectedGroup;
            set => this.RaiseAndSetIfChanged(ref selectedGroup, value);
        }

        private readonly Dictionary<AdvancedDeviceGroupControlViewModel, DeviceGroup> m_controlToPageMap;

        /// <summary>
        /// Maps a device to which advanced control panel it belongs to.
        /// </summary>
        private readonly Dictionary<IDevice, AdvancedDeviceGroupControlViewModel> m_deviceToControlMap;

        /// <summary>
        /// Maps a device group name to the advanced control panel.
        /// </summary>
        private readonly Dictionary<string, AdvancedDeviceGroupControlViewModel> m_nameToControlMap;

        /// <summary>
        /// Handles when a device is removed from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRemoved(object sender, IDevice device)
        {
            if (m_deviceToControlMap.ContainsKey(device))
            {
                var control = m_deviceToControlMap[device];
                control.RemoveDevice(device);

                m_deviceToControlMap.Remove(device);

                if (control.IsDeviceGroupEmpty)
                {
                    var page = m_controlToPageMap[control];
                    if (deviceGroups.Contains(page))
                    {
                        deviceGroups.Remove(page);
                    }
                    if (m_nameToControlMap.ContainsKey(page.Name))
                    {
                        m_nameToControlMap.Remove(page.Name);
                    }
                    m_controlToPageMap.Remove(control);
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
                var monitorAttribute = o as DeviceControlAttribute;
                if (monitorAttribute != null)
                {
                    IDeviceControl control = null;
                    if (monitorAttribute.ControlType != null)
                    {
                        control = Activator.CreateInstance(monitorAttribute.ControlType) as IDeviceControl;
                    }

                    AddDeviceControl(monitorAttribute.Category, device, control);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds a new device to the user interface.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="device"></param>
        /// <param name="deviceControl"></param>
        private void AddDeviceControl(string groupName, IDevice device, IDeviceControl deviceControl)
        {
            // Ensure that this always happens on the main thread
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                // Make sure group control exists.
                AdvancedDeviceGroupControlViewModel control = null;
                if (!m_nameToControlMap.ContainsKey(groupName))
                {
                    // Create the tab page
                    var page = new DeviceGroup(groupName);
                    m_nameToControlMap.Add(groupName, page.Content);
                    m_controlToPageMap.Add(page.Content, page);
                    deviceGroups.Add(page);
                }

                control = m_nameToControlMap[groupName];
                m_deviceToControlMap.Add(device, control);

                if (deviceControl == null)
                {
                    deviceControl = new DefaultUserDeviceViewModel();
                }

                deviceControl.Device = device;

                // Adds the device control to the group control
                control.AddDevice(device, deviceControl);
            });
        }
    }
}
