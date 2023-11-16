using System;
using System.Collections.Generic;
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

namespace LcmsNetPlugins.ZaberStage.UI
{
    /// <summary>
    /// Interaction logic for StageControlView.xaml
    /// </summary>
    public partial class StageControlView : UserControl
    {
        public StageControlView()
        {
            InitializeComponent();
        }

        private void Move_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StageControlViewModel scvm)
            {
                scvm.StopAxis();
            }
        }

        private void Move_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is StageControlViewModel scvm)
            {
                scvm.StopAxis();
            }
        }
    }
}
