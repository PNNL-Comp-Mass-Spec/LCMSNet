using System.Windows;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for MoveToMethodSelectorWindow.xaml
    /// </summary>
    public partial class MoveToMethodSelectorWindow : Window
    {
        public MoveToMethodSelectorWindow()
        {
            InitializeComponent();
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

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }
    }
}
