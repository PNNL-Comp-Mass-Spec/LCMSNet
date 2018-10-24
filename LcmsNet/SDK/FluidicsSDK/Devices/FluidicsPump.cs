using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices
{
    public sealed class FluidicsPump : FluidicsDevice
    {
        #region members

        /// <summary>
        /// defines minimum distance from the primary primitive of the pump for others to be drawn
        /// </summary>
        const int MIN_DIST_FROM_EDGE = 12;

        /// <summary>
        /// defines the length of the pump rectangle primitive
        /// </summary>
        const int LENGTH = 100;

        /// <summary>
        /// defines the width of the pump rectangle primitive
        /// </summary>
        const int WIDTH = 50;

        const int MAX_PIXEL_VARIANCE = 5;

        IFluidicsPump m_pump;

        private bool useMonitoringData = false;
        private double flowRate = 0;
        private double pressure = 0;
        private double percentB = 0;
        private DateTime lastStatusUpdate = DateTime.MinValue;
        private DateTime lastMonitorUpdate = DateTime.MinValue;

        #endregion

        #region Methods

        public override void ActivateState(int state)
        {
            // do nothing. we have no state.
        }

        /// <summary>
        /// base class constructor
        /// </summary>
        public FluidicsPump()
        {
            var offset = new Point(2, 4 + Port.PORT_DEFAULT_RADIUS * 2);
            AddRectangle(offset, new Size(LENGTH, WIDTH), Colors.Black, Brushes.White);
            var portLoc = GeneratePortLoc(offset);
            AddPort(portLoc);
            var states = SetupStates();
            MaxVariance = MAX_PIXEL_VARIANCE;
            Source = true;
            Sink = false;
            // set port as source for model checking.
            Ports[0].Source = true;
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">Point representing  location on screen to draw the pump(upper left corner)</param>
        public FluidicsPump(Point loc)
        {
            var offset = new Point(loc.X + 2, loc.X + 4 + Port.PORT_DEFAULT_RADIUS * 2);
            AddRectangle(offset, new Size(LENGTH, WIDTH), Colors.Black, Brushes.White);
            var portLoc = GeneratePortLoc(offset);
            AddPort(portLoc);
        }

        /// <summary>
        /// generate the locations of the ports associated with the pump, used at creation or when the device is moved around the screen
        /// </summary>
        /// <returns>a list of Point objects, one for each port</returns>
        private Point GeneratePortLoc(Point offset)
        {
            //create port1 at top center side of pump
            return new Point(Loc.X + offset.X + (int) (Size.Width / 2), Loc.Y + offset.Y - MIN_DIST_FROM_EDGE);
        }

        /// <summary>
        /// setup the states
        /// </summary>
        /// <returns></returns>
        private Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();
            // pump only has no states, it is a source
            return states;
        }

        protected override void SetDevice(IDevice device)
        {
            if (m_pump is IPump pOld)
            {
                pOld.MonitoringDataReceived -= PumpOnMonitoringDataReceived;
            }
            m_pump = device as IFluidicsPump;
            useMonitoringData = false;
            if (m_pump is IPump p)
            {
                useMonitoringData = true;
                p.MonitoringDataReceived += PumpOnMonitoringDataReceived;
            }
            // m_pump.FlowChanged += new EventHandler<PumpEventArgs<double>>(pump_flow_change_event_handler);
        }

        protected override void ClearDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// test code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pump_flow_change_event_handler(object sender, PumpEventArgs<double> e)
        {
            MessageBox.Show(string.Format("Flowrate changed to: {0}", e.Value));
        }

        private void PumpOnMonitoringDataReceived(object sender, PumpDataEventArgs e)
        {
            flowRate = e.Flowrate.Last();
            pressure = e.Pressure.Last();
            percentB = e.PercentB.Last();
            lastMonitorUpdate = DateTime.Now;
        }

        public override string StateString()
        {
            if (useMonitoringData && DateTime.Now.AddSeconds(-65) >= lastMonitorUpdate)
            {
                // If we aren't receiving any monitoring data, turn off this optimization.
                useMonitoringData = false;
            }

            if (!useMonitoringData && DateTime.Now.AddSeconds(-30) >= lastStatusUpdate)
            {
                flowRate = m_pump.GetFlowRate();
                pressure = m_pump.GetPressure();
                percentB = m_pump.GetPercentB();
                lastStatusUpdate = DateTime.Now;
            }

            var stateString = new StringBuilder();
            stateString.Append("Flow Rate: ");
            stateString.Append(flowRate);
            stateString.Append(Environment.NewLine);
            stateString.Append("Pressure: ");
            stateString.Append(pressure);
            stateString.Append(Environment.NewLine);
            stateString.Append("%B: ");
            stateString.Append(percentB);
            stateString.Append(Environment.NewLine);

            return stateString.ToString();
        }

        #endregion

        #region Properties

        public override int CurrentState
        {
            get
            {
                //pumps don't have a state of this kind.
                return -1;
            }
            set
            {
                //do nothing
            }
        }

        #endregion
    }
}
