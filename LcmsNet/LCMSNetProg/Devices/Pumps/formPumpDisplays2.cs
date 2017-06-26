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
        }

        /// <summary>
        /// Gets or sets whether the window is tacked.
        /// </summary>
        public bool IsTacked { get; set; }

        public event EventHandler Tack;
        public event EventHandler UnTack;
    }
}
