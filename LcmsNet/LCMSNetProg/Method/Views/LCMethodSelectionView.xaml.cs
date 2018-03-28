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
using LcmsNet.Method.ViewModels;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for LCMethodSelectionView.xaml
    /// </summary>
    public partial class LCMethodSelectionView : UserControl
    {
        public LCMethodSelectionView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Provide a version of "SelectedItems" one-way-to-source binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MethodList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as LCMethodSelectionViewModel;
            if (dc == null)
            {
                return;
            }

            var selector = sender as ListBox;
            if (selector == null)
            {
                return;
            }

            using (dc.SelectedListLCMethods.SuppressChangeNotifications())
            {
                dc.SelectedListLCMethods.Clear();
                if (selector.SelectedItems.Count == 0)
                {
                    return;
                }
                if (selector.SelectedItems.Count == 1)
                {
                    dc.SelectedListLCMethods.Add((classLCMethod)selector.SelectedItems[0]);
                    return;
                }
                dc.SelectedListLCMethods.AddRange(selector.SelectedItems.Cast<classLCMethod>());
            }
        }

        private void ComboBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.Key != Key.Return)
            {
                return;
            }

            var dc = this.DataContext as LCMethodSelectionViewModel;

            dc?.AddCommand.Execute().Subscribe();
        }
    }
}
