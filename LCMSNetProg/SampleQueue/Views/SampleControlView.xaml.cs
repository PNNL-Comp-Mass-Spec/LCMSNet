using System.Windows;
using System.Windows.Controls;
using LcmsNet.UIHelpers;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for SampleControlView.xaml
    /// </summary>
    public partial class SampleControlView : UserControl, IProvidesWindowReference
    {
        public SampleControlView()
        {
            InitializeComponent();

            // Can't get the containing window until the control is loaded.
            Loaded += (sender, args) => InWindow = Window.GetWindow(this);
        }

        /// <summary>
        /// Reference to the window that contains this view for context menu commands
        /// </summary>
        public Window InWindow
        {
            get => (Window)GetValue(InWindowProperty);
            set => SetValue(InWindowProperty, value);
        }

        // Using a DependencyProperty as the backing store for InWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InWindowProperty =
            DependencyProperty.Register("InWindow", typeof(Window), typeof(SampleControlView), new PropertyMetadata(null));

        private void ButtonSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            SampleView.SampleGrid.SelectAll();
        }
    }
}
