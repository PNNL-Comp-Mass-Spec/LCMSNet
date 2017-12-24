using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using LcmsNetDataClasses.Logging;

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for DynamicSplashScreenWindow.xaml
    /// </summary>
    public partial class DynamicSplashScreenWindow : Window
    {
        public DynamicSplashScreenWindow()
        {
            InitializeComponent();
            Version.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Updating at the highest priority, to make sure stuff is shown.
        /// </summary>
        public string Status
        {
            set
            {
                try
                {
                    Dispatcher.Invoke(() => StatusText.Text = value);
                }
                catch (Exception)
                {
                    classApplicationLogger.LogMessage(0, "Could not update splash screen status. Message: " + value);
                }
            }
        }

        public string SoftwareCopyright
        {
            set { Dispatcher.Invoke(() => Copyright.Text = value); }
        }

        public string SoftwareDevelopers
        {
            set { Dispatcher.Invoke(() => Developers.Text = value); }
        }

        public void SetEmulatedLabelVisibility(string cartName, bool visible)
        {
            if (Dispatcher == null || Dispatcher.CheckAccess())
            {
                if (visible)
                {
                    Cart.Foreground = Brushes.Red;
                    Cart.Text = cartName + "\n [EMULATED] ";
                }
                else
                {
                    Cart.Text = cartName;
                }
            }
            else
            {
                Dispatcher.Invoke(() => SetEmulatedLabelVisibility(cartName, visible));
            }
        }

        public async void LoadComplete()
        {
            // Wait for any updates to complete, to avoid thread cancellation exceptions
            await Dispatcher.Yield();
            Dispatcher.InvokeShutdown();
        }
    }
}
