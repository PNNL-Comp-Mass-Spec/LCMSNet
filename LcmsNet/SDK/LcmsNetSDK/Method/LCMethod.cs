using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LcmsNetData.Data;
using LcmsNetData.Method;

namespace LcmsNetSDK.Method
{
    /// <summary>
    /// A method is a collection of LC-Events that define physical actions used to pipeline the control in an experiment.
    /// </summary>
    [Serializable]
    public class LCMethod : LCMethodBasic, ICloneable, IEquatable<LCMethod>
    {
        /// <summary>
        /// Number indicating that a sample has not run.
        /// </summary>
        private const int CONST_CURRENT_EVENT_NOT_RUN = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LCMethod()
        {
            Column = -1;
            AllowPostOverlap = true;
            AllowPreOverlap = true;
            m_events = new List<LCEvent>();
            m_actualEvents = new List<LCEvent>();
            Name = "";
            CurrentEventNumber = CONST_CURRENT_EVENT_NOT_RUN;
            IsSpecialMethod = false;
            HasNonDeterministicStart = false;
            RelativeMethod = null;
        }

        /// <summary>
        /// Function called when deserializing to avoid have Events or ActualEvents return null, since deserialization does not use the constructor
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void InitializedNonSerialized(StreamingContext context)
        {
            m_events = new List<LCEvent>();
            m_actualEvents = new List<LCEvent>();
        }

        #region Members

        /// <summary>
        /// List of LC-events.
        /// </summary>
        [NonSerialized] private List<LCEvent> m_events;

        /// <summary>
        /// List of LC-events whose values should reflect the actual start times and durations for an LC-Event.
        /// </summary>
        [NonSerialized] private List<LCEvent> m_actualEvents;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the method for a
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public LCEventRelative RelativeMethod { get; set; }

        /// <summary>
        /// Gets or sets whether this has a deterministic start or not.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool HasNonDeterministicStart { get; set; }

        /// <summary>
        /// Gets or sets the LC-Events to be performed by this method.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public List<LCEvent> Events
        {
            get { return m_events; }
            set { m_events = value; }
        }

        /// <summary>
        /// Gets or sets the LC-Events data that were performed by this method.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public List<LCEvent> ActualEvents
        {
            get { return m_actualEvents; }
            set { m_actualEvents = value; }
        }

        /// <summary>
        /// Gets or sets the event number being executed.  -1 = not run, 0-N is the current event, where N is the total events defined in LCEvents.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public int CurrentEventNumber { get; set; }

        /// <summary>
        /// Gets or sets the index of the column that this method is associated with.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets whether the method is special.  This means that the method
        /// is not targeted to run on a column.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool IsSpecialMethod { get; set; }

        /// <summary>
        /// Gets or sets whether to allow pre-method overlap.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool AllowPreOverlap { get; set; }

        /// <summary>
        /// Gets or sets whether to allow post-method overlap.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
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
            foreach (var lcEvent in m_events)
            {
                //
                // Only write this if we have performance data for this method....
                // meaning that it has something we need to know or save for later
                // to reproduce performance information to understand when the
                // cart misbehaves.
                //
                if (lcEvent.MethodAttribute.HasPerformanceData)
                {
                    lcEvent.Device.WritePerformanceData(directoryPath, lcEvent.MethodAttribute.Name, lcEvent.Parameters);
                }
            }
        }

        /// <summary>
        /// Sets the start time for the method and updates the internal event start times.
        /// </summary>
        /// <param name="start">Time to start the method.</param>
        public override void SetStartTime(DateTime start)
        {
            //
            // Update the start time and cascade the calculation so that
            // the event times are all updated allowing us to calculate
            // the duration and end time of the entire method.
            //
            UpdateEventTimes(start);

            //
            // Calculate the duration of the method.
            //
            var duration = TimeSpan.Zero;
            if (Events.Count > 0)
            {
                duration = Events[Events.Count - 1].End.Subtract(start);
            }

            SetStartTimeAndDuration(start, duration);
            //System.Diagnostics.Debug.WriteLine(string.Format("Method {0} start time: {1} end time {2}", this.Name, start, this.End));
        }

        /// <summary>
        /// Updates the event start times.
        /// </summary>
        private void UpdateEventTimes(DateTime newStartTime)
        {
            var adjustedStart = newStartTime;
            //
            // Iterate through each event (ultimately) and
            // adjust the start times of the event based on the time span provided.
            //
            foreach (var controlEvent in Events)
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
            return string.IsNullOrWhiteSpace(Name) ? "Undefined method" : Name;
        }

        /// <summary>
        /// Clones the current method and returns a new method
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newMethod = new LCMethod {
                Name = Name
            };

            if (m_events != null)
            {
                foreach (var lcEvent in Events)
                {
                    var clonedEvent = lcEvent.Clone() as LCEvent;
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

        #region "IEquatable Implementation"

        public bool Equals(LCMethod other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasNonDeterministicStart == other.HasNonDeterministicStart &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && Column == other.Column &&
                   IsSpecialMethod == other.IsSpecialMethod && AllowPreOverlap == other.AllowPreOverlap && AllowPostOverlap == other.AllowPostOverlap &&
                   Events.Distinct().Aggregate(0, (x,y) => x.GetHashCode() ^ y.GetHashCode()) == other.Events.Distinct().Aggregate(0, (x, y) => x.GetHashCode() ^ y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LCMethod) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HasNonDeterministicStart.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ Column;
                hashCode = (hashCode * 397) ^ IsSpecialMethod.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowPreOverlap.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowPostOverlap.GetHashCode();
                hashCode = (hashCode * 397) ^ Events.Distinct().Aggregate(0, (x, y) => x.GetHashCode() ^ y.GetHashCode());
                return hashCode;
            }
        }

        #endregion
    }
}
