using System;
using System.Windows.Forms;
using LcmsNet.Devices.Pumps.ViewModels;

namespace LcmsNet.Devices.Pumps
{
    public partial class formPumpDisplays2 : Form
    {
        private PumpDisplaysViewModel pumpDisplaysViewModel;

        public formPumpDisplays2()
        {
            InitializeComponent();
            pumpDisplaysViewModel = new PumpDisplaysViewModel();
            pumpDisplaysView.DataContext = pumpDisplaysViewModel;

            pumpDisplaysViewModel.Tack += mbutton_expand_Click;
            pumpDisplaysViewModel.UnTack += mbutton_expand_Click;
        }

        /// <summary>
        /// Gets or sets whether the window is tacked.
        /// </summary>
        public bool IsTacked { get; set; }

        public event EventHandler Tack;
        public event EventHandler UnTack;

        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            IsTacked = (IsTacked == false);
            if (IsTacked)
            {
                Tack?.Invoke(this, e);
            }
            else
            {
                UnTack?.Invoke(this, e);
            }
        }
    }
}
