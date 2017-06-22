using System.Windows;
using System.Windows.Controls;

namespace LcmsNet.Devices.Views
{
    /// <summary>
    /// Inherits ContentControl to limit the requested render size (measure) to the minimum required by the content
    /// This prevents a nested scroll viewer from disappearing in favor of a wrapping scroll viewer
    /// </summary>
    public class SizeLimitedContentControl : ContentControl
    {
        protected override Size MeasureOverride(Size constraint)
        {
            var minWidth = 0.0;
            var minHeight = 0.0;
            if (Content is FrameworkElement fe)
            {
                minWidth = fe.MinWidth;
                minHeight = fe.MinHeight;
            }

            var minSize = new Size(minWidth, minHeight);

            base.MeasureOverride(minSize);

            return minSize;
        }
    }
}
