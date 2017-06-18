using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;

namespace FluidicsSDK.Managers
{
    public class PortManagerWpf
    {
        #region Members
        private readonly List<PortWpf> m_ports;
        private static PortManagerWpf m_instance;
        public event EventHandler<PortChangedEventArgs<PortWpf>> PortChanged;
        #endregion

        #region Methods

        /// <summary>
        /// default constructor
        /// </summary>
        private PortManagerWpf()
        {
            m_ports = new List<PortWpf>();
        }


        /// <summary>
        /// find a port by its string ID
        /// </summary>
        /// <param name="portID">the ID to search for</param>
        /// <returns>A Port object, or null if the portID doesn't exist within the ports in the list</returns>
        public PortWpf FindPort(string portID)
        {
            PortWpf foundPort = null;
            foreach (var port in m_ports)
            {
                var equals = string.Compare(port.ID, portID, StringComparison.CurrentCulture);
                if (equals == 0)
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
        /// <param name="ports">a list of classPort objects</param>
        public void AddPorts(List<PortWpf> ports)
        {
            m_ports.AddRange(ports);
            PortChanged?.Invoke(this, new PortChangedEventArgs<PortWpf>());
        }

        public void AddPort(PortWpf port)
        {
            m_ports.Add(port);
            PortChanged?.Invoke(this, new PortChangedEventArgs<PortWpf>());
        }
        /// <summary>
        /// remove a port from the lsit of ports managed
        /// </summary>
        /// <param name="port">a classPort object</param>
        public void RemovePort(PortWpf port)
        {
            m_ports.Remove(port);
            PortChanged?.Invoke(this, new PortChangedEventArgs<PortWpf>());
        }

        /// <summary>
        /// remove a list of ports that belong to specified device
        /// </summary>
        /// <param name="device">fluidics device</param>
        public void RemovePorts(FluidicsDeviceWpf device)
        {

            foreach (var port in new List<PortWpf>(m_ports))
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
        /// <param name="g">a System.Windows.Media.DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the ports with</param>
        /// <param name="scale">a float representing the scale at which to draw the ports</param>
        public void Render(DrawingContext g, byte alpha, float scale)
        {
            foreach (var port in m_ports)
            {
                port.Render(g, alpha, scale);
            }
        }

        /// <summary>
        /// tries to find a port at the specified location
        /// </summary>
        /// <param name="location">a System.Drawing.Point representing the location clicked</param>
        /// <returns>a classPort object if one is found, or null otherwise</returns>
        internal PortWpf Select(Point location)
        {
            //reverse the ports list, search from newest created to oldest created.
            var tmpList = new List<PortWpf>(m_ports);
            tmpList.Reverse();
            foreach (var port in tmpList)
            {
                if (port.Contains(location))
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
        internal void ConfirmSelect(PortWpf port)
        {
            port.Select();
        }

        /// <summary>
        /// deselect a selected port
        /// </summary>
        /// <param name="port">a classPort object</param>
        internal void Deselect(PortWpf port)
        {
            port.Deselect();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property for retrieving the class instance of the port manager
        /// </summary>
        public static PortManagerWpf GetPortManager => m_instance ?? (m_instance = new PortManagerWpf());


        public List<PortWpf> Ports => m_ports;

        #endregion
    }
}
