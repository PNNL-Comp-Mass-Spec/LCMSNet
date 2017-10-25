using System.Windows;

namespace ASIpumpTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Event handler that is triggered on application start up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = new AsiTestWindow();
            var mainWindowVm = new AsiTestViewModel();
            mainWindow.DataContext = mainWindowVm;
            mainWindow.Show();
        }
    }
}
