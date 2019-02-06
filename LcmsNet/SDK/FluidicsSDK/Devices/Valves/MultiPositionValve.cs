using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using LcmsNetData;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices.Valves
{
    /// <summary>
    /// Represents a valve with multiple positions
    /// </summary>
    // can technically have 1-16 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public abstract class MultiPositionValve<T, T1> : FluidicsDevice where T : Enum where T1 : class, IMultiPositionValve<T>
    {
        #region Members
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        private const int _radius = 75;
        // number of ports the valve has
        private readonly int _numberOfPositions;
        protected Dictionary<int, List<Tuple<int, int>>> _states;
        protected T1 Valve { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPositions">the number of positions the valve will have</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        protected MultiPositionValve(int numberOfPositions, int xOffset = 2, int yOffset = 2) :
            base()
        {
            _numberOfPositions = numberOfPositions;
            _states = SetupStates();
            CurrentState = 1;
            Offset = new Point(xOffset, yOffset);
            AddCircle(Offset, _radius, Colors.Black, Brushes.White, fill: true);
            m_info_controls_box = new Rect(Loc.X, Loc.Y + (int)Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);
            var portLocs = GeneratePortLocs();
            foreach (var p in portLocs)
            {
                AddPort(p);
            }
            MaxVariance = 5;
            Source = false;
            Sink = false;

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
        /// The x,y offset of the main drawing to compensate for line width, ports, etc.
        /// </summary>
        protected readonly Point Offset;

        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of Point types</returns>
        private Point[] GeneratePortLocs()
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at pi/2, pi, 3pi/2, and 2pi.
            var currentAngle = (3 * Math.PI) / 2;
            var angle = (2 * Math.PI) / (_numberOfPositions);

            var points = new Point[_numberOfPositions + 1];
            points[0] = new Point(Center.X, Center.Y);
            for (var i = 1; i <= _numberOfPositions; i++)
            {
                // place them on a circle at a radius of m_radius/2 from the center
                // of the valve.
                var x = (int)((_radius * .75) * Math.Cos(currentAngle)) + Center.X;
                var y = (int)((_radius * .75) * Math.Sin(currentAngle)) + Center.Y;
                points[i] = new Point(x, y);

                currentAngle -= angle;
            }
            return points;
        }

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
            for (var i = 1; i <= _numberOfPositions; i++)
            {
                states.Add(i, GenerateState(0, i));
            }
            //states.Add(FifteenPositionState.P9, GenerateState(-1, -1));
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
            Valve.SetPosition(GetPositionForState(state));
        }

        protected abstract int LowestPosition { get; }
        protected abstract int HighestPosition { get; }
        protected abstract int UnknownPosition { get; }
        protected abstract T CurrentPosition { get; }

        protected abstract T GetPositionForState(int state);

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
            return CurrentPosition.GetEnumDescription();
        }

        protected abstract T1 GetCastDevice(IDevice device);

        protected override void SetDevice(IDevice device)
        {
            Valve = GetCastDevice(device);
            try
            {
                if (Valve != null)
                    Valve.PosChanged += Valve_PositionChanged;
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
            Valve = null;
        }

        protected override List<Tuple<int, int>> GenerateState(int startingPortIndex, int endingPortIndex)
        {
            //the eleven port valve connects the center port to one of the outer ports, so it's states are unique
            //so we override the generation.
            var stateConnection = new List<Tuple<int, int>>();
            //if startingPortIndex == -1, return empty list representing a "no connections" state
            if (startingPortIndex == -1) { return stateConnection; }
            stateConnection.Add(new Tuple<int, int>(startingPortIndex, endingPortIndex));
            return stateConnection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center => ((Graphic.FluidicsCircle)m_primitives[PRIMARY_PRIMITIVE]).Center;

        public override int CurrentState { get; set; }

        #endregion
    }
}
