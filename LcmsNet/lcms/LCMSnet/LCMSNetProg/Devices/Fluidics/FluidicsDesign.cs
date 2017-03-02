using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using FluidicsSDK;
using LcmsNet.Devices.Dashboard;
using LcmsNetDataClasses;

namespace LcmsNet.Devices.Fluidics
{
    public partial class FluidicsDesign : Form
    {
        #region Properties

        public bool DevicesLocked
        {
            get { return m_locked; }
            set
            {
                m_locked = value;
                if (m_locked)
                {
                    // Locked, we want the unlock button to be pressable to unlock.
                    mbutton_lock.Enabled = false;
                    mbutton_unlock.Enabled = true;
                    mbutton_addDevice.Enabled = false;
                    mbutton_removeDevice.Enabled = false;
                    mbutton_save.Enabled = false;
                    mbutton_loadHardware.Enabled = false;
                    mbutton_saveAs.Enabled = false;
                    mbutton_initialize.Enabled = false;
                    btnConnect.Enabled = false;
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                        "The designer has been locked.");
                }
                else
                {
                    // Unlocked, we want the lock button to be lockable.
                    mbutton_lock.Enabled = true;
                    mbutton_unlock.Enabled = false;
                    mbutton_addDevice.Enabled = true;
                    mbutton_removeDevice.Enabled = true;
                    mbutton_save.Enabled = true;
                    mbutton_loadHardware.Enabled = true;
                    mbutton_saveAs.Enabled = true;
                    mbutton_initialize.Enabled = true;
                    btnConnect.Enabled = true;
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                        "The designer has been un-locked.");
                }
            }
        }

        #endregion

        private void mbutton_loadHardware_Click(object sender, EventArgs e)
        {
            LoadHardware();
        }

        private void mbutton_save_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        private void mbutton_saveAs_Click(object sender, EventArgs e)
        {
            SaveHardwareAs();
        }

        private void controlDesign_Load(object sender, EventArgs e)
        {
        }

        private void FluidicsDesign_Paint(object sender, PaintEventArgs e)
        {
        }

        #region Members

        // fluidics moderator, controls access to fluidics managers and provides interface for working with them
        private readonly classFluidicsModerator m_fluidics_mod;

        /// <summary>
        /// Filter for file dialogs.
        /// </summary>
        private const string CONST_HARDWARE_CONFIG_FILTER = "Config files (*.ini)|*.ini|All files (*.*)|*.*";

        /// <summary>
        /// Default configuration for hardware.
        /// </summary>
        private const string CONST_DEFAULT_CONFIG_FILEPATH = "HardwareConfig.ini";

        private bool m_locked;
        private readonly ModelCheckReportViewer m_reporter;

        #endregion

        #region Methods

        public FluidicsDesign()
        {
            InitializeComponent();

            classDeviceManager.Manager.DeviceAdded +=
                new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceAdded);
            classDeviceManager.Manager.DeviceRemoved +=
                new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRemoved);
            classDeviceManager.Manager.DeviceRenamed += new DelegateDeviceUpdated(Manager_DeviceRenamed);
            m_fluidics_mod = classFluidicsModerator.Moderator;
            m_reporter = new ModelCheckReportViewer(m_fluidics_mod);
            m_reporter.Dock = DockStyle.Fill;
            tabControl1.TabPages["tabPageModelStatus"].Controls.Add(m_reporter);
            tabControl1.Selecting += tabControl1_Selecting;
            m_fluidics_mod.ScaleWorldView(1);
            m_fluidics_mod.ModelChanged += new classFluidicsModerator.ModelChange(FluidicsModelChanged);
            FormClosing += new FormClosingEventHandler(FluidicsDesign_FormClosing);
            Activated += FluidicsDesign_Activated;
        }

        void FluidicsDesign_Activated(object sender, EventArgs e)
        {
            var fd0 = tabControl1.Controls.Find("controlFluidicsControlDesigner", true);
            var fd = (controlFluidicsControl) fd0[0];
            fd.UpdateImage();
        }

        void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var page = sender as TabControl;
            var fd = page.Controls.Find("controlFluidicsControlDesigner", true);
            if (fd != null && e.TabPage == fd[0].Parent)
            {
                fd[0].Visible = true;
            }
            else if (fd != null && e.TabPage != fd[0].Parent)
            {
                fd[0].Visible = false;
            }
        }

        private void Manager_DeviceRenamed(object sender, IDevice device)
        {
            this.Refresh();
        }

        /// <summary>
        /// show error to user
        /// </summary>
        /// <param name="message">message to show user</param>
        private void ShowError(Exception ex)
        {
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, ex.Message, ex);
        }

        /// <summary>
        /// remove selected connections and devices
        /// </summary>
        private void RemoveSelected()
        {
            var devicesToRemoveFromDeviceManager = m_fluidics_mod.RemoveSelectedDevices();
            foreach (var device in devicesToRemoveFromDeviceManager)
            {
                classDeviceManager.Manager.RemoveDevice(device);
            }
            m_fluidics_mod.RemoveSelectedConnections();
        }

        /// <summary>
        ///  save the current fluidics design as a bitmap
        /// </summary>
        /// <returns>the image of the current fluidics design</returns>
        public Bitmap GetImage()
        {
            var r = m_fluidics_mod.GetBoundingBox();
            var fluidicsImage = new Bitmap(r.Width + 150, r.Height + 150);
            using (var g = Graphics.FromImage(fluidicsImage))
            {
                //create white background
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.White, r);
                float scale = 1;
                m_fluidics_mod.Render(g, 255, scale, Layer.Devices);
                m_fluidics_mod.Render(g, 255, scale, Layer.Ports);
                m_fluidics_mod.Render(g, 255, scale, Layer.Connections);
            }
            //fluidicsImage.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "testImage.bmp"));
            return fluidicsImage;
        }

        public List<Tuple<string, string, string>> ListDevicesAndStatus()
        {
            return m_fluidics_mod.ListDevicesAndStatus();
        }

        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration()
        {
            SaveConfiguration(CONST_DEFAULT_CONFIG_FILEPATH);
        }

        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration(string path)
        {
            var configuration = new classDeviceConfiguration();
            configuration.CartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);


            classDeviceManager.Manager.ExtractToPersistConfiguration(ref configuration);
            var connectionIds = new List<long>();
            //For each device, extract the X,Y position for persistence.
            foreach (var device in m_fluidics_mod.GetDevices())
            {
                configuration.AddSetting(device.IDevice.Name, "dashboard-x", device.Loc.X);
                configuration.AddSetting(device.IDevice.Name, "dashboard-y", device.Loc.Y);
                configuration.AddSetting(device.IDevice.Name, "State", (int) device.CurrentState);

                //for every port in a device, add any connection that is not internal to the device to the list of connections
                foreach (var port in device.Ports)
                {
                    foreach (var conn in port.Connections)
                    {
                        if (!connectionIds.Contains(conn.ID) && conn.InternalConnectionOf == null)
                        {
                            connectionIds.Add(conn.ID);
                            configuration.AddConnection(conn.ID.ToString(),
                                conn.P1.ID + ", " + conn.P2.ID + "," + conn.ConnectionStyle);
                        }
                    }
                }
            }

            var writer = new classINIDeviceConfigurationWriter();
            writer.WriteConfiguration(path, configuration);

            classApplicationLogger.LogMessage(0, string.Format("Saved device configuration to {0}.",
                path,
                CONST_DEFAULT_CONFIG_FILEPATH));
        }

        /// <summary>
        /// Loads the default hardware configuration.
        /// </summary>
        public void LoadConfiguration()
        {
            if (System.IO.File.Exists(CONST_DEFAULT_CONFIG_FILEPATH))
            {
                LoadConfiguration(CONST_DEFAULT_CONFIG_FILEPATH);
            }
        }

        /// <summary>
        /// Loads the hardware configuration from file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfiguration(string path)
        {
            m_fluidics_mod.BeginModelSuspension();
            var reader = new classINIDeviceConfigurationReader();
            var configuration = reader.ReadConfiguration(path);

            classDeviceManager.Manager.LoadPersistentConfiguration(configuration);

            foreach (var device in m_fluidics_mod.GetDevices())
            {
                try
                {
                    var settings = configuration.GetDeviceSettings(device.IDevice.Name);
                    if (settings.ContainsKey("dashboard-x") && settings.ContainsKey("dashboard-y"))
                    {
                        var x = Convert.ToInt32(settings["dashboard-x"]);
                        var y = Convert.ToInt32(settings["dashboard-y"]);
                        device.MoveBy(new Point(x, y));
                    }
                    if (settings.ContainsKey("State"))
                    {
                        var stateAsInt = Convert.ToInt32(settings["State"]);
                        device.ActivateState(stateAsInt);
                    }
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                        "Could not load the position or state of the device.", ex);
                }
            }
            var connections = configuration.GetConnections();
            foreach (var connection in connections.Keys)
            {
                var delimiter = new string[] {",", "\n"};
                var properties = connections[connection].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (properties.Length == 3)
                {
                    try
                    {
                        m_fluidics_mod.CreateConnection(properties[0].Trim(), properties[1].Trim(), properties[2].Trim());
                    }
                    catch (Exception)
                    {
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                            "Unable to create connection between " + properties[0] + " " + properties[1]);
                    }
                }
                else
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                        "Unable to create connection, specified ports invalid or missing connection style." + " port1: " +
                        properties[0] + " port2: " + properties[1] + " style: " + properties[2]);
                }
            }
            System.Diagnostics.Trace.WriteLine("Configuration Loaded");
            m_fluidics_mod.EndModelSuspension(true);
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

        /// <summary>
        /// Loads the hardware configuration.
        /// </summary>
        private void LoadHardware()
        {
            var deviceCount = classDeviceManager.Manager.DeviceCount;
            if (deviceCount > 0)
            {
                var result = MessageBox.Show("Do you want to clear the existing device configuration?",
                    "Clear Configuration", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = CONST_HARDWARE_CONFIG_FILTER;
                openFileDialog.FilterIndex = 0;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // The device manager sends us an event when it removes the devices.
                    // Since this is an event driven architecture, we dont have to worry about explicitly
                    // clearing our glyphs.
                    m_fluidics_mod.BeginModelSuspension();
                    classDeviceManager.Manager.ShutdownDevices(true);
                    m_fluidics_mod.EndModelSuspension(true);
                    // Then we actually load the next data
                    LoadConfiguration(openFileDialog.FileName);

                    classApplicationLogger.LogMessage(0, "Device configuration loaded from " + openFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Saves the hardware configuration to a new path.
        /// </summary>
        private void SaveHardwareAs()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = CONST_HARDWARE_CONFIG_FILTER;
                saveFileDialog.FilterIndex = 0;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveConfiguration(saveFileDialog.FileName);
                }
            }
        }

        #endregion

        #region Events

        void FluidicsDesign_FormClosing(object sender, FormClosingEventArgs e)
        {
            classDeviceManager.Manager.ShutdownDevices();
        }

        /// <summary>
        /// unlock button click event, unlock the fluidics designer controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_unlock_Click(object sender, EventArgs e)
        {
            DevicesLocked = false;
            controlFluidicsControlDesigner.DevicesLocked = DevicesLocked;
        }

        /// <summary>
        /// lock button click event, lock the fluidics designer controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_lock_Click(object sender, EventArgs e)
        {
            DevicesLocked = true;
            controlFluidicsControlDesigner.DevicesLocked = DevicesLocked;
        }

        /// <summary>
        /// Initialize devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_initialize_Click(object sender, EventArgs e)
        {
            var initializedCount = classDeviceManager.Manager.InitializedDeviceCount;

            var reinitialize = false;
            if (initializedCount > 0)
            {
                var result =
                    MessageBox.Show("Some devices are initialized already.  Do you want to re-initialize those?",
                        "Initialization", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    reinitialize = true;
                }
            }

            var failedDevices = classDeviceManager.Manager.InitializeDevices(reinitialize);
            if (failedDevices != null && failedDevices.Count > 0)
            {
                var display = new formFailedDevicesDisplay(failedDevices);
                display.StartPosition = FormStartPosition.CenterParent;
                display.Icon = ParentForm.Icon;
                display.ShowDialog();
            }
        }

        /// <summary>
        /// event handler for when a device is added to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            try
            {
                m_fluidics_mod.AddDevice(device);
            }
                //this should never, ever, happen
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// event handler for when a device is removed from the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            try
            {
                m_fluidics_mod.RemoveDevice(device);
            }
                //shouldn't ever happen.
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// event handler for when a fluidics model changes.
        /// </summary>
        private void FluidicsModelChanged()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new EventHandler(changeHandler));
            }
            else
            {
                changeHandler(this, new EventArgs());
            }
        }

        /// <summary>
        /// handle change in fluidics devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeHandler(object sender, EventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine("changeHandler sender: " + sender.ToString());
            controlFluidicsControlDesigner.UpdateImage();
            tabPageDesign.Refresh();
        }

        /// <summary>
        /// When btnConnect is clicked, attempt to create a new connection between two ports and update the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                m_fluidics_mod.CreateConnection();
                m_fluidics_mod.DeselectPorts();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex);
            }
            catch (ApplicationException ex)
            {
                ShowError(ex);
            }
                // shouldn't ever get to here
            catch (Exception ex)
            {
                ShowError(ex);
            }
            //this.panelFluidicsDesign.Invalidate();
        }

        /// <summary>
        /// when btnRemove is clicked, attempt to remove selected connections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                var areYouSure = MessageBox.Show("Are you sure you want to delete this device or connection?",
                    "Delete Device", MessageBoxButtons.YesNo);

                if (areYouSure == DialogResult.Yes)
                {
                    RemoveSelected();
                }
            }
                //shouldn't ever get this
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// add device button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDevice_Click(object sender, EventArgs e)
        {
            AddDeviceToDeviceManager();
        }

        #endregion
    }
}