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
            if (!(this.DataContext is TrayVialViewModel dc))
            {
                return;
            }

            if (!(sender is MultiSelector selector))
            {
                return;
            }

            dc.SelectedSamples.Clear();
            dc.SelectedSamples.AddRange(selector.SelectedItems.Cast<TrayVialSampleViewModel>());
        }
    }
}
