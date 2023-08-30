using System.Windows;
using System.Windows.Controls;
using LcmsNet.SampleQueue.ViewModels;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for ColumnControlView.xaml
    /// </summary>
    public partial class ColumnControlView : UserControl
    {
        public ColumnControlView()
        {
            InitializeComponent();
            if (!CommandsGrid.IsVisible)
            {
                GridButtonRow.Height = new GridLength(0);
            }

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

        private void UIElement_OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ColumnControlViewModel ccvm)
            {
                ccvm.ContainsKeyboardFocus = this.IsKeyboardFocusWithin;
            }
        }

        /// <summary>
        /// Hides the grid row for the commands according to the set visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Grid g)
            {
                if (g.IsVisible)
                {
                    GridButtonRow.Height = new GridLength(70);
                }
                else
                {
                    GridButtonRow.Height = new GridLength(0);
                }
            }
        }
    }
}
