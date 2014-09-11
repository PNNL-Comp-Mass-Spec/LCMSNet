using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using LcmsNetDataClasses.Devices;
using LcmsNet.Method;

namespace LcmsNetDataClasses.Method
{

    /// <summary>
    /// Class that holds data for relative starting of LC-methods/events 
    /// </summary>
	[Serializable]
    public class classLCEventRelative
    {
        public classLCEventRelative(classLCMethod method, TimeSpan span)
        {
            RelativeMethod    = method;
            RelativeStartTime = span;
        }

        /// <summary>
        /// Gets or sets the LC method to start after an indeterminant event.
        /// </summary>
        public classLCMethod RelativeMethod
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the time to add to the relative method 
        /// to properly start the above method relative to ourself
        /// </summary>
        public TimeSpan RelativeStartTime
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// An atomic object operation used by stages in a method.
    /// </summary>
    /// 
    [Serializable]
    public class classLCEvent: ICloneable
    {
        #region Members
        /// <summary>
        /// Duration of the event.
        /// </summary>
        private TimeSpan mtimespan_duration;
        #endregion

        /// <summary>
        /// Constructor.  
        /// </summary>
        public classLCEvent()
        {
            RelativeMethod      = null;
            HasDiscreteStates   = false;
            IsIndeterminant     = false;
            //ensures that MethodData isn't null for the simulator.
            MethodData = new classLCMethodData(null, null, null, null);
        }


        #region Properties      
        /// <summary>
        /// Gets or sets the method to be started when an event is indeterminant.
        /// </summary>
        public classLCEventRelative RelativeMethod
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the event is determinant or not.
        /// </summary>
        public bool IsIndeterminant
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the method attributed reflected from the method.
        /// </summary>
        public classLCMethodAttribute MethodAttribute
        { 
            get; 
            set; 
        }

        /// <summary>
        /// gets or sets the method data associated with this event
        /// </summary>
        public classLCMethodData MethodData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the duration for this action.  
        /// 
        /// Throws: 
        ///     classInvalidDurationException (value lt 0)
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return mtimespan_duration;
            }
            set
            {
                if (value.TotalSeconds < 0)
                    throw new classInvalidTimeSpanException("The total duration for the LC-Event was not valid.  Length should be greater than 0.");

                mtimespan_duration = value;
            }
        }

        /// <summary>
        /// Notify UI that we're stopping simulation on this event as it is a breakpoint
        /// </summary>
        public void BreakHere()
        {
            MethodData.Break();
        }

        /// <summary>
        /// notify UI that we're moving passed this breakpoint.
        /// </summary>
        public void PassBreakPoint()
        {
            MethodData.PassBreakPoint();
        }

        /// <summary>
        /// Gets or sets the associated device to this method.
        /// </summary>
        public IDevice Device { get; set; }
        /// <summary>
        /// Gets the end time for the event.
        /// </summary>
        public DateTime End
        {
            get
            {
                return Start.Add(Duration.Add(HoldTime));
            }
        }
        /// <summary>
        /// Gets or sets if this event has discrete states that can be compared.
        /// </summary>
        public bool HasDiscreteStates
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the time to hold past the duration of this event in case 
        /// the device has a method that needs to be held in a certain state for 
        /// longer than it takes to execute the method.
        /// </summary>
        public TimeSpan HoldTime { get; set; }
        /// <summary>
        /// Gets or sets the method used to perform the deterministic action required.
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// Gets or set the name of the event.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets whether to use this event for optimizing a method against.
        /// </summary>
        public bool OptimizeWith { get; set; }
        /// <summary>
        /// Gets or sets the parameters to use when calling the method.
        /// </summary>
        public object[] Parameters {get; set;}
        /// <summary>
        /// Gets or sets the array of parameter names.
        /// </summary>
        public string[] ParameterNames { get; set; }
        /// <summary>
        /// Gets or sets the start time of this action.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Gets or sets the flag indicating an error occurred during event execution.
        /// </summary>
        public bool HadError { get; set; }

        public bool BreakPoint {
            get
            {
                return MethodData.BreakPoint;
            }
            private set { }
        }

        #endregion

        /// <summary>
        /// Soft copies the current object returning a new classLCEvent.
        /// </summary>
        /// <returns>classLCEvent with same parameters as used with this method.</returns>
        public object Clone()
        {
            classLCEvent newEvent       = new classLCEvent();
            newEvent.Device             = Device;
            newEvent.Duration           = Duration;
            newEvent.HasDiscreteStates  = HasDiscreteStates;
            newEvent.HoldTime           = HoldTime;
            newEvent.Method             = Method;
            newEvent.Name               = Name;
            newEvent.OptimizeWith       = OptimizeWith;
            newEvent.Parameters         = new object[Parameters.Length];
            newEvent.ParameterNames     = new string[ParameterNames.Length];
            newEvent.MethodAttribute    = MethodAttribute;
            ParameterNames.CopyTo(newEvent.ParameterNames, 0);
            Parameters.CopyTo(newEvent.Parameters, 0);
            newEvent.IsIndeterminant    = IsIndeterminant;
            newEvent.BreakPoint         = BreakPoint;
            newEvent.Start              = Start;
            newEvent.MethodData         = MethodData;           

            return newEvent;
        }
    }
}
