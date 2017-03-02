using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using LcmsNetDataClasses.Devices;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Header for a device control
    /// </summary>
    public partial class controlDeviceStatusDisplay : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlDeviceStatusDisplay()
        {
            InitializeComponent();
            BeginUpdate();
            m_isDeviceInUse = false;
            m_isError = false;
            m_isInitialized = false;
            EndUpdate();

            MouseClick += new MouseEventHandler(controlDeviceStatusDisplay_MouseClick);

            Refresh();
            PerformLayout();
        }

        public IDevice Device { get; set; }

        void controlDeviceStatusDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }


        public void SetActiveGlyph()
        {
        }

        public void SetDeActiveGlyph()
        {
        }

        private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.BringToFront();
            }
        }

        private void sendToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.SendToBack();
            }
        }

        private void fixErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ErrorIndicatorClicked != null)
            {
                ErrorIndicatorClicked(this, e);
            }
        }

        #region Members

        /// <summary>
        /// Number of pixels to pad the indicators from the margins.
        /// </summary>
        private const int CONST_INDICATOR_PADDING = 5;

        /// <summary>
        /// Number of pixels to pad the indicators from the margins.
        /// </summary>
        private const int CONST_INDICATOR_TOP_PADDING = 1;

        /// <summary>
        /// Flag that indicates if the device has an error.
        /// </summary>
        private bool m_isError;

        /// <summary>
        /// Flag that indicates if a device has been initialized.
        /// </summary>
        private bool m_isInitialized;

        /// <summary>
        /// Flag that indicates if a device is in use..
        /// </summary>
        private bool m_isDeviceInUse;

        /// <summary>
        /// Flag indicating the control is updating and to suspend layout and painting.
        /// </summary>
        private bool m_updating;

        /// <summary>
        /// fired when the show control window is pressed.
        /// </summary>
        public event EventHandler ShowDetailsWindow;

        public event EventHandler ErrorIndicatorClicked;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to signal the error indicator.
        /// </summary>
        public bool IsError
        {
            get { return m_isError; }
            set
            {
                m_isError = value;
                if (!m_updating)
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to signal the initialized indicator.
        /// </summary>
        public bool IsInitialized
        {
            get { return m_isInitialized; }
            set
            {
                m_isInitialized = value;
                if (!m_updating)
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the device is in use.
        /// </summary>
        public bool IsInUse
        {
            get { return m_isDeviceInUse; }
            set
            {
                m_isDeviceInUse = value;
                if (!m_updating)
                {
                    Refresh();
                }
            }
        }

        #endregion

        #region Device Event Handlers

        public void UpdateIndicators()
        {
            Refresh();
        }

        public void DeviceStatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            switch (e.Status)
            {
                case enumDeviceStatus.Initialized:
                    m_isInitialized = true;
                    m_isError = false;
                    m_isDeviceInUse = false;
                    break;
                case enumDeviceStatus.NotInitialized:
                    m_isInitialized = false;
                    m_isDeviceInUse = false;
                    m_isError = false;
                    break;
                case enumDeviceStatus.InUseByMethod:
                    m_isDeviceInUse = true;
                    m_isError = false;
                    break;
                case enumDeviceStatus.Error:
                    m_isError = true;
                    m_isDeviceInUse = false;
                    break;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(UpdateIndicators));
            }
            else
            {
                UpdateIndicators();
            }
        }

        public void DeviceError(object sender, classDeviceErrorEventArgs e)
        {
            if (e.ErrorStatus == enumDeviceErrorStatus.NoError)
                m_isError = false;
            else
                m_isError = true;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(UpdateIndicators));
            }
            else
            {
                UpdateIndicators();
            }
        }

        #endregion

        #region User Interface Event Handlers

        /// <summary>
        /// Suspend custom rendering.
        /// </summary>
        public void BeginUpdate()
        {
            m_updating = true;
        }

        /// <summary>
        /// Resume custom rendering.
        /// </summary>
        public void EndUpdate()
        {
            m_updating = false;
            Refresh();
        }

        /// <summary>
        /// Draws the indicator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="onColor"></param>
        /// <param name="offColor"></param>
        protected void DrawIndicator(Graphics g, int left, int top, int width, int height, Color onColor, Color offColor,
            bool isOn)
        {
            var indicatorColor = onColor;
            if (!isOn)
            {
                indicatorColor = offColor;
            }

            //using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(left, top, left + width - CONST_INDICATOR_PADDING, top + height- CONST_INDICATOR_PADDING), Color.White, indicatorColor, LinearGradientMode.ForwardDiagonal))
            using (Brush brush = new SolidBrush(indicatorColor))
            {
                g.FillEllipse(brush, left, top, width, height);

                using (Brush outline = new SolidBrush(Color.Black))
                {
                    using (var pen = new Pen(outline, 1.0F))
                    {
                        g.DrawEllipse(pen, left, top, width, height);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a custom background with error and status indicators.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            using (
                var brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.White,
                    Color.LightGray, LinearGradientMode.Vertical))
            {
                using (Brush outline = new SolidBrush(Color.Black))
                {
                    using (var pen = new Pen(outline, 1.0F))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
                    }
                }
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
            }

            var offset = 0;

            if (BorderStyle != BorderStyle.None)
                offset += CONST_INDICATOR_TOP_PADDING;

            var height = Height - CONST_INDICATOR_PADDING;
            var width = height;
            var left = Width - height + offset - CONST_INDICATOR_PADDING;
            var top = CONST_INDICATOR_TOP_PADDING;
            // Draw the initialized indicator.       
            DrawIndicator(e.Graphics, left, top, width, height, Color.Lime, Color.DarkOliveGreen, m_isInitialized);
            // Draw the in use indicator.
            left -= (height + CONST_INDICATOR_PADDING);
            DrawIndicator(e.Graphics, left, top, width, height, Color.Yellow, Color.Olive, m_isDeviceInUse);
            // Draw the error indicator.
            left -= (height + CONST_INDICATOR_PADDING);
            DrawIndicator(e.Graphics, left, top, width, height, Color.Red, Color.DarkRed, m_isError);
        }

        /// <summary>
        /// Displays the attached control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_show_Click(object sender, EventArgs e)
        {
            if (ShowDetailsWindow != null)
            {
                ShowDetailsWindow(this, new EventArgs());
            }
        }

        #endregion
    }
}