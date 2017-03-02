using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;

namespace DemoPluginLibrary
{
    public class FluidicsTee:FluidicsDevice
    {
        Dictionary<string, FluidicsLine> m_primLines;
        public FluidicsTee()
        {
            m_primLines = new Dictionary<string, FluidicsLine>();
            //left line going down
            FluidicsLine newLine = new FluidicsLine(new Point(0,0), new Point(0,29));
            AddPrimitive(newLine);
            m_primLines["left"] = newLine;
            //left line angled in
            newLine = new FluidicsLine(new Point(0, 28), new Point(10, 46));
            AddPrimitive(newLine);
            m_primLines["leftAngled"] = newLine;
            //bottom line across
            newLine = new FluidicsLine(new Point(10, 46), new Point(58, 46));
            AddPrimitive(newLine);
            m_primLines["bottom"] = newLine;
            //right line angled in
            newLine = new FluidicsLine(new Point(57, 46), new Point(70, 26));
            AddPrimitive(newLine);
            m_primLines["rightAngled"] = newLine;
            // right line going up
            newLine = new FluidicsLine(new Point(70,27), new Point(70,0));
            AddPrimitive(newLine);
            m_primLines["right"] = newLine;
            //top line going from right line to left line
            newLine = new FluidicsLine(new Point(70,0), new Point(0,0));
            AddPrimitive(newLine);
            m_primLines["top"] = newLine;
            foreach (Point p in GeneratePortLocs())
            {
                base.AddPort(p);
            }
            List<Tuple<int, int>> Connections = new List<Tuple<int, int>>();
            Connections.Add(new Tuple<int, int>(0, 1));
            Connections.Add(new Tuple<int, int>(2, 1));
            m_info_controls_box.Width = 70;
            m_info_controls_box.Height = 46;
            ActivateState(Connections);
        }

        public override string StateString()
        {
            return string.Empty;
        }

        protected Point[] GeneratePortLocs()
        {
            Point[] points = new Point[3];
            Point orig, term;
            orig = m_primLines["left"].Origin;
            term = m_primLines["left"].Term;
            points[0] = new Point(orig.X - Port.PORT_DEFAULT_RADIUS, (orig.Y + term.Y)/2);
            orig = m_primLines["bottom"].Origin;
            term = m_primLines["bottom"].Term;
            points[1] = new Point((orig.X + term.X) / 2, orig.Y + Port.PORT_DEFAULT_RADIUS);
            orig = m_primLines["right"].Origin;
            term = m_primLines["right"].Term;
            points[2] = new Point(orig.X + Port.PORT_DEFAULT_RADIUS, (orig.Y + term.Y) /2);
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
                if (DeviceChanged != null)
                {
                    DeviceChanged(this, new FluidicsDevChangeEventArgs());
                }

        }

        public override bool Contains(Point location)
        {
            int minx = m_primitives.Min(x => x.Loc.X);
            int miny = m_primitives.Min(x => x.Loc.Y);
            int maxx = m_primitives.Max(x => x.Loc.X);
            int maxy = m_primitives.Max(x => x.Loc.Y);
            Rectangle rect = new Rectangle(minx, miny, maxx - minx, maxy - miny);
            return rect.Contains(location);
        }

        protected override Rectangle UpdateControlBoxLocation()
        {
            int padding = 10;
            int left = m_primitives.Min(x => x.Loc.X);
            int maxyDevice = m_primitives.Max(x => x.Loc.Y);
            int maxyPorts = m_portList.Max(x => x.Loc.Y + x.Radius);
            return new Rectangle(left, (maxyDevice < maxyPorts ? maxyPorts : maxyDevice) + padding, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        //event for when device changes
        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
    }
}
