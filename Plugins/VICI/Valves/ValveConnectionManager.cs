using System;
using System.Collections.Generic;
using LcmsNetSDK.Logging;

namespace LcmsNetPlugins.VICI.Valves
{
    /// <summary>
    /// Class to allow multiple "devices" to connect via a single serial port, but block multiple devices controlling the same valve
    /// </summary>
    internal class ValveConnectionManager : IDisposable
    {
        public static ValveConnectionManager Instance { get; private set; } = new ValveConnectionManager();


        private readonly Dictionary<string, ValveConnection> connections = new Dictionary<string, ValveConnection>();

        public void Dispose()
        {
            foreach (var conn in connections)
            {
                conn.Value.Close();
                conn.Value.Dispose();
            }

            connections.Clear();
        }

        public IValveConnection GetConnection(ValveConnectionID valve, int readTimeout = 500, int writeTimeout = 500)
        {
            var port = valve.PortName.ToUpper();
            if (connections.TryGetValue(port, out var connection))
            {
                if (!connection.ActiveIDs.Contains(valve.ID))
                {
                    connection.ActiveIDs.Add(valve.ID);
                }
                else
                {
                    ApplicationLogger.LogMessage(LogLevel.Warning,
                        $"Warning: Multiple valve connections attempted on port '{port}' with ID '{valve.ID}'." +
                        " Software restart may be required after saving hardware config to avoid errors.");
                }

                // Update the read timeout to the max of the devices connected to this port
                connection.ReadTimeout = Math.Max(connection.ReadTimeout, readTimeout);
                connection.WriteTimeout = Math.Max(connection.WriteTimeout, writeTimeout);

                return connection;
            }

            var valveConnection = new ValveConnection(port, readTimeout, writeTimeout);
            valveConnection.ActiveIDs.Add(valve.ID);

            connections.Add(port, valveConnection);

            return valveConnection;
        }

        /// <summary>
        /// Release the connection
        /// </summary>
        /// <param name="valve">Valve connection data</param>
        /// <param name="closePort">if true, close port if there are no active connections after releasing</param>
        public void ReleaseConnection(ValveConnectionID valve, bool closePort = true)
        {
            var port = valve.PortName.ToUpper();
            if (connections.TryGetValue(port, out var connection) && connection.ActiveIDs.Contains(valve.ID))
            {
                connection.ActiveIDs.Remove(valve.ID);

                if (connection.ActiveIDs.Count == 0 && closePort)
                {
                    connections.Remove(port);
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
