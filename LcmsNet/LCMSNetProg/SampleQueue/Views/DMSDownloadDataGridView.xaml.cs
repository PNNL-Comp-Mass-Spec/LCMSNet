using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using LcmsNet.Method.ViewModels;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNetSDK.Data;
using ReactiveUI;

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
            dc.SelectedData.AddRange(selector.SelectedItems.Cast<SampleData>());
        }

        private void DmsDownloadData_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is DMSDownloadDataViewModel ddvm)
            {
                if (dmsDownloadDataDataContext != null)
                {
                    dmsDownloadDataDataContext.ClearViewSort -= ClearViewSort;
                }
                dmsDownloadDataDataContext = ddvm;
                dmsDownloadDataDataContext.ClearViewSort += ClearViewSort;
            }
        }

        private DMSDownloadDataViewModel dmsDownloadDataDataContext = null;

        private void ClearViewSort(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearViewSortInternal);
            }
            else
            {
                ClearViewSortInternal();
            }
        }

        private void ClearViewSortInternal()
        {
            if (this.DataContext == null)
            {
                return;
            }

            var view = CollectionViewSource.GetDefaultView(SampleDataGrid.ItemsSource);
            if (view != null && view.SortDescriptions.Count > 0)
            {
                view.SortDescriptions.Clear();
            }

            foreach (var column in SampleDataGrid.Columns)
            {
                column.SortDirection = null;
            }
        }
    }
}
