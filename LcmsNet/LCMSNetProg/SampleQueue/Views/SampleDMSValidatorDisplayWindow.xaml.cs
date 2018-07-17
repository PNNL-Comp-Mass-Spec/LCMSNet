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
using System.Windows.Shapes;
using LcmsNet.SampleQueue.ViewModels;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for SampleDMSValidatorDisplayWindow.xaml
    /// </summary>
    public partial class SampleDMSValidatorDisplayWindow : Window
    {
        public SampleDMSValidatorDisplayWindow()
        {
            InitializeComponent();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Provide a version of "SelectedItems" one-way-to-source binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is SampleDMSValidatorDisplayViewModel dc && sender is ListView selector)
            {
                dc.SelectedItems.Clear();
                dc.SelectedItems.AddRange(selector.SelectedItems.Cast<SampleDMSValidationViewModel>());
            }
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is Control c))
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        MoveFocus(c, FocusNavigationDirection.Up);
                    }
                    else
                    {
                        MoveFocus(c, FocusNavigationDirection.Down);
                    }
                    break;
                case Key.Up:
                case Key.VolumeUp:
                    MoveFocus(c, FocusNavigationDirection.Up);
                    break;
                case Key.Down:
                case Key.VolumeDown:
                    MoveFocus(c, FocusNavigationDirection.Down);
                    break;
                case Key.Right:
                    MoveFocus(c, FocusNavigationDirection.Next);
                    break;
                case Key.Left:
                    MoveFocus(c, FocusNavigationDirection.Previous);
                    break;
            }
        }

        private void MoveFocus(Control c, FocusNavigationDirection direction)
        {
            var success = c.MoveFocus(new TraversalRequest(direction));
            if (success && (direction == FocusNavigationDirection.Up || direction == FocusNavigationDirection.Down))
            {
                var name = c.Name;
                var x = SampleListView.SelectedItem;
                var item = SampleListView.ItemContainerGenerator.ContainerFromItem(x) as ListViewItem;
                var target = GetDescendantByType(item, c.GetType(), name);
                if (target != null && target is Control c2)
                {
                    c2.Focus();
                }
            }
        }

        private Visual GetDescendantByType(Visual element, Type type, string name)
        {
            if (element == null) return null;
            if (element.GetType() == type)
            {
                FrameworkElement fe = element as FrameworkElement;
                if (fe != null)
                {
                    if (fe.Name == name)
                    {
                        return fe;
                    }
                }
            }
            if (element is FrameworkElement frameworkElement)
            {
                frameworkElement.ApplyTemplate();
            }
            Visual foundElement = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type, name);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }
    }
}
