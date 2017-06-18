using System.Linq;
using System.Windows;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using System.Windows.Media;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public sealed class FluidicsUnionWpf : FluidicsDeviceWpf
    {
        #region Members
        private const int MAIN_RECT_WIDTH = 50;
        private const int MAIN_RECT_HEIGHT = 20;
        #endregion

        #region Methods
        public FluidicsUnionWpf()
        {
            var mainStartPoint = new Point(0, 0);
            var leftMostStartPoint = new Point(-(MAIN_RECT_WIDTH / 3) - 5, 0);
            var rightMostStartPoint = new Point(MAIN_RECT_WIDTH + 5, 0);

            //main rectangle
            AddRectangle(mainStartPoint, new Size(MAIN_RECT_WIDTH, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            //left most rectangle
            AddRectangle(leftMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // bottom left parallelogram + connecting line
            var leftPara = new FluidicsPolygonWpf();
            leftPara.AddPoint(new Point(leftMostStartPoint.X, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            leftPara.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));

            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));

            AddPrimitive(leftPara);

            //bottom middle trapezoid
            var botmid = new FluidicsPolygonWpf();
            botmid.AddPoint(new Point(mainStartPoint.X, mainStartPoint.Y + MAIN_RECT_HEIGHT));
            botmid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botmid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y + MAIN_RECT_HEIGHT));
            AddPrimitive(botmid);

            // bottom right parallelogram + connecting line
            var botRt = new FluidicsPolygonWpf();
            botRt.AddPoint(new Point(rightMostStartPoint.X, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));
            botRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));

            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));
            AddPrimitive(botRt);

            // rightmost rectangle
            AddRectangle(rightMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // upper left parallelogram + connecting line
            var upLft = new FluidicsPolygonWpf();
            upLft.AddPoint(leftMostStartPoint);
            upLft.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10));

            upLft.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10));
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10));

            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10));
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y));
            AddPrimitive(upLft);
            AddPrimitive(new FluidicsLineWpf(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 2, leftMostStartPoint.Y - 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y - 5)));

            // upper middle trapezoid
            var upMid = new FluidicsPolygonWpf();
            upMid.AddPoint(mainStartPoint);
            upMid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10));

            upMid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10));
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 4, mainStartPoint.Y - 10));

            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y - 10));
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y));
            AddPrimitive(upMid);

            // upper right parallelogram + connecting line
            var upRt = new FluidicsPolygonWpf();
            upRt.AddPoint(rightMostStartPoint);
            upRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10));

            upRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10));
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10));

            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10));
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y));
            AddPrimitive(upRt);

            AddPrimitive(new FluidicsLineWpf(new Point(rightMostStartPoint.X - 2, rightMostStartPoint.Y - 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y - 5)));

            AddPort(new Point(leftMostStartPoint.X - 12, leftMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            AddPort(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 12, rightMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            ConnectionManagerWpf.GetConnectionManager.Connect(Ports[0], Ports[1], this);
            var c = ConnectionManagerWpf.GetConnectionManager.FindConnection(Ports[0], Ports[1]);
            c.Transparent = true;
        }

        public override bool Contains(Point location)
        {
            var contains = false;
            var minX = m_primitives.Min(z => z.Loc.X);
            var maxX = m_primitives.Max(z => z.Loc.X);
            var minY = m_primitives.Min(z => z.Loc.Y);
            var maxY = m_primitives.Max(z => z.Loc.Y);
            if ((minX <= location.X && location.X <= maxX) && (minY <= location.Y && location.Y <= maxY))
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
