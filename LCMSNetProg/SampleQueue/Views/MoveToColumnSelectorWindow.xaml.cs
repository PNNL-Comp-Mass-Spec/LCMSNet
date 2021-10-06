using System.Windows;
using System.Windows.Controls;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Interaction logic for MoveToColumnSelectorWindow.xaml
    /// </summary>
    public partial class MoveToColumnSelectorWindow : Window
    {
        public MoveToColumnSelectorWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            if (sender is Button btn)
            {
                if (btn.IsCancel)
                {
                    this.DialogResult = false;
                }
                //if (btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
                //{
                //    btn.Command.Execute(btn.CommandParameter);
                //}
            }
            this.Close();
        }
    }
}
