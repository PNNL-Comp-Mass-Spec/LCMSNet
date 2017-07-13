using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public enum Orient
    {
        Right,
        Down,
        Left,
        Up
    }

    public class FluidicsTriangle : GraphicsPrimitive
    {
        #region Members
        Rect m_area;
        Rect m_scaledArea;
        readonly Point[] m_points;
        #endregion

        #region Methods
        public FluidicsTriangle(Rect a, Orient or)
        {
            m_points = new Point[3];
            m_area = a;
            m_scaledArea = a;
            Orientation = or;
            DeterminePoints(Orientation);
            FillBrush = Brushes.Black;
            Fill = true;
        }

        public override void Render(DrawingContext g, byte alpha, float scale, bool selected, bool error)
        {
            Render(g, alpha, scale, new Point(0, 0), selected, error);
        }

        public override void Render(DrawingContext g, byte alpha, float scale, Point moveby, bool selected, bool error)
        {
            /*scale points to match device scaling
             * figure out x and y shift values
             * figure out which point to move in x-plane and which to move in y-plane
             * move points
             */
            var newPoints = new Point[3];
            for (var i = 0; i < m_points.Length; i++)
            {
                var point = m_points[i];
                point.X = Convert.ToInt32(Convert.ToSingle(point.X) * scale) + moveby.X;
                point.Y = Convert.ToInt32(Convert.ToSingle(point.Y) * scale) + moveby.Y;
                newPoints[i] = point;
            }
            Brush fillBrush = null;
            Pen drawingPen = null;
            if (Fill)
            {
                if (selected)
                {
                    fillBrush = Highlighter.Brush;
                }
                else if (error)
                {
                    fillBrush = ErrorPen.Brush;
                }

                else
                {
                    fillBrush = Pen.Brush;
                }
            }
            else
            {
                if (selected)
                {
                    drawingPen = Highlighter;
                }
                else if (error)
                {
                    drawingPen = ErrorPen;
                }
                else
                {
                    drawingPen = Pen;
                }
            }

            var figure = new PathFigure(newPoints[0], new[]
            {
                new LineSegment(newPoints[1], true),
                new LineSegment(newPoints[2], true),
            }, true);
            var geometry = new PathGeometry(new[] { figure });

            g.DrawGeometry(fillBrush, drawingPen, geometry);
        }

        /// <summary>
        /// Determine location of the three point where lines meet in the triangle
        /// </summary>
        /// <param name="currentOrientation">enum representing the orientation of the triangle</param>
        private void DeterminePoints(Orient currentOrientation)
        {
            var half = m_area.Width / 2;
            switch (currentOrientation)
            {
                case Orient.Down:
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[2] = new Point(m_area.X + half, m_area.Y + m_area.Height / 2);
                    break;
                case Orient.Right:
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X, m_area.Y + m_area.Height);
                    m_points[2] = new Point(m_area.X + m_area.Width, m_area.Y + m_area.Height / 2);
                    break;
                case Orient.Left:
                    m_points[0] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y + m_area.Height);
                    m_points[2] = new Point(m_area.X, m_area.Y + m_area.Height / 2);
                    break;
                case Orient.Up:
                    m_points[0] = new Point(m_area.X, m_area.Y);
                    m_points[1] = new Point(m_area.X + m_area.Width, m_area.Y);
                    m_points[2] = new Point(m_area.X + half, m_area.Y - m_area.Height / 2);
                    break;
                default:
                    throw new Exception("invalid orientation");
            }
        }


        public override void MoveBy(Point relativeValues)
        {
            double maxx = 0;
            double maxy = 0;
            double minx = int.MaxValue;
            double miny = int.MaxValue;

            for (var i = 0; i < m_points.Length; i++)
            {
                m_points[i].X += relativeValues.X;
                m_points[i].Y += relativeValues.Y;

                maxx = Math.Max(m_points[i].X, maxx);
                maxy = Math.Max(m_points[i].Y, maxy);
                minx = Math.Min(m_points[i].X, minx);
                miny = Math.Min(m_points[i].Y, miny);

            }

            if (m_points.Length < 0)
                return;

            m_scaledArea = new Rect(minx, miny, Math.Abs(maxx - minx), Math.Abs(maxy - miny));
        }

        public override bool Contains(Point point, int maxVariance)
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
                var minx = m_points.Min(x => x.X);
                var miny = m_points.Min(x => x.Y);
                var maxx = m_points.Max(x => x.X);
                var maxy = m_points.Max(x => x.Y);
                m_scaledArea = new Rect(minx, miny, maxx - minx, maxy - miny);
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
