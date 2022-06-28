using System.Reflection;
using System.Windows;

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            CopyrightText.Text = App.SOFTWARE_COPYRIGHT;
            Developers.Text = App.SOFTWARE_DEVELOPERS;
            VersionText.Text = Assembly.GetEntryAssembly().GetName().Version.ToString(3);
        }
    }
}
