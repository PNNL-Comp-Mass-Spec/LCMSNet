using System.Windows;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for ConvertToolWindow.xaml
    /// </summary>
    public partial class ConvertToolWindow : Window
    {
        public ConvertToolWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
