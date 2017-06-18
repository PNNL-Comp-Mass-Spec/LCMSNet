using System.Linq;
using System.Windows;
using System.Windows.Media;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;

namespace DemoPluginLibrary
{
    public sealed class FluidicsSprayNeedleWpf : FluidicsDeviceWpf
    {
        #region Members
        private const int MAIN_RECT_WIDTH = 50;
        private const int MAIN_RECT_HEIGHT = 20;
        #endregion

        #region Methods
        public FluidicsSprayNeedleWpf()
        {
            var mainStartPoint = new Point(0, 0);
            var leftMostStartPoint = new Point(-(MAIN_RECT_WIDTH / 3) - 5, 0);
            var rightMostStartPoint = new Point(MAIN_RECT_WIDTH + 5, 0);

            //main rectangle
            AddRectangle(mainStartPoint, new Size(MAIN_RECT_WIDTH, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            //left most rectangle
            AddRectangle(leftMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // bottom left parallelogram + connecting line
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X, leftMostStartPoint.Y + MAIN_RECT_HEIGHT), new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT),
                new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10),
                new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));

            //bottom middle trapezoid
            AddPrimitive(new FluidicsLineWpf(new Point(mainStartPoint.X, mainStartPoint.Y + MAIN_RECT_HEIGHT), new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y + MAIN_RECT_HEIGHT),
                new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10),
                new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10)));

            // bottom right parallelogram + connecting line
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X, rightMostStartPoint.Y + MAIN_RECT_HEIGHT), new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT),
                new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10),
                new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));

            // rightmost rectangle
            AddRectangle(rightMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // upper left parallelogram + connecting line
            AddPrimitive(new FluidicsLineWpf(leftMostStartPoint, new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y),
                new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10), new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 2, leftMostStartPoint.Y - 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y - 5)));

            // upper middle trapezoid
            AddPrimitive(new FluidicsLineWpf(mainStartPoint, new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 4, mainStartPoint.Y - 10)));

            // upper right parallelogram + connecting line
            AddPrimitive(new FluidicsLineWpf(rightMostStartPoint, new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y),
                new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10),
                new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10)));
            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 2, rightMostStartPoint.Y - 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y - 5)));

            // needle
            var needleStartPoint = new Point(leftMostStartPoint.X - MAIN_RECT_WIDTH / 2, MAIN_RECT_HEIGHT / 3);
            AddRectangle(needleStartPoint, new Size(MAIN_RECT_WIDTH / 2, MAIN_RECT_HEIGHT / 2), Colors.Black, Brushes.White);
            //needle tip
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X - MAIN_RECT_WIDTH / 2, MAIN_RECT_HEIGHT / 3), new Point(needleStartPoint.X - 25, (MAIN_RECT_HEIGHT / 3) +
                                                                                                                                                          MAIN_RECT_HEIGHT / 4)));
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X - MAIN_RECT_WIDTH / 2, MAIN_RECT_HEIGHT / 3 + (MAIN_RECT_HEIGHT / 2)), new Point(needleStartPoint.X - 25,
                (MAIN_RECT_HEIGHT / 3) + MAIN_RECT_HEIGHT / 4)));

            AddPort(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 12, rightMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            m_info_controls_box.X = leftMostStartPoint.X;
            m_info_controls_box.Y = m_primitives.Max(x => x.Loc.Y);
            Ports[0].Sink = true;
        }

        public override bool Contains(Point location)
        {
            var contains = false;
            var minX = m_primitives.Min(z => z.Loc.X);
            var maxX = m_primitives.Max(z => z.Loc.X);
            var minY = m_primitives.Min(z => z.Loc.Y);
            var maxY = m_primitives.Max(z => z.Loc.Y);
            if ((minX - 20 <= location.X && location.X <= maxX + 20) && (minY - 20 <= location.Y && location.Y <= maxY + 20))
            {
                contains = true;
            }
            return contains;
        }

        public override void ActivateState(int state)
        {
        }

        protected override void SetDevice(IDevice device)
        {
        }

        protected override void ClearDevice(IDevice device)
        {
        }

        public override string StateString()
        {
            return string.Empty;
        }
        #endregion

        #region Properties

        public override int CurrentState
        {
            get
            {
                //pumps don't have a state of this kind.
                return -1;
            }
            set
            {
                //do nothing
            }
        }

        #endregion
    }
}
