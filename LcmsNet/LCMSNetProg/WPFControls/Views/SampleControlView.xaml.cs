using System.Windows;
using System.Windows.Controls;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for SampleControlView.xaml
    /// </summary>
    public partial class SampleControlView : UserControl
    {
        public SampleControlView()
        {
            InitializeComponent();
        }

        private void ButtonSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            SampleView.SampleGrid.SelectAll();
        }
    }
}
