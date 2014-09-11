using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using FluidicsSDK.Managers;
using FluidicsSDK.Base;
using System.Drawing;
using FluidicsSDK.Graphic;

namespace FluidicsPack
{

    public class FluidicsColumnGlyph : FluidicsDevice , IFluidicsDevice
    {        
        public event EventHandler DeviceSaveRequired;
        private IDevice m_device;
        private Port m_inputPort;
        private Port m_exitPort;

        public FluidicsColumnGlyph()
        {
            Size newsize = new Size(20, 200);

            GraphicsPrimitive primitive = new FluidicsRectangle(new Point(0, 0),
                                                                newsize,
                                                                Color.Black,
                                                                 Brushes.White);

            AddPrimitive(primitive);

            Point[] points      = GeneratePortLocs();
            m_inputPort         = new Port(points[0], this);
            m_inputPort.Source  = false;
            m_inputPort.Sink = false;

            m_exitPort        = new Port(points[1], this);
            m_exitPort.Source = false;
            m_exitPort.Sink   = false;

            AddPort(m_inputPort);
            AddPort(m_exitPort);
            ConnectionManager.GetConnectionManager.Connect(m_inputPort, m_exitPort, this);
            Connection c = ConnectionManager.GetConnectionManager.FindConnection(m_inputPort, m_exitPort);
            c.Transparent = true;
            m_info_controls_box.Width = 20;
            m_info_controls_box.Height = 200;

            
        }

        protected override void SetDevice(IDevice device) 
        {
            m_device = device;
        }

        protected override void ClearDevice(IDevice device)
        {
            m_device = null;
        }

        public override void ActivateState(int state)
        {
            
        }

        protected Point[] GeneratePortLocs()
        {
            Point[] points  = new Point[2];
            points[0] = new Point(Convert.ToInt32(Loc.X + (Size.Width / 2)), Loc.Y);
            points[1] = new Point(Convert.ToInt32(Loc.X + (Size.Width / 2)), Loc.Y + Convert.ToInt32(Size.Height - m_info_controls_box.Size.Height));

            return points;
        }


        /// <summary>
        /// draw controls to screen override of base fluidicsdevice method
        /// </summary>
        /// <param name="g"></param>
        /// <param name="alpha"></param>
        /// <param name="scale"></param>
        protected override void DrawControls(Graphics g, int alpha, float scale)
        {
            Color realColor = Color.FromArgb(alpha, Color.Black.R, Color.Black.G, Color.Black.B);
            using (IDisposable p = new Pen(realColor), b = new SolidBrush(realColor))
            {
                var pen = p as Pen;
                var br = b as SolidBrush;
                //determine font size, used to scale font with graphics primitives
                int stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);
                using (Font stringFont = new Font("Calibri", 11 + stringScale))
                {
                    // draw name to screen
                    string name = DeviceName;                    

                    m_info_controls_box = UpdateControlBoxLocation();

                    //place the name at the top middle of the box
                    System.Drawing.StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical);
                    SizeF nameSize = g.MeasureString(name, stringFont);

                    Point nameLocation = CreateStringLocation((int)(m_info_controls_box.Y * scale), nameSize.Height, scale);                    
                    g.DrawString(name, stringFont, br, nameLocation, sf);                   
                }
            }
        }


        /// <summary>
        /// update the location of the control box.
        /// </summary>
        /// <returns></returns>
        protected override Rectangle UpdateControlBoxLocation()
        {
            int top = Ports[0].Center.Y + Ports[0].Radius;
            int x = Ports[0].Center.X;
            return new Rectangle(x, top, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        /// <summary>
        /// create location of a string to be drawn in the control box.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="stringWidth"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected override Point CreateStringLocation(int y, float stringHeight, float scale)
        {
            // The height is actually our "width" here, since we are drawing the string vertically. 
            return new Point((int)((m_info_controls_box.X * scale) - stringHeight/2),
                    (int)(y + 10));
        }

        public override string StateString()
        {
            return "";
        }

        public override int CurrentState
        {
            get;
            set;
        }
    }
}