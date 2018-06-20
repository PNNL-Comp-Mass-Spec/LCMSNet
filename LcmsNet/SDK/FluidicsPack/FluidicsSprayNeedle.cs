using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    public sealed class FluidicsSprayNeedle : FluidicsDevice
    {
        #region Members
        private const int MAIN_RECT_WIDTH = 50;
        private const int MAIN_RECT_HEIGHT = 20;
        #endregion

        #region Methods
        public FluidicsSprayNeedle()
        {
            const int startPointYOffset = 12; // Also the height of the top shape, compared to the middle rectangle, plus 2 for full line coverage
            const int needleTipWidth = 25;
            const int needleShaftWidth = MAIN_RECT_WIDTH / 2;
            const int leftNutWidth = MAIN_RECT_WIDTH / 3;
            const int nutGapWidth = 5;

            var needleTipPoint = new Point(2, (MAIN_RECT_HEIGHT / 3) + MAIN_RECT_HEIGHT / 4 + startPointYOffset);          // Needle tip point, add 2 to X for full line coverage
            var needleStartPoint = new Point(needleTipPoint.X + needleTipWidth, MAIN_RECT_HEIGHT / 3 + startPointYOffset); // Top left of needle shaft
            var leftMostStartPoint = new Point(needleStartPoint.X + needleShaftWidth, startPointYOffset);                  // Top left of left middle rectangle
            var mainStartPoint = new Point(leftMostStartPoint.X + leftNutWidth + nutGapWidth, startPointYOffset);          // Top left of center middle rectangle
            var rightMostStartPoint = new Point(mainStartPoint.X + MAIN_RECT_WIDTH + nutGapWidth, startPointYOffset);      // Top left of right middle rectangle

            //main rectangle
            AddRectangle(mainStartPoint, new Size(MAIN_RECT_WIDTH, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            //left most rectangle
            AddRectangle(leftMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // bottom left parallelogram + connecting line
            var leftPara = new FluidicsPolygon();
            leftPara.AddPoint(new Point(leftMostStartPoint.X, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));                                // Top Left
            leftPara.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));                       // Bottom Left
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)); // Bottom Right
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));          // Top Right
            AddPrimitive(leftPara);

            AddPrimitive(new FluidicsLine(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));

            //bottom middle trapezoid
            var botmid = new FluidicsPolygon();
            botmid.AddPoint(new Point(mainStartPoint.X, mainStartPoint.Y + MAIN_RECT_HEIGHT));                            // Top Left
            botmid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));                   // Bottom Left
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10)); // Bottom Right
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y + MAIN_RECT_HEIGHT));          // Top Right
            AddPrimitive(botmid);

            // bottom right parallelogram + connecting line
            var botRt = new FluidicsPolygon();
            botRt.AddPoint(new Point(rightMostStartPoint.X, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));                                // Top Left
            botRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));                       // Bottom Left
            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10)); // Bottom Right
            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));          // Top Right

            AddPrimitive(new FluidicsLine(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));
            AddPrimitive(botRt);

            // rightmost rectangle
            AddRectangle(rightMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Colors.Black, Brushes.White);

            // upper left parallelogram + connecting line
            var upLft = new FluidicsPolygon();
            upLft.AddPoint(leftMostStartPoint);                                                                   // Bottom Left
            upLft.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10));                       // Top Left
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10)); // Top Right
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y));          // Bottom Right
            AddPrimitive(upLft);
            AddPrimitive(new FluidicsLine(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 2, leftMostStartPoint.Y - 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y - 5)));

            // upper middle trapezoid
            var upMid = new FluidicsPolygon();
            upMid.AddPoint(mainStartPoint);                                                           // Bottom Left
            upMid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10));                   // Top Left
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y - 10)); // Top Right
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y));          // Bottom Right
            AddPrimitive(upMid);

            // upper right parallelogram + connecting line
            var upRt = new FluidicsPolygon();
            upRt.AddPoint(rightMostStartPoint);                                                                    // Bottom Left
            upRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10));                       // Top Left
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10)); // Top Right
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y));          // Bottom Right
            AddPrimitive(upRt);

            AddPrimitive(new FluidicsLine(new Point(rightMostStartPoint.X - 2, rightMostStartPoint.Y - 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y - 5)));

            // needle
            AddRectangle(needleStartPoint, new Size(MAIN_RECT_WIDTH / 2, MAIN_RECT_HEIGHT / 2), Colors.Black, Brushes.White);
            // needle tip
            AddPrimitive(new FluidicsLine(needleStartPoint, needleTipPoint));
            AddPrimitive(new FluidicsLine(new Point(needleStartPoint.X, needleStartPoint.Y + (MAIN_RECT_HEIGHT / 2)), needleTipPoint));
            // For a sharp pointed needle, use below code
            //var needlePoint = new FluidicsPolygon();
            //needlePoint.AddPoint(needleStartPoint);
            //needlePoint.AddPoint(needleTipPoint);
            //needlePoint.AddPoint(needleStartPoint.X, needleStartPoint.Y + (MAIN_RECT_HEIGHT / 2));
            //AddPrimitive(needlePoint);

            AddPort(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 12, rightMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            Sink = true;
            Source = false;
            // need to set port as sink for model checking.
            Ports[0].Sink = true;
            m_info_controls_box.X = leftMostStartPoint.X;
            m_info_controls_box.Y = m_primitives.Max(x => x.Loc.Y);
        }

        /// <summary>
        /// update the location of the control box.
        /// </summary>
        /// <returns></returns>
        protected override Rect UpdateControlBoxLocation()
        {
            var top = m_primitives.Max(x => x.Loc.Y + x.Size.Height);
            var xa = m_primitives[1].Loc.X;
            return new Rect(xa, top, m_info_controls_box.Width, m_info_controls_box.Height);
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
