﻿/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 *
 * Last Modified 12/18/2013 By Christopher Walters
 ********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FluidicsSDK.Graphic
{

    public enum Orient { Right, Down, Left, Up };

    public class FluidicsTriangle : GraphicsPrimitive
    {
        #region Members
        Rectangle m_area;
        Rectangle m_scaledArea;
        Point[] m_points;
        #endregion

        #region Methods
        public FluidicsTriangle(Rectangle a, Orient or)
        {
            m_points = new Point[3];
            m_area = a;
            m_scaledArea = a;
            Orientation = or;
            DeterminePoints(Orientation);
            FillBrush = Brushes.Black;
            Fill = true;
        }

        public override void Render(Graphics g, int alpha, float scale, bool selected, bool error)
        {
            Render(g, alpha, scale, new Point(0, 0), selected, error);
        }

        public override void Render(Graphics g, int alpha, float scale, Point moveby, bool selected, bool error)
        {           
           /*scale points to match device scaling
            * figure out x and y shift values
            * figure out which point to move in x-plane and which to move in y-plane
            * move points
            */
            Point[] newPoints = new Point[3];
            for(int i = 0; i < m_points.Length; i++)
            {
                Point point  = m_points[i];
                point.X = Convert.ToInt32(Convert.ToSingle(point.X) * scale) + moveby.X;
                point.Y = Convert.ToInt32(Convert.ToSingle(point.Y) * scale) + moveby.Y;
                newPoints[i] = point;
            }          
            if (base.Fill)
            {
                Brush fillBrush;
                if (selected)
                {
                    fillBrush = base.Highlighter.Brush; ;
                }
                else if (error)
                {
                    fillBrush = base.ErrorPen.Brush;
                }
                
                else
                {
                    fillBrush = base.Pen.Brush;
                }
                g.FillPolygon(fillBrush, newPoints);
            }
            else
            {
                Pen drawingPen;
                if (selected)
                {
                    drawingPen = base.Highlighter;
                }
                else if (error)
                {
                    drawingPen = base.ErrorPen;
                }              
                else
                {
                    drawingPen = base.Pen;
                }
                g.DrawPolygon(drawingPen, newPoints);
            }
        }

        /// <summary>
        /// determine location of the three point where lines meet in the triangle
        /// </summary>
        /// <param name="or">enum representing the orientation of the triangle</param>
        private void DeterminePoints(Orient or)
        {
            int half = m_area.Width / 2;
            switch (Orientation)
            {                
                case Orient.Down:                       
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[2] = new Point(m_area.X + half, m_area.Y + m_area.Height /2);
                    break;
                case Orient.Right:
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X, m_area.Y + m_area.Height);
                    m_points[2] = new Point(m_area.X + m_area.Width, m_area.Y + m_area.Height/2);
                    break;
                case Orient.Left:
                    m_points[0] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y + m_area.Height);
                    m_points[2] = new Point(m_area.X, m_area.Y + m_area.Height/2);
                    break;
                case Orient.Up:
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[2] = new Point(m_area.X + half, m_area.Y - m_area.Height/2);
                    break;
                default:
                    throw new Exception("invalid orientation");
            }
        }

      
        public override void MoveBy(Point relativeValues)
        {      
            int maxx = 0;
            int maxy = 0;
            int minx = int.MaxValue;
            int miny = int.MaxValue;

            for (int i = 0; i < m_points.Length; i++)
            {
                m_points[i].X += relativeValues.X;
                m_points[i].Y += relativeValues.Y;
            
                maxx = Math.Max(m_points[i].X, maxx);
                maxy = Math.Max(m_points[i].Y, maxy);
                minx = Math.Min(m_points[i].X, minx);
                miny = Math.Min(m_points[i].Y, miny);

            }

            if (m_points.Length < 0)
                return ;

            m_scaledArea = new Rectangle(minx, miny, Math.Abs(maxx - minx), Math.Abs(maxy - miny));
        }

        public override bool Contains(Point point, int max_variance)
        {            
            return m_scaledArea.Contains(point);
        }
        #endregion

        #region Properties

        public Orient Orientation
        {
            get;
            set;
        }

        public override Point Loc
        {
            get
            {
                return m_area.Location;
            }
            set
            {
                m_area.Location = value;
                DeterminePoints(Orientation);
                int minx = m_points.Min(x => x.X);
                int miny = m_points.Min(x => x.Y);
                int maxx = m_points.Max(x => x.X);
                int maxy = m_points.Max(x => x.Y);
                m_scaledArea = new Rectangle(minx, miny, maxx - minx, maxy - miny);
            }
        }   

        public override Size Size
        {
            get
            {
                return m_area.Size;
            }
            set
            {
                m_area.Size = value;
            }
        }
        #endregion
    }
}
