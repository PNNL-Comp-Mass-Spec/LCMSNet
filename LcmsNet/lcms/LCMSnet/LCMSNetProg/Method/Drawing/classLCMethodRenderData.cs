using System;
using System.Drawing;

using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Rendering information for a given method
    /// </summary>
    public class classLCMethodRenderData
    {
        /// <summary>
        /// Constructor that takes a background color and a method.
        /// </summary>
        /// <param name="background">Background color of the event.</param>
        /// <param name="lineColor">Color of the line to display.</param>
        /// <param name="method">Method to render</param>
        /// <param name="useGradientBrush">Flag indicating to use a solid or gradient brush for the background.</param>
        public classLCMethodRenderData( Color background, 
                                        Color lineColor,
                                        float lineWidth,
                                        classLCMethod method,
                                        bool useGradientBrush)
        {
            LineColor           = lineColor;
            LineWidth           = lineWidth;
            Background          = background;
            UseGradientBrush    = useGradientBrush;
            Method              = method;
        }
        /// <summary>
        /// Constructor that takes a method to render and leaves the background white.
        /// </summary>
        /// <param name="method">Method to render.</param>
        public classLCMethodRenderData(classLCMethod method)
        {
            LineColor           = Color.Black;
            LineWidth           = 2.0F;
            Background          = Color.White;
            UseGradientBrush    = false;
            Method              = method;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        public float LineWidth { get; set; }
        /// <summary>
        /// Gets or sets the line color of the method.
        /// </summary>
        public Color LineColor { get; set; }
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color Background { get; set; }
        /// <summary>
        /// Gets or sets the method to render.
        /// </summary>
        public classLCMethod Method { get; set; }
        /// <summary>
        /// Gets or sets whether to use a gradient brush (true) or solid brush (false);
        /// </summary>
        public bool UseGradientBrush { get; set; }
        #endregion
    }
}
