/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 9/5/2013
 *
 * Last Modified 1/3/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FluidicsSDK.Base;

namespace FluidicsSDK.Managers
{
    public class ConnectionManager
    {
        #region Members
        private static ConnectionManager m_instance;
        private List<Connection> m_connections;
        public event EventHandler<ConnectionChangedEventArgs<Connection>> ConnectionChanged;
  
        #endregion


        #region Methods
        /// <summary>
        /// default constructor
        /// </summary>
        private ConnectionManager()
        {
            m_connections = new List<Connection>();
        }

        /// <summary>
        /// Find Ports that the specified connection connects.
        /// </summary>
        /// <param name="connection">a classConnection object</param>
        /// <returns>a list of classPort objects</returns>
        public List<Port> FindPorts(Connection connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a connection that connects two ports, if it exists.
        /// </summary>
        /// <param name="port1">a classPort object</param>
        /// <param name="port2">a classPort object</param>
        /// <returns>a classConnection object or null if one is not found.</returns>
        public Connection FindConnection(Port port1, Port port2)
        {
            foreach (Connection conn in m_connections)
            {
                if ((conn.P1 == port1 || conn.P1 == port2) &&
                    (conn.P2 == port1 || conn.P2 == port2))
                {
                    return conn;
                }
            }
            return null;
        }   

        /// <summary>
        /// Remove a connection from the connection manager's list
        /// </summary>
        /// <param name="connection">a classConnection object</param>
        /// <exception cref="ArgumentException">tried to remove a connection internal to a device, or a connection the manager doesn't know about</exception>"
        public void Remove(Connection connection, FluidicsDevice device = null)
        {           
            
            //if the connection is in the list remove it
            if (m_connections.Contains(connection))
            {
                //if it's an internal connection, only let devices remove it, conversely, if it's not an internal connection (internalconnectionof is null)
                //, don't let devices remove it.
                if (connection.InternalConnectionOf == device)
                {
                    m_connections.Remove(connection);
                    connection.Destroy();
                    if (ConnectionChanged != null)
                    {
                        ConnectionChanged(this, new ConnectionChangedEventArgs<Connection>());
                    }
                }
                else
                {
                    throw new ArgumentException("Internal device connection can only be removed by device.");
                }
            }
            //if not, throw an exception
            else
            {
                throw new ArgumentException("The connection is not in manager collection.");
            }          
        }


        public void RemoveConnections(Port port)
        {
            foreach (Connection conn in new List<Connection>(m_connections))
            {
                if (conn.P1 == port || conn.P2 == port)
                {
                    m_connections.Remove(conn);
                    // make sure it's removed from list of connections on the ports.
                    conn.P1.RemoveConnection(conn.ID);
                    conn.P2.RemoveConnection(conn.ID);
                }
            }
        }
        /// <summary>
        /// Connect two ports
        /// </summary>
        /// <param name="port1">a classPort object</param>
        /// <param name="port2">a classPort object</param>
        /// <exception cref="ArgumentException">tried to connect a port to itself</exception>"
        public void Connect(Port port1, Port port2, FluidicsDevice device = null, ConnectionStyles? style = null)
        {
            //if the connection doesn't already exist, try to create it, or if it's an internal device connection associated with a state, create regardless of if connection exist between them otherwise
            if (FindConnection(port1, port2) == null || (port1.ParentDevice == device && port2.ParentDevice == device && device != null) )
            {
                //but if port1 IS port 2, throw an error, as you cannot connect a port to itself.
                if (port1 != port2)
                {
                    Connection newConnection = new Connection(port1, port2, device, style);
                    m_connections.Add(newConnection);
                    if (ConnectionChanged != null)
                    {
                        ConnectionChanged(this, new ConnectionChangedEventArgs<Connection>());
                    }
                }
                else
                {
                    // should not be able to get here...
                    throw new ArgumentException("Cannot connect a port to itself!");
                }
            }
            else
            {
                throw new ArgumentException("Connection between those ports already exists");
            }
        }         

        /// <summary>
        /// Render all connections
        /// </summary>
        /// <param name="g">a System.Drawing Graphics object</param>
        /// <param name="alpha">an integer representing the requested alpha value to draw the connections with</param>
        /// <param name="scale">a float repsenting how much to scale the connections by</param>
        public void Render(Graphics g, int alpha, float scale)
        {
            foreach (Connection connection in m_connections)
            {
                connection.Render(g, alpha, scale);
            }
        }

        /// <summary>
        /// Tries to find a classConnection that exists at the specified location
        /// </summary>
        /// <param name="location"> a System.Drawing.Point representing the location clicked by the user</param>
        /// <returns>a classConnection object, if one exists at that location, or null</returns>
        internal Connection Select(Point location)
        {
            //start with most recently created connection and work down.
            List<Connection> tmpList = new List<Connection>(m_connections);
            tmpList.Reverse(); //so list is from most-recently created to first-created
            foreach (Connection connection in tmpList)
            {
                if (connection.OnPoint(location) && connection.InternalConnectionOf == null)
                {                   
                    return connection;
                }
            }
            return null;
        }

        /// <summary>
        /// Confirm selection of the specified classConnection, allows selection hilighting for user
        /// </summary>
        /// <param name="connection">a classConnection object</param>
        internal void ConfirmSelect(Connection connection)
        {
            connection.Select();

        }

        /// <summary>
        /// Deselect a classConnection
        /// </summary>
        /// <param name="Connection">a  classConnection object</param>
        internal void Deselect(Connection Connection)
        {
            Connection.Deselect();
        }

        public IEnumerable<Connection> GetConnections()
        {
            return m_connections;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property to return class instance of classConnectionManager
        /// </summary>
        public static ConnectionManager GetConnectionManager
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ConnectionManager();
                }
                return m_instance;
            }
        }

        #endregion

    }
}
