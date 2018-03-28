using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace LcmsNetPlugins.VICI.Devices.Valves
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
