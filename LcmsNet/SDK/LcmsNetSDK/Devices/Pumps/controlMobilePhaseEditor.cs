using System;
using System.Windows.Forms;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Devices.Pumps
{
    public partial class controlMobilePhaseEditor : UserControl
    {
        readonly MobilePhase m_phase;

        public controlMobilePhaseEditor()
        {
            InitializeComponent();
        }

        public controlMobilePhaseEditor(MobilePhase phase) : this()
        {
            m_phase = phase;

            mtext_comment.Text = phase.Comment;
            mtext_name.Text = phase.Name;
        }

        private void mtext_name_TextChanged(object sender, EventArgs e)
        {
            m_phase.Name = mtext_name.Text;
        }

        private void mtext_comment_TextChanged(object sender, EventArgs e)
        {
            m_phase.Comment = mtext_comment.Text;
        }
    }
}