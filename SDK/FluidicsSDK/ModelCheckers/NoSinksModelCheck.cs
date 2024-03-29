﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Base;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.ModelCheckers
{
    /// <summary>
    /// Model check to ensure that each fluid source has a path leading to a fluid sink to exit out of the system
    /// </summary>
    public class NoSinksModelCheck: IFluidicsModelChecker
    {
        private readonly List<string> m_status;
        private const string NO_PATH_FOUND = "No Sink Found";

        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<DeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }


        public NoSinksModelCheck()
        {
            Name        = "No Fluid Sink on Path";
            IsEnabled   = true;
            m_status    = new List<string> {NO_PATH_FOUND};
        }

        /// <summary>
        /// Determines if a sink out of the system exists from a given source.
        /// </summary>
        /// <param name="fromPort">the port to look for sinks from</param>
        /// <param name="pathTaken"></param>
        /// <returns>true if a sink is found, false otherwise</returns>
        private static bool FindSinks(Port fromPort, out List<Connection> pathTaken)
        {
            // if visitedPorts is null, this is our starting point and we should create a list of visited ports
            var unexplored = new Queue<Port>();
            var visitedPorts = new List<Port>();
            pathTaken = new List<Connection>();
            unexplored.Enqueue(fromPort);
            while (unexplored.Count > 0)
            {
                var unexploredPort = unexplored.Dequeue();
                visitedPorts.Add(unexploredPort);
                // trace each connection leaving fromPort.
                foreach (var connection in unexploredPort.Connections)
                {
                    // grab the port on the other end of the connection
                    var destinationPort = connection.FindOppositeEndOfConnection(unexploredPort);
                    // only check for sink if the port is NOT in the same device
                    // don't go backwards, if visitedPorts contains destinationPort, move on. (also ignores cycles in the graph)
                    // if destinationPort is a sink, return true
                    if (destinationPort.ParentDevice != fromPort.ParentDevice && destinationPort.Sink)
                    {
                        return true;
                    }
                    // otherwise, we start the search again at destinationPort, if and only if, we have not already scanned that port.
                    if (!visitedPorts.Contains(destinationPort))
                    {
                        pathTaken.Add(connection);
                        unexplored.Enqueue(destinationPort);
                    }
                }
            }
            // No sink was found.
            return false;
        }

        private string name;
        private bool isEnabled;
        private ModelStatusCategory category;

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }

        public ModelStatusCategory Category
        {
            get => category;
            set => this.RaiseAndSetIfChanged(ref category, value);
        }

        public IEnumerable<ModelStatus> CheckModel()
        {
            const string message = "No fluidics path found from source {0} to a sink";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var status = new List<ModelStatus>();
            //for each port in the device, see if it leads to a sink, if and only if, that port has not been previously tested and that port is a source.
            var sources = Managers.PortManager.GetPortManager.Ports.FindAll(x => x.Source);
            foreach (var p in sources)
            {
                var sinkFound = FindSinks(p, out var pathTaken);

                // if found a sink for this path, move on to the next source and check it
                if (sinkFound) continue;

                //No sink was found, so color the path red.
                foreach(var connection in pathTaken)
                {
                    connection.Color = Colors.Red;
                }

                //if no sink is found, report to the notification system and add to the status list to be returned
                StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(DeviceStatus.Initialized, NO_PATH_FOUND,  this, message));
                status.Add(new ModelStatus("No Sink on Path", "No sink on on path", Category, string.Empty,string.Empty, null, p.ParentDevice.IDevice));
            }
            watch.Stop();
            //LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "NoSink check time elapsed: " + watch.Elapsed.TotalMilliseconds.ToString() + "ms");
            return status;
        }

        public List<string> GetStatusNotificationList()
        {
            return m_status;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
