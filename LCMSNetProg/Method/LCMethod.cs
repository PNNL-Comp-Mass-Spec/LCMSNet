using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// A method is a collection of LC-Events that define physical actions used to pipeline the control in an experiment.
    /// </summary>
    [Serializable]
    public class LCMethod : ILCMethod, ICloneable, IEquatable<LCMethod>, INotifyPropertyChangedExt
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
            events = new List<LCEvent>();
            actualEvents = new List<LCEvent>();
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
            events = new List<LCEvent>();
            actualEvents = new List<LCEvent>();
        }

        /// <summary>
        /// Start time of the method
        /// </summary>
        [NonSerialized] private DateTime startTime;

        /// <summary>
        /// Duration of the method.
        /// </summary>
        [NonSerialized] private TimeSpan methodDuration;

        /// <summary>
        /// Actual start time of the method
        /// </summary>
        [NonSerialized] private DateTime actualStart;

        /// <summary>
        /// Actual end time of the method
        /// </summary>
        [NonSerialized] private DateTime actualEnd;

        /// <summary>
        /// List of LC-events.
        /// </summary>
        [NonSerialized] private List<LCEvent> events;

        /// <summary>
        /// List of LC-events whose values should reflect the actual start times and durations for an LC-Event.
        /// </summary>
        [NonSerialized] private List<LCEvent> actualEvents;

        /// <summary>
        /// Duration for this action.
        /// </summary>
        public TimeSpan Duration => methodDuration;

        /// <summary>
        /// End time of the action.
        /// </summary>
        public DateTime End => Start.Add(methodDuration);

        /// <summary>
        /// Method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Start time of this action.
        /// </summary>
        public DateTime Start => startTime;

        /// <summary>
        /// Actual start of the sample.
        /// </summary>
        public DateTime ActualStart
        {
            get => actualStart;
            set => this.RaiseAndSetIfChanged(ref actualStart, value, nameof(ActualStart));
        }

        /// <summary>
        /// Actual end time of the sample.
        /// </summary>
        public DateTime ActualEnd
        {
            get => actualEnd;
            set => this.RaiseAndSetIfChanged(ref actualEnd, value, nameof(ActualEnd));
        }

        /// <summary>
        /// Actual duration of the experiment that was run.
        /// </summary>
        public TimeSpan ActualDuration => ActualEnd.Subtract(ActualStart);

        /// <summary>
        /// The method for a
        /// </summary>
        public LCEventRelative RelativeMethod { get; set; }

        /// <summary>
        /// Whether this has a deterministic start or not.
        /// </summary>
        public bool HasNonDeterministicStart { get; set; }

        /// <summary>
        /// LC-Events to be performed by this method.
        /// </summary>
        public List<LCEvent> Events
        {
            get => events;
            set => events = value;
        }

        IReadOnlyList<ILCEvent> ILCMethod.Events => Events;

        /// <summary>
        /// LC-Events data that were performed by this method.
        /// </summary>
        public List<LCEvent> ActualEvents
        {
            get => actualEvents;
            set => actualEvents = value;
        }

        /// <summary>
        /// Event number being executed.  -1 = not run, 0-N is the current event, where N is the total events defined in LCEvents.
        /// </summary>
        public int CurrentEventNumber { get; set; }

        /// <summary>
        /// Index of the column that this method is associated with.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Whether the method is special.  This means that the method is not targeted to run on a column.
        /// </summary>
        public bool IsSpecialMethod { get; set; }

        /// <summary>
        /// Whether to allow pre-method overlap.
        /// </summary>
        public bool AllowPreOverlap { get; set; }

        /// <summary>
        /// Whether to allow post-method overlap.
        /// </summary>
        public bool AllowPostOverlap { get; set; }

        /// <summary>
        /// Method comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Set the method start time and duration
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        protected void SetStartTimeAndDuration(DateTime start, TimeSpan duration)
        {
            startTime = start;
            SetDuration(duration);
            OnPropertyChanged(nameof(Start));
        }

        /// <summary>
        /// Set the method duration
        /// </summary>
        /// <param name="duration"></param>
        private void SetDuration(TimeSpan duration)
        {
            methodDuration = duration;
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(End));
        }

        /// <summary>
        /// Writes the required data for this event to the path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to write data to.</param>
        public void WritePerformanceData(string directoryPath)
        {
            // For each event, we tell the device to write the required used data.
            foreach (var lcEvent in events)
            {
                // Only write this if we have performance data for this method....
                // meaning that it has something we need to know or save for later
                // to reproduce performance information to understand when the
                // cart misbehaves.
                if (lcEvent.MethodAttribute.HasPerformanceData)
                {
                    if (lcEvent.Device is IHasPerformanceData perf)
                    {
                        perf.WritePerformanceData(directoryPath, lcEvent.MethodAttribute.Name, lcEvent.Parameters);
                    }
                    else
                    {
                        ApplicationLogger.LogError(LogLevel.Error, $"LC Event {lcEvent.Name} has performance data, but device {lcEvent.Device.GetType()} does not implement {nameof(IHasPerformanceData)}");
                    }
                }
            }
        }

        /// <summary>
        /// Sets the start time for the method and updates the internal event start times.
        /// </summary>
        /// <param name="start">Time to start the method.</param>
        public void SetStartTime(DateTime start)
        {
            // Update the start time and cascade the calculation so that
            // the event times are all updated allowing us to calculate
            // the duration and end time of the entire method.
            UpdateEventTimes(start);

            // Calculate the duration of the method.
            var duration = TimeSpan.Zero;
            if (Events.Count > 0)
            {
                duration = Events[Events.Count - 1].End.Subtract(start);
            }

            SetStartTimeAndDuration(start, duration);
            //System.Diagnostics.Debug.WriteLine(string.Format("Method {0} start time: {1} end time {2}", this.Name, start, this.End));
        }

        /// <summary>
        /// Update the method event times and duration. Used when the duration of a single event is changed during the run.
        /// </summary>
        public void UpdateMethodEventTimes()
        {
            // Update the start time and cascade the calculation so that
            // the event times are all updated allowing us to calculate
            // the duration and end time of the entire method.
            UpdateEventTimes(Start);

            // Calculate the duration of the method.
            var duration = TimeSpan.Zero;
            if (Events.Count > 0)
            {
                duration = Events[Events.Count - 1].End.Subtract(Start);
            }

            SetDuration(duration);
        }

        /// <summary>
        /// Updates the event start times.
        /// </summary>
        private void UpdateEventTimes(DateTime newStartTime)
        {
            var adjustedStart = newStartTime;
            // Iterate through each event (ultimately) and
            // adjust the start times of the event based on the time span provided.
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

            if (events != null)
            {
                foreach (var lcEvent in Events)
                {
                    if (lcEvent.Clone() is LCEvent clonedEvent)
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

        public bool Equals(LCMethod other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasNonDeterministicStart == other.HasNonDeterministicStart &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(Comment, other.Comment, StringComparison.OrdinalIgnoreCase) && Column == other.Column &&
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
                hashCode = (hashCode * 397) ^ (Comment != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Comment) : 0);
                hashCode = (hashCode * 397) ^ Column;
                hashCode = (hashCode * 397) ^ IsSpecialMethod.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowPreOverlap.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowPostOverlap.GetHashCode();
                hashCode = (hashCode * 397) ^ Events.Distinct().Aggregate(0, (x, y) => x.GetHashCode() ^ y.GetHashCode());
                return hashCode;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
