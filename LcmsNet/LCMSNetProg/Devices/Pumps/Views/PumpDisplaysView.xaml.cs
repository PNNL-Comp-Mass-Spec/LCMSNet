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
using LcmsNet.Devices.Pumps.ViewModels;
using ReactiveUI;

namespace LcmsNet.Devices.Pumps.Views
{
    /// <summary>
    /// Interaction logic for PumpDisplaysView.xaml
    /// </summary>
    public partial class PumpDisplaysView : UserControl
    {
        public PumpDisplaysView()
        {
            InitializeComponent();

            this.DataContextChanged += PumpDisplaysView_DataContextChanged;
            UpdatePumpStatusGrid();
        }

        private void PumpDisplaysView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is PumpDisplaysViewModel pdvm)
            {
                pdvm.WhenAnyValue(x => x.PumpMonitorDisplays.CountChanged).Subscribe(x => this.UpdatePumpStatusGrid());
            }
        }

        private Dictionary<string, PumpDisplayView> pumpDisplayViews = new Dictionary<string, PumpDisplayView>();

        private void UpdatePumpStatusGrid()
        {
            PumpStatusGrid.ColumnDefinitions.Clear();
            PumpStatusGrid.Children.Clear();
            if (this.DataContext is PumpDisplaysViewModel pdvm)
            {
                var previousList = pumpDisplayViews;
                pumpDisplayViews = new Dictionary<string, PumpDisplayView>();

                var column = 0;
                foreach (var pumpDisplay in pdvm.PumpMonitorDisplays)
                {
                    PumpDisplayView pdv = null;
                    if (previousList.ContainsKey(pumpDisplay.PumpName) && ReferenceEquals(previousList[pumpDisplay.PumpName].DataContext, pumpDisplay))
                    {
                        pdv = previousList[pumpDisplay.PumpName];
                    }
                    else
                    {
                        pdv = new PumpDisplayView { DataContext = pumpDisplay };
                    }

                    pumpDisplayViews.Add(pumpDisplay.PumpName, pdv);

                    var splitter = new GridSplitter()
                    {
                        ShowsPreview = false,
                        Width = 5,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };

                    PumpStatusGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    PumpStatusGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5) });

                    pdv.SetValue(Grid.ColumnProperty, column++);
                    splitter.SetValue(Grid.ColumnProperty, column++);

                    PumpStatusGrid.Children.Add(pdv);
                    PumpStatusGrid.Children.Add(splitter);
                }
            }
            else
            {
                pumpDisplayViews.Clear();
            }
        }
    }
}
