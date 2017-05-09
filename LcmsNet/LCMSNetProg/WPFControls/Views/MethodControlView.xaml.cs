using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for MethodControlView.xaml
    /// </summary>
    public partial class MethodControlView : UserControl
    {
        public MethodControlView()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Background = Brushes.White;
            }
            if (!CommandsGrid.IsVisible)
            {
                GridButtonRow.Height = new GridLength(0);
            }
        }

        private void UIElement_OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ColumnControlViewModel sccvm)
            {
                sccvm.ContainsKeyboardFocus = this.IsKeyboardFocusWithin;
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
