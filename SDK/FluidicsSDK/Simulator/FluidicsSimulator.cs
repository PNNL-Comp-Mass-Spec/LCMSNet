using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace FluidicsSDK.Simulator
{
    /// <summary>
    /// Event args for LCEvent simulation event
    /// </summary>
    public class SimulatedEventArgs:EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lcEvent"></param>
        /// <param name="elapsed"></param>
        public SimulatedEventArgs(ILCEvent lcEvent, TimeSpan elapsed)
        {
            Event = lcEvent;
            SimulatedTimeElapsed = elapsed;
        }

        /// <summary>
        /// LC Event
        /// </summary>
        public ILCEvent Event { get; set; }

        /// <summary>
        /// Elapsed time
        /// </summary>
        public TimeSpan SimulatedTimeElapsed { get; set; }
    }

    /// <summary>
    /// enumeration of possible status changes the user may want to be notified about during Model Checking and/or at the end of simulation.
    /// </summary>
    public enum WantedStatusChanges
    {
        InformationOnly, WarningsOnly, ErrorsOnly,  InformationAndWarnings, InformationAndErrors, WarningsAndErrors, All
    }

    /// <summary>
    /// Class for simulating fluidics events
    /// </summary>
    public class FluidicsSimulator: IModelCheckController
    {
        /// <summary>
        /// instance of the fluidics simulator, for singleton pattern
        /// </summary>
        private static FluidicsSimulator m_instance;


        /// <summary>
        /// Object used to check for errors after simulating event.
        /// </summary>
        private static List<IFluidicsModelChecker> m_modelCheckers;

        /// <summary>
        /// Holds list of model status information that has been generated over the course of the simulation
        /// </summary>
        private List<ModelStatus> m_AllStatusChangesThisRun;

        /// <summary>
        /// Stack to hold the completed events, lets us step backwards through simulation.
        /// </summary>
        private readonly Stack<ILCEvent> m_completedEvents;

        /// <summary>
        /// Sorted Set allows us to enumerate over simulation event lists as if they were in a priority queue.
        /// simply take the .Min (closest time to current) to get the "highest priority" list of events.
        /// </summary>
        private SortedSet<SimEventList> m_simulationQueue;

        /// <summary>
        /// The list that the simulator is currently executing(or stepping through)
        /// </summary>
        private SimEventList m_runningEvents;

        private ILCEvent m_breakEvent;
        private DateTime m_FirstStartTime;
        private DateTime m_currentStartTime;
        private TimeSpan m_elapsedTime;

        /// <summary>
        /// this timer will determine when the simulator takes a step automatically.
        /// </summary>
        private Timer m_simulationTimer;
        private const int DEFAULT_TIMER_INTERVAL = 500; // 1/2 second

        // <summary>
        // private class constructor as per singleton pattern
        // </summary>
        private FluidicsSimulator()
        {
            m_completedEvents = new Stack<ILCEvent>();
            m_simulationQueue = new SortedSet<SimEventList>();
            m_runningEvents = null;
            SimulationSpeed = DEFAULT_TIMER_INTERVAL;
            m_modelCheckers = new List<IFluidicsModelChecker>();
            m_AllStatusChangesThisRun = new List<ModelStatus>();
            IsReady = false;
            InProgress = false;
            AtBreakPoint = false;
            StopOnModelStatusChange = false;
            StopOnStatusLevel = WantedStatusChanges.All;
            CategoriesRequested = WantedStatusChanges.All;
        }

        /// <summary>
        /// simulate events until either a breakpoint is reached, an error has occurred, or all events simulated.
        /// </summary>
        public void Simulate()
        {
            // activate the timer let it and the Step() method do the heavy lifting.
            if (!InProgress && IsReady)
            {
                InProgress = !InProgress;
                StartSimulationTimer();
            }
        }

        /// <summary>
        /// Stop the simulation of events in progress
        /// </summary>
        public void StopSimulation()
        {
            if (InProgress)
            {
                InProgress = !InProgress;
            }
        }

        /// <summary>
        /// clear simulator
        /// </summary>
        public void ClearSimulator()
        {
            //clear out the simulator and reset it, make sure to notify any existing events that they need to return to their original color(via IsDone and PassBreakPoint)
            if (m_completedEvents.Count != 0)
            {
                m_completedEvents.Peek().MethodData.IsDone();
            }
            m_completedEvents.Clear();
            m_simulationQueue.Clear();
            m_runningEvents = null;
            if(m_breakEvent != null)
            {
                //tell it to change back to normal color.
                m_breakEvent.MethodData.PassBreakPoint();
                // ensure it's not also set as executing.
                m_breakEvent.MethodData.IsDone();
            }
            m_breakEvent = null;
            m_elapsedTime = new TimeSpan(0, 0, 0);
            m_FirstStartTime = new DateTime();
            m_AllStatusChangesThisRun = new List<ModelStatus>();
            AtBreakPoint = false;
            IsReady = false;
            InProgress = false;
        }

        /// <summary>
        /// Take an optimized list of LCMethods and prepare to simulate it
        /// </summary>
        /// <param name="methods"></param>
        public void PrepSimulation(IEnumerable<ILCMethod> methods)
        {
            ClearSimulator();
            try
            {
                // break the list of events into several SimEventLists of events that happen "concurrently" then add
                // them into our sortedset to get the effect of a priority queue
                var startTime = DateTime.Now;

                m_simulationQueue = BuildEventList(methods, startTime);
                if (m_simulationQueue.Count == 0)
                {
                    IsReady = false;
                    return;
                }
                m_FirstStartTime = m_simulationQueue.Min.Time;
                // set simulator as ready to go.
                IsReady = true;
            }
            catch (Exception)
            {
                //TODO: handle errors.
            }
        }

        public static SortedSet<SimEventList> BuildEventList(IEnumerable<ILCMethod> methods, DateTime startTime)
        {
            var sequence = new SortedSet<SimEventList>();

            foreach (var method in methods)
            {
                foreach (var evt in method.Events)
                {
                    var timeOfNextEvent = evt.Start;

                    //grab the list that is supposed to run at that time; create a new one if none exists
                    var current = sequence.FirstOrDefault(x => x.Time == timeOfNextEvent) ?? new SimEventList(timeOfNextEvent);

                    //add event to the list
                    current.Add(evt);

                    // since our "queue" is a SortedSet, it will ignore if we try to add the same list more than once, and as long as it shows up once, we don't care.
                    sequence.Add(current);
                    timeOfNextEvent += evt.Duration;
                }
            }

            return sequence;
        }

        private void StartSimulationTimer()
        {
            m_simulationTimer = new Timer(SimulationTimer_Elapsed, this, SimulationSpeed, SimulationSpeed);
        }

        private void SimulationTimer_Elapsed(object sender)
        {
            Step(this);
        }

        private void Step(object caller)
        {
            // check if actively simulating if step was not called by the simulator itself
            // if so, stop the simulator timer, as the user has called for a single step.

            if (this != caller && InProgress)
            {
                InProgress = !InProgress;
            }
            // we take the next priority list of concurrent events and execute them here.
            // checking for any errors in the model, or if a breakpoint is in the list.
            if (!IsReady)
                return;

            //prep an event to run
            var currentEvent = PrepareEventForExecution();

            //Execute event, remove it from the running events list and add it to the completed events stack, unless it's a breakpoint
            if (IsReady)
            {
                CheckForBreakpointExecuteEventIfNot(currentEvent);
            }
            // if breakevent and currentevent are the same, don't bother to check for completion, since nothing has changed as we're at a breakpoint.
            if (m_breakEvent != currentEvent)
            {
                CheckLCMethodCompletion();
            }
        }

        private void CheckLCMethodCompletion()
        {
            if (m_simulationQueue.Count == 0 && m_runningEvents == null)
            {
                m_simulationTimer?.Dispose();
                InProgress = false;
                IsReady = false;
                Thread.Sleep(500);
                m_completedEvents.Peek().MethodData.IsDone();
                // report simulation complete and pass any and all status changes that occurred, that the user hasn't asked not to be notified about
                SimulationComplete?.Invoke(this, new ModelStatusChangeEventArgs(FilterStatusChanges(m_AllStatusChangesThisRun, CategoriesRequested)));
            }
        }

        private void CheckForBreakpointExecuteEventIfNot(ILCEvent currentEvent)
        {
            //if the prepared event is a breakpoint, temporarily halt operations, if however, the current event is a breakpoint AND we've already stopped
            // at this breakpoint, let it pass through and execute it.
            if (currentEvent.MethodData.BreakPoint && (m_breakEvent != currentEvent) && InProgress)
            {
                m_simulationTimer.Dispose();
                InProgress = false;
                currentEvent.MethodData.Break();
                m_breakEvent = currentEvent;
                AtBreakPoint = true;
            }
            else
            {
                //otherwise execute the event.
                ExecuteEvent(currentEvent);
            }
        }

        /// <summary>
        /// prepare an event to be executed
        /// </summary>
        /// <returns></returns>
        private ILCEvent PrepareEventForExecution()
        {
            ILCEvent currentEvent;

            //if we are tat a breakpoint, we want to run the event that we halted at
            if (AtBreakPoint)
            {
                m_breakEvent.MethodData.PassBreakPoint();
                currentEvent = m_breakEvent;
                AtBreakPoint = false;
                return currentEvent;
            }

            // If the current step doesn't have events, we need to pull from the simulation queue
            // and then make sure we remove it from the queue so we don't actually re-run the events again
            if (m_runningEvents == null)
            {
                m_runningEvents = m_simulationQueue.Min;
                if (m_runningEvents != null)
                {
                    m_simulationQueue.Remove(m_runningEvents);
                    m_currentStartTime = m_runningEvents.Time;
                }
                else
                {
                    IsReady = !IsReady;
                    if (InProgress)
                    {
                        InProgress = !InProgress;
                    }
                    m_completedEvents.Peek().MethodData.IsDone();
                }
            }


            if (m_runningEvents == null)
                return null;

            // Now if we have 3 events to occur at the same time, we want to pull the first (order doesn't matter since they
            // resolve at the same time t,
            // then we update the running list, so if there are more events to execute, they will be picked up next step
            // otherwise, we set the value of the running list to null, so that the next time step t will occur.
            currentEvent = m_runningEvents.OrderBy(x => x.Duration).First();
            if (m_completedEvents.Count != 0)
            {
                m_completedEvents.Peek().MethodData.IsDone();
            }
            currentEvent.MethodData.IsCurrent();
            m_runningEvents.Remove(currentEvent);

            // if we've pulled the last event from the list, set m_runningEvents to null so we grab the next list from
            // the queue on the next run through
            if (m_runningEvents.Count == 0)
            {
                m_runningEvents = null;
            }

            return currentEvent;
        }

        /// <summary>
        /// Execute the current LCEvent
        /// </summary>
        /// <param name="currentEvent">the LCEvent to execute</param>
        private void ExecuteEvent(ILCEvent currentEvent)
        {
            // if device is a builtin or virtual, just mark it as complete, don't actually try to invoke it
            // this keeps odd events from happening, such as timers from actually causing the simulator
            // to actually wait <xx> seconds before it continues to the next step.

            if (currentEvent.Device.DeviceType != DeviceType.BuiltIn && currentEvent.Device.DeviceType != DeviceType.Virtual)
            {
                currentEvent.Method.Invoke(currentEvent.Device, currentEvent.Parameters);
                //CheckModel(m_currentEvent);
            }
            m_completedEvents.Push(currentEvent);
            m_elapsedTime = m_currentStartTime.Subtract(m_FirstStartTime).Add(currentEvent.Duration);
            EventSimulated?.Invoke(this, new SimulatedEventArgs(currentEvent, m_elapsedTime));
        }

        /// <summary>
        /// Take a single step in the simulation
        /// </summary>
        public void Step()
        {
            Step(null);
        }

        /// <summary>
        /// Add a model check to the simulator, will not add the same check object more than once, and will not add objects with the same name as another.
        /// </summary>
        /// <param name="check">an ISimulationModelChecker object to add</param>
        public void AddModelCheck(IFluidicsModelChecker check)
        {
            if (!FluidicsSimulator.GetInstance.InProgress)
            {
                if (!m_modelCheckers.Contains(check) && !(m_modelCheckers.Any(x => x.Name.Equals(check.Name))))
                {
                    m_modelCheckers.Add(check);
                    ModelCheckAdded?.Invoke(this, new ModelCheckControllerEventArgs(check));
                }
            }
        }

        /// <summary>
        ///  remove a model check from the simulator
        /// </summary>
        /// <param name="check">an ISimulationModelChecker object to remove</param>
        public void RemoveModelCheck(IFluidicsModelChecker check)
        {
            if (!FluidicsSimulator.GetInstance.InProgress)
            {
                m_modelCheckers.Remove(check);
                ModelCheckRemoved?.Invoke(this, new ModelCheckControllerEventArgs(check));
            }
        }

        /// <summary>
        ///  remove a model check from the simulator by name
        /// </summary>
        /// <param name="check">a string containing the name of the model check to remove</param>
        public void RemoveModelCheck(string check)
        {
            if (!FluidicsSimulator.GetInstance.InProgress)
            {
                var c = m_modelCheckers.Find(x => x.Name.Equals(check));
                m_modelCheckers.Remove(c);
                ModelCheckRemoved?.Invoke(this, new ModelCheckControllerEventArgs(c));
            }
        }

        /// <summary>
        /// get model checks available to the simulator
        /// </summary>
        /// <returns>a list of ISimulationModelChecker objects</returns>
        public List<IFluidicsModelChecker> GetModelCheckers()
        {
            var checks = new List<IFluidicsModelChecker>(m_modelCheckers);
            return checks;
        }

        /// <summary>
        /// used to filter status reports from model checkers
        /// </summary>
        /// <param name="status">A list of ModelStatus objects</param>
        /// <param name="categories">A WantedStatusCategories</param>
        /// <returns>A filtered list of ModelStatus objects</returns>
        private List<ModelStatus> FilterStatusChanges(List<ModelStatus> status, WantedStatusChanges categories)
        {
            var filteredStatus = new List<ModelStatus>();
            switch (categories)
            {
                case WantedStatusChanges.All:
                    filteredStatus.AddRange(status);
                    break;
                case WantedStatusChanges.WarningsAndErrors:
                    filteredStatus.AddRange(status.FindAll(x => x.Category >= ModelStatusCategory.Warning));
                    break;
                case WantedStatusChanges.InformationAndWarnings:
                    filteredStatus.AddRange(status.FindAll(x => x.Category < ModelStatusCategory.Error));
                    break;
                case WantedStatusChanges.ErrorsOnly:
                    filteredStatus.AddRange(status.FindAll(x => x.Category == ModelStatusCategory.Error));
                    break;
                case WantedStatusChanges.WarningsOnly:
                    filteredStatus.AddRange(status.FindAll(x => x.Category == ModelStatusCategory.Warning));
                    break;
                case WantedStatusChanges.InformationOnly:
                    filteredStatus.AddRange(status.FindAll(x => x.Category == ModelStatusCategory.Information));
                    break;
                case WantedStatusChanges.InformationAndErrors:
                    filteredStatus.AddRange(status.FindAll(x => (x.Category == ModelStatusCategory.Error || x.Category == ModelStatusCategory.Information)));
                    break;
            }
            return filteredStatus;
        }

        /// <summary>
        /// Check the fluidics model for any status changes
        /// </summary>
        private void CheckModel(ILCEvent lcEvent)
        {
            foreach (var check in m_modelCheckers)
            {
                if (check.IsEnabled)
                {
                    var statusChanges = check.CheckModel().ToList();
                    foreach(var change in statusChanges)
                    {
                        change.Event = lcEvent.Name;
                        change.EventDevice = lcEvent.Device;
                    }
                    ModelStatusChangeEvent?.Invoke(this, new ModelStatusChangeEventArgs(FilterStatusChanges(statusChanges.ToList(), CategoriesRequested)));
                }

            }
        }

        /// <summary>
        /// gets or sets if the simulator is to stop on model status changes
        /// </summary>
        public bool StopOnModelStatusChange { get; set; }

        /// <summary>
        /// gets or sets what level of status change the simulator should stop on, if it's stopping on status changes
        /// </summary>
        public WantedStatusChanges StopOnStatusLevel { get; set; }

        /// <summary>
        /// gets or sets what level of status changes the simulator should report at the end of the simulation.
        /// </summary>
        public WantedStatusChanges CategoriesRequested { get; set; }

        /// <summary>
        /// gets or sets if simulator is at a breakpoint.
        /// </summary>
        public bool AtBreakPoint { get; private set; }

        /// <summary>
        /// gets if simulator is ready to take a step.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// gets if a simulation is in progress
        /// </summary>
        public bool InProgress { get; private set; }

        /// <summary>
        /// get or set the speed of of simulation in milliseconds
        /// </summary>
        public int SimulationSpeed { get; set; }

        /// <summary>
        /// get instance of the fluidics simulator.
        /// </summary>
        public static FluidicsSimulator GetInstance => m_instance ?? (m_instance = new FluidicsSimulator());

#pragma warning disable 67
        public event EventHandler<SimulatedEventArgs> EventExecuting
        {
            add { }
            remove { }
        }
#pragma warning restore 67

        public event EventHandler<SimulatedEventArgs> EventSimulated;
        public event EventHandler<ModelStatusChangeEventArgs> SimulationComplete;
        public event EventHandler<ModelStatusChangeEventArgs> ModelStatusChangeEvent;
        public event EventHandler<ModelCheckControllerEventArgs> ModelCheckAdded;
        public event EventHandler<ModelCheckControllerEventArgs> ModelCheckRemoved;
    }
}
