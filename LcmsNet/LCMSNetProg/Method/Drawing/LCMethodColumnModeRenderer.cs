using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK;
using LcmsNetData;
using LcmsNetData.Configuration;
using LcmsNetData.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class that renders LC-methods like the column sample renderer but without column data.
    /// </summary>
    public class LCMethodColumnModeRenderer : LCMethodRenderer
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
        public LCMethodColumnModeRenderer()
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
        public void RenderColumnName(DrawingContext graphics, string columnName, double x, double y)
        {
            var font = new Typeface(new FontFamily("Microsoft Sans Serif"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
            var brushColor = Colors.Black;
            brushColor.A = 128;
            var brush = new SolidColorBrush(brushColor);
            var columnNameText = new FormattedText(columnName, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, EightPt, brush, FluidicsModerator.Moderator.DrawingScaleFactor);
            graphics.DrawText(columnNameText, new Point(x, y - columnNameText.Height));
        }

        /// <summary>
        /// Renders the samples provided in a column ordered way.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="methods"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        /// <param name="progress"></param>
        public override void RenderLCMethod(DrawingContext graphics, Rect bounds, List<LCMethod> methods, DateTime startTime, TimeSpan duration, Dictionary<IDevice, Color> colorMap, DateTime progress)
        {
            // Calculate formatting paddings.
            // This tells us how far down from the top of the rendering area we are before we draw anything!
            var offset = bounds.Height * CONST_HEADER_PADDING;
            offset = Math.Min(offset, CONST_HEADER_PADDING_MAX);
            // This tells us how much room we get per method or column spacing.
            var columnsEnabled = CartConfiguration.Columns.Count(x => x.Status != ColumnStatus.Disabled);
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true))
            {
                // Also show the "Special" column
                columnsEnabled += 1;
            }
            //var heightPer = (bounds.Height - CONST_COLUMN_SPACING * CONST_NUMBER_OF_COLUMNS - offset) / Convert.ToSingle(CONST_NUMBER_OF_COLUMNS);
            var heightPer = (bounds.Height - CONST_COLUMN_SPACING * columnsEnabled - offset) / Convert.ToSingle(columnsEnabled);
            heightPer = Math.Max(CONST_MIN_HEIGHT, Math.Min(heightPer, (int)(CONST_MAX_HEIGHT * 3)));

            // Draw the data for the columns
            for (var i = 0; i < ColumnNames.Count; i++)
            {
                var top = offset + ((heightPer + CONST_COLUMN_SPACING) * i) - CONST_COLUMN_BACKGROUND_HEIGHT_PADDING;
                var x = bounds.X;
                var width = bounds.Width;
                var height = heightPer;

                var area = new Rect(x, top, width, height);
                graphics.DrawRectangle(new LinearGradientBrush(Color.FromArgb(255, 245, 245, 245), Colors.White, 90), new Pen(Brushes.LightGray, 2.0F), area);

                RenderColumnName(graphics, string.Format("Column {0}: {1}", i + 1, ColumnNames[i]), x, top);
            }

            var alignedEvents = new List<LCEvent>();
            if (methods != null && methods.Count > 0)
            {
                // Find the start and end times of the samples so we can scale everything accordingly.
                DateTime methodStart;
                TimeSpan methodDuration;
                FindTimeExtrema(methods, out methodStart, out methodDuration);

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

                RenderAcquisitionTimes(graphics, bounds, startTime, duration, alignedEvents);

                // Draw each method
                foreach (var method in methods)
                {
                    if (method == null)
                        continue;

                    var columnID = method.Column;

                    if (columnID < 0)
                    {
                        //columnID = CONST_NUMBER_OF_COLUMNS;
                        columnID = columnsEnabled;
                    }

                    // Calculate the number of pixels the method should start from based on its time value.
                    var top = offset + ((heightPer + CONST_COLUMN_SPACING) * columnID);

                    var methodBounds = new Rect(bounds.X, top, bounds.Width, heightPer);

                    RenderLCMethod(graphics, methodBounds, method, startTime, duration, colorMap, progress);
                }
            }

            RenderOptimizationsOnTimeline(graphics, bounds, startTime, duration, methods);

            // Draw a timeline
            RenderTimeline(graphics, bounds, startTime, duration);
        }
    }
}
