﻿/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 11/21/2013
 *
 * Last Modified 12/3/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using System.Drawing;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;

namespace FluidicsSDK.Devices
{
    /// <summary>
    /// Represents a valve with multiple positions
    /// </summary>
    // can technically have 1-16 ports, more than that and the ports start overlapping and become unclickable.
    // used as a base class for other valve glyphs
    public class MultiPositionValve: FluidicsDevice
    {
        #region Members
        // radius of the valve's circle primitive in pixels, arbitrarily chosen
        protected const int m_radius = 75;
        // number of ports the valve has
        protected int m_numberOfPorts;
        protected Dictionary<int, List<Tuple<int, int>>> m_states;
        protected int m_currentState;
        #endregion

        
        #region Methods
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="device">Idevice the object will represent on the fluidics display</param>
        /// <param name="numberOfPorts">the number of ports the valve will have</param>
        public MultiPositionValve(int numberOfPorts):
            base()
        {
            base.AddCircle(new Point(0, 0), m_radius, Color.Black, Brushes.White, fill: true);
            m_info_controls_box = new Rectangle(base.Loc.X, base.Loc.Y + (int)base.Size.Height + 5, m_primitives[PRIMARY_PRIMITIVE].Size.Width, 50);
            m_numberOfPorts = numberOfPorts;
            Point[] portLocs = GeneratePortLocs();
            foreach (Point p in portLocs)
            {
                base.AddPort(p);
            }
            MaxVariance = 5;
            base.Source = false;
            base.Sink = false;
        }


        /// <summary>
        /// generate the locations of the ports on screen relative to the valve itself
        /// </summary>
        /// <returns>an array of System.Drawing.Point types</returns>
        protected virtual Point[] GeneratePortLocs()
        {
            // angle is required to equally space ports around the center of the valve..
            // simply assign each one to an point that is on a vector corresponding to
            // an angle which is a multiple of this one. e.g. port 4 is on the vector
            // of (angle * 4). So 4 ports would be placed at pi/2, pi, 3pi/2, and 2pi.
            double currentAngle = (3*Math.PI) / 2;
            double angle = (2 * Math.PI) / (m_numberOfPorts - 1);

            Point[] points = new Point[m_numberOfPorts];
            points[0] = new Point(Center.X, Center.Y);
            for (int i = 1; i < m_numberOfPorts; i++)
            {
                // place them on a circle at a radius of m_radius/2 from the center
                // of the valve.
                int x = (int)((m_radius * .75) * Math.Cos(currentAngle)) + Center.X;
                int y = (int)((m_radius * .75) * Math.Sin(currentAngle)) + Center.Y;
                points[i] = new Point(x, y);

                currentAngle -= angle;
            }
            return points;
        }

        public override string StateString()
        {
            throw new NotImplementedException("StateString() must be implemented in class inheriting MultiPositionValveBase");
        }

        public override void ActivateState(int requestedState)
        {
            m_currentState = requestedState;
            if (m_currentState != -1) // -1 is unknown state
            {
                ActivateState(m_states[requestedState]);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// gets the center point of the valve on screen.
        /// </summary>
        public Point Center
        {
            get
            {
                return ((Graphic.FluidicsCircle)m_primitives[PRIMARY_PRIMITIVE]).Center;
            }
            private set { }
        }

        public override int CurrentState
        {
            get
            {
                return m_currentState;
            }
            set
            {
                m_currentState = value;
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
