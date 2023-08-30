using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using FluidicsSDK.Managers;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    public sealed class SolidPhaseExtractor : TwoPositionValve
    {
        private const int NUMBER_OF_PORTS = 6;
        private ISolidPhaseExtractor m_valve;

        public SolidPhaseExtractor()
            : base(NUMBER_OF_PORTS)
        {
            m_states = SetupStates();
            m_currentState = TwoPositionState.PositionA;
            base.ActivateState(m_states[m_currentState]);
            var stateControlSize = new Size(15, 15);
            var stateControl1Loc = new Point(Center.X - (stateControlSize.Width * 2), Center.Y - (stateControlSize.Height / 2));
            var stateControl2Loc = new Point(Center.X + stateControlSize.Width, Center.Y - (stateControlSize.Height / 2));
            var stateControlRectangle = new Rect(stateControl1Loc, stateControlSize);
            var stateControlRectangle2 = new Rect(stateControl2Loc, stateControlSize);
            //add left control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle, Orient.Left), LeftButtonAction);
            //add right control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle2, Orient.Right), RightButtonAction);
            // add loop
            AddPrimitive(new FluidicsLine(m_portList[2].Center, m_portList[5].Center));
            AddPrimitive(new FluidicsRectangle(new Point(Center.X - 25, Center.Y - 15), new Size(50, 30), Colors.Black, Brushes.White, fill: true, atScale: 1));
        }

        public override void Render(DrawingContext g, byte alpha, float scale = 1)
        {
            base.Render(g, alpha, scale);
            var stringScale = (int)Math.Round(scale < 1 ? -(1 / scale) : scale, 0, MidpointRounding.AwayFromZero);

            var font = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var volumeText = new FormattedText("SPE", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, ElevenPoint * stringScale, Brushes.Black, FluidicsModerator.Moderator.DrawingScaleFactor);
            g.DrawText(volumeText, new Point(Center.X - 20, Center.Y - 10));
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
                if (m_valve != null)
                    m_valve.PositionChanged += m_valve_PositionChanged;
            }
            catch (Exception)
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

        /// <summary>
        /// Take a list of tuples and use it to create the internal connections
        /// of the device.
        /// </summary>
        /// <param name="state">a list of tuples, each tuple represents a single internal connection</param>
        public override void ActivateState(List<Tuple<int, int>> state)
        {
            // Overridden to maintain the SPE loop connection
            FluidicsModerator.Moderator.BeginModelSuspension();
            // Run the standard required changes, which also removes all internal connections
            ActivateStateWork(state);

            // add SPE loop connection.
            ConnectionManager.GetConnectionManager.Connect(m_portList[2], m_portList[5], this);
            var injectionLoopConnection = ConnectionManager.GetConnectionManager.FindConnection(m_portList[2], m_portList[5]);
            injectionLoopConnection.Transparent = true;

            FluidicsModerator.Moderator.EndModelSuspension(true);
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
