using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace LcmsNet.Devices.Valves
{
    /// <summary>
    /// Interaction logic for ValveVICIMultiPosView.xaml
    /// </summary>
    public partial class ValveVICIMultiPosView : UserControl
    {
        public ValveVICIMultiPosView()
        {
            InitializeComponent();
        }

        private void PropertyGrid_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (this.DataContext is ValveVICIMultiPosViewModel v)
            {
                v.OnSaveRequired();
            }
        }
    }
}
