using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LcmsNet.SampleQueue.Views
{
    public class ColorInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return InvertColor(c);
            }

            if (value is SolidColorBrush b)
            {
                return new SolidColorBrush(InvertColor(b.Color));
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        private Color InvertColor(Color c)
        {
            // Cast to byte truncates values above 255, basically performing "mod 256"
            return Color.FromArgb(c.A, (byte)(c.R + 128), (byte)(c.G + 128), (byte)(c.B + 128));
        }
    }
}
