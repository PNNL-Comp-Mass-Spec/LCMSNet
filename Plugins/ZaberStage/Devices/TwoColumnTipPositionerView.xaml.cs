using LcmsNetPlugins.ZaberStage.UI;
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

namespace LcmsNetPlugins.ZaberStage.Devices
{
    /// <summary>
    /// Interaction logic for TwoColumnTipPositionerView.xaml
    /// </summary>
    public partial class TwoColumnTipPositionerView : UserControl
    {
        public TwoColumnTipPositionerView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TwoColumnTipPositionerViewModel xyzOld)
            {
                xyzOld.DeviceControlsOutOfView -= OnDeviceOutOfView;
            }

            if (e.NewValue is TwoColumnTipPositionerViewModel xyz)
            {
                xyz.DeviceControlsOutOfView += OnDeviceOutOfView;
            }
        }

        private void OnDeviceOutOfView(object sender, EventArgs e)
        {
            KeyControlEnabled.IsChecked = false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is TwoColumnTipPositionerViewModel xyz && !e.IsRepeat)
            {
                if (e.Key == Key.Left)
                {
                    ((ICommand)xyz.XStageVM.DecJogCommand).Execute(null);
                }
                else if (e.Key == Key.Right)
                {
                    ((ICommand)xyz.XStageVM.IncJogCommand).Execute(null);
                }

                if (e.Key == Key.Down)
                {
                    ((ICommand)xyz.YStageVM.DecJogCommand).Execute(null);
                }
                else if (e.Key == Key.Up)
                {
                    ((ICommand)xyz.YStageVM.IncJogCommand).Execute(null);
                }

                if (e.Key == Key.Subtract)
                {
                    ((ICommand)xyz.ZStageVM.DecJogCommand).Execute(null);
                }
                else if (e.Key == Key.Add)
                {
                    ((ICommand)xyz.ZStageVM.IncJogCommand).Execute(null);
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (DataContext is TwoColumnTipPositionerViewModel xyz)
            {
                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    ((ICommand)xyz.XStageVM.StopCommand).Execute(null);
                }

                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    ((ICommand)xyz.YStageVM.StopCommand).Execute(null);
                }

                if (e.Key == Key.Add || e.Key == Key.Subtract)
                {
                    ((ICommand)xyz.ZStageVM.StopCommand).Execute(null);
                }
            }
        }

        private void KeyControl_OnChecked(object sender, RoutedEventArgs e)
        {
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            var w = Window.GetWindow(this);
            if (w != null)
            {
                w.KeyDown += OnKeyDown;
                w.KeyUp += OnKeyUp;
            }
        }

        private void KeyControl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            KeyDown -= OnKeyDown;
            KeyUp -= OnKeyUp;

            var w = Window.GetWindow(this);
            if (w != null)
            {
                w.KeyDown -= OnKeyDown;
                w.KeyUp -= OnKeyUp;
            }
        }
    }
}
