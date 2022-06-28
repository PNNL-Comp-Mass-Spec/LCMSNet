using System.Windows;
using System.Windows.Controls;

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
