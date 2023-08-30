using System;
using System.Collections.Generic;
using System.Windows;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Represents a valve with two positions
    /// </summary>
    // can technically have 1-13 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public abstract class TwoPositionValve : ValveBase
    {
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> m_states;
        protected TwoPositionState m_currentState;
        private ITwoPositionValve m_valve;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="portNumberingClockwise"></param>
        protected TwoPositionValve(int numberOfPorts, int xOffset = 2, int yOffset = 2, bool portNumberingClockwise = false) :
            base(numberOfPorts, xOffset, yOffset)
        {
            foreach (var point in GeneratePerimeterPortLocations(portNumberingClockwise))
            {
                AddPort(point.Point, point.Number);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        /// <param name="positionAis1To2">True if position A connects ports 1 and 2; false if position A connects ports N and 1</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="portNumberingClockwise"></param>
        protected TwoPositionValve(int numberOfPorts, bool positionAis1To2, int xOffset = 2, int yOffset = 2, bool portNumberingClockwise = false) : this(numberOfPorts, xOffset, yOffset, portNumberingClockwise)
        {
            m_states = SetupStates(positionAis1To2);
            m_currentState = TwoPositionState.PositionA;
            ActivateState(m_states[m_currentState]);
            var stateControlSize = new Size(15, 15);
            var stateControl1Loc = new Point(Center.X - (stateControlSize.Width * 2), Center.Y - (stateControlSize.Height / 2));
            var stateControl2Loc = new Point(Center.X + stateControlSize.Width, Center.Y - (stateControlSize.Height / 2));
            var stateControlRectangle = new Rect(stateControl1Loc, stateControlSize);
            var stateControlRectangle2 = new Rect(stateControl2Loc, stateControlSize);
            //add left control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle, Orient.Left), LeftButtonAction);
            //add right control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle2, Orient.Right), RightButtonAction);
        }

        private protected void SetBaseDevice(ITwoPositionValve valve)
        {
            m_valve = valve;
            try
            {
                //m_valve.SetPosition(10);
                if (m_valve != null)
                    m_valve.PositionChanged += ValvePositionChanged;
            }
            catch (Exception)
            {
                // MessageBox.Show("Null valve: " + ex.Message);
            }
        }

        private protected ITwoPositionValve GetBaseDevice()
        {
            return m_valve;
        }

        void ValvePositionChanged(object sender, ValvePositionEventArgs<TwoPositionState> e)
        {
            ActivateState((int)e.Position);
        }

        protected override void ClearDevice(IDevice device)
        {
            m_valve = null;
        }

        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <param name="positionAis1To2">True if position A connects ports 1 and 2; false if position A connects ports N and 1</param>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        private Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates(bool positionAis1To2)
        {
            // https://www.vici.com/support/app/2p_japp.php has usage examples that also show positions
            if (positionAis1To2)
            {
                // Valco non-Cheminert 2-position valves (some; port numbers are clockwise)
                return new Dictionary<TwoPositionState, List<Tuple<int, int>>>
                {
                    { TwoPositionState.PositionA, GenerateState(0, Ports.Count - 1) },
                    { TwoPositionState.PositionB, GenerateState(1, Ports.Count) }
                };
            }

            // Valco Cheminert valves (port numbers are counterclockwise, position A is n-1)
            return new Dictionary<TwoPositionState, List<Tuple<int, int>>>
            {
                {TwoPositionState.PositionB, GenerateState(0, Ports.Count - 1)},
                {TwoPositionState.PositionA, GenerateState(1, Ports.Count)}
            };
        }

        public override string StateString()
        {
            return m_currentState.GetEnumDescription();
        }

        public override void ActivateState(int state)
        {
            var requestedState = (TwoPositionState) state;
            m_currentState = requestedState;
            if (m_currentState != TwoPositionState.Unknown)
            {
                ActivateState(m_states[requestedState]);
            }
        }

        /// <summary>
        /// generates a single 'state' of connections, by connecting pairs of ports, starting
        /// with the ports at startingPortIndex and startingPortIndex+1, then startingPortIndex+2 and startingPortIndex+3,
        /// etc.
        /// </summary>
        /// <param name="startingPortIndex">an index into m_ports</param>
        /// <param name="endingPortIndex">an index into m_ports</param>
        /// <returns>a list of int,int tuples</returns>
        protected List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
        {
            var connectionState = new List<Tuple<int, int>>();
            if (startingPortIndex != -1)
            {
                //make sure that this isn't a connectionless state
                if (m_portList.Count - 1 <= endingPortIndex)
                {
                    for (var i = startingPortIndex; i < endingPortIndex; i += 2)
                    {
                        Tuple<int, int> connectionTuple;
                        //this if handles the case where you want to connect the last port with the first port(example: connect port 3 and port 0
                        //in a four port valve)
                        if (i == endingPortIndex - 1 && startingPortIndex != 0)
                        {
                            connectionTuple = new Tuple<int, int>(i, 0);
                        }
                        else
                        {
                            connectionTuple = new Tuple<int, int>(i, i + 1);
                        }
                        connectionState.Add(connectionTuple);
                    }
                }
            }
            //will return an empty list if startingPortIndex is -1, represents a "no connections" state.
            return connectionState;
        }

        public override int CurrentState
        {
            get => (int) m_currentState;
            set => m_currentState = (TwoPositionState) value;
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
