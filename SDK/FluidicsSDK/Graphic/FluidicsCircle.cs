﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public class FluidicsCircle : GraphicsPrimitive
    {
        private Rect m_rect;
        // Used for default constructor
        private const int DEFAULT_RADIUS = 10;
        private readonly int m_myRadius;
        private readonly string m_fillText = string.Empty;

        /// <summary>
        /// default constructor
        /// </summary>
        public FluidicsCircle()
        {
            m_myRadius = DEFAULT_RADIUS;
            m_rect = new Rect(0, 0, m_myRadius * 2, m_myRadius * 2);
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">a Point representing the circles location on screen</param>
        /// <param name="color">a Color to represent the circle</param>
        /// <param name="fillBrush">a Color to represent the color to fill the circle with</param>
        /// <param name="radius">an integer representing the radius of the circle, defaults to 5</param>
        /// <param name="fill">a bool determining if the circle should be filled with the fillColor</param>
        /// <param name="fillText">Text (like a number) to output in the fill area</param>
        public FluidicsCircle(Point loc, Color color, Brush fillBrush, int radius = DEFAULT_RADIUS, bool fill = true, string fillText = "")
        {
            m_myRadius = radius;
            m_rect = new Rect(loc.X, loc.Y, m_myRadius * 2, m_myRadius * 2);
            Color = color;
            FillBrush = fillBrush;
            Fill = fill;
            m_fillText = fillText;
        }

        /// <summary>
        /// Render the circle
        /// </summary>
        public override void Render(DrawingContext g, byte alpha, float scale, bool selected, bool error)
        {
            // alter the color of the circle to be proper alpha.
            Color = Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            Highlight = Color.FromArgb(alpha, Highlight.R, Highlight.G, Highlight.B);
            var scaledRect = new Rect
            {
                Location = new Point((int)(m_rect.Location.X * scale), (int)(m_rect.Location.Y * scale)),
                Size = new Size((int)(m_rect.Size.Width * scale), (int)(m_rect.Height * scale))
            };
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

            var center = new Point(scaledRect.X + (scaledRect.Width / 2.0), scaledRect.Y + (scaledRect.Height / 2.0));
            g.DrawEllipse(brush, drawingPen, center, scaledRect.Width / 2.0, scaledRect.Height / 2.0);

            if (!string.IsNullOrWhiteSpace(m_fillText))
            {
                // Draw the text inside the ellipse...
                var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);
                var font = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var text = new FormattedText(m_fillText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    font, 14.667 * stringScale, Brushes.Black, FluidicsModerator.Moderator.DrawingScaleFactor);
                g.DrawText(text, new Point(scaledRect.X + (scaledRect.Width / 2) - (text.Width / 2), scaledRect.Y + (scaledRect.Height / 2) - (text.Height / 2)));
            }
        }

        public override bool Contains(Point point, int max_variance)
        {
            // standard pythagorean to determine if point is in/on the circle that defines the valve, e.g, if A^2 + B^2 <= C^2, the point is contained by the valve.
            if (Math.Pow(point.X - Center.X, 2) + Math.Pow(point.Y - Center.Y, 2) <= Math.Pow(m_myRadius + max_variance, 2))
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
        /// Property determining the location of the circle on screen
        /// </summary>
        public override Point Loc
        {
            get => m_rect.Location;
            // need to make a new rectangle as well as change the base Loc value to match the new Point location.
            set => m_rect = new Rect(value.X, value.Y, m_myRadius * 2, m_myRadius * 2);
        }

        /// <summary>
        ///  property for determining the size of the circle
        /// </summary>
        public override Size Size
        {
            get => m_rect.Size;
            set { }
        }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        public override Rect Bounds => new Rect(m_rect.Location, m_rect.Size);

        /// <summary>
        /// Property for finding the center point of the circle
        /// </summary>
        public Point Center => new Point(Loc.X + m_myRadius, Loc.Y + m_myRadius);
    }
}
