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

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for ConvertToolWindow.xaml
    /// </summary>
    public partial class ConvertToolWindow : Window
    {
        public ConvertToolWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
