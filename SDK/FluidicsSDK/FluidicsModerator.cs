using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Managers;
using LcmsNetSDK.Devices;

namespace FluidicsSDK
{
    /// <summary>
    /// Controls interaction between connections, Fluidics devices, and other managers
    /// </summary>
    public class FluidicsModerator : IModelCheckController
    {
        private static FluidicsModerator m_instance;
        private readonly ConnectionManager m_conManager;
        private readonly FluidicsDeviceManager m_fluidicsDevManager;
        private readonly PortManager m_portManager;
        private readonly bool m_methodRunning;
        private readonly List<FluidicsDevice> m_selectedDevices;
        private readonly List<Connection> m_selectedConnections;
        private readonly List<Port> m_selectedPorts;
        private float m_last_scaled_at;
        private Rect m_worldView;
        private Rect m_scaledWorldView;
        public delegate void ModelChange();
        public event ModelChange ModelChanged;
        private bool m_suspendModelUpdates;
        private readonly bool m_holdModelUpdates;
        private readonly List<IFluidicsModelChecker> m_fluidicsCheckers;
        private int m_suspendCounter;

        /// <summary>
        /// Private class constructor, private due to the fluidics moderator being a singleton.
        /// </summary>
        private FluidicsModerator()
        {
            //instantiate manager
            m_conManager = ConnectionManager.GetConnectionManager;
            m_fluidicsDevManager = FluidicsDeviceManager.DeviceManager;
            m_portManager = PortManager.GetPortManager;

            // assign event handlers
            m_fluidicsDevManager.DeviceAdded += m_fluidicsDevManager_DeviceAdded;
            m_fluidicsDevManager.DeviceRemoved += m_fluidicsDevManager_DeviceChanged;
            m_fluidicsDevManager.DeviceChanged += m_fluidicsDevManager_DeviceChanged;

            m_conManager.ConnectionChanged += m_conManager_ConnectionChanged;
            m_portManager.PortChanged += m_portManager_PortChanged;

            //setup member variables
            m_methodRunning = false;
            m_selectedConnections = new List<Connection>();
            m_selectedDevices = new List<FluidicsDevice>();
            m_selectedPorts = new List<Port>();
            m_last_scaled_at = 0F;
            m_worldView = new Rect();
            m_fluidicsCheckers = new List<IFluidicsModelChecker>();
            var sinkCheck = new ModelCheckers.NoSinksModelCheck();
            var cycleCheck = new ModelCheckers.FluidicsCycleCheck();
            var multipleCheck = new ModelCheckers.MultipleSourcesModelCheck();
            // Disable the model checkers by default. They're too slow for normal use. They can
            // be turned on/off for use with the simulator this way.
            sinkCheck.IsEnabled = false;
            cycleCheck.IsEnabled = false;
            multipleCheck.IsEnabled = false;
            m_fluidicsCheckers.Add(sinkCheck);
            m_fluidicsCheckers.Add(cycleCheck);
            m_fluidicsCheckers.Add(multipleCheck);
            ModelCheckAdded?.Invoke(this, new ModelCheckControllerEventArgs(sinkCheck));
            ModelCheckAdded?.Invoke(this, new ModelCheckControllerEventArgs(cycleCheck));
            ModelCheckAdded?.Invoke(this, new ModelCheckControllerEventArgs(multipleCheck));
            m_suspendModelUpdates = false;
            m_holdModelUpdates = false;
            m_suspendCounter = 0;
            DrawingScaleFactor = 1;
        }

        void m_fluidicsDevManager_DeviceChanged(object sender, EventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine("FluidicsDevManager_deviceChanged sender: " + sender.ToString());
            OnModelChanged();
        }

        private IEnumerable<ModelStatus> CheckErrors()
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            var statusChanges = new List<ModelStatus>();
            if (m_fluidicsCheckers.Count > 0)
            {
                //watch.Start();
                foreach (var check in m_fluidicsCheckers)
                {
                    if (check.IsEnabled)
                    {
                        statusChanges.AddRange(check.CheckModel());
                    }
                }
                //watch.Stop();
                //LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Model Checkers took " + watch.Elapsed.TotalMilliseconds + "ms to run. Found " + statusChanges.Count + " Possible Errors");
            }
            return statusChanges;
        }


        public List<IFluidicsModelChecker> GetModelCheckers()
        {
            return m_fluidicsCheckers;
        }

        /// <summary>
        /// Causes any model updates to be suspended until end model suspension is called.
        /// </summary>
        public void BeginModelSuspension()
        {
            m_suspendModelUpdates = true;
            m_suspendCounter++;

        }

        /// <summary>
        /// Causes any model updates to immediately resume (and invoke).
        /// </summary>
        public void EndModelSuspension(bool updateImmediately)
        {
            m_suspendModelUpdates = false;
            m_suspendCounter--;
            if (updateImmediately)
            {
                if (m_suspendCounter == 0)
                {
                    //System.Diagnostics.Trace.WriteLine("EndModelSuspension");
                    OnModelChanged();
                }
            }
        }

        public int GetSelectedPortCount()
        {
            return m_selectedPorts.Count;
        }

        private void OnModelChanged()
        {
            if (m_suspendModelUpdates || m_holdModelUpdates)
            {
                //System.Diagnostics.Trace.WriteLine("Ignoring Model Change");
                return;
            }
            // This foreach loop resets the color of all connections after every change in the model.
            foreach (var conn in m_conManager.GetConnections().ToList())
            {
                conn.Color = Colors.Black;
            }
            var modelStatus = CheckErrors();
            if (modelStatus != null)
            {
                var errorMessages = modelStatus.ToList();
                if (errorMessages.Any())
                {
                    ModelStatusChangeEvent?.Invoke(this, new ModelStatusChangeEventArgs(errorMessages));
                }
            }
            ModelChanged?.Invoke();
        }

        /// <summary>
        /// tells specified layer to render to the screen
        /// </summary>
        /// <param name="g">the DrawingContext object to be used for rendering</param>
        /// <param name="alpha">Transparency value to be  applied to the devices</param>
        /// <param name="scale">scale all objects in the layer by this amount</param>
        /// <param name="layer"></param>
        public void Render(DrawingContext g, byte alpha, float scale, RenderLayer layer)
        {
            //System.Diagnostics.Trace.WriteLine(string.Format("Moderator Render called for {0}", layer));
            //define at what scale the graphics are being drawn, for use in selection of glyphs while scaled.
            m_last_scaled_at = scale;
            switch (layer)
            {
                case RenderLayer.Devices:
                    m_fluidicsDevManager.Render(g, alpha, scale);
                    break;
                case RenderLayer.Ports:
                    m_portManager.Render(g, alpha, scale);
                    break;
                case RenderLayer.Connections:
                    m_conManager.Render(g, alpha, scale);
                    break;
            }
        }

        /// <summary>
        /// Try to take an action at point where mouse button was clicked
        /// </summary>
        /// <param name="location">x,y location where mouse was clicked</param>
        /// <returns>true if action taken, false if not</returns>
        public bool TakeAction(Point location)
        {
            var retVal = m_fluidicsDevManager.TakeAction(location);
            return retVal;
        }

        /// <summary>
        /// selects the first connection, port, or device found at the specified location
        /// </summary>
        /// <param name="mouse_location">a Point representing the location of the item to be selected</param>
        /// <param name="multiple_select"></param>
        public bool Select(Point mouse_location, bool multiple_select)
        {
            BeginModelSuspension();
            Point rescaledLocation;
            var selected = false;
            if (m_last_scaled_at < 1)
            {
                rescaledLocation = new Point((mouse_location.X * (1 / m_last_scaled_at)), (mouse_location.Y * (1 / m_last_scaled_at)));
            }
            else
            {
                rescaledLocation = new Point((mouse_location.X / m_last_scaled_at), (mouse_location.Y / m_last_scaled_at));
            }
            var conUnderMouse = m_conManager.Select(rescaledLocation);
            var portUnderMouse = m_portManager.Select(rescaledLocation);
            var devUnderMouse = m_fluidicsDevManager.Select(rescaledLocation);
            if (m_selectedDevices.Count == 1 && m_selectedDevices.Contains(devUnderMouse) && !multiple_select)
            {
                EndModelSuspension(true);
                return true;
            }
            if (portUnderMouse == null && devUnderMouse == null && conUnderMouse == null)
            {
                selected = TakeAction(rescaledLocation);
            }
            //if we're not doing multiple selection(AKA holding ctrl key), clear out the currently selected items before selecting the new one.
            //add the first one detected to the list of selected items.
            else if (portUnderMouse != null)
            {
                if (!m_selectedPorts.Contains(portUnderMouse))
                {
                    if (!multiple_select)
                    {
                        ClearSelected();
                    }
                    m_selectedPorts.Add(portUnderMouse);
                    m_portManager.ConfirmSelect(portUnderMouse);
                    selected = true;
                }
            }
            else if (conUnderMouse != null)
            {
                if (!m_selectedConnections.Contains(conUnderMouse))
                {
                    if (!multiple_select)
                    {
                        ClearSelected();
                    }
                    m_selectedConnections.Add(conUnderMouse);
                    m_conManager.ConfirmSelect(conUnderMouse);
                    selected = true;
                }
            }
            else if (devUnderMouse != null)
            {
                if (!m_selectedDevices.Contains(devUnderMouse))
                {
                    // try to take an action( if user clicked on action primitive don't "select")
                    if (!multiple_select)
                    {
                        ClearSelected();
                    }
                    m_selectedDevices.Add(devUnderMouse);
                    m_fluidicsDevManager.ConfirmSelect(devUnderMouse, rescaledLocation);
                    selected = true;
                }
            }
            else
            {
                // Select nothing if nothing is under the mouse.
                // Based on the above logic, this code should never be reached
                EndModelSuspension(false);
            }
            if (selected)
            {
                EndModelSuspension(true);
            }
            else
            {
                EndModelSuspension(false);
            }
            return selected;
        }

        /// <summary>
        /// used to keep from moving devices when inappropriate
        /// for instance when a device is selected, but the user clicks and drags
        /// outside the device, which would likely cause an accidental move.
        /// </summary>
        /// <param name="mouse_location"> point mouse is located at</param>
        /// <returns>true or false</returns>
        public bool MouseOnSelected(Point mouse_location)
        {
            var devFound = false;
            var realLoc = new Point((mouse_location.X / m_last_scaled_at), (mouse_location.Y / m_last_scaled_at));
            // if the mouse is located over one of the selected devices
            // return true, otherwise false
            foreach (var device in m_selectedDevices)
            {
                if (device.Contains(realLoc))
                {
                    devFound = true;
                    break;
                }
            }
            return devFound;
        }

        /// <summary>
        /// create the world view that devices can move around in
        /// </summary>
        /// <param name="graphicsView">A rectangle representing the origin and size of the visible area</param>
        public void SetWorldView(Rect graphicsView)
        {
            // set the worldview to the same place and size as the provide graphics view.
            m_worldView = new Rect(graphicsView.Location, graphicsView.Size);
        }

        public void ScaleWorldView(float scale)
        {
            // only change the world view if scale is different from last time we scaled.
            if (Math.Abs(scale - m_last_scaled_at) > float.Epsilon)
            {
                m_last_scaled_at = scale;
                var scaleWidthValue = (int)(m_worldView.Size.Width - (m_worldView.Size.Width * scale));
                var scaleHeightValue = (int)(m_worldView.Size.Height - (m_worldView.Size.Height * scale));
                var scaledView = new Rect(m_worldView.X, m_worldView.Y, m_worldView.Width - scaleWidthValue, m_worldView.Height - scaleHeightValue);
                if (scaledView.Size.Width < m_worldView.Size.Width || scaledView.Size.Height < m_worldView.Size.Height)
                {
                    m_scaledWorldView = m_worldView;
                }
                else
                {
                    var deviceBoundingBox = m_fluidicsDevManager.GetBoundingBox(false);
                    var boxWidth = deviceBoundingBox.Size.Width;
                    var boxHeight = deviceBoundingBox.Size.Height;
                    // this is to take care of the case when a user scales down devices drags them around, and then scales up again...which may leave the devices outside the "worldview"
                    // if we don't do this.
                    scaledView.Size = new Size(scaledView.Size.Width < boxWidth ? (int)(boxWidth * scale) : scaledView.Size.Width, scaledView.Size.Height < boxHeight ? (int)(boxHeight * scale) : scaledView.Size.Height);
                    m_scaledWorldView = scaledView;
                }
            }
        }

        /// <summary>
        /// deselect all items.
        /// </summary>
        public void ClearSelected()
        {
            foreach (var connection in m_selectedConnections)
            {
                connection.Deselect();
            }
            m_selectedConnections.Clear();
            foreach (var device in m_selectedDevices)
            {
                device.Deselect();
            }
            m_selectedDevices.Clear();
            foreach (var port in m_selectedPorts)
            {
                port.Deselect();
            }
            m_selectedPorts.Clear();
        }

        /// <summary>
        /// Try to deselect an item at a specific location
        /// </summary>
        /// <param name="location">a Point representing the mouse location</param>
        /// <param name="multiselect">bool determining if multiselect mode is active</param>
        public void Deselect(Point location, bool multiselect)
        {
            var conUnderMouse = m_conManager.Select(location);
            var portUnderMouse = m_portManager.Select(location);
            var devUnderMouse = m_fluidicsDevManager.Select(location);
            //add the first one detected to the list of selected items.
            if (conUnderMouse != null)
            {
                if (m_selectedConnections.Contains(conUnderMouse))
                {
                    m_conManager.Deselect(conUnderMouse);
                    m_selectedConnections.Remove(conUnderMouse);
                }
            }
            else if (portUnderMouse != null)
            {
                if (m_selectedPorts.Contains(portUnderMouse))
                {
                    m_portManager.Deselect(portUnderMouse);
                    m_selectedPorts.Remove(portUnderMouse);
                }
            }
            else if (devUnderMouse != null)
            {
                if (m_selectedDevices.Contains(devUnderMouse))
                {
                    m_fluidicsDevManager.Deselect(devUnderMouse);
                    m_selectedDevices.Remove(devUnderMouse);
                }
            }
            else
            {
                if (!multiselect)
                {
                    ClearSelected();
                }
            }
        }

        /// <summary>
        /// Deselect selected ports
        /// </summary>
        public void DeselectPorts()
        {
            foreach (var port in m_selectedPorts)
            {
                port.Deselect();
            }
            m_selectedPorts.Clear();
        }

        /// <summary>
        /// Add a device to the fluidics system
        /// </summary>
        /// <param name="device">an IDevice repesenting the device to add</param>
        public void AddDevice(IDevice device)
        {
            if (!m_methodRunning)
            {
                BeginModelSuspension();
                m_fluidicsDevManager.Add(device);
                EndModelSuspension(true);
            }
        }

        /// <summary>
        /// get a list of all active fluidics devices.
        /// </summary>
        /// <returns>a List of devices</returns>
        public List<FluidicsDevice> GetDevices()
        {
            return m_fluidicsDevManager.GetDevices();
        }

        /// <summary>
        /// Allows for setting the selected device outside of the interactive display
        /// </summary>
        /// <param name="device"></param>
        public void SetSelectedDevice(IDevice device)
        {
            BeginModelSuspension();
            ClearSelected();

            if (device != null)
            {
                m_selectedDevices.Add(FluidicsDeviceManager.DeviceManager.FindDevice(device));
                EndModelSuspension(true);
            }
            else
            {
                EndModelSuspension(false);
            }
        }

        /// <summary>
        /// remove a device from the fluidics system based on its IDevice
        /// </summary>
        /// <param name="device">an IDevice representing the device to remove</param>
        public void RemoveDevice(IDevice device)
        {
            // find the ports associated with the device
            BeginModelSuspension();
            var fdevice = FluidicsDeviceManager.DeviceManager.FindDevice(device);
            foreach (var port in fdevice.Ports)
            {
                //remove the connections associated with those ports
                m_conManager.RemoveConnections(port);
            }
            // remove the ports themselves
            m_portManager.RemovePorts(fdevice);
            // finally, remove the device itself
            m_selectedDevices.Remove(fdevice);
            m_fluidicsDevManager.Remove(device);
            // notify of the change in the model.
            EndModelSuspension(true);
        }

        /// <summary>
        /// Remove specified devices from the fluidics system
        /// </summary>
        public List<IDevice> RemoveSelectedDevices()
        {
            var removedDevices = new List<IDevice>();
            BeginModelSuspension();
            foreach (var device in m_selectedDevices)
            {
                if (!m_methodRunning)
                {
                    removedDevices.Add(device.IDevice);
                    //remove device from the selected devices list..if it exists there.
                }
            }
            EndModelSuspension(true);
            return removedDevices;
        }

        /// <summary>
        /// Remove specified connections from the fluidics system
        /// </summary>
        /// <param name="connections">A list of classConnections</param>
        public void RemoveConnections(List<Connection> connections)
        {
            if (!m_methodRunning)
            {
                BeginModelSuspension();
                foreach (var connection in connections)
                {
                    m_conManager.Remove(connection);
                    if (m_selectedConnections.Contains(connection))
                    {
                        m_selectedConnections.Remove(connection);
                    }
                }
                EndModelSuspension(true);
            }
        }

        /// <summary>
        /// remove all selected connections
        /// </summary>
        public void RemoveSelectedConnections()
        {
            BeginModelSuspension();
            var tmp = new List<Connection>(m_selectedConnections);
            RemoveConnections(tmp);
            EndModelSuspension(true);
        }

        /// <summary>
        /// remove connections that connect to the specified port
        /// </summary>
        /// <param name="port">a port to remove connections from</param>
        public void RemoveConnections(Port port)
        {
            BeginModelSuspension();
            m_conManager.RemoveConnections(port);
            EndModelSuspension(true);

        }

        /// <summary>
        /// Move the selected devices around the screen by the specified amount
        /// </summary>
        /// <param name="amountMoved">a Point representing how far the mouse moved </param>
        public void MoveSelectedDevices(Point amountMoved)
        {
            MoveDevices(amountMoved, m_selectedDevices);
        }

        /// <summary>
        /// move specified devices by the specified amount
        /// </summary>
        /// <param name="amountMoved"></param>
        /// <param name="devices"></param>
        private void MoveDevices(Point amountMoved, List<FluidicsDevice> devices)
        {
            //normalize amount moved so that the devices move the same way at any scale.
            var normalizedAmountMoved = new Point((amountMoved.X * (1 / m_last_scaled_at)), (amountMoved.Y * (1 / m_last_scaled_at)));
            BeginModelSuspension();
            foreach (var device in devices)
            {
                device.MoveBy(normalizedAmountMoved);
            }
            //reorder the list as necessary.
            m_fluidicsDevManager.BringToFront(new List<FluidicsDevice>(devices));
            EndModelSuspension(true);
        }

        public Rect GetBoundingBox(bool addBuffer = true)
        {
            return m_fluidicsDevManager.GetBoundingBox(addBuffer);
        }

        /// <summary>
        /// try to create a connection between two selected ports
        /// </summary>
        /// <exception cref="ArgumentException">tried to connect a port to itself or tried to connection more than two ports at once</exception>
        public void CreateConnection()
        {
            if (m_selectedPorts.Count == 2)
            {
                CreateConnection(m_selectedPorts[0], m_selectedPorts[1]);
            }
            else
            {
                throw new ArgumentException("Only two ports at a time can be connected.");
            }

        }

        /// <summary>
        /// create a connection between to Port objects
        /// </summary>
        /// <param name="p1">the first port to connect</param>
        /// <param name="p2">the second port to connect</param>
        public void CreateConnection(Port p1, Port p2)
        {
            m_conManager.Connect(p1, p2);
        }

        /// <summary>
        /// create a connection between two ports based on their string ID
        /// </summary>
        /// <param name="p1">the ID of the first port</param>
        /// <param name="p2">the ID of the second port</param>
        /// <param name="connectionStyle"></param>
        public void CreateConnection(string p1, string p2, string connectionStyle)
        {
            var p1Real = m_portManager.FindPort(p1);
            var p2Real = m_portManager.FindPort(p2);
            if (p1Real != null && p2Real != null)
            {
                var style = (ConnectionStyles)Enum.Parse(typeof(ConnectionStyles), connectionStyle);
                m_conManager.Connect(p1Real, p2Real, null, style);
            }
            else
            {
                throw new Exception("A port was not found when attempting to create connection!");
            }
        }

        /// <summary>
        /// get a list of all devices and their status, used by the PDF generator
        /// </summary>
        /// <returns>a list of 3-tuples of strings giving device type, name, and status</returns>
        public List<Tuple<string, string, string>> ListDevicesAndStatus()
        {
            return m_fluidicsDevManager.ListDevicesAndStatus();
        }

        /// <summary>
        /// Take some action on doubleclick
        /// </summary>
        /// <param name="location"></param>
        public void DoubleClickActions(Point location)
        {
            var clickedConnection = m_conManager.Select(location);
            clickedConnection?.DoubleClicked();
            OnModelChanged();
        }

        /// <summary>
        /// the "world view" of the fluidics system. All fluidics devices exist/move around within this world view.
        /// it controls the boundaries
        /// </summary>
        public Rect WorldView => new Rect(m_scaledWorldView.X, m_scaledWorldView.Y, m_scaledWorldView.Width, m_scaledWorldView.Height);

        /// <summary>
        /// Static instance property. Returns instance of the moderator class
        /// </summary>
        public static FluidicsModerator Moderator { get; } = m_instance ?? (m_instance = new FluidicsModerator());

        /// <summary>
        /// Property for determining if a method is currently running on the system
        /// </summary>
        public bool MethodRunning => m_methodRunning;

        /// <summary>
        /// Scale factor for device drawing. Defaults to '1'. Also know as 'Pixels per DIP', or 'Pixels per Device Independent Pixel', and should later be set using VisualTreeHelper.GetDpi(Visual visual).PixelsPerDip
        /// </summary>
        public double DrawingScaleFactor { get; set; }

        /// <summary>
        /// Event handler for the FluidicsDeviceManager's Device change events
        /// </summary>
        void m_fluidicsDevManager_DeviceChanged(object sender, Devices.FluidicsDeviceChangeEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("DeviceChange sender: " + sender.ToString());
            OnModelChanged();
        }

        void m_fluidicsDevManager_DeviceAdded(object sender, Devices.FluidicsDeviceChangeEventArgs e)
        {
            OnModelChanged();
        }

        void m_conManager_ConnectionChanged(object sender, ConnectionChangedEventArgs<Connection> e)
        {
            //System.Diagnostics.Trace.WriteLine("ConnectionChange sender: " + sender.ToString());
            OnModelChanged();
        }

        void m_portManager_PortChanged(object sender, PortChangedEventArgs<Port> e)
        {
            //System.Diagnostics.Trace.WriteLine("PortChange sender: " + sender.ToString());
            OnModelChanged();
        }

        public IEnumerable<Connection> GetConnections()
        {
            return m_conManager.GetConnections();
        }

        public event EventHandler<ModelCheckControllerEventArgs> ModelCheckAdded;

        public event EventHandler<ModelCheckControllerEventArgs> ModelCheckRemoved
        {
            add { }
            remove { }
        }

#pragma warning disable 67
        public event EventHandler<ModelStatusChangeEventArgs> ModelStatusChanged
        {
            add { }
            remove { }
        }
#pragma warning restore 67

        public event EventHandler<ModelStatusChangeEventArgs> ModelStatusChangeEvent;
    }
}
