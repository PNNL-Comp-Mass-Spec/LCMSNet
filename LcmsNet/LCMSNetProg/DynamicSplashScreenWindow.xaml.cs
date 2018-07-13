using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using LcmsNetData;
using LcmsNetData.Logging;

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for DynamicSplashScreenWindow.xaml
    /// </summary>
    public partial class DynamicSplashScreenWindow : Window, INotifyPropertyChangedExt
    {
        private string status;
        private bool isEmulated;
        private string cartName;

        public DynamicSplashScreenWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Synch to the logger so we can display any messages coming back from the rest of the program and interface.
            ApplicationLogger.Message += ApplicationLogger_ItemLogged;
            ApplicationLogger.Error += ApplicationLogger_ItemLogged;

            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Status = "Loading...";
            SoftwareCopyright = "Copyright Battelle Memorial Institute, 2017";
            SoftwareDevelopers = "Developers: Populated at runtime";
            CartName = "Cart";
        }

        public DynamicSplashScreenWindow(string copyright, string developers) : this()
        {
            SoftwareCopyright = copyright;
            SoftwareDevelopers = developers;
        }

        public string Version { get; }
        public string SoftwareCopyright { get; }
        public string SoftwareDevelopers { get; }

        /// <summary>
        /// Updating at the highest priority, to make sure stuff is shown.
        /// </summary>
        public string Status
        {
            get => status;
            private set => this.RaiseAndSetIfChanged(ref status, value);
        }

        public bool IsEmulated
        {
            get => isEmulated;
            private set => this.RaiseAndSetIfChanged(ref isEmulated, value);
        }

        public string CartName
        {
            get => cartName;
            private set => this.RaiseAndSetIfChanged(ref cartName, value);
        }

        public void SetEmulatedLabelVisibility(string cartNameStr, bool visible)
        {
            if (Dispatcher == null || Dispatcher.CheckAccess())
            {
                IsEmulated = visible;
                if (visible)
                {
                    CartName = cartNameStr + "\n [EMULATED] ";
                }
                else
                {
                    CartName = cartNameStr;
                }
            }
            else
            {
                Dispatcher.Invoke(() => SetEmulatedLabelVisibility(cartNameStr, visible));
            }
        }

        /// <summary>
        /// Updates the splash screen with the appropriate messages.
        /// </summary>
        /// <param name="messageLevel">Filter for displaying messages.</param>
        /// <param name="args">Messages and other arguments passed from the sender.</param>
        void ApplicationLogger_ItemLogged(int messageLevel, MessageLoggerArgs args)
        {
            try
            {
                if (messageLevel < 1)
                {
                    Dispatcher.Invoke(() => Status = args.Message, DispatcherPriority.Send);
                }
            }
            catch (Exception)
            {
                ApplicationLogger.LogMessage(0, "Could not update splash screen status. Message: " + args.Message);
            }
        }

        public async void LoadComplete()
        {
            // Wait for any updates to complete, to avoid thread cancellation exceptions
            await Dispatcher.Yield();
            ApplicationLogger.Message -= ApplicationLogger_ItemLogged;
            ApplicationLogger.Error -= ApplicationLogger_ItemLogged;
            await Dispatcher.Yield();
            Dispatcher.Invoke(Close);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
