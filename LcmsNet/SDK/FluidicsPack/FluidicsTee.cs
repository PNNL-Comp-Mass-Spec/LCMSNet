using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;

namespace FluidicsPack
{
    public sealed class FluidicsTee:FluidicsDevice
    {
        public FluidicsTee()
        {
            var teePolygon = new FluidicsPolygon();
            teePolygon.AddPoint(new Point(0,0));
            teePolygon.AddPoint(new Point(0, 29));

            teePolygon.AddPoint(new Point(0, 28));
            teePolygon.AddPoint(new Point(10, 46));

            teePolygon.AddPoint(new Point(10, 46));
            teePolygon.AddPoint(new Point(58, 46));

            teePolygon.AddPoint(new Point(57, 46));
            teePolygon.AddPoint(new Point(70, 26));

            teePolygon.AddPoint(new Point(70, 27));
            teePolygon.AddPoint(new Point(70, 0));

            teePolygon.AddPoint(new Point(70, 0));
            teePolygon.AddPoint(new Point(0, 0));
            AddPrimitive(teePolygon);
            foreach (var p in GeneratePortLocs())
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

        private Point[] GeneratePortLocs()
        {
            var points = new Point[3];
            points[0] = new Point(0 - Port.PORT_DEFAULT_RADIUS, 29/2);
            points[1] = new Point( (10 + 58)/ 2, 46 + Port.PORT_DEFAULT_RADIUS);
            points[2] = new Point(70 + Port.PORT_DEFAULT_RADIUS, (27 + 0) /2);
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

            return m_primitives[0].Contains(location, 5);
        }

        protected override Rectangle UpdateControlBoxLocation()
        {
            var padding = 5;
            var left = m_primitives.Min(x => x.Loc.X);
            var top = m_primitives.Min(x => x.Loc.Y);
            return new Rectangle(left, top + padding, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        /// <summary>
        /// create location of a string to be drawn in the control box.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="stringWidth"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected override Point CreateStringLocation(int y, float stringWidth, float scale)
        {

            return new Point((int)((m_info_controls_box.X * scale + (m_info_controls_box.Size.Width * scale / 2)) - (stringWidth / 2)),
                    (int)(y + 2));
        }
        //event for when device changes
        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
    }
}
