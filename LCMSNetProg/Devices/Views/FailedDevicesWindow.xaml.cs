using System.Windows;

namespace LcmsNet.Devices.Views
{
    /// <summary>
    /// Interaction logic for FailedDevicesWindow.xaml
    /// </summary>
    public partial class FailedDevicesWindow : Window
    {
        public FailedDevicesWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
