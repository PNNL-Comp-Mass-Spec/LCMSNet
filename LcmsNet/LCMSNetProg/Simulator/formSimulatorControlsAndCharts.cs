using System.Windows.Forms;

namespace LcmsNet.Simulator
{
    public partial class formSimulatorControlsAndCharts : Form
    {
        private readonly SimControlsAndChartsControl m_controls;
        private FluidicsSimulator.FluidicsSimulator m_simulator;

        public formSimulatorControlsAndCharts()
        {
            InitializeComponent();
            m_simulator = FluidicsSimulator.FluidicsSimulator.GetInstance;
            m_controls = SimControlsAndChartsControl.GetInstance;
            m_controls.Dock = DockStyle.Fill;
            //m_controls.Tack += new EventHandler<TackEventArgs>(m_controls_Tack);
        }


        void m_controls_Tack(object sender, TackEventArgs e)
        {
            if (e.Tacked)
            {
                //this.Hide();
            }
            else
            {
                Show();
            }
        }

        public new void Show()
        {
            Controls.Add(m_controls);
            if (!m_controls.Tacked)
            {
                MdiParent = null;
            }
            base.Show();
        }

        private void formSimulatorControlsAndCharts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }
    }
}