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

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for InsertOntoUnusedWindow.xaml
    /// </summary>
    public partial class InsertOntoUnusedWindow : Window
    {
        public InsertOntoUnusedWindow()
        {
            InitializeComponent();
        }

        private void Insert_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }

        private void Append_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }
    }
}
