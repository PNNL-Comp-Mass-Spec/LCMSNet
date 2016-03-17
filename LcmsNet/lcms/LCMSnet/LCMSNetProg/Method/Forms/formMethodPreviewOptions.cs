using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Form to display animation options for method alignment.
    /// </summary>
    public partial class formMethodPreviewOptions : Form
    {
        public formMethodPreviewOptions()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(formMethodPreviewOptions_FormClosing);
        }

        private void mcheckBox_animate_CheckedChanged(object sender, EventArgs e)
        {
        }

        #region Properties

        /// <summary>
        /// Gets or sets whether to animate the method optimization not.
        /// </summary>
        public bool Animate
        {
            get { return mcheckBox_animate.Checked; }
            set { mcheckBox_animate.Checked = value; }
        }

        /// <summary>
        /// Gets or sets the animation delay time in ms.
        /// </summary>
        public int AnimationDelay
        {
            get { return Convert.ToInt32(mnum_delay.Value); }
            set { mnum_delay.Value = Convert.ToDecimal(value); }
        }

        /// <summary>
        /// Gets or sets the frame delay count.
        /// </summary>
        public int FrameDelay
        {
            get { return Convert.ToInt32(mnum_frameCount.Value); }
            set { mnum_frameCount.Value = Convert.ToDecimal(value); }
        }

        #endregion

        #region Form Event Handlers

        void formMethodPreviewOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                DialogResult = DialogResult.Cancel;
                Hide();
            }
        }

        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void mbutton_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        #endregion
    }
}