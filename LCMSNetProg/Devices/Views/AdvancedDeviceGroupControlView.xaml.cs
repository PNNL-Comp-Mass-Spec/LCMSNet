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

namespace LcmsNet.Devices.Views
{
    /// <summary>
    /// Interaction logic for AdvancedDeviceGroupControlView.xaml
    /// </summary>
    public partial class AdvancedDeviceGroupControlView : UserControl
    {
        public AdvancedDeviceGroupControlView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is ScrollViewer sv) || !(sv.Content is SizeLimitedContentControl slcc))
            {
                return;
            }

            // ScrollViewer passes infinite size to content controls; we are doing this to get the desired behavior
            // Give the new size to the ScrollViewer content, and re-measure it.
            slcc.AvailableSize = e.NewSize;
            slcc.InvalidateMeasure();

            // Re-measure the ScrollViewer to determine if scroll bars are needed.
            sv.InvalidateMeasure();
        }
    }
}
