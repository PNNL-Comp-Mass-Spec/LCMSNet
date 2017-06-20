﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace LcmsNet.Devices.Valves
{
    /// <summary>
    /// Interaction logic for ValveVICIMultiPosView.xaml
    /// </summary>
    public partial class ValveVICIMultiPosView : UserControl
    {
        public ValveVICIMultiPosView()
        {
            InitializeComponent();
        }

        public SerialPort SerialPortBinding
        {
            get { return (SerialPort)GetValue(SerialPortBindingProperty); }
            set { SetValue(SerialPortBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SerialPortBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SerialPortBindingProperty =
            DependencyProperty.Register("SerialPortBinding", typeof(SerialPort), typeof(ValveVICIMultiPosView), new PropertyMetadata(null, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is ValveVICIMultiPosView v))
            {
                return;
            }
            v.WinFormsPropertyGrid.SelectedObject = dependencyPropertyChangedEventArgs.NewValue;
        }

        private void PropertyGrid_OnPropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            if (this.DataContext is ValveVICIMultiPosViewModel v)
            {
                v.OnSaveRequired();
            }
        }

        private void ValveVICIMultiPosView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ValveVICIViewModelBase vm)
            {
                var binding = new Binding("ComPort");
                binding.Source = e.NewValue;
                this.SetBinding(SerialPortBindingProperty, binding);
            }
        }
    }
}