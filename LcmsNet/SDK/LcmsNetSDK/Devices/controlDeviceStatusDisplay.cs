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
        private bool mbool_isError;
        /// <summary>
        /// Flag that indicates if a device has been initialized.
        /// </summary>
        private bool mbool_isInitialized;
        /// <summary>
        /// Flag that indicates if a device is in use..
        /// </summary>
        private bool mbool_isDeviceInUse;
        /// <summary>
        /// Flag indicating the control is updating and to suspend layout and painting.
        /// </summary>
        private bool mbool_updating;
        /// <summary>
        /// fired when the show control window is pressed.
        /// </summary>
        public event EventHandler ShowDetailsWindow;
        public event EventHandler ErrorIndicatorClicked;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlDeviceStatusDisplay()
        {
            InitializeComponent();
            BeginUpdate();
            mbool_isDeviceInUse = false;
            mbool_isError       = false;
            mbool_isInitialized = false;            
            EndUpdate();

            MouseClick += new MouseEventHandler(controlDeviceStatusDisplay_MouseClick);

            Refresh();
            PerformLayout();
        }

        public IDevice Device
        {
            get;
            set;
        }

        void controlDeviceStatusDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {                
                contextMenuStrip1.Show(Cursor.Position);                
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets whether to signal the error indicator.
        /// </summary>
        public bool IsError
        {
            get
            {
                return mbool_isError;
            }
            set
            {
                mbool_isError = value;
                if (!mbool_updating)
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
            get
            {
                return mbool_isInitialized;
            }
            set 
            {
                mbool_isInitialized = value;
                if (!mbool_updating)
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
            get
            {
                return mbool_isDeviceInUse;
            }
            set
            {
                mbool_isDeviceInUse = value;
                if (!mbool_updating)
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
                    mbool_isInitialized = true;
                    mbool_isError       = false;
                    mbool_isDeviceInUse = false;
                    break;
                case enumDeviceStatus.NotInitialized:
                    mbool_isInitialized = false;
                    mbool_isDeviceInUse = false;
                    mbool_isError       = false;
                    break;
                case enumDeviceStatus.InUseByMethod:
                    mbool_isDeviceInUse = true;
                    mbool_isError       = false;
                    break;
                case enumDeviceStatus.Error:
                    mbool_isError       = true;
                    mbool_isDeviceInUse = false;
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
                mbool_isError = false;
            else
                mbool_isError = true;

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
            mbool_updating = true;
        }
        /// <summary>
        /// Resume custom rendering.
        /// </summary>
        public void EndUpdate()
        {
            mbool_updating = false;
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
        protected void DrawIndicator(Graphics g, int left, int top, int width, int height, Color onColor, Color offColor, bool isOn)
        {
            Color indicatorColor = onColor;
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
                    using (Pen pen = new Pen(outline, 1.0F))
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
                        
            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.White, Color.LightGray, LinearGradientMode.Vertical))
            {                
                using (Brush outline = new SolidBrush(Color.Black))
                {
                    using (Pen pen = new Pen(outline, 1.0F))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
                    }
                }
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
            }

            int offset = 0;
            
            if (BorderStyle != BorderStyle.None)
                offset += CONST_INDICATOR_TOP_PADDING;

            int height  = Height - CONST_INDICATOR_PADDING;
            int width   = height;
            int left    = Width - height + offset - CONST_INDICATOR_PADDING;
            int top     = CONST_INDICATOR_TOP_PADDING;
            // Draw the initialized indicator.       
            DrawIndicator(e.Graphics, left, top, width, height, Color.Lime, Color.DarkOliveGreen, mbool_isInitialized);
            // Draw the in use indicator.
            left -= (height + CONST_INDICATOR_PADDING);
            DrawIndicator(e.Graphics, left, top, width, height, Color.Yellow, Color.Olive, mbool_isDeviceInUse);
            // Draw the error indicator.
            left -= (height + CONST_INDICATOR_PADDING);
            DrawIndicator(e.Graphics, left, top, width, height, Color.Red, Color.DarkRed, mbool_isError);
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
    }
}
