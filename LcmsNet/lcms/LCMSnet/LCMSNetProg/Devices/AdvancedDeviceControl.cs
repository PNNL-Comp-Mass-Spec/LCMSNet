using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNet.Devices.Dashboard;

namespace LcmsNet.Devices
{
    public partial class AdvancedDeviceControl : Form
    {
        /// <summary>
        /// Maps a device to which advanced control panel it belongs to.
        /// </summary>
        private Dictionary<IDevice, AdvancedDeviceGroupControl> m_deviceToControlMap;
        /// <summary>
        /// Maps a device group name to the advanced control panel.
        /// </summary>
        private Dictionary<string, AdvancedDeviceGroupControl> m_nameToControlMap;
        private Dictionary<AdvancedDeviceGroupControl, TabPage> m_controlToPageMap;

        public AdvancedDeviceControl()
        {
            InitializeComponent();

            m_controlToPageMap      = new Dictionary<AdvancedDeviceGroupControl,TabPage>();
            m_nameToControlMap      = new Dictionary<string, AdvancedDeviceGroupControl>();
            m_deviceToControlMap    = new Dictionary<IDevice, AdvancedDeviceGroupControl>();

            classDeviceManager.Manager.DeviceAdded   += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceAdded);
            classDeviceManager.Manager.DeviceRemoved += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRemoved);

            FormClosing += new FormClosingEventHandler(AdvancedDeviceControl_FormClosing);
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
                AdvancedDeviceGroupControl control = m_deviceToControlMap[device];
                control.RemoveDevice(device);

                m_deviceToControlMap.Remove(device);

                if (control.IsDeviceGroupEmpty)
                {
                    TabPage page = m_controlToPageMap[control];
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
            
            Type type = device.GetType();

            object[] attributes = type.GetCustomAttributes(typeof(classDeviceControlAttribute), false);
            foreach (object o in attributes)
            {
                classDeviceControlAttribute monitorAttribute = o as classDeviceControlAttribute;
                if (monitorAttribute != null)
                {                    
                    IDeviceControl control  = null;
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
                control         = new AdvancedDeviceGroupControl();
                control.Name    = groupName;
                control.Dock    = DockStyle.Fill;
                m_nameToControlMap.Add(groupName, control);
                
                // Create the tab page
                TabPage page    = new TabPage();
                page.Text       = groupName;
                page.BackColor  = Color.White;
                page.AutoScroll = true;
                page.Controls.Add(control);
                control.AutoScroll = true;
                m_controlToPageMap.Add(control, page);
                m_advancedTabControl.TabPages.Add(page);
            }
            control         = m_nameToControlMap[groupName];
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
            DeviceAddController controller = new DeviceAddController();
            formDeviceAddForm addForm = new formDeviceAddForm();
            addForm.Icon = Icon;

            List<classDevicePluginInformation> availablePlugins = controller.GetAvailablePlugins();
            addForm.AddPluginInformation(availablePlugins);
            addForm.Owner = this;


            DialogResult result = addForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<classDevicePluginInformation> plugins = addForm.GetSelectedPlugins();
                List<classDeviceErrorEventArgs> failedDevices = controller.AddDevices(plugins, addForm.InitializeOnAdd);

                if (failedDevices != null && failedDevices.Count > 0)
                {
                    formFailedDevicesDisplay display = new formFailedDevicesDisplay(failedDevices);
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
