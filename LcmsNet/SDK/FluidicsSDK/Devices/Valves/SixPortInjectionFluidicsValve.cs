using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using System.Drawing;
using FluidicsSDK.Managers;
using FluidicsSDK.Graphic;
using LcmsNetDataClasses.Devices;
using System.Windows.Forms;

namespace FluidicsSDK.Devices
{
    public class SixPortInjectionFluidicsValve : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;
        ISixPortInjectionValve m_valve;

        public SixPortInjectionFluidicsValve() :
            base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = TwoPositionState.PositionA;
            base.ActivateState(m_states[m_currentState]);
            var StateControlSize = new Size(15, 15);
            var StateControl1Loc = new Point(base.Center.X - (StateControlSize.Width * 2), base.Center.Y - (StateControlSize.Height / 2));
            var StateControl2Loc = new Point(base.Center.X + StateControlSize.Width, base.Center.Y - (StateControlSize.Height / 2));
            var StateControlRectangle = new Rectangle(StateControl1Loc, StateControlSize);
            var StateControlRectangle2 = new Rectangle(StateControl2Loc, StateControlSize);
            //add left control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle, Orient.Left), new Action(LeftButtonAction));
            //add right control
            base.AddPrimitive(new FluidicsTriangle(StateControlRectangle2, Orient.Right), new Action(RightButtonAction));
            //add top "injection port"
            base.AddPrimitive(new FluidicsRectangle(new Point(Loc.X + (int)Size.Width/4, Loc.Y - 30), new Size((int)Size.Width/2, 10), Color.Black,  Brushes.White));
            base.AddPrimitive(new FluidicsRectangle(new Point(Loc.X + (((int)Size.Width / 2) -5), Loc.Y - 20), new Size(10, 20), Color.Black,  Brushes.White));
            // add injection loop
            base.AddPrimitive(new FluidicsLine(m_portList[2].Center, m_portList[5].Center));
            base.AddPrimitive(new FluidicsRectangle(new Point(Center.X - 25, Center.Y - 15), new Size(50,30), Color.Black, Brushes.White, true, 1));
        }


        public override void Render(Graphics g, int alpha, float scale = 1)
        {
            base.Render(g, alpha, scale);
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);
            using(var f = new Font("Calibri", 11 + stringScale))
            {
                g.DrawString(Volume.ToString() + " \u00b5" + "L", f, Brushes.Black, new PointF((Center.X * scale - 20), (Center.Y * scale - 10)));
            }
        }
        
        public override Point Loc
        {
            get
            {
                return base.Loc;
            }
            protected set
            {
                base.Loc = value;
            }
        }

        private void SetValvePosition(TwoPositionState pos)
        {
            m_valve.SetPosition(pos);
        }

        protected override void SetDevice(IDevice device)
        {
            m_valve = device as ISixPortInjectionValve;
            try
            {
                Volume = m_valve.InjectionVolume;
                m_valve.PositionChanged += new EventHandler<ValvePositionEventArgs<TwoPositionState>>(m_valve_PositionChanged);
                m_valve.InjectionVolumeChanged += new EventHandler(m_valve_InjectionVolumeChanged);
            }
            catch (Exception)
            {
                // MessageBox.Show("This is crap..." + ex.Message);
            }
        }

        void m_valve_InjectionVolumeChanged(object sender, EventArgs e)
        {
            Volume = m_valve.InjectionVolume;
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs());
        }

        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();
            states.Add(TwoPositionState.PositionB, GenerateState(0, Ports.Count - 1));
            states.Add(TwoPositionState.PositionA, GenerateState(1, Ports.Count));
            return states;
        }


        void m_valve_PositionChanged(object sender, ValvePositionEventArgs<TwoPositionState> e)
        {
            ActivateState((int)e.Position);
        }
        
        /// <summary>
        /// Take a list of tuples and use it to create the internal connections
        /// of the device.
        /// </summary>
        /// <param name="state">a list of tuples, each tuple represents a single internal connection</param>
        public override void ActivateState(List<Tuple<int, int>> state)
        {
            classFluidicsModerator.Moderator.BeginModelSuspension();
            //remove internal connections
            foreach (var p in Ports)
            {

                foreach (var c in p.Connections)
                {
                    if (c.InternalConnectionOf == this)
                    {
                        ConnectionManager.GetConnectionManager.Remove(c, this);
                    }
                }
            }

            //create new internal connections
            foreach (var t in state)
            {
                ConnectionManager.GetConnectionManager.Connect(m_portList[t.Item1], m_portList[t.Item2], this);
            }
            // add injection loop connection.
            ConnectionManager.GetConnectionManager.Connect(m_portList[2], m_portList[5], this);
            var injectionLoopConnection =ConnectionManager.GetConnectionManager.FindConnection(m_portList[2], m_portList[5]);
            injectionLoopConnection.Transparent = true;
            classFluidicsModerator.Moderator.EndModelSuspension(true);
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

        public double Volume
        {
            get;
            set;
        }

        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;
    }
}
