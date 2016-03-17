using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;


namespace LcmsNet.Method.Drawing
{
    /// <summary>
    /// Class that renders samples as column format.
    /// </summary>
    public class classColumnSampleRenderer: classLCMethodRenderer
    {
        /// <summary>
        /// Constant defining the number of columns to use.
        /// </summary>
        private const int CONST_NUMBER_OF_COLUMNS = 4;

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
        public override void RenderSamples( Graphics graphics,
                                            RectangleF bounds,
                                            List<classSampleData> samples,
                                            DateTime startTime,
                                            TimeSpan duration,
                                            Dictionary<IDevice, Color> colorMap,
                                            DateTime progress)
        {
            ///
            /// Calculate formatting paddings.
            ///
            float heightPer = bounds.Height / Convert.ToSingle(CONST_NUMBER_OF_COLUMNS);
            heightPer       = Math.Max(CONST_MIN_HEIGHT, Math.Min(heightPer, CONST_MAX_HEIGHT));
            float offset    = Convert.ToSingle(bounds.Height) * CONST_HEADER_PADDING;

            ///
            /// Make sure we dont use any non-null-samples to find the date and time values.
            ///
            List<classLCMethod> methods = new List<classLCMethod>();
            foreach (classSampleData sample in samples)
            {
                if (sample != null && sample.LCMethod != null)
                    methods.Add(sample.LCMethod);
            }
            if (methods.Count < 1)
                return;

            ///
            /// Find the start and end times of the samples
            ///
            DateTime methodStart;
            TimeSpan methodDuration;
            FindTimeExtremas(methods, out methodStart, out methodDuration);

            ///
            /// Then calculate the number of pixels per second to use.
            ///
            float pixelsPerSecond = (Convert.ToSingle(bounds.Width) / Convert.ToSingle(methodDuration.TotalSeconds));

            ///
            /// Draw each method
            ///
            foreach (classSampleData sample in samples)
            {
                if (sample == null)
                    continue;

                float width, height, x, top;

                ///
                /// Calculate the number of pixels the method should start from based on its time value.
                ///
                x       = Convert.ToSingle(sample.LCMethod.Start.Subtract(methodStart).TotalSeconds)*pixelsPerSecond;
                width   = Convert.ToSingle(sample.LCMethod.Duration.TotalSeconds) * pixelsPerSecond;
                height  = heightPer;
                top     = offset + (heightPer * sample.ColumnData.ID);

                RectangleF methodBounds = new RectangleF(x, top, width, height);
                RenderLCMethod(graphics,
                               methodBounds,
                               sample.LCMethod,
                               startTime,
                               duration,
                               colorMap,
                               progress);
            }
        }
    }
}
