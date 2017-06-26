using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            if (this.DataContext is MainWindowViewModel mwvm)
            {
                mwvm.Shutdown();
            }
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (PumpTab.IsSelected)
                {
                    PumpPopout.TryActivateWindow();
                }
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //string msg = "Hey, man. Some dude said to close this here durned program. " + Environment.NewLine +
            //             "Y'all should check ta be shore no samples is runnin' afore closin' things up." + Environment.NewLine +
            //             Environment.NewLine + "Is you'uns sure ya wants ta do this?";
            var msg = "Application shutdown requested. If samples are running, data may be lost" +
                      Environment.NewLine +
                      Environment.NewLine + "Are you sure you want to shut down?";
            var result = MessageBox.Show(msg, "Closing Application", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}
