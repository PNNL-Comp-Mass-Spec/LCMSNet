using System.Windows;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for SampleMethodFillDownWindow.xaml
    /// </summary>
    public partial class SampleMethodFillDownWindow : Window
    {
        public SampleMethodFillDownWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
