using System.Windows;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for FailedMethodLoadWindow.xaml
    /// </summary>
    public partial class FailedMethodLoadWindow : Window
    {
        public FailedMethodLoadWindow()
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
