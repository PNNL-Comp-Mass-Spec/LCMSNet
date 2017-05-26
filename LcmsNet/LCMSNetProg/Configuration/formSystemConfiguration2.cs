using System;
using System.Windows.Forms;
using LcmsNet.Configuration.ViewModels;

namespace LcmsNet.Configuration
{
    public partial class formSystemConfiguration2 : Form
    {
        private SystemConfigurationViewModel systemConfigurationViewModel;

        /// <summary>
        ///  Default constructor for displaying column data.
        /// </summary>
        public formSystemConfiguration2()
        {
            InitializeComponent();

            systemConfigurationViewModel = new SystemConfigurationViewModel();
            systemConfigurationView.DataContext = systemConfigurationViewModel;

            systemConfigurationViewModel.ColumnNameChanged += ColumnNameChangedHandler;
        }

        public event EventHandler ColumnNameChanged;

        void ColumnNameChangedHandler(object sender, EventArgs e)
        {
            ColumnNameChanged?.Invoke(this, e);
        }
    }
}
