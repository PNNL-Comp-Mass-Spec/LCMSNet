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
using LcmsNetSDK.Method;

namespace LcmsNet.Reporting
{
    /// <summary>
    /// Interaction logic for CreateErrorReportWindow.xaml
    /// </summary>
    public partial class CreateErrorReportWindow : Window
    {
        public CreateErrorReportWindow()
        {
            InitializeComponent();
        }

        private void MethodList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as CreateErrorReportViewModel;
            if (dc == null)
            {
                return;
            }

            var selector = sender as ListBox;
            if (selector == null)
            {
                return;
            }

            using (dc.LCMethodsSelected.SuspendNotifications())
            {
                dc.LCMethodsSelected.Clear();
                if (selector.SelectedItems.Count == 0)
                {
                    return;
                }
                if (selector.SelectedItems.Count == 1)
                {
                    dc.LCMethodsSelected.Add((LCMethod)selector.SelectedItems[0]);
                    return;
                }
                dc.LCMethodsSelected.AddRange(selector.SelectedItems.Cast<LCMethod>());
            }
        }

        private void CreateErrorReportWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is CreateErrorReportViewModel)
            {
                MethodList.SelectAll();
            }
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Create_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CreateErrorReportWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            MethodList.SelectAll();
        }
    }
}
