using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Graphic;

namespace FluidicsSDK.Base
{
    public sealed class Connection : IRenderable
    {
        // the list of line primitives that make up the device
        private List<GraphicsPrimitive> graphicPrimitives;

        // the device the connection belongs to, if it is a connection internal to a device.
        private FluidicsDevice m_device;

        //max  variance is used to give users leeway when attempting to click on a connection, so they do not have to be pixel accurate with their clicks.
        // 400 because the formula tell if a point is on a line uses derivatives, and 400 gives a few pixels worth of inaccuracy to users.
        const int MAX_VARIANCE = 800;

        // an id to identify the connection
        private readonly long m_id;

        //we want each connection to have a unique id, this tracks the next available id..first connection made will get ID 0, next connection 1, next connection 2, etc.
        //no need for it to be a GUUID or such, since it's incredibly unlikely that any program would need 2^64 -1 ports.
        private static long availableId;

        /// <summary>
        /// empty constructor..does not create a useful connection, must still have
        /// </summary>
        public Connection()
        {
            //assign the availableId to this, and then increment it so the next class gets availableId+1 as its id.
            m_id = availableId++;
            graphicPrimitives = new List<GraphicsPrimitive>();
            ConnectionStyle = ConnectionStyles.Standard;
            //TODO: Improve line start and end points so they touch the edge of the port circle at the proper points for the direction
            //that the line will go.
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="from">a classPort object representing where the connection is coming from</param>
        /// <param name="to">a classPort object repsenting where the connection is going to</param>
        /// <param name="device"></param>
        /// <param name="style"></param>
        public Connection(Port from, Port to, FluidicsDevice device, ConnectionStyles? style = null)
        {
            //assign the availableId to this, and then increment it so the next class gets availableId+1 as its id.
            m_id = availableId++;
            from.AddConnection(m_id, this);
            to.AddConnection(m_id, this);
            ConnectionStyle = style ?? ConnectionStyles.Standard;
            graphicPrimitives = SetupLines(from, to);
            P1 = from;
            P2 = to;
            InternalConnectionOf = device;
        }

        /// <summary>
        /// offset the line so that it doesn't enter the port graphics
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private List<GraphicsPrimitive> SetupLines(Port from, Port to)
        {
            var lines = new List<GraphicsPrimitive>();
            if (ConnectionStyle == ConnectionStyles.Elbow)
            {
                var diffX = Math.Abs(to.Center.X - from.Center.X);
                var diffY = Math.Abs(to.Center.Y - from.Center.Y);
                var midPoint = new Point(0, 0);
                if (diffY > diffX)
                {
                    midPoint = new Point(from.Center.X, to.Center.Y);
                }
                else if (diffX > diffY)
                {
                    midPoint = new Point(to.Center.X, from.Center.Y);
                }
                var line = new FluidicsLine(from.Center, midPoint);
                lines.Add(line);
                // elbow off to port "to"
                line = new FluidicsLine(midPoint, to.Center);
                lines.Add(line);
            }
            else
            {
                var line = new FluidicsLine(new Point(from.Center.X, from.Center.Y), new Point(to.Center.X, to.Center.Y));
                lines.Add(line);
            }
            return lines;
        }

        /// <summary>
        /// Render all connections
        /// </summary>
        /// <param name="g">a System.Windows.Media DrawingContext object</param>
        /// <param name="alpha">a integer representing the alpha value to draw the connections with</param>
        /// <param name="scale">a float representing the value to scale the connections by</param>
        public void Render(DrawingContext g, byte alpha, float scale)
        {
            if (!Transparent)
            {
                foreach (var singleLine in graphicPrimitives)
                {
                    var line = (FluidicsLine)singleLine;
                    line.Render(g, alpha, scale, Selected, false);
                }
            }
        }

        /// <summary>
        /// move the connection with the port
        /// </summary>
        /// <param name="port">a classPort object</param>
        public void MoveWith(Port port)
        {
            if (ConnectionStyle == ConnectionStyles.Elbow)
            {
                var diffX = Math.Abs(P2.Center.X - P1.Center.X);
                var diffY = Math.Abs(P2.Center.Y - P1.Center.Y);
                var midPoint = new Point(0, 0);
                if (diffY > diffX)
                {
                    midPoint = new Point(P1.Center.X, P2.Center.Y);
                }
                else if (diffX > diffY)
                {
                    midPoint = new Point(P2.Center.X, P1.Center.Y);
                }
                var lineA = graphicPrimitives[0] as FluidicsLine;
                if (lineA != null)
                {
                    lineA.Origin = P1.Center;
                    lineA.Term = midPoint;
                }

                var lineB = graphicPrimitives[1] as FluidicsLine;
                if (lineB != null)
                {
                    lineB.Origin = midPoint;
                    lineB.Term = P2.Center;
                }
            }
            else
            {
                var line = graphicPrimitives[0] as FluidicsLine;
                if (line != null)
                {
                    line.Origin = P1.Center;
                    line.Term = P2.Center;
                }
            }

            //if (port.ID == P1.ID)
            //{
            //    List<GraphicsPrimitive> prims = SetupLines(port, P2);
            //    for (int i = 0; i < prims.Count; i++)
            //    {
            //        m_prims[i] = prims[i];
            //    }
            //}
            //else if (port.ID == P2.ID)
            //{
            //     List<GraphicsPrimitive> prims = SetupLines(P1,port);
            //    for (int i = 0; i < prims.Count; i++)
            //    {
            //        m_prims[i] = prims[i];
            //    }
            //}
            //else
            //{
            //    throw new Exception("Attempt to move connection not belonging to moving port detected!");
            //}
        }

        /// <summary>
        /// destroy this connection
        /// </summary>
        public void Destroy()
        {
            P1.RemoveConnection(m_id);
            P2.RemoveConnection(m_id);
            InternalConnectionOf = null;
        }

        /// <summary>
        ///  Determine if the connection occupies a location on screen
        /// </summary>
        /// <param name="location">a System.Windows.Point object</param>
        /// <returns>true if so, false if not</returns>
        public bool OnPoint(Point location)
        {
            //check to see if the point is on any of the lines that make up the connection
            if (!Transparent) // if the connection is transparent, we don't want to be able to select it
            {
                foreach (var singleLine in graphicPrimitives)
                {
                    var line = (FluidicsLine)singleLine;
                    var existsOnPoint = line.Contains(location, MAX_VARIANCE);
                    if (existsOnPoint)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Select the connection
        /// </summary>
        public void Select()
        {
            if (!Transparent)
            {
                Selected = true;
            }
        }

        /// <summary>
        /// Deselect the connection
        /// </summary>
        public void Deselect()
        {
            Selected = false;
        }


        public void DoubleClicked()
        {
            if (!Transparent) //if transparent, we don't want to be able to do this
            {
                if (ConnectionStyle == ConnectionStyles.Standard)
                {
                    ConnectionStyle = ConnectionStyles.Elbow;
                }
                else
                {
                    ConnectionStyle = ConnectionStyles.Standard;
                }
                graphicPrimitives.Clear();
                graphicPrimitives = SetupLines(P1, P2);
            }
        }

        public Port FindOppositeEndOfConnection(Port p)
        {
            if (p.ID == P1.ID)
            {
                return P2;
            }

            return P1;
        }


        /// <summary>
        ///  This is used to make connections that are invisible.
        /// </summary>
        public bool Transparent { get; set; }

        /// <summary>
        /// Property determining the Inbound port
        /// </summary>
        public Port P1 { get; }

        /// <summary>
        /// Property determing the Outbound port.
        /// </summary>
        public Port P2 { get; }

        /// <summary>
        /// Property to determine if this is a connection internal to a device
        /// </summary>
        public FluidicsDevice InternalConnectionOf
        {
            get => m_device;
            private set
            {
                if (value != null)
                {
                    m_device = value;
                }
            }
        }

        /// <summary>
        /// Property to determine the color of the connection, set only
        /// </summary>
        public Color Color
        {
            set
            {
                foreach (var prim in graphicPrimitives)
                {
                    prim.Color = value;
                }
            }
        }

        /// <summary>
        /// Property to determine if the connection is selected or not.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// ID to identify connection
        /// </summary>
        public long ID => m_id;

        /// <summary>
        /// Internal volume of the connection
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// length property of connection
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// diameter of the connection
        /// </summary>
        public double Diameter { get; set; }

        public ConnectionStyles ConnectionStyle { get; set; }
    }
}
