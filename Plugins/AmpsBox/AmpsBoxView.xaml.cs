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
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace AmpsBox
{
    /// <summary>
    /// Interaction logic for AmpsBoxView.xaml
    /// </summary>
    public partial class AmpsBoxView : UserControl
    {
        public AmpsBoxView()
        {
            InitializeComponent();
        }

        private void SerialPortPropertyGrid_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (this.DataContext is AmpsBoxViewModel abvm)
            {
                abvm.OnSaveRequired();
            }
        }
    }
}
