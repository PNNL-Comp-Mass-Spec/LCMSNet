using System.Drawing;
using System.Windows.Forms;

namespace LcmsNet
{
    /// <summary>
    /// Custom panel class that includes double buffering, used for rendering fluidics objects
    /// </summary>
    public sealed class controlBufferedPanel : Panel
    {
        public controlBufferedPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            AutoScroll = true;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // controlBufferedPanel
            //
            BackColor = Color.White;
            ResumeLayout(false);
        }
    }
}