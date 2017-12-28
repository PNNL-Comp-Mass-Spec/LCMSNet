using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;

namespace FluidicsPack
{
    public sealed class FluidicsTee : FluidicsDevice
    {
        private const int VerticalLineHeight = 28;
        private const int AngledLineDiffX = 10;
        private const int AngledLineDiffY = 18;
        private const int MainWidth = 70;
        private const int MainHeight = VerticalLineHeight + AngledLineDiffY;
        private const int BottomHorizontalWidth = MainWidth - AngledLineDiffX * 2;

        public FluidicsTee()
        {
            var teeOffset = new Point(Port.PORT_DEFAULT_RADIUS * 2 + 2, 2); // Add a top and left side buffer to not crop lines
            var teePolygon = new FluidicsPolygon();

            teePolygon.AddPoint(teeOffset);                                                                                                 // Top left point
            teePolygon.AddPoint(new Point(teeOffset.X, teeOffset.Y + VerticalLineHeight));                                                  // Bottom of left vertical
            teePolygon.AddPoint(new Point(teeOffset.X + AngledLineDiffX, teeOffset.Y + MainHeight));                                        // Bottom of left angle
            teePolygon.AddPoint(new Point(teeOffset.X + AngledLineDiffX + BottomHorizontalWidth, teeOffset.Y + MainHeight));                // Right end of bottom horizontal
            teePolygon.AddPoint(new Point(teeOffset.X + MainWidth, teeOffset.Y + VerticalLineHeight));                                      // Top of right angle
            teePolygon.AddPoint(new Point(teeOffset.X + MainWidth, teeOffset.Y));                                                           // Top right point
            AddPrimitive(teePolygon);

            foreach (var p in GeneratePortLocs(teeOffset))
            {
                AddPort(p);
            }
            var Connections = new List<Tuple<int, int>> {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(2, 1)
            };
            m_info_controls_box.Width = 70;
            m_info_controls_box.Height = 46;
            ActivateState(Connections);
            Ports[0].Connections[0].Transparent = true;
            Ports[2].Connections[0].Transparent = true;
        }

        public override string StateString()
        {
            return string.Empty;
        }

        private Point[] GeneratePortLocs(Point originOffset)
        {
            var points = new Point[3];
            points[0] = new Point(originOffset.X - Port.PORT_DEFAULT_RADIUS, originOffset.Y + VerticalLineHeight / 2);
            points[1] = new Point(originOffset.X + MainWidth / 2, originOffset.Y + MainHeight + Port.PORT_DEFAULT_RADIUS);
            points[2] = new Point(originOffset.X + MainWidth + Port.PORT_DEFAULT_RADIUS, originOffset.Y + VerticalLineHeight / 2);
            return points;
        }

        public override int CurrentState
        {
            get
            {
                return -1;
            }
            set
            {
                //do nothing
            }
        }

        public override void ActivateState(int state)
        {
            //do nothing
        }

        protected override void SetDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            //do nothing
        }

        protected override void ClearDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            // do nothing
        }

        public override void Select(Point mouse_location)
        {
            if (Contains(mouse_location))
                Selected = true;
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());

        }

        public override bool Contains(Point location)
        {
            var contains = false;
            var minX = m_primitives.Min(z => z.Loc.X);
            var maxX = m_primitives.Max(z => z.Loc.X + z.Size.Width);
            var minY = m_primitives.Min(z => z.Loc.Y);
            var maxY = m_primitives.Max(z => z.Loc.Y + z.Size.Height);
            if ((minX - 10 <= location.X && location.X <= maxX + 10) && (minY - 10 <= location.Y && location.Y <= maxY + 10))
            {
                contains = true;
            }
            return contains;
        }

        protected override Rect UpdateControlBoxLocation()
        {
            var padding = 10;
            var left = m_primitives.Min(x => x.Loc.X);
            var maxYDevice = m_primitives.Max(x => x.Loc.Y + x.Size.Height);
            var maxYPort = m_portList.Max(x => x.Loc.Y + x.Radius);
            var maxY = Math.Max(maxYDevice, maxYPort);
            return new Rect(left, maxY + padding, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        //event for when device changes
        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
    }
}
