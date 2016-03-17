using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using LcmsNet.Method.Drawing;
using LcmsNet.Method.Forms;
using LcmsNetDataClasses.Method;
using LcmsNet.Configuration;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Control that handles rendering the method timeline.
    /// </summary>
    public partial class controlLCMethodTimeline : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public controlLCMethodTimeline()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();

            //
            // Create internal lists
            //
            mlist_methods = new List<classLCMethod>();
            mdict_deviceColorMappings = new Dictionary<IDevice, Color>();

            //
            // We map before we register our delegates with events to make sure devices that are built-in are
            // mapped.
            //
            try
            {
                RemapDevicesToColors();
                //
                // Register device additions and deletions so that we remap color information for display.
                //
                classDeviceManager.Manager.DeviceAdded +=
                    new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceAdded);
                classDeviceManager.Manager.DeviceRemoved +=
                    new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRemoved);
            }
            catch
            {
            }
            leftMouseDown = false;
            m_index = 0;
            unmoved = false;
            Resize += new EventHandler(controlLCMethodTimeline_Resize);
        }

        /// <summary>
        /// Gets or sets the type of rendering to perform.
        /// </summary>
        public enumLCMethodRenderMode RenderMode { get; set; }

        public int StartEventIndex
        {
            get { return m_index; }
            set
            {
                m_index = value;
                if (!unmoved)
                {
                    this.Refresh();
                }
            }
        }

        private void controlLCMethodTimeline_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                leftMouseDown = true;
                oldMouseLoc = e.Location;
                unmoved = true;
            }
        }

        private void controlLCMethodTimeline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && RenderMode == enumLCMethodRenderMode.Conversation)
            {
                leftMouseDown = false;
                if (buttonLocations[0].Contains(e.Location))
                {
                    unmoved = false;
                    if (0 <= StartEventIndex - 1)
                    {
                        StartEventIndex--;
                    }
                }
                else if (buttonLocations[1].Contains(e.Location))
                {
                    unmoved = false;
                    if (mlist_methods.Count > 0)
                    {
                        var maxIndex =
                            FluidicsSimulator.FluidicsSimulator.BuildEventList(mlist_methods, mlist_methods[0].Start)
                                .Count - 1;
                        if (StartEventIndex + 1 <= maxIndex)
                        {
                            StartEventIndex++;
                        }
                    }
                }
            }
        }

        private void controlLCMethodTimeline_MouseMove(object sender, MouseEventArgs e)
        {
            //if left mouse is held down and render mode is conversation...scroll this way.
            if (leftMouseDown && RenderMode == enumLCMethodRenderMode.Conversation)
            {
                int indexChange = e.Y - oldMouseLoc.Y;
                int eventHeight = 48; // from classLCMethodConversationRenderer
                if (mlist_methods.Count > 0)
                {
                    var maxIndex =
                        FluidicsSimulator.FluidicsSimulator.BuildEventList(mlist_methods, mlist_methods[0].Start).Count -
                        1;
                        //numer of lists of events that happen at the same time. This is the upper bound of the index we are trying to change and track.
                    if (indexChange <= -eventHeight)
                    {
                        if ((StartEventIndex + 1) <= maxIndex)
                        {
                            StartEventIndex += 1;
                        }
                    }
                    else if (indexChange >= eventHeight)
                    {
                        if (0 <= (StartEventIndex - 1))
                        {
                            StartEventIndex -= 1;
                        }
                    }
                    if (indexChange < -eventHeight || indexChange > eventHeight)
                    {
                        oldMouseLoc = e.Location;
                    }
                }
                unmoved = false;
            }
        }

        #region Members

        /// <summary>
        /// The list of methods to render.
        /// </summary>
        private List<classLCMethod> mlist_methods;

        /// <summary>
        /// Maps a device to a color.
        /// </summary>
        private Dictionary<IDevice, Color> mdict_deviceColorMappings;

        //member variables for scrolling
        private int m_index; //tracks index
        private bool leftMouseDown; //determines if left mouse button is down(held)
        private Point oldMouseLoc; //used for tracking mouse movement
        private bool unmoved; // used to determine if refresh is needed.
        private Rectangle[] buttonLocations; // used for scroll buttons on conversation view

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
            //
            // Clear the list so we can re-adjust the mappings
            //
            mdict_deviceColorMappings = classLCMethodRenderer.ConstructDeviceColorMap(classDeviceManager.Manager.Devices);
        }

        #endregion

        #region Rendering and Render Invoking

        /// <summary>
        /// Causes the timeline renderings to be re-drawn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void controlLCMethodTimeline_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="method"></param>
        public void RenderLCMethod(classLCMethod method)
        {
            mlist_methods.Clear();
            mlist_methods.Add(method);
            Invalidate();
        }

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="method"></param>
        public void RenderLCMethod(List<classLCMethod> methods)
        {
            mlist_methods.Clear();
            mlist_methods.AddRange(methods);
            Invalidate();
        }

        /// <summary>
        /// Renders the method as a timeline in the height specified.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            classLCMethodRenderer renderer = LCRendererFactory.GetRenderer(RenderMode);
            DateTime startTime = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            TimeSpan duration = new TimeSpan(0, 30, 0);
            foreach (classColumnData column in classCartConfiguration.Columns)
            {
                renderer.ColumnNames.Add(column.Name);
            }
            renderer.ColumnNames.Add("Special");
            if (mlist_methods != null && mlist_methods.Count > 0)
            {
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Map the colors appropiately if they havent already been
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (mdict_deviceColorMappings.Count < 1)
                    RemapDevicesToColors();

                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Find the duration of the total comparison
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                startTime = mlist_methods[0].Start;
                DateTime endTime = mlist_methods[mlist_methods.Count - 1].End;
                foreach (classLCMethod renderData in mlist_methods)
                {
                    TimeSpan span = renderData.End.Subtract(endTime);
                    if (span.TotalMilliseconds > 0)
                        endTime = renderData.End;
                }
                duration = endTime.Subtract(startTime);
            }
            if (RenderMode == enumLCMethodRenderMode.Conversation)
            {
                renderer.StartEventIndex = StartEventIndex;
            }
            Rectangle bounds = ClientRectangle;
            bounds.Width -= 10;
            bounds.X += 5;

            renderer.RenderLCMethod(e.Graphics,
                bounds,
                mlist_methods,
                startTime,
                duration,
                mdict_deviceColorMappings,
                DateTime.MaxValue);
            //Render scroll buttons
            if (RenderMode == enumLCMethodRenderMode.Conversation)
            {
                buttonLocations = (renderer as classLCMethodConversationRenderer).GetButtonLocations();
                Rectangle upButton = buttonLocations[0];
                Rectangle downButton = buttonLocations[1];

                //scroll up button rendering
                e.Graphics.FillRectangle(Brushes.LightGray, upButton);
                e.Graphics.DrawRectangle(Pens.LightSlateGray, upButton);
                //up-pointing arrow
                Point[] arrowPoints = new Point[3];
                arrowPoints[0] = new Point(upButton.X + (upButton.Width / 2), upButton.Y + upButton.Height / 3);
                arrowPoints[1] = new Point(upButton.X + (upButton.Width / 4), upButton.Y + 2 * (upButton.Height / 3));
                arrowPoints[2] = new Point(upButton.X + 3 * (upButton.Width / 4), upButton.Y + 2 * (upButton.Height / 3));
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPolygon(Brushes.Black, arrowPoints);

                //scroll down button rendering
                e.Graphics.FillRectangle(Brushes.LightGray, downButton);
                e.Graphics.DrawRectangle(Pens.LightSlateGray, downButton);
                //down-pointing arrow
                arrowPoints[0] = new Point(downButton.X + (downButton.Width / 2),
                    downButton.Y + 2 * (downButton.Height / 3));
                arrowPoints[1] = new Point(downButton.X + (downButton.Width / 4), downButton.Y + (downButton.Height / 3));
                arrowPoints[2] = new Point(downButton.X + 3 * (downButton.Width / 4),
                    downButton.Y + (downButton.Height / 3));
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPolygon(Brushes.Black, arrowPoints);
            }
        }

        #endregion
    }
}