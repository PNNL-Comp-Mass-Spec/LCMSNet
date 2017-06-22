using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class AdvancedDeviceControlPanelViewModel : ReactiveObject
    {
        public AdvancedDeviceControlPanelViewModel()
        {
            m_controlToPageMap = new Dictionary<AdvancedDeviceGroupControlViewModel, DeviceGroup>();
            m_nameToControlMap = new Dictionary<string, AdvancedDeviceGroupControlViewModel>();
            m_deviceToControlMap = new Dictionary<IDevice, AdvancedDeviceGroupControlViewModel>();

            classDeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            classDeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
        }

        public class DeviceGroup : IEquatable<DeviceGroup>
        {
            public string Name { get; private set; }
            public AdvancedDeviceGroupControlViewModel Content { get; private set; }

            public DeviceGroup(string name)
            {
                Name = name;
                Content = new AdvancedDeviceGroupControlViewModel();
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

        public ReactiveList<DeviceGroup> DeviceGroups => deviceGroups;

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
                    if (DeviceGroups.Contains(page))
                    {
                        DeviceGroups.Remove(page);
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

            var attributes = type.GetCustomAttributes(typeof(classDeviceControlAttribute), false);
            foreach (var o in attributes)
            {
                var monitorAttribute = o as classDeviceControlAttribute;
                if (monitorAttribute != null)
                {
                    IDeviceControlWpf control = null;
                    Type t = null;
                    if (monitorAttribute.ControlType != null)
                    {
                        t = monitorAttribute.ControlType;
                        var ns = t.Namespace;
                        var n = t.Name.Replace("control", "") + "ViewModel";
                        var fullname = ns + "." + n;
                        try
                        {
                            var t2 = t.Assembly.GetType(fullname, true);
                            var obj = Activator.CreateInstance(t2);
                            control = obj as IDeviceControlWpf;
                        }
                        catch (Exception)
                        {
                            // Silent failure is acceptable, but do report it to the error output
                            System.Diagnostics.Debug.WriteLine("Error: Could not find WPF version of control \"{0}\"; looked for \"{1}\", in assembly \"{2}\"", t.FullName, fullname, t.Assembly.FullName);
                        }
                    }
                    if (t != null && control == null)
                    {
                        //control = Activator.CreateInstance(monitorAttribute.ControlType) as IDeviceControlWpf;
                        var obj = Activator.CreateInstance(monitorAttribute.ControlType);
                        control = obj as IDeviceControlWpf;
                    }
                    // TODO: FIX: if (monitorAttribute.ControlType != null)
                    // TODO: FIX: {
                    // TODO: FIX:     control = Activator.CreateInstance(monitorAttribute.ControlType) as IDeviceControlWpf;
                    // TODO: FIX: }

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
        private void AddDeviceControl(string groupName, IDevice device, IDeviceControlWpf deviceControl)
        {
            // Make sure group control exists exists.
            AdvancedDeviceGroupControlViewModel control = null;
            if (!m_nameToControlMap.ContainsKey(groupName))
            {
                // Create the tab page
                var page = new DeviceGroup(groupName);
                m_nameToControlMap.Add(groupName, page.Content);
                m_controlToPageMap.Add(page.Content, page);
                DeviceGroups.Add(page);
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
        }
    }
}
