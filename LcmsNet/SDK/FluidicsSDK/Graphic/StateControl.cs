using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FluidicsSDK.Graphic
{
    public class StateControlPrimitive:GraphicsPrimitive
    {
        Rectangle m_rect;
        Size m_size;

        public StateControlPrimitive(Point location, Size sz)
        {
            m_size = sz;
            m_rect = new Rectangle(location, m_size);
        }

        /// <summary>
        /// Render the rectangle to screen
        /// </summary>
        /// <param name="g">a System.Drawing.Graphics object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the rectangle at</param>
        /// <param name="scale">a float representing the scale to draw the rectangle at</param>
        /// <param name="selected">a bool representing if the rectangle is hilighted or not</param>
        public override void Render(Graphics g, int alpha, float scale, bool selected, bool error)
        {            
            base.Color = Color.FromArgb(alpha, base.Color.R, base.Color.G, base.Color.B);
            base.Highlight = Color.FromArgb(alpha, base.Highlight.R, base.Highlight.G, base.Highlight.B);
            
            RectangleF scaledRect = new RectangleF(m_rect.X * scale, m_rect.Y * scale, m_rect.Size.Width * scale, m_rect.Size.Height * scale);
            if (base.Fill)
            {
                if (!selected)
                {
                    g.FillRectangle(base.FillBrush, scaledRect);
                }
                else
                {
                    g.FillRectangle(base.Highlighter.Brush, scaledRect);
                }
            }
            else
            {
                //for some reason there is no DrawRectangle overload that takes a RectangleF, so we have to draw it this way.
                if (!selected)
                {
                    g.DrawRectangle(base.Pen, scaledRect.X, scaledRect.Y, scaledRect.Width, scaledRect.Height);
                }
                else
                {
                    g.DrawRectangle(base.Highlighter, scaledRect.X, scaledRect.Y, scaledRect.Width, scaledRect.Height);
                }
            }
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

            int oldX = m_rect.X;
            int oldY = m_rect.Y;
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
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
                m_rect.Size = m_size;
            }
                
        }

        public override Point Loc
        {
            get
            {
                return base.Loc;
            }
            set
            {
                base.Loc = value;
                m_rect = new Rectangle(value, m_size);
            }
        }
    }
}
