using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNet.Devices.Dashboard;

namespace LcmsNet.Devices
{
    public partial class AdvancedDeviceControl : Form
    {
        private readonly Dictionary<AdvancedDeviceGroupControl, TabPage> m_controlToPageMap;

        /// <summary>
        /// Maps a device to which advanced control panel it belongs to.
        /// </summary>
        private readonly Dictionary<IDevice, AdvancedDeviceGroupControl> m_deviceToControlMap;

        /// <summary>
        /// Maps a device group name to the advanced control panel.
        /// </summary>
        private readonly Dictionary<string, AdvancedDeviceGroupControl> m_nameToControlMap;

        public AdvancedDeviceControl()
        {
            InitializeComponent();

            m_controlToPageMap = new Dictionary<AdvancedDeviceGroupControl, TabPage>();
            m_nameToControlMap = new Dictionary<string, AdvancedDeviceGroupControl>();
            m_deviceToControlMap = new Dictionary<IDevice, AdvancedDeviceGroupControl>();

            classDeviceManager.Manager.DeviceAdded +=
                Manager_DeviceAdded;
            classDeviceManager.Manager.DeviceRemoved +=
                Manager_DeviceRemoved;

            FormClosing += AdvancedDeviceControl_FormClosing;
        }

        void AdvancedDeviceControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// Handles when a device is removed from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            if (m_deviceToControlMap.ContainsKey(device))
            {
                var control = m_deviceToControlMap[device];
                control.RemoveDevice(device);

                m_deviceToControlMap.Remove(device);

                if (control.IsDeviceGroupEmpty)
                {
                    var page = m_controlToPageMap[control];
                    if (m_advancedTabControl.TabPages.Contains(page))
                    {
                        m_advancedTabControl.TabPages.Remove(page);
                    }
                    if (m_nameToControlMap.ContainsKey(control.Name))
                    {
                        m_nameToControlMap.Remove(control.Name);
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
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            var type = device.GetType();

            var attributes = type.GetCustomAttributes(typeof (classDeviceControlAttribute), false);
            foreach (var o in attributes)
            {
                var monitorAttribute = o as classDeviceControlAttribute;
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
            // Make sure it exists.
            AdvancedDeviceGroupControl control = null;
            if (!m_nameToControlMap.ContainsKey(groupName))
            {
                control = new AdvancedDeviceGroupControl();
                control.Name = groupName;
                control.Dock = DockStyle.Fill;
                m_nameToControlMap.Add(groupName, control);

                // Create the tab page
                var page = new TabPage();
                page.Text = groupName;
                page.BackColor = Color.White;
                page.AutoScroll = true;
                page.Controls.Add(control);
                control.AutoScroll = true;
                m_controlToPageMap.Add(control, page);
                m_advancedTabControl.TabPages.Add(page);
            }
            control = m_nameToControlMap[groupName];
            m_deviceToControlMap.Add(device, control);

            if (deviceControl == null)
            {
                deviceControl = new DefaultUserDevice();
            }
            deviceControl.Device = device;

            // Adds the device to the control
            control.AddDevice(device, deviceControl);
        }

        /// <summary>
        /// Adds the currently selected device to the dashboard.
        /// </summary>
        private void AddDeviceToDeviceManager()
        {
            var controller = new DeviceAddController();
            var addForm = new formDeviceAddForm();
            addForm.Icon = Icon;

            var availablePlugins = controller.GetAvailablePlugins();
            addForm.AddPluginInformation(availablePlugins);
            addForm.Owner = this;


            var result = addForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                var plugins = addForm.GetSelectedPlugins();
                var failedDevices = controller.AddDevices(plugins, addForm.InitializeOnAdd);

                if (failedDevices != null && failedDevices.Count > 0)
                {
                    var display = new formFailedDevicesDisplay(failedDevices);
                    display.StartPosition = FormStartPosition.CenterScreen;
                    display.Icon = ParentForm.Icon;
                    display.ShowDialog();
                }
            }
        }

        private void mbutton_addDevice_Click(object sender, EventArgs e)
        {
            AddDeviceToDeviceManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}