using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using System.Windows.Forms;
using FluidicsSDK.Base;
using FluidicsSDK.Managers;
using FluidicsSDK.Graphic;
using System.Drawing;

namespace FluidicsSDK.Devices
{
    public class FourPortFluidicsValve: TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 4;
        IFourPortValve m_valve;

        public FourPortFluidicsValve():
            base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = TwoPositionState.PositionA;
            base.ActivateState(m_states[m_currentState]);
            var StateControlSize = new Size(15, 15);
            var StateControl1Loc = new Point(base.Center.X - (StateControlSize.Width * 2), base.Center.Y - (StateControlSize.Height / 2));
            var StateControl2Loc = new Point(base.Center.X + StateControlSize.Width, base.Center.Y - (StateControlSize.Height /2));
            var StateControlRectangle = new Rectangle(StateControl1Loc, StateControlSize);
            var StateControlRectangle2 = new Rectangle(StateControl2Loc, StateControlSize);
            //add left control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle, Orient.Left), new Action(LeftButtonAction));
            //add right control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle2, Orient.Right), new Action(RightButtonAction));
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
                m_valve.PositionChanged += new EventHandler<ValvePositionEventArgs<TwoPositionState>>(m_valve_PositionChanged);
            }
            catch(Exception)
            {
               // MessageBox.Show("This is crap..." + ex.Message);
            }
        }

        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();
            states.Add(TwoPositionState.PositionA, GenerateState(0, Ports.Count - 1));
            states.Add(TwoPositionState.PositionB, GenerateState(1, Ports.Count));
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
