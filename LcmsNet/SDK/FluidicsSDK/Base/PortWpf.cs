using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;

namespace FluidicsSDK.Base
{
    public sealed class PortWpf : IRenderableWpf
    {
        #region Members
        //fluids currently passing through port/being injected/whatever.
        private List<Fluid> m_fluids;
        //graphics primitive repsenting the port
        private FluidicsCircleWpf m_primitive;
        private int m_radius;
        private const int MAX_PIXEL_VARIANCE = 7;
        private Dictionary<long, ConnectionWpf> m_connections;
        public const int PORT_DEFAULT_RADIUS = 10;
        #endregion

        #region Methods

        /// <summary>
        /// default constructor
        /// </summary>
        private PortWpf()
        {
            var f = new Point(0, 0);
            DefinePort(f, null);
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">a System.Windows.Point representing the location of the port on screen</param>
        /// <param name="parent">a classFluidicsDevice representing what device the port belongs to</param>
        public PortWpf(Point loc, FluidicsDeviceWpf parent)
        {
            DefinePort(loc, parent);
        }

        private void DefinePort(Point loc, FluidicsDeviceWpf parent)
        {
            Radius = PORT_DEFAULT_RADIUS;
            m_connections = new Dictionary<long, ConnectionWpf>();

            // this makes the provided location the center point of the port, instead of the top left corner.
            var trueLoc = new Point(loc.X - Radius, loc.Y - Radius);
            ParentDevice = parent;
            m_fluids = new List<Fluid>();
            m_primitive = new FluidicsCircleWpf(trueLoc, Colors.Black, Brushes.White, Radius)
            {
                Fill = true
            };
            PortManagerWpf.GetPortManager.AddPort(this);
        }


        /// <summary>
        /// add a connection to this port
        /// </summary>
        /// <param name="id">a Guid identifying the connection</param>
        /// <param name="connection">the connection to be added</param>
        public void AddConnection(long id, ConnectionWpf connection)
        {
            m_connections[id] = connection;
        }

        /// <summary>
        /// add a fluid to the list of fluids that have passed through the device
        /// </summary>
        /// <param name="fluid">a classFluid object</param>
        public void AddFluid(Fluid fluid)
        {
            m_fluids.Add(fluid);
        }

        /// <summary>
        /// remove a fluid from the list of fluids that have passed through the device
        /// </summary>
        /// <param name="fluid"></param>
        public void RemoveFluid(Fluid fluid)
        {
            m_fluids.Remove(fluid);
        }

        /// <summary>
        /// render the port to the screen
        /// </summary>
        /// <param name="g">a System.Windows.Media.DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the port at</param>
        /// <param name="scale">a float representing how much to scale the port by</param>
        public void Render(DrawingContext g, byte alpha, float scale)
        {
            m_primitive.Render(g, alpha, scale, Selected, false);
        }

        /// <summary>
        /// destroy the port, and any connections related to it
        /// </summary>
        ~PortWpf()
        {
            m_fluids.Clear();
            m_connections.Clear();
            m_primitive = null;
        }

        /// <summary>
        /// select the port
        /// </summary>
        public void Select()
        {
            Selected = true;
        }

        /// <summary>
        /// deselect the port
        /// </summary>
        internal void Deselect()
        {
            Selected = false;
        }

        /// <summary>
        ///  Check to see if the port contains a location within its bounds
        /// </summary>
        /// <param name="location">a System.Windows.Point to check</param>
        /// <returns> true if the port contains that point, false otherwise</returns>
        internal bool Contains(Point location)
        {
            //standard pythagorean to determine if point is in/on the circle
            var dist = (int)Math.Sqrt(Math.Pow((Center.X - location.X), 2) + Math.Pow((Center.Y - location.Y), 2));
            return (dist <= (m_radius + MAX_PIXEL_VARIANCE));
        }


        public void MoveBy(Point amtToMove)
        {
            //we are moving the center point of the circle, but locations are based on the upper left corner, so adjust for the radius
            var newLoc = new Point(Loc.X + amtToMove.X + m_radius, Loc.Y + amtToMove.Y + m_radius);
            Loc = newLoc;
        }

        internal void RemoveConnection(long id)
        {
            m_connections.Remove(id);
        }

        #endregion

        #region Properties

        /// <summary>
        /// property for determining parent device of the port
        /// </summary>
        public FluidicsDeviceWpf ParentDevice
        {
            get;
            private set;
        }

        /// <summary>
        /// property to determine if the port is a source of a fluid into the system.
        /// </summary>
        public bool Source
        {
            get;
            set;
        }

        /// <summary>
        /// property to determine if the port is a fluid sink out of the system.
        /// </summary>
        public bool Sink
        {
            get;
            set;
        }

        /// <summary>
        /// property to determine location of the port on screen.
        /// </summary>
        public Point Loc
        {
            get
            {
                return m_primitive.Loc;
            }
            set
            {
                var oldLoc = this.Center;
                // make the new location the center point of the port, not the top left corner.
                m_primitive.Loc = new Point(value.X - m_radius, value.Y - m_radius);
                //tell connections about the move.
                foreach (var connection in m_connections.Values)
                {
                    connection.MoveWith(this);
                }
            }
        }

        /// <summary>
        /// property for determining if the port is selected or not.
        /// </summary>
        public bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// property for getting the center of the port.
        /// </summary>
        public Point Center => m_primitive.Center;

        public List<ConnectionWpf> Connections
        {
            get
            {
                var temp = new List<ConnectionWpf>(m_connections.Values);
                return temp;
            }
        }

        public int Radius
        {
            get
            {
                return m_radius;
            }
            private set
            {
                m_radius = value;
            }
        }

        public string ID { get; set; }

        #endregion
    }
}
