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
    /// Interaction logic for X1StageView.xaml
    /// </summary>
    public partial class X1StageView : UserControl
    {
        public X1StageView()
        {
            InitializeComponent();
        }

        private void StageControls_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (DataContext is X1StageViewModel xyz)
            {
                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    xyz.XStageVM.StopAxis();
                }
            }
        }
    }
}
