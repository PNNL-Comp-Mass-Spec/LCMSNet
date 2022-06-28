using System.Windows;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for TrayVialAssignmentWindow.xaml
    /// </summary>
    public partial class TrayVialAssignmentWindow : Window
    {
        public TrayVialAssignmentWindow()
        {
            InitializeComponent();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
