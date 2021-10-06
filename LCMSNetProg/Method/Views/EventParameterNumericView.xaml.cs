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
using LcmsNet.Method.ViewModels;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for EventParameterNumericView.xaml
    /// </summary>
    public partial class EventParameterNumericView : UserControl
    {
        public EventParameterNumericView()
        {
            InitializeComponent();
        }

        private void ConversionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EventParameterNumericViewModel epnvm)
            {
                var conversionVM = new ConvertToolViewModel(Convert.ToInt32(epnvm.NumberValue), epnvm.DecimalPlaces);
                var conversion = new ConvertToolWindow();
                conversion.DataContext = conversionVM;

                conversion.WindowStartupLocation = WindowStartupLocation.Manual;
                var pos = PointToScreen(ConversionButton.TranslatePoint(new Point(0, 0), this));
                conversion.Left = pos.X;
                conversion.Top = pos.Y;

                var result = conversion.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    switch (conversionVM.ConversionType)
                    {
                        case ConversionType.Time:
                            epnvm.NumberValue = Math.Min(epnvm.NumberMaximum, Math.Max(epnvm.NumberMinimum, conversionVM.TotalSeconds));
                            break;
                        case ConversionType.Precision:
                            epnvm.DecimalPlaces = conversionVM.DecimalPlaces;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
