using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LcmsNet.WPFControls.ViewModels;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for SampleColumnView.xaml
    /// </summary>
    public partial class SampleColumnView : UserControl
    {
        public SampleColumnView()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Subscribe to the "OnScrollUpdate" Event from the data context
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dependencyPropertyChangedEventArgs"></param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue != null && dependencyPropertyChangedEventArgs.OldValue is SampleControlViewModel oldScvm)
            {
                oldScvm.ScrollUpdateEvent -= SampleControlViewModel_OnScrollUpdateEvent;
            }
            if (dependencyPropertyChangedEventArgs.NewValue != null && dependencyPropertyChangedEventArgs.NewValue is SampleControlViewModel newScvm)
            {
                newScvm.ScrollUpdateEvent += SampleControlViewModel_OnScrollUpdateEvent;
            }
        }

        private void SampleControlViewModel_OnScrollUpdateEvent(object sender, SampleControlViewModel.SampleScrollChangeEventArgs sampleScrollChangeEventArgs)
        {
            if (sampleScrollChangeEventArgs.SampleToShow == null)
            {
                return;
            }
            this.Dispatcher.Invoke(() => this.SampleGrid.ScrollIntoView(sampleScrollChangeEventArgs.SampleToShow));
        }

        /// <summary>
        /// Provide a version of "SelectedItems" one-way-to-source binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as SampleControlViewModel;
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
            dc.SelectedSamples.AddRange(selector.SelectedItems.Cast<SampleViewModel>());
        }
    }
}
