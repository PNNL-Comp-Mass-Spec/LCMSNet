/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/16/2013
 *
 * Last Modified 10/21/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Drawing;

namespace FluidicsSDK.Graphic
{
    /// <summary>
    /// abstract Graphics Primitive for Fluidics Designer. base class for holding graphics data for fluidics
    /// devices/ports/connections.
    /// </summary>
    public abstract class GraphicsPrimitive
    {

        #region Members
        /// <summary>
        ///   Should the object drawn be filled with the FillColor or not.
        /// </summary>
        private bool m_fill;
        /// <summary>
        ///  Color of the object to be drawn
        /// </summary>
        private Color m_color;
        /// <summary>
        ///  Brush to use to draw object, solid, hatched, crosshatched are the options.
        /// </summary>        
        private Brush m_fillBrush;
        /// <summary>
        ///  the current pen to draw with.
        /// </summary>
        private Pen m_drawingPen;
        /// <summary>
        /// color to use when object is highlighted
        /// </summary>
        private Color m_highlightColor;
        //the pen to draw with when the primitive is highlighted
        private Pen m_highlightPen;

        private Color m_errorColor;

        private Pen m_errorPen;
        /// <summary>
        /// the location of the upper-left corner of the primitive, or start point of the line.
        /// </summary>
        private Point m_loc;
        private const int PEN_WIDTH = 4;
        private static readonly Color DEFAULT_HILIGHT = Color.DarkGray;
        private static readonly Color DEFAULT_COLOR = Color.Black;
        private static readonly Color DEFAULT_ERROR = Color.Red;

        #endregion

        #region Methods

        /// <summary>
        ///  Base class constructor
        /// </summary>
        public GraphicsPrimitive()
        {
            m_fill = true;
            m_color = DEFAULT_COLOR;
            m_drawingPen = new Pen(new SolidBrush(m_color));
            m_drawingPen.Width = PEN_WIDTH;
            m_highlightColor = DEFAULT_HILIGHT;
            m_highlightPen = new Pen(new SolidBrush(m_highlightColor));
            m_highlightPen.Width = PEN_WIDTH;
            m_errorColor = DEFAULT_ERROR;
            m_errorPen = new Pen(new SolidBrush(m_errorColor));
            m_errorPen.Width = PEN_WIDTH;
            m_fillBrush = new SolidBrush(Color.White);
        }
            
        /// <summary>
        /// Constructor for a graphics object
        /// </summary>
        /// <param name="myColor">the primary color to draw the object</param>
        /// <param name="loc">a System.Drawing.Point defining where on screen to draw the object</param>
        /// <param name="brushType">A selection from the BrushType enum determining the type of brush to use when filling the object</param>
        /// <param name="fill">boolean, determines if the graphic is filled </param>
        public GraphicsPrimitive(Nullable<Color> myColor = null, bool fill = true, float atScale = 1):
            this(new SolidBrush(Color.White), myColor:myColor, fill:fill, atScale:atScale)
        {            
        }

        public GraphicsPrimitive(Brush fillbrush , Nullable<Color> myColor = null, bool fill = true, float atScale = 1)
        {                 
            m_fill  = fill;
            m_color = myColor != null ? (Color)myColor : DEFAULT_COLOR;
            m_fillBrush = fillbrush;
            m_drawingPen = new Pen(new SolidBrush(m_color));
            m_drawingPen.Width = PEN_WIDTH;
            m_highlightColor = DEFAULT_HILIGHT;
            m_highlightPen = new Pen(new SolidBrush(m_highlightColor));
            m_highlightPen.Width = PEN_WIDTH;
            m_errorColor = Color.Red;
            m_errorPen = new Pen(new SolidBrush(ErrorColor));
            m_errorPen.Width = PEN_WIDTH;
        }

        //cleanup to prevent leaks. both the Pen and Brush objects need to be disposed as they wrap unmanaged resources
        ~GraphicsPrimitive()
        {          
            m_drawingPen.Dispose();
            m_highlightPen.Dispose();
            m_errorPen.Dispose();
        }

            
        // ALL inheriting classes must define their own Render() method, since only they can determine how they are rendered
        public abstract void Render(Graphics g, int alpha, float scale, bool highlight, bool error);
                
        public virtual void Render(Graphics g, int alpha, float scale, Point moveby, bool highlight, bool error)
        {
            Render(g, alpha, scale, highlight, error);
        }
        
            
        // a primitive should be able to tell if a point is within itself.
        public abstract bool Contains(Point point, int max_variance);

        public abstract void MoveBy(Point relativeValues);

        #endregion

        #region Properties

            /// <summary>
            /// toggle fill status of graphics object. True to fill object, False not to.
            /// </summary>
            public virtual bool Fill
            {
                get
                {
                        return this.m_fill;
                }
                set
                {                 
                    this.m_fill = value;
                }
            }


            /// <summary>
            /// Primary draw color of the graphics object
            /// </summary>
            public virtual Color Color
            {
                get
                {
                    return this.m_color;
                }
                set
                {
                    this.m_color = value;
                    m_drawingPen.Dispose();
                    m_drawingPen = new Pen(new SolidBrush(m_color));
                    m_drawingPen.Width = PEN_WIDTH;
                }
            }    


            /// <summary>
            /// read-only pen property
            /// </summary>
            // to change the pens color, one must change the color of the primitive.
            public virtual Pen Pen
            {
                get
                {
                    return m_drawingPen;
                }
                private set { }
            }

            /// <summary>
            /// property to change the highlight color of the primitive.
            /// </summary>
            public virtual Color Highlight
            {
                get
                {
                    return m_highlightColor;
                }
                set
                {
                    m_highlightColor = value;
                    m_highlightPen.Dispose();
                    m_highlightPen = new Pen(new SolidBrush(m_highlightColor));
                    m_highlightPen.Width = PEN_WIDTH;
                }
            }


            public virtual Color ErrorColor
            {
                get
                {
                    return m_errorColor;
                }
                set
                {
                    m_errorColor = value;
                    m_errorPen.Dispose();
                    m_errorPen = new Pen(new SolidBrush(m_errorColor));
                    m_errorPen.Width = PEN_WIDTH;
                }

            }

            public virtual Pen ErrorPen
            {
                get
                {
                    return m_errorPen;
                }
                private set { }
            }

            /// <summary>
            /// property to retrieve the highlighting pen
            /// </summary>
            public virtual Pen Highlighter
            {
                get
                {
                    return m_highlightPen;
                }
                private set { }
            }

            /// <summary>
            ///  get the fillbrush of the primitive
            /// </summary>
            public virtual Brush FillBrush
            {
                get
                {
                    return m_fillBrush;
                }
                set
                {
                    m_fillBrush = value;
                }
            }

            /// <summary>
            /// property to represent the size of the graphics primitive
            /// </summary>
            public abstract Size Size { get; set; }

            /// <summary>
            /// property to repsenting the location of the primitive on screen
            /// </summary>
            public virtual Point Loc
            {
                get
                {
                    return m_loc;
                }
                set
                {                    
                    m_loc = value;
                }
            }

        #endregion
    }
}