using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetSDK;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Represents a valve with two positions
    /// </summary>
    // can technically have 1-13 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public abstract class TwoPositionValve : FluidicsDevice
    {
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        const int m_radius = 75;

        // number of ports the valve has
        const float M_DISTFROMCENTER = .75f;

        private readonly int m_numberOfPorts;
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> m_states;
        protected TwoPositionState m_currentState;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="portNumberingClockwise"></param>
        protected TwoPositionValve(int numberOfPorts, int xOffset = 2, int yOffset = 2, bool portNumberingClockwise = false) :
            base()
        {
            Offset = new Point(xOffset, yOffset);
            AddCircle(Offset, m_radius, Colors.Black, Brushes.White, fill: true);
            m_info_controls_box = new Rect(Loc.X, Loc.Y + (int) Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);
            m_numberOfPorts = numberOfPorts;
            var portLocs = GeneratePortLocs();
            for (var i = 0; i < portLocs.Length; i++)
            {
                var number = portNumberingClockwise ? i + 1 : (portLocs.Length - i) % portLocs.Length + 1;
                AddPort(portLocs[i], number);
            }
            MaxVariance = 5;
            Sink = false;
            Source = false;
        }

        /// <summary>
        /// The x,y offset of the main drawing to compensate for line width, ports, etc.
        /// </summary>
        protected readonly Point Offset;

        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of Point types</returns>
        private Point[] GeneratePortLocs()
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at 0, pi/2, pi, and 3pi/2.
            var angle = (2 * Math.PI) / m_numberOfPorts;
            var points = new Point[m_numberOfPorts];
            for (var i = 0; i < m_numberOfPorts; i++)
            {
                // Position the first port above the center point, shift all other ports to correct locations after.
                var currentAngle = (Math.PI / 2) + (angle * i) + Math.PI;
                // place them on a circle at a radius of m_radius* 3/4 from the center
                // of the valve.
                var x = (int) ((m_radius * M_DISTFROMCENTER) * Math.Cos(currentAngle) + Center.X);
                var y = (int) ((m_radius * M_DISTFROMCENTER) * Math.Sin(currentAngle) + Center.Y);

                points[i] = new Point(x, y);
            }
            return points;
        }

        public override string StateString()
        {
            return m_currentState.GetEnumDescription();
        }

        public override void ActivateState(int state)
        {
            var requestedState = (TwoPositionState) state;
            m_currentState = requestedState;
            if (m_currentState != TwoPositionState.Unknown)
            {
                ActivateState(m_states[requestedState]);
            }
        }

        /// <summary>
        /// generates a single 'state' of connections, by connecting pairs of ports, starting
        /// with the ports at startingPortIndex and startingPortIndex+1, then startingPortIndex+2 and startingPortIndex+3,
        /// etc.
        /// </summary>
        /// <param name="startingPortIndex">an index into m_ports</param>
        /// <param name="endingPortIndex">an index into m_ports</param>
        /// <returns>a list of int,int tuples</returns>
        protected List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
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
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center => ((Graphic.FluidicsCircle) m_primitives[PRIMARY_PRIMITIVE]).Center;

        public override int CurrentState
        {
            get => (int) m_currentState;
            set => m_currentState = (TwoPositionState) value;
        }
    }
}
