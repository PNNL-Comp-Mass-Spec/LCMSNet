using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using LcmsNetDataClasses.Devices;

namespace FluidicsSDK.Base
{
    public abstract class FluidicsDeviceWpf
    {
        #region Members
        //graphics primitives that make up the device
        protected List<GraphicsPrimitiveWpf> m_primitives;
        /* location of the primary primitive in the m_primitives list*/
        protected const int PRIMARY_PRIMITIVE = 0;
        // list of classPorts
        protected List<PortWpf> m_portList;
        // name may be unnecessary
        protected string m_deviceName;
        // top-left corner's location
        private Point m_loc;
        // the IDevice this fluidicsdeivce controls
        private IDevice m_lcmsDevice;
        // dictionary of actions that can be taken by this device, keyed by the primitves that
        // activate them
        private readonly Dictionary<GraphicsPrimitiveWpf, Action> m_actions;
        //list of primitives that are associated with actions..aka controls.
        protected List<GraphicsPrimitiveWpf> m_actionPrims;
        //box to be drawn around controls and info strings, also determines positioning of said controls and strings
        protected Rect m_info_controls_box;
        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        protected FluidicsDeviceWpf()
        {
            m_primitives = new List<GraphicsPrimitiveWpf>();
            m_actionPrims = new List<GraphicsPrimitiveWpf>();
            m_portList = new List<PortWpf>();
            m_loc = new Point(0, 0);
            m_info_controls_box = new Rect(0, 0, 100, 100);
            m_actions = new Dictionary<GraphicsPrimitiveWpf, Action>();
            Selected = false;
            Source = false;
            Sink = false;
        }

        ~FluidicsDeviceWpf()
        {
            m_portList.Clear();
            m_deviceName = string.Empty;
            m_lcmsDevice = null;
            m_primitives.Clear();
        }

        public virtual void RegisterDevice(IDevice device)
        {
            m_deviceName = device.Name;
            m_lcmsDevice = device;
            //TODO: Setup any error handlers
            SetDevice(device);
            foreach (var p in m_portList)
            {
                p.ID = DeviceName + "." + m_portList.IndexOf(p);
            }
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());
        }

        public virtual void DeregisterDevice(IDevice device)
        {
            //TODO: Remove any error handlers
            ClearDevice(device);
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());
        }

        /// <summary>
        /// Called upon registration for custom fluidics devices to interact with the underlying device
        /// </summary>
        /// <param name="device"></param>
        protected abstract void SetDevice(IDevice device);

        /// <summary>
        /// Called upon registration for custom fluidics devices to remove any interactions with the underlying device
        /// </summary>
        /// <param name="device"></param>
        protected abstract void ClearDevice(IDevice device);

        /// <summary>
        /// generates a single 'state' of connections, by connecting pairs of ports, starting
        /// with the ports at startingPortIndex and startingPortIndex+1, then startingPortIndex+2 and startingPortIndex+3,
        /// etc.
        /// </summary>
        /// <param name="startingPortIndex">an index into m_ports</param>
        /// <param name="endingPortIndex">an index into m_ports</param>
        /// <returns>a list of int,int tuples</returns>
        protected virtual List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
        {
            var connectionState = new List<Tuple<int, int>>();
            if (startingPortIndex != -1)
            {
                //make sure that this isn't a connectionless state
                if (m_portList.Count - 1 <= endingPortIndex)
                {
                    for (var i = startingPortIndex; i < endingPortIndex; i += 2)
                    {
                        Tuple<int, int> connectionTuple;
                        //this if handles the case where you want to connect the last port with the first port(example: connect port 3 and port 0
                        //in a four port valve)
                        if (i == endingPortIndex - 1 && startingPortIndex != 0)
                        {
                            connectionTuple = new Tuple<int, int>(i, 0);
                        }
                        else
                        {
                            connectionTuple = new Tuple<int, int>(i, i + 1);
                        }
                        connectionState.Add(connectionTuple);
                    }
                }
            }
            //will return an empty list if startingPortIndex is -1, represents a "no connections" state.
            return connectionState;
        }


        /// <summary>
        /// Activates the state number "state" in the devices list of states
        /// </summary>
        /// <param name="state">an integer determining which state to switch to.</param>
        public abstract void ActivateState(int state);

        /// <summary>
        /// Take a list of tuples and use it to create the internal connections
        /// of the device.
        /// </summary>
        /// <param name="state">a list of tuples, each tuple represents a single internal connection</param>
        public virtual void ActivateState(List<Tuple<int, int>> state)
        {
            FluidicsModeratorWpf.Moderator.BeginModelSuspension();
            //TODO: change device state here
            //remove internal connections
            foreach (var p in Ports)
            {

                foreach (var c in p.Connections)
                {
                    if (c.InternalConnectionOf == this)
                    {
                        ConnectionManagerWpf.GetConnectionManager.Remove(c, this);
                    }
                }
            }

            //create new internal connections
            foreach (var t in state)
            {
                ConnectionManagerWpf.GetConnectionManager.Connect(m_portList[t.Item1], m_portList[t.Item2], this);
            }
            FluidicsModeratorWpf.Moderator.EndModelSuspension(true);
        }

        /// <summary>
        /// Render device to screen
        /// </summary>
        /// <param name="g">Graphics object to draw with/on</param>
        /// <param name="alpha"></param>
        /// <param name="scale">scale device by this amount on screen</param>
        public virtual void Render(DrawingContext g, byte alpha, float scale = 1)
        {
            foreach (var primitive in m_primitives)
            {
                //don't highlight primitives that have actions associated with them.
                if (m_actions[primitive] == null)
                {
                    primitive.Render(g, alpha, scale, Selected, IDevice.Status == enumDeviceStatus.Error);
                }
                else
                {
                    primitive.Render(g, alpha, scale, false, IDevice.Status == enumDeviceStatus.Error);
                }
            }
            DrawControls(g, alpha, scale);
        }

        /// <summary>
        /// draw controls to screen
        /// </summary>
        /// <param name="g"></param>
        /// <param name="alpha"></param>
        /// <param name="scale"></param>
        protected virtual void DrawControls(DrawingContext g, byte alpha, float scale)
        {
            var br = new SolidColorBrush(Color.FromArgb(alpha, Colors.Black.R, Colors.Black.G, Colors.Black.B));
            //determine font size, used to scale font with graphics primitives
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);
            var nameFont = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            // draw name to screen
            var name = DeviceName;
            var deviceNameText = new FormattedText(name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, (11.0F * stringScale) * (96.0 / 72.0), br);
            m_info_controls_box = UpdateControlBoxLocation();
            //place the name at the top middle of the box
            var nameLocation = CreateStringLocation((int)(m_info_controls_box.Y * scale), deviceNameText.Width, scale);
            g.DrawText(deviceNameText, nameLocation);
            //place the state below the name, centered as well
            var stateText = new FormattedText(StateString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, (11.0F * stringScale) * (96.0 / 72.0), br);
            var stateLocation = CreateStringLocation((int)(nameLocation.Y + deviceNameText.Height), stateText.Width, scale);
            g.DrawText(stateText, stateLocation);
            // default implementation, expecting only two controls. if a better implementation is needed...override it.
            if (m_actionPrims.Count == 2)
            {
                var prim = m_actionPrims[0];
                //left triangle padding(so it doesn't draw over the state string
                var padding = 3;
                var relativeMove = new Vector(stateLocation.X - (prim.Size.Width * scale + padding + (prim.Loc.X * scale)), stateLocation.Y - (prim.Loc.Y * scale));
                prim.Loc = Point.Add(prim.Loc, relativeMove);
                prim.Render(g, alpha, scale, false, false);
                prim = m_actionPrims[1];

                relativeMove = new Vector(stateLocation.X + stateText.Width - (prim.Loc.X * scale) + padding, stateLocation.Y - (prim.Loc.Y * scale));
                prim.Loc = Point.Add(prim.Loc, relativeMove);
                prim.Render(g, alpha, scale, false, false);
            }
        }

        /// <summary>
        /// update the location of the control box.
        /// </summary>
        /// <returns></returns>
        protected virtual Rect UpdateControlBoxLocation()
        {
            var top = m_primitives.Max(x => x.Loc.Y + x.Size.Height);
            var xa = m_primitives.Min(j => j.Loc.X);
            return new Rect(xa, top, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        /// <summary>
        /// create location of a string to be drawn in the control box.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="stringWidth"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected virtual Point CreateStringLocation(int y, double stringWidth, float scale)
        {
            return new Point((int)((m_info_controls_box.X * scale + (m_info_controls_box.Size.Width * scale / 2)) - (stringWidth / 2)),
                (int)(y + 2));
        }

        /// <summary>
        /// selects the device
        /// </summary>
        public virtual void Select(Point mouse_location)
        {
            Selected = true;
        }

        /// <summary>
        /// Take action for primitive that is at the location clicked, if one is defined.
        /// such as changing state of the device when clicking on a state control primitive.
        /// </summary>
        /// <param name="mouse_location">the x,y position the mouse was at when the button was clicked</param>
        /// <returns>true if an action was taken, false if not</returns>
        public virtual bool TakeAction(Point mouse_location)
        {
            foreach (var prim in m_actionPrims)
            {
                if (prim.Contains(mouse_location, MaxVariance))
                {
                    m_actions[prim]();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// deselects the device
        /// </summary>
        public virtual void Deselect()
        {
            Selected = false;
        }

        /// <summary>
        /// Adds a custom graphics primitive to the fluidics device.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="action"></param>
        protected virtual void AddPrimitive(GraphicsPrimitiveWpf primitive, Action action = null)
        {
            //don't add primitive to m_primitives if it has an associated action
            if (action == null)
            {
                m_primitives.Add(primitive);
            }
            else
            {
                m_actionPrims.Add(primitive);
            }
            m_actions[primitive] = action;
        }

        /// <summary>
        ///  Add a circle to the list of graphics primitives representing this device.
        /// </summary>
        /// <param name="loc">Point representing the location of the circle on screen</param>
        /// <param name="radius">an int representing the radius of the circle</param>
        /// <param name="color">a Color representing the primary color of the circle</param>
        /// <param name="fillBrush">a Color representing the color the circle should be filled with(used if Brushtype is not Solid)</param>
        /// <param name="fill">a boolean defining if the circle should be filled or not</param>
        /// <param name="action">an Action delegate that takes 0 parameters, this is the action that is taken when the primitive is clicked</param>
        protected virtual void AddCircle(Point loc, int radius, Color color, Brush fillBrush, bool fill = true, Action action = null)
        {
            var circle = new FluidicsCircleWpf(loc, color, fillBrush, radius);
            AddPrimitive(circle, action);
        }

        /// <summary>
        /// Add a rectangle to the devices list of Graphics Primitives
        /// </summary>
        /// <param name="loc">a System.Drawing.Point representing the location the device should be drawn on screen</param>
        /// <param name="size">a System.Drawing.SizeF representing the size of the primitive</param>
        /// <param name="color">a System.Drawing.Color representing the primary draw color(pen color) of the primitive</param>
        /// <param name="fillBrush">a System.Drawing.Color representing the color to fill the primitive with(only used when fill=true)</param>
        /// <param name="fill">boolean that determines if the primitive is filled or not</param>
        /// <param name="action">an Action delegate that takes 0 parameters, this is the action that is taken when the primitive is clicked</param>
        protected virtual void AddRectangle(Point loc, Size size, Color color, Brush fillBrush, bool fill = true, Action action = null)
        {
            var rect = new FluidicsRectangleWpf(loc, size, color, fillBrush);
            AddPrimitive(rect, action);
        }

        /// <summary>
        /// Add a port to the device
        /// </summary>
        /// <param name="location">Point representing location of the port within the device</param>
        protected virtual void AddPort(Point location)
        {
            var newPort = new PortWpf(location, this);
            m_portList.Add(newPort);
        }

        /// <summary>
        /// Add a port to the device
        /// </summary>
        protected virtual void AddPort(PortWpf port)
        {
            m_portList.Add(port);
        }

        /// <summary>
        /// transforms the way the device looks in some way, such as rotation or translation(IE movement across screen)
        /// </summary>
        /// <param name="translateRelativeValues">a System.Drawing.Point describing the relative x and y movement values from the devices current position.</param>
        public virtual void MoveBy(Point translateRelativeValues)
        {
            /*  translation*/
            var oldLoc = m_primitives[PRIMARY_PRIMITIVE].Loc;
            var moveTo = new Point(oldLoc.X + translateRelativeValues.X, oldLoc.Y + translateRelativeValues.Y);
            Loc = moveTo;
            m_primitives[PRIMARY_PRIMITIVE].MoveBy(translateRelativeValues);

            // move ports with device
            foreach (var p in m_portList)
            {
                p.MoveBy(translateRelativeValues);
            }

            // Don't change the primitive at index PRIMARY_PRIMITIVE since it was already moved.
            foreach (var prim in m_primitives.GetRange(PRIMARY_PRIMITIVE + 1, m_primitives.Count - 1))
            {
                prim.MoveBy(translateRelativeValues);
            }
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());
        }

        /// <summary>
        ///  all fluidics devices should be able to determine if they contain a specified point within their bounds
        /// </summary>
        /// <param name="location">a System.Drawing.Point specifying the location to be checkeda against</param>
        /// <returns>a bool determing if the device contains that location</returns>
        public virtual bool Contains(Point location)
        {
            var inDevice = false;
            foreach (var prim in m_primitives)
            {
                if (prim.Contains(location, MaxVariance))
                {
                    inDevice = true;
                    break;
                }
            }
            return inDevice;
        }

        /// <summary>
        /// return the string representing the state of the device
        /// </summary>
        /// <returns></returns>
        public abstract string StateString();
        #endregion

        #region Properties

        public int MaxVariance
        {
            get;
            set;
        }

        /// <summary>
        ///  Property for determining the location of the device on screen
        /// </summary>
        public virtual Point Loc
        {
            get
            {
                return m_loc;
            }
            protected set
            {
                var oldLoc = m_loc;
                m_loc = value;
                //m_info_controls_box.Location = new Point(value.X, value.Y + (int)Size.Height + 5);
                DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs(new System.Drawing.Point((int)oldLoc.X, (int)oldLoc.Y)));
            }
        }

        /// <summary>
        /// Property for determining the size of the device on screen
        /// </summary>
        public virtual Size Size
        {
            get
            {
                var s = new Size
                {
                    Width = m_primitives[PRIMARY_PRIMITIVE].Size.Width,
                    Height = m_primitives[PRIMARY_PRIMITIVE].Size.Height
                };
                s.Height += m_info_controls_box.Size.Height;
                return s;
            }
        }

        /// <summary>
        /// Gets or sets the determining color of the device
        /// </summary>
        public virtual Color Color
        {
            get
            {
                return m_primitives[PRIMARY_PRIMITIVE].Color;
            }
            set
            {
                foreach (var prim in m_primitives)
                {
                    prim.Color = value;
                }
            }
        }


        /// <summary>
        /// property for device name
        /// </summary>
        public virtual string DeviceName => IDevice.Name;

        /// <summary>
        /// property for retrieving the IDevice associated with the device
        /// </summary>
        public virtual IDevice IDevice
        {
            get
            {
                return m_lcmsDevice;
            }
            protected set
            {
                m_lcmsDevice = value;
            }
        }

        /// <summary>
        /// property for determining the color the device is hilighted by
        /// </summary>
        public virtual Color Highlight
        {
            get
            {
                return m_primitives[PRIMARY_PRIMITIVE].Color;
            }
            set
            {
                m_primitives[PRIMARY_PRIMITIVE].Color = value;
            }
        }

        /// <summary>
        /// property for determining if the device is selected or not.
        /// </summary>
        protected virtual bool Selected
        {
            get;
            set;
        }

        public virtual List<PortWpf> Ports => new List<PortWpf>(m_portList);

        /// <summary>
        /// stateless devices should return -1
        /// </summary>
        ///
        public abstract int CurrentState { get; set; }

        public bool Source
        {
            get;
            set;
        }

        public bool Sink
        {
            get;
            set;
        }
        #endregion

        #region Events

        //event for when device changes
        public virtual event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
        #endregion
    }
}
