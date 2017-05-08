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

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for MoveToColumnSelectorWindow.xaml
    /// </summary>
    public partial class MoveToColumnSelectorWindow : Window
    {
        public MoveToColumnSelectorWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            if (sender is Button btn)
            {
                if (btn.IsCancel)
                {
                    this.DialogResult = false;
                }
                //if (btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
                //{
                //    btn.Command.Execute(btn.CommandParameter);
                //}
            }
            this.Close();
        }
    }
}
