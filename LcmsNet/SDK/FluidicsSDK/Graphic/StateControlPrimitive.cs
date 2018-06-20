using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public class StateControlPrimitive : GraphicsPrimitive
    {
        Rect m_rect;
        Size m_size;

        public StateControlPrimitive(Point location, Size sz)
        {
            m_size = sz;
            m_rect = new Rect(location, m_size);
        }

        /// <summary>
        /// Render the rectangle to screen
        /// </summary>
        /// <param name="g">a DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the rectangle at</param>
        /// <param name="scale">a float representing the scale to draw the rectangle at</param>
        /// <param name="selected">a bool representing if the rectangle is hilighted or not</param>
        /// <param name="error"></param>
        public override void Render(DrawingContext g, byte alpha, float scale, bool selected, bool error)
        {
            Color = Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            Highlight = Color.FromArgb(alpha, Highlight.R, Highlight.G, Highlight.B);

            var scaledRect = new Rect(m_rect.X * scale, m_rect.Y * scale, m_rect.Size.Width * scale, m_rect.Size.Height * scale);
            Brush brush = null;
            Pen pen = null;
            if (Fill)
            {
                if (!selected)
                {
                    brush = FillBrush;
                }
                else
                {
                    brush = Highlighter.Brush;
                }
            }
            else
            {
                if (!selected)
                {
                    pen = Pen;
                }
                else
                {
                    pen = Highlighter;
                }
            }
            g.DrawRectangle(brush, pen, scaledRect);
        }

        public override bool Contains(Point point, int max_variance)
        {
            // for the pump..which contains only a rectangle, if the point is inside or on the rectangle, plus a little variance,
            // return true.
            if (m_rect.X - max_variance <= point.X && point.X <= (m_rect.X + m_rect.Size.Width + max_variance) &&
                m_rect.Y - max_variance <= point.Y && point.Y <= (m_rect.Y + m_rect.Size.Height + max_variance))
            {
                return true;
            }
            return false;
        }

        public override void MoveBy(Point relativeValues)
        {
            var oldX = m_rect.X;
            var oldY = m_rect.Y;
            m_rect.X += relativeValues.X;
            if (m_rect.X < 0)
            {
                m_rect.X = oldX;
            }
            m_rect.Y += relativeValues.Y;
            if (m_rect.Y < 0)
            {
                m_rect.Y = oldY;
            }
        }

        public override Size Size
        {
            get { return m_size; }
            set
            {
                m_size = value;
                m_rect.Size = m_size;
            }
        }

        public override Point Loc
        {
            get { return m_rect.Location; }
            set { m_rect = new Rect(value, m_size); }
        }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        public override Rect Bounds
        {
            get { return new Rect(m_rect.Location, m_rect.Size); }
        }
    }
}
