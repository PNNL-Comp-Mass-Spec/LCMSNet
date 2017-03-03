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
    public class MultipleSourcesModelCheck:IFluidicsModelChecker
    {
        private readonly List<string> m_notifications;
        private const string notification = "Multiple Sources";
        public MultipleSourcesModelCheck()
        {
            Name        = "Multiple Fluid Sources on Path";
            IsEnabled   = true;
            m_notifications = new List<string> { notification };
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
            var sources = PortManager.GetPortManager.Ports.FindAll(x => x.Source);
            var otherSourceFound = false;
            
            foreach(var source in sources)
            {
                var visitedPorts = new List<Port>();
                List<Connection> pathTaken;
                otherSourceFound = FindOtherSources(source, out pathTaken);
                if (otherSourceFound)
                {
                    // Multiple sources found on a path, color path red.
                    foreach(var connection in pathTaken)
                    {
                        connection.Color = Color.Red;
                    }
                    if (StatusUpdate != null)
                    {
                        const string message = "Multiple Sources";
                        var deviceName = source.ParentDevice.DeviceName;
                        StatusUpdate(this, new LcmsNetDataClasses.Devices.classDeviceStatusEventArgs(LcmsNetDataClasses.Devices.enumDeviceStatus.Initialized, message, this));
                    }
                    status.Add(new ModelStatus("Multiple Source Path", "More than one source found on path", Category, null, LcmsNetSDK.TimeKeeper.Instance.Now.ToString(), null, source.ParentDevice.IDevice));
                }
            }
            watch.Stop();
            //LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "MultiSource check time elapsed: " + watch.Elapsed.TotalMilliseconds.ToString() + "ms");
            return status;
        }

        /// <summary>
        /// find other sources from a starting port
        /// </summary>
        /// <param name="startingSource"></param>
        /// <param name="visitedPorts"></param>
        /// <returns></returns>
        // this algorithm is a basic Breadth-First-Search
        private bool FindOtherSources(Port source, out List<Connection> pathTaken)
        {
            var unexplored = new Queue<Port>();
            var visitedPorts = new List<Port>();
            pathTaken = new List<Connection>();
            unexplored.Enqueue(source);
            var otherSourceFound = false;
            while(unexplored.Count > 0)
            {
                var unexploredPort = unexplored.Dequeue();
                visitedPorts.Add(unexploredPort);
                foreach(var connection in unexploredPort.Connections)
                {
                    var otherEnd = connection.FindOppositeEndOfConnection(unexploredPort);
                    if (!visitedPorts.Contains(otherEnd)) // keeps us from getting stuck in a cycle in the graph, checking ports we've already checked.
                    {
                        //If the other end of the connection has a source, at least two sources can see each other, and thus we return true.
                        if (otherEnd.Source)
                        {
                            otherSourceFound = true;
                        }
                        else
                        {
                            pathTaken.Add(connection);
                            unexplored.Enqueue(otherEnd);
                            //otherSourceFound = FindOtherSources(otherEnd, visitedPorts, pathTaken);
                        }
                    }
                    if (otherSourceFound) { return otherSourceFound; } //short circuit out, we've found another source on the same path.
                }
            }
            return otherSourceFound;
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
