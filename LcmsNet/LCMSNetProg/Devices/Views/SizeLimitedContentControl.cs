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
        public SizeLimitedContentControl()
        {
            AvailableSize = new Size(0, 0);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // ScrollViewer: always passes Size(Infinity, Infinity) to contents; we don't quite want that.
            var newSize = AvailableSize;
            var minWidth = 0.0;
            var minHeight = 0.0;
            if (Content is FrameworkElement fe)
            {
                minWidth = fe.MinWidth;
                minHeight = fe.MinHeight;
            }

            if (AvailableSize.Width < minWidth || AvailableSize.Height < minHeight)
            {
                newSize = new Size(minWidth, minHeight);
            }

            // Re-trigger a measure of the child elements
            return base.MeasureOverride(newSize);
        }

        public Size AvailableSize { get; set; }
    }
}
