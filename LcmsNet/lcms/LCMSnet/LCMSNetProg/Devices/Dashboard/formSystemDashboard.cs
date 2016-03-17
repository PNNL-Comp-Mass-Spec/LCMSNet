using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNet.Devices.Dashboard;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Displays the status of each device.
    /// </summary>
    public partial class formSystemDashboard : Form
    {
        #region Constants
        /// <summary>
        /// Constant defining the spacing (or padding) between two device glyphs.
        /// </summary>
        private const int CONST_GLYPH_PADDING = 5;
        /// <summary>
        /// Padding around control when mouse hovered over a dashboard object.
        /// </summary>
        private const int CONST_HIGHLIGHT_PADDING = 2;
        /// <summary>
        /// Height of the drag panel for a given device glyph.
        /// </summary>
        private const int CONST_DEVICE_DRAG_PANEL_HEIGHT = 20;
        /// <summary>
        /// Default configuration for hardware.
        /// </summary>
        private const string CONST_DEFAULT_CONFIG_FILEPATH = "HardwareConfig.ini";
        /// <summary>
        /// Filter for file dialogs.
        /// </summary>
        private const string CONST_HARDWARE_CONFIG_FILTER = "Config files (*.ini)|*.ini|All files (*.*)|*.*";
        /// <summary>
        /// Size of save panel when not displayed.
        /// </summary>
        private const int CONST_HEIGHT_NOT_DISPLAYED = 16;
        /// <summary>
        /// Size of save panel when not displayed.
        /// </summary>
        private const int CONST_HEIGHT_PANEL_PADDING = 8;
        /// <summary>
        /// Size of save panel when displayed.
        /// </summary>
        private readonly int mint_displayedHeight;
        /// <summary>
        /// Amount of the devices panel to display.
        /// </summary>
        private readonly int mint_devicesDisplayedHeight;
        #endregion

        #region Members
        /// <summary>
        /// Maps the device to its displaying control.
        /// </summary>
        Dictionary<IDevice, classDeviceControlData> mdict_deviceControlMap;
        /// <summary>
        /// Flag indicating whether the user is draggin a control around.
        /// </summary>
        private bool mbool_draggingGlyph;
        /// <summary>
        /// Glyph that is currently being drug.
        /// </summary>
        private IDeviceGlyph mobj_movingGlyph;
        /// <summary>
        /// Glyph that has been selected for some action.
        /// </summary>
        private IDeviceGlyph mobj_selectedGlyph;
        /// <summary>
        /// Maps the user control an event occurred on to the device glyph of question.
        /// </summary>
        private Dictionary<UserControl, IDevice> mdict_userControlToDeviceMap;
        /// <summary>
        /// Starting point where user control dragging started.
        /// </summary>
        private Point mobj_dragStartPoint;
        /// <summary>
        /// Flag indicating if the designer is locked.
        /// </summary>
        private bool mbool_locked;
        /// <summary>
        /// Offset amount in X when adding a new glyph.
        /// </summary>
        private int mint_xOffset;
        /// <summary>
        /// Offset amount in Y when adding a new glyph.
        /// </summary>
        private int mint_yOffset;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public formSystemDashboard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();

            this.Text += " - " + CONST_DEFAULT_CONFIG_FILEPATH;

            mdict_deviceControlMap       = new Dictionary<IDevice, classDeviceControlData>();
            mdict_userControlToDeviceMap = new Dictionary<UserControl, IDevice>();

            classDeviceManager.Manager.DeviceAdded          += new DelegateDeviceUpdated(Manager_DeviceAdded);
            classDeviceManager.Manager.DeviceRemoved        += new DelegateDeviceUpdated(Manager_DeviceRemoved);
            classDeviceManager.Manager.PluginsLoaded        += new EventHandler(Manager_PluginsLoaded);
            mcontrol_dashboardPanel.MouseMove               += new MouseEventHandler(formSystemDashboard_MouseMove);
            mcontrol_dashboardPanel.MouseLeave              += new EventHandler(formSystemDashboard_MouseLeave);
            mcontrol_dashboardPanel.KeyUp                   += new KeyEventHandler(formSystemDashboard_KeyUp);
            mcontrol_dashboardPanel.MouseUp                 += new MouseEventHandler(formSystemDashboard_MouseUp);

            TurnOffDrag();
            DeselectGlyph();

            // This size is determined by the button height and placement for aesthics.
            //          *2 is to provide padding on top and bottom of button when we display it.
            mint_displayedHeight        = mbutton_loadHardware.Height + CONST_HEIGHT_PANEL_PADDING*2;
            //          Using the hardware button is just a way to add padding to the display height so we can see the bottom of the button
            //          when it's displayed.
            mint_devicesDisplayedHeight = mbutton_addDevice.Top + mbutton_addDevice.Height + CONST_HEIGHT_PANEL_PADDING;

            FormClosing += new FormClosingEventHandler(formSystemDashboard_FormClosing);
        }

        void formSystemDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            classDeviceManager.Manager.ShutdownDevices();
        }



        #region Helper Methods
        /// <summary>
        /// Invalidates the area around the user control on the dashboard for highlighting.
        /// </summary>
        /// <param name="control"></param>
        private void InvalidateUserControlArea(UserControl control)
        {
            Point location = control.Location;
            location.X -= CONST_HIGHLIGHT_PADDING;
            location.Y -= CONST_HIGHLIGHT_PADDING;

            Invalidate(new Rectangle(location.X, location.Y, control.Width + 2*CONST_HIGHLIGHT_PADDING, control.Height + 2*CONST_HIGHLIGHT_PADDING));
        }
        /// <summary>
        /// Retreives the user control as a glyph if it is a valid dashboard object and not a built-in control.
        /// </summary>
        /// <param name="control">Control that was clicked.</param>
        /// <returns>Glyph of the device that displays succinct status.</returns>
        private IDeviceGlyph GetGlyphFromDashboardControl(UserControl control)
        {
            IDeviceGlyph glyph = null;

            // First we need to retrieve the device because
            // we need it to map back to the glyph we want.
            IDevice device = GetDeviceFromDashboardControl(control);
            if (device != null)
            {
                // Now we can map out the glyph (dashboard device interface)
                if (mdict_deviceControlMap.ContainsKey(device))
                {
                    classDeviceControlData data = mdict_deviceControlMap[device];
                    glyph = data.DashboardGlyph;
                }
            }
            return glyph;
        }
        /// <summary>
        /// Retrieves a device given the user control on the dashboard that was clicked.
        /// </summary>
        /// <param name="control">Control that was clicked.</param>
        /// <returns>Related underlying device.</returns>
        private IDevice GetDeviceFromDashboardControl(UserControl control)
        {
            IDevice device = null;
            if (mdict_userControlToDeviceMap.ContainsKey(control))
            {
                device = mdict_userControlToDeviceMap[control];
            }
            return device;
        }
        /// <summary>
        /// Retrieves the advanced device control (not the one of the dashboard) given the dashboard control.
        /// </summary>
        /// <param name="control">Control that was clicked.</param>
        /// <returns>Advanced device control with all of the device properties.</returns>
        private IDeviceControl GetDeviceControlFromDashboardControl(UserControl control)
        {
            IDeviceControl deviceControl = null;
            // First we need to retrieve the device because
            // we need it to map back to the glyph we want.
            IDevice device = GetDeviceFromDashboardControl(control);
            if (device != null)
            {
                // Now we can map out the glyph (dashboard device interface)
                if (mdict_deviceControlMap.ContainsKey(device))
                {
                    classDeviceControlData data = mdict_deviceControlMap[device];
                    deviceControl = data.DeviceControl;
                }
            }
            return deviceControl;
        }
        /// <summary>
        /// Adds the device controls and places them onto the form.
        /// </summary>
        /// <param name="controls"></param>
        private void AddDeviceControls(IDevice device, IDeviceControl control, IDeviceGlyph glyph, classDeviceControlAttribute attribute)
        {
            int x, y;
            x = CONST_GLYPH_PADDING + mint_xOffset;
            y = CONST_GLYPH_PADDING + mint_yOffset;

            mint_xOffset += glyph.UserControl.Width  / 2;
            mint_yOffset += glyph.UserControl.Height / 2;

            // Reset if we are too far over.
            if (mint_yOffset > Height - CONST_GLYPH_PADDING)
            {
                mint_yOffset = CONST_GLYPH_PADDING;
            }
            if (mint_xOffset > Width - CONST_GLYPH_PADDING)
            {
                mint_xOffset = CONST_GLYPH_PADDING;
            }

            x = Math.Max(CONST_GLYPH_PADDING, x);
            y = Math.Max(CONST_GLYPH_PADDING, y);

            control.Device = device;
            glyph.RegisterDevice(device);

            glyph.UserControl.Location     = new Point(x, y);

            // Setup the drag panel.
            controlDeviceStatusDisplay dragPanel = new controlDeviceStatusDisplay();
            dragPanel.Height                     = CONST_DEVICE_DRAG_PANEL_HEIGHT;
            dragPanel.Dock                       = DockStyle.Top;
            dragPanel.KeyUp                     += new KeyEventHandler(formSystemDashboard_KeyUp);
            dragPanel.MouseDown                 += new MouseEventHandler(UserControl_MouseDown);
            dragPanel.MouseUp                   += new MouseEventHandler(UserControl_MouseUp);
            dragPanel.ShowDetailsWindow         += new EventHandler(dragPanel_ShowDetailsWindow);
            dragPanel.MouseMove                 += new MouseEventHandler(UserControl_MouseMove);
            dragPanel.ErrorIndicatorClicked     += new EventHandler(dragPanel_ErrorIndicatorClicked);
            dragPanel.Device                     = device;
            device.Error                        += new EventHandler<classDeviceErrorEventArgs>(dragPanel.DeviceError);
            device.StatusUpdate                 += new EventHandler<classDeviceStatusEventArgs>(dragPanel.DeviceStatusUpdate);

            glyph.UserControl.Size               = new Size(glyph.UserControl.Width, glyph.UserControl.Height + dragPanel.Height);
            glyph.UserControl.Controls.Add(dragPanel);
            glyph.ZOrder                         = mdict_userControlToDeviceMap.Values.Count;
            glyph.UserControl.BringToFront();

            dragPanel.SendToBack();
            glyph.UserControl.MouseEnter        += new EventHandler(UserControl_MouseEnter);
            glyph.UserControl.KeyUp             += new KeyEventHandler(formSystemDashboard_KeyUp);


            // This maps the control we render on the dashboard to the device
            mdict_userControlToDeviceMap.Add(glyph.UserControl, device);
            // This maps the device to the glyph (summary data) and device control (for the advanced window)
            mdict_deviceControlMap.Add(device, new classDeviceControlData(control, glyph, attribute));

            mcontrol_dashboardPanel.Controls.Add(glyph.UserControl);
        }

        void dragPanel_ErrorIndicatorClicked(object sender, EventArgs e)
        {
            controlDeviceStatusDisplay display = sender as controlDeviceStatusDisplay;
            if (display != null)
            {
                IDevice device = display.Device;

                if (device != null && device.Status == enumDeviceStatus.Error)
                {
                    using (formResolveDeviceStatus form = new formResolveDeviceStatus())
                    {
                        form.StartPosition = FormStartPosition.CenterScreen;
                        if (DialogResult.OK == form.ShowDialog())
                        {
                            device.Status = form.Status;
                        }
                    }
                }
            }
        }
        void dragPanel_ShowDetailsWindow(object sender, EventArgs e)
        {
            Control childControl = sender as Control;
            if (childControl == null)
                return;

            UserControl control = childControl.Parent as UserControl;

            if (control != null)
            {
                IDeviceControl deviceControl = GetDeviceControlFromDashboardControl(control);
                if (deviceControl != null)
                {
                    UserControl deviceUserControl = deviceControl as UserControl;
                    if (deviceUserControl == null)
                        return;

                    /// Open the administrative control window.
                    Form deviceForm          = new Form();
                    deviceForm.Size          = new Size(deviceUserControl.Width + 10, deviceUserControl.Height + 10);
                    deviceForm.Controls.Add(deviceUserControl);
                    deviceUserControl.Dock   = DockStyle.Fill;
                    deviceForm.Icon          = Icon;
                    deviceForm.StartPosition = FormStartPosition.CenterParent;
                    deviceForm.Text          = "Device: " + deviceControl.Name;
                    deviceForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Adds the currently selected device to the dashboard.
        /// </summary>
        private void AddDeviceToDeviceManager()
        {
            LcmsNet.Devices.Dashboard.formDeviceAddForm addForm = new formDeviceAddForm();
            addForm.Icon                                        = Icon;
            classDeviceManager manager                          = classDeviceManager.Manager;
            List<classDevicePluginInformation> availablePlugins = new List<classDevicePluginInformation>();
            foreach (classDevicePluginInformation plugin in manager.AvailablePlugins)
            {
                availablePlugins.Add(plugin);
            }
            addForm.AddPluginInformation(availablePlugins);
            addForm.Owner       = this;
            DialogResult result = addForm.ShowDialog();
            if (result == DialogResult.OK)
            {

                bool initializeOnAdd                          = addForm.InitializeOnAdd;
                List<classDeviceErrorEventArgs> failedDevices = new List<classDeviceErrorEventArgs>();
                List<classDevicePluginInformation> plugins    = addForm.GetSelectedPlugins();

                foreach (classDevicePluginInformation plugin in plugins)
                {
                    if (plugin != null)
                    {
                        try
                        {
                            bool wasAdded = classDeviceManager.Manager.AddDevice(plugin, initializeOnAdd);
                            if (!wasAdded)
                            {
                                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Could not create the selected device: " + plugin.DeviceAttribute.Name);
                            }
                            else
                            {
                                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "A " + plugin.DeviceAttribute.Name + " device was added.");
                            }
                        }
                        catch(classDeviceInitializationException ex)
                        {
                            failedDevices.Add(ex.ErrorDetails);
                            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, string.Format("The device {0} was added but was not initialized properly. Error Message: {1}", ex.ErrorDetails.Device.Name, ex.ErrorDetails.Error));
                        }
                        catch(Exception ex)
                        {
                            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Could not create the device " + plugin.DeviceAttribute.Name + " " + ex.Message, ex);
                        }
                    }
                }
                if (failedDevices != null && failedDevices.Count > 0)
                {
                    formFailedDevicesDisplay display = new formFailedDevicesDisplay(failedDevices);
                    display.StartPosition = FormStartPosition.CenterScreen;
                    display.Icon = ParentForm.Icon;
                    display.ShowDialog();
                }
            }
        }
        /// <summary>
        /// Sets current glyph as parameter.
        /// </summary>
        /// <param name="glyph">Glyph to select.</param>
        private void SelectGlyph(IDeviceGlyph glyph)
        {
            if (DevicesLocked)
                return;

            mobj_selectedGlyph = glyph;
            if (mobj_selectedGlyph != null)
            {
                if (!mbool_locked)
                {
                    mbutton_removeDevice.Enabled = true;
                }
                mobj_selectedGlyph.UserControl.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        /// <summary>
        /// Sets the currently selected glyph to null.
        /// </summary>
        private void DeselectGlyph()
        {
            if (mobj_selectedGlyph != null)
            {
                mobj_selectedGlyph.UserControl.BorderStyle = BorderStyle.None;
                if (!mbool_locked)
                {
                    mbutton_removeDevice.Enabled = false;
                }
            }
            mobj_selectedGlyph = null;
        }
        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration(string path)
        {
            // classDeviceManager.Manager.SaveConfiguration(path);
            classDeviceConfiguration configuration = new classDeviceConfiguration();
            configuration.CartName      = LcmsNetDataClasses.classLCMSSettings.GetParameter("CartName");

            classDeviceManager.Manager.ExtractToPersistConfiguration(ref configuration);

            foreach (IDevice device in mdict_deviceControlMap.Keys)
            {
                IDeviceGlyph glyph = mdict_deviceControlMap[device].DashboardGlyph;
                configuration.AddSetting(device.Name, "dashboard-x", glyph.UserControl.Location.X);
                configuration.AddSetting(device.Name, "dashboard-y", glyph.UserControl.Location.Y);
            }
            classINIDeviceConfigurationWriter writer = new classINIDeviceConfigurationWriter();
            writer.WriteConfiguration(path, configuration);

            classApplicationLogger.LogMessage(0, string.Format("Saved device configuration to {0}.",
                                                                path,
                                                                CONST_DEFAULT_CONFIG_FILEPATH));
        }
        /// <summary>
        /// Loads the hardware configuration.
        /// </summary>
        private void LoadHardware()
        {
            int deviceCount = classDeviceManager.Manager.DeviceCount;
            if (deviceCount > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to clear the existing device configuration?", "Clear Configuration", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = CONST_HARDWARE_CONFIG_FILTER;
                openFileDialog.FilterIndex = 0;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // The device manager sends us an event when it removes the devices.
                    // Since this is an event driven architecture, we dont have to worry about explicitly
                    // clearing our glyphs.
                    classDeviceManager.Manager.ShutdownDevices(true);

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
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter           = CONST_HARDWARE_CONFIG_FILTER;
                saveFileDialog.FilterIndex      = 0;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveConfiguration(saveFileDialog.FileName);
                }
            }
        }
        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration()
        {
            SaveConfiguration(CONST_DEFAULT_CONFIG_FILEPATH);
        }
        /// <summary>
        /// Loads the hardware configuration from file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfiguration(string path)
        {
            classINIDeviceConfigurationReader reader = new classINIDeviceConfigurationReader();
            classDeviceConfiguration configuration   = reader.ReadConfiguration(path);
            classDeviceManager.Manager.LoadPersistentConfiguration(configuration);

            foreach (IDevice device in mdict_deviceControlMap.Keys)
            {
                try
                {
                    Dictionary<string, object> settings = configuration.GetDeviceSettings(device.Name);
                    if (settings.ContainsKey("dashboard-x") && settings.ContainsKey("dashboard-y"))
                    {
                        int x = Convert.ToInt32(settings["dashboard-x"]);
                        int y = Convert.ToInt32(settings["dashboard-y"]);

                        classDeviceControlData data              = mdict_deviceControlMap[device];
                        data.DashboardGlyph.UserControl.Location = new Point(x, y);
                    }
                }
                catch(Exception ex)
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "Could not load the position of the device glyph.", ex);
                }
            }
            Refresh();
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
        #endregion

        #region Device Glyph Dragging and Rendering
        /// <summary>
        /// Disables the dragging of a glyph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formSystemDashboard_MouseLeave(object sender, EventArgs e)
        {
            TurnOffDrag();
        }
        /// <summary>
        /// Handles moving the device glyph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formSystemDashboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbool_draggingGlyph && !DevicesLocked)
            {
                MoveGlyph(mobj_movingGlyph, e.Location, mobj_dragStartPoint);
            }
        }
        /// <summary>
        /// Handles rendering the control as it moves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbool_draggingGlyph && !DevicesLocked)
            {
                MoveGlyph(mobj_movingGlyph, e.Location, mobj_dragStartPoint);
            }
        }
        /// <summary>
        /// Signals the movement of a glyph off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (mbool_draggingGlyph && !DevicesLocked)
            {
                MoveGlyph(mobj_movingGlyph, e.Location, mobj_dragStartPoint);
            }
            TurnOffDrag();
        }
        /// <summary>
        /// Signals the movement of a glyph on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (DevicesLocked)
            {
                return;
            }
            Control childControl = sender as Control;

            if (childControl == null)
                return;
            UserControl control = childControl.Parent as UserControl;

            // Display the control information.
            if (e.Button == MouseButtons.Right)
            {
                // This used to show the control window.
            }
            else
            {
                // Start the moving process.
                mbool_draggingGlyph = true;
                if (control != null)
                {
                    if (mdict_userControlToDeviceMap.ContainsKey(control))
                    {
                        mobj_dragStartPoint     = e.Location;
                        IDeviceGlyph glyph      = GetGlyphFromDashboardControl(control);
                        if (glyph != null)
                        {
                            DeselectGlyph();    // Deselect the old glyph
                            SelectGlyph(glyph); // Select the new one.
                            Cursor              = Cursors.Hand;
                            mobj_movingGlyph    = glyph;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Highlights the control in question.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControl_MouseEnter(object sender, EventArgs e)
        {
            if (DevicesLocked)
            {
                return;
            }

            UserControl control = sender as UserControl;
            if (control != null)
            {
                control.BorderStyle  = BorderStyle.FixedSingle;
            }
        }
        #endregion

        #region Rendering and Layout
        /// <summary>
        /// Disables dragging of the glyph.
        /// </summary>
        private void TurnOffDrag()
        {
            mbool_draggingGlyph     = false;
            mobj_movingGlyph        = null;
            Cursor                  = Cursors.Arrow;
        }
        /// <summary>
        /// Moves the directed glyph to the target location.
        /// </summary>
        /// <param name="glyph">Glyph to move.</param>
        /// <param name="target">Target location.</param>
        /// <param name="offset">Offset within glyph for centering purposes.</param>
        private void MoveGlyph(IDeviceGlyph glyph, Point target, Point offset)
        {
            if (mbool_draggingGlyph && glyph != null)
            {
                Point devicePoint   = glyph.UserControl.Location;
                target.X            = devicePoint.X + target.X - offset.X;
                target.Y            = devicePoint.Y + target.Y - offset.Y;

                // Enforce the boundary before drawing.
                target.X = Math.Max(Math.Min(target.X, Width - CONST_GLYPH_PADDING), CONST_GLYPH_PADDING);
                target.Y = Math.Max(Math.Min(target.Y, Height - CONST_GLYPH_PADDING), CONST_GLYPH_PADDING);

                Invalidate(new Rectangle(glyph.UserControl.Location.X - CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Location.Y - CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Width  + 2*CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Height + 2*CONST_HIGHLIGHT_PADDING));
                glyph.UserControl.Location = target;
                Invalidate(new Rectangle(glyph.UserControl.Location.X - CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Location.Y - CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Width  + 2*CONST_HIGHLIGHT_PADDING,
                                         glyph.UserControl.Height + 2*CONST_HIGHLIGHT_PADDING));
                Update();
            }
        }
        #endregion

        #region Device Manager Event Handlers and Utilities
        /// <summary>
        /// Handles when new plug-ins (or older ones) are loaded (re-loaded).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Manager_PluginsLoaded(object sender, EventArgs e)
        {
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Loaded plugins.");
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Removed " + device.Name);
            if (mdict_deviceControlMap.ContainsKey(device))
            {
                // 1. Retrieve the dashboard user control data
                // 2. Remove the device key
                // 3. Then remove the dashboard user control information as well.
                classDeviceControlData deviceData = mdict_deviceControlMap[device];
                mdict_deviceControlMap.Remove(device);

                if (deviceData == null || deviceData.DashboardGlyph == null || deviceData.DashboardGlyph.UserControl == null)
                {
                    return;
                }

                if (mdict_userControlToDeviceMap.ContainsKey(deviceData.DashboardGlyph.UserControl))
                {
                    mdict_userControlToDeviceMap.Remove(deviceData.DashboardGlyph.UserControl);
                    mcontrol_dashboardPanel.Controls.Remove(deviceData.DashboardGlyph.UserControl);
                    deviceData.DashboardGlyph.DeRegisterDevice();
                }
            }
        }
        /// <summary>
        /// Updates tracking references with device monitoring objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Added " + device.Name);
            Type type                   = device.GetType();

            object [] attributes = type.GetCustomAttributes(typeof(classDeviceControlAttribute), false);
            foreach (object o in attributes)
            {
                classDeviceControlAttribute monitorAttribute = o as classDeviceControlAttribute;
                if (monitorAttribute != null)
                {
                   IDeviceGlyph     glyph   = null;
                   IDeviceControl   control = null;
                   if (monitorAttribute.ControlType != null)
                   {
                        control = Activator.CreateInstance(monitorAttribute.ControlType) as IDeviceControl;
                   }
                   if (monitorAttribute.GlyphType != null)
                   {
                        glyph = Activator.CreateInstance(monitorAttribute.GlyphType) as IDeviceGlyph;
                   }

                   AddDeviceControls(device, control, glyph, monitorAttribute);
                   break;
                }
            }
        }
        #endregion

        #region User Control Event Handlers
        /// <summary>
        /// Adds a device to the dashboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_addDevice_Click(object sender, EventArgs e)
        {
            AddDeviceToDeviceManager();
        }
        /// <summary>
        /// Disables selecting control.
        /// </summary>
        /// <param name="sender">Form.</param>
        /// <param name="e">Event data.</param>
        void formSystemDashboard_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    DeselectGlyph();
                    break;
                case Keys.Delete:
                    RemoveSelectedDevice();
                    break;
                case Keys.U:
                    if (e.Modifiers == Keys.Control)
                    {
                        DevicesLocked = false;
                    }
                    break;
                case Keys.L:
                    if (e.Modifiers == Keys.Control)
                    {
                        DevicesLocked = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Removes a selected device from the dashboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_removeDevice_Click(object sender, EventArgs e)
        {
            RemoveSelectedDevice();
        }

        private void RemoveSelectedDevice()
        {
            if (mbool_locked == true)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, "Cannot remove device when dashboard is locked.");
                return;
            }

            if (mobj_selectedGlyph == null)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, "No device selected to remove.");
                return;
            }

            IDevice device = GetDeviceFromDashboardControl(mobj_selectedGlyph.UserControl);
            if (device != null)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to remove the " + device.Name + "?", "Remove Device", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    classDeviceManager.Manager.RemoveDevice(device);
                    DeselectGlyph();
                }
            }
        }
        /// <summary>
        /// Handles events when the form is clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formSystemDashboard_MouseUp(object sender, MouseEventArgs e)
        {
            DeselectGlyph();

            if (e.Button == MouseButtons.Right)
            {
                mcontextMenu_arrange.Show(Cursor.Position);
            }
        }
        /// <summary>
        /// Initialize devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_initialize_Click(object sender, EventArgs e)
        {
            int initializedCount = classDeviceManager.Manager.InitializedDeviceCount;

            bool reinitialize = false;
            if (initializedCount > 0)
            {
                DialogResult result = MessageBox.Show("Some devices are initialized already.  Do you want to re-initialize those?", "Initialization", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    reinitialize = true;
                }
            }

            List<classDeviceErrorEventArgs> failedDevices = classDeviceManager.Manager.InitializeDevices(reinitialize);
            if (failedDevices != null && failedDevices.Count > 0)
            {
                formFailedDevicesDisplay display = new formFailedDevicesDisplay(failedDevices);
                display.StartPosition            = FormStartPosition.CenterParent;
                display.Icon                     = ParentForm.Icon;
                display.ShowDialog();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemSaveAsDefault_Click(object sender, EventArgs e)
        {
            SaveConfiguration(CONST_DEFAULT_CONFIG_FILEPATH);
        }
        private void mbutton_unlock_Click(object sender, EventArgs e)
        {
            DevicesLocked = false;
        }
        private void mbutton_lock_Click(object sender, EventArgs e)
        {
            DevicesLocked = true;
        }
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
        #endregion

        #region Properties
        public bool DevicesLocked
        {
            get
            {
                return mbool_locked;
            }
            set
            {
                mbool_locked = value;
                if (mbool_locked)
                {
                    // Locked, we want the unlock button to be pressable to unlock.
                    mbutton_lock.Enabled         = false;
                    mbutton_unlock.Enabled       = true;
                    mbutton_addDevice.Enabled    = false;
                    mbutton_removeDevice.Enabled = false;
                    mbutton_save.Enabled         = false;
                    mbutton_loadHardware.Enabled = false;
                    mbutton_saveAs.Enabled = false;
                    mbutton_initialize.Enabled = false;
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER, "The dashboard has been locked.");

                    foreach(classDeviceControlData data in mdict_deviceControlMap.Values)
                    {
                        data.DashboardGlyph.UserControl.Enabled = false;
                    }

                }
                else
                {
                    // Unlocked, we want the lock button to be lockable.
                    mbutton_lock.Enabled         = true;
                    mbutton_unlock.Enabled       = false;
                    mbutton_addDevice.Enabled    = true;
                    mbutton_removeDevice.Enabled = true;
                    mbutton_save.Enabled         = true;
                    mbutton_loadHardware.Enabled = true;
                    mbutton_saveAs.Enabled       = true;
                    mbutton_initialize.Enabled   = true;
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER, "The dashboard has been un-locked.");


                    foreach (classDeviceControlData data in mdict_deviceControlMap.Values)
                    {
                        data.DashboardGlyph.UserControl.Enabled = true;
                    }
                }
            }
        }
        #endregion

        #region Window Arranging
        /// <summary>
        /// Arranges controls in a tile fashion
        /// </summary>
        private void TileArrange()
        {
            List<UserControl> controls = new List<UserControl>();
            controls.AddRange(mdict_userControlToDeviceMap.Keys);
            TileArrange(controls);
        }
        private void TileArrange(List<UserControl> controls)
        {
            int paddingY        = 10;
            int paddingX        = 10;
            int maxRowHeight    = 0;
            int left            = paddingX;
            int top             = paddingY;

            foreach (UserControl x in controls)
            {
                // Increase the row if needed
                int farWidth = left + x.Width + paddingX + mpanel_deviceQuickBoard.Width;
                if (farWidth > Width )
                {
                    left = paddingX;
                    top += paddingY + maxRowHeight;
                    maxRowHeight = 0;
                }

                maxRowHeight = Math.Max(x.Height, maxRowHeight);
                x.Location = new Point(left, top);

                // Move the next guy over a smidge including its padding
                left += x.Width + paddingX;
            }

            Refresh();
        }
        #endregion

        private void ToggleQuickBoardView()
        {
            if (mpanel_deviceQuickBoard.Width == 36)
            {
                mpanel_deviceQuickBoard.Width   = 85;
                mbutton_initialize.Visible      = true;
                mbutton_lock.Visible            = true;
                mbutton_unlock.Visible          = true;
                mbutton_addDevice.Visible       = true;
                mbutton_removeDevice.Visible    = true;
            }
            else
            {
                mpanel_deviceQuickBoard.Width   = 36;
                mbutton_initialize.Visible      = false;
                mbutton_lock.Visible            = false;
                mbutton_unlock.Visible          = false;
                mbutton_addDevice.Visible       = false;
                mbutton_removeDevice.Visible    = false;
            }
        }
        private void SortByName()
        {
            SortByDelegate(delegate (IDevice x, IDevice y)
                {
                    return x.Name.CompareTo(y.Name);
                }
            );
        }
        private void SortByPluginType()
        {
            SortByDelegate(delegate(IDevice x, IDevice y)
            {
                return x.GetType().Name.CompareTo(y.GetType().Name);
            }
            );
        }
        private void SortByDeviceType()
        {
            SortByDelegate(delegate(IDevice x, IDevice y)
            {
                classDeviceControlAttribute xc = mdict_deviceControlMap[x].DeviceAttribute;
                classDeviceControlAttribute yc = mdict_deviceControlMap[y].DeviceAttribute;

                int result = xc.Category.CompareTo(yc.Category);
                if (result != 0)
                {
                    return result;
                }
                else
                    return xc.Name.CompareTo(yc.Name);
            }
            );
        }
        private void SortByStatus()
        {
            SortByDelegate(delegate(IDevice x, IDevice y)
            {
                return x.Status.CompareTo(y.Status);
            }
            );
        }
        private void SortByHeight()
        {
            SortByDelegate(delegate(IDevice x, IDevice y)
            {
                UserControl xc = mdict_deviceControlMap[x].DashboardGlyph.UserControl;
                UserControl yc = mdict_deviceControlMap[y].DashboardGlyph.UserControl;

                return xc.Height.CompareTo(yc.Height);
            }
            );
        }
        private void SortByDelegate(Comparison<IDevice> comparer)
        {
            List<UserControl> controls = new List<UserControl>();
            List<IDevice> devices      = new List<IDevice>();
            devices.AddRange(mdict_deviceControlMap.Keys);
            devices.Sort(comparer);

            foreach (IDevice device in devices)
            {
                controls.Add(mdict_deviceControlMap[device].DashboardGlyph.UserControl);
            }

            TileArrange(controls);
        }

        #region Event Handlers for context menu sorting
        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            ToggleQuickBoardView();
        }
        private void tileArrangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TileArrange();
        }
        private void sortByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByName();
        }
        private void sortByTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByDeviceType();
        }
        private void sortByStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByStatus();
        }
        private void sortByHeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByHeight();
        }
        private void sortByPluginTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByPluginType();
        }
        #endregion

    }
}
