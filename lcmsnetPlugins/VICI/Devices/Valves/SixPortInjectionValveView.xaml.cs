using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace LcmsNetPlugins.VICI.Devices.Valves
{
    /// <summary>
    /// Interaction logic for SixPortInjectionValveView.xaml
    /// </summary>
    public partial class SixPortInjectionValveView : UserControl
    {
        public SixPortInjectionValveView()
        {
            InitializeComponent();
        }

        private void PropertyGrid_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (this.DataContext is SixPortInjectionValveViewModel v)
            {
                v.OnSaveRequired();
            }
        }
    }
}
