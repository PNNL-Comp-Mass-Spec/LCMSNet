﻿using System;
using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public class FluidicsLine : GraphicsPrimitive
    {
        // origination of line
        private Point m_orig;
        // termination of line
        private Point m_term;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="from">a Point representing where the line starts</param>
        /// <param name="to">a point representing where the line ends</param>
        public FluidicsLine(Point from, Point to)
        {
            m_orig = from;
            m_term = to;
        }

        /// <summary>
        /// Render the line to the screen
        /// </summary>
        /// <param name="g">a DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the line with</param>
        /// <param name="scale">a float determining how much to scale the line by</param>
        /// <param name="selected">bool determining if the line is drawn highlighted or not</param>
        /// <param name="error"></param>
        public override void Render(DrawingContext g, byte alpha, float scale, bool selected, bool error)
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
            else if (error)
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
            //basic linear algebra, check that the cross-product is between -MAX_PIXEL_VARIANCE and MAX_PIXEL_VARIANCE, this way users don't
            //have to click 100% precisely on pixels, that the dot product is > 0, and that the dot product is less than the squared distance
            //between point1 and point2. if the user click meets these criteria, they've clicked on the connection. Also, modify the values by the scale
            //to ensure they can select while scaled.
            double crossProduct = ((location.Y - Origin.Y) * (Term.X - Origin.X)) - ((location.X - Origin.X) * (Term.Y - Origin.Y));
            double dotProduct = ((location.X - Origin.X) * (Term.X - Origin.X)) + ((location.Y - Origin.Y) * (Term.Y - Origin.Y));
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

        /// <summary>
        /// property for the size of the line..only throws an error since lines don't have a size
        /// </summary>
        public override Size Size
        {
            get => new Size(0, 0);
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
            get => m_orig;
            set => m_orig = value;
        }

        /// <summary>
        ///  Property for the Termination point of the line
        /// </summary>
        public Point Term
        {
            get => m_term;
            set => m_term = value;
        }

        public override Point Loc
        {
            get => Origin;
            set
            {
                // do nothing
            }
        }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        //var minX = Math.Min(Origin.X, Term.X);
        //var minY = Math.Min(Origin.Y, Term.Y);
        //var maxX = Math.Max(Origin.X, Term.X);
        //var maxY = Math.Max(Origin.Y, Term.Y);
        //return new Rect(minX, minY, maxX - minX, maxY - minY);
        public override Rect Bounds => new Rect(Origin, Term);
    }
}
