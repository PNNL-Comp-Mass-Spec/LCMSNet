using System;
using System.Collections.Generic;
using FluidicsSDK.Base;
using System.Drawing;
using FluidicsSDK.Graphic;
using LcmsNetDataClasses.Devices;

namespace FluidicsSDK.Devices
{
    public class SixteenPortFluidicsValve : MultiPositionValve
    {
        private const int NUMBER_OF_PORTS = 16;
        ISixteenPortValve m_valve;

        public SixteenPortFluidicsValve() :
            base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = (int)FifteenPositionState.P1;
            base.ActivateState(m_states[m_currentState]);
            var StateControlSize = new Size(15, 15);
            var StateControl1Loc = new Point(base.Center.X - (StateControlSize.Width * 3), base.Center.Y - (StateControlSize.Height / 2));
            var StateControl2Loc = new Point(base.Center.X + StateControlSize.Width * 2, base.Center.Y - (StateControlSize.Height / 2));
            var StateControlRectangle = new Rectangle(StateControl1Loc, StateControlSize);
            var StateControlRectangle2 = new Rectangle(StateControl2Loc, StateControlSize);
            //add left control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle, Orient.Left), LeftButtonAction);
            //add right control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle2, Orient.Right), RightButtonAction);
        }

        private void SetValvePosition(FifteenPositionState pos)
        {
            m_valve.SetPosition(pos);
        }

        public override string StateString()
        {
            return ((FifteenPositionState)m_currentState).ToCustomString();
        }

        protected override void SetDevice(IDevice device)
        {
            m_valve = device as ISixteenPortValve;
            try
            {
                m_valve.PosChanged += m_valve_PositionChanged;
            }
            catch (Exception)
            {
                // MessageBox.Show("This is crap..." + ex.Message);
            }
        }


        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        protected Dictionary<int, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<int, List<Tuple<int, int>>>();
            for (var i = 1; i < 16; i++)
            {
                states.Add(i, GenerateState(0, i));
            }
            //states.Add(FifteenPositionState.P9, GenerateState(-1, -1));
            return states;
        }

        void m_valve_PositionChanged(object sender, ValvePositionEventArgs<int> e)
        {
            ActivateState((int)e.Position);
        }

        protected override void ClearDevice(IDevice device)
        {
            m_valve = null;
        }

        private void ChangePosition(bool left)
        {
            var state = m_currentState;
            if (m_currentState != (int)FifteenPositionState.Unknown)
            {
                if (left)
                {
                    state--;
                    state = state < (int)FifteenPositionState.P1 ? (int)FifteenPositionState.P15 : state;
                }
                else
                {
                    state++;
                    state = state > (int)FifteenPositionState.P15 ? (int)FifteenPositionState.P1 : state;
                }
            }
            else
            {
                state = (int)FifteenPositionState.P1;
            }
            m_valve.SetPosition((FifteenPositionState)state);

        }

        private void LeftButtonAction()
        {
            ChangePosition(true);

        }

        private void RightButtonAction()
        {
            ChangePosition(false);
        }

        protected override List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
        {
            //the nine port valve connects the center port to one of the outer ports, so it's states are unique
            //so we override the generation.
            var stateConnection = new List<Tuple<int, int>>();
            //if startingPortIndex == -1, return empty list representing a "no connections" state
            if (startingPortIndex == -1) { return stateConnection; }
            stateConnection.Add(new Tuple<int, int>(startingPortIndex, endingPortIndex));
            return stateConnection;
        }
    }
}
