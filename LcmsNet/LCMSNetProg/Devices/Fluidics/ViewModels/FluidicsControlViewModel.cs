using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FluidicsSDK;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNet.Devices.Fluidics.ViewModels
{
    public class FluidicsControlViewModel : ReactiveObject
    {
        public FluidicsControlViewModel()
        {
            fluidicsModerator = FluidicsModerator.Moderator;
            ZoomPercent = 100;
            PortTransparency = 255;
            DeviceTransparency = 255;
            ConnectionTransparency = 255;
            staticDevicesLocked = false;
            dragAndDrop = false;

            this.WhenAnyValue(x => x.ZoomPercent).Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(x => fluidicsModerator.ScaleWorldView(x / SCALE_CONVERSION));
            this.WhenAnyValue(x => x.ZoomPercent, x => x.ConnectionTransparency, x => x.PortTransparency, x => x.DeviceTransparency)
                .Throttle(TimeSpan.FromMilliseconds(250)).Subscribe(x => Refresh());
        }

        //consts for determining if multiple selection is allowed.
        private const bool MULTISELECT_YES = true;
        private const bool MULTISELECT_NO = false;

        //scale factor minimum and maximum in percent(since the trackbar provides int values, using a percent is convenient)
        private const int SCALE_MIN = 10;
        private const int SCALE_MAX = 200;

        //scale converter...divide the scale (in percent) from the trackbar by this value to come out with the proper scale factor for the graphics primitives.
        private const float SCALE_CONVERSION = 100.0F;

        private static bool staticDevicesLocked;
        private bool dragAndDrop;
        private readonly FluidicsModerator fluidicsModerator;
        private bool mouseMoving;

        private Point newMouseLocation;

        // member variables used to track mouse movement, used for movement of fluidics glyphs.
        private Point oldMouseLocation;

        private bool selectionMade;
        private int zoomPercent;
        private int connectionTransparency;
        private int portTransparency;
        private int deviceTransparency;
        private Size designPanelSize;
        private bool inSelectionMode = false;

        public bool DevicesLocked
        {
            get { return staticDevicesLocked; }
            set { this.RaiseAndSetIfChanged(ref staticDevicesLocked, value); }
        }

        public int ConnectionTransparency
        {
            get { return connectionTransparency; }
            set { this.RaiseAndSetIfChanged(ref connectionTransparency, value); }
        }

        public int PortTransparency
        {
            get { return portTransparency; }
            set { this.RaiseAndSetIfChanged(ref portTransparency, value); }
        }

        public int DeviceTransparency
        {
            get { return deviceTransparency; }
            set { this.RaiseAndSetIfChanged(ref deviceTransparency, value); }
        }

        public int ZoomPercent
        {
            get { return zoomPercent; }
            set
            {
                value = Math.Max(value, SCALE_MIN);
                value = Math.Min(value, SCALE_MAX);
                this.RaiseAndSetIfChanged(ref zoomPercent, value);
            }
        }

        public int ZoomMin => SCALE_MIN;
        public int ZoomMax => SCALE_MAX;

        public Size DesignPanelSize
        {
            get { return designPanelSize; }
            set { this.RaiseAndSetIfChanged(ref designPanelSize, value); }
        }

        public bool InSelectionMode
        {
            get { return inSelectionMode; }
            set { this.RaiseAndSetIfChanged(ref inSelectionMode, value); }
        }

        public event EventHandler RefreshView;

        public void RefreshHandler(object sender, EventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            RefreshView?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// event handler for the mousedown event
        /// </summary>
        /// <param name="mouseLoc"></param>
        public void MouseDownUpdates(Point mouseLoc)
        {
            fluidicsModerator.ScaleWorldView(ZoomPercent / SCALE_CONVERSION);
            if (DevicesLocked == false)
            {
                try
                {
                    selectionMade = false;
                    var drawingPoint = new Point((int)mouseLoc.X, (int)mouseLoc.Y);
                    if (!fluidicsModerator.MethodRunning)
                    {
                        oldMouseLocation = mouseLoc;
                        dragAndDrop = true;
                        //toggle dragndrop mode
                        //select the connection/port/device at the location of the mouse.
                        var trueLocation = drawingPoint;
                        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                        {
                            selectionMade = fluidicsModerator.Select(trueLocation, MULTISELECT_NO);
                        }
                        else
                        {
                            selectionMade = fluidicsModerator.Select(trueLocation, MULTISELECT_YES);
                        }
                    }
                    //only deselect if you did *not* just select something or taken an action
                    if ((!selectionMade))
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                        {
                            fluidicsModerator.Deselect(drawingPoint, MULTISELECT_NO);
                        }
                        else
                        {
                            fluidicsModerator.Deselect(drawingPoint, MULTISELECT_YES);
                        }
                    }
                }
                // really shouldn't be possible
                catch (Exception ex)
                {
                    ShowError(ex);
                }
                //this focus allows the panel to catch keyboard events e.g. a press of the 'delete' key.
                //this.panelFluidicsDesign.Focus();
            }
        }

        /// <summary>
        /// event handler for the mouseup event
        /// </summary>
        /// <param name="mouseLoc"></param>
        public void MouseUpUpdates(Point mouseLoc)
        {
            try
            {
                if (DevicesLocked == false)
                {
                    dragAndDrop = false;
                    mouseMoving = false;
                    if (selectionMade && fluidicsModerator.GetSelectedPortCount() == 2)
                    {
                        var result = MessageBox.Show("Do you want to connect the selected ports?", "Connect", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            fluidicsModerator.CreateConnection();
                            fluidicsModerator.DeselectPorts();
                        }
                    }
                    selectionMade = false;
                    //this.UpdateImage();
                    //this.Refresh();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private Size UpdateImage(DrawingContext drawingContext, Size size)
        {
            Debug.WriteLine("Updating Image Wpf");
            try
            {
                fluidicsModerator.SetWorldView(new Rect(0, 0, size.Width, size.Height));
                var scale = ZoomPercent / SCALE_CONVERSION;
                fluidicsModerator.ScaleWorldView(scale);
                const int bufferSize = 150;
                var imageSize = fluidicsModerator.GetBoundingBox().Size;
                if (scale > 1)
                {
                    imageSize.Width = (int)(imageSize.Width * scale);
                    imageSize.Height = (int)(imageSize.Height * scale);
                }
                if (imageSize.Height <= size.Height &&
                    imageSize.Width <= size.Width)
                {
                    imageSize = new Size(size.Width, size.Height);
                }
                else
                {
                    imageSize.Height += bufferSize;
                    imageSize.Width += bufferSize;
                }

                //each layer is rendered at the transparency value specified by the UI transparency trackbar for that layer, possible values 0-255.
                fluidicsModerator.Render(drawingContext, (byte)DeviceTransparency, scale, Layer.Devices);
                fluidicsModerator.Render(drawingContext, (byte)ConnectionTransparency, scale, Layer.Connections);
                fluidicsModerator.Render(drawingContext, (byte)PortTransparency, scale, Layer.Ports);

                return new Size(Math.Max(size.Width, imageSize.Width), Math.Max(size.Height, imageSize.Height));
            }
            //should never happen
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return size;
        }

        public Size RenderGraphics(DrawingContext drawContext, Size size)
        {
            return UpdateImage(drawContext, size);
        }

        /// <summary>
        /// show error to user
        /// </summary>
        /// <param name="ex">message to show user</param>
        private void ShowError(Exception ex)
        {
            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_USER, ex.Message, ex);
        }

        /// <summary>
        /// when the mouse moves, if the left button is held down, it should drag selected items with it
        /// this event handler accomplishes that task
        /// </summary>
        /// <param name="mouseLoc"></param>
        public void MouseMovedUpdates(Point mouseLoc)
        {
            try
            {
                if (dragAndDrop || mouseMoving)
                {
                    mouseMoving = true;
                    newMouseLocation = mouseLoc;
                    //amount the mouse moved.

                    var amountMoved = new Point(newMouseLocation.X - oldMouseLocation.X, newMouseLocation.Y - oldMouseLocation.Y);
                    if (amountMoved.X + amountMoved.Y != 0)
                    {
                        fluidicsModerator.MoveSelectedDevices(amountMoved);
                    }
                    //once the items have been moved, the new location is the 'old' location for purposes of the calculation, this keeps the objects from zipping
                    //around and off the screen super fast. Without this, the amountMoved point would grow rapidly for even small movements of the mouse.
                    oldMouseLocation = newMouseLocation;
                }
            }
            //shouldn't happen
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public void DesignDoubleClick(Point mouse)
        {
            var oldPoint = new Point((int)mouse.X, (int)mouse.Y);
            fluidicsModerator.DoubleClickActions(oldPoint);
        }
    }
}
