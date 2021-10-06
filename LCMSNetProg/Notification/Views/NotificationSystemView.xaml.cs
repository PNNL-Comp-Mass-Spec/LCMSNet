using System;
using System.Collections.Generic;
using System.Linq;
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
