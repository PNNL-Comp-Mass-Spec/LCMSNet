using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LcmsNet.Method.Drawing;
using LcmsNetData;
using LcmsNetData.System;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class SampleProgressViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SampleProgressViewModel()
        {
            // By default show N minutes
            PreviewMinutes = CONST_PREVIEW_MINUTES;
            PreviewSeconds = CONST_PREVIEW_SECONDS;

            // Create internal lists
            samples = new List<SampleData>();

            deviceColorMappings = new Dictionary<IDevice, Color>();

            // Maintain a list of errors.
            errors = new Dictionary<int, List<LCEvent>>();
            for (var i = 0; i < CONST_TOTAL_COLUMNS + 1; i++)
            {
                errors.Add(i, new List<LCEvent>());
                samples.Add(null);
            }

            // Synch to the device manager so we can construct device-to-color mappings.
            try
            {
                if (DeviceManager.Manager != null)
                {
                    // Maps device colors from device manager.
                    RemapDevicesToColors();

                    // Register device additions and deletions so that we remap color information for display.
                    DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
                    DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
                }
            }
            catch
            {
                // Ignore errors here
            }
        }

        /// <summary>
        /// Use the given sample to clear out displayed progress and errors for the column
        /// </summary>
        /// <param name="sample"></param>
        public void ResetColumn(SampleData sample)
        {
            if (sample == null)
            {
                return;
            }

            if (samples.Count(x => x != null) <= 1)
            {
                ClearSamples();
                return;
            }

            var columnID = sample.ColumnData.ID;
            if (sample.ActualLCMethod.IsSpecialMethod)
            {
                columnID = CONST_TOTAL_COLUMNS;
            }

            samples[columnID] = null;
            errors[columnID].Clear();
        }

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="sample"></param>
        public void UpdateSample(SampleData sample)
        {
            if (sample == null)
            {
                return;
            }

            var columnID = sample.ColumnData.ID;
            if (sample.ActualLCMethod.IsSpecialMethod)
            {
                columnID = CONST_TOTAL_COLUMNS;
            }

            if (samples.Contains(sample) == false)
                samples[columnID] = sample;

            Refresh();
        }

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="lcEvent"></param>
        public void UpdateError(SampleData sample, LCEvent lcEvent)
        {
            if (sample != null)
            {
                var columnID = sample.ColumnData.ID;
                if (sample.ActualLCMethod.IsSpecialMethod)
                {
                    columnID = CONST_TOTAL_COLUMNS;
                }

                samples[columnID] = sample;
                if (lcEvent != null)
                {
                    errors[columnID].Add(lcEvent);
                }
            }

            Refresh();
        }

        /// <summary>
        /// Clears the sample list.
        /// </summary>
        public void ClearSamples()
        {
            for (var i = 0; i < samples.Count; i++)
            {
                samples[i] = null;
            }

            foreach (var errorCollection in errors.Values)
            {
                errorCollection.Clear();
            }

            Refresh();
        }

        public void Refresh()
        {
            RefreshView?.Invoke(this, EventArgs.Empty);
        }

        public void RenderGraph(DrawingContext e, Rect bounds)
        {
            var now = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            if (samples != null && samples.Count > 0)
            {
                try
                {
                    var renderer = new LCMethodColumnModeRenderer();
                    foreach (var column in CartConfiguration.Columns)
                    {
                        if (column.Status != ColumnStatus.Disabled)
                        {
                            renderer.ColumnNames.Add(column.Name);
                        }
                    }

                    if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true))
                    {
                        // Also show the "Special" column
                        renderer.ColumnNames.Add("Special");
                    }

                    var minTime = DateTime.MaxValue;
                    var maxTime = DateTime.MinValue;

                    // Find the minimum and maximum time values.
                    foreach (var data in samples)
                    {
                        if (data != null)
                        {
                            if (minTime.CompareTo(data.ActualLCMethod.Start) > 0)
                                minTime = data.ActualLCMethod.Start;
                            if (maxTime.CompareTo(data.ActualLCMethod.End) < 0)
                                maxTime = data.ActualLCMethod.End;
                        }
                    }

                    // Then ask, if we only want to see from now until the end...only
                    // allow DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)) to be shown.
                    if (RenderCurrent)
                    {
                        minTime = now;
                    }
                    else if (minTime.Equals(DateTime.MaxValue))
                    {
                        minTime = now;
                    }

                    TimeSpan span;

                    // Now we see how long to render for...if rendering all events then we are
                    // using the time between the start time until the last event
                    if (RenderAllEvents)
                    {
                        span = maxTime.Subtract(minTime);

                        // But make sure we aren't rendering too little time.
                        if (span.TotalMinutes < 0)
                            span = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, 0);
                    }
                    else
                    {
                        span = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, 0);
                    }

                    // now render!
                    renderer.RenderSamples(e, bounds, samples, minTime, span, deviceColorMappings, DateTime.MaxValue);

                    if (RenderDisplayWindow)
                    {
                        var x1Seconds = Convert.ToSingle(now.Subtract(minTime).TotalSeconds);
                        var spanx = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, 0);
                        var x2Seconds = Convert.ToSingle(now.Add(spanx).Subtract(minTime).TotalSeconds);
                        var ppt = (bounds.Width - bounds.X) / Convert.ToSingle(span.TotalSeconds);

                        var xx = ppt * x1Seconds;
                        var xw = ppt * x2Seconds - xx;

                        var pen = new Pen(Brushes.Black, 1);
                        e.DrawLine(pen, new Point(xx, 0), new Point(xx, bounds.Height));
                        e.DrawLine(pen, new Point(ppt * x2Seconds, 0), new Point(Math.Min(ppt * x2Seconds, bounds.Width - 1), bounds.Height));

                        var brush = new SolidColorBrush(Color.FromArgb(10, 255, 0, 0));
                        e.DrawRectangle(brush, null, new Rect(xx, 0, xw, bounds.Height));
                    }

                    foreach (var key in errors.Keys)
                    {
                        var oldEvents = new List<LCEvent>();
                        var lcEvents = errors[key];
                        foreach (var lcEvent in lcEvents)
                        {
                            if (lcEvent.Start.Subtract(minTime).TotalMilliseconds <= 0)
                            {
                                oldEvents.Add(lcEvent);
                            }
                            else
                            {
                                renderer.RenderLCEventError(e, bounds, lcEvent, minTime, span);
                            }
                        }
                        foreach (var lcEvent in oldEvents)
                        {
                            lcEvents.Remove(lcEvent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // We don't care about the exceptions if it's drawing...we just want it to keep rendering!
                    System.Console.WriteLine(ex);
                }
            }
            else
            {
                var renderer = new LCMethodColumnModeRenderer();
                foreach (var column in CartConfiguration.Columns)
                {
                    if (column.Status != ColumnStatus.Disabled)
                    {
                        renderer.ColumnNames.Add(column.Name);
                    }
                }

                if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true))
                {
                    // Also show the "Special" column
                    renderer.ColumnNames.Add("Special");
                }

                renderer.RenderSamples(e, bounds, null, now, new TimeSpan(1, 0, 0), deviceColorMappings, DateTime.MaxValue);
            }
        }

        #region Constants

        /// <summary>
        /// Total Number of columns.
        /// </summary>
        private const int CONST_TOTAL_COLUMNS = 4;

        private const int CONST_PREVIEW_MINUTES = 30;
        private const int CONST_PREVIEW_SECONDS = 1;

        #endregion

        #region Device-Color Mapping Methods

        /// <summary>
        /// Remaps the devices to colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            RemapDevicesToColors();
        }

        /// <summary>
        /// Remaps the devices to colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            RemapDevicesToColors();
        }

        /// <summary>
        /// Maps each device to a list of colors
        /// </summary>
        private void RemapDevicesToColors()
        {
            // Clear the list so we can re-adjust the mappings
            deviceColorMappings = LCMethodRenderer.ConstructDeviceColorMap(DeviceManager.Manager.Devices);
        }

        #endregion

        public event EventHandler<EventArgs> RefreshView;

        #region Members

        private int previewMinutes = CONST_PREVIEW_MINUTES;
        private int previewSeconds = CONST_PREVIEW_SECONDS;
        private bool renderCurrent = false;
        private bool renderAllEvents = false;
        private bool renderWindow = false;

        /// <summary>
        /// List of samples that are run on columns.
        /// </summary>
        private readonly List<SampleData> samples;

        /// <summary>
        /// Dictionary to map a device to a color for visual integrity
        /// </summary>
        private Dictionary<IDevice, Color> deviceColorMappings;

        /// <summary>
        /// Maintains a list of errors that happened on the column.
        /// </summary>
        private readonly Dictionary<int, List<LCEvent>> errors;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the total time in minutes to show a preview of.
        /// </summary>
        public int PreviewMinutes
        {
            get => previewMinutes;
            set => this.RaiseAndSetIfChanged(ref previewMinutes, value);
        }

        /// <summary>
        /// Gets or sets the total number of seconds to preview.
        /// </summary>
        public int PreviewSeconds
        {
            get => previewSeconds;
            set => this.RaiseAndSetIfChanged(ref previewSeconds, value);
        }

        /// <summary>
        /// Gets or sets whether to show the full sample time or just from the current Date Time until PreviewMinutes or RenderAllEvents
        /// </summary>
        public bool RenderCurrent
        {
            get => renderCurrent;
            set => this.RaiseAndSetIfChanged(ref renderCurrent, value);
        }

        /// <summary>
        /// Gets or sets whether to draw PreviewMinutes from the start time or whether to draw all events to completion.
        /// </summary>
        public bool RenderAllEvents
        {
            get => renderAllEvents;
            set => this.RaiseAndSetIfChanged(ref renderAllEvents, value);
        }

        /// <summary>
        /// Gets or sets whether to render a display overlay window.
        /// </summary>
        public bool RenderDisplayWindow
        {
            get => renderWindow;
            set => this.RaiseAndSetIfChanged(ref renderWindow, value);
        }

        #endregion

        public void UpdatePreviewTime(int minutes, int seconds)
        {
            PreviewMinutes = minutes;
            PreviewSeconds = seconds;
        }
    }
}
