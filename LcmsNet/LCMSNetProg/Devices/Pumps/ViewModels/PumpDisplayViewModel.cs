using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices.Pumps;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace LcmsNet.Devices.Pumps.ViewModels
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
            };
            DataPressureMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom
            });
            DataPressureMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Pressure (bar)",
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.Red,
            });

            // Flow rate
            DataFlowMonitorPlot = new PlotModel()
            {
                Title = "Flow Rate",
            };
            DataFlowMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom
            });
            DataFlowMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Flow Rate",
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.DarkGreen,
            });

            // Percent B
            DataBMonitorPlot = new PlotModel()
            {
                Title = "Composition",
            };
            DataBMonitorPlot.Axes.Add(new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom
            });
            DataBMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Composition (%B)",
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.DarkGreen,
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
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData) item).Time), ((PlotData) item).Pressure),
            };

            var flowSeries = new LineSeries()
            {
                Title = "FlowRate",
                MarkerType = MarkerType.Triangle,
                Color = OxyColors.DarkGreen,
                ItemsSource = data,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData) item).Time), ((PlotData) item).Flow),
            };

            var percentBSeries = new LineSeries()
            {
                Title = "% B",
                MarkerType = MarkerType.Square,
                Color = OxyColors.Blue,
                ItemsSource = data,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData) item).Time), ((PlotData) item).PercentB),
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
