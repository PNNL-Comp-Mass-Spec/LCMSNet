using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LcmsNet.Method.Drawing;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// Control that handles rendering the method timeline.
    /// </summary>
    public class LCMethodTimelineViewModel : ReactiveObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LCMethodTimelineViewModel()
        {
            // Create internal lists
            methods = new List<LCMethod>();
            deviceColorMappings = new Dictionary<IDevice, Color>();

            // We map before we register our delegates with events to make sure devices that are built-in are
            // mapped.
            try
            {
                RemapDevicesToColors();
                // Register device additions and deletions so that we remap color information for display.
                DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
                DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
            }
            catch
            {
                // ignored
            }
            index = 0;

            this.WhenAnyValue(x => x.RenderMode).Subscribe(x => this.Refresh());
        }

        private LCMethodRenderMode renderMode;

        /// <summary>
        /// Gets or sets the type of rendering to perform.
        /// </summary>
        public LCMethodRenderMode RenderMode
        {
            get => renderMode;
            set => this.RaiseAndSetIfChanged(ref renderMode, value);
        }

        public int StartEventIndex
        {
            get => index;
            set => this.RaiseAndSetIfChanged(ref index, value);
        }

        public event EventHandler<EventArgs> RefreshView;

        public void MouseUpUpdates(Point mouseLocation, ref bool mouseNotMoved)
        {
            if (RenderMode != LCMethodRenderMode.Conversation)
            {
                return;
            }
            if (buttonLocations[0].Contains(mouseLocation))
            {
                if (0 <= StartEventIndex - 1)
                {
                    StartEventIndex--;
                }
                mouseNotMoved = false;
            }
            else if (buttonLocations[1].Contains(mouseLocation))
            {
                if (methods.Count > 0)
                {
                    var maxIndex = FluidicsSimulator.FluidicsSimulator.BuildEventList(methods, methods[0].Start).Count - 1;
                    if (StartEventIndex + 1 <= maxIndex)
                    {
                        StartEventIndex++;
                    }
                }
                mouseNotMoved = false;
            }
        }

        public void MouseMovedUpdates(Point mouseLocation, ref Point oldMouseLocation)
        {
            if (RenderMode != LCMethodRenderMode.Conversation)
            {
                return;
            }
            var indexChange = mouseLocation.Y - oldMouseLocation.Y;
            var eventHeight = 48; // from LCMethodConversationRenderer
            if (methods.Count > 0)
            {
                var maxIndex = FluidicsSimulator.FluidicsSimulator.BuildEventList(methods, methods[0].Start).Count - 1;
                //number of lists of events that happen at the same time. This is the upper bound of the index we are trying to change and track.
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
                    oldMouseLocation = mouseLocation;
                }
            }
        }

        /// <summary>
        /// The list of methods to render.
        /// </summary>
        private readonly List<LCMethod> methods;

        /// <summary>
        /// Maps a device to a color.
        /// </summary>
        private Dictionary<IDevice, Color> deviceColorMappings;

        //member variables for scrolling
        private int index; //tracks index
        private Rect[] buttonLocations; // used for scroll buttons on conversation view

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

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="method"></param>
        public void RenderLCMethod(LCMethod method)
        {
            methods.Clear();
            methods.Add(method);
            Refresh();
        }

        /// <summary>
        /// Renders the method provided.
        /// </summary>
        /// <param name="methodList"></param>
        public void RenderLCMethod(List<LCMethod> methodList)
        {
            methods.Clear();
            methods.AddRange(methodList);
            Refresh();
        }

        public void Refresh()
        {
            RefreshView?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Renders the method as a timeline in the height specified.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="bounds"></param>
        public void RenderGraphics(DrawingContext e, Rect bounds)
        {
            var renderer = LCRendererFactory.GetRenderer(RenderMode);
            if (renderer == null)
                return;

            var startTime = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            var duration = new TimeSpan(0, 30, 0);
            foreach (var column in CartConfiguration.Columns)
            {
                if (column.Status != ColumnStatus.Disabled)
                {
                    renderer.ColumnNames.Add(column.Name);
                }
            }
            renderer.ColumnNames.Add("Special");
            if (methods != null && methods.Count > 0)
            {
                // Map the colors appropriately if they haven't already been
                if (deviceColorMappings.Count < 1)
                    RemapDevicesToColors();

                // Find the duration of the total comparison
                startTime = methods[0].Start;
                var endTime = methods[methods.Count - 1].End;
                foreach (var renderData in methods)
                {
                    var span = renderData.End.Subtract(endTime);
                    if (span.TotalMilliseconds > 0)
                        endTime = renderData.End;
                }
                duration = endTime.Subtract(startTime);
            }
            if (RenderMode == LCMethodRenderMode.Conversation)
            {
                renderer.StartEventIndex = StartEventIndex;
            }

            if (bounds.Width > 0)
            {
                bounds.Width -= 10;
            }
            bounds.X += 5;

            renderer.RenderLCMethod(e, bounds, methods, startTime, duration, deviceColorMappings, DateTime.MaxValue);

            //Render scroll buttons
            if (RenderMode == LCMethodRenderMode.Conversation)
            {
                var classLcMethodConversationRenderer = renderer as LCMethodConversationRenderer;
                if (classLcMethodConversationRenderer != null)
                    buttonLocations = classLcMethodConversationRenderer.GetButtonLocations();

                var upButton = buttonLocations[0];
                var downButton = buttonLocations[1];

                //scroll up button rendering
                e.DrawRectangle(Brushes.LightGray, new Pen(Brushes.LightSlateGray, 1), upButton);
                //up-pointing arrow
                var upFigure = new PathFigure(new Point(upButton.X + (upButton.Width / 2), upButton.Y + upButton.Height / 3), new []
                {
                    new LineSegment(new Point(upButton.X + (upButton.Width / 4), upButton.Y + 2 * (upButton.Height / 3)), true),
                    new LineSegment(new Point(upButton.X + 3 * (upButton.Width / 4), upButton.Y + 2 * (upButton.Height / 3)), true),
                }, true);
                var upGeometry = new PathGeometry(new[] { upFigure });

                e.DrawGeometry(Brushes.Black, null, upGeometry);

                //scroll down button rendering
                e.DrawRectangle(Brushes.LightGray, new Pen(Brushes.LightSlateGray, 1), downButton);
                //down-pointing arrow
                var downFigure = new PathFigure(new Point(downButton.X + (downButton.Width / 2), downButton.Y + 2 * (downButton.Height / 3)), new[]
                {
                    new LineSegment(new Point(downButton.X + (downButton.Width / 4), downButton.Y + (downButton.Height / 3)), true),
                    new LineSegment(new Point(downButton.X + 3 * (downButton.Width / 4), downButton.Y + (downButton.Height / 3)), true),
                }, true);
                var downGeometry = new PathGeometry(new[] { downFigure });

                e.DrawGeometry(Brushes.Black, null, downGeometry);
            }
        }
    }
}
