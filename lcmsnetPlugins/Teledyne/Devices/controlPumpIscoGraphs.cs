//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/31/2011
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace LcmsNet.Devices.Pumps
{
    /// <summary>
    /// Control for display of Isco operation graphs
    /// </summary>
    public partial class controlPumpIscoGraphs : UserControl
    {
        #region "Constants"
        #endregion

        #region "Class variables"
            ZedGraphControl[] mplot_PumpGraphArray;
            
            bool m_FirstPointReceived;
            // DateTime mdate_GraphStartTime = LcmsNetSDK.TimeKeeper.Instance.Now;//DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            List<DateTime>[] m_SampleTimes;
            List<double>[] m_FlowData;
            List<double>[] m_PressData;
        #endregion

        #region "Delegates"
            //private delegate void delegateUpdateGraphHandler(ZedGraphControl inpGraph, classPumpIscoData newData);
            private delegate void delegateUpdateGraphHandler(ZedGraphControl inpGraph, List<DateTime> time,
                List<double> pressure, List<double> flow);
        #endregion

        #region "Events"
        #endregion

        #region "Properties"
        #endregion

        #region "Constructors"
            public controlPumpIscoGraphs()
            {
                InitializeComponent();

                InitControl();
            }
        #endregion

        #region "Methods"
            /// <summary>
            /// Initializes the control
            /// </summary>
            private void InitControl()
            {
                // Initialize array holding the graphs
                mplot_PumpGraphArray = new[] {mplot_PumpA, mplot_PumpB, mplot_PumpC};

                // Initialize the graphs
                InitPlot(ref mplot_PumpGraphArray[0], "Pump A");
                InitPlot(ref mplot_PumpGraphArray[1], "Pump B");
                InitPlot(ref mplot_PumpGraphArray[2], "Pump C");

                // Initialize pump data arrays
                m_SampleTimes = new[] { new List<DateTime>(), new List<DateTime>(), new List<DateTime>() };
                m_FlowData = new[] { new List<double>(), new List<double>(), new List<double>() };
                m_PressData = new[] { new List<double>(), new List<double>(), new List<double>() };
            }   
            
            ///// <summary>
            ///// Clears all graphs
            ///// </summary>
            //public void ClearGraphs()
            //{
            //   for (int indx = 0; indx < mplot_PumpGraphArray.Length; indx++)
            //   {
            //      ClearPlot(ref mplot_PumpGraphArray[indx]);
            //   }
            //} 

            /// <summary>
            /// Initializes a graph
            /// </summary>
            /// <param name="inpGraph">Graph to initialize</param>
            /// <param name="grphTitle">Title of graph</param>
            private void InitPlot(ref ZedGraphControl inpGraph, string grphTitle)
            {
                var pumpPane = inpGraph.GraphPane;

                pumpPane.Title.Text = grphTitle;
                pumpPane.Legend.IsVisible = false;

                pumpPane.XAxis.Title.Text = "Time (s)";
                pumpPane.XAxis.Type = AxisType.Date;
                pumpPane.XAxis.MajorGrid.IsVisible = true;



                //// Manually control X-Axis range so graph scrolls continuously
                //pumpPane.XAxis.Title.Text = "Time(Hours)";
                //pumpPane.XAxis.Scale.Max = 2;
                //pumpPane.XAxis.Scale.MajorStep = 0.5;
                //pumpPane.XAxis.Scale.MinorStep = 0.1;

                // Setup Y axis
                pumpPane.YAxis.Title.Text = "Pressure (PSI)";
                pumpPane.YAxis.MajorTic.IsOpposite = false;
                pumpPane.YAxis.MinorTic.IsOpposite = false;

                // Add 2nd Y axis
                pumpPane.Y2Axis.Title.Text = "Flow (mL/min)";
                pumpPane.Y2Axis.IsVisible = true;
                pumpPane.Y2Axis.Scale.FontSpec.FontColor = Color.Red;
                pumpPane.Y2Axis.Title.FontSpec.FontColor = Color.Red;
                pumpPane.Y2Axis.MajorTic.IsOpposite = false;
                pumpPane.Y2Axis.MinorTic.IsOpposite = false;

                //// Save 600 points (approx. 10 minutes of data at 2 sec/point).
                //RollingPointPairList pressList = new RollingPointPairList(600);
                //RollingPointPairList flowList = new RollingPointPairList(600);

                //// Add initial curves with no data
                //LineItem pressCurve = pumpPane.AddCurve("Press", pressList, Color.Black, SymbolType.None);
                //LineItem flowCurve = pumpPane.AddCurve("Flow", flowList, Color.Red, SymbolType.None);
                //flowCurve.IsY2Axis = true;

                //// Scale the axes
                //inpGraph.AxisChange();
            }   

            /// <summary>
            /// Updates all the pump graphs
            /// </summary>
            /// <param name="pumpData">New data for all pumps</param>
            public void UpdateAllPlots(classPumpIscoData[] pumpData)
            {
                for (var indx = 0; indx < mplot_PumpGraphArray.Length; indx++)
                {
                    UpdatePlotDataLists(pumpData[indx], ref m_SampleTimes[indx], ref m_PressData[indx], ref m_FlowData[indx]);
                    UpdatePlot(mplot_PumpGraphArray[indx], m_SampleTimes[indx], m_PressData[indx], m_FlowData[indx]);
                    //UpdatePlot(mplot_PumpGraphArray[indx], pumpData[indx]);
                }
            }   

            /// <summary>
            /// Adds current data point to plot data lists and removes old data
            /// </summary>
            /// <param name="pumpData">Input data</param>
            /// <param name="times">List of data point times</param>
            /// <param name="press">List of pressure data points</param>
            /// <param name="flow">List of flow data points</param>
            private void UpdatePlotDataLists(classPumpIscoData pumpData, ref List<DateTime> times, ref List<double> press,
                ref List<double> flow)
            {
                var currTime = LcmsNetSDK.TimeKeeper.Instance.Now; //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

                // Add data points to storage lists
                times.Add(pumpData.PointTime);
                press.Add(pumpData.Pressure);
                flow.Add(pumpData.Flow);

                // Remove data more than two hours old
                var indx = 0;
                // Get index of first time that's less than two hours old
                while (currTime.Subtract(times[indx]).TotalMinutes > 120 && indx < times.Count) indx++;
                // Remove the old time values
                if (indx > 0)
                {
                    indx = Math.Min(indx, times.Count - 1);
                    times.RemoveRange(0, indx);
                    press.RemoveRange(0, indx);
                    flow.RemoveRange(0, indx);
                }
            }   

            /// <summary>
            ///  Updates a single plot, using a delegate if necessary
            /// </summary>
            /// <param name="inpGraph">Plot to update</param>
            /// <param name="time">List of sample times</param>
            /// <param name="pressure">List of pressures</param>
            /// <param name="flow">List of flows</param>
            private void UpdatePlot(ZedGraphControl inpGraph, List<DateTime> time, List<double> pressure, List<double> flow)
            {
                if (inpGraph.InvokeRequired)
                {
                    var d = new delegateUpdateGraphHandler(UpdatePlot);
                    inpGraph.BeginInvoke(d, inpGraph, time, pressure, flow);
                }
                else UpdatePlot_Delegated(inpGraph, time, pressure, flow);
            }   

            ///// <summary>
            ///// Updates plot via a delgate
            ///// </summary>
            ///// <param name="inpGraph">Graph to update</param>
            ///// <param name="newData">New data for update</param>
            //private void UpdatePlot(ZedGraphControl inpGraph, classPumpIscoData newData)
            //{
            //   if (inpGraph.InvokeRequired)
            //   {
            //      delegateUpdateGraphHandler d = new delegateUpdateGraphHandler(UpdatePlot);
            //      inpGraph.BeginInvoke(d, new object[] { inpGraph, newData });
            //   }
            //   else UpdatePlot_Delegated(inpGraph, newData);
            //} 

            /// <summary>
            /// Performs actual plot update
            /// </summary>
            /// <param name="inpGraph">Plot to update</param>
            /// <param name="time">List of sample times</param>
            /// <param name="pressure">List of pressures</param>
            /// <param name="flow">List of flows</param>
            private void UpdatePlot_Delegated(ZedGraphControl inpGraph, List<DateTime> time, List<double> pressure, List<double> flow)
            {
                var pumpPane = inpGraph.GraphPane;

                // Fill a time array for the plot
                var tempTime = new double[time.Count];
                var indx = 0;
                foreach (var tick in time)
                {
                    tempTime[indx] = tick.ToOADate();
                    indx++;
                }

                // Fill the pressure array
                var pressurePoints = new double[pressure.Count];
                pressure.CopyTo(pressurePoints, 0);

                // Fill the flow array
                var flowPoints = new double[flow.Count];
                flow.CopyTo(flowPoints, 0);

                // Clear the existing plot
                pumpPane.CurveList.Clear();

                var pressLine = pumpPane.AddCurve("Press", tempTime, pressurePoints, Color.Black, SymbolType.None);
                var flowLine = pumpPane.AddCurve("Flow", tempTime, flowPoints, Color.Red, SymbolType.None);
                flowLine.IsY2Axis = true;

                // Force rescale
                pumpPane.AxisChange();

                // Force redraw
                inpGraph.Invalidate();
            }   

            ///// <summary>
            ///// Performs actual graph update when called by delegate
            ///// </summary>
            ///// <param name="inpGraph">Graph to update</param>
            ///// <param name="pumpData">Data for update</param>
            //private void UpdatePlot_Delegated(ZedGraphControl inpGraph, classPumpIscoData pumpData)
            //{
            //   GraphPane pumpPane = inpGraph.GraphPane;

            //   // If this is the first point in a new graph, then set appropriate class variables and clear point arrays
            //   if (!m_FirstPointReceived)
            //   {
            //      m_FirstPointReceived = true;
            //      mdate_GraphStartTime = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            //      foreach (LineItem currentCurve in pumpPane.CurveList)
            //      {
            //         IPointListEdit currentPointList = currentCurve.Points as IPointListEdit;
            //         currentPointList.Clear();
            //      }
            //      pumpPane.XAxis.Scale.Min = 0;
            //      pumpPane.XAxis.Scale.Max = 10;
            //   }

            //   // Make sure the curve list has correct number of curves
            //   if (inpGraph.GraphPane.CurveList.Count != 2) return;

            //   // Get the curveitems for the graph
            //   LineItem pressCurve = pumpPane.CurveList[0] as LineItem;
            //   LineItem flowCurve = pumpPane.CurveList[1] as LineItem;
            //   if ((pressCurve == null) || (flowCurve == null)) return;

            //   // Get the PointPairLists for the curves
            //   IPointListEdit pressPointList = pressCurve.Points as IPointListEdit;
            //   IPointListEdit flowPointList = flowCurve.Points as IPointListEdit;
            //   // If any list is null, exit because it doesn't support IPointListEdit and can't be edited
            //   if ((pressPointList == null) || (flowPointList == null)) return;

            //   // Get the elapsed time
            //   TimeSpan timeDiff = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).Subtract(mdate_GraphStartTime);
            //   double elapsedTime = timeDiff.TotalMinutes;

            //   // Add the new points to the curves
            //   pressPointList.Add(elapsedTime, pumpData.Pressure);
            //   flowPointList.Add(elapsedTime, pumpData.Flow);

            //   // Keep the X axis at 10 minutes, with one minor step between max X value and end of axis
            //   ZedGraph.Scale xScale = pumpPane.XAxis.Scale;
            //   if (elapsedTime > xScale.Max - xScale.MinorStep)
            //   {
            //      xScale.Max = elapsedTime + xScale.MinorStep;
            //      xScale.Min = xScale.Max - 10.0;
            //   }

            //   // Make sure Y axes are rescaled to accomodate actual data
            //   inpGraph.AxisChange();

            //   // Force a redraw
            //   inpGraph.Invalidate();
            //} 

            /// <summary>
            /// Clears a pump graph
            /// </summary>
            /// <param name="inpGraph">Graph to clear</param>
            private void ClearPlot(ref ZedGraphControl inpGraph)
            {
                // All that is necessary is to clear the data received flag. UpdatePlot takes care of the rest
                m_FirstPointReceived = false;
            }   
        #endregion
    }   
}
