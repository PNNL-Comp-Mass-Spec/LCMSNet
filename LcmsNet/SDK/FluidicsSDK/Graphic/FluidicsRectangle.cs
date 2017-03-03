/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/22/2013
 *
 * Last Modified 10/17/2013 By Christopher Walters
 ********************************************************************************************************/

using System.Drawing;

namespace FluidicsSDK.Graphic
{
    public class FluidicsRectangle:GraphicsPrimitive
    {
        #region Members
        Rectangle m_rect;
        #endregion

        #region Methods

        /// <summary>
        /// default constructor
        /// </summary>
        private FluidicsRectangle()
        {
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">a System.Drawing.Point representing the rectangle's location on screen</param>
        /// <param name="size">a System.Drawing.Size representing the size of the rectangle</param>
        /// <param name="color">a System.Drawing.Color representing the color of the rectangle</param>
        /// <param name="fillColor">a System.Drawing.Color representing the color to fill the rectangle with</param>
        /// <param name="type">a BrushType object representing which brush to draw with</param>
        /// <param name="fill">a bool determining if rectangle should be filled with fillColor or not</param>
        /// <param name="atScale">scale of the rectangle</param>
        public FluidicsRectangle(Point loc, Size size, Color color, Brush fillBrush, bool fill = true, float atScale = 1):
            base(fillBrush, myColor:color, fill:fill)
        {
            m_rect = new Rectangle(loc, size);
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
            // alter the color of the circle to be proper alpha, EXCEPT for error which should always be solid.
            base.Color = Color.FromArgb(alpha, base.Color.R, base.Color.G, base.Color.B);
            base.Highlight  = Color.FromArgb(alpha, base.Highlight.R, base.Highlight.G, base.Highlight.B);
            
            // Color fillColor = Color.FromArgb(alpha, base.FillColor.R, base.FillColor.G, base.FillColor.B);
             
            var scaledRect = new RectangleF(m_rect.X * scale, m_rect.Y * scale, m_rect.Size.Width * scale, m_rect.Size.Height * scale);
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
            if (base.Fill)
            {
                g.FillRectangle(base.FillBrush, scaledRect);
            }
            //for some reason there is no DrawRectangle overload that takes a RectangleF, so we have to draw it this way.
            g.DrawRectangle(drawingPen, scaledRect.X, scaledRect.Y, scaledRect.Width, scaledRect.Height);
        }

        public override bool Contains(Point point, int max_variance)
        {
            // for the pump..which contains only a rectangle, if the point is inside or on the rectangle, return true.
            if (m_rect.X - max_variance <= point.X && point.X <= (m_rect.X + m_rect.Size.Width + max_variance) &&
                m_rect.Y - max_variance <= point.Y && point.Y <= (m_rect.Y + m_rect.Size.Height + max_variance))
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

        #endregion

        #region Properties
             
        /// <summary>
        /// Property to determine the location of the rectangle
        /// </summary>
        public override Point Loc
        {
            get
            {
                return m_rect.Location;
            }
            set
            {                    
                base.Loc = value;
                m_rect.Location = base.Loc;
            }
        }

        /// <summary>
        /// property to determine the size of the rectangle
        /// </summary>
        public override Size Size
        {
            get
            {
                return m_rect.Size;
            }
            set
            {
                m_rect.Size = value;
            }
        }
        #endregion

    }
}
