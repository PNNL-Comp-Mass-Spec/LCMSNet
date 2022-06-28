using System.Windows;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for SampleValidatorErrorDisplayWindow.xaml
    /// </summary>
    public partial class SampleValidatorErrorDisplayWindow : Window
    {
        public SampleValidatorErrorDisplayWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
