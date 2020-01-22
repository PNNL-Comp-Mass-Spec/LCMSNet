using System;
using System.ComponentModel;
using LcmsNetData.Data;

namespace LcmsNetData.Method
{
    public class LCMethodBasic : INotifyPropertyChangedExt
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LCMethodBasic()
        {
        }

        #region Members

        /// <summary>
        /// Start time of the method
        /// </summary>
        [NonSerialized] private DateTime startTime;

        /// <summary>
        /// Duration of the method.
        /// </summary>
        [NonSerialized] private TimeSpan methodDuration;

        /// <summary>
        /// End date only calculated at call of End property to get around serialization issues.
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// Actual start time of the method
        /// </summary>
        [NonSerialized] private DateTime actualStart;

        /// <summary>
        /// Actual end time of the method
        /// </summary>
        [NonSerialized] private DateTime actualEnd;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the duration for this action.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public TimeSpan Duration => methodDuration;

        /// <summary>
        /// Gets the end time of the action.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public DateTime End
        {
            get
            {
                endTime = Start.Add(methodDuration);
                return endTime;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Method.
        /// </summary>
        [PersistenceSetting(ColumnName = "ExperimentName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the start time of this action.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public DateTime Start => startTime;

        /// <summary>
        /// Gets the actual start of the sample.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public DateTime ActualStart
        {
            get => actualStart;
            set => this.RaiseAndSetIfChanged(ref actualStart, value, nameof(ActualStart));
        }

        /// <summary>
        /// Gets the actual end time of the sample.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public DateTime ActualEnd
        {
            get => actualEnd;
            set => this.RaiseAndSetIfChanged(ref actualEnd, value, nameof(ActualEnd));
        }

        /// <summary>
        /// Gets the actual duration of the experiment that was run.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public TimeSpan ActualDuration => ActualEnd.Subtract(ActualStart);

        #endregion

        #region Methods

        /// <summary>
        /// Sets the start time for the method and updates the internal event start times.
        /// </summary>
        /// <param name="start">Time to start the method.</param>
        public virtual void SetStartTime(DateTime start)
        {
            // Set the start time. Inheriting implementations also need to cascade information.
            var duration = ActualEnd.Subtract(ActualStart);
            if (duration < TimeSpan.Zero)
            {
                duration = TimeSpan.Zero;
            }
            SetStartTimeAndDuration(start, duration);
        }

        /// <summary>
        /// Set the method start time and duration
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        protected void SetStartTimeAndDuration(DateTime start, TimeSpan duration)
        {
            startTime = start;
            methodDuration = duration;
            OnPropertyChanged(nameof(Start));
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(End));
        }

        /// <summary>
        /// Returns the name of the method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Undefined method" : Name;
        }

        #endregion

        #region "INotifyPropertyChanged implementation"

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
