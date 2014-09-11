using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices.Pumps;
using ZedGraph;

namespace LcmsNetDataClasses.Devices.Pumps
{
    public partial class controlPumpDisplay : UserControl
    {
        public controlPumpDisplay()
        {
            InitializeComponent();
            InitializePlots();
        }

        /// <summary>
        /// Initialize the plotting capabilities
        /// </summary>
        private void InitializePlots()
        {
            /// 
            /// Time and setup 
            /// 
            GraphPane pane = mplot_monitoringDataPressure.GraphPane;
            pane.Title.Text = "Pressure (bar)";
            pane.XAxis.Title.Text = "Time";
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.MajorGrid.IsVisible = true;

            /// 
            /// Pressure
            /// 
            pane.YAxis.Color = Color.Red;
            pane.YAxis.Title.Text = "Pressure (bar)";

            /// 
            /// Flow rate 
            /// 
            GraphPane paneFlow          = mplot_monitoringDataFlow.GraphPane;
            paneFlow.Title.Text         = "Flow Rate";
            paneFlow.XAxis.Title.Text   = "Time";
            paneFlow.XAxis.Type         = AxisType.Date;
            paneFlow.XAxis.MajorGrid.IsVisible = true;
            paneFlow.YAxis.Title.Text         = "Flow Rate";            
            paneFlow.YAxis.Color        = Color.DarkGreen;

            /// 
            /// Percent B 
            /// 
            GraphPane paneComp = mplot_monitoringDataB.GraphPane;
            paneComp.Title.Text = "Composition";
            paneComp.XAxis.Title.Text = "Time";
            paneComp.XAxis.Type = AxisType.Date;
            paneComp.XAxis.MajorGrid.IsVisible = true;
            paneComp.YAxis.Title.Text = "Composition (%B)";
            paneComp.YAxis.Color = Color.Blue;            
        }


        /// <summary>
        /// Displays the monitoring data.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pressure"></param>
        /// <param name="flowrate"></param>
        /// <param name="percentB"></param>
        public void DisplayMonitoringData(object sender, PumpDataEventArgs args) //List<DateTime> time, List<double> pressure, List<double> flowrate, List<double> percentB)
        {
            try
            {
                GraphPane panePressure  = mplot_monitoringDataPressure.GraphPane;
                GraphPane paneFlow      = mplot_monitoringDataFlow.GraphPane;
                GraphPane paneB         = mplot_monitoringDataB.GraphPane;

                double[] x = new double[args.Time.Count];
                int i = 0;
                foreach (DateTime tick in args.Time)
                    x[i++] = tick.ToOADate();

                double[] pressurePoints = new double[args.Pressure.Count];
                args.Pressure.CopyTo(pressurePoints, 0);

                double[] flowrates = new double[args.Flowrate.Count];
                args.Flowrate.CopyTo(flowrates, 0);

                double[] percentBs = new double[args.PercentB.Count];
                args.PercentB.CopyTo(percentBs, 0);

                panePressure.CurveList.Clear();
                paneFlow.CurveList.Clear();
                paneB.CurveList.Clear();


                LineItem pressureItems  = panePressure.AddCurve("Pressure (psi)", x, pressurePoints, Color.Red, SymbolType.Circle);
                LineItem flowItems      = paneFlow.AddCurve("Flow Rate", x, flowrates, Color.DarkGreen, SymbolType.Triangle);
                LineItem percentItems   = paneB.AddCurve("% B", x, percentBs, Color.Blue, SymbolType.Square);

                paneB.AxisChange();
                paneFlow.AxisChange();
                panePressure.AxisChange();

                mplot_monitoringDataPressure.Refresh();
                mplot_monitoringDataFlow.Refresh();
                mplot_monitoringDataB.Refresh();
            }
            catch
            {
            }
        }

        public void SetPumpName(string name)
        {
            mlabel_pumpName.Text = name;
        }

        public event EventHandler Tack;
        public event EventHandler UnTack;
        public bool Tacked
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the width for the tack icon.
        /// </summary>
        public int TackWidth
        {
            get
            {
                return 36;
            }
        }
        private void mbutton_expand_Click(object sender, EventArgs e)
        {

            Tacked = (Tacked == false);
            if (Tacked)
            {
                if (Tack != null)
                {
                    Tack(this, e);
                }
            }
            else
            {
                if (UnTack != null)
                {
                    UnTack(this, e);
                }
            }
        }
    }
}
