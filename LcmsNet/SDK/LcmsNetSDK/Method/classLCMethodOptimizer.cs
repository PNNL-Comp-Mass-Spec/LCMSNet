using System;
using System.Collections.Generic;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method
{

    public class classLCMethodOptimizer
    {
        public delegate void DelegateUpdateUserInterface(classLCMethodOptimizer sender);
        public event DelegateUpdateUserInterface UpdateRequired;

        #region Constants
        /// <summary>
        /// Required number of seconds to space each LC-event by.
        /// </summary>
        private const int CONST_REQUIRED_LC_EVENT_SPACING_SECONDS = 5;
        /// <summary>
        /// Time required to space the lc methods in milliseconds.
        /// </summary>
        private const int CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS = 10000;
        /// <summary>
        /// Offset of non-overlapping events
        /// </summary>
        private const double CONST_NON_OVERLAPPING_EVENTS_OFFSET = -1.0;
        #endregion

        /// <summary>
        /// Number of columns aligning on.
        /// </summary>
       // private int mint_numberOfColumns;

        /// <summary>
        /// Constructor.
        /// </summary>
        public classLCMethodOptimizer()
        {            
            Methods              = null;            
        }

        #region Methods
        /// <summary>
        /// Builds the hashmap of events for distinct devices.
        /// </summary>
        /// <param name="method">Method to build event table for.</param>
        /// <returns>Hash-map (dictionary) of events for each device.</returns>
        private Dictionary<IDevice, List<classLCEvent>> BuildDeviceHash(classLCMethod method)
        {
            Dictionary<IDevice, List<classLCEvent>> hash = new Dictionary<IDevice, List<classLCEvent>>();

            /// 
            /// Build the event table for each device.            
            /// 
            foreach (classLCEvent lcEvent in method.Events)
            {
                IDevice device = lcEvent.Device;
                if (device != null && device.GetType() != typeof(classTimerDevice))
                {
                    /// 
                    /// If the key exists, then we add to the list of events
                    /// Otherwise, we create a new list of events
                    /// 
                    if (hash.ContainsKey(device) == true)
                    {
                        hash[device].Add(lcEvent);
                    }
                    else
                    {
                        hash.Add(device, new List<classLCEvent>());
                        hash[device].Add(lcEvent);
                    }
                }
            }

            return hash;
        }
        /// <summary>
        /// Finds the events relevant to start that span the given duration.
        /// </summary>
        /// <param name="starttime">Time to start looking for.</param>
        /// <param name="duration">Time span to search for.</param>
        /// <returns>List of events that occur between the given time span.</returns>
        public List<classLCEvent> FindOverlappingEvents(List<classLCEvent> events, DateTime startTime, TimeSpan duration)
        {
            List<classLCEvent> overLappingEvents = new List<classLCEvent>();
            DateTime endTime = startTime.Add(duration);

            foreach (classLCEvent controlEvent in events)
            {
                TimeSpan differenceStart = controlEvent.Start.Subtract(endTime);
                TimeSpan differenceEnd   = controlEvent.End.Subtract(startTime);

                /// 
                /// We only have to assert that:
                ///     1. The end time is less than the start time - so continue until we find overlaps                
                ///     2. The start time is greater than the end time, since these are ordered sequentially, 
                ///        then we know we have looked outside our window.
                /// 
                if (differenceEnd.TotalMilliseconds < 0)
                {
                    continue;
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
        private double CompareOverlappingEvent(classLCEvent aligneeEvent, List<classLCEvent> events)
        {
            double requiredOffset = CONST_NON_OVERLAPPING_EVENTS_OFFSET;

            /// 
            /// We need to figure out what events are overlapping.
            /// 
            /// 1.  Starting point of comparison:
            /// We start with the last event, and work backwards.  Because if they are overlapping
            /// then the last event will be the event that we would need to ultimately align to.
            /// 
            /// 2.  State comparisons:
            /// While we work backwards, we see if the states are discrete. If so then we can
            /// compare their values to see if just because they are overlapping events, 
            /// they may be in the same state.           
            /// 
            /// If we have a conflict (overlapping continuous state or conflicting discrete state)
            /// then we just adjust by the last event we found.  We get this offset value and report it.
            /// Chances are we will return to this comparison of states (overlapping + discrete).
            /// But we need/want to handle any further alignment here, we just need to know the minimal
            /// amount of time we need to offset the first event by to make the alignment happy.            
            /// 
            /// Required Offset Note:  (EMPTY Events List)
            /// Finally, notice that if the event list is empty, the count = 0, and i = -1.
            /// Since i would be lt 0 then this loop would not be executed, and the required
            /// offset value would be -1.0
            /// 
            for (int i = events.Count - 1; i >= 0; i--)
            {
                classLCEvent lastEvent  = events[events.Count - 1];                
                /// 
                /// Compare discrete states if we have to 
                /// 
                if (lastEvent.HasDiscreteStates == true)
                {

                    if (lastEvent.Parameters.Length == aligneeEvent.Parameters.Length)
                    {
                        /// 
                        /// Check to see if the states are equal.
                        /// 
                        for (int j = 0; j < lastEvent.Parameters.Length; j++)
                        {
                            
                            bool isAssignableA = typeof(IComparable).IsAssignableFrom(aligneeEvent.Parameters[j].GetType());
                            bool isAssignableB = typeof(IComparable).IsAssignableFrom(lastEvent.Parameters[j].GetType());
                            if (isAssignableA && isAssignableB)
                            {
                                IComparable compareA = aligneeEvent.Parameters[j] as IComparable;
                                IComparable compareB = lastEvent.Parameters[j] as IComparable;
                                int value = compareA.CompareTo(compareB);
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
                    /// 
                    /// Otherwise, we have a continuous state, so exit the loop here!
                    /// 
                    return lastEvent.End.Subtract(aligneeEvent.Start).TotalSeconds;                                
                }                
            }
            return requiredOffset;
        }
        /// <summary>
        /// Determines if two methods overlap in time.
        /// </summary>
        /// <param name="baselineMethd">Baseline method that should come before alignee method.</param>
        /// <param name="aligneeMethod">Alignee method that should come after the baseline method.</param>
        /// <returns>True if they overlap</returns>
        public bool IsMethodOverlapping(classLCMethod baselineMethod, classLCMethod aligneeMethod)
        {
            bool isOverlapped   = true;

            /// 
            /// Test to see if the alignee method start time comes before the baseline end time.
            /// 
            /// If so, then we potentially have an overlap.  Otherwise, we need to make sure the 
            /// alignee method end time doesnt come before the baseline method time.
            /// 
            TimeSpan span       = aligneeMethod.Start.Subtract(baselineMethod.End);
            isOverlapped        = (span.TotalMilliseconds <= 0.0);

            /// 
            /// Test here to make sure that we do indeed have an overlap, the 
            /// end time of the alignee method should come after the start time
            /// of the baseline method, If less than zero, than we know that 
            /// the alignee method ends before the baseline has begun.  And it must be 
            /// > 0 for an overlap to not occur.
            /// 
            if (isOverlapped == true)
            {
                span            = aligneeMethod.End.Subtract(baselineMethod.Start);
                isOverlapped    = (span.TotalMilliseconds >= 0); 
            }
            return isOverlapped;
        }
        /// <summary>
        /// Takes a set of LC method data and constructs a series of LC events spacing them out appropiately with timing.
        /// </summary>
        /// <param name="methods">Method selections to convert into events.</param>
        /// <returns>List of LC-events to perform as part of an overall method.</returns>
        public static List<classLCEvent> ConstructEvents(List<classLCMethodData> methods)
        {            
            //DateTime startTime = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            DateTime startTime = LcmsNetSDK.TimeKeeper.Instance.Now;
            TimeSpan span = new TimeSpan(0, 0, 0, 0, 0);

            List<classLCEvent> events = new List<classLCEvent>();

            /// 
            /// Every event control should have a selected method value.
            /// 
            foreach (classLCMethodData data in methods)
            {                
                classLCEvent lcEvent = null;
                 
                /// 
                /// Make sure that we have some method data here!
                /// Otherwise the event was never assigned!! BAD! 
                /// 
                if (data == null)
                {
                    //TODO:  Should we throw an exception?  
                }
                else
                {

                    lcEvent                 = new classLCEvent();
                    lcEvent.Device          = data.Device;
                    lcEvent.Method          = data.Method;
                    lcEvent.MethodAttribute = data.MethodAttribute;
                    lcEvent.OptimizeWith    = data.OptimizeWith;

                    object[] parameters = new object[data.Parameters.Values.Count];
                    data.Parameters.Values.CopyTo(parameters, 0);
                    
                    string[] parameterNames = new string[data.Parameters.Names.Count];
                    data.Parameters.Names.CopyTo(parameterNames, 0);

                    lcEvent.Parameters      = parameters;
                    lcEvent.ParameterNames  = parameterNames; 
                    lcEvent.Start           = startTime.Add(span);
                    lcEvent.Name            = data.MethodAttribute.Name;
                    lcEvent.MethodData      = data;

                    /// 
                    /// This tells any optimizer or scheduler that the event has discreet parameters.
                    /// 
                    /// This is useful if we are trying to optimize two methods that use the same valve, but
                    /// dont require it to change the state.                    
                    /// 
                    lcEvent.HasDiscreteStates = data.MethodAttribute.HasDiscreteParameters;

                    /// 
                    /// Check to see if the device is a timer so we can space out the events 
                    /// 
                    classTimerDevice timer = data.Device as classTimerDevice;
                    if (timer != null)
                    {
                        /// 
                        /// Construct a new instance of the timer since timers are only for waiting
                        /// and not for device control.
                        /// 
                        classTimerDevice newTimer   = new classTimerDevice();
                        newTimer.Name               = timer.Name;
                        lcEvent.Device              = newTimer;
                        lcEvent.Duration = new TimeSpan(0, 0, 0, Convert.ToInt32(Convert.ToDouble(data.Parameters.Values[0])));
                    }
                    else
                    {                       
                        int time = Convert.ToInt32(data.MethodAttribute.OperationTime);

                        /// 
                        /// Check to see if the operation time is specified by the first parameter of the 
                        /// method.
                        /// 
                        if (data.MethodAttribute.TimeoutType == enumMethodOperationTime.Parameter)
                        {
                            time = Convert.ToInt32(data.Parameters.Values[0]); 
                        }                            
                        else if (time <= 0)
                        {
                            /// 
                            /// Otherwise force the time here to be the standard event spacing
                            /// 
                            time = CONST_REQUIRED_LC_EVENT_SPACING_SECONDS;                            
                        }
                        lcEvent.Duration = new TimeSpan(0,0,0,time);
                    }

                    /// 
                    /// Add 
                    /// 
                    //lcEvent.HoldTime = lcEvent.Duration;
                    span.Add(lcEvent.Duration);
                    span = span.Add(new TimeSpan(0,0,0,CONST_REQUIRED_LC_EVENT_SPACING_SECONDS));

                    events.Add(lcEvent);
                }
            }
            return events;
        }
        #endregion

        #region Method Alignment
        /// <summary>
        /// Aligns a list of samples.
        /// </summary>
        /// <param name="samples">Samples to align.</param>
        /// <returns>True if aligned, false if not.</returns>
        public bool AlignSamples(List<classSampleData> samples)
        {
            List<classLCMethod> methods = new List<classLCMethod>();
            foreach (classSampleData sample in samples)
            {
                if (sample.LCMethod != null)                
                    methods.Add(sample.LCMethod);
            }

            /// 
            /// If there is nothing to align, then just return.
            /// 
            if (methods.Count < 1)
                return true;

            return AlignMethods(methods);
        }
		/// <summary>
		/// Aligns a sample to the baseline samples.
		/// </summary>
		/// <param name="samples">Samples to align data to.</param>
		/// <returns>True if aligned, false if could not be.</returns>
		public bool AlignSamples(List<classSampleData> baselineSamples, classSampleData aligneeSample)
		{
			List<classLCMethod> methods = new List<classLCMethod>();
			foreach (classSampleData sample in baselineSamples)
			{
				if (sample.LCMethod != null)
					methods.Add(sample.LCMethod);
			}

			/// 
			/// If there is nothing to align, then just return.
			/// 
			if (methods.Count < 1)
				return true;

			return AlignMethods(methods, aligneeSample.LCMethod);
		}
        /// <summary>
        /// Aligns and adjusts the alignee method to the baseline method.
        /// </summary>
        /// <param name="baselineMethod">Method to align the rest of the methods with.</param>
        /// <param name="aligneeMethod">Method to align to the baseline.</param>
        /// <param name="startAligneeAtBaselineStart">Flag indicating if we should set the start time of the alignee to the 
        /// start time of the baseline.</param>
        /// <returns>True if the alignment was successful.  False if alignment failed.</returns>
        public bool AlignMethods(classLCMethod baselineMethod, classLCMethod aligneeMethod, bool startAligneeAtBaselineStart)
        {
            bool    aligned             = false;
            double  secondsToAdd        = 0;
            bool    requiredAlignment   = false; // Flag indicating that an adjustment to the alignee method was made.

            /// 
            /// Build a hash -- based on devices of all the LC - events to happen in time.
            ///     We do this because we are looking only for overlaps on the devices
            ///     This reduces search times for overlapping events considerably to O(m) 
            ///         where m = # of events for a given device
            ///     instead of O(n*m) where n is the number of events defined in the method, and n > m.
            /// 
            Dictionary<IDevice, List<classLCEvent>> baselineDeviceHash = BuildDeviceHash(baselineMethod);

            /// 
            /// We dont force the start time of the alignee method to come after 
            /// the baseline start, however, here we allow the user to pass a flag indicating to do
            /// such a thing, so that the baseline will be the start of the alignee method.
            /// We'll base adjustments after that.
            /// 
            DateTime startTime = aligneeMethod.Start;
            if (startAligneeAtBaselineStart == true)
            {
                startTime = baselineMethod.Start;
            }

            do
            {
                /// 
                /// Set the start time for the method 
                ///     - Noting that the start time may be adjusted by seconds to add
                ///     - Or that the start time may be running for the first time.
                ///     
                aligneeMethod.SetStartTime(startTime.AddSeconds(secondsToAdd));

                /// 
                /// Set this flag to true, because we assume that the events are aligned.
                /// If someone finds a conflicting event, then this flag will be set to false
                /// and we will try again.
                /// 
                aligned = true;

                /// 
                /// Look at each event in the alignee method, and compare it to the baseline method
                ///     1.  Compare them
                ///                 
                foreach (classLCEvent aligneeEvent in aligneeMethod.Events)
                {
                    /// 
                    /// Find any overlapping events for the alignee device. 
                    ///                     
                    if (aligneeEvent.Device != null)
                    {
                        /// 
                        /// If the baseline does not have the key, then dont worry about aligning this method
                        /// 
                        if (baselineDeviceHash.ContainsKey(aligneeEvent.Device) == true)
                        {
                            List<classLCEvent> deviceEvents = baselineDeviceHash[aligneeEvent.Device];
                            /// 
                            /// If there are no events for this device, then we dont need to worry about alignment.
                            /// 
                            if (deviceEvents.Count > 0)
                            {
                                List<classLCEvent> overlappingEvents = FindOverlappingEvents(deviceEvents, aligneeEvent.Start, aligneeEvent.Duration.Add(aligneeEvent.HoldTime));

                                /// 
                                /// Now that we have the overlapping events, we can compare their states (if discreet) to the states of the alignee event.
                                /// 
                                double requiredOffset = CompareOverlappingEvent(aligneeEvent, overlappingEvents);

                                /// 
                                /// If we have a required offset then we had conflicting overlap the scheduler found.
                                /// So we have to add to the baseline start, and shift the events over by this aggregate amount.
                                /// 
                                /// This also means that we have to start the beginning and recalculate the number of conflicts.
                                /// 
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
                if (UpdateRequired != null)
                {                    
                    UpdateRequired(this);
                }
            }
            while (aligned == false);
            // Adjust for DST transition, if necessary
            if (LcmsNetSDK.TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(aligneeMethod.Start, aligneeMethod.End))
            {
                AdjustForDaylightSavingsTransition(aligneeMethod);
                aligned = true;
            }
            else if (LcmsNetSDK.TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(baselineMethod.End, aligneeMethod.Start))
            {
                AdjustForDaylightSavingsTransition(aligneeMethod);
                aligned = true;
            }
            /// 
            /// Return whether the method was re-aligned or not 
            /// 
            return requiredAlignment;
        }
        /// <summary>
        /// Finds the last event that is non-deterministic
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public classLCEvent FindNonDeterministicEvent(classLCMethod method)
        {
            classLCEvent iEvent = null;

            foreach (classLCEvent ev in method.Events)
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
		public bool AlignMethods(List<classLCMethod> baselineMethods, classLCMethod aligneeMethod)
		{
            // No need to align self to self.
            if (baselineMethods.Count == 0)
                return true;

			int j = 0;
            /// 
            /// Here we want to find out what the earliest possible time is that we could possibly
            /// have device overlap is.  So we search, starting with the earliest possible time,
            /// i.e. the method before us = k. And work our way backwards to see what previous methods
            /// that method K overlaps with.  We cannot start before k, and because k is already
            /// aligned and optimized, we know that k-j is the earliest method that could possibly
            /// provide conflict for us.
            /// 
            /// We also want to look for things that are on the same column as us.  The end of that method
            /// j' will be the earliest possible time that we can start.
            /// 
            int k                       = baselineMethods.Count - 1;
            int lastColumnContention    =  -1;  // This marks the last column contention we saw
                                                // where method i's column = method j''s column.
                                                // We know due to rules of alignment that 
                                                // any other contentions (method h') on column
                                                // X always asserts this rule (h' < j'), and whose
                                                // start and end time are always before the start of j'
            try
            {
                aligneeMethod.SetStartTime(baselineMethods[k].Start);   // Start at method k, the earliest possible being the method
            }
            catch( ArgumentOutOfRangeException myException)
            {
                throw myException;
            }
                                                                // before us.  


            // Easy alignment scenarious -- don't allow anything to overlap!
            if (!baselineMethods[k].AllowPostOverlap)
            {
                aligneeMethod.SetStartTime(baselineMethods[k].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                return true;
            }
            if (!aligneeMethod.AllowPreOverlap)
            {
                aligneeMethod.SetStartTime(baselineMethods[k].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                return true;
            }

            int lastOverlap = k;
            // So the bottom line is, we are searching for the method j that provides the start of our comparisons.
            // We also restrict the number of total columns that we can run on so we dont get BS overlap -- could do this.
            // but the structure of the way this works doesnt allow for that.  The only way for this to happen is to have 
            // more columns that is available for scheduling.
            //int batch = Math.Max(i - mint_numberOfColumns, 0);
            for (j = k - 1; j >= 0; j--)
            {
                classLCMethod methodK = baselineMethods[k];
                classLCMethod methodJ = baselineMethods[j];

                int colI = aligneeMethod.Column;
                int colJ = baselineMethods[j].Column;

                if (colI == colJ && j > lastColumnContention)
                {
                    if (DateTime.Compare(aligneeMethod.Start, baselineMethods[j].End) < 0)
                    {
                        lastColumnContention = j;
                        // We buffer the last of the sample
                        aligneeMethod.SetStartTime(baselineMethods[j].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                    }
                }
                // We cannot just break here when there is no overlap,
                // because even if method j starts after method j - n, method j-n may end
                // after method j (where j,n are integers) 
                if (!IsMethodOverlapping(methodJ, methodK))
                {
                    lastOverlap = j;
                }
            }
            j = lastOverlap;            

			while (j < baselineMethods.Count)
			{
				/// 
				/// Only try to align if methods are overlapping, otherwise who cares
				/// 
				if (IsMethodOverlapping(baselineMethods[j], aligneeMethod))
				{

					/// 
					/// Align, but dont reset the start time of the alignee method, just add to it to perform any alignment.
					/// 
					bool alignmentOccurred = AlignMethods(baselineMethods[j],
														  aligneeMethod,
														  false);
					/// 
					/// If alignment occured, then we need to re-check all the previously overlapping methods!
					///     This way we can never overlap critical sections in i-1, or i-n methods.
					/// 
					if (alignmentOccurred == true)
					{
						// This prevents the alignment portion from entering an infinite loop.                            
						j = lastOverlap;
					}
					else
						j++;
				}
				else
				{
					j++;
				}
			}		
			return true;
		}
        /// <summary>
        /// Aligns the list of methods together.
        /// </summary>
        /// <param name="methods">Methods to align.</param>
        public bool AlignMethods(List<classLCMethod> methods)
        {            
            // 
            // Align methods
            // 
            Methods = methods;

            // i = current method index, j=baseline method index.
            // Start aligning the first methods, because j-1 alignments have already been 
            // resolved.  This is to also assist the animation parts for verification.            
            for (int i = 1; i < methods.Count; i++)                
            {
                methods[i].SetStartTime(methods[i-1].End);
                methods[i].HasNonDeterministicStart = false;
            }

            if (UpdateRequired != null)
                UpdateRequired(this);

            for (int i = 1; i < methods.Count; i++)
            {
                int j = 0;
                
                
                // Here we want to find out what the earliest possible time is that we could possibly
                // have device overlap is.  So we search, starting with the earliest possible time,
                // i.e. the method before us = k. And work our way backwards to see what previous methods
                // that method K overlaps with.  We cannot start before k, and because k is already
                // aligned and optimized, we know that k-j is the earliest method that could possibly
                // provide conflict for us.
                // 
                // We also want to look for things that are on the same column as us.  The end of that method
                // j' will be the earliest possible time that we can start.
                //                 
                int lastColumnContention = -1;              // This marks the last column contention we saw
                                                            // where method i's column = method j''s column.
                                                            // We know due to rules of alignment that 
                                                            // any other contentions (method h') on column
                                                            // X always asserts this rule (h' < j'), and whose
                                                            // start and end time are always before the start of j'

                DateTime startOfPrevious = methods[i - 1].Start;
                
                if (!methods[i].AllowPreOverlap || !methods[i-1].AllowPostOverlap)
                {
                    classLCMethod methodI = methods[i];
                    classLCMethod methodJ = methods[j];
                    int colI = methods[i].Column;
                    int colJ = methods[j].Column;

                    if (colI == colJ && j > lastColumnContention)
                    {
                        startOfPrevious = methods[i - 1].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS);
                    }
                    else
                    {
                        startOfPrevious = methods[i - 1].End;
                    }                                  

                    // Just because we dont have an overlap, does not mean that this guy shouldn't be re-started by the end guy...
                    classLCEvent iEvent = FindNonDeterministicEvent(methods[i - 1]);
                    if (iEvent != null)
                    {
                        // This means that the method could end whenever... and we need to link the event
                        // to this guy for restarting
                        iEvent.RelativeMethod = new classLCEventRelative(methods[i],
                                                                         methods[i - 1].End.Subtract(iEvent.End));
                        methods[i].HasNonDeterministicStart = true;
                        methods[i].SetStartTime(methods[i - 1].End);
                    }
                    else
                    {
                        methods[i].SetStartTime(startOfPrevious);
                    }

                    continue;
                }                
                methods[i].SetStartTime(startOfPrevious);  // Start at method k, the earliest possible being the method
                                                           // before us.                
                int lastOverlap = i - 1;
                // So the bottom line is, we are searching for the method j that provides the start of our comparisons.
                // We also restrict the number of total columns that we can run on so we dont get BS overlap -- could do this.
                // but the structure of the way this works doesnt allow for that.  The only way for this to happen is to have 
                // more columns that is available for scheduling.
                //int batch = Math.Max(i - mint_numberOfColumns, 0);
                for (j = i - 1; j >= 0; j--)
                {
                    classLCMethod methodI = methods[i];
                    classLCMethod methodJ = methods[j];

                    int colI = methods[i].Column;
                    int colJ = methods[j].Column;

                    // If the two methods are running on the same column.
                    if (colI == colJ && j > lastColumnContention)
                    {
                        if (DateTime.Compare(methods[i].Start, methods[j].End) < 0)
                        {
                            lastColumnContention = j;
                            methods[i].SetStartTime(methods[j].End.AddMilliseconds(CONST_REQUIRED_LC_METHOD_SPACING_MILLISECONDS));
                        }
                    }
                    // We cannot just break here when there is no overlap,
                    // because even if method j starts after method j - n, method j-n may end
                    // after method j (where j,n are integers) 
                    if (IsMethodOverlapping(methodJ, methodI))
                    {
                        lastOverlap = j;
                    }
                }

                j = lastOverlap;                
                while (j < i)
                {
                    /// Only try to align if methods are overlapping, otherwise who cares
                    if (IsMethodOverlapping(methods[j], methods[i]))
                    {
                        /// Align, but dont reset the start time of the alignee method, just add to it to perform any alignment.
                        bool alignmentOccurred = AlignMethods(methods[j],
                                                              methods[i],
                                                              false);
                        /// If alignment occured, then we need to re-check all the previously overlapping methods!
                        ///     This way we can never overlap critical sections in i-1, or i-n methods.
                        if (alignmentOccurred == true)
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
            }
         
            /// 
            /// Now we aren't aligning methods.
            /// 
            Methods = null;

            return true;
        }

        /// <summary>
        /// Adjust optimization to deal with DST transition.
        /// Adds 1 hour to specified method
        /// </summary>
        /// <param name="methods"></param>
        public void AdjustForDaylightSavingsTransition(classLCMethod method)
        {
            LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "OPTIMIZATION occurred around a Daylight Savings Time Transition. Some methods have been moved to start one hour later in order to prevent odd behavior.");
            method.SetStartTime(method.Start.Add(new TimeSpan(1, 0, 0)));
        }
        #endregion

        /// <summary>
        /// Gets the list of methods currently being optimized.
        /// </summary>
        public List<classLCMethod> Methods
        {
            get;
            private set;
        }
    }
}
