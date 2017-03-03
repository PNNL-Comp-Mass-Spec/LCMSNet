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
using System.Text;
using LcmsNetDataClasses;
using LcmsNetSDK;
using FluidicsSDK.Base;
using FluidicsSDK.Managers;
using System.Drawing;

namespace FluidicsSDK.ModelCheckers
{
    public class FluidicsCycleCheck:IFluidicsModelChecker
    {
        private List<string> m_notifications;

        private const string cycle = "Cycle in physical configuration";

        public FluidicsCycleCheck()
        {
            Name = "Cyclical Path";
            IsEnabled = true;
            m_notifications = new List<string> { cycle };
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
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var status = new List<ModelStatus>();
            var sources = PortManager.GetPortManager.Ports.FindAll(x => x.Source == true);
            var cycleFound = false;
            foreach (var source in sources)
            {
                var pathTaken = new List<Connection>();
                cycleFound = FindCycles(source, new List<Port>(), pathTaken);
                if (cycleFound)
                {
                    foreach(var connection in pathTaken)
                    {
                        connection.Color = Color.Red;
                    }
                    status.Add(new ModelStatus("Cycle found", "Cycle found in path", Category, null, LcmsNetSDK.TimeKeeper.Instance.Now.ToString(), null, source.ParentDevice.IDevice));
                    if (StatusUpdate != null)
                    {
                        const string message = "Cycle in physical configuration";
                        var deviceName = source.ParentDevice.IDevice.Name;
                        StatusUpdate(this, new LcmsNetDataClasses.Devices.classDeviceStatusEventArgs(LcmsNetDataClasses.Devices.enumDeviceStatus.Initialized, message, deviceName, this));
                    }
                }
            }
            watch.Stop();
            //LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Cycle check time elapsed: " + watch.Elapsed.TotalMilliseconds.ToString() + "ms");
            return status;
        }

        //Uses a depth-first search to find cycles.
        private bool FindCycles(Port startingSource, List<Port> visitedPorts, List<Connection> pathTaken, Connection PrevConnection=null)
        {
            var cycleFound = false;
            visitedPorts.Add(startingSource);
            // check where every connection from the starting source goes
            foreach(var conn in startingSource.Connections)
            {                
                var otherEnd = conn.FindOppositeEndOfConnection(startingSource);
                if (PrevConnection == null || conn.ID != PrevConnection.ID) // if conn.ID is the same as PrevConnection.ID, ignore it, since that's the connection we just traveled, and we don't want to go backwards as this would be detected as a cycle, when in reality it is not.
                {
                    pathTaken.Add(conn);
                    // If at any point we find a port we've already been to, there is a cycle in the graph,
                    // or in other words, we have connections that lead in a "circular" path back to a place we've already been
                    if (visitedPorts.Contains(otherEnd))
                    {                        
                        cycleFound = true;
                        return cycleFound;
                    }
                    else
                    {                       
                        cycleFound = FindCycles(otherEnd, visitedPorts, pathTaken, conn);
                    }
                    if (cycleFound) { return cycleFound; }
                }                
            }        
            return cycleFound;
        }


        public List<string> GetStatusNotificationList()
        {
            return m_notifications;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }
    }
}