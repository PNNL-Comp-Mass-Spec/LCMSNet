using System.Globalization;
using System.Windows;

namespace LcmsNetData
{
    /// <summary>
    /// Utility functions for WPF, particularly for conversions from WinForms formatting values to WPF formatting values
    /// </summary>
    public static class WpfConversions
    {
        private static readonly LengthConverter LengthConverter = new LengthConverter();

        /// <summary>
        /// Converts a size, like what can be specified in xaml, to the pixel size.
        /// </summary>
        /// <param name="srcLength">Desired length, with units. Format should be like "8pt" (no space), supported units are 'px', 'in', 'cm', and 'pt', no units assumes 'px'.</param>
        /// <returns></returns>
        public static double GetWpfLength(string srcLength)
        {
            // Could also use FontSizeConverter, which converts
            return (double)LengthConverter.ConvertFrom(null, CultureInfo.InvariantCulture, srcLength);
        }
    }
}
