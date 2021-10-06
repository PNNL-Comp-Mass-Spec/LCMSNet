using System;
using System.Windows;
using System.Windows.Media;

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

        /// <summary>
        /// The pen to draw with when the primitive is highlighted
        /// </summary>
        private Pen m_highlightPen;

        private Color m_errorColor;

        private Pen m_errorPen;

        private const int PEN_WIDTH = 4;
        private static readonly Color DEFAULT_HIGHLIGHT = Colors.DarkGray;
        private static readonly Color DEFAULT_COLOR = Colors.Black;
        private static readonly Color DEFAULT_ERROR = Colors.Red;

        #endregion

        #region Methods

        /// <summary>
        ///  Base class constructor
        /// </summary>
        protected GraphicsPrimitive()
        {
            m_fill = true;
            m_color = DEFAULT_COLOR;
            m_drawingPen = new Pen(new SolidColorBrush(m_color), PEN_WIDTH);
            m_highlightColor = DEFAULT_HIGHLIGHT;
            m_highlightPen = new Pen(new SolidColorBrush(m_highlightColor), PEN_WIDTH);
            m_errorColor = DEFAULT_ERROR;
            m_errorPen = new Pen(new SolidColorBrush(m_errorColor), PEN_WIDTH);
            m_fillBrush = Brushes.White;
            m_drawingPen.TryFreeze();
            m_highlightPen.TryFreeze();
            m_errorPen.TryFreeze();
        }

        /// <summary>
        /// Constructor for a graphics object
        /// </summary>
        /// <param name="myColor">the primary color to draw the object</param>
        /// <param name="fill">boolean, determines if the graphic is filled </param>
        protected GraphicsPrimitive(Color? myColor = null, bool fill = true) :
            this(Brushes.White, myColor: myColor, fill: fill)
        {
        }

        /// <summary>
        /// Constructor for a graphics object
        /// </summary>
        /// <param name="fillbrush"></param>
        /// <param name="myColor"></param>
        /// <param name="fill"></param>
        protected GraphicsPrimitive(Brush fillbrush, Color? myColor = null, bool fill = true)
        {
            m_fill = fill;
            m_color = myColor ?? DEFAULT_COLOR;
            m_fillBrush = fillbrush;
            m_drawingPen = new Pen(new SolidColorBrush(m_color), PEN_WIDTH);
            m_highlightColor = DEFAULT_HIGHLIGHT;
            m_highlightPen = new Pen(new SolidColorBrush(m_highlightColor), PEN_WIDTH);
            m_errorColor = Colors.Red;
            m_errorPen = new Pen(new SolidColorBrush(m_errorColor), PEN_WIDTH);
            m_drawingPen.TryFreeze();
            m_highlightPen.TryFreeze();
            m_errorPen.TryFreeze();
            m_fillBrush.TryFreeze();
        }

        // ALL inheriting classes must define their own Render() method, since only they can determine how they are rendered
        [Obsolete("Use version with \"byte\" alpha")]
        public void Render(DrawingContext g, int alpha, float scale, bool highlight, bool error)
        {
            Render(g, (byte)alpha, scale, highlight, error);
        }

        [Obsolete("Use version with \"byte\" alpha")]
        public void Render(DrawingContext g, int alpha, float scale, Point moveby, bool highlight, bool error)
        {
            Render(g, (byte)alpha, scale, highlight, error);
        }

        // ALL inheriting classes must define their own Render() method, since only they can determine how they are rendered
        public abstract void Render(DrawingContext g, byte alpha, float scale, bool highlight, bool error);

        public virtual void Render(DrawingContext g, byte alpha, float scale, Point moveby, bool highlight, bool error)
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
            get => m_fill;
            set => m_fill = value;
        }

        /// <summary>
        /// Primary draw color of the graphics object
        /// </summary>
        public virtual Color Color
        {
            get => m_color;
            set
            {
                m_color = value;
                m_drawingPen = new Pen(new SolidColorBrush(m_color), PEN_WIDTH);
                m_drawingPen.TryFreeze();
            }
        }

        /// <summary>
        /// read-only pen property
        /// </summary>
        // to change the pens color, one must change the color of the primitive.
        public virtual Pen Pen => m_drawingPen;

        /// <summary>
        /// property to change the highlight color of the primitive.
        /// </summary>
        public virtual Color Highlight
        {
            get => m_highlightColor;
            set
            {
                m_highlightColor = value;
                m_highlightPen = new Pen(new SolidColorBrush(m_highlightColor), PEN_WIDTH);
                m_highlightPen.TryFreeze();
            }
        }

        public virtual Color ErrorColor
        {
            get => m_errorColor;
            set
            {
                m_errorColor = value;
                m_errorPen = new Pen(new SolidColorBrush(m_errorColor), PEN_WIDTH);
                m_errorPen.TryFreeze();
            }
        }

        public virtual Pen ErrorPen => m_errorPen;

        /// <summary>
        /// property to retrieve the highlighting pen
        /// </summary>
        public virtual Pen Highlighter => m_highlightPen;

        /// <summary>
        ///  get the fillbrush of the primitive
        /// </summary>
        public virtual Brush FillBrush
        {
            get => m_fillBrush;
            set
            {
                m_fillBrush = value;
                m_fillBrush.TryFreeze();
            }
        }

        /// <summary>
        /// property to represent the size of the graphics primitive
        /// </summary>
        public abstract Size Size { get; set; }

        /// <summary>
        /// Property representing the location of the primitive on the drawing
        /// </summary>
        /// <remarks>The location of the upper-left corner of the primitive, or start point of the line.</remarks>
        public abstract Point Loc { get; set; }

        /// <summary>
        /// The boundaries of the primitive
        /// </summary>
        public abstract Rect Bounds { get; }

        #endregion
    }
}
