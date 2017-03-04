/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/22/2013
 *
 * Last Modified 10/17/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    public sealed class ContactClosure : FluidicsDevice
    {

        #region members
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

        #endregion

        #region Methods

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
            AddRectangle(new Point(0, 0), new Size(LENGTH, WIDTH), Color.Black, Brushes.White);
            MaxVariance = MAX_PIXEL_VARIANCE;
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="loc">Point representing  location on screen to draw the pump(upper left corner)</param>
        public ContactClosure(Point loc)
        {
            AddRectangle(loc, new Size(LENGTH, WIDTH), Color.Black, Brushes.White);
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
        #endregion

        #region Properties

        public override int CurrentState
        {
            get
            {
                //pumps don't have a state of this kind.
                return -1;
            }
            set
            {
                //do nothing
            }
        }
        #endregion

    }
}
