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

namespace Agilent.Devices.Pumps
{
    /// <summary>
    /// Interaction logic for AgilentPumpPurgeWindow.xaml
    /// </summary>
    public partial class AgilentPumpPurgeWindow : Window
    {
        public AgilentPumpPurgeWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
