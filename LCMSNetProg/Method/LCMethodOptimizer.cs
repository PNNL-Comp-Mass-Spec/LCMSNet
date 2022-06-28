using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNet.Method
{
    public class LCMethodOptimizer
    {
        public delegate void DelegateUpdateUserInterface(LCMethodOptimizer sender);

        /// <summary>
        /// Constructor.
        /// </summary>
        public LCMethodOptimizer()
        {
            Methods = null;
        }

        /// <summary>
        /// Gets the list of methods currently being optimized.
        /// </summary>
        public List<LCMethod> Methods { get; private set; }

        public event DelegateUpdateUserInterface UpdateRequired;

        /// <summary>
        /// Required number of seconds to space each LC-event by.
        /// </summary>
        private const int CONST_REQUIRED_LC_EVENT_SPACING_SECONDS = 5;

        /// <summary>
        /// Time required to space the lc methods in milliseconds.
        /// </summary>
        private const int CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS = 2000;

        /// <summary>
        /// Offset of non-overlapping events
        /// </summary>
        private const double CONST_NON_OVERLAPPING_EVENTS_OFFSET = -1.0;

        /// <summary>
        /// Builds the hashmap of events for distinct devices.
        /// </summary>
        /// <param name="method">Method to build event table for.</param>
        /// <returns>Hash-map (dictionary) of events for each device.</returns>
        private Dictionary<IDevice, List<LCEvent>> BuildDeviceHash(LCMethod method)
        {
            var hash = new Dictionary<IDevice, List<LCEvent>>();

            //
            // Build the event table for each device.
            //
            foreach (var lcEvent in method.Events)
            {
                var device = lcEvent.Device;
                if (device != null && device.GetType() != typeof (TimerDevice))
                {
                    // If the key exists, then we add to the list of events
                    // Otherwise, we create a new list of events
                    if (hash.ContainsKey(device))
                    {
                        hash[device].Add(lcEvent);
                    }
                    else
                    {
                        hash.Add(device, new List<LCEvent>());
                        hash[device].Add(lcEvent);
                    }
                }
            }

            return hash;
        }

        /// <summary>
        /// Finds the events relevant to start that span the given duration.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="startTime">Time to start looking for.</param>
        /// <param name="duration">Time span to search for.</param>
        /// <returns>List of events that occur between the given time span.</returns>
        public List<LCEvent> FindOverlappingEvents(List<LCEvent> events, DateTime startTime, TimeSpan duration)
        {
            var overLappingEvents = new List<LCEvent>();
            var endTime = startTime.Add(duration);

            foreach (var controlEvent in events)
            {
                var differenceStart = controlEvent.Start.Subtract(endTime);
                var differenceEnd = controlEvent.End.Subtract(startTime);

                // We only have to assert that:
                //     1. The end time is less than the start time - so continue until we find overlaps
                //     2. The start time is greater than the end time, since these are ordered sequentially,
                //        then we know we have looked outside our window.
                if (differenceEnd.TotalMilliseconds < 0)
                {
                }
                else if (differenceStart.TotalMilliseconds > 0)
                {
                    return overLappingEvents;
                }
                else
                {
                    overLappingEvents.Add(controlEvent);
                }
            }
            return overLappingEvents;
        }

        /// <summary>
        /// Compares the event to the list of events to see if the states are ok.  The events should be generated from the save device.
        /// </summary>
        /// <param name="aligneeEvent">Event to align with</param>
        /// <param name="events">Events whose states dictate how the alignee is shifted.</param>
        /// <returns>-1 if no offset required, >= 0 if states cannot be resolved and an offset required.</returns>
        private double CompareOverlappingEvent(LCEvent aligneeEvent, List<LCEvent> events)
        {
            var requiredOffset = CONST_NON_OVERLAPPING_EVENTS_OFFSET;

            // We need to figure out what events are overlapping.
            //
            // 1.  Starting point of comparison:
            // We start with the last event, and work backwards.  Because if they are overlapping
            // then the last event will be the event that we would need to ultimately align to.
            //
            // 2.  State comparisons:
            // While we work backwards, we see if the states are discrete. If so then we can
            // compare their values to see if just because they are overlapping events,
            // they may be in the same state.
            //
            // If we have a conflict (overlapping continuous state or conflicting discrete state)
            // then we just adjust by the last event we found.  We get this offset value and report it.
            // Chances are we will return to this comparison of states (overlapping + discrete).
            // But we need/want to handle any further alignment here, we just need to know the minimal
            // amount of time we need to offset the first event by to make the alignment happy.
            //
            // Required Offset Note:  (EMPTY Events List)
            // Finally, notice that if the event list is empty, the count = 0, and i = -1.
            // Since i would be lt 0 then this loop would not be executed, and the required
            // offset value would be -1.0
            for (var i = events.Count - 1; i >= 0; i--)
            {
                var lastEvent = events[events.Count - 1];

                // Compare discrete states if we have to
                if (lastEvent.HasDiscreteStates)
                {
                    if (lastEvent.Parameters.Length == aligneeEvent.Parameters.Length)
                    {
                        // Check to see if the states are equal.
                        for (var j = 0; j < lastEvent.Parameters.Length; j++)
                        {
                            var isAssignableA =
                                typeof (IComparable).IsAssignableFrom(aligneeEvent.Parameters[j].GetType());
                            var isAssignableB = typeof (IComparable).IsAssignableFrom(lastEvent.Parameters[j].GetType());
                            if (isAssignableA && isAssignableB)
                            {
                                var compareA = aligneeEvent.Parameters[j] as IComparable;
                                var compareB = lastEvent.Parameters[j] as IComparable;
                                var value = compareA.CompareTo(compareB);
                                if (value != 0)
                                {
                                    return lastEvent.End.Subtract(aligneeEvent.Start).TotalSeconds;
                                }
                            }
                            else if (lastEvent.Parameters[j] != aligneeEvent.Parameters[j])
                            {
                                return lastEvent.End.Subtract(aligneeEvent.Start).TotalSeconds;
                            }
                        }
                    }
                }
                else
                {
                    // Otherwise, we have a continuous state, so exit the loop here!
                    return lastEvent.End.Subtract(aligneeEvent.Start).TotalSeconds;
                }
            }
            return requiredOffset;
        }

        /// <summary>
        /// Determines if two methods overlap in time.
        /// </summary>
        /// <param name="baselineMethod">Baseline method that should come before alignee method.</param>
        /// <param name="aligneeMethod">Alignee method that should come after the baseline method.</param>
        /// <returns>True if they overlap</returns>
        public bool IsMethodOverlapping(LCMethod baselineMethod, LCMethod aligneeMethod)
        {
            var isOverlapped = true;

            // Test to see if the alignee method start time comes before the baseline end time.
            //
            // If so, then we potentially have an overlap.  Otherwise, we need to make sure the
            // alignee method end time doesnt come before the baseline method time.
            var span = aligneeMethod.Start.Subtract(baselineMethod.End);
            isOverlapped = (span.TotalMilliseconds <= 0.0);

            // Test here to make sure that we do indeed have an overlap, the
            // end time of the alignee method should come after the start time
            // of the baseline method, If less than zero, than we know that
            // the alignee method ends before the baseline has begun.  And it must be
            // > 0 for an overlap to not occur.
            if (isOverlapped)
            {
                span = aligneeMethod.End.Subtract(baselineMethod.Start);
                isOverlapped = (span.TotalMilliseconds >= 0);
            }
            //System.Diagnostics.Debug.WriteLine("overlap check result: " + isOverlapped);
            return isOverlapped;
        }

        /// <summary>
        /// Takes a set of LC method data and constructs a series of LC events spacing them out appropriately with timing.
        /// </summary>
        /// <param name="methods">Method selections to convert into events.</param>
        /// <returns>List of LC-events to perform as part of an overall method.</returns>
        public static List<LCEvent> ConstructEvents(List<LCMethodEventData> methods)
        {
            //DateTime startTime = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            var startTime = TimeKeeper.Instance.Now;
            var span = new TimeSpan(0, 0, 0, 0, 0);

            var events = new List<LCEvent>();

            // Every event control should have a selected method value.
            foreach (var data in methods)
            {
                LCEvent lcEvent = null;

                // Make sure that we have some method data here!
                // Otherwise the event was never assigned!! BAD!
                if (data == null)
                {
                    //TODO:  Should we throw an exception?
                }
                else
                {
                    lcEvent = new LCEvent();
                    lcEvent.Device = data.Device;
                    lcEvent.Method = data.Method;
                    lcEvent.MethodAttribute = data.MethodEventAttribute;
                    lcEvent.OptimizeWith = data.OptimizeWith;

                    var parameters = data.Parameters.Select(x => x.Value).ToArray();
                    var parameterNames = data.Parameters.Select(x => x.Name).ToArray();

                    lcEvent.Parameters = parameters;
                    lcEvent.ParameterNames = parameterNames;
                    lcEvent.Start = startTime.Add(span);
                    lcEvent.Name = data.MethodEventAttribute.Name;
                    lcEvent.MethodData = data;

                    // This tells any optimizer or scheduler that the event has discrete parameters.
                    //
                    // This is useful if we are trying to optimize two methods that use the same valve, but
                    // don't require it to change the state.
                    lcEvent.HasDiscreteStates = data.MethodEventAttribute.HasDiscreteParameters;

                    // Check to see if the device is a timer so we can space out the events
                    var timer = data.Device as TimerDevice;
                    if (timer != null)
                    {
                        // Construct a new instance of the timer since timers are only for waiting
                        // and not for device control.
                        var newTimer = new TimerDevice { Name = timer.Name };
                        lcEvent.Device = newTimer;
                        lcEvent.Duration = new TimeSpan(0, 0, 0,
                            Convert.ToInt32(Convert.ToDouble(data.Parameters[0].Value)));
                    }
                    else
                    {
                        var time = Convert.ToInt32(data.MethodEventAttribute.OperationTime);

                        // Check to see if the operation time is specified by the first parameter of the method.
                        if (data.MethodEventAttribute.TimeoutType == MethodOperationTimeoutType.Parameter)
                        {
                            time = Convert.ToInt32(data.Parameters[0].Value);
                        }
                        else if (data.MethodEventAttribute.TimeoutType == MethodOperationTimeoutType.CallMethod)
                        {
                            // Calculate the needed timeout based on parameters that can be used to calculate a timeout
                            if (string.IsNullOrWhiteSpace(data.MethodEventAttribute.TimeoutCalculationMethod))
                            {
                                throw new NotSupportedException("Developer Error!!! Timeout Calculation Method is not set with TimeoutType CallMethod, device is " + data.Device.GetType());
                            }

                            var timeMethod = data.Device.GetType().GetMethod(data.MethodEventAttribute.TimeoutCalculationMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                            if (timeMethod == null || timeMethod.ReturnType != typeof(int))
                            {
                                throw new NotSupportedException("Developer Error!!! Valid Timeout Calculation Method not found, device is " + data.Device.GetType() + ", method supplied is " + data.MethodEventAttribute.TimeoutCalculationMethod);
                            }

                            var methodParameters = data.Parameters.Select(x => x.Value).ToArray();
                            if (timeMethod.GetParameters().Length != methodParameters.Length)
                            {
                                throw new NotSupportedException("Developer Error!!! Valid Timeout Calculation Method not found (parameter count), device is " + data.Device.GetType() + ", method supplied is " + data.MethodEventAttribute.TimeoutCalculationMethod);
                            }

                            // For some reason the boxing returns a double, instead of an int
                            time = (int)timeMethod.Invoke(data.Device, methodParameters);
                        }
                        else if (time <= 0)
                        {
                            // Otherwise force the time here to be the standard event spacing
                            time = CONST_REQUIRED_LC_EVENT_SPACING_SECONDS;
                        }

                        lcEvent.Duration = new TimeSpan(0, 0, 0, time);
                    }

                    // Add
                    span.Add(lcEvent.Duration);
                    span = span.Add(new TimeSpan(0, 0, 0, CONST_REQUIRED_LC_EVENT_SPACING_SECONDS));

                    events.Add(lcEvent);
                }
            }
            return events;
        }

        /// <summary>
        /// Aligns a list of samples.
        /// </summary>
        /// <param name="samples">Samples to align.</param>
        /// <returns>True if aligned, false if not.</returns>
        public bool AlignSamples(List<ISampleInfo> samples)
        {
            var methods = new List<LCMethod>();
            foreach (var sample in samples)
            {
                if (sample.ActualLCMethod != null)
                    methods.Add(sample.ActualLCMethod);
            }

            // If there is nothing to align, then just return.
            if (methods.Count < 1)
                return true;

            var done = AlignMethods(methods);
            //PrintAlignedMethods(methods);
            return done;
        }

        /// <summary>
        /// Aligns a sample to the baseline samples.
        /// </summary>
        /// <param name="baselineSamples">Samples to align data to.</param>
        /// <param name="aligneeSample">Samples to align.</param>
        /// <returns>True if aligned, false if could not be.</returns>
        public bool AlignSamples(List<ISampleInfo> baselineSamples, ISampleInfo aligneeSample)
        {
            var methods = new List<LCMethod>();
            foreach (var sample in baselineSamples)
            {
                if (sample.ActualLCMethod != null)
                    methods.Add(sample.ActualLCMethod);
            }

            // If there is nothing to align, then just return.
            if (methods.Count < 1)
                return true;
            var done = AlignMethods(methods, aligneeSample.ActualLCMethod);
            //PrintAlignedMethods(methods, aligneeSample.LCMethod);
            return done;
        }

        private void PrintAlignedMethods(List<LCMethod> methods, LCMethod alignedMethod = null)
        {
            Debug.WriteLine("Optimized Methods: ");
            var formatStr = "method: {0} start time: {1} end time {2}";
            Debug.WriteLine("Current Time: {0}", TimeKeeper.Instance.Now);
            foreach (var m in methods)
            {
                Debug.WriteLine(formatStr, m.Name, m.Start, m.End);
            }
            if (alignedMethod != null)
            {
                Debug.WriteLine(formatStr, alignedMethod.Name, alignedMethod.Start, alignedMethod.End);
            }
            Debug.WriteLine("Optimized Methods Done.");
        }

        /// <summary>
        /// Aligns and adjusts the alignee method to the baseline method.
        /// </summary>
        /// <param name="baselineMethod">Method to align the rest of the methods with.</param>
        /// <param name="aligneeMethod">Method to align to the baseline.</param>
        /// <param name="startAligneeAtBaselineStart">Flag indicating if we should set the start time of the alignee to the
        /// start time of the baseline.</param>
        /// <returns>True if the alignment was successful.  False if alignment failed.</returns>
        public bool AlignMethods(LCMethod baselineMethod, LCMethod aligneeMethod,
            bool startAligneeAtBaselineStart)
        {
            var aligned = false;
            double secondsToAdd = 0;
            var requiredAlignment = false; // Flag indicating that an adjustment to the alignee method was made.

            // Build a hash -- based on devices of all the LC - events to happen in time.
            //     We do this because we are looking only for overlaps on the devices
            //     This reduces search times for overlapping events considerably to O(m)
            //         where m = # of events for a given device
            //     instead of O(n*m) where n is the number of events defined in the method, and n > m.
            var baselineDeviceHash = BuildDeviceHash(baselineMethod);

            // We dont force the start time of the alignee method to come after
            // the baseline start, however, here we allow the user to pass a flag indicating to do
            // such a thing, so that the baseline will be the start of the alignee method.
            // We'll base adjustments after that.
            var startTime = aligneeMethod.Start;
            if (startAligneeAtBaselineStart)
            {
                startTime = baselineMethod.Start;
            }

            do
            {
                // Set the start time for the method
                //     - Noting that the start time may be adjusted by seconds to add
                //     - Or that the start time may be running for the first time.
                //
                //System.Diagnostics.Debug.WriteLine(string.Format("Checking Alignment method: {0} against baseline method.", aligneeMethod.Name));
                aligneeMethod.SetStartTime(startTime.AddSeconds(secondsToAdd));

                // Set this flag to true, because we assume that the events are aligned.
                // If someone finds a conflicting event, then this flag will be set to false
                // and we will try again.
                aligned = true;

                // Look at each event in the alignee method, and compare it to the baseline method
                //     1.  Compare them
                foreach (var aligneeEvent in aligneeMethod.Events)
                {
                    // Find any overlapping events for the alignee device.
                    if (aligneeEvent.Device != null)
                    {
                        // If the baseline does not have the key, then don't worry about aligning this method
                        if (baselineDeviceHash.ContainsKey(aligneeEvent.Device))
                        {
                            var deviceEvents = baselineDeviceHash[aligneeEvent.Device];
                            // If there are no events for this device, then we don't need to worry about alignment.
                            if (deviceEvents.Count > 0)
                            {
                                var overlappingEvents = FindOverlappingEvents(deviceEvents,
                                    aligneeEvent.Start, aligneeEvent.Duration);

                                // Now that we have the overlapping events, we can compare their states (if discreet) to the states of the alignee event.
                                var requiredOffset = CompareOverlappingEvent(aligneeEvent, overlappingEvents);

                                // If we have a required offset then we had conflicting overlap the scheduler found.
                                // So we have to add to the baseline start, and shift the events over by this aggregate amount.
                                //
                                // This also means that we have to start the beginning and recalculate the number of conflicts.
                                if (requiredOffset > 0)
                                {
                                    secondsToAdd += requiredOffset;
                                    aligned = false;
                                    requiredAlignment = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                UpdateRequired?.Invoke(this);
            } while (aligned == false);

            // Adjust for DST transition, if necessary
            if (TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(aligneeMethod.Start,
                aligneeMethod.End))
            {
                AdjustForDaylightSavingsTransition(aligneeMethod);
                aligned = true;
            }
            else if (TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(baselineMethod.End,
                aligneeMethod.Start))
            {
                AdjustForDaylightSavingsTransition(aligneeMethod);
                aligned = true;
            }

            // Return whether the method was re-aligned or not
            return requiredAlignment;
        }

        /// <summary>
        /// Finds the last event that is non-deterministic
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public LCEvent FindNonDeterministicEvent(LCMethod method)
        {
            LCEvent iEvent = null;

            foreach (var ev in method.Events)
            {
                // We only want the last event
                if (ev.IsIndeterminant)
                {
                    iEvent = ev;
                }
            }

            return iEvent;
        }

        /// <summary>
        /// Aligns the alignee method to the baseline methods.  No update events are fired with this method..
        /// </summary>
        /// <param name="baselineMethods">Methods to align to.</param>
        /// <param name="aligneeMethod"></param>
        /// <returns></returns>
        public bool AlignMethods(List<LCMethod> baselineMethods, LCMethod aligneeMethod)
        {
            // No need to align self to self.
            if (baselineMethods.Count == 0)
            {
                //System.Diagnostics.Debug.WriteLine("No Baseline samples to align to");
                return true;
            }

            var j = 0;

            // Here we want to find out what the earliest possible time is that we could possibly
            // have device overlap is.  So we search, starting with the earliest possible time,
            // i.e. the method before us = k. And work our way backwards to see what previous methods
            // that method K overlaps with.  We cannot start before k, and because k is already
            // aligned and optimized, we know that k-j is the earliest method that could possibly
            // provide conflict for us.
            //
            // We also want to look for things that are on the same column as us.  The end of that method
            // j' will be the earliest possible time that we can start.
            var k = baselineMethods.Count - 1;
            var lastColumnContention = -1; // This marks the last column contention we saw
            // where method i's column = method j''s column.
            // We know due to rules of alignment that
            // any other contentions (method h') on column
            // X always asserts this rule (h' < j'), and whose
            // start and end time are always before the start of j'
            try
            {
                //System.Diagnostics.Debug.WriteLine("Setting start time to start time of previous method before moving to further optimization.");
                aligneeMethod.SetStartTime(baselineMethods[k].Start);
                    // Start at method k, the earliest possible being the method before us.
            }
            catch (ArgumentOutOfRangeException myException)
            {
                throw myException;
            }

            //System.Diagnostics.Debug.WriteLine("checking for overlap allowances...");
            // Easy alignment scenarios -- don't allow anything to overlap!
            if (!baselineMethods[k].AllowPostOverlap || !aligneeMethod.AllowPreOverlap)
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("Pre or Post Overlap not allowed, setting start time of Method: {0} to end of method at index {1} plus ten seconds", aligneeMethod.Name, k));
                aligneeMethod.SetStartTime(
                    baselineMethods[k].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                if (false)
                    PrintAlignedMethods(baselineMethods, aligneeMethod);
                return true;
            }

            var lastOverlap = k;
            // So the bottom line is, we are searching for the method j that provides the start of our comparisons.
            // We also restrict the number of total columns that we can run on so we don't get BS overlap -- could do this.
            // but the structure of the way this works doesn't allow for that.  The only way for this to happen is to have
            // more columns that is available for scheduling.
            for (j = k; j >= 0; j--)
            {
                var methodK = baselineMethods[k];
                var methodJ = baselineMethods[j];
                var colI = aligneeMethod.Column;
                var colJ = methodJ.Column;

                if (colI == colJ && j > lastColumnContention - 1)
                {
                    if (DateTime.Compare(aligneeMethod.Start, methodJ.End) < 0)
                    {
                        lastColumnContention = j;
                        // We buffer the last of the sample
                        //System.Diagnostics.Debug.WriteLine(string.Format("Setting start time of Method due to having the same column as method {0} at index: {1} ", methodJ.Name, j));
                        aligneeMethod.SetStartTime(
                            methodJ.End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                    }
                }
                // We cannot just break here when there is no overlap,
                // because even if method j starts after method j - n, method j-n may end
                // after method j (where j,n are integers)
                if (!IsMethodOverlapping(methodJ, methodK))
                {
                    //System.Diagnostics.Debug.WriteLine("No overlap detected, moving to next phase...");
                    lastOverlap = j;
                }
            }
            j = lastOverlap;

            while (j < baselineMethods.Count)
            {
                // Only try to align if methods are overlapping, otherwise who cares
                if (IsMethodOverlapping(baselineMethods[j], aligneeMethod))
                {
                    // Align, but don't reset the start time of the alignee method, just add to it to perform any alignment.
                    var alignmentOccurred = AlignMethods(baselineMethods[j],
                        aligneeMethod,
                        false);

                    // If alignment occurred, then we need to re-check all the previously overlapping methods!
                    //     This way we can never overlap critical sections in i-1, or i-n methods.
                    if (alignmentOccurred)
                    {
                        // This prevents the alignment portion from entering an infinite loop.
                        j = lastOverlap;
                    }
                    else
                    {
                        j++;
                    }
                }
                else
                {
                    j++;
                }
            }
            if (false)
                PrintAlignedMethods(baselineMethods, aligneeMethod);
            return true;
        }

        /// <summary>
        /// Aligns the list of methods together.
        /// </summary>
        /// <param name="methods">Methods to align.</param>
        public bool AlignMethods(List<LCMethod> methods)
        {
            // Align methods
            Methods = methods;
            var alignedMethods = new List<LCMethod>();
            foreach (var method in methods)
            {
                AlignMethods(alignedMethods, method);
                alignedMethods.Add(method);
            }

            // Now we aren't aligning methods.
            Methods = null;

            return true;
        }

        /// <summary>
        /// Adjust optimization to deal with DST transition.
        /// Adds 1 hour to specified method
        /// </summary>
        /// <param name="method"></param>
        public void AdjustForDaylightSavingsTransition(LCMethod method)
        {
            Debug.WriteLine(
                string.Format("Adjusting start time of method: {0} by 1 hour(forward) due to DST transition",
                    method.Name));
            ApplicationLogger.LogError(
                ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                "OPTIMIZATION occurred around a Daylight Savings Time Transition. Some methods have been moved to start one hour later in order to prevent odd behavior.");
            method.SetStartTime(method.Start.Add(new TimeSpan(1, 0, 0)));
        }
    }
}
