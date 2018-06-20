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

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
