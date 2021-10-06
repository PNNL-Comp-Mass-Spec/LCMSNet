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
using LcmsNet.SampleQueue.ViewModels;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for TrayVialView.xaml
    /// </summary>
    public partial class TrayVialView : UserControl
    {
        public TrayVialView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as TrayVialViewModel;
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
            dc.SelectedSamples.AddRange(selector.SelectedItems.Cast<TrayVialSampleViewModel>());
        }
    }
}
