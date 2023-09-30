using System;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    /// <summary>
    /// Defines the attribute the scheduling algorithm and method handler will use to determine what methods to call
    /// while running a LC method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LCMethodEventAttribute : Attribute
    {
        private LCMethodEventAttribute()
        {
            IgnoreLeftoverTime = false;
            SampleParameterIndex = -1;
            RequiresSampleInput = false;
            OperationTime = -1;
            TimeoutType = MethodOperationTimeoutType.Attribute;
            HasDiscreteParameters = false;
            DataProvider = new DataProviderSpec();
            HasPerformanceData = false;
        }

        /// <summary>
        /// Constructor that allows the designer to say that this method is called with parameters specified by the user.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex">Index of the parameter that expects a value supplied by the data provider</param>
        public LCMethodEventAttribute(string methodName, double operationTime, string dataProvider = "", int dataProviderIndex = -1) : this()
        {
            Name = methodName;
            OperationTime = operationTime;

            if (!string.IsNullOrEmpty(dataProvider) || dataProviderIndex >= 0)
            {
                DataProvider = new DataProviderSpec(dataProvider, dataProviderIndex);
            }
        }

        /// <summary>
        /// Constructor that allows the designer to say that this method is called with parameters specified by the user, and expects sample data at runtime
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        /// <param name="sampleParameterIndex">Index of the parameter that expects the sample data object</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex">Index of the parameter that expects a value supplied by the data provider</param>
        public LCMethodEventAttribute(string methodName, double operationTime, int sampleParameterIndex, string dataProvider = "", int dataProviderIndex = -1) : this(methodName, operationTime, dataProvider, dataProviderIndex)
        {
            if (sampleParameterIndex < 0)
                throw new Exception("The SampleParameterIndex cannot be negative");

            RequiresSampleInput = true;
            SampleParameterIndex = sampleParameterIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="timeout">Use when the timeout should be specified by the user in the first parameter</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex">Index of the parameter that expects a value supplied by the data provider</param>
        public LCMethodEventAttribute(string methodName, MethodOperationTimeoutType timeout,
            string dataProvider = "", int dataProviderIndex = -1) : this()
        {
            Name = methodName;
            TimeoutType = timeout;

            if (!string.IsNullOrEmpty(dataProvider) || dataProviderIndex >= 0)
            {
                DataProvider = new DataProviderSpec(dataProvider, dataProviderIndex);
            }
        }

        /// <summary>
        /// Constructor for an LC Method Event that expects sample data at runtime
        /// </summary>
        /// <param name="methodName">Name to display to the user.</param>
        /// <param name="timeoutType">Use when the timeout should be specified by the user in the first parameter</param>
        /// <param name="sampleParameterIndex">Index of the parameter that expects the sample data object</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex">Index of the parameter that expects a value supplied by the data provider</param>
        public LCMethodEventAttribute(string methodName, MethodOperationTimeoutType timeoutType,
            int sampleParameterIndex, string dataProvider = "", int dataProviderIndex = -1)
            : this(methodName, timeoutType, dataProvider, dataProviderIndex)
        {
            if (sampleParameterIndex < 0)
                throw new Exception($"The {nameof(sampleParameterIndex)} cannot be negative");

            RequiresSampleInput = true;
            SampleParameterIndex = sampleParameterIndex;
        }

        /// <summary>
        /// Gets whether the method has performance data that should be associated with it or not. If true, method's class must implement <see cref="IHasPerformanceData"/>
        /// </summary>
        public bool HasPerformanceData { get; set; }

        /// <summary>
        /// Specifications for the data provider set for this method. If set (not blank/default), method's class must implement <see cref="IHasDataProvider"/>
        /// </summary>
        public DataProviderSpec DataProvider { get; }

        /// <summary>
        /// Gets or sets whether this method has discrete states.
        /// </summary>
        public bool HasDiscreteParameters { get; set; }

        /// <summary>
        /// Gets or sets if the timeout is specified by the first parameter value.
        /// </summary>
        public MethodOperationTimeoutType TimeoutType { get; }

        /// <summary>
        /// Gets or sets the total operation time required by method to execute in seconds.  Methods must be guaranteed to execute within this range.
        /// </summary>
        public double OperationTime { get; }

        /// <summary>
        /// Flag that, if set, signifies to the timer that it can continue with the next event as soon as the current event exits
        /// </summary>
        public bool IgnoreLeftoverTime { get; set; }

        /// <summary>
        /// Whether the method requires sample data.
        /// </summary>
        public bool RequiresSampleInput { get; }

        /// <summary>
        /// Index of the parameter that expects the sample data object.
        /// </summary>
        public int SampleParameterIndex { get; }

        /// <summary>
        /// The Description text displayed if the user hovers over the event in the GUI.
        /// </summary>
        public string EventDescription { get; set; }

        /// <summary>
        /// The name of the public method to call with the event parameters to determine the event duration. Method must return an int.
        /// </summary>
        public string TimeoutCalculationMethod { get; set; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the name of the method.
        /// </summary>
        /// <returns>The name of the method attributing.</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Undefined method" : Name;
        }
    }
}
