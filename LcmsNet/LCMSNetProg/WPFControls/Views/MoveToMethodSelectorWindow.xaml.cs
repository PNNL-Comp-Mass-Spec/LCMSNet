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
    /// Interaction logic for MoveToMethodSelectorWindow.xaml
    /// </summary>
    public partial class MoveToMethodSelectorWindow : Window
    {
        public MoveToMethodSelectorWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }
    }
}
