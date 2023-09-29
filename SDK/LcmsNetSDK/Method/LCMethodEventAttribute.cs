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
            DataProvider = string.Empty;
            DataProviderIndex = -1;
            HasPerformanceData = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        public LCMethodEventAttribute(string methodName, double operationTime) : this()
        {
            Name = methodName;
            OperationTime = operationTime;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex"></param>
        /// <param name="hasPerformanceData"></param>
        public LCMethodEventAttribute(string methodName, double operationTime, string dataProvider,
            int dataProviderIndex, bool hasPerformanceData = false) : this(methodName, operationTime)
        {
            HasPerformanceData = hasPerformanceData;

            if (!string.IsNullOrEmpty(dataProvider))
            {
                DataProvider = dataProvider;
                DataProviderIndex = dataProviderIndex;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="timeout">Use when the timeout should be specified by the user in the first parameter</param>
        public LCMethodEventAttribute(string methodName, MethodOperationTimeoutType timeout) : this()
        {
            Name = methodName;
            TimeoutType = timeout;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name of the method to display in the event list table of the method creator.</param>
        /// <param name="timeout">Use when the timeout should be specified by the user in the first parameter</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex"></param>
        /// <param name="hasPerformanceData"></param>
        public LCMethodEventAttribute(string methodName, MethodOperationTimeoutType timeout,
            string dataProvider, int dataProviderIndex, bool hasPerformanceData) : this(methodName, timeout)
        {
            HasPerformanceData = hasPerformanceData;

            if (!string.IsNullOrEmpty(dataProvider))
            {
                DataProvider = dataProvider;
                DataProviderIndex = dataProviderIndex;
            }
        }

        /// <summary>
        /// Constructor that allows the designer to say that this method is called with parameters specified by the user.
        /// </summary>
        /// <param name="methodName">Name to display to the user.</param>
        /// <param name="isSampleSpecific">Flag indicating that the sample contains information about how to run the method.</param>
        /// <param name="sampleParameterIndex"></param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        public LCMethodEventAttribute(string methodName, bool isSampleSpecific, int sampleParameterIndex,
            double operationTime)
            : this(methodName, operationTime)
        {
            if (isSampleSpecific && sampleParameterIndex < 0)
                throw new Exception("The SampleParameterIndex cannot be negative");

            RequiresSampleInput = isSampleSpecific;
            SampleParameterIndex = sampleParameterIndex;
        }

        /// <summary>
        /// Constructor that allows the designer to say that this method is called with parameters specified by the user.
        /// </summary>
        /// <param name="methodName">Name to display to the user.</param>
        /// <param name="isSampleSpecific">Flag indicating that the sample contains information about how to run the method.</param>
        /// <param name="sampleParameterIndex"></param>
        /// <param name="operationTime">The total operation time required by method to execute in milliseconds.  Methods must be guaranteed to execute within this range.</param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex"></param>
        /// <param name="hasPerformanceData"></param>
        public LCMethodEventAttribute(string methodName, bool isSampleSpecific, int sampleParameterIndex,
            double operationTime, string dataProvider = "", int dataProviderIndex = -1, bool hasPerformanceData = false)
            : this(methodName, operationTime, dataProvider, dataProviderIndex, hasPerformanceData)
        {
            if (isSampleSpecific && sampleParameterIndex < 0)
                throw new Exception("The SampleParameterIndex cannot be negative");

            RequiresSampleInput = isSampleSpecific;
            SampleParameterIndex = sampleParameterIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Name to display to the user.</param>
        /// <param name="timeoutType"></param>
        /// <param name="isSampleSpecific">Flag indicating that the sample contains information about how to run the method.</param>
        /// <param name="sampleParameterIndex"></param>
        /// <param name="dataProvider">Name of object provided by device that will provide data to a user control dynamically created. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="dataProviderIndex"></param>
        /// <param name="hasPerformanceData"></param>
        public LCMethodEventAttribute(string methodName, MethodOperationTimeoutType timeoutType,
            bool isSampleSpecific, int sampleParameterIndex, string dataProvider = "", int dataProviderIndex = -1,
            bool hasPerformanceData = false)
            : this(methodName, timeoutType, dataProvider, dataProviderIndex, hasPerformanceData)
        {
            if (isSampleSpecific && sampleParameterIndex < 0)
                throw new Exception("The SampleParameterIndex cannot be negative");

            RequiresSampleInput = isSampleSpecific;
            SampleParameterIndex = sampleParameterIndex;
        }

        /// <summary>
        /// Gets whether the method has performance data that should be associated with it or not.
        /// </summary>
        public bool HasPerformanceData { get; }

        /// <summary>
        /// Gets the name of the data provider that provides data for a given control. If not empty string, method's class must implement <see cref="IHasDataProvider"/>
        /// </summary>
        public string DataProvider { get; }

        /// <summary>
        /// Gets the data provider index.
        /// </summary>
        public int DataProviderIndex { get; }

        /// <summary>
        /// Gets or sets whether this method has discrete states.
        /// </summary>
        public bool HasDiscreteParameters { get; set; }

        /// <summary>
        /// Gets or sets if the timeout is specified by the first parameter value.
        /// </summary>
        public MethodOperationTimeoutType TimeoutType { get; set; }

        /// <summary>
        /// Gets or sets the total operation time required by method to execute in seconds.  Methods must be guaranteed to execute within this range.
        /// </summary>
        public double OperationTime { get; set; }

        /// <summary>
        /// Flag that, if set, signifies to the timer that it can continue with the next event as soon as the current event exits
        /// </summary>
        public bool IgnoreLeftoverTime { get; set; }

        /// <summary>
        /// Gets or sets the index of the sample parameter.
        /// </summary>
        public int SampleParameterIndex { get; set; }

        /// <summary>
        /// Gets or sets whether the method requires sample data.
        /// </summary>
        public bool RequiresSampleInput { get; set; }

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
