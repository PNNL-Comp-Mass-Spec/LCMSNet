using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Devices
{
    public sealed class ContactClosure : FluidicsDevice
    {
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

        IContactClosure m_pump;

        public override void ActivateState(int state)
        {
            // do nothing
            //TwoPositionState requestedState = (TwoPositionState)state;
            //ActivateState(m_states[requestedState]);
            //m_currentState = requestedState;
        }

        /// <summary>
        /// base class constructor
        /// </summary>
        public ContactClosure()
        {
            AddRectangle(new Point(2, 2), new Size(LENGTH, WIDTH), Colors.Black, Brushes.White);
            MaxVariance = MAX_PIXEL_VARIANCE;
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">Point representing  location on screen to draw the pump(upper left corner)</param>
        public ContactClosure(Point loc)
        {
            AddRectangle(loc, new Size(LENGTH, WIDTH), Colors.Black, Brushes.White);
        }

        /// <summary>
        /// setup the states
        /// </summary>
        /// <returns></returns>
        private Dictionary<TwoPositionState, List<Tuple<int, int>>> SetupStates()
        {
            var states = new Dictionary<TwoPositionState, List<Tuple<int, int>>>();
            // contact closure has no states, it is a source
            return states;
        }

        protected override void SetDevice(IDevice device)
        {
            m_pump = device as IContactClosure;
        }

        protected override void ClearDevice(IDevice device)
        {

        }

        public override string StateString()
        {
            return string.Empty;
        }

        public override int CurrentState
        {
            //pumps don't have a state of this kind.
            get => -1;
            set
            {
                //do nothing
            }
        }
    }
}
