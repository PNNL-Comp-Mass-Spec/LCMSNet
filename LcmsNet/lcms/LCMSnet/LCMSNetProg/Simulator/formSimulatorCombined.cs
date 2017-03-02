using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LcmsNet.Simulator
{
    public partial class formSimulatorCombined : Form
    {
        private SimConfigControl m_config;

        private SimControlsAndChartsControl m_controls;

        /// <summary>
        /// dictionary that will hold methods queued for simulation.
        /// </summary>
        private Dictionary<string, LcmsNetDataClasses.Method.classLCMethod> m_queuedMethods;

        public formSimulatorCombined()
        {
            m_queuedMethods = new Dictionary<string, LcmsNetDataClasses.Method.classLCMethod>();
            InitializeComponent();
            m_controls = SimControlsAndChartsControl.GetInstance;
            m_controls.Dock = DockStyle.Fill;
            m_controls.Tack += new EventHandler<TackEventArgs>(controlsTack);
            m_config = SimConfigControl.GetInstance;
            m_config.Dock = DockStyle.Fill;
            m_config.Tack += new EventHandler<TackEventArgs>(controlsTack);
            splitFluidicsAndControls.Panel2.Controls.Add(m_controls);
            splitFluidicsAndControls.Panel1.Controls.Add(m_config);
            this.Activated += formSimulatorCombined_Activated;
        }

        public event EventHandler<TackEventArgs> Tack;

        private void formSimulatorCombined_Activated(object sender, EventArgs e)
        {
            //work around to ensure that simulator shows config image when simulator gets focus.
            m_config.UpdateImage();
            m_config.Refresh();
        }

        void Setup()
        {
            m_controls = SimControlsAndChartsControl.GetInstance;
            m_controls.Dock = DockStyle.Fill;
            m_config = SimConfigControl.GetInstance;
            m_config.Dock = DockStyle.Fill;
            splitFluidicsAndControls.Panel2.Controls.Add(m_controls);
            splitFluidicsAndControls.Panel1.Controls.Add(m_config);
        }

        public new void Show()
        {
            Setup();
            base.Show();
        }

        void controlsTack(object sender, TackEventArgs e)
        {
            // this form is NOT triggering the tack, but rather the object this form is listening to is, so send THAT as the
            // sender of this event...we're just passing it up the chain to formMDIMain
            Tack?.Invoke(sender, e);
        }
    }
}