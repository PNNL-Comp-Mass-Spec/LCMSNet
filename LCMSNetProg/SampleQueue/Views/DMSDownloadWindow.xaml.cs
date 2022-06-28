using System.Windows;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for DMSDownloadWindow.xaml
    /// </summary>
    public partial class DMSDownloadWindow : Window
    {
        public DMSDownloadWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }
    }
}
