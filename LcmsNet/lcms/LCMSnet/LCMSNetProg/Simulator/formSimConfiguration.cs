using System.Windows.Forms;

namespace LcmsNet.Simulator
{
    public partial class formSimConfiguration : Form
    {
        private readonly SimConfigControl m_config;

        public formSimConfiguration()
        {
            InitializeComponent();
            m_config = SimConfigControl.GetInstance;
            m_config.Dock = DockStyle.Fill;
            //m_config.Tack += new EventHandler<TackEventArgs>(m_config_Tack);
        }

        private void m_config_Tack(object sender, TackEventArgs e)
        {
            if (e.Tacked)
            {
                //this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        public new void Show()
        {
            this.Controls.Add(m_config);
            if (!m_config.Tacked)
            {
                this.MdiParent = null;
            }
            base.Show();
        }

        private void formSimConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }
    }
}