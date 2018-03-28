using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LcmsNet.Devices.ViewModels;
using LcmsNetSDK.Devices;

namespace LcmsNet.Devices.Views
{
    /// <summary>
    /// Interaction logic for DeviceAddWindow.xaml
    /// </summary>
    public partial class DeviceAddWindow : Window
    {
        public DeviceAddWindow()
        {
            InitializeComponent();
        }

        private void ListboxDevices_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.DataContext is DeviceAddViewModel davm)
            {
                davm.RemoveCommand.Execute();
            }
        }

        private void TreeViewAvailable_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is DeviceAddViewModel davm)
            {
                davm.AddCommand.Execute();
            }
        }

        private void TreeViewAvailable_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.DataContext is DeviceAddViewModel davm)
            {
                davm.AddCommand.Execute();
            }
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

        private void ListboxDevices_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as DeviceAddViewModel;
            if (dc == null)
            {
                return;
            }

            var selector = sender as ListBox;
            if (selector == null)
            {
                return;
            }

            using (dc.SelectedPlugins.SuppressChangeNotifications())
            {
                dc.SelectedPlugins.Clear();
                if (selector.SelectedItems.Count == 0)
                {
                    return;
                }
                if (selector.SelectedItems.Count == 1)
                {
                    dc.SelectedPlugins.Add((DevicePluginInformation)selector.SelectedItems[0]);
                    return;
                }
                dc.SelectedPlugins.AddRange(selector.SelectedItems.Cast<DevicePluginInformation>());
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var dc = this.DataContext as DeviceAddViewModel;
            if (dc == null)
            {
                return;
            }

            dc.SelectedPlugin = e.NewValue as DevicePluginInformation;
        }
    }
}
