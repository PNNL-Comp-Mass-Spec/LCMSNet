﻿using System;
using System.Collections.Generic;
using LcmsNetSDK.Devices;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices.Pumps
{
    /// <summary>
    /// View Model for displaying a plot with pump monitoring data
    /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Pump name</param>
        public PumpDisplayViewModel(string name)
        {
            InitializePlots();
            PumpName = name;
        }

        private string pumpName = "TestPump";
        private PlotModel dataPressureMonitorPlot;
        private PlotModel dataFlowMonitorPlot;
        private PlotModel dataBMonitorPlot;

        /// <summary>
        /// List for supplying data to the plots. Does not need to be an observable list because we directly trigger updates.
        /// </summary>
        private readonly List<PlotData> plotDataList = new List<PlotData>();

        /// <summary>
        /// Name of the pump
        /// </summary>
        public string PumpName
        {
            get => pumpName;
            private set => this.RaiseAndSetIfChanged(ref pumpName, value);
        }

        /// <summary>
        /// The Data Pressure Plot
        /// </summary>
        public PlotModel DataPressureMonitorPlot
        {
            get => dataPressureMonitorPlot;
            private set => this.RaiseAndSetIfChanged(ref dataPressureMonitorPlot, value);
        }

        /// <summary>
        /// The Data Flow Plot
        /// </summary>
        public PlotModel DataFlowMonitorPlot
        {
            get => dataFlowMonitorPlot;
            private set => this.RaiseAndSetIfChanged(ref dataFlowMonitorPlot, value);
        }

        /// <summary>
        /// The Percent B Plot
        /// </summary>
        public PlotModel DataBMonitorPlot
        {
            get => dataBMonitorPlot;
            private set => this.RaiseAndSetIfChanged(ref dataBMonitorPlot, value);
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
                AxisTickToLabelDistance = 0,
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
                AxisTickToLabelDistance = 0,
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
                AxisTickToLabelDistance = 0,
            });
            DataBMonitorPlot.Axes.Add(new LinearAxis()
            {
                Title = "Composition (%B)",
                TitleFontSize = 10,
                Position = AxisPosition.Left,
                AxislineColor = OxyColors.DarkGreen,
                FontSize = 10,
            });

            var pressureSeries = new LineSeries()
            {
                Title = "Pressure (psi)",
                //MarkerType = MarkerType.Circle,
                Color = OxyColors.Red,
                ItemsSource = plotDataList,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData)item).Time), ((PlotData)item).Pressure),
                RenderInLegend = false,
            };

            var flowSeries = new LineSeries()
            {
                Title = "FlowRate",
                //MarkerType = MarkerType.Triangle,
                Color = OxyColors.DarkGreen,
                ItemsSource = plotDataList,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData)item).Time), ((PlotData)item).Flow),
                RenderInLegend = false,
            };

            var percentBSeries = new LineSeries()
            {
                Title = "% B",
                //MarkerType = MarkerType.Square,
                Color = OxyColors.Blue,
                ItemsSource = plotDataList,
                Mapping = item => new DataPoint(DateTimeAxis.ToDouble(((PlotData)item).Time), ((PlotData)item).PercentB),
                RenderInLegend = false,
            };

            DataPressureMonitorPlot.Series.Add(pressureSeries);
            DataFlowMonitorPlot.Series.Add(flowSeries);
            DataBMonitorPlot.Series.Add(percentBSeries);
        }

        /// <summary>
        /// Displays the monitoring data.
        /// </summary>
        public void DisplayMonitoringData(object sender, PumpDataEventArgs args)
        {
            // TODO: Change this so that we are not re-generating the whole list with every update.
            plotDataList.Clear();
            for (var i = 0; i < args.Time.Count; i++)
            {
                plotDataList.Add(new PlotData(args.Time[i], args.Pressure[i], args.Flowrate[i], args.PercentB[i]));
            }

            DataPressureMonitorPlot.InvalidatePlot(true);
            DataFlowMonitorPlot.InvalidatePlot(true);
            DataBMonitorPlot.InvalidatePlot(true);
        }

        /// <summary>
        /// Set the pump name
        /// </summary>
        /// <param name="name"></param>
        public void SetPumpName(string name)
        {
            PumpName = name;
        }

        /// <summary>
        /// Class to hold information for plotting
        /// </summary>
        public class PlotData
        {
            /// <summary>
            /// The timestamp
            /// </summary>
            public DateTime Time { get; }

            /// <summary>
            /// Pressure
            /// </summary>
            public double Pressure { get; }

            /// <summary>
            /// Flow
            /// </summary>
            public double Flow { get; }

            /// <summary>
            /// Percent B
            /// </summary>
            public double PercentB { get; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="time"></param>
            /// <param name="pressure"></param>
            /// <param name="flow"></param>
            /// <param name="percentB"></param>
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