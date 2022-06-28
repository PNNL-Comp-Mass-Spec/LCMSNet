using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using LcmsNet.Method.ViewModels;
using LcmsNetSDK.Method;

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
            if (!(this.DataContext is LCMethodSelectionViewModel dc))
            {
                return;
            }

            if (!(sender is ListBox selector))
            {
                return;
            }

            using (dc.SelectedListLCMethods.SuspendNotifications())
            {
                dc.SelectedListLCMethods.Clear();
                if (selector.SelectedItems.Count == 0)
                {
                    return;
                }
                if (selector.SelectedItems.Count == 1)
                {
                    dc.SelectedListLCMethods.Add((LCMethod)selector.SelectedItems[0]);
                    return;
                }
                dc.SelectedListLCMethods.AddRange(selector.SelectedItems.Cast<LCMethod>());
            }
        }

        private void ComboBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.Key != Key.Return)
            {
                return;
            }

            if (this.DataContext is LCMethodSelectionViewModel dc)
            {
                dc.AddCommand.Execute().Subscribe();
            }
        }
    }
}
