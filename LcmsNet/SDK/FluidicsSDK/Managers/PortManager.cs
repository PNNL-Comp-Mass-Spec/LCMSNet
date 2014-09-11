/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 9/5/2013
 * 
 * Last Modified 9/20/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FluidicsSDK.Base;

namespace FluidicsSDK.Managers
{
    public class PortManager
    {
        #region Members
        private List<Port> m_ports;
        private static PortManager m_instance;
        public event EventHandler<PortChangedEventArgs<Port>> PortChanged;
        #endregion

        #region Methods

        /// <summary>
        /// default constructor
        /// </summary>
        private PortManager()
        {
            m_ports = new List<Port>();
        }


        /// <summary>
        /// find a port by its string ID
        /// </summary>
        /// <param name="portID">the ID to search for</param>
        /// <returns>A Port object, or null if the portID doesn't exist within the ports in the list</returns>
        public Port FindPort(string portID)
        {
            Port foundPort = null;
            foreach (Port port in m_ports)
            {
                int equals = string.Compare(port.ID, portID, StringComparison.CurrentCulture);
                if ( equals == 0)
                {
                    foundPort = port;
                    break;
                }
            }
            return foundPort;
        }

        /// <summary>
        /// add a list of ports to the list of ports managed
        /// </summary>
        /// <param name="port">a classPort object</param>
        public void AddPorts(List<Port> ports)
        {
            m_ports.AddRange(ports);
            if (PortChanged != null)
            {
                PortChanged(this, new PortChangedEventArgs<Port>());
            }
        }


        public void AddPort(Port port)
        {
            m_ports.Add(port);
            if (PortChanged != null)
            {
                PortChanged(this, new PortChangedEventArgs<Port>());
            }
        }
        /// <summary>
        /// remove a port from the lsit of ports managed
        /// </summary>
        /// <param name="port">a classPort object</param>
        public void RemovePort(Port port)
        {
            m_ports.Remove(port);
            if (PortChanged != null)
            {
                PortChanged(this, new PortChangedEventArgs<Port>());
            }
        }

        /// <summary>
        /// remove a list of ports that belong to specified device
        /// </summary>
        /// <param name="ports">a list of classPort objects</param>
        public void RemovePorts(FluidicsDevice device)
        {

            foreach (Port port in new List<Port>(m_ports))
            {
                if (port.ParentDevice == device)
                {
                    m_ports.Remove(port);                    
                }
            }
        }
         
        /// <summary>
        /// Render ports to the screen
        /// </summary>
        /// <param name="g">a System.Drawing.Graphics object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the ports with</param>
        /// <param name="scale">a float representing the scale at which to draw the ports</param>
        public void Render(Graphics g, int alpha, float scale)
        {
            foreach (Port port in m_ports)
            {
                port.Render(g, alpha, scale);
            }
        }

        /// <summary>
        /// tries to find a port at the specified location
        /// </summary>
        /// <param name="location">a System.Drawing.Point representing the location clicked</param>
        /// <returns>a classPort object if one is found, or null otherwise</returns>
        internal Port Select(Point location)
        {
            //reverse the ports list, search from newest created to oldest created.
            List<Port> tmpList = new List<Port>(m_ports);
            tmpList.Reverse();
            foreach(Port port in tmpList)
            {
                if(port.Contains(location))
                {
                    return port;
                }
            }
            return null;
        }

        /// <summary>
        /// confirm selection of a port
        /// </summary>
        /// <param name="port">a classPort object</param>
        internal void ConfirmSelect(Port port)
        {
            port.Select();
        }

        /// <summary>
        /// deselect a selected port
        /// </summary>
        /// <param name="port">a classPort object</param>
        internal void Deselect(Port port)
        {
            port.Deselect();   
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property for retrieving the class instance of the port manager
        /// </summary>
        public static PortManager GetPortManager
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new PortManager();
                }
                return m_instance;
            }
        }


        public List<Port> Ports
        {
            get
            {
                return m_ports;
            }
            private set
            {
                m_ports = value;
            }
        }
        #endregion
    }
}
