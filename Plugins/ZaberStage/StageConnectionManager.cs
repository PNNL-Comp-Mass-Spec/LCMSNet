using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using LcmsNetSDK.Logging;
using Zaber.Motion.Ascii;

namespace LcmsNetPlugins.ZaberStage
{
    /// <summary>
    /// Class to allow multiple "devices" to connect via a single controller, but block multiple devices controlling the same stage
    /// </summary>
    internal class StageConnectionManager : IDisposable
    {
        public static StageConnectionManager Instance { get; private set; } = new StageConnectionManager();

        private readonly Dictionary<string, ConnectionStages> connections = new Dictionary<string, ConnectionStages>();

        private class ConnectionStages
        {
            public Connection Connection { get; }
            public List<StageBase> Stages { get; }

            public ConnectionStages(Connection connection)
            {
                Connection = connection;
                Stages = new List<StageBase>(3);
            }
        }

        private readonly List<long> activeStageSerialNumbers = new List<long>(10);
        private readonly object usedStagesLocker = new object();

        public SourceList<ConnectionStageID> ConnectionSerials { get; } = new SourceList<ConnectionStageID>();

        private readonly Dictionary<string, DateTime> lastReadTime = new Dictionary<string, DateTime>();

        public void Dispose()
        {
            foreach (var conn in connections)
            {
                conn.Value.Connection.Close();
                conn.Value.Connection.Dispose();
            }

            connections.Clear();
        }

        public Connection OpenConnection(StageBase stage, string portName = "")
        {
            if (string.IsNullOrEmpty(portName))
            {
                portName = stage.PortName;
            }

            var port = portName.ToUpper();
            if (connections.TryGetValue(port, out var connectionStages))
            {
                if (!connectionStages.Stages.Contains(stage))
                {
                    connectionStages.Stages.Add(stage);
                }

                return connectionStages.Connection;
            }

            var conn = Connection.OpenSerialPort(port);
            conn.EnableAlerts();

            var connStages = new ConnectionStages(conn);
            connStages.Stages.Add(stage);

            connections.Add(port, connStages);

            return conn;
        }

        public void CloseConnection(StageBase stage, string portName = "")
        {
            if (string.IsNullOrEmpty(portName))
            {
                portName = stage.PortName;
            }

            var port = portName.ToUpper();
            if (connections.TryGetValue(port, out var connectionStages) && connectionStages.Stages.Contains(stage))
            {
                connectionStages.Stages.Remove(stage);

                if (connectionStages.Stages.Count == 0)
                {
                    connections.Remove(port);
                    connectionStages.Connection.Dispose();
                }
            }
        }

        public void ReadStagesForConnection(string portName)
        {
            if (lastReadTime.TryGetValue(portName, out var lastRead) && lastRead.AddSeconds(30) > DateTime.Now)
            {
                return;
            }

            if (connections.TryGetValue(portName.ToUpper(), out var connStage))
            {
                ReadStagesForConnection(portName, connStage.Connection);
                return;
            }

            try
            {
                using (var conn = Connection.OpenSerialPort(portName))
                {
                    ReadStagesForConnection(portName, conn);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, $"Could not read Zaber devices for port '{portName}'", ex);
            }
        }

        public void ReadStagesForConnection(string portName, Connection connection)
        {
            try
            {
                var devices = connection.DetectDevices();
                ReadStagesForConnection(portName, devices);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, $"Could not read Zaber devices for port '{portName}'", ex);
            }
        }

        public void ReadStagesForConnection(string portName, Device[] devices)
        {
            lastReadTime[portName] = DateTime.Now;
            UpdateConnectionStageIDs(portName, devices.Select(x => new ConnectionStageID(portName, x.SerialNumber, x.Name)));
        }

        public void UpdateConnectionStageIDs(string portName, IEnumerable<ConnectionStageID> newIds)
        {
            ConnectionSerials.Edit(list =>
            {
                var existing = list.Where(x => x.PortName == portName).ToList();
                var toRemove = existing.Where(x => newIds.All(y => x.SerialNumber != y.SerialNumber));
                var toAdd = newIds.Where(x => existing.All(y => x.SerialNumber != y.SerialNumber));

                list.RemoveMany(toRemove);
                list.AddRange(toAdd);
            });
        }

        public bool IsStageUsed(long serialNumber)
        {
            return activeStageSerialNumbers.Contains(serialNumber);
        }

        /// <summary>
        /// Add used stages
        /// </summary>
        /// <param name="requestedStages"></param>
        /// <returns>True if stages were unused</returns>
        public bool AddUsedStages(IEnumerable<StageControl> requestedStages)
        {
            return AddUsedStages(requestedStages.Select(x => x.SerialNumber));
        }

        /// <summary>
        /// Add used stages
        /// </summary>
        /// <param name="requestedStageSerialNumbers"></param>
        /// <returns>True if stages were unused</returns>
        public bool AddUsedStages(IEnumerable<long> requestedStageSerialNumbers)
        {
            lock (usedStagesLocker)
            {
                foreach (var stageSerialNumber in requestedStageSerialNumbers)
                {
                    if (stageSerialNumber <= 0)
                    {
                        continue;
                    }

                    if (activeStageSerialNumbers.Contains(stageSerialNumber))
                    {
                        return false;
                    }

                    activeStageSerialNumbers.Add(stageSerialNumber);
                }
            }

            return true;
        }

        public void RemoveUsedStages(IEnumerable<StageControl> freedStages)
        {
            var stages = freedStages.ToList();
            RemoveUsedStages(stages.Select(x => x.AttachedSerialNumber));

            foreach (var stage in stages)
            {
                stage.DeviceRef = null;
            }
        }

        public void RemoveUsedStages(IEnumerable<long> freedStageSerialNumbers)
        {
            lock (usedStagesLocker)
            {
                foreach (var stageSerialNumber in freedStageSerialNumbers)
                {
                    if (stageSerialNumber <= 0)
                    {
                        continue;
                    }

                    // Returns true if removed, false if it can't remove it or the item is not found
                    activeStageSerialNumbers.Remove(stageSerialNumber);
                }
            }
        }
    }
}
