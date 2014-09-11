/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 1/3/2013
 * 
 * Last Modified 1/3/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using System.Drawing;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public class FluidicsUnion : FluidicsDevice
    {
        #region Members
        private const int MAIN_RECT_WIDTH = 50;
        private const int MAIN_RECT_HEIGHT = 20;
        #endregion

        #region Methods
        public FluidicsUnion()
        {
            Point mainStartPoint = new Point(0, 0);
            Point leftMostStartPoint = new Point(-(MAIN_RECT_WIDTH / 3) - 5, 0);
            Point rightMostStartPoint = new Point(MAIN_RECT_WIDTH + 5, 0);

            //main rectangle            
            base.AddRectangle(mainStartPoint, new Size(MAIN_RECT_WIDTH, MAIN_RECT_HEIGHT), Color.Black, Brushes.White);

            //left most rectangle            
            base.AddRectangle(leftMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Color.Black, Brushes.White);

            // bottom left parallelogram + connecting line
            FluidicsPolygon leftPara = new FluidicsPolygon();
            leftPara.AddPoint(new Point(leftMostStartPoint.X, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            leftPara.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            leftPara.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT));

            base.AddPrimitive(new FluidicsLine(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));

            base.AddPrimitive(leftPara);


            //bottom middle trapezoid
            FluidicsPolygon botmid = new FluidicsPolygon();
            botmid.AddPoint(new Point(mainStartPoint.X, mainStartPoint.Y + MAIN_RECT_HEIGHT));
            botmid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botmid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botmid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y + MAIN_RECT_HEIGHT));
            base.AddPrimitive(botmid);


            // bottom right parallelogram + connecting line
            FluidicsPolygon botRt = new FluidicsPolygon();
            botRt.AddPoint(new Point(rightMostStartPoint.X, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));
            botRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));

            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10));
            botRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT));

            base.AddPrimitive(new FluidicsLine(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y + MAIN_RECT_HEIGHT + 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y + MAIN_RECT_HEIGHT + 5)));
            base.AddPrimitive(botRt);



            // rightmost rectangle            
            base.AddRectangle(rightMostStartPoint, new Size(MAIN_RECT_WIDTH / 3, MAIN_RECT_HEIGHT), Color.Black, Brushes.White);

            // upper left parallelogram + connecting line
            FluidicsPolygon upLft = new FluidicsPolygon();
            upLft.AddPoint(leftMostStartPoint);
            upLft.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10));

            upLft.AddPoint(new Point(leftMostStartPoint.X + 3, leftMostStartPoint.Y - 10));
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10));

            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 3, leftMostStartPoint.Y - 10));
            upLft.AddPoint(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3, leftMostStartPoint.Y));
            base.AddPrimitive(upLft);
            base.AddPrimitive(new FluidicsLine(new Point(leftMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 2, leftMostStartPoint.Y - 10), new Point(mainStartPoint.X + 3, mainStartPoint.Y - 5)));

            // upper middle trapezoid
            FluidicsPolygon upMid = new FluidicsPolygon();
            upMid.AddPoint(mainStartPoint);
            upMid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10));

            upMid.AddPoint(new Point(mainStartPoint.X + 5, mainStartPoint.Y - 10));
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 4, mainStartPoint.Y - 10));

            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 5, mainStartPoint.Y - 10));
            upMid.AddPoint(new Point(mainStartPoint.X + MAIN_RECT_WIDTH, mainStartPoint.Y));
            base.AddPrimitive(upMid);


            // upper right parallelogram + connecting line
            FluidicsPolygon upRt = new FluidicsPolygon();
            upRt.AddPoint(rightMostStartPoint);
            upRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10));

            upRt.AddPoint(new Point(rightMostStartPoint.X - 3, rightMostStartPoint.Y - 10));
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10));

            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 - 3, rightMostStartPoint.Y - 10));
            upRt.AddPoint(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3, rightMostStartPoint.Y));
            base.AddPrimitive(upRt);

            base.AddPrimitive(new FluidicsLine(new Point(rightMostStartPoint.X - 2, rightMostStartPoint.Y - 10), new Point(mainStartPoint.X + MAIN_RECT_WIDTH - 3, mainStartPoint.Y - 5)));

            base.AddPort(new Point(leftMostStartPoint.X - 12, leftMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            base.AddPort(new Point(rightMostStartPoint.X + MAIN_RECT_WIDTH / 3 + 12, rightMostStartPoint.Y + MAIN_RECT_HEIGHT / 2));
            ConnectionManager.GetConnectionManager.Connect(Ports[0], Ports[1], this);
            Connection c = ConnectionManager.GetConnectionManager.FindConnection(Ports[0], Ports[1]);
            c.Transparent = true;
        }

        public override bool Contains(Point location)
        {
            bool contains = false;
            int minX = m_primitives.Min(z => z.Loc.X);
            int maxX = m_primitives.Max(z => z.Loc.X);
            int minY = m_primitives.Min(z => z.Loc.Y);
            int maxY = m_primitives.Max(z => z.Loc.Y);
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
