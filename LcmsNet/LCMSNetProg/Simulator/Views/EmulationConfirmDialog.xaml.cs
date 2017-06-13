using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;

namespace LcmsNet.Simulator.Views
{
    /// <summary>
    /// Interaction logic for EmulationConfirmDialog.xaml
    /// </summary>
    public partial class EmulationConfirmDialog : Window
    {
        public EmulationConfirmDialog()
        {
            InitializeComponent();
            Icon = SystemIcons.Warning.ToBitmap().ToBitmapImage();
        }

        public MessageBoxResult Result { get; private set; }

        private void EnableEmulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void DoNotEnableEmulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void CancelSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Result = MessageBoxResult.Cancel;
            this.Close();
        }
    }
}
