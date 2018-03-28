using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for DMSDownloadDataGridView.xaml
    /// </summary>
    public partial class DMSDownloadDataGridView : UserControl
    {
        public DMSDownloadDataGridView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as DMSDownloadDataViewModel;
            if (dc == null)
            {
                return;
            }

            var selector = sender as MultiSelector;
            if (selector == null)
            {
                return;
            }

            dc.SelectedData.Clear();
            //dc.SelectedData.AddRange(selector.SelectedItems.Cast<DMSDownloadDataGridViewModel>());
            dc.SelectedData.AddRange(selector.SelectedItems.Cast<classSampleData>());
        }
    }
}
