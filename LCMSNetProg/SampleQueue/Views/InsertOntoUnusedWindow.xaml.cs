using System.Windows;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for InsertOntoUnusedWindow.xaml
    /// </summary>
    public partial class InsertOntoUnusedWindow : Window
    {
        public InsertOntoUnusedWindow()
        {
            InitializeComponent();
        }

        private void Insert_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (sender is Button btn && btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
            //{
            //    btn.Command.Execute(btn.CommandParameter);
            //}
            this.Close();
        }

        private void Append_OnClick(object sender, RoutedEventArgs e)
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
