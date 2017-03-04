/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/16/2013
 *
 * Last Modified 12/3/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using System.Drawing;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    /// <summary>
    /// Represents a valve with two positions
    /// </summary>
    // can technically have 1-13 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public class TwoPositionValve: FluidicsDevice
    {
        #region Members
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        const int m_radius = 75;
        // number of ports the valve has
        const float M_DISTFROMCENTER = .75f;
        private readonly int m_numberOfPorts;
        protected Dictionary<TwoPositionState, List<Tuple<int, int>>> m_states;
        protected TwoPositionState m_currentState;
        #endregion

        
        #region Methods
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        public TwoPositionValve(int numberOfPorts):
            base()
        {

            AddCircle(new Point(0, 0), m_radius, Color.Black, Brushes.White, fill:true);
            m_info_controls_box = new Rectangle(Loc.X, Loc.Y + (int)Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);
            m_numberOfPorts = numberOfPorts;
            var portLocs = GeneratePortLocs();
            foreach (var p in portLocs)
            {
                AddPort(p);
            }
            MaxVariance = 5;
            Sink = false;
            Source = false;
        }


        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of System.Drawing.Point types</returns>
        private Point[] GeneratePortLocs()
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at 0, pi/2, pi, and 3pi/2.
            var angle = (2 * Math.PI) / m_numberOfPorts;
            var points = new Point[m_numberOfPorts];
            var shiftX = Center.X;
            for (var i = 0; i < m_numberOfPorts; i++)
            {
                // Position the first port above the center point, shift all other ports to correct locations after.
                var currentAngle = (Math.PI/2) + (angle * i);
                // place them on a circle at a radius of m_radius* 3/4 from the center
                // of the valve.
                var x = (int)((m_radius * M_DISTFROMCENTER) * Math.Cos(currentAngle) + shiftX);
                var y = (int)((m_radius * M_DISTFROMCENTER) * Math.Sin(currentAngle) + Center.Y);
                           
                points[i] = new Point(x, y);
            }         
            return points;
        }

        public override string StateString()
        {
            return m_currentState.ToCustomString();
        }

        public override void ActivateState(int state)
        {
            var requestedState = (TwoPositionState)state;
            m_currentState = requestedState;
            if (m_currentState != TwoPositionState.Unknown)
            {
                ActivateState(m_states[requestedState]);
            }         
        }  
        #endregion

        #region Properties
        /// <summary>
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center => ((Graphic.FluidicsCircle)m_primitives[PRIMARY_PRIMITIVE]).Center;

        public override int CurrentState
        {
            get
            {
                return (int)m_currentState;
            }
            set
            {
                m_currentState = (TwoPositionState)value;
            }
        }      
        #endregion

        protected override void SetDevice(IDevice device)
        {
            
        }

        protected override void ClearDevice(IDevice device)
        {
            
        }

    }
}
