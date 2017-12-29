using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Devices;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public sealed class FluidicsColumnGlyph : FluidicsDevice, IFluidicsDevice
    {
        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        private IDevice m_device;

        private const int RectWidth = 20;
        private const int RectHeight = 200;

        public FluidicsColumnGlyph()
        {
            var newsize = new Size(RectWidth, RectHeight);

            var offset = new Point(2, Port.PORT_DEFAULT_RADIUS + 2);
            var rect = new FluidicsRectangle(offset, newsize, Colors.Black, Brushes.White);

            AddPrimitive(rect);

            var points = GeneratePortLocs(offset);
            var inputPort = new Port(points[0], this)
            {
                Source = false,
                Sink = false
            };

            var exitPort = new Port(points[1], this)
            {
                Source = false,
                Sink = false
            };

            AddPort(inputPort);
            AddPort(exitPort);
            ConnectionManager.GetConnectionManager.Connect(inputPort, exitPort, this);
            var c = ConnectionManager.GetConnectionManager.FindConnection(inputPort, exitPort);
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

        private Point[] GeneratePortLocs(Point offset)
        {
            var points = new Point[2];
            points[0] = new Point(offset.X + (RectWidth / 2), offset.Y);
            points[1] = new Point(offset.X + (RectWidth / 2), offset.Y + RectHeight);

            return points;
        }

        /// <summary>
        /// draw controls to screen override of base fluidicsdevice method
        /// </summary>
        /// <param name="g"></param>
        /// <param name="alpha"></param>
        /// <param name="scale"></param>
        protected override void DrawControls(DrawingContext g, byte alpha, float scale)
        {
            var realColor = Color.FromArgb(alpha, Colors.Black.R, Colors.Black.G, Colors.Black.B);

            var br = new SolidColorBrush(realColor);
            //determine font size, used to scale font with graphics primitives
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);

            var nameFont = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            // draw name to screen
            var name = DeviceName;
            var deviceNameText = new FormattedText(name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, (11.0F * stringScale) * (96.0 / 72.0), br);
            m_info_controls_box = UpdateControlBoxLocation();
            //place the name at the top middle of the box
            var nameLocation = CreateStringLocation((int)(m_info_controls_box.Y * scale), deviceNameText.Width, scale);
            g.PushTransform(new RotateTransform(90, Ports[0].Center.X, nameLocation.Y + deviceNameText.Height / 2));
            g.DrawText(deviceNameText, nameLocation);
            g.Pop();

            if (Math.Abs(scale - 1) <= float.Epsilon)
            {
                RenderedOnceFullScale = true;
                lastRenderedUnscaledControlsBounds = new Rect(nameLocation.Y, nameLocation.X, deviceNameText.Height, deviceNameText.Width);
            }
        }

        /// <summary>
        /// update the location of the control box.
        /// </summary>
        /// <returns></returns>
        protected override Rect UpdateControlBoxLocation()
        {
            var top = Ports[0].Center.Y + Ports[0].Radius;
            var x = Ports[0].Center.X;
            return new Rect(x, top, m_info_controls_box.Width, m_info_controls_box.Height);
        }

        /// <summary>
        /// create location of a string to be drawn in the control box.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="stringWidth"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected override Point CreateStringLocation(int y, double stringWidth, float scale)
        {
            // We don't really care about the width, we just need to make sure we are positioned properly for the rotate
            return new Point((m_info_controls_box.X * scale) - 10, (y + 10));
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
