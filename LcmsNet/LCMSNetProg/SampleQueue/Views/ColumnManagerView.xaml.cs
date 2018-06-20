using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LcmsNet.SampleQueue.ViewModels;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for ColumnManagerView.xaml
    /// </summary>
    public partial class ColumnManagerView : UserControl
    {
        public ColumnManagerView()
        {
            InitializeComponent();
            // need to do a visibility update when first loaded.
            Loaded += ColumnManagerView_OnLoaded;
        }

        private void Column_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ColumnControlView ccv)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (ReferenceEquals(ccv, Column1View))
                {
                    cd = Column1Column;
                }
                else if (ReferenceEquals(ccv, Column2View))
                {
                    cd = Column2Column;
                }
                else if (ReferenceEquals(ccv, Column3View))
                {
                    cd = Column3Column;
                }
                else if (ReferenceEquals(ccv, Column4View))
                {
                    cd = Column4Column;
                }
                if (ccv.IsVisible)
                {
                    cd.Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    cd.Width = new GridLength(0);
                }
            }
        }

        private void Border_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.DataContext is ColumnControlViewModel ccvm)
            {
                if (this.DataContext is ColumnManagerViewModel cmvm)
                {
                    cmvm.ClearFocused();
                }
                ccvm.ContainsKeyboardFocus = true;
            }
        }

        /// <summary>
        /// Make sure the column visibility is correct once loaded, but only do it once.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnManagerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ColumnManagerView_OnLoaded;
            var args = new DependencyPropertyChangedEventArgs();
            Column_OnIsVisibleChanged(Column1View, args);
            Column_OnIsVisibleChanged(Column2View, args);
            Column_OnIsVisibleChanged(Column3View, args);
            Column_OnIsVisibleChanged(Column4View, args);
        }
    }
}
