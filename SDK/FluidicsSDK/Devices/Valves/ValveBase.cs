using System;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Base class for a valve with two or multiple positions. NOTE: Prefer <see cref="TwoPositionValve"/> <see cref="MultiPositionValve"/> over using this
    /// </summary>
    // can technically have 1-13 (or 16 for multiple position) ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public abstract class ValveBase : FluidicsDevice
    {
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        private const int _radius = 75;
        private const double _portDistanceFromCenter = .75;
        private readonly int _numberOfPerimeterPorts;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPerimeterPorts">the number of ports (two-position) or positions (multi-position) the valve will have</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        protected ValveBase(int numberOfPerimeterPorts, int xOffset = 2, int yOffset = 2) :
            base()
        {
            _numberOfPerimeterPorts = numberOfPerimeterPorts;
            Offset = new Point(xOffset, yOffset);
            AddCircle(Offset, _radius, Colors.Black, Brushes.White, fill: true);
            m_info_controls_box = new Rect(Loc.X, Loc.Y + (int)Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);

            // Example: This code is needed in the constructor of every class that inherits this class, with the appropriate parameter value
            //foreach (var point in GeneratePerimeterPortLocations(portNumberingClockwise))
            //{
            //    AddPort(point.Point, point.Number);
            //}

            MaxVariance = 5;

            Source = false;
            Sink = false;
        }

        /// <summary>
        /// The x,y offset of the main drawing to compensate for line width, ports, etc.
        /// </summary>
        protected readonly Point Offset;

        protected readonly struct PointAndNumber
        {
            public Point Point { get; }
            public int Number { get; }

            public PointAndNumber(Point point, int number)
            {
                Point = point;
                Number = number;
            }

            public PointAndNumber(double x, double y, int number)
            {
                Point = new Point(x, y);
                Number = number;
            }
        }

        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of Point types</returns>
        protected PointAndNumber[] GeneratePerimeterPortLocations(bool portNumberingClockwise = true)
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at pi/2, 0 (or 2pi), 3pi/2, and pi.
            // Drawing coordinate (0,0) is the top left corner, so we have the following state table for 4 ports:
            //            t  r     b   l   (top, right, bottom, left)
            //      x     0  1     0  -1
            //      y    -1  0     1   0
            //  angle 3pi/2  0  pi/2  pi
            // cos(a)     0  1     0  -1
            // sin(a)    -1  0     1   0
            // Also note that the unit circle has increasing values of pi counterclockwise, but we have mirrored that top-to-bottom
            // So for clockwise port ordering, we need to add the angle from the starting point
            var currentAngle = (3 * Math.PI) / 2 + (Math.PI * 2); // Necessary value for port 1 to be at the top; add 2pi to avoid negative values
            var angle = (2 * Math.PI) / _numberOfPerimeterPorts;

            var ports = new PointAndNumber[_numberOfPerimeterPorts];
            for (var i = 0; i < _numberOfPerimeterPorts; i++)
            {
                // place them on a circle at a radius of _radius/2 from the center
                // of the valve.
                var x = (int)(_radius * _portDistanceFromCenter * Math.Cos(currentAngle)) + Center.X;
                var y = (int)(_radius * _portDistanceFromCenter * Math.Sin(currentAngle)) + Center.Y;

                // Store the appropriate port numbering with the port points
                ports[i] = new PointAndNumber(x, y, i + 1);

                // Add the angle difference between ports to add the next port clockwise
                if (portNumberingClockwise)
                {
                    currentAngle += angle;
                }
                else
                {
                    currentAngle -= angle;
                }
            }
            return ports;
        }

        /// <summary>
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center => ((Graphic.FluidicsCircle)m_primitives[PRIMARY_PRIMITIVE]).Center;
    }
}
