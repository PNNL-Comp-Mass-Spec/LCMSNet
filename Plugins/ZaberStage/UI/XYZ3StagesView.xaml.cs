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
    /// Interaction logic for XYZ3StagesView.xaml
    /// </summary>
    public partial class XYZ3StagesView : UserControl
    {
        public XYZ3StagesView()
        {
            InitializeComponent();
        }

        private void StageControls_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (DataContext is XYZ3StagesViewModel xyz)
            {
                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    xyz.XStageVM.StopAxis();
                }

                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    xyz.YStageVM.StopAxis();
                }

                if (e.Key == Key.Add || e.Key == Key.Subtract)
                {
                    xyz.ZStageVM.StopAxis();
                }
            }
        }
    }
}
