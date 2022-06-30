using System;
using System.Reflection;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that holds data for relative starting of LC-methods/events
    /// </summary>
    [Serializable]
    public class LCEventRelative
    {
        public LCEventRelative(LCMethod method, TimeSpan span)
        {
            RelativeMethod = method;
            RelativeStartTime = span;
        }

        /// <summary>
        /// Gets or sets the LC method to start after an indeterminant event.
        /// </summary>
        public LCMethod RelativeMethod { get; private set; }

        /// <summary>
        /// Gets or sets the time to add to the relative method
        /// to properly start the above method relative to this event
        /// </summary>
        public TimeSpan RelativeStartTime { get; private set; }
    }

    /// <summary>
    /// An atomic object operation used by stages in a method.
    /// </summary>
    [Serializable]
    public class LCEvent : ILCEvent, ICloneable, IEquatable<LCEvent>
    {
        /// <summary>
        /// Duration of the event.
        /// </summary>
        private TimeSpan mtimespan_duration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LCEvent()
        {
            RelativeMethod = null;
            HasDiscreteStates = false;
            IsIndeterminant = false;
            //ensures that MethodData isn't null for the simulator.
            MethodData = new LCMethodEventEmpty();
        }

        /// <summary>
        /// Soft copies the current object returning a new classLCEvent.
        /// </summary>
        /// <returns>LCEvent with same parameters as used with this method.</returns>
        public object Clone()
        {
            var newEvent = new LCEvent
            {
                Device = Device,
                Duration = Duration,
                HasDiscreteStates = HasDiscreteStates,
                Method = Method,
                Name = Name,
                OptimizeWith = OptimizeWith,
                Parameters = new object[Parameters.Length],
                ParameterNames = new string[ParameterNames.Length],
                MethodAttribute = MethodAttribute,
                IsIndeterminant = IsIndeterminant,
                Start = Start,
                MethodData = MethodData
            };

            ParameterNames.CopyTo(newEvent.ParameterNames, 0);
            Parameters.CopyTo(newEvent.Parameters, 0);

            return newEvent;
        }

        /// <summary>
        /// Method to be started when an event is indeterminant.
        /// </summary>
        public LCEventRelative RelativeMethod { get; set; }

        /// <summary>
        /// Whether the event is determinant or not.
        /// </summary>
        public bool IsIndeterminant { get; set; }

        /// <summary>
        /// Method attribute reflected from the method.
        /// </summary>
        public LCMethodEventAttribute MethodAttribute { get; set; }

        /// <summary>
        /// Method data associated with this event
        /// </summary>
        public ILCMethodEvent MethodData { get; set; }

        /// <summary>
        /// Duration for this action.
        /// </summary>
        /// <exception cref="InvalidTimeSpanException">when set value &lt; 0</exception>
        public TimeSpan Duration
        {
            get => mtimespan_duration;
            set
            {
                if (value.TotalSeconds < 0)
                    throw new InvalidTimeSpanException(
                        "The total duration for the LC-Event was not valid.  Length should be greater than 0.");

                mtimespan_duration = value;
            }
        }

        /// <summary>
        /// Device associated with this method.
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// End time for the event.
        /// </summary>
        public DateTime End => Start.Add(Duration);

        /// <summary>
        /// Whether this event has discrete states that can be compared.
        /// </summary>
        public bool HasDiscreteStates { get; set; }

        /// <summary>
        /// Class method used to perform the deterministic action required.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether to use this event for optimizing a method against.
        /// </summary>
        public bool OptimizeWith { get; set; }

        /// <summary>
        /// Parameters to use when calling the method.
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Array of parameter names.
        /// </summary>
        public string[] ParameterNames { get; set; }

        /// <summary>
        /// Start time of this action.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Event comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Flag indicating an error occurred during event execution.
        /// </summary>
        public bool HadError { get; set; }

        public bool Equals(LCEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(RelativeMethod, other.RelativeMethod) && IsIndeterminant == other.IsIndeterminant &&
                   Equals(MethodAttribute, other.MethodAttribute) && Equals(MethodData, other.MethodData) && Equals(Device, other.Device) &&
                   HasDiscreteStates == other.HasDiscreteStates && Equals(Method, other.Method) &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && OptimizeWith == other.OptimizeWith &&
                   Equals(Parameters, other.Parameters) && Equals(ParameterNames, other.ParameterNames) && Start.Equals(other.Start) &&
                   string.Equals(Comment, other.Comment, StringComparison.OrdinalIgnoreCase) && HadError == other.HadError;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LCEvent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RelativeMethod != null ? RelativeMethod.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsIndeterminant.GetHashCode();
                hashCode = (hashCode * 397) ^ (MethodAttribute != null ? MethodAttribute.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MethodData != null ? MethodData.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Device != null ? Device.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasDiscreteStates.GetHashCode();
                hashCode = (hashCode * 397) ^ (Method != null ? Method.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ OptimizeWith.GetHashCode();
                hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ParameterNames != null ? ParameterNames.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                hashCode = (hashCode * 397) ^ (Comment != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Comment) : 0);
                hashCode = (hashCode * 397) ^ HadError.GetHashCode();
                return hashCode;
            }
        }
    }
}
