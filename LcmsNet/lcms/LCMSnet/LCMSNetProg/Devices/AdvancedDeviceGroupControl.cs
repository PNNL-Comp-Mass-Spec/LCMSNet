using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices
{
    public partial class AdvancedDeviceGroupControl : UserControl
    {
        private const int CONST_BUTTON_HEIGHT = 48;
        readonly Dictionary<Button, IDevice> m_buttonToDeviceMap;

        readonly Dictionary<IDevice, Button> m_deviceToButtonMap;
        readonly Dictionary<IDevice, IDeviceControl> m_deviceToControlMap;

        /// <summary>
        /// Selected device
        /// </summary>
        IDevice m_selectedDevice;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdvancedDeviceGroupControl()
        {
            InitializeComponent();

            m_deviceToControlMap = new Dictionary<IDevice, IDeviceControl>();
            m_buttonToDeviceMap = new Dictionary<Button, IDevice>();
            m_deviceToButtonMap = new Dictionary<IDevice, Button>();
            m_selectedDevicePanel.AutoScroll = true;
            classDeviceManager.Manager.DeviceRenamed += Manager_DeviceRenamed;

            m_selectedDevice = null;

            SelectedColor = Color.Red;
            NotSelectedColor = Color.Black;
        }


        /// <summary>
        /// Determines if there are any devices in this group.
        /// </summary>
        public bool IsDeviceGroupEmpty => m_deviceToButtonMap.Keys.Count < 1;

        public Color SelectedColor { get; set; }

        public Color NotSelectedColor { get; set; }

        /// <summary>
        /// Handles any device renames.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRenamed(object sender, IDevice device)
        {
            if (m_deviceToButtonMap.ContainsKey(device))
            {
                var button = m_deviceToButtonMap[device];
                button.Text = device.Name;
            }
        }

        private void DisableSelectedDevice(bool enabled)
        {
            mbutton_initialize.Enabled = enabled;
            mbutton_RenameDevice.Enabled = enabled;
        }

        /// <summary>
        /// Removes the selected device from the user interface.
        /// </summary>
        private void RemoveSelectedDevice()
        {
            // Remove the current selected control if one is selected
            if (m_selectedDevice != null)
            {
                var button = m_deviceToButtonMap[m_selectedDevice];
                button.FlatAppearance.BorderColor = NotSelectedColor;

                var currentSelected = m_deviceToControlMap[m_selectedDevice];
                var userControl = currentSelected as UserControl;
                if (userControl != null)
                {
                    m_selectedDevicePanel.Controls.Remove(userControl);
                }
                m_selectedDevice = null;
                DisableSelectedDevice(false);
            }
        }

        /// <summary>
        /// Selects the device for display.
        /// </summary>
        /// <param name="device"></param>
        private void SelectDevice(IDevice device)
        {
            m_selectedDevice = device;

            if (device == null)
            {
                DisableSelectedDevice(false);
                return;
            }

            // Find the user control
            var control = m_deviceToControlMap[device];
            var newUserControl = control as UserControl;
            if (newUserControl != null)
            {
                newUserControl.Dock = DockStyle.Fill;
                newUserControl.AutoScroll = true;
                m_selectedDevicePanel.Controls.Add(newUserControl);
            }

            m_deviceToButtonMap[m_selectedDevice].FlatAppearance.BorderColor = SelectedColor;
            mtextBox_NewDeviceName.Text = device.Name;
            DisableSelectedDevice(true);
        }


        void button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var device = m_buttonToDeviceMap[button];
            RemoveSelectedDevice();
            SelectDevice(device);
        }


        private void mbutton_initialize_Click(object sender, EventArgs e)
        {
            if (m_selectedDevice == null)
                return;

            var message = "";

            try
            {
                classDeviceManager.Manager.InitializeDevice(m_selectedDevice);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, message, ex);
                classApplicationLogger.LogError(0, ex.Message);
            }
            //bool wasOk = m_selectedDevice.Initialize(ref message);
            //if (!wasOk)
            //{
            //    classApplicationLogger.LogError(0, message);
            //}
        }

        private void mbutton_RenameDevice_Click(object sender, EventArgs e)
        {
            if (m_selectedDevice == null)
                return;

            RenameDevice(m_selectedDevice, mtextBox_NewDeviceName.Text);
        }

        private void RenameDevice(IDevice device, string newName)
        {
            //
            // It's the same name so don't mess with it
            //
            if (m_selectedDevice.Name == mtextBox_NewDeviceName.Text)
                return;

            DeviceManagerBridge.RenameDevice(device, newName);

            // Update the user interface with the new name
            var button = m_deviceToButtonMap[device];
            button.Text = device.Name;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.Black;
            button.FlatAppearance.BorderSize = 4;
            mtextBox_NewDeviceName.Text = device.Name;
        }

        private void clearError_Click(object sender, EventArgs e)
        {
            if (m_selectedDevice != null)
                m_selectedDevice.Status = enumDeviceStatus.Initialized;
        }

        #region Public Methods For Adding and Removing Devices for the UI

        public void RemoveDevice(IDevice device)
        {
            m_selectedDevicePanel.SuspendLayout();

            if (m_deviceToButtonMap.ContainsKey(device))
            {
                // Find the button to remove
                var button = m_deviceToButtonMap[device];
                button.Click -= button_Click;


                // Remove from UI
                m_deviceButtonPanel.Controls.Remove(button);

                // Remove if currently selected
                if (m_selectedDevice == device)
                    RemoveSelectedDevice();

                // Remove from maps
                m_buttonToDeviceMap.Remove(button);
                m_deviceToButtonMap.Remove(device);
                m_deviceToControlMap.Remove(device);
            }
            m_selectedDevicePanel.ResumeLayout();
        }

        /// <summary>
        /// Adds the device to the panel for us.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="control"></param>
        public void AddDevice(IDevice device, IDeviceControl control)
        {
            var button = new Button
            {
                Text = device.Name,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };

            button.Size = new Size(button.Width, CONST_BUTTON_HEIGHT);
            button.Font = new Font(button.Font.FontFamily, 12, FontStyle.Bold);
            button.Click += button_Click;

            m_buttonToDeviceMap.Add(button, device);
            m_deviceToButtonMap.Add(device, button);
            m_deviceButtonPanel.Controls.Add(button);
            m_deviceToControlMap.Add(device, control);

            if (m_deviceButtonPanel.Controls.Count == 1)
            {
                SelectDevice(device);
            }
        }

        #endregion
    }
}