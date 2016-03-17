using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class that renders LC-methods like the column sample renderer but without column data.
    /// </summary>
    public class classLCMethodConversationRenderer : classLCMethodRenderer
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

        private Rectangle[] buttonLocations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public classLCMethodConversationRenderer()
        {
            PixelPadding = 5.0F;
            ColumnNames = new List<string>();
            StartEventIndex = 0;
            buttonLocations = new Rectangle[2];
        }

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
            ref float y)
        {
            using (Font font = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Italic))
            {
                SizeF fontSize = graphics.MeasureString(columnName, font);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Black)))
                {
                    y += fontSize.Height;
                    graphics.DrawString(columnName, font, brush, x, y);
                }
            }
        }

        public Rectangle[] GetButtonLocations()
        {
            return buttonLocations;
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
            float offset = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;
            offset = Math.Min(offset, CONST_HEADER_PADDING_MAX);

            // This tells us how much room we get per method or column spacing.
            if (ColumnNames.Count < 1)
                return;
            float timePadding = 64;

            float widthPer = (bounds.Width - timePadding - (CONST_COLUMN_SPACING * ColumnNames.Count)) /
                             Convert.ToSingle(ColumnNames.Count);
            widthPer = Math.Max(CONST_MIN_WIDTH, widthPer);


            // Draw the data for the columns
            float width, height, x, top;
            top = bounds.Top;
            x = 0; // just to get rid of compiler complaints.


            // Draw the columns
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                //top     = offset + ((heightPer + CONST_COLUMN_SPACING) * i) - CONST_COLUMN_BACKGROUND_HEIGHT_PADDING;
                top = bounds.Top;

                x = ((widthPer + CONST_COLUMN_SPACING) * i) + timePadding;
                width = widthPer;
                height = bounds.Height - offset;

                RectangleF area = new RectangleF(x, top, width, height);
                int color = 245;
                graphics.FillRectangle(new LinearGradientBrush(area,
                    Color.FromArgb(255, color, color, color),
                    Color.White,
                    LinearGradientMode.Horizontal),
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
                    ref top);
            }
            top = bounds.Top + offset;
            Rectangle scrollUpButtonLocation = new Rectangle((int) (x + widthPer) + 5, 0,
                (int) (bounds.Width - (x + widthPer)), (int) offset);
            Rectangle scrollDownButtonLocation = new Rectangle((int) (x + widthPer) + 5,
                (int) (bounds.Height - offset - 5), (int) (bounds.Width - (x + widthPer)), (int) offset);
            buttonLocations[0] = scrollUpButtonLocation;
            buttonLocations[1] = scrollDownButtonLocation;
            List<classLCEvent> alignedEvents = new List<classLCEvent>();
            if (methods != null && methods.Count > 0)
            {
                // //////////////////////////////////////////////////////////////////////////////////////////
                // Find the start and end times of the samples so we can scale everything accordingly.
                // //////////////////////////////////////////////////////////////////////////////////////////
                DateTime methodStart;
                TimeSpan methodDuration;
                FindTimeExtremas(methods, out methodStart, out methodDuration);

                foreach (classLCMethod method in methods)
                {
                    if (method == null)
                        continue;

                    foreach (classLCEvent lcEvent in method.Events)
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

                // Draw each method
                int columnID = 0;

                Dictionary<classLCEvent, classLCMethod> methodMap = new Dictionary<classLCEvent, classLCMethod>();
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

                using (var timeFont = new Font(FontFamily.GenericSansSerif, 8.0F))
                {
                    using (var brush = new SolidBrush(Color.Gray))
                    {
                        using (var linePen = new Pen(brush))
                        {
                            foreach (var simList in eventList)
                            {
                                if (StartEventIndex <= eventList.ToList().IndexOf(simList))
                                {
                                    // Render the time
                                    var timeSpanSinceStart = simList[0].Start.Subtract(firstEvent);

                                    graphics.DrawLine(linePen, 0, top, bounds.Width - CONST_COLUMN_SPACING, top);
                                    graphics.DrawString(timeSpanSinceStart.ToString("g"), timeFont, brush, 0, top);
                                    graphics.DrawString(simList[0].Start.ToString("h:mm:ss"), timeFont, brush, 0,
                                        top + timeFont.Height);
                                    graphics.DrawLine(linePen, 0, top + eventHeight, bounds.Width - CONST_COLUMN_SPACING,
                                        top + eventHeight);
                                    foreach (var lcEvent in simList)
                                    {
                                        var method = methodMap[lcEvent];

                                        if (method == null)
                                            continue;

                                        columnID = method.Column;

                                        if (columnID < 0)
                                        {
                                            columnID = ColumnNames.Count;
                                        }

                                        //Calculate the number of pixels the method should start from based on its column.
                                        x = ((widthPer + CONST_COLUMN_SPACING) * columnID) + timePadding;

                                        RectangleF methodBounds = new RectangleF(x, top, widthPer, eventHeight);

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
        }
    }
}