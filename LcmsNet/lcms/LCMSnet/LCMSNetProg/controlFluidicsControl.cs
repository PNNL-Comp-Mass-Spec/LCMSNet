using System;
using System.Drawing;
using System.Windows.Forms;
using FluidicsSDK;
using LcmsNetDataClasses.Logging;

namespace LcmsNet
{
    public partial class controlFluidicsControl : UserControl
    {
        //consts for determining if multiple selection is allowed.
        private const bool MULTISELECT_YES = true;
        private const bool MULTISELECT_NO = false;
        //scale factor minimum and maximum in percent(since the trackbar provides int values, using a percent is convenient)
        private const int SCALE_MIN = 10;
        private const int SCALE_MAX = 200;
        //scale converter...divde the scale (in percent) from the trackbar by this value to come out with the proper scale factor for the graphics primitives.
        private const float SCALE_CONVERSION = 100.0F;

        private static bool m_locked;
        private bool dragndrop;
        private Image m_bitmap;
        private readonly classFluidicsModerator m_fluidics_mod;
        private bool m_moving;
        private Point m_NewMouseLocation;
        // member variables used to track mouse movement, used for movement of fluidics glyphs.
        private Point m_OldMouseLocation;
        private bool selectionMade;


        public controlFluidicsControl()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            InitializeComponent();
            m_fluidics_mod = classFluidicsModerator.Moderator;
            panelFluidicsDesign.MouseDown += panelFluidicsDesign_MouseDown;
            panelFluidicsDesign.MouseUp += panelFluidicsDesign_MouseUp;
            panelFluidicsDesign.MouseMove += panelFluidicsDesign_MouseMove;
            panelFluidicsDesign.DoubleClick += panelFluidicsDesign_DoubleClick;
            panelFluidicsDesign.Paint += panelFluidicsDesign_Paint;
            VisibleChanged += new EventHandler((sender, e) =>
            {
                if ((sender as Control).Visible)
                {
                    UpdateImage();
                    Refresh();
                }
            }); // ensures that the control has the most up-to-date image when a user switches to it.
            trackBarScale.Value = 100;
            trackBarPortTransparency.Value = 255;
            trackBarDeviceTransparency.Value = 255;
            trackBarConnectionTransparency.Value = 255;
            textBoxZoom.Text = "100";
            m_locked = false;
            dragndrop = false;
            panelFluidicsDesign.AutoScrollMinSize = panelFluidicsDesign.ClientRectangle.Size;
            m_bitmap = new Bitmap(panelFluidicsDesign.Size.Width, panelFluidicsDesign.Size.Height);
            //panelFluidicsDesign.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }

        public bool DevicesLocked
        {
            get { return m_locked; }
            set { m_locked = value; }
        }

        /// <summary>
        /// When a trackbar value is changed by the user, we want the UI to automatically update the scale textbox
        /// </summary>
        /// <param name="sender">the trackbar changed</param>
        /// <param name="e">event arguments</param>
        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var bar = (TrackBar) sender;
                if (bar.Name == "trackBarScale")
                {
                    //if the textbox value is different from the trackbar value, update the textbox
                    if (int.Parse(textBoxZoom.Text) != bar.Value)
                    {
                        textBoxZoom.Text = bar.Value.ToString();
                    }
                    m_fluidics_mod.ScaleWorldView(bar.Value / SCALE_CONVERSION);
                }
                UpdateImage();
                panelFluidicsDesign.Invalidate(true);
                panelFluidicsDesign.Update();
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex);
            }
            catch (ArgumentNullException ex)
            {
                ShowError(ex);
            }
            catch (OverflowException ex)
            {
                ShowError(ex);
            }
                //shouldn't ever get here
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// if the text entered in the textbox is valid, we want to check and see if the change was made by a user, or by the trackbar
        /// and if it was made by the user, update the trackbar and scale the fluidics elements accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textboxZoom_Validated(object sender, EventArgs e)
        {
            try
            {
                var self = (TextBox) sender;
                var val = int.Parse(self.Text);
                // if the trackbar value is not the same as the text value, it was entered by a user (or some method other than the trackbar)
                // so change the trackbar to match
                if (trackBarScale.Value != val)
                {
                    trackBarScale.Value = val;
                }
            }
            catch (FormatException ex)
            {
                ShowError(ex);
            }
            catch (ArgumentNullException ex)
            {
                ShowError(ex);
            }
            catch (OverflowException ex)
            {
                ShowError(ex);
            }
                // shouldn't ever get this
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Make the textbox update when the enter key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxZoom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // if enter is pressed...just tab to the trackBar_zoom
            var enterKeyASCII = 13;
            if (e.KeyChar == (char) enterKeyASCII)
            {
                try
                {
                    SendKeys.Send("{Tab}");
                }
                catch (InvalidOperationException ex)
                {
                    ShowError(ex);
                }
                catch (ArgumentException ex)
                {
                    ShowError(ex);
                }
                    // shouldn't ever get this
                catch (Exception ex)
                {
                    ShowError(ex);
                }
                // keep the textbox from making an audible ding by setting handled to true
                e.Handled = true;
            }
        }

        /// <summary>
        /// Allow the textbox to respond to the enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxZoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.IsInputKey = true;
            }
        }

        /// <summary>
        /// When the textbox is entered, select all the text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxZoom_Enter(object sender, EventArgs e)
        {
            var zoomBox = (TextBox) sender;
            zoomBox.SelectAll();
        }

        /// <summary>
        /// event handler for the mousedown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void panelFluidicsDesign_MouseDown(object sender, MouseEventArgs e)
        {
            m_fluidics_mod.ScaleWorldView(trackBarScale.Value / SCALE_CONVERSION);
            if (DevicesLocked == false)
            {
                try
                {
                    selectionMade = false;
                    if (!m_fluidics_mod.MethodRunning)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            m_OldMouseLocation = e.Location;
                            dragndrop = true;
                            //toggle dragndrop mode
                            //select the connection/port/device at the location of the mouse.
                            var trueLocation = new Point(e.Location.X - panelFluidicsDesign.AutoScrollPosition.X,
                                e.Location.Y - panelFluidicsDesign.AutoScrollPosition.Y);
                            if (ModifierKeys != Keys.Control)
                            {
                                selectionMade = m_fluidics_mod.Select(trueLocation, MULTISELECT_NO);
                            }
                            else
                            {
                                selectionMade = m_fluidics_mod.Select(trueLocation, MULTISELECT_YES);
                            }
                        }
                    }
                    //only deselect if you did *not* just select something or taken an action
                    if ((!selectionMade))
                    {
                        if (ModifierKeys != Keys.Control)
                        {
                            m_fluidics_mod.Deselect(e.Location, MULTISELECT_NO);
                        }
                        else
                        {
                            m_fluidics_mod.Deselect(e.Location, MULTISELECT_YES);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void panelFluidicsDesign_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (DevicesLocked == false)
                {
                    dragndrop = false;
                    m_moving = false;
                    if (selectionMade && m_fluidics_mod.GetSelectedPortCount() == 2)
                    {
                        var result = MessageBox.Show("Do you want to connect the selected ports?", "Connect",
                            MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            m_fluidics_mod.CreateConnection();
                            m_fluidics_mod.DeselectPorts();
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


        public void UpdateImage()
        {
            if ((ParentForm.MdiParent == null || ParentForm.MdiParent.ActiveMdiChild == ParentForm) &&
                Visible)
            {
                System.Diagnostics.Debug.WriteLine("Updating Image");
                try
                {
                    m_fluidics_mod.SetWorldView(panelFluidicsDesign.ClientRectangle);
                    var scale = trackBarScale.Value / SCALE_CONVERSION;
                    m_fluidics_mod.ScaleWorldView(scale);
                    const int bufferSize = 150;
                    var imageSize = m_fluidics_mod.GetBoundingBox().Size;
                    if (scale > 1)
                    {
                        imageSize.Width = (int) (imageSize.Width * scale);
                        imageSize.Height = (int) (imageSize.Height * scale);
                    }
                    if (imageSize.Height <= panelFluidicsDesign.ClientRectangle.Height &&
                        imageSize.Width <= panelFluidicsDesign.ClientRectangle.Width)
                    {
                        imageSize = panelFluidicsDesign.ClientRectangle.Size;
                    }
                    else
                    {
                        imageSize.Height += bufferSize;
                        imageSize.Width += bufferSize;
                    }
                    m_bitmap.Dispose();
                    m_bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                    using (var g = Graphics.FromImage(m_bitmap))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        //each layer is rendered at the transparency value specified by the UI transparency trackbar for that layer, possible values 0-255.
                        m_fluidics_mod.Render(g, trackBarDeviceTransparency.Value, scale, Layer.Devices);
                        m_fluidics_mod.Render(g, trackBarConnectionTransparency.Value, scale, Layer.Connections);
                        m_fluidics_mod.Render(g, trackBarPortTransparency.Value, scale, Layer.Ports);
                    }
                }
                    //should never happen
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            }
        }

        /// <summary>
        /// event handler for the panel's paint event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelFluidicsDesign_Paint(object sender, PaintEventArgs e)
        {
            panelFluidicsDesign.AutoScrollMinSize = m_bitmap.Size;
            e.Graphics.TranslateTransform(panelFluidicsDesign.AutoScrollPosition.X,
                panelFluidicsDesign.AutoScrollPosition.Y);
            e.Graphics.DrawImageUnscaled(m_bitmap, 0, 0);
            e.Graphics.ResetTransform();
        }

        /// <summary>
        /// show error to user
        /// </summary>
        /// <param name="message">message to show user</param>
        private void ShowError(Exception ex)
        {
            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, ex.Message, ex);
        }


        /// <summary>
        /// when the mouse moves, if the left button is held down, it should drag selected items with it
        /// this event handler accomplishes that task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelFluidicsDesign_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (dragndrop || m_moving)
                {
                    m_moving = true;
                    m_NewMouseLocation = e.Location;
                    //amount the mouse moved.

                    var amountMoved = new Point(m_NewMouseLocation.X - m_OldMouseLocation.X,
                        m_NewMouseLocation.Y - m_OldMouseLocation.Y);
                    if (amountMoved.X + amountMoved.Y != 0)
                    {
                        m_fluidics_mod.MoveSelectedDevices(amountMoved);
                    }
                    //once the items have been moved, the new location is the 'old' location for purposes of the calculation, this keeps the objects from zipping
                    //around and off the screen super fast. Without this, the amountMoved point would grow rapidly for even small movements of the mouse.
                    m_OldMouseLocation = m_NewMouseLocation;
                }
            }
                //shouldn't happen
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void panelFluidicsDesign_DoubleClick(object sender, EventArgs e)
        {
            var me = e as MouseEventArgs;
            var p = MousePosition;
            var p2 = me.Location;
            var trueLocation = new Point(p2.X - panelFluidicsDesign.AutoScrollPosition.X,
                p2.Y - panelFluidicsDesign.AutoScrollPosition.Y);
            m_fluidics_mod.DoubleClickActions(trueLocation);
        }

        private void controlFluidicsControl_SizeChanged(object sender, EventArgs e)
        {
            panelFluidicsDesign.AutoScrollMinSize = panelFluidicsDesign.ClientRectangle.Size;
        }

        private void trackBarConnectionTransparency_Scroll(object sender, EventArgs e)
        {
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void controlFluidicsControl_ParentChanged(object sender, EventArgs e)
        {
        }

        private void controlFluidicsControl_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}