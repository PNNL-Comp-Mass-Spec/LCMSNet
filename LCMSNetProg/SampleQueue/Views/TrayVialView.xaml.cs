using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LcmsNet.SampleQueue.ViewModels;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for TrayVialView.xaml
    /// </summary>
    public partial class TrayVialView : UserControl
    {
        public TrayVialView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as TrayVialViewModel;
            if (dc == null)
            {
                return;
            }

            var selector = sender as MultiSelector;
            if (selector == null)
            {
                return;
            }

            dc.SelectedSamples.Clear();
            dc.SelectedSamples.AddRange(selector.SelectedItems.Cast<TrayVialSampleViewModel>());
        }
    }
}
