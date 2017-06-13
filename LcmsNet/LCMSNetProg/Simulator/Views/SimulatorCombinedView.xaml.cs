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
using LcmsNet.Simulator.ViewModels;
using ReactiveUI;

namespace LcmsNet.Simulator.Views
{
    /// <summary>
    /// Interaction logic for SimulatorCombinedView.xaml
    /// </summary>
    public partial class SimulatorCombinedView : UserControl
    {
        public SimulatorCombinedView()
        {
            InitializeComponent();
        }

        private void SimulatorCombinedView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is SimulatorCombinedViewModel scvm)
            {
                scvm.ConfigPopoutVm.WhenAnyValue(x => x.Tacked).Subscribe(x => ConfigTackedChanged(x));
                scvm.ControlsPopoutVm.WhenAnyValue(x => x.Tacked).Subscribe(x => ControlsTackedChanged(x));
            }
        }

        private double lastCombinedConfigWidth = 1;
        private double lastCombinedControlsWidth = 2.2;
        private bool configWidthAuto = false;
        private bool controlsWidthAuto = false;

        private void ConfigTackedChanged(bool newTackedValue)
        {
            if (newTackedValue)
            {
                ConfigColumn.Width = new GridLength(1, GridUnitType.Star);
                configWidthAuto = false;
                RestoreColumnWidths();
            }
            else
            {
                StoreColumnWidths();
                ConfigColumn.Width = GridLength.Auto;
                configWidthAuto = true;
            }
        }

        private void ControlsTackedChanged(bool newTackedValue)
        {
            if (newTackedValue)
            {
                ControlsColumn.Width = new GridLength(2.2, GridUnitType.Star);
                controlsWidthAuto = false;
                RestoreColumnWidths();
            }
            else
            {
                StoreColumnWidths();
                ControlsColumn.Width = GridLength.Auto;
                controlsWidthAuto = true;
            }
        }

        private void StoreColumnWidths()
        {
            if (!configWidthAuto && !controlsWidthAuto)
            {
                lastCombinedConfigWidth = ConfigColumn.Width.Value;
                lastCombinedControlsWidth = ControlsColumn.Width.Value;
                Splitter.Visibility = Visibility.Collapsed;
            }
        }

        private void RestoreColumnWidths()
        {
            if (!configWidthAuto && !controlsWidthAuto)
            {
                ConfigColumn.Width = new GridLength(lastCombinedConfigWidth, GridUnitType.Star);
                ControlsColumn.Width = new GridLength(lastCombinedControlsWidth, GridUnitType.Star);
                Splitter.Visibility = Visibility.Visible;
            }
        }
    }
}
