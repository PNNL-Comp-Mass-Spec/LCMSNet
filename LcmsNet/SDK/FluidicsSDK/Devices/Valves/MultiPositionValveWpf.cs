using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetDataClasses.Devices;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Represents a valve with multiple positions
    /// </summary>
    // can technically have 1-16 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public class MultiPositionValveWpf : FluidicsDeviceWpf
    {
        #region Members
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        private const int m_radius = 75;
        // number of ports the valve has
        private readonly int m_numberOfPorts;
        protected Dictionary<int, List<Tuple<int, int>>> m_states;
        protected int m_currentState;
        #endregion

        #region Methods
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        public MultiPositionValveWpf(int numberOfPorts):
            base()
        {
            AddCircle(new Point(0, 0), m_radius, Colors.Black, Brushes.White, fill: true);
            m_info_controls_box = new Rect(Loc.X, Loc.Y + (int)Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);
            m_numberOfPorts = numberOfPorts;
            var portLocs = GeneratePortLocs();
            foreach (var p in portLocs)
            {
                AddPort(p);
            }
            MaxVariance = 5;
            Source = false;
            Sink = false;
        }

        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of System.Drawing.Point types</returns>
        private Point[] GeneratePortLocs()
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at pi/2, pi, 3pi/2, and 2pi.
            var currentAngle = (3 * Math.PI) / 2;
            var angle = (2 * Math.PI) / (m_numberOfPorts - 1);

            var points = new Point[m_numberOfPorts];
            points[0] = new Point(Center.X, Center.Y);
            for (var i = 1; i < m_numberOfPorts; i++)
            {
                // place them on a circle at a radius of m_radius/2 from the center
                // of the valve.
                var x = (int)((m_radius * .75) * Math.Cos(currentAngle)) + Center.X;
                var y = (int)((m_radius * .75) * Math.Sin(currentAngle)) + Center.Y;
                points[i] = new Point(x, y);

                currentAngle -= angle;
            }
            return points;
        }

        public override string StateString()
        {
            throw new NotImplementedException("StateString() must be implemented in class inheriting MultiPositionValveBase");
        }

        public override void ActivateState(int requestedState)
        {
            m_currentState = requestedState;
            if (m_currentState != -1) // -1 is unknown state
            {
                ActivateState(m_states[requestedState]);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center => ((Graphic.FluidicsCircleWpf)m_primitives[PRIMARY_PRIMITIVE]).Center;

        public override int CurrentState
        {
            get
            {
                return m_currentState;
            }
            set
            {
                m_currentState = value;
            }
        }
        #endregion

        protected override void SetDevice(IDevice device)
        {

        }

        protected override void ClearDevice(IDevice device)
        {

        }
    }
}
