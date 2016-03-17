using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Configuration;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// A method is a collection of LC-Events that define physical actions used to pipeline the control in an experiment.
    /// </summary>
    [Serializable]
    public class classLCMethod : classDataClassBase, ICloneable
    {
        /// <summary>
        /// Sample method key name.
        /// </summary>
        private const string CONST_NAME_METHOD_KEY = "ExperimentName";

        /// <summary>
        /// Number indicating that a sample has not run.
        /// </summary>
        private const int CONST_CURRENT_EVENT_NOT_RUN = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classLCMethod()
        {
            Column = -1;
            AllowPostOverlap = true;
            AllowPreOverlap = true;
            mlist_events = new List<classLCEvent>();
            mlist_actualEvents = new List<classLCEvent>();
            Name = "";
            CurrentEventNumber = CONST_CURRENT_EVENT_NOT_RUN;
            IsSpecialMethod = false;
            HasNonDeterministicStart = false;
            RelativeMethod = null;
        }

        #region Members

        /// <summary>
        /// Start time of the method
        /// </summary>        
        [NonSerialized] private DateTime mtime_start;

        /// <summary>
        /// Duration of the method.
        /// </summary>        
        [NonSerialized] private TimeSpan mspan_duration;

        /// <summary>
        /// List of LC-events.
        /// </summary>
        [NonSerialized] private List<classLCEvent> mlist_events;

        /// <summary>
        /// List of LC-events whose values should reflect the actual start times and durations for an LC-Event.
        /// </summary>
        [NonSerialized] private List<classLCEvent> mlist_actualEvents;

        /// <summary>
        /// End date only calculated at call of End property to get around serialization issues.
        /// </summary>
        private DateTime mdate_end;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the method for a 
        /// </summary>
        public classLCEventRelative RelativeMethod { get; set; }

        /// <summary>
        /// Gets or sets whether this has a deterministic start or not.
        /// </summary>
        public bool HasNonDeterministicStart { get; set; }

        /// <summary>
        /// Gets or sets the duration for this action.
        /// </summary>
        public TimeSpan Duration
        {
            get { return mspan_duration; }
        }

        /// <summary>
        /// Gets the end time of the action. 
        /// </summary>
        public DateTime End
        {
            get
            {
                mdate_end = Start.Add(mspan_duration);
                return mdate_end;
            }
        }

        /// <summary>
        /// Gets or sets the LC-Events to be performed by this method.
        /// </summary>
        public List<classLCEvent> Events
        {
            get { return mlist_events; }
            set { mlist_events = value; }
        }

        /// <summary>
        /// Gets or sets the LC-Events data that were performed by this method.
        /// </summary>
        public List<classLCEvent> ActualEvents
        {
            get { return mlist_actualEvents; }
            set { mlist_actualEvents = value; }
        }

        /// <summary>
        /// Gets or sets the name of the Method.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the start time of this action.
        /// </summary>
        public DateTime Start
        {
            get { return mtime_start; }
        }

        /// <summary>
        /// Gets the actual start of the sample.
        /// </summary>
        public DateTime ActualStart { get; set; }

        /// <summary>
        /// Gets the actual end time of the sample.
        /// </summary>
        public DateTime ActualEnd { get; set; }

        /// <summary>
        /// Gets the actual duration of the experiment that was run.
        /// </summary>
        public TimeSpan ActualDuration
        {
            get { return ActualEnd.Subtract(ActualStart); }
        }

        /// <summary>
        /// Gets or sets the event number being executed.  -1 = not run, 0-N is the current event, where N is the total events defined in LCEvents.
        /// </summary>
        public int CurrentEventNumber { get; set; }

        /// <summary>
        /// Gets or sets the index of the column that this method is associated with.
        /// </summary>        		
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets whether the method is special.  This means that the method 
        /// is not targeted to run on a column.
        /// </summary>
        public bool IsSpecialMethod { get; set; }

        /// <summary>
        /// Gets or sets whether to allow pre-method overlap.
        /// </summary>
        public bool AllowPreOverlap { get; set; }

        /// <summary>
        /// Gets or sets whether to allow post-method overlap.
        /// </summary>
        public bool AllowPostOverlap { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the required data for this event to the path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to write data to.</param>
        public void WritePerformanceData(string directoryPath)
        {
            // 
            // For each event, we tell the device to write the required used data.
            // 
            foreach (classLCEvent lcEvent in mlist_events)
            {
                // 
                // Only write this if we have performance data for this method....
                // meaning that it has something we need to know or save for later
                // to reproduce performance information to understand when the 
                // cart misbehaves.
                // 
                if (lcEvent.MethodAttribute.HasPerformanceData == true)
                {
                    lcEvent.Device.WritePerformanceData(directoryPath, lcEvent.MethodAttribute.Name, lcEvent.Parameters);
                }
            }
        }

        /// <summary>
        /// Sets the start time for the method and updates the internal event start times.
        /// </summary>
        /// <param name="start">Time to start the method.</param>
        public void SetStartTime(DateTime start)
        {
            // 
            // Update the start time and cascade the calculation so that 
            // the event times are all updated allowing us to calculate 
            // the duration and end time of the entire method.
            // 
            mtime_start = start;
            UpdateEventTimes();

            // 
            // Calculate the duration of the method.
            // 
            if (Events.Count <= 0)
            {
                mspan_duration = new TimeSpan(0, 0, 0, 0, 0);
            }
            else
            {
                mspan_duration = Events[Events.Count - 1].End.Subtract(mtime_start);
            }
            //System.Diagnostics.Debug.WriteLine(string.Format("Method {0} start time: {1} end time {2}", this.Name, start, this.End));
        }

        /// <summary>
        /// Updates the event start times.
        /// </summary>
        private void UpdateEventTimes()
        {
            DateTime adjustedStart = mtime_start;
            // 
            // Iterate through each event (ultimately) and 
            // adjust the start times of the event based on the time span provided.
            // 
            foreach (classLCEvent controlEvent in Events)
            {
                controlEvent.Start = adjustedStart;
                adjustedStart = adjustedStart.Add(controlEvent.Duration);
            }
        }

        /// <summary>
        /// Returns the name of the method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Clones the current method and returns a new method 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            classLCMethod newMethod = new classLCMethod();
            newMethod.Name = Name;
            if (mlist_events != null)
            {
                foreach (classLCEvent lcEvent in Events)
                {
                    classLCEvent clonedEvent = lcEvent.Clone() as classLCEvent;
                    if (clonedEvent != null)
                    {
                        newMethod.Events.Add(clonedEvent);
                    }
                }
            }
            newMethod.Column = Column;
            newMethod.AllowPostOverlap = AllowPostOverlap;
            newMethod.AllowPreOverlap = AllowPreOverlap;
            newMethod.IsSpecialMethod = IsSpecialMethod;
            newMethod.HasNonDeterministicStart = HasNonDeterministicStart;

            newMethod.SetStartTime(Start);
            return newMethod;
        }

        #endregion

        #region Overrides for Dictionary Name

        /// <summary>
        /// Returns a string dictionary containing the key to the method rather than all of the
        /// events contained within.
        /// </summary>
        /// <returns>StringDictionary containing the name of the LC Method</returns>
        public override StringDictionary GetPropertyValues()
        {
            StringDictionary dictionary = new StringDictionary();
            dictionary.Add(CONST_NAME_METHOD_KEY, Name);

            return dictionary;
        }

        /// <summary>
        /// Loads the name of the method and stores it.
        /// </summary>
        /// <param name="PropValues"></param>
        public override void LoadPropertyValues(StringDictionary PropValues)
        {
            if (PropValues == null)
                return;

            Name = PropValues[CONST_NAME_METHOD_KEY];
        }

        #endregion
    }
}