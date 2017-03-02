using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class that renders LC-methods like the column sample renderer but without column data.
    /// </summary>
    public class classLCMethodColumnModeRenderer : classLCMethodRenderer
    {
        /// <summary>
        /// Constant defining the number of columns to use.
        /// </summary>
        private const int CONST_NUMBER_OF_COLUMNS = 4;

        /// <summary>
        /// Number of pixels to pad the column background streak.
        /// </summary>
        private const int CONST_COLUMN_BACKGROUND_HEIGHT_PADDING = 15;

        /// <summary>
        /// Spacing between column plots.
        /// </summary>
        private const int CONST_COLUMN_SPACING = 15;

        /// <summary>
        /// Constructor.
        /// </summary>
        public classLCMethodColumnModeRenderer()
        {
            PixelPadding = 5.0F;
            ColumnNames = new List<string>();
        }

        /// <summary>
        /// Note that this overrides the base class ColumnNames
        /// </summary>
        public new List<string> ColumnNames { get; set; }

        /// <summary>
        /// Renders the column name on the background.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="columnName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RenderColumnName(Graphics graphics,
            string columnName,
            float x,
            float y)
        {
            using (var font = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Italic))
            {
                var fontSize = graphics.MeasureString(columnName, font);
                using (var brush = new SolidBrush(Color.FromArgb(128, Color.Black)))
                {
                    graphics.DrawString(columnName, font, brush, x, y - fontSize.Height);
                }
            }
        }

        /// <summary>
        /// Renders the samples provided in a column ordered way.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="samples"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        /// <param name="progress"></param>
        public override void RenderLCMethod(
            Graphics graphics,
            RectangleF bounds,
            List<classLCMethod> methods,
            DateTime startTime,
            TimeSpan duration,
            Dictionary<IDevice, Color> colorMap,
            DateTime progress)
        {
            // //////////////////////////////////////////////////////////////////////////////////////////
            // Calculate formatting paddings.
            // //////////////////////////////////////////////////////////////////////////////////////////
            //
            // This tells us how far down from the top of the rendering area we are before we draw anything!
            //
            var offset = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;
            offset = Math.Min(offset, CONST_HEADER_PADDING_MAX);
            //
            // This tells us how much room we get per method or column spacing.
            //
            var heightPer = (bounds.Height - CONST_COLUMN_SPACING * CONST_NUMBER_OF_COLUMNS - offset) /
                              Convert.ToSingle(CONST_NUMBER_OF_COLUMNS);
            heightPer = Math.Max(CONST_MIN_HEIGHT, Math.Min(heightPer, CONST_MAX_HEIGHT));


            // //////////////////////////////////////////////////////////////////////////////////////////
            // Draw the data for the columns
            // //////////////////////////////////////////////////////////////////////////////////////////
            for (var i = 0; i < ColumnNames.Count; i++)
            {
                float width, height, x, top;
                top = offset + ((heightPer + CONST_COLUMN_SPACING) * i) - CONST_COLUMN_BACKGROUND_HEIGHT_PADDING;
                x = bounds.X;
                width = bounds.Width;
                height = heightPer;

                var area = new RectangleF(x, top, width, height);
                var color = 245;
                graphics.FillRectangle(new LinearGradientBrush(area,
                    Color.FromArgb(255, color, color, color),
                    Color.White,
                    LinearGradientMode.Vertical),
                    x,
                    top,
                    width,
                    height);

                graphics.DrawRectangle(new Pen(new SolidBrush(Color.LightGray), 2.0F),
                    x,
                    top,
                    width,
                    height);

                RenderColumnName(graphics,
                    string.Format("Column {0}: {1}", i + 1, ColumnNames[i]),
                    x,
                    top);
            }


            var alignedEvents = new List<classLCEvent>();
            if (methods != null && methods.Count > 0)
            {
                // //////////////////////////////////////////////////////////////////////////////////////////
                // Find the start and end times of the samples so we can scale everything accordingly.
                // //////////////////////////////////////////////////////////////////////////////////////////
                DateTime methodStart;
                TimeSpan methodDuration;
                FindTimeExtremas(methods, out methodStart, out methodDuration);

                foreach (var method in methods)
                {
                    if (method == null)
                        continue;

                    foreach (var lcEvent in method.Events)
                    {
                        if (lcEvent.OptimizeWith)
                        {
                            alignedEvents.Add(lcEvent);
                        }
                    }
                }

                RenderAcquisitionTimes(graphics,
                    bounds,
                    startTime,
                    duration,
                    alignedEvents);

                // //////////////////////////////////////////////////////////////////////////////////////////
                // Draw each method
                // //////////////////////////////////////////////////////////////////////////////////////////
                var columnID = 0;
                foreach (var method in methods)
                {
                    if (method == null)
                        continue;

                    columnID = method.Column;

                    if (columnID < 0)
                    {
                        columnID = CONST_NUMBER_OF_COLUMNS;
                    }

                    //
                    // Calculate the number of pixels the method should start from based on its time value.
                    //
                    var top = offset + ((heightPer + CONST_COLUMN_SPACING) * columnID);

                    var methodBounds = new RectangleF(bounds.X, top, bounds.Width, heightPer);

                    RenderLCMethod(graphics,
                        methodBounds,
                        method,
                        startTime,
                        duration,
                        colorMap,
                        progress);
                }
            }

            RenderOptimizationsOnTimeline(graphics, bounds, startTime, duration, methods);

            // Draw a timeline
            RenderTimeline(graphics,
                bounds,
                startTime,
                duration);
        }
    }
}