using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LcmsNet.WPFControls.ViewModels;
using LcmsNetDataClasses;

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for sampleView.xaml
    /// </summary>
    public partial class sampleView : UserControl
    {
        public sampleView()
        {
            InitializeComponent();
        }

        internal void FixScrollPosition()
        {
            var selected = SelectedSample;
            if (selected == null)
            {
                return;
            }

            var counter = 0;
            foreach (var item in this.SampleGrid.Items.SourceCollection)
            {
                if (selected.Equals(item))
                {
                    break;
                }
                counter++;
            }
            SetScrollOffset(Math.Max(counter - 3, 0));
        }

        public sampleViewModel SelectedSample
        {
            get { return this.SampleGrid.SelectedItem as sampleViewModel; }
        }

        public IEnumerable<sampleViewModel> SelectedSamples
        {
            get
            {
                foreach (var item in this.SampleGrid.SelectedItems)
                {
                    yield return item as sampleViewModel;
                }
            }
        }

        /// <summary>
        /// Get the vertical scroll offset in the datagrid
        /// </summary>
        /// <returns>vertical scroll offset</returns>
        public int GetCurrentScrollOffset()
        {
            var sv = GetScrollViewer();
            if (sv != null && sv.ComputedVerticalScrollBarVisibility == Visibility.Visible && sv.CanContentScroll)
            {
                //var a = sv.VerticalOffset;         // first visible row index
                //var b = sv.ContentVerticalOffset;  // first visible row index
                //var c = sv.ExtentHeight;           // total number of rows
                //var d = sv.ScrollableHeight;       // number of rows not visible
                //var e = sv.ViewportHeight;         // number of rows visible
                return (int)sv.VerticalOffset;
            }

            return 0;
        }

        /// <summary>
        /// Set the vertical scroll offset in the datagrid
        /// </summary>
        /// <param name="scrollOffset"></param>
        public void SetScrollOffset(int scrollOffset)
        {
            var sv = GetScrollViewer();
            if (sv != null && sv.ComputedVerticalScrollBarVisibility == Visibility.Visible && sv.CanContentScroll)
            {
                scrollOffset = Math.Max(scrollOffset, 0);
                var maxOffset = sv.ExtentHeight - sv.ViewportHeight;
                var offset = Math.Min(scrollOffset, maxOffset);
                sv.ScrollToVerticalOffset(offset);
            }
        }

        private PropertyInfo dataGridScrollViewerPropInfo = null;

        private ScrollViewer GetScrollViewer()
        {
            // Should also be able to use the following, but it might be more expensive (but probably more reliable in the long term):
            // var scrollViewer = FindVisualChild<ScrollViewer>(this.SampleGrid);
            if (dataGridScrollViewerPropInfo == null)
            {
                var allProps = this.SampleGrid.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                dataGridScrollViewerPropInfo = allProps.First(x => x.Name.ToLower().Equals("internalscrollhost"));
            }

            if (dataGridScrollViewerPropInfo != null)
            {
                return dataGridScrollViewerPropInfo.GetValue(this.SampleGrid) as ScrollViewer;
            }
            return null;
        }

        private static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
}
