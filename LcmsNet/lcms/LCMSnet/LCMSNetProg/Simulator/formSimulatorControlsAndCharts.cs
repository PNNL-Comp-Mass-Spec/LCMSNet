using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNet.Method;
using LcmsNet.Devices;
using LcmsNet.Method.Forms;

namespace LcmsNet.Simulator
{
    public partial class formSimulatorControlsAndCharts : Form
    {
        private SimControlsAndChartsControl m_controls;
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
                this.Show();
            }
        }

        public new void Show()
        {
            this.Controls.Add(m_controls);
            if (!m_controls.Tacked)
            {
                this.MdiParent = null;
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