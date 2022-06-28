using System.Windows;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for ThroughputPreviewWindow.xaml
    /// </summary>
    public partial class ThroughputPreviewWindow : Window
    {
        public ThroughputPreviewWindow()
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
