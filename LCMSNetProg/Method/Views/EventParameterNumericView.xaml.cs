using System;
using System.Windows;
using System.Windows.Controls;
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
                var conversion = new ConvertToolWindow
                {
                    DataContext = conversionVM,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Owner = Window.GetWindow(this)
                };

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
