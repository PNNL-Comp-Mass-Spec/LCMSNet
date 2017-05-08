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

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for SampleColumnControl.xaml
    /// </summary>
    public partial class SampleColumnControlView : UserControl
    {
        public SampleColumnControlView()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Background = Brushes.White;
            }
        }
    }
}
