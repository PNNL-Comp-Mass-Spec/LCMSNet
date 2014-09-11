/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/19/2013
 * 
 * Last Modified 9/20/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FluidicsSDK.Graphic
{
    public class FluidicsCircle: GraphicsPrimitive
    {
        #region Members

            Rectangle m_rect;
            // Used for default constructor
            const int DEFAULT_RADIUS = 10;            
            int m_myRadius;    

        #endregion

        #region Constructors
       
            
        #endregion

        #region Methods

            /// <summary>
            /// default constructor
            /// </summary>
            public FluidicsCircle()
            {
                m_myRadius = DEFAULT_RADIUS;
                m_rect = new Rectangle(0, 0, m_myRadius * 2, m_myRadius * 2);
            }

            /// <summary>
            /// class constructor 
            /// </summary>
            /// <param name="loc">a System.Drawing.Point representing the cirlces location on screen</param>
            /// <param name="color">a System.Drawing.Color to represent the cirlce</param>
            /// <param name="fillColor">a System.Drawing.Color to represent the color to fill the circle with</param>
            /// <param name="radius">an integer representing the radius of the circle, defaults to 5</param>
            /// <param name="brushType">a brushtype to draw the circle with, defaults to a solid brush</param>
            /// <param name="fill">a bool determining if the circle should be filled with the fillColor</param>
            public FluidicsCircle(Point loc, Color color, Brush fillBrush, int radius = DEFAULT_RADIUS, bool fill = true)
            {
                m_myRadius = radius;
                m_rect = new Rectangle(loc.X, loc.Y, m_myRadius * 2, m_myRadius * 2);
                base.Color = color;
                base.FillBrush = fillBrush;
               // base.FillColor = fillColor;
                base.Fill = fill;
               // base.BrushType = brushType;
            }
            
            /// <summary>
            /// Render the circle
            /// </summary>
            public override void Render(Graphics g, int alpha, float scale, bool selected, bool error)
            {
                // alter the color of the circle to be proper alpha.
                base.Color = Color.FromArgb(alpha, base.Color.R, base.Color.G, base.Color.B);
                base.Highlight = Color.FromArgb(alpha, base.Highlight.R, base.Highlight.G, base.Highlight.B);                
                //base.FillColor = Color.FromArgb(alpha, base.FillColor.R, base.FillColor.G, base.FillColor.B);
                    Rectangle scaledRect = new Rectangle();
                    scaledRect.Location = new Point((int)(m_rect.Location.X * scale), (int)(m_rect.Location.Y * scale));
                    scaledRect.Size = new Size((int)(m_rect.Size.Width * scale), (int)(m_rect.Height * scale));
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
                        g.FillEllipse(base.FillBrush, scaledRect);
                        g.DrawEllipse(drawingPen, scaledRect);      
                    }
                    else
                    {
                        g.DrawEllipse(drawingPen, scaledRect);                                    
                    }          
            }

            public override bool Contains(Point point, int max_variance)
            {
                // standard pythagorean to determine if point is in/on the circle that defines the valve, e.g, if A^2 + B^2 <= C^2, the point is contained by the valve.
                if (((Math.Pow(point.X - Center.X, 2) + Math.Pow(point.Y - Center.Y, 2)) <= Math.Pow(m_myRadius + max_variance, 2)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void MoveBy(Point relativeValues)
            {
                m_rect.X += relativeValues.X;
                m_rect.Y += relativeValues.Y;
            }
        #endregion

        #region Properties

            /// <summary>
            /// Property determing the location of the circle on screen
            /// </summary>
            public override Point Loc
            {
                get
                {
                    return m_rect.Location;
                }
                // need to make a new rectangle as well as change the base Loc value to match the new Point location.
                set
                {                  
                    m_rect = new Rectangle(value.X, value.Y, m_myRadius * 2 , m_myRadius * 2);                    
                }    
            }

            /// <summary>
            ///  property for determining the size of the circle
            /// </summary>
            public override Size Size
            {
                get
                {
                    return m_rect.Size;
                }
                set { }
            }

            /// <summary>
            /// Property for finding the center point of the circle
            /// </summary>
            public Point Center
            {
                get
                {
                    return new Point(Loc.X + m_myRadius, Loc.Y + m_myRadius);
                }
            }

        #endregion
    }
}
