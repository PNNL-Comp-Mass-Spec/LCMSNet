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
using System.Windows.Threading;
using FluidicsSDK;
using LcmsNet.Devices.Pumps.Views;
using LcmsNetCommonControls.Views;

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
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // We only need to do this once.
            FluidicsModerator.Moderator.DrawingScaleFactor = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            Loaded -= OnLoaded;
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
                if (PumpsTab.IsSelected)
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

        private void ReportError_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel mwvm))
            {
                return;
            }

            // Force all tabs to render first, if they haven't already
            var currentTab = MainTabControl.SelectedItem;
            QueueTab.ForceLoad();
            FluidicsDesignTab.ForceLoad();
            ProgressTab.ForceLoad();
            MethodsTab.ForceLoad();
            PumpsTab.ForceLoad();
            NotificationsTab.ForceLoad();
            MessagesTab.ForceLoad();
            ConfigTab.ForceLoad();

            MainTabControl.SelectedItem = currentTab;

            var controls = new List<ContentControl>();
            if (PumpPopout.Content is PumpDisplaysView pdv)
            {
                controls.Add(pdv);
            }
            else if (Application.Current != null)
            {
                PopoutWindow existing = null;
                foreach (var window in Application.Current.Windows)
                {
                    if (window is PopoutWindow pw && pw.Title.Equals(PumpPopout.Title))
                    {
                        existing = pw;
                    }
                }
                if (existing != null && existing.Content is PumpDisplaysView pdv2)
                {
                    controls.Add(pdv2);
                }
            }
            controls.Add(MessagesPage);
            controls.Add(MethodEditorPage);
            controls.Add(NotificationsPage);
            controls.Add(SampleProgressPage);
            controls.Add(SampleManagerPage);

            mwvm.ReportErrorCommand.Execute(controls.ToArray()).Subscribe();
        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        /// <summary>
        /// Forces the UIElement to render
        /// </summary>
        /// <param name="uiElement"></param>
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        /// <summary>
        /// Sets tab selection to true, and forces it to render.
        /// </summary>
        /// <param name="tab"></param>
        public static void ForceLoad(this TabItem tab)
        {
            tab.IsSelected = true;
            tab.Refresh();
        }
    }
}
