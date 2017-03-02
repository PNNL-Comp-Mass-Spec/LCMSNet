/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 * Last Modified 6/4/2014 By Christopher Walters
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using FluidicsSDK.Base;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;
using System.Drawing;

namespace FluidicsSDK.ModelCheckers
{
    /// <summary>
    /// Model check to ensure that each fluid source has a path leading to a fluid sink to exit out of the system
    /// </summary>
    public class NoSinksModelCheck: IFluidicsModelChecker
    {
        private readonly List<string> m_status;
        private const string NO_PATH_FOUND = "No Sink Found";

        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<classDeviceErrorEventArgs> Error
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
        /// <returns>true if a sink is found, false otherwise</returns>
        private static bool FindSinks(Port fromPort, out List<Connection> pathTaken)
        {
            // if visitedPorts is null, this is our starting point and we should create a list of visited ports
            Queue<Port> unexplored = new Queue<Port>();
            List<Port> visitedPorts = new List<Port>();
            pathTaken = new List<Connection>();
            unexplored.Enqueue(fromPort);
            while (unexplored.Count > 0)
            {
                Port unexploredPort = unexplored.Dequeue();
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

        public string Name
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public ModelStatusCategory Category
        {
            get;
            set;
        }
        public IEnumerable<ModelStatus> CheckModel()
        {
            const string message = "No fluidics path found from source {0} to a sink";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var status = new List<ModelStatus>();
            //for each port in the device, see if it leads to a sink, if and only if, that port has not been previously tested and that port is a source.
            var sources = Managers.PortManager.GetPortManager.Ports.FindAll(x => x.Source);
            foreach (var p in sources)
            {                
                List<Connection> pathTaken;
                var sinkFound = FindSinks(p, out pathTaken);

                // if found a sink for this path, move on to the next source and check it
                if (sinkFound) continue;

                //No sink was found, so color the path red.
                foreach(Connection connection in pathTaken)
                {
                    connection.Color = Color.Red;
                }

                //if no sink is found, report to the notifcation system and add to the status list to be returned
                if (StatusUpdate != null)
                {
                    var deviceName = p.ParentDevice.DeviceName;
                    StatusUpdate(this, new classDeviceStatusEventArgs(enumDeviceStatus.Initialized, NO_PATH_FOUND, message,  this));
                }
                status.Add(new ModelStatus("No Sink on Path", "No sink on on path", Category, string.Empty,string.Empty, null, p.ParentDevice.IDevice));
            }
            watch.Stop();
            //LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "NoSink check time elapsed: " + watch.Elapsed.TotalMilliseconds.ToString() + "ms");
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

    }
}
