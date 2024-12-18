﻿using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public class FluidicsRectangle : GraphicsPrimitive
    {
        Rect m_rect;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">a Point representing the rectangle's location on screen</param>
        /// <param name="size">a Size representing the size of the rectangle</param>
        /// <param name="color">a Color representing the color of the rectangle</param>
        /// <param name="fillBrush">a Brush representing the color to fill the rectangle with</param>
        /// <param name="fill">a bool determining if rectangle should be filled with fillColor or not</param>
        /// <param name="atScale">scale of the rectangle</param>
        public FluidicsRectangle(Point loc, Size size, Color color, Brush fillBrush, bool fill = true, float atScale = 1):
            base(fillBrush, myColor:color, fill:fill)
        {
            m_rect = new Rect(loc, size);
        }

        /// <summary>
        /// Render the rectangle to screen
        /// </summary>
        /// <param name="g">a DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the rectangle at</param>
        /// <param name="scale">a float representing the scale to draw the rectangle at</param>
        /// <param name="selected">a bool representing if the rectangle is highlighted or not</param>
        /// <param name="error"></param>
        public override void Render(DrawingContext g, byte alpha, float scale, bool selected, bool error)
        {
            // alter the color of the circle to be proper alpha, EXCEPT for error which should always be solid.
            Color = Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            Highlight = Color.FromArgb(alpha, Highlight.R, Highlight.G, Highlight.B);

            // Color fillColor = Color.FromArgb(alpha, base.FillColor.R, base.FillColor.G, base.FillColor.B);

            var scaledRect = new Rect(m_rect.X * scale, m_rect.Y * scale, m_rect.Size.Width * scale, m_rect.Size.Height * scale);
            Pen drawingPen;
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
            Brush brush = null;
            if (Fill)
            {
                brush = FillBrush;
            }

            g.DrawRectangle(brush, drawingPen, scaledRect);
        }

        public override bool Contains(Point point, int maxVariance)
        {
            // for the pump..which contains only a rectangle, if the point is inside or on the rectangle, return true.
            if (m_rect.X - maxVariance <= point.X && point.X <= (m_rect.X + m_rect.Size.Width + maxVariance) &&
                m_rect.Y - maxVariance <= point.Y && point.Y <= (m_rect.Y + m_rect.Size.Height + maxVariance))
            {
                return true;
            }
            return false;
        }

        public override void MoveBy(Point relativeValues)
        {
            m_rect.X += relativeValues.X;
            m_rect.Y += relativeValues.Y;
        }

        /// <summary>
        /// Property to determine the location of the rectangle
        /// </summary>
        public override Point Loc
        {
            get => m_rect.Location;
            set => m_rect.Location = value;
        }

        /// <summary>
        /// property to determine the size of the rectangle
        /// </summary>
        public override Size Size
        {
            get => m_rect.Size;
            set => m_rect.Size = value;
        }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        public override Rect Bounds => new Rect(m_rect.Location, m_rect.Size);
    }
}
