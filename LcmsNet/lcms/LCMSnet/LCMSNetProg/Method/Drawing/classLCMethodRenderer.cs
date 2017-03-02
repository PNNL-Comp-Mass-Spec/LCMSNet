using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class for rendering LC Methods
    /// </summary>
    public class classLCMethodRenderer
    {
        /// <summary>
        /// Default constructor whose pixel padding is 0.0F
        /// </summary>
        public classLCMethodRenderer()
        {
            PixelPadding = 0.0F;
            TimelinePixelSpacing = 5.0F;
            TimelineDisplayDatesCount = 10;
            TimelineDrawRelativeTimes = true;
            AcquisitionColor = Color.Red;
            ColumnNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets the color to use when rendering the acquisition times.
        /// </summary>
        public Color AcquisitionColor { get; set; }

        #region Time Conversions

        /// <summary>
        /// Finds the start time and duration of the samples provided.
        /// </summary>
        /// <param name="samples">Samples to analyze</param>
        /// <param name="start">Starting time.</param>
        /// <param name="duration">Duration.</param>
        protected void FindTimeExtremas(List<classLCMethod> methods,
            out DateTime start,
            out TimeSpan duration)
        {
            start = DateTime.MaxValue;
            var end = DateTime.MinValue;
            duration = new TimeSpan(0, 0, 0, 0);

            foreach (var method in methods)
            {
                if (start.CompareTo(method.Start) > 0)
                    start = method.Start;

                if (end.CompareTo(method.End) < 0)
                    end = method.End;
            }
            duration = end.Subtract(start);
        }

        #endregion

        #region Data Construction

        /// <summary>
        /// Creates a map from a device to a color.
        /// </summary>
        /// <param name="devices">Devices to map.</param>
        /// <returns>Dictionary mapping devices to colors.</returns>
        public static Dictionary<IDevice, Color> ConstructDeviceColorMap(List<IDevice> devices)
        {
            var deviceMap = new Dictionary<IDevice, Color>();

            //
            // Iterate through the device list to map a Device to a color
            //
            var colors = new[]
            {
                Color.Red,
                Color.Lime,
                Color.Yellow,
                Color.Salmon,
                Color.RoyalBlue,
                Color.LightBlue,
                Color.LightGreen,
                Color.White,
                Color.Thistle,
                Color.PeachPuff,
                Color.CadetBlue,
                Color.PaleGreen
            };

            var i = 0;
            var count = colors.Length;
            foreach (var device in devices)
            {
                deviceMap.Add(device, colors[i % count]);
                i++;
            }

            return deviceMap;
        }

        #endregion

        #region CONSTANTS

        protected const float CONST_HEADER_PADDING = .25F;
        protected const float CONST_HEADER_PADDING_MAX = 25.0F;
        protected const float CONST_TRIANGLE_HEIGHT = 15.0F;
        protected const float CONST_MIN_HEIGHT = 8.0F;
        protected const float CONST_MID_HEIGHT = 30.0F;
        protected const float CONST_MAX_HEIGHT = 60.0F;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pixel padding to enforce against the clipping region bounds.
        /// </summary>
        public float PixelPadding { get; set; }

        /// <summary>
        /// Gets or sets the spacing in Pixels between two timeline tickmarks
        /// </summary>
        public float TimelinePixelSpacing { get; set; }

        /// <summary>
        /// Gets or sets the number of times to display a date/time.
        /// </summary>
        public int TimelineDisplayDatesCount { get; set; }

        /// <summary>
        /// Gets or sets the flag to draw relative times (seconds/hours) or absolute (date/time).
        /// </summary>
        public bool TimelineDrawRelativeTimes { get; set; }

        public List<string> ColumnNames { get; set; }

        public int StartEventIndex { get; set; }

        public int StopEventIndex { get; set; }

        #endregion

        #region Rendering

        /// <summary>
        /// Draws when the instrument is acquiring data.
        /// </summary>
        public virtual void RenderAcquisitionTimes(Graphics g,
            RectangleF bounds,
            DateTime start,
            TimeSpan duration,
            List<classLCEvent> events)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);
            var startPoint = PixelPadding;
            startPoint += Convert.ToSingle(duration.TotalSeconds * ppt);

            //
            // This gives us the time between ticks as time units.
            //
            var pixelTime = TimelinePixelSpacing * (1.0F / ppt);

            //
            // Then we plot a number of ticks until the end - first here, we setup some variables
            // to help us in later calculations.
            //
            var totalTime = Convert.ToSingle(duration.TotalSeconds);

            using (var brush = new SolidBrush(AcquisitionColor))
            {
                var y = bounds.Height - 10.0F;
                var height = 15.0F;
                foreach (var lcEvent in events)
                {
                    var span = lcEvent.Start.Subtract(start);

                    var startX = Convert.ToSingle(span.TotalSeconds) * ppt;
                    var length = Convert.ToSingle(lcEvent.HoldTime.TotalSeconds) * ppt;
                    var x = bounds.X + startX;
                    g.FillRectangle(brush, x, y, length, height);
                }
            }
        }

        /// <summary>
        /// Renders a timeline at the bounds location on the graphics object between start and finish.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public virtual void RenderTimeline(Graphics graphics,
            RectangleF bounds,
            DateTime start,
            TimeSpan duration)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);
            var startPoint = PixelPadding;
            startPoint += Convert.ToSingle(duration.TotalSeconds * ppt);

            //
            // This gives us the time between ticks as time units.
            //
            var pixelTime = TimelinePixelSpacing * (1.0F / ppt);

            //
            // Then we plot a number of ticks until the end - first here, we setup some variables
            // to help us in later calculations.
            //
            var currentTime = 0.0F;
            var totalTime = Convert.ToSingle(duration.TotalSeconds);


            //
            // Here we figure out the heights of ticks and variables to use to draw them.  Also draw the base tick line.
            //
            var tickHeight = 10.0F;
            var y = bounds.Height - 5.0F;
            var timelineBrush = new SolidBrush(Color.Black);
            var pen = new Pen(timelineBrush);
            graphics.DrawLine(pen, bounds.X, y, bounds.Width, y);

            //
            // Now we need to figure out how many times to draw a time...
            // Here we see how many seconds should go by before drawing a date time.
            //
            var pixelsPerDate = ((bounds.Width - bounds.X) / TimelineDisplayDatesCount);
            var dateFont = new Font(FontFamily.GenericSansSerif, 6.0F);

            //
            // Loop drawing the ticks...
            //
            while (currentTime - totalTime <= 0 && pixelTime > 0)
            {
                //
                // Now we have to conver the x value (time = currentTime) into pixels...err
                //
                var x = bounds.X + (currentTime * ppt);

                //
                // Draw a tick
                //
                graphics.DrawLine(pen, x, y, x, y - tickHeight);
                currentTime += pixelTime;
            }
            graphics.DrawLine(pen, bounds.Width, y, bounds.Width, y - tickHeight);

            //
            // Draw the date strings (times if relative flag set).
            //
            var timePerDraw = Convert.ToSingle(duration.TotalSeconds) / TimelineDisplayDatesCount;
            currentTime = 0;
            var datePen = new Pen(new SolidBrush(Color.LightGray));

            for (var i = 0; i <= TimelineDisplayDatesCount; i++)
            {
                var dateString = string.Format("{0:0}", currentTime);

                //if (TimelineDrawRelativeTimes == false)
                //    dateString = start.AddSeconds(currentTime).ToShortTimeString();


                var x = bounds.X + (pixelsPerDate * Convert.ToSingle(i));

                graphics.DrawLine(datePen, x, y - tickHeight, x, 0);

                graphics.DrawString(dateString, dateFont, timelineBrush, x - 3.0F, y - tickHeight * 2);

                dateString = start.AddSeconds(currentTime).ToLongTimeString();
                graphics.DrawString(dateString, dateFont, timelineBrush, x - 3.0F, y - tickHeight * 3);
                currentTime += timePerDraw;
            }

            datePen.Dispose();
            pen.Dispose();
            timelineBrush.Dispose();
            dateFont.Dispose();
        }

        /// <summary>
        /// Renders a timeline at the bounds location on the graphics object between start and finish.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public virtual void RenderOptimizationsOnTimeline(Graphics graphics,
            RectangleF bounds,
            DateTime start,
            TimeSpan duration,
            List<classLCMethod> methods)
        {
            if (methods == null || methods.Count == 0)
                return;

            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);
            var startPoint = PixelPadding;
            startPoint += Convert.ToSingle(duration.TotalSeconds * ppt);

            var pixelTime = TimelinePixelSpacing * (1.0F / ppt);
            var totalTime = Convert.ToSingle(duration.TotalSeconds);
            var y = bounds.Height - 5.0F;

            var optimize = "acquiring";
            using (var optBrush = new SolidBrush(AcquisitionColor))
            {
                using (var timelineBrush = new SolidBrush(Color.FromArgb(128, 128, 128, 128)))
                {
                    using (var dateFont = new Font(FontFamily.GenericSansSerif, 6.0F))
                    {
                        foreach (var method in methods)
                        {
                            foreach (var lcEvent in method.Events)
                            {
                                if (lcEvent.OptimizeWith)
                                {
                                    var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;
                                    var x = bounds.X +
                                              Convert.ToSingle(lcEvent.Start.Subtract(start).TotalSeconds) * ppt;
                                    var alignedBounds = new RectangleF(x, y - 15.0F, eventWidth, 20.0F);
                                    graphics.SetClip(alignedBounds, CombineMode.Replace);
                                    graphics.FillRectangle(optBrush, x, y - 15.0F, eventWidth, 20.0F);
                                    graphics.DrawString(optimize, dateFont, timelineBrush, x, y);
                                    graphics.ResetClip();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders an LC-Event
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="lcEvent"></param>
        /// <param name="backColor"></param>
        public virtual void RenderLCEvent(Graphics graphics,
            RectangleF bounds,
            classLCEvent lcEvent,
            Color backColor)
        {
            // Make sure that we don't have some garbage event that has a zero time width.
            if (bounds.Width <= 0)
                return;

            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Construct a gradient brush so that we can see the text better
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Brush brush = new LinearGradientBrush(bounds,
                Color.White,
                backColor,
                LinearGradientMode.Horizontal);


            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Then fill the background of the event
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            graphics.FillRectangle(brush, bounds);
            brush.Dispose();

            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Setup the outline data objects
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var outlineBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            // If the lcEvent is currently executing, outline it in green, otherwise outline it in black.
            var outlineBrush = lcEvent.MethodData.Executing
                ? new SolidBrush(Color.Green)
                : new SolidBrush(Color.Black);

            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Render the name of the device
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var nameFont = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Bold, GraphicsUnit.Point);
            Brush nameBrush = new SolidBrush(Color.Black);
            var name = lcEvent.Name;

            if (bounds.Height >= CONST_MID_HEIGHT)
            {
                graphics.DrawString(lcEvent.Device.Name,
                    nameFont,
                    nameBrush,
                    bounds.X,
                    bounds.Y);

                var nameLength = graphics.MeasureString(name, nameFont);

                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // See if the name is too long then we can make an ellipse type construct...
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (nameLength.Width > outlineBounds.Width)
                {
                    var names = name.Split(' ');
                    name = "";
                    foreach (var n in names)
                    {
                        name += n[0].ToString();
                    }
                    nameLength = graphics.MeasureString(name, nameFont);
                }

                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Render the name of the event
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                graphics.DrawString(name,
                    nameFont,
                    nameBrush,
                    outlineBounds.X,
                    bounds.Y + (bounds.Height / 2.0F));


                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Render the params of the event
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (lcEvent.Parameters.Length > 0 && lcEvent.Parameters[0] != null)
                {
                    graphics.DrawString(lcEvent.Parameters[0].ToString(),
                        nameFont,
                        new SolidBrush(Color.FromArgb(80, 80, 80)),
                        outlineBounds.X,
                        outlineBounds.Y + (outlineBounds.Height / 2.0F) + nameLength.Height);
                }
            }

            //
            // Finally draw the outline
            //
            var outlinePen = new Pen(outlineBrush);
            if (lcEvent.MethodData.Executing)
            {
                outlinePen.Width = 4;
            }
            graphics.DrawRectangle(outlinePen, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            outlineBrush.Dispose();
            outlinePen.Dispose();

            nameBrush.Dispose();
            nameFont.Dispose();
        }

        /// <summary>
        /// Renders a list of events to the graphics object
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="allBounds"></param>
        /// <param name="events"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        public virtual void RenderLCEventError(Graphics graphics,
            RectangleF allBounds,
            classLCEvent lcEvent,
            DateTime startTime,
            TimeSpan duration)
        {
            // Pixels per time (PPT)
            var ppt = (allBounds.Width - allBounds.X) / Convert.ToSingle(duration.TotalSeconds);

            var startPoint = allBounds.X + Convert.ToSingle((lcEvent.Start.Subtract(startTime)).TotalSeconds * ppt);
            var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;
            var x = startPoint;
            var y = allBounds.Y;

            var triHeight = CONST_TRIANGLE_HEIGHT;
            var triWidth = triHeight / 2.0F;

            var eventBounds = new RectangleF(x - triWidth, y, eventWidth, allBounds.Height * .8F);
            var color = Color.Red;

            graphics.SetClip(eventBounds, CombineMode.Replace);

            var points = new PointF[3];
            points[0] = new PointF(startPoint, y + triHeight);
            points[1] = new PointF(startPoint + triWidth, y);
            points[2] = new PointF(startPoint - triWidth, y);

            // Black Outline
            using (var pen = new Pen(Color.Black, 2.0F))
            {
                graphics.DrawPolygon(pen, points);
            }

            // Red Fill
            using (var brush = new SolidBrush(Color.Red))
            {
                graphics.FillPolygon(brush, points);
            }


            graphics.ResetClip();
        }

        /// <summary>
        /// Renders a list of events to the graphics object
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="allBounds"></param>
        /// <param name="events"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        public virtual void RenderLCEvent(Graphics graphics,
            RectangleF allBounds,
            List<classLCEvent> events,
            DateTime startTime,
            TimeSpan duration,
            Dictionary<IDevice, Color> colorMap)
        {
            var bounds = allBounds;
            bounds.Height = allBounds.Height * .80F;

            //
            // Figure out how many pixels are in a second
            //     ppt breaks the naming convention, however, we are using the
            //     standard "parts per" or "pixels per" time here.
            //
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);

            //
            // Identify the starting pixel using any padding for pixels
            //
            var startPoint = bounds.X;
            startPoint += Convert.ToSingle((events[0].Start.Subtract(startTime)).TotalSeconds * ppt);

            //
            // For every event render a rectangle.
            //
            foreach (var lcEvent in events)
            {
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Calculate how wide the event will be
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;

                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Define start x,y position for event block
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var x = startPoint;
                var y = bounds.Y;

                var eventBounds = new RectangleF(x, y, eventWidth, bounds.Height);

                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Find the color mapping, if it doesnt exist
                //     then we assign a gradient gray color to it
                // We also may want to show progress in the event...so here we only fill if it's the current event
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var color = Color.LightGray;
                if (colorMap.ContainsKey(lcEvent.Device))
                    color = colorMap[lcEvent.Device];


                if (lcEvent.OptimizeWith)
                {
                    var alignedBounds = new RectangleF(x, y - 15.0F, eventWidth, bounds.Height + 20.0F);
                    graphics.SetClip(alignedBounds, CombineMode.Replace);
                    using (var optBrush = new SolidBrush(AcquisitionColor))
                    {
                        graphics.FillRectangle(optBrush, x, y - 15.0F, eventWidth, bounds.Height + 20.0F);
                    }
                    graphics.ResetClip();
                }
                graphics.SetClip(eventBounds, CombineMode.Replace);
                RenderLCEvent(graphics, eventBounds, lcEvent, color);
                graphics.ResetClip();
                startPoint += eventWidth;
            }
        }

        /// <summary>
        /// Renders the LC Method provided to the graphics object.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="methods"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        /// <param name="progress"></param>
        public virtual void RenderLCMethod(Graphics graphics,
            RectangleF bounds,
            classLCMethod method,
            DateTime startTime,
            TimeSpan duration,
            Dictionary<IDevice, Color> colorMap,
            DateTime progress)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);

            //
            // Find the first pixel
            //
            var startPoint = bounds.X;
            startPoint += Convert.ToSingle((method.Start.Subtract(startTime)).TotalSeconds * ppt);

            //
            // Find out the last pixel.
            //
            var endPoint = bounds.X + Convert.ToSingle((method.End.Subtract(startTime)).TotalSeconds * ppt);

            if (method.IsSpecialMethod)
            {
                using (var specialBrush = new SolidBrush(Color.FromArgb(64, Color.Orange)))
                {
                    var newBounds = new RectangleF(startPoint - 5, bounds.Top - 5, endPoint, bounds.Height);
                    graphics.FillRectangle(specialBrush, newBounds);
                }
                var specialName = "special";
                using (var font = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Italic))
                {
                    var fontSize = graphics.MeasureString(specialName, font);
                    using (var specialBrush = new SolidBrush(Color.FromArgb(70, Color.Black)))
                    {
                        graphics.DrawString(specialName, font, specialBrush, startPoint, bounds.Top + fontSize.Height);
                    }
                }
            }

            //
            // Event details
            //
            RenderLCEvent(graphics,
                bounds,
                method.Events,
                startTime,
                duration,
                colorMap);


            //
            // Name of the method
            //
            var nameFont = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Bold);
            var timeFont = new Font(FontFamily.GenericSansSerif, 6.0F, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Black);

            graphics.DrawString(method.Name,
                nameFont,
                brush,
                startPoint,
                bounds.Y - 13);

            //
            // Draw the duration strings for the method
            //

            var span = method.Duration;

            var durationString = span.ToString();

            var nameLength = graphics.MeasureString(method.Name, nameFont);
            var startLength = graphics.MeasureString(method.Start.ToLongTimeString(), timeFont);
            var durationLength = graphics.MeasureString(durationString, timeFont);
            var endLength = graphics.MeasureString(method.End.ToLongTimeString(), timeFont);

            var x = startPoint + nameLength.Width + 15.0F;

            if ((x + startLength.Width) < endPoint)
            {
                graphics.DrawString(method.Start.ToLongTimeString(),
                    timeFont,
                    brush,
                    x,
                    bounds.Y - 13);
            }

            x += startLength.Width + 15.0F;
            if ((x + durationLength.Width) < endPoint)
            {
                graphics.DrawString(durationString,
                    timeFont,
                    brush,
                    x,
                    bounds.Y - 13);
            }
            x += durationLength.Width + 15.0F;

            if ((x + endLength.Width) <= endPoint)
            {
                graphics.DrawString(method.End.ToLongTimeString(),
                    timeFont,
                    brush,
                    endPoint - durationLength.Width,
                    bounds.Y - 13);
            }
        }

        /// <summary>
        /// Renders the methods to the graphics object provided
        /// between the start and end time and up to the progress indicator.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="methods"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        /// <param name="progress"></param>
        public virtual void RenderLCMethod(Graphics graphics,
            RectangleF bounds,
            List<classLCMethod> methods,
            DateTime startTime,
            TimeSpan duration,
            Dictionary<IDevice, Color> colorMap,
            DateTime progress)
        {
            if (methods != null && methods.Count > 0)
            {
                var top = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;
                top = Math.Min(top, CONST_HEADER_PADDING_MAX);

                //
                // Re-adjust the size to fit on the screen
                //
                var heightPer = (Convert.ToSingle(bounds.Height) - top) / Convert.ToSingle(methods.Count);
                if (heightPer > CONST_MAX_HEIGHT)
                    heightPer = CONST_MAX_HEIGHT;
                else if (heightPer < CONST_MIN_HEIGHT)
                    heightPer = CONST_MIN_HEIGHT;

                //
                // Find the start and end times of the samples
                //
                DateTime methodStart;
                TimeSpan methodDuration;
                FindTimeExtremas(methods, out methodStart, out methodDuration);

                //
                // Then calculate the number of pixels per second to use.
                //
                var pixelsPerSecond = Convert.ToSingle(bounds.Width - bounds.X) /
                                        Convert.ToSingle(methodDuration.TotalSeconds);

                //
                // Draw each method now
                //
                foreach (var method in methods)
                {
                    var methodBounds = new RectangleF(bounds.X, top, bounds.Width, heightPer);
                    RenderLCMethod(graphics,
                        methodBounds,
                        method,
                        startTime,
                        duration,
                        colorMap,
                        progress);

                    top += heightPer;
                }
            }

            RenderOptimizationsOnTimeline(graphics, bounds, startTime, duration, methods);

            // Draw a timeline
            RenderTimeline(graphics,
                bounds,
                startTime,
                duration);
        }

        /// <summary>
        /// Renders a list of samples to the graphics object provided.
        /// </summary>
        /// <param name="graphics">Object to render to.</param>
        /// <param name="bounds">Area to render in.</param>
        /// <param name="samples">Samples to render.</param>
        /// <param name="colorMap">Device to color mapping.</param>
        public virtual void RenderSamples(Graphics graphics,
            RectangleF bounds,
            List<classSampleData> samples,
            DateTime startTime,
            TimeSpan duration,
            Dictionary<IDevice, Color> colorMap,
            DateTime progress)
        {
            //
            // Gather the sample methods into a list, then call the
            // drawing method.
            //
            var methods = new List<classLCMethod>();
            foreach (var sample in samples)
            {
                if (sample != null)
                    methods.Add(sample.LCMethod);
            }

            RenderLCMethod(graphics,
                bounds,
                methods,
                startTime,
                duration,
                colorMap,
                progress);
        }

        #endregion
    }

    #region Comparison Classes for Sorting

    /// <summary>
    /// Compares two LC-methods based on their start times.
    /// </summary>
    public class classMethodStartTimeComparer : IComparer<classLCMethod>
    {
        #region IComparer<classSampleData> Members

        public int Compare(classLCMethod x, classLCMethod y)
        {
            return x.Start.CompareTo(y.Start);
        }

        #endregion
    }

    /// <summary>
    /// Compares two LC-methods based on their end times.
    /// </summary>
    public class classMethodEndTimeComparer : IComparer<classLCMethod>
    {
        #region IComparer<classSampleData> Members

        public int Compare(classLCMethod x, classLCMethod y)
        {
            return x.End.CompareTo(y.End);
        }

        #endregion
    }

    #endregion
}