using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;

namespace FluidicsSDK.Base
{
    public sealed class Port : IRenderable
    {
        //fluids currently passing through port/being injected/whatever.
        private List<Fluid> m_fluids;
        //graphics primitive representing the port
        private FluidicsCircle m_primitive;
        private const int MAX_PIXEL_VARIANCE = 7;
        private Dictionary<long, Connection> m_connections;
        public const int PORT_DEFAULT_RADIUS = 10;
        private readonly int m_portNumber = -1;

        /// <summary>
        /// default constructor
        /// </summary>
        private Port()
        {
            var f = new Point(0, 0);
            DefinePort(f, null);
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">a System.Windows.Point representing the location of the port on screen</param>
        /// <param name="parent">a classFluidicsDevice representing what device the port belongs to</param>
        /// <param name="portNumber">a port number to display inside the port circle drawing</param>
        public Port(Point loc, FluidicsDevice parent, int portNumber = -1)
        {
            m_portNumber = portNumber;
            DefinePort(loc, parent);
        }

        private void DefinePort(Point loc, FluidicsDevice parent)
        {
            Radius = PORT_DEFAULT_RADIUS;
            m_connections = new Dictionary<long, Connection>();

            // this makes the provided location the center point of the port, instead of the top left corner.
            var trueLoc = new Point(loc.X - Radius, loc.Y - Radius);
            ParentDevice = parent;
            m_fluids = new List<Fluid>();
            var portNumText = m_portNumber >= 0 ? m_portNumber.ToString() : string.Empty;
            m_primitive = new FluidicsCircle(trueLoc, Colors.Black, Brushes.White, Radius, fillText: portNumText)
            {
                Fill = true
            };
            PortManager.GetPortManager.AddPort(this);
        }


        /// <summary>
        /// add a connection to this port
        /// </summary>
        /// <param name="id">a Guid identifying the connection</param>
        /// <param name="connection">the connection to be added</param>
        public void AddConnection(long id, Connection connection)
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
        ~Port()
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
            return (dist <= (Radius + MAX_PIXEL_VARIANCE));
        }


        public void MoveBy(Point amtToMove)
        {
            //we are moving the center point of the circle, but locations are based on the upper left corner, so adjust for the radius
            var newLoc = new Point(Loc.X + amtToMove.X + Radius, Loc.Y + amtToMove.Y + Radius);
            Loc = newLoc;
        }

        internal void RemoveConnection(long id)
        {
            m_connections.Remove(id);
        }

        /// <summary>
        /// property for determining parent device of the port
        /// </summary>
        public FluidicsDevice ParentDevice { get; private set; }

        /// <summary>
        /// property to determine if the port is a source of a fluid into the system.
        /// </summary>
        public bool Source { get; set; }

        /// <summary>
        /// property to determine if the port is a fluid sink out of the system.
        /// </summary>
        public bool Sink { get; set; }

        /// <summary>
        /// property to determine location of the port on screen.
        /// </summary>
        public Point Loc
        {
            get => m_primitive.Loc;
            set
            {
                var oldLoc = this.Center;
                // make the new location the center point of the port, not the top left corner.
                m_primitive.Loc = new Point(value.X - Radius, value.Y - Radius);
                //tell connections about the move.
                foreach (var connection in m_connections.Values)
                {
                    connection.MoveWith(this);
                }
            }
        }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        public Rect Bounds => m_primitive.Bounds;

        /// <summary>
        /// property for determining if the port is selected or not.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// property for getting the center of the port.
        /// </summary>
        public Point Center => m_primitive.Center;

        public List<Connection> Connections
        {
            get
            {
                var temp = new List<Connection>(m_connections.Values);
                return temp;
            }
        }

        public int Radius { get; private set; }

        public string ID { get; set; }
    }
}
