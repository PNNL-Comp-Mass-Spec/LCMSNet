using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices.Pumps;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices.ViewModels
{
    public class PumpDisplayViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the pump display view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public PumpDisplayViewModel()
        {
            InitializePlots();
        }

        public PumpDisplayViewModel(string name)
        {
            InitializePlots();
            PumpName = name;
        }

        public bool Tacked { get; set; }

        /// <summary>
        /// Gets the width for the tack icon.
        /// </summary>
        public int TackWidth => 36;

        private string pumpName = "TestPump";
        private PlotModel dataPressureMonitorPlot;
        private PlotModel dataFlowMonitorPlot;
        private PlotModel dataBMonitorPlot;

        public string PumpName
        {
            get { return pumpName; }
            private set { this.RaiseAndSetIfChanged(ref pumpName, value); }
        }

        public PlotModel DataPressureMonitorPlot
        {
            get { return dataPressureMonitorPlot; }
            private set { this.RaiseAndSetIfChanged(ref dataPressureMonitorPlot, value); }
        }

        public PlotModel DataFlowMonitorPlot
        {
            get { return dataFlowMonitorPlot; }
            private set { this.RaiseAndSetIfChanged(ref dataFlowMonitorPlot, value); }
        }

        public PlotModel DataBMonitorPlot
        {
            get { return dataBMonitorPlot; }
            private set { this.RaiseAndSetIfChanged(ref dataBMonitorPlot, value); }
        }

        /// <summary>
        /// Initialize the plotting capabilities
        /// </summary>
        private void InitializePlots()
        {
            // Time and setup
            // Pressure
            DataPressureMonitorPlot = new PlotModel()
            {
                Title = "Pressure (bar)",
                TitleFontSize = 10,
            };
            DataPressureMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                TitleFontSize = 10,
                Position = AxisPosition.Bottom,
                FontSize = 10,
            });
            DataPressureMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Pressure (bar)",
                TitleFontSize = 10,
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.Red,
                FontSize = 10,
            });

            // Flow rate
            DataFlowMonitorPlot = new PlotModel()
            {
                Title = "Flow Rate",
                TitleFontSize = 10,
            };
            DataFlowMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                TitleFontSize = 10,
                Position = AxisPosition.Bottom,
                FontSize = 10,
            });
            DataFlowMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Flow Rate",
                TitleFontSize = 10,
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.DarkGreen,
                FontSize = 10,
            });

            // Percent B
            DataBMonitorPlot = new PlotModel()
            {
                Title = "Composition",
                TitleFontSize = 10,
            };
            DataBMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                TitleFontSize = 10,
                Position = AxisPosition.Bottom,
                FontSize = 10,
            });
            DataBMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Composition (%B)",
                TitleFontSize = 10,
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.DarkGreen,
                FontSize = 10,
            });
        }

        /// <summary>
        /// Displays the monitoring data.
        /// </summary>
        public void DisplayMonitoringData(object sender, PumpDataEventArgs args)
        {
            var data = new List<PlotData>(args.Time.Count);
            for (var i = 0; i < args.Time.Count; i++)
            {
                data.Add(new PlotData(args.Time[i], args.Pressure[i], args.Flowrate[i], args.PercentB[i]));
            }

            var pressureSeries = new LineSeries()
            {
                Title = "Pressure (psi)",
                MarkerType = MarkerType.Circle,
                Color = OxyColors.Red,
                ItemsSource = data,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData)item).Time), ((PlotData)item).Pressure),
                RenderInLegend = false,
            };

            var flowSeries = new LineSeries()
            {
                Title = "FlowRate",
                MarkerType = MarkerType.Triangle,
                Color = OxyColors.DarkGreen,
                ItemsSource = data,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData) item).Time), ((PlotData) item).Flow),
                RenderInLegend = false,
            };

            var percentBSeries = new LineSeries()
            {
                Title = "% B",
                MarkerType = MarkerType.Square,
                Color = OxyColors.Blue,
                ItemsSource = data,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData) item).Time), ((PlotData) item).PercentB),
                RenderInLegend = false,
            };

            DataPressureMonitorPlot.Series.Clear();
            DataFlowMonitorPlot.Series.Clear();
            DataBMonitorPlot.Series.Clear();

            DataPressureMonitorPlot.Series.Add(pressureSeries);
            DataFlowMonitorPlot.Series.Add(flowSeries);
            DataBMonitorPlot.Series.Add(percentBSeries);
        }

        public void SetPumpName(string name)
        {
            PumpName = name;
        }

        public class PlotData
        {
            public DateTime Time { get; private set; }
            public double Pressure { get; private set; }
            public double Flow { get; private set; }
            public double PercentB { get; private set; }

            public PlotData(DateTime time, double pressure, double flow, double percentB)
            {
                Time = time;
                Pressure = pressure;
                Flow = flow;
                PercentB = percentB;
            }
        }
    }
}
