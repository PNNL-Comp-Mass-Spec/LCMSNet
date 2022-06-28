using System.Windows;
using System.Windows.Controls;
using LcmsNet.Notification.ViewModels;

namespace LcmsNet.Notification.Views
{
    /// <summary>
    /// Interaction logic for NotificationSystemView.xaml
    /// </summary>
    public partial class NotificationSystemView : UserControl
    {
        public NotificationSystemView()
        {
            InitializeComponent();
        }

        private void EventsListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Properly trigger an update when the listbox in focus is changed
            if (e.Source is ListBox lb && DataContext is NotificationSystemViewModel nsvm)
            {
                if (lb.Items.Count == 1 && lb.Items[0] is string singleItem)
                {
                    nsvm.SelectedEvent = singleItem;
                }
                if (lb.SelectedItem is string selected)
                {
                    nsvm.SelectedEvent = selected;
                }
            }
        }
    }
}
