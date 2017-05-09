using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LcmsNet.WPFControls.ViewModels;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for sampleView.xaml
    /// </summary>
    public partial class SampleView : UserControl
    {
        public SampleView()
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
