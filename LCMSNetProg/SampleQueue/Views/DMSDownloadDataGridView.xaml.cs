﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LcmsNet.IO.DMS;
using LcmsNet.SampleQueue.ViewModels;

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
            if (!(this.DataContext is DMSDownloadDataViewModel dc))
            {
                return;
            }

            if (!(sender is MultiSelector selector))
            {
                return;
            }

            dc.SelectedData.Clear();
            //dc.SelectedData.AddRange(selector.SelectedItems.Cast<DMSDownloadDataGridViewModel>());
            dc.SelectedData.AddRange(selector.SelectedItems.Cast<DmsDownloadData>());
        }

        private void DmsDownloadData_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is DMSDownloadDataViewModel ddvm)
            {
                if (dmsDownloadDataDataContext != null)
                {
                    dmsDownloadDataDataContext.SetSortBatchBlockRunOrder -= SetSortBatchBlockRunOrder;
                }
                dmsDownloadDataDataContext = ddvm;
                dmsDownloadDataDataContext.SetSortBatchBlockRunOrder += SetSortBatchBlockRunOrder;
            }
        }

        private DMSDownloadDataViewModel dmsDownloadDataDataContext = null;

        private void SetSortBatchBlockRunOrder(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(SetSortBatchBlockRunOrderInternal);
            }
            else
            {
                SetSortBatchBlockRunOrderInternal();
            }
        }

        private void SetSortBatchBlockRunOrderInternal()
        {
            if (this.DataContext == null)
            {
                return;
            }

            foreach (var column in SampleDataGrid.Columns)
            {
                column.SortDirection = null;
            }

            var view = (ICollectionView)SampleDataGrid.ItemsSource;
            if (view != null)
            {
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new SortDescription("DmsData.Batch", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("DmsData.Block", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("DmsData.RunOrder", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("DmsData.RequestID", ListSortDirection.Ascending));
            }
        }
    }
}
