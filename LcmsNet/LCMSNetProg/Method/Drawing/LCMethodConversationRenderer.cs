using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class that renders LC-methods like the column sample renderer but without column data.
    /// </summary>
    public class LCMethodConversationRenderer : LCMethodRenderer
    {
        /// <summary>
        /// Constant defining the number of columns to use.
        /// </summary>
        private const int CONST_NUMBER_OF_COLUMNS = 4;

        /// <summary>
        /// Number of pixels to pad the column background streak.
        /// </summary>
        private const int CONST_COLUMN_BACKGROUND_WIDTH_PADDING = 15;

        /// <summary>
        /// Spacing between column plots.
        /// </summary>
        private const int CONST_COLUMN_SPACING = 15;

        private const int CONST_MIN_WIDTH = 60;

        private readonly Rect[] buttonLocations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LCMethodConversationRenderer()
        {
            PixelPadding = 5.0F;
            ColumnNames = new List<string>();
            StartEventIndex = 0;
            buttonLocations = new Rect[2];
        }

        /// <summary>
        /// Renders the column name on the background.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="columnName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RenderColumnName(DrawingContext graphics, string columnName, double x, ref double y)
        {
            var font = new Typeface(new FontFamily("Microsoft Sans Serif"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
            var brushColor = Colors.Black;
            brushColor.A = 128;
            var brush = new SolidColorBrush(brushColor);
            var columnNameText = new FormattedText(columnName, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, 8.0F * (96.0 / 72.0), brush);
            y += columnNameText.Height;
            graphics.DrawText(columnNameText, new Point(x, y));
        }

        public Rect[] GetButtonLocations()
        {
            return buttonLocations;
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
            var offset = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;
            offset = Math.Min(offset, CONST_HEADER_PADDING_MAX);

            // This tells us how much room we get per method or column spacing.
            if (ColumnNames.Count < 1)
                return;
            double timePadding = 64;

            var widthPer = (bounds.Width - timePadding - (CONST_COLUMN_SPACING * ColumnNames.Count)) / Convert.ToSingle(ColumnNames.Count);
            widthPer = Math.Max(CONST_MIN_WIDTH, widthPer);

            // Draw the data for the columns
            double top;
            double x = 0;

            // Draw the columns
            for (var i = 0; i < ColumnNames.Count; i++)
            {
                //top     = offset + ((heightPer + CONST_COLUMN_SPACING) * i) - CONST_COLUMN_BACKGROUND_HEIGHT_PADDING;
                top = bounds.Top;

                x = ((widthPer + CONST_COLUMN_SPACING) * i) + timePadding;
                var width = widthPer;
                var height = bounds.Height - offset;

                var area = new Rect(x, top, width, height);
                graphics.DrawRectangle(new LinearGradientBrush(Color.FromArgb(255, 245, 245, 245), Colors.White, 0), new Pen(Brushes.LightGray, 2.0F), area);

                RenderColumnName(graphics, string.Format("Column {0}: {1}", i + 1, ColumnNames[i]), x, ref top);
            }
            top = bounds.Top + offset;

            if (bounds.Width - (x + widthPer) > 5)
            {
                buttonLocations[0] = new Rect((x + widthPer) + 5, 0, (bounds.Width - (x + widthPer)), offset);
                buttonLocations[1] = new Rect((x + widthPer) + 5, (bounds.Height - offset - 5), (bounds.Width - (x + widthPer)), offset);
            }
            else
            {
                var negOffset = Math.Abs(bounds.Width - (x + widthPer)) + 5;

                buttonLocations[0] = new Rect((x + widthPer) + 5 - negOffset, 0, (bounds.Width - (x + widthPer)) + negOffset, offset);
                buttonLocations[1] = new Rect((x + widthPer) + 5 - negOffset, (bounds.Height - offset - 5), (bounds.Width - (x + widthPer)) + negOffset, offset);
            }

            var alignedEvents = new List<LCEvent>();
            if (methods != null && methods.Count > 0)
            {
                // Find the start and end times of the samples so we can scale everything accordingly.
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

                RenderAcquisitionTimes(graphics, bounds, startTime, duration, alignedEvents);

                // Draw each method

                var methodMap = new Dictionary<LCEvent, LCMethod>();
                foreach (var method in methods)
                {
                    method.Events.ForEach(u => methodMap.Add(u, method));
                }

                var eventList = FluidicsSimulator.FluidicsSimulator.BuildEventList(methods, startTime);
                float eventHeight = 48;
                float eventPadding = 5;

                if (eventList.Count < 0)
                    return;

                var firstEvent = methods[0].Start;

                //var timeFont = new Font(FontFamily.GenericSansSerif, 8.0F);
                var timeFont = new Typeface("Microsoft Sans Serif");
                var brush = Brushes.Gray;
                var linePen = new Pen(brush, 1);
                foreach (var simList in eventList)
                {
                    if (StartEventIndex <= eventList.ToList().IndexOf(simList))
                    {
                        // Render the time
                        var timeSpanSinceStart = simList[0].Start.Subtract(firstEvent);

                        graphics.DrawLine(linePen, new Point(0, top), new Point(bounds.Width - CONST_COLUMN_SPACING, top));
                        var timeSpanSinceStartText = new FormattedText(timeSpanSinceStart.ToString("g"), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, timeFont, 8.0F * (96.0 / 72.0), brush);
                        var simListStart = new FormattedText(simList[0].Start.ToString("h:mm:ss"), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, timeFont, 8.0F * (96.0 / 72.0), brush);
                        graphics.DrawText(timeSpanSinceStartText, new Point(0, top));
                        graphics.DrawText(simListStart, new Point(0, top + timeSpanSinceStartText.Height));
                        graphics.DrawLine(linePen, new Point(0, top + eventHeight), new Point(bounds.Width - CONST_COLUMN_SPACING, top + eventHeight));
                        foreach (var lcEvent in simList)
                        {
                            var method = methodMap[lcEvent];

                            if (method == null)
                                continue;

                            var columnID = method.Column;

                            if (columnID < 0)
                            {
                                columnID = ColumnNames.Count;
                            }

                            //Calculate the number of pixels the method should start from based on its column.
                            x = ((widthPer + CONST_COLUMN_SPACING) * columnID) + timePadding;

                            var methodBounds = new Rect(x, top, widthPer, eventHeight);

                            var color = colorMap[lcEvent.Device];
                            RenderLCEvent(graphics, methodBounds, lcEvent, color);
                        }

                        top += eventHeight + eventPadding;
                    }
                }
            }
        }
    }
}
