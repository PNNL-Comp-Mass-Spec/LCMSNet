/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 9/5/2013
 *
 * Last Modified 12/18/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Drawing;

namespace FluidicsSDK.Graphic
{
    public class FluidicsLine:GraphicsPrimitive
    {
        #region Members
        // origination of line
        private Point m_orig;
        // termination of line
        private Point m_term;

        #endregion
        
        #region Methods

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="from">a System.Drawing.Point representing where the line starts</param>
        /// <param name="to">a System.Drawing.point representing where the line ends</param>
        public FluidicsLine(Point from, Point to)
        {
            m_orig = from;
            m_term = to;
        }

        /// <summary>
        /// Render the line to the screen
        /// </summary>
        /// <param name="g">a System.Drawing Graphics object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the line with</param>
        /// <param name="scale">a float determining how much to scale the line by</param>
        /// <param name="selected">bool determining if the line is drawn hilighted or not</param>
        /// <param name="error"></param>
        public override void Render(Graphics g, int alpha, float scale, bool selected, bool error)
        {
            Color = Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            Highlight = Color.FromArgb(alpha, Highlight.R, Highlight.G, Highlight.B);
            var scaled_orig = new Point((int)(m_orig.X * scale), (int)(m_orig.Y * scale));
            var scaled_term = new Point((int)(m_term.X * scale), (int)(m_term.Y * scale));
            Pen drawingPen;
            if (selected)
            {
                drawingPen = Highlighter;
            }
            else if(error)
            {
                drawingPen = ErrorPen;
            }
            else
            {
                drawingPen = Pen;
            }
            g.DrawLine(drawingPen, scaled_orig, scaled_term);
        }

        public override bool Contains(Point location, int max_variance)
        {
             //basic linear algebra, check that the crossproduct is between -MAX_PIXEL_VARIANCE and MAX_PIXEL_VARIANCE, this way users don't
            //have to click 100% precisely on pixels, that the dot product is > 0, and that the dot product is less than the squared distance
            //between point1 and point2. if the user click meets these critera, they've clicked on the connection. Also, modify the values by the scale
            //to ensure they can select while scaled.
            float crossProduct = ((location.Y - Origin.Y) * (Term.X - Origin.X)) - ((location.X - Origin.X) * (Term.Y - Origin.Y));
            float dotProduct = ((location.X - Origin.X) * (Term.X - Origin.X)) + ((location.Y - Origin.Y) * (Term.Y - Origin.Y));
            var squaredLength = Math.Pow((Term.X - Origin.X), 2) + Math.Pow((Term.Y - Origin.Y), 2);
            if ((-max_variance) <= crossProduct && crossProduct <= (max_variance) && dotProduct > 0 && dotProduct < squaredLength)
            {
                return true;
            }          
            return false;
        }

        public override void MoveBy(Point relativeValues)
        {
            m_orig.X += relativeValues.X;
            m_term.X += relativeValues.X;
            m_orig.Y += relativeValues.Y;
            m_term.Y += relativeValues.Y;
        }
        #endregion

        #region Properties

        /// <summary>
        /// property for the size of the line..only throws an error since lines don't have a size
        /// </summary>
        public override Size Size
        {
            get
            {
                return new Size(0, 0);
            }
            set
            {
                //ignore
            }
        }

        /// <summary>
        /// Property for the Origin point of the line
        /// </summary>
        public Point Origin
        {
            get
            {
                return m_orig;
            }
            set
            {                
                m_orig = value;
            }

        }

        /// <summary>
        ///  Property for the Terminiation point of the line
        /// </summary>
        public Point Term
        {
            get
            {
                return m_term;
            }
            set
            {
                m_term = value;
            }
        }

        public override Point Loc
        {
            get
            {
                return Origin;
            }
            set
            {
                // do nothing
            }
        }
        #endregion

    }
}
