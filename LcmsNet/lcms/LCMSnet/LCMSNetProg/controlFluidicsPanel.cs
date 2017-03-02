using System.Windows.Forms;

namespace LcmsNet
{
    /// <summary>
    /// Custom panel class that includes double buffering, used for rendering fluidics objects
    /// </summary>
    public class controlBufferedPanel : System.Windows.Forms.Panel
    {
        public controlBufferedPanel()
            : base()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            this.AutoScroll = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // controlBufferedPanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
        }
    }
}