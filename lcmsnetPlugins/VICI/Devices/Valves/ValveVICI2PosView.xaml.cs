using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace LcmsNetPlugins.VICI.Devices.Valves
{
    /// <summary>
    /// Interaction logic for ValveVICI2PosView.xaml
    /// </summary>
    public partial class ValveVICI2PosView : UserControl
    {
        public ValveVICI2PosView()
        {
            InitializeComponent();
        }

        private void PropertyGrid_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (this.DataContext is ValveVICI2PosViewModel v)
            {
                v.OnSaveRequired();
            }
        }
    }
}
