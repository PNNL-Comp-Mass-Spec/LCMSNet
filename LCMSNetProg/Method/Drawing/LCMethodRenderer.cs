using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK;
using LcmsNet.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class for rendering LC Methods
    /// </summary>
    public abstract class LCMethodRenderer
    {
        /// <summary>
        /// Default constructor whose pixel padding is 0.0F
        /// </summary>
        protected LCMethodRenderer()
        {
            PixelPadding = 0.0F;
            TimelinePixelSpacing = 5.0F;
            TimelineDisplayDatesCount = 10;
            TimelineDrawRelativeTimes = true;
            AcquisitionColor = Brushes.Red;
            ColumnNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets the color to use when rendering the acquisition times.
        /// </summary>
        public SolidColorBrush AcquisitionColor { get; set; }

        // Convert value from '6pt'/'8pt' to the equivalent in pixels, since WPF uses pixels instead. See source for System.Windows.LengthConverter.
        protected readonly double SixPt = 8.0; // 6.0 * 4.0 / 3.0;
        protected readonly double EightPt = 10.667; // 8.0 * 4.0 / 3.0;

        #region Time Conversions

        /// <summary>
        /// Finds the start time and duration of the samples provided.
        /// </summary>
        /// <param name="methods">Samples to analyze</param>
        /// <param name="start">Starting time.</param>
        /// <param name="duration">Duration.</param>
        protected void FindTimeExtrema(List<LCMethod> methods, out DateTime start, out TimeSpan duration)
        {
            start = DateTime.MaxValue;
            var end = DateTime.MinValue;

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

            // Iterate through the device list to map a Device to a color
            var colors = new[]
            {
                Colors.Red,
                Colors.Lime,
                Colors.Yellow,
                Colors.Salmon,
                Colors.RoyalBlue,
                Colors.LightBlue,
                Colors.LightGreen,
                Colors.White,
                Colors.Thistle,
                Colors.PeachPuff,
                Colors.CadetBlue,
                Colors.PaleGreen
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
        protected const float CONST_HEADER_PADDING_MAX = 27.0F;
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
        /// Gets or sets the spacing in Pixels between two timeline tick marks
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
        public virtual void RenderAcquisitionTimes(DrawingContext g, Rect bounds, DateTime start, TimeSpan duration, List<LCEvent> events)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);
            var startPoint = PixelPadding;
            startPoint += Convert.ToSingle(duration.TotalSeconds * ppt);

            // This gives us the time between ticks as time units.
            var pixelTime = TimelinePixelSpacing * (1.0F / ppt);

            // Then we plot a number of ticks until the end - first here, we setup some variables
            // to help us in later calculations.
            var totalTime = Convert.ToSingle(duration.TotalSeconds);

            var brush = AcquisitionColor;
            var y = bounds.Height - 10.0F;
            var height = 15.0F;
            foreach (var lcEvent in events)
            {
                var span = lcEvent.Start.Subtract(start);

                var startX = Convert.ToSingle(span.TotalSeconds) * ppt;
                var length = Convert.ToSingle(lcEvent.HoldTime.TotalSeconds) * ppt;
                var x = bounds.X + startX;
                g.DrawRectangle(brush, null, new Rect(x, y, length, height));
            }
        }

        /// <summary>
        /// Renders a timeline at the bounds location on the graphics object between start and finish.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        public virtual void RenderTimeline(DrawingContext graphics, Rect bounds, DateTime start, TimeSpan duration)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);
            var startPoint = PixelPadding;
            startPoint += Convert.ToSingle(duration.TotalSeconds * ppt);

            // This gives us the time between ticks as time units.
            var pixelTime = TimelinePixelSpacing * (1.0F / ppt);

            // Then we plot a number of ticks until the end - first here, we setup some variables
            // to help us in later calculations.
            var currentTime = 0.0d;
            var totalTime = Convert.ToSingle(duration.TotalSeconds);

            // Here we figure out the heights of ticks and variables to use to draw them. Also draw the base tick line.
            var tickHeight = 10.0F;
            var y = bounds.Height - 5.0F;
            var timelineBrush = Brushes.Black;
            var pen = new Pen(timelineBrush, 1);
            graphics.DrawLine(pen, new Point(bounds.X, y), new Point(bounds.Width, y));

            // Now we need to figure out how many times to draw a time...
            // Here we see how many seconds should go by before drawing a date time.
            var pixelsPerDate = ((bounds.Width - bounds.X) / TimelineDisplayDatesCount);
            var dateFont = new Typeface("Microsoft Sans Serif");

            // Loop drawing the ticks...
            while (currentTime - totalTime <= 0 && pixelTime > 0)
            {
                // Now we have to convert the x value (time = currentTime) into pixels...err
                var x = bounds.X + (currentTime * ppt);

                // Draw a tick
                graphics.DrawLine(pen, new Point(x, y), new Point(x, y - tickHeight));
                currentTime += pixelTime;
            }
            graphics.DrawLine(pen, new Point(bounds.Width, y), new Point(bounds.Width, y - tickHeight));

            // Draw the date strings (times if relative flag set).
            var timePerDraw = Convert.ToSingle(duration.TotalSeconds) / TimelineDisplayDatesCount;
            currentTime = 0;
            var datePen = new Pen(Brushes.LightGray, 1);

            for (var i = 0; i <= TimelineDisplayDatesCount; i++)
            {
                var dateString = string.Format("{0:0}", currentTime);

                //if (TimelineDrawRelativeTimes == false)
                //    dateString = start.AddSeconds(currentTime).ToShortTimeString();


                var x = bounds.X + (pixelsPerDate * Convert.ToSingle(i));

                graphics.DrawLine(pen, new Point(x, y - tickHeight), new Point(x, 0));

                var startDateText = new FormattedText(dateString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, dateFont, SixPt, timelineBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                graphics.DrawText(startDateText, new Point(x - 3.0F, y - tickHeight * 2));

                dateString = start.AddSeconds(currentTime).ToLongTimeString();
                var endDateText = new FormattedText(dateString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, dateFont, SixPt, timelineBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                graphics.DrawText(endDateText, new Point(x - 3.0F, y - tickHeight * 3));
                currentTime += timePerDraw;
            }
        }

        /// <summary>
        /// Renders a timeline at the bounds location on the graphics object between start and finish.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        /// <param name="methods"></param>
        public virtual void RenderOptimizationsOnTimeline(DrawingContext graphics, Rect bounds, DateTime start, TimeSpan duration, List<LCMethod> methods)
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
            var optBrush = AcquisitionColor;
            var timelineBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
            var dateFont = new Typeface("Microsoft Sans Serif");
            foreach (var method in methods)
            {
                foreach (var lcEvent in method.Events)
                {
                    if (lcEvent.OptimizeWith)
                    {
                        var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;
                        var x = bounds.X + Convert.ToSingle(lcEvent.Start.Subtract(start).TotalSeconds) * ppt;
                        var alignedBounds = new Rect(x, y - 15.0F, eventWidth, 20.0F);
                        graphics.PushClip(new RectangleGeometry(alignedBounds));
                        graphics.DrawRectangle(optBrush, null, new Rect(x, y - 15.0F, eventWidth, 20.0F));
                        var endDateText = new FormattedText(optimize, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, dateFont, SixPt, timelineBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                        graphics.DrawText(endDateText, new Point(x, y));
                        graphics.Pop();
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
        public virtual void RenderLCEvent(DrawingContext graphics, Rect bounds, LCEvent lcEvent, Color backColor)
        {
            // Make sure that we don't have some garbage event that has a zero time width.
            if (bounds.Width <= 0)
                return;

            // Construct a gradient brush so that we can see the text better
            Brush brush = new LinearGradientBrush(Colors.White, backColor, 0);

            // Then fill the background of the event
            graphics.DrawRectangle(brush, null, bounds);

            // If the lcEvent is currently executing, outline it in green, otherwise outline it in black.
            var outlineBrush = lcEvent.MethodData.Executing ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);

            // Render the name of the device
            var nameFont = new Typeface(new FontFamily("Microsoft Sans Serif"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

            Brush nameBrush = Brushes.Black;
            var name = lcEvent.Name;

            if (bounds.Height >= CONST_MID_HEIGHT)
            {
                var deviceNameText = new FormattedText(lcEvent.Device.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, EightPt, nameBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                graphics.DrawText(deviceNameText, bounds.Location);

                // Render the name of the event
                var nameText = new FormattedText(name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, EightPt, nameBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                nameText.MaxTextWidth = bounds.Width;
                nameText.MaxTextHeight = bounds.Height;
                graphics.DrawText(nameText, new Point(bounds.X, bounds.Y + (bounds.Height / 2.0F)));

                // Render the params of the event
                if (lcEvent.Parameters.Length > 0 && lcEvent.Parameters[0] != null)
                {
                    var parametersText = new FormattedText(lcEvent.Parameters[0].ToString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, nameFont, EightPt, new SolidColorBrush(Color.FromRgb(80, 80, 80)), FluidicsModerator.Moderator.DrawingScaleFactor);
                    graphics.DrawText(parametersText, new Point(bounds.X, bounds.Y + (bounds.Height / 2.0F) + nameText.Height));
                }
            }

            // Finally draw the outline
            var outlinePen = new Pen(outlineBrush, 1);
            if (lcEvent.MethodData.Executing)
            {
                outlinePen.Thickness = 4;
            }
            graphics.DrawRectangle(null, outlinePen, bounds);
        }

        /// <summary>
        /// Renders a list of events to the graphics object
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="allBounds"></param>
        /// <param name="lcEvent"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        public virtual void RenderLCEventError(DrawingContext graphics, Rect allBounds, LCEvent lcEvent, DateTime startTime, TimeSpan duration)
        {
            // Pixels per time (PPT)
            var ppt = (allBounds.Width - allBounds.X) / Convert.ToSingle(duration.TotalSeconds);

            var startPoint = allBounds.X + Convert.ToSingle((lcEvent.Start.Subtract(startTime)).TotalSeconds * ppt);
            var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;
            var x = startPoint;
            var y = allBounds.Y;

            var triHeight = CONST_TRIANGLE_HEIGHT;
            var triWidth = triHeight / 2.0F;

            var eventBounds = new Rect(x - triWidth, y, eventWidth, allBounds.Height * .8F);

            graphics.PushClip(new RectangleGeometry(eventBounds));

            var figure = new PathFigure(new Point(startPoint, y + triHeight), new[]
            {
                new LineSegment(new Point(startPoint + triWidth, y), true),
                new LineSegment(new Point(startPoint - triWidth, y), true),
            }, true);
            var geometry = new PathGeometry(new [] { figure });

            // Black Outline
            var pen = new Pen(Brushes.Black, 2.0F);

            // Red Fill
            var brush = Brushes.Red;
            graphics.DrawGeometry(brush, pen, geometry);

            graphics.Pop();
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
        public virtual void RenderLCEvent(DrawingContext graphics, Rect allBounds, List<LCEvent> events, DateTime startTime, TimeSpan duration, Dictionary<IDevice, Color> colorMap)
        {
            if (events == null || events.Count == 0)
            {
                return;
            }

            var bounds = allBounds;
            bounds.Height = allBounds.Height * .80F;

            // Figure out how many pixels are in a second
            //     ppt breaks the naming convention, however, we are using the
            //     standard "parts per" or "pixels per" time here.
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);

            // Identify the starting pixel using any padding for pixels
            var startPoint = bounds.X;
            startPoint += Convert.ToSingle((events[0].Start.Subtract(startTime)).TotalSeconds * ppt);

            // For every event render a rectangle.
            foreach (var lcEvent in events)
            {
                // Calculate how wide the event will be
                var eventWidth = Convert.ToSingle(lcEvent.Duration.TotalSeconds) * ppt;

                // Define start x,y position for event block
                var x = startPoint;
                var y = bounds.Y;

                var eventBounds = new Rect(x, y, eventWidth, bounds.Height);

                // Find the color mapping, if it doesn't exist
                //     then we assign a gradient gray color to it
                // We also may want to show progress in the event...so here we only fill if it's the current event
                var color = Colors.LightGray;
                if (colorMap.ContainsKey(lcEvent.Device))
                    color = colorMap[lcEvent.Device];


                if (lcEvent.OptimizeWith)
                {
                    var alignedBounds = new Rect(x, y - 15.0F, eventWidth, bounds.Height + 20.0F);
                    graphics.PushClip(new RectangleGeometry(alignedBounds));
                    var optBrush = AcquisitionColor;
                    graphics.DrawRectangle(optBrush, null, new Rect(x, y - 15.0F, eventWidth, bounds.Height + 20.0F));
                    graphics.Pop();
                }
                graphics.PushClip(new RectangleGeometry(eventBounds));
                RenderLCEvent(graphics, eventBounds, lcEvent, color);
                graphics.Pop();
                startPoint += eventWidth;
            }
        }

        /// <summary>
        /// Renders the LC Method provided to the graphics object.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        /// <param name="method"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap"></param>
        /// <param name="progress"></param>
        public virtual void RenderLCMethod(DrawingContext graphics, Rect bounds, LCMethod method, DateTime startTime, TimeSpan duration, Dictionary<IDevice, Color> colorMap, DateTime progress)
        {
            var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(duration.TotalSeconds);

            // Find the first pixel
            var startPoint = bounds.X;
            startPoint += Convert.ToSingle((method.Start.Subtract(startTime)).TotalSeconds * ppt);

            // Find out the last pixel.
            var endPoint = bounds.X + Convert.ToSingle((method.End.Subtract(startTime)).TotalSeconds * ppt);

            if (method.IsSpecialMethod)
            {
                var specialColor = Colors.Orange;
                specialColor.A = 32;
                var newBounds = new Rect(startPoint - 5, bounds.Top - 5, endPoint, bounds.Height);
                graphics.DrawRectangle(new SolidColorBrush(specialColor), null, newBounds);
                var specialName = "special";
                var font = new Typeface(new FontFamily("Microsoft Sans Serif"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
                specialColor = Colors.Black;
                specialColor.A = 70;
                var specialBrush = new SolidColorBrush(specialColor);
                var specialNameText = new FormattedText(specialName, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, EightPt, specialBrush, FluidicsModerator.Moderator.DrawingScaleFactor);
                graphics.DrawText(specialNameText, new Point(startPoint, bounds.Top + specialNameText.Height));
            }

            // Event details
            RenderLCEvent(graphics, bounds, method.Events, startTime, duration, colorMap);

            // Name of the method
            var textFont = new Typeface(new FontFamily("Microsoft Sans Serif"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            Brush brush = Brushes.Black;

            var methodNameText = new FormattedText(method.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, textFont, EightPt, brush, FluidicsModerator.Moderator.DrawingScaleFactor);
            graphics.DrawText(methodNameText, new Point(startPoint, bounds.Y - 13));

            // Draw the duration strings for the method
            var span = method.Duration;
            var durationString = span.ToString();

            var methodStartText = new FormattedText(method.Start.ToLongTimeString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, textFont, SixPt, brush, FluidicsModerator.Moderator.DrawingScaleFactor);
            var methodDurationText = new FormattedText(durationString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, textFont, SixPt, brush, FluidicsModerator.Moderator.DrawingScaleFactor);
            var methodEndText = new FormattedText(method.End.ToLongTimeString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, textFont, SixPt, brush, FluidicsModerator.Moderator.DrawingScaleFactor);

            var x = startPoint + methodNameText.Width + 15.0F;

            if ((x + methodStartText.Width) < endPoint)
            {
                graphics.DrawText(methodStartText, new Point(x, bounds.Y - 13));
            }

            x += methodStartText.Width + 15.0F;
            if ((x + methodDurationText.Width) < endPoint)
            {
                graphics.DrawText(methodDurationText, new Point(x, bounds.Y - 13));
            }
            x += methodDurationText.Width + 15.0F;

            if ((x + methodEndText.Width) <= endPoint)
            {
                graphics.DrawText(methodEndText, new Point(endPoint - methodDurationText.Width, bounds.Y - 13));
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
        public virtual void RenderLCMethod(DrawingContext graphics, Rect bounds, List<LCMethod> methods, DateTime startTime, TimeSpan duration, Dictionary<IDevice, Color> colorMap, DateTime progress)
        {
            if (methods != null && methods.Count > 0)
            {
                var top = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;
                top = Math.Min(top, CONST_HEADER_PADDING_MAX);

                // Re-adjust the size to fit on the screen
                var heightPer = (Convert.ToSingle(bounds.Height) - top) / Convert.ToSingle(methods.Count);
                if (heightPer > CONST_MAX_HEIGHT)
                    heightPer = CONST_MAX_HEIGHT;
                else if (heightPer < CONST_MIN_HEIGHT)
                    heightPer = CONST_MIN_HEIGHT;

                // Find the start and end times of the samples
                DateTime methodStart;
                TimeSpan methodDuration;
                FindTimeExtrema(methods, out methodStart, out methodDuration);

                // Then calculate the number of pixels per second to use.
                var pixelsPerSecond = Convert.ToSingle(bounds.Width - bounds.X) /
                                      Convert.ToSingle(methodDuration.TotalSeconds);

                // Draw each method now
                foreach (var method in methods)
                {
                    var methodBounds = new Rect(bounds.X, top, bounds.Width, heightPer);
                    RenderLCMethod(graphics, methodBounds, method, startTime, duration, colorMap, progress);

                    top += heightPer;
                }
            }

            RenderOptimizationsOnTimeline(graphics, bounds, startTime, duration, methods);

            // Draw a timeline
            RenderTimeline(graphics, bounds, startTime, duration);
        }

        /// <summary>
        /// Renders a list of samples to the graphics object provided.
        /// </summary>
        /// <param name="graphics">Object to render to.</param>
        /// <param name="bounds">Area to render in.</param>
        /// <param name="samples">Samples to render.</param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="colorMap">Device to color mapping.</param>
        /// <param name="progress"></param>
        public virtual void RenderSamples(DrawingContext graphics, Rect bounds, List<SampleData> samples, DateTime startTime, TimeSpan duration, Dictionary<IDevice, Color> colorMap, DateTime progress)
        {
            // Gather the sample methods into a list, then call the
            // drawing method.
            var methods = new List<LCMethod>();
            foreach (var sample in samples)
            {
                if (sample != null)
                    methods.Add(sample.ActualLCMethod);
            }

            RenderLCMethod(graphics, bounds, methods, startTime, duration, colorMap, progress);
        }

        #endregion
    }
}
