using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using LcmsNet.Method.Drawing;

using LcmsNetDataClasses.Configuration;
using LcmsNet.Configuration;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Control that renders the four columns showing sample progress.
    /// </summary>
    public partial class controlSampleProgress : UserControl
    {
        #region Constants
        /// <summary>
        /// Total Number of columns.
        /// </summary>
        private const int CONST_TOTAL_COLUMNS = 4;
        private const int CONST_PREVIEW_MINUTES = 30;
        private const int CONST_PREVIEW_SECONDS = 1;
        private const int CONST_PREVIEW_MILLISECONDS = 0;
        #endregion

        #region Members
        /// <summary>
        /// List of samples that are run on columns.  
        /// </summary>
        private List<classSampleData>      mlist_samples;
        /// <summary>
        /// Dictionary to map a device to a color for visual integrity
        /// </summary>
        private Dictionary<IDevice, Color> mdict_deviceColorMappings;
        /// <summary>
        /// Defines how to refresh the graphics.
        /// </summary>
        private delegate void DelegateUpdateGraphics();
		/// <summary>
		/// Maintains a list of errors that happened on the column.
		/// </summary>
		private Dictionary<int, List<LcmsNetDataClasses.Method.classLCEvent>> mdict_errors;
        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlSampleProgress()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();

            /// By default show N minutes
            PreviewMinutes      = CONST_PREVIEW_MINUTES;
            PreviewMilliseconds = CONST_PREVIEW_MILLISECONDS;
            PreviewSeconds      = CONST_PREVIEW_SECONDS;

            /// 
            /// Create internal lists
            /// 
            mlist_samples = new List<classSampleData>();
            
            
            mdict_deviceColorMappings   = new Dictionary<IDevice, Color>();

			// Maintain a list of errors.
			mdict_errors = new Dictionary<int, List<classLCEvent>>();
			for(int i = 0; i < CONST_TOTAL_COLUMNS + 1 ; i++)
			{
				mdict_errors.Add(i, new List<classLCEvent>());
                mlist_samples.Add(null);
			}

            
            // Synch to the device manager so we can construct device-to-color mappings.            
            try
            {
                if (classDeviceManager.Manager != null)
                {
                    /// 
                    /// Maps device colors from device manager.
                    /// 
                    RemapDevicesToColors();

                    /// 
                    /// Register device additions and deletions so that we remap color information for display.
                    /// 
                    classDeviceManager.Manager.DeviceAdded += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceAdded);
                    classDeviceManager.Manager.DeviceRemoved += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRemoved);
                }
            }
            catch
            {
            }

            /// 
            /// Catch the resize event so we can always update our display.
            /// 
            Resize += new EventHandler(controlLCMethodTimeline_Resize);
        }
        /// <summary>
        /// Invalidate forces the OnPaint method to be called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void controlLCMethodTimeline_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

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
            /// 
            /// Clear the list so we can re-adjust the mappings
            /// 
            mdict_deviceColorMappings = classLCMethodRenderer.ConstructDeviceColorMap(classDeviceManager.Manager.Devices);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the total time in minutes to show a preview of.
        /// </summary>
        public int  PreviewMinutes { get; set; }
        /// <summary>
        /// Gets or sets the total number of seconds to preview.
        /// </summary>
        public int PreviewSeconds { get; set; }
        /// <summary>
        /// Gets or sets the total number of milliseconds to preview.
        /// </summary>
        public int PreviewMilliseconds { get; set; }
        /// <summary>
        /// Gets or sets whether to show the full sample time or just from the current Date Time until PreviewMinutes or RenderAllEvents
        /// </summary>
        public bool RenderCurrent { get; set; }
        /// <summary>
        /// Gets or sets whether to draw PreviewMinutes from the start time or whether to draw all events to completion.
        /// </summary>
        public bool RenderAllEvents { get; set; }
        /// <summary>
        /// Gets or sets whether to render a display overlay window.
        /// </summary>
        public bool RenderDisplayWindow { get; set; }
        #endregion

		/// <summary>
		/// Renders the method provided.
		/// </summary>
		/// <param name="method"></param>
		public void UpdateSample(classSampleData sample)
		{
            int columnID = sample.ColumnData.ID;
            if (sample.LCMethod.IsSpecialMethod == true)
            {
                columnID = CONST_TOTAL_COLUMNS;
            }

			if (mlist_samples.Contains(sample) == false)
				mlist_samples[columnID] = sample;

			if (InvokeRequired == true)
				this.BeginInvoke(new DelegateUpdateGraphics(DelegateRefresh));
			else
				Refresh();
		}
		/// <summary>
		/// Renders the method provided.
		/// </summary>
		/// <param name="method"></param>
		public void UpdateError(classSampleData sample, classLCEvent lcEvent)
		{
			if (sample != null)
			{
                int columnID = sample.ColumnData.ID;
                if (sample.LCMethod.IsSpecialMethod == true)
                {
                    columnID = CONST_TOTAL_COLUMNS;
                }

				mlist_samples[columnID] = sample;
                if (lcEvent != null)
                {
                    mdict_errors[columnID].Add(lcEvent);
                }
			}

			if (InvokeRequired == true)
				this.BeginInvoke(new DelegateUpdateGraphics(DelegateRefresh));
			else
				Refresh();
		}
		/// <summary>
		/// Clears the sample list.
		/// </summary>
		public void ClearSamples()
		{
			for (int i = 0; i < mlist_samples.Count; i++)
			{
				mlist_samples[i] = null;
				foreach (int key in mdict_errors.Keys)
				{
					mdict_errors[key].Clear();
				}
			}
		}
		/// <summary>
		/// Updates the display.
		/// </summary>
		private void DelegateRefresh()
        {
			Refresh();

			if (InvokeRequired == true)
				this.BeginInvoke(new DelegateUpdateGraphics(DelegateRefresh));
			else
				Refresh();
        }

        /// <summary>
        /// Renders the method as a timeline in the height specified.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RenderGraph(e.Graphics);
        }

        public void RenderGraph(Graphics e)
        {
            DateTime now = LcmsNetSDK.TimeKeeper.Instance.Now;// DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            if (mlist_samples != null && mlist_samples.Count > 0)
            {
                try
                {
                    RectangleF bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);
                    classLCMethodColumnModeRenderer renderer = new classLCMethodColumnModeRenderer();
                    foreach (classColumnData column in classCartConfiguration.Columns)
                    {
                        renderer.ColumnNames.Add(column.Name);
                    }
                    renderer.ColumnNames.Add("Special");

                    DateTime minTime = DateTime.MaxValue;
                    DateTime maxTime = DateTime.MinValue;

                    /// 
                    /// Find the minimum and maximum time values.
                    /// 
                    foreach (classSampleData data in mlist_samples)
                    {
                        if (data != null)
                        {
                            if (minTime.CompareTo(data.LCMethod.Start) > 0)
                                minTime = data.LCMethod.Start;
                            if (maxTime.CompareTo(data.LCMethod.End) < 0)
                                maxTime = data.LCMethod.End;
                        }
                    }

                    /// 
                    /// Then ask, if we only want to see from now until the end...only 
                    /// allow DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)) to be shown.
                    /// 
                    if (RenderCurrent == true)
                    {
                        minTime = now;
                    }
                    else if (minTime.Equals(DateTime.MaxValue))
                    {
                        minTime = now;
                    }

                    TimeSpan span = default(TimeSpan);
                    /// 
                    /// Now we see how long to render for...if rendering all events then we are
                    /// using the time between the start time until the last event
                    /// 
                    if (RenderAllEvents == true)
                    {
                        span = maxTime.Subtract(minTime);

                        /// 
                        /// But make sure we arent rendering too little time.
                        /// 
                        if (span.TotalMinutes < 0)
                            span = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, PreviewMilliseconds);
                    }
                    else
                    {
                        span = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, PreviewMilliseconds);
                    }

                    /// 
                    /// now render!
                    /// 
                    renderer.RenderSamples(e,
                                             bounds,
                                             mlist_samples,
                                             minTime,
                                             span,
                                             mdict_deviceColorMappings,
                                             DateTime.MaxValue);

                    if (RenderDisplayWindow == true)
                    {
                        float x1Seconds = Convert.ToSingle(now.Subtract(minTime).TotalSeconds);
                        TimeSpan spanx = new TimeSpan(0, 0, PreviewMinutes, PreviewSeconds, PreviewMilliseconds);
                        float x2Seconds = Convert.ToSingle(now.Add(spanx).Subtract(minTime).TotalSeconds);
                        float ppt = (bounds.Width - bounds.X) / Convert.ToSingle(span.TotalSeconds);

                        float xx = ppt * x1Seconds;
                        float xw = ppt * x2Seconds - xx;

                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            using (Pen pen = new Pen(brush))
                            {
                                e.DrawLine(pen,
                                                    new PointF(xx, 0),
                                                    new PointF(xx, bounds.Height));

                                e.DrawLine(pen,
                                                    new PointF(ppt * x2Seconds, 0),
                                                    new PointF(Math.Min(ppt * x2Seconds, bounds.Width - 1), bounds.Height));

                            }
                        }
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(10, 255, 0, 0)))
                        {
                            e.FillRectangle(brush, xx, 0, xw, bounds.Height);
                        }
                    }



                    foreach (int key in mdict_errors.Keys)
                    {

                        List<LcmsNetDataClasses.Method.classLCEvent> oldEvents = new List<classLCEvent>();
                        List<LcmsNetDataClasses.Method.classLCEvent> lcEvents = mdict_errors[key];
                        foreach (LcmsNetDataClasses.Method.classLCEvent lcEvent in lcEvents)
                        {
                            if (lcEvent.Start.Subtract(minTime).TotalMilliseconds <= 0)
                            {
                                oldEvents.Add(lcEvent);
                            }
                            else
                            {
                                renderer.RenderLCEventError(e,
                                                            bounds,
                                                            lcEvent,
                                                            minTime,
                                                            span);
                            }
                        }
                        foreach (LcmsNetDataClasses.Method.classLCEvent lcEvent in oldEvents)
                        {
                            lcEvents.Remove(lcEvent);
                        }
                    }
                }
                catch
                {
                    // We dont care about the exceptions if it's drawing...we just want it to keep rendering!
                }
            }
            else
            {
                classLCMethodColumnModeRenderer renderer = new classLCMethodColumnModeRenderer();
                foreach (classColumnData column in classCartConfiguration.Columns)
                {
                    renderer.ColumnNames.Add(column.Name);
                }
                renderer.ColumnNames.Add("Special");
                renderer.RenderSamples(e,
                                         Bounds,
                                         null,
                                         now,
                                         new TimeSpan(1, 0, 0),
                                         mdict_deviceColorMappings,
                                         DateTime.MaxValue);
            }
        }
    }
}
