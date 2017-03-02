/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/16/2013
 *
 * Last Modified 10/14/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Graphic;
using System.Drawing;
using FluidicsSDK.Managers;

namespace FluidicsSDK.Base
{
    public class Port:IRenderable
    {
        #region Members
        //fluids currently passing through port/being injected/whatever.
        private List<Fluid> m_fluids;
        //graphics primitive repsenting the port
        private FluidicsCircle m_primitive;
        private int m_radius;
        private string m_id;
        private const int MAX_PIXEL_VARIANCE = 7;
        private Dictionary<long, Connection> m_connections;
        public const int PORT_DEFAULT_RADIUS = 10;
        #endregion

        #region Methods

            /// <summary>
            /// default constructor
            /// </summary>
            private Port()
            {
                Point f = new Point(0,0);
                DefinePort(f, null);
            }

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="loc">a System.Drawing.Point representing the location of the port on screen</param>
            /// <param name="parent">a classFluidicsDevice representing what device the port belongs to</param>
            public Port(Point loc, FluidicsDevice parent)
            {
                DefinePort(loc, parent);
            }

            protected virtual void DefinePort(Point loc, FluidicsDevice parent)
            {
                Radius = PORT_DEFAULT_RADIUS;
                m_connections = new Dictionary<long, Connection>();

                // this makes the provided location the center point of the port, instead of the top left corner.
                Point trueLoc = new Point(loc.X - Radius, loc.Y - Radius);
                ParentDevice = parent;
                m_fluids = new List<Fluid>();
                m_primitive = new FluidicsCircle(trueLoc, Color.Black, Brushes.White, Radius);
                m_primitive.Fill = true;
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
            /// <param name="g">a System.Drawing.Graphics object</param>
            /// <param name="alpha">an integer representing the alpha value to draw the port at</param>
            /// <param name="scale">a float representing how much to scale the port by</param>
            public void Render(Graphics g, int alpha, float scale)
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
            /// <param name="location">a System.Drawing.Point to check</param>
            /// <returns> true if the port contains that point, false otherwise</returns>
            internal bool Contains(Point location)
            {
                //standard pythagorean to determine if point is in/on the circle
                int dist = (int)Math.Sqrt(Math.Pow((Center.X - location.X) , 2) + Math.Pow((Center.Y - location.Y)  ,2));
                return (dist <= (m_radius + MAX_PIXEL_VARIANCE));
            }


            public void MoveBy(Point amtToMove)
            {
                //we are moving the center point of the circle, but locations are based on the upper left corner, so adjust for the radius
                Point newLoc = new Point(Loc.X + amtToMove.X + m_radius, Loc.Y + amtToMove.Y + m_radius);
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
            public FluidicsDevice ParentDevice
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
                    Point oldLoc = this.Center;
                    // make the new location the center point of the port, not the top left corner.
                    m_primitive.Loc = new Point(value.X -m_radius, value.Y - m_radius);
                    //tell connections about the move.
                    foreach (Connection connection in m_connections.Values)
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
            public Point Center
            {
                get
                {
                    return m_primitive.Center;
                }
            }

            public List<Connection> Connections
            {
                get
                {
                    List<Connection> temp = new List<Connection>(m_connections.Values);
                    return temp;
                }
                private set {  }
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

            public string ID
            {
                get
                {
                    return m_id;
                }
                set
                {
                    m_id = value;
                }
            }
                    
        #endregion
    }
}