using System;
using System.Collections.Generic;
using FluidicsSDK.Base;
using LcmsNetSDK;

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
    }
}
