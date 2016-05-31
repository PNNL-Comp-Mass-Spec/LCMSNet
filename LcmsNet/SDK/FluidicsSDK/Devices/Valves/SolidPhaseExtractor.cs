/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 12/31/2013
 * 
 * Last Modified 12/31/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Graphic;
using FluidicsSDK.Base;
using System.Drawing;
using LcmsNetDataClasses.Devices;

namespace FluidicsSDK.Devices
{
    public class SolidPhaseExtractor:TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;
        private ISolidPhaseExtractor m_valve;

        public SolidPhaseExtractor()
            : base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = TwoPositionState.PositionA;
            base.ActivateState(m_states[m_currentState]);
            Size StateControlSize = new Size(15, 15);
            Point StateControl1Loc = new Point(base.Center.X - (StateControlSize.Width * 2), base.Center.Y - (StateControlSize.Height / 2));
            Point StateControl2Loc = new Point(base.Center.X + StateControlSize.Width, base.Center.Y - (StateControlSize.Height / 2));
            Rectangle StateControlRectangle = new Rectangle(StateControl1Loc, StateControlSize);
            Rectangle StateControlRectangle2 = new Rectangle(StateControl2Loc, StateControlSize);
            //add left control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle, Orient.Left), new Action(LeftButtonAction));
            //add right control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle2, Orient.Right), new Action(RightButtonAction));      
            // add loop
            base.AddPrimitive(new FluidicsLine(m_portList[2].Center, m_portList[5].Center));
            base.AddPrimitive(new FluidicsRectangle(new Point(Center.X - 25, Center.Y - 15), new Size(50, 30), Color.Black, Brushes.White, fill:true, atScale:1));
        }

        public override void Render(Graphics g, int alpha, float scale = 1)
        {
            base.Render(g, alpha, scale);
            int stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);
            using (Font f = new Font("Calibri", 11 + stringScale))
            {
                g.DrawString("SPE", f, Brushes.Black, new PointF(Center.X - 20, Center.Y - 10));
            }
        }

        private void SetValvePosition(TwoPositionState pos)
        {
            m_valve.SetPosition(pos);
        }

        protected override void SetDevice(IDevice device)
        {
            m_valve = device as ISolidPhaseExtractor;
            try
            {
                m_valve.PositionChanged += new EventHandler<ValvePositionEventArgs<TwoPositionState>>(m_valve_PositionChanged);
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
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            Dictionary<TwoPositionState, List<Tuple<int, int>>> states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();
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
            int pos = (int)m_currentState;
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
