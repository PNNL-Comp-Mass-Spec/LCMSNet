using System.Windows;

namespace LcmsNetPlugins.Agilent.Pumps
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

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
