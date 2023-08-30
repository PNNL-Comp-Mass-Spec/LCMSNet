using System;
using System.Collections.Generic;
using System.Windows;
using FluidicsSDK.Graphic;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Represents a valve with multiple positions
    /// </summary>
    // can technically have 1-16 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public class MultiPositionValve : ValveBase
    {
        protected Dictionary<int, List<Tuple<int, int>>> _states;
        private IMultiPositionValve _valve;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPositions">the number of positions the valve will have</param>
        /// <param name="portNumberingClockwise"></param>
        public MultiPositionValve(int numberOfPositions, bool portNumberingClockwise = true) : this(numberOfPositions, 2, 2, portNumberingClockwise)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPositions">the number of positions the valve will have</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="portNumberingClockwise"></param>
        protected MultiPositionValve(int numberOfPositions, int xOffset = 2, int yOffset = 2, bool portNumberingClockwise = true) :
            base(numberOfPositions, xOffset, yOffset)
        {
            NumberOfPositions = numberOfPositions;
            _states = SetupStates();
            CurrentState = 1;

            // Add the center/primary port
            AddPort(new Point(Center.X, Center.Y), 0);

            // Add the rest of the ports
            foreach (var point in GeneratePerimeterPortLocations(portNumberingClockwise))
            {
                AddPort(point.Point, point.Number);
            }

            ActivateState(_states[CurrentState]);

            var stateControlSize = new Size(15, 15);
            var stateControl1Loc = new Point(Center.X - (stateControlSize.Width * 3), Center.Y - (stateControlSize.Height / 2));
            var stateControl2Loc = new Point(Center.X + stateControlSize.Width * 2, Center.Y - (stateControlSize.Height / 2));
            var stateControlRectangle = new Rect(stateControl1Loc, stateControlSize);
            var stateControlRectangle2 = new Rect(stateControl2Loc, stateControlSize);
            //add left control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle, Orient.Left), LeftButtonAction);
            //add right control
            AddPrimitive(new FluidicsTriangle(stateControlRectangle2, Orient.Right), RightButtonAction);
        }

        /// <summary>
        /// Number of positions the valve has
        /// </summary>
        public int NumberOfPositions { get; }

        public override void ActivateState(int requestedState)
        {
            CurrentState = requestedState;
            if (CurrentState != -1) // -1 is unknown state
            {
                ActivateState(_states[requestedState]);
            }
        }

        /// <summary>
        /// Setup the devices states
        /// </summary>
        /// <returns>a dictionary of with TwoPositionState enums as the keys and lists of tuples of int, int type as the values </returns>
        private Dictionary<int, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<int, List<Tuple<int, int>>>();
            for (var i = 1; i <= NumberOfPositions; i++)
            {
                states.Add(i, GenerateState(0, i));
            }
            return states;
        }

        /// <summary>
        /// Change the position of the valve by one position
        /// </summary>
        /// <param name="left"></param>
        private void ChangePosition(bool left)
        {
            var state = CurrentState;
            if (CurrentState != UnknownPosition)
            {
                if (left)
                {
                    state--;
                    state = state < LowestPosition ? HighestPosition : state;
                }
                else
                {
                    state++;
                    state = state > HighestPosition ? LowestPosition : state;
                }
            }
            else
            {
                state = LowestPosition;
            }
            _valve.SetPosition(state);
        }

        protected int LowestPosition => 1;
        protected int HighestPosition => NumberOfPositions;
        protected int UnknownPosition => -1;
        protected int CurrentPosition { get; }

        private void LeftButtonAction()
        {
            ChangePosition(true);
        }

        private void RightButtonAction()
        {
            ChangePosition(false);
        }

        public override string StateString()
        {
            return LowestPosition <= CurrentState && CurrentState <= HighestPosition ? CurrentState.ToString() : "Unknown";
        }

        protected override void SetDevice(IDevice device)
        {
            _valve = device as IMultiPositionValve;
            try
            {
                if (_valve != null)
                    _valve.PosChanged += Valve_PositionChanged;
            }
            catch (Exception)
            {
                // MessageBox.Show("Null valve: " + ex.Message);
            }
        }

        void Valve_PositionChanged(object sender, ValvePositionEventArgs<int> e)
        {
            ActivateState(e.Position);
        }

        protected override void ClearDevice(IDevice device)
        {
            _valve = null;
        }

        protected virtual List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
        {
            //the eleven port valve connects the center port to one of the outer ports, so it's states are unique
            //so we override the generation.
            var stateConnection = new List<Tuple<int, int>>();
            //if startingPortIndex == -1, return empty list representing a "no connections" state
            if (startingPortIndex == -1) { return stateConnection; }
            stateConnection.Add(new Tuple<int, int>(startingPortIndex, endingPortIndex));
            return stateConnection;
        }

        public override int CurrentState { get; set; }
    }
}
