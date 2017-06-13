using System;
using System.Windows.Forms;
using LcmsNet.Simulator.ViewModels;

namespace LcmsNet.Simulator
{
    public partial class formSimulatorCombined2 : Form
    {
        public formSimulatorCombined2()
        {
            InitializeComponent();
            simConfigVm = new SimulatorCombinedViewModel();
            Activated += formSimulatorCombined_Activated;

            simCombinedView.DataContext = simConfigVm;
        }

        private readonly SimulatorCombinedViewModel simConfigVm;

        private void formSimulatorCombined_Activated(object sender, EventArgs e)
        {
            //work around to ensure that simulator shows config image when simulator gets focus.
            simConfigVm.ConfigVm.UpdateImage();
        }

        public new void Show()
        {
            base.Show();
        }
    }
}