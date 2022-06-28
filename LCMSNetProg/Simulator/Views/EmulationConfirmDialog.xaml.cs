using System.Windows;

namespace LcmsNet.Simulator.Views
{
    /// <summary>
    /// Interaction logic for EmulationConfirmDialog.xaml
    /// </summary>
    public partial class EmulationConfirmDialog : Window
    {
        public EmulationConfirmDialog()
        {
            InitializeComponent();
        }

        public MessageBoxResult Result { get; private set; }

        private void EnableEmulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void DoNotEnableEmulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void CancelSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Result = MessageBoxResult.Cancel;
            this.Close();
        }
    }
}
