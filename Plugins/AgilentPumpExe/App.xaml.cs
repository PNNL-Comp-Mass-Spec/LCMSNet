using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using LcmsNetData.Logging;

namespace AgilentPumpExe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            // Kill the extra logging thread
            ApplicationLogger.ShutDownLogging();

            var mainVm = new MainWindowViewModel();
            MainWindow = new MainWindow() { DataContext = mainVm };

            try
            {
                MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unhandled Exception, Closing", MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow.Close();
            }
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            // TODO: This is the best place to notify about exceptions, but catches others we don't want to worry about. It also doesn't let us say that it has been handled.
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            if (e.Exception.GetType() == typeof(ReactiveUI.UnhandledErrorException) && e.Exception.InnerException != null)
            {
                ex = e.Exception.InnerException;
            }
            MessageBox.Show(ex.ToString(), "Unhandled Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}
