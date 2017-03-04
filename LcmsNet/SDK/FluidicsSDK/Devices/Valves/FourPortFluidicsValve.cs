using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using System.Drawing;

namespace FluidicsSDK.Devices
{
    public sealed class FourPortFluidicsValve: TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 4;
        IFourPortValve m_valve;

        public FourPortFluidicsValve():
            base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = TwoPositionState.PositionA;
            base.ActivateState(m_states[m_currentState]);
            var stateControlSize = new Size(15, 15);
            var stateControl1Loc = new Point(Center.X - (stateControlSize.Width * 2), Center.Y - (stateControlSize.Height / 2));
            var stateControl2Loc = new Point(Center.X + stateControlSize.Width, Center.Y - (stateControlSize.Height /2));
            var stateControlRectangle = new Rectangle(stateControl1Loc, stateControlSize);
            var stateControlRectangle2 = new Rectangle(stateControl2Loc, stateControlSize);
            //add left control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle, Orient.Left), LeftButtonAction);
            //add right control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle2, Orient.Right), RightButtonAction);
        }

        private void SetValvePosition(TwoPositionState pos)
        {
            m_valve.SetPosition(pos);
        }   

        protected override void SetDevice(IDevice device)
        {
            m_valve = device as IFourPortValve;
            try
            {
                //m_valve.SetPosition(10);
                if (m_valve != null)
                    m_valve.PositionChanged += m_valve_PositionChanged;
            }
            catch(Exception)
            {
                // MessageBox.Show("Null valve: " + ex.Message);
            }
        }

        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        private Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>
            {
                {TwoPositionState.PositionA, GenerateState(0, Ports.Count - 1)},
                {TwoPositionState.PositionB, GenerateState(1, Ports.Count)}
            };
            return states;
        }


        void m_valve_PositionChanged(object sender, ValvePositionEventArgs<TwoPositionState> e)
        {            
            ActivateState((int)e.Position);
        }

        protected override void ClearDevice(IDevice device)
        {
            m_valve = null;
        }

        private void ChangePosition(bool left)
        {
            var pos = (int)m_currentState;
            if (m_currentState != TwoPositionState.Unknown)
            {                
                if (left)
                {
                    pos--;
                    if (pos < (int)TwoPositionState.PositionA)
                    {
                        pos = (int)TwoPositionState.PositionB;
                    }
                }
                else
                {
                    pos++;
                    if (pos > (int)TwoPositionState.PositionB)
                    {
                        pos = (int)TwoPositionState.PositionA;
                    }
                }
            }
            else
            {
                pos = (int)TwoPositionState.PositionA;
            }
            m_valve.SetPosition((TwoPositionState)pos);
        }

        /// <summary>
        /// action to take when left state primitive is clicked
        /// </summary>
        private void LeftButtonAction()
        {
            ChangePosition(true);
        }

        /// <summary>
        /// action to take when right state primitive is clicked
        /// </summary>
        private void RightButtonAction()
        {
            ChangePosition(false);
        }
    }
}
