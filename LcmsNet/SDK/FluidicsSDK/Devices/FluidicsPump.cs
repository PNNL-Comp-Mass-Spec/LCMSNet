/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/22/2013
 * 
 * Last Modified 10/17/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using System.Windows.Forms;
using FluidicsSDK.Base;
using FluidicsSDK.Managers;

namespace FluidicsSDK
{
    public class FluidicsPump: FluidicsDevice
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

            private Dictionary<TwoPositionState, List<Tuple<int, int>>> m_states;
            //private TwoPositionState m_currentState;

            IFluidicsPump m_pump;

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
            base.AddRectangle(new Point(0, 0), new Size(LENGTH, WIDTH), Color.Black, Brushes.White);
            Point portLoc = GeneratePortLoc();
            base.AddPort(portLoc);
            m_states = SetupStates();                        
            MaxVariance = MAX_PIXEL_VARIANCE;
            Source = true;
            Sink = false;
            // set port as source for model checking.
            base.Ports[0].Source = true;
        }

        /// <summary>
        /// class constructor 
        /// </summary>
        /// <param name="loc">Point representing  location on screen to draw the pump(upper left corner)</param>
        public FluidicsPump(Point loc)
        {
            base.AddRectangle(loc, new Size(LENGTH, WIDTH), Color.Black, Brushes.White);
            Point portLoc = GeneratePortLoc();
            base.AddPort(portLoc);
        }

        /// <summary>
        /// generate the locations of the ports associated with the pump, used at creation or when the device is moved around the screen
        /// </summary>
        /// <returns>a list of System.Drawing.Point objects, one for each port</returns>
        protected Point GeneratePortLoc()
        {           
            //create port1 to left side of pump
            return new Point(Loc.X + (int)(Size.Width/2), Loc.Y - MIN_DIST_FROM_EDGE);          
        }

        /// <summary>
        /// setup the states
        /// </summary>
        /// <returns></returns>
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            Dictionary<TwoPositionState, List<Tuple<int, int>>> states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();               
            // pump only has no states, it is a source
            return states;
        }
 
        protected override void SetDevice(IDevice device)
        {
            m_pump = device as IFluidicsPump;           
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


        public void SetPostion(TwoPositionState state)
        {          
        }

        public override string StateString()
        {
            StringBuilder stateString = new StringBuilder();
            stateString.Append("Flow Rate: ");
            stateString.Append(m_pump.GetFlowRate());
            stateString.Append(Environment.NewLine);
            stateString.Append("Pressure: ");            
            stateString.Append(m_pump.GetPressure());
            stateString.Append(Environment.NewLine);
            stateString.Append("%B: ");
            stateString.Append(m_pump.GetPercentB());
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