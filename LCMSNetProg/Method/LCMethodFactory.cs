namespace LcmsNet.Method
{
    /// <summary>
    /// Factory class that defines methods and constants for use across method classes.
    /// </summary>
    public class LCMethodFactory
    {
        #region Constants

        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_IS_SPECIAL = "IsSpecial";

        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_ALLOW_PRE_OVERLAP = "AllowPreOverlap";

        /// <summary>
        /// X-Path string for a LC-method if it is special (not run on a specific column).
        /// </summary>
        public const string CONST_XPATH_ALLOW_POST_OVERLAP = "AllowPostOverlap";

        /// <summary>
        /// X-path string for a LC-Method Column list (integers).
        /// </summary>
        public const string CONST_XPATH_COLUMN_DATA = "Column";

        /// <summary>
        /// X-path string for a LC-Method Column list (integers).
        /// </summary>
        public const string CONST_XPATH_COLUMN_DATA_NUMBER = "Number";

        /// <summary>
        /// LC Methods folder name.
        /// </summary>
        public const string CONST_LC_METHOD_FOLDER = @"C:\LCMSNet\LCMethods";

        /// <summary>
        /// Deterministic or non-deterministic events.
        /// </summary>
        public const string CONST_IS_EVENT_INDETERMINANT = "Indeterminant";

        /// <summary>
        /// LC Methods folder name.
        /// </summary>
        public const string CONST_LC_METHOD_EXTENSION = ".xml";

        /// <summary>
        /// X-Path string for lc-events node.
        /// </summary>
        public const string CONST_XPATH_EVENTS = "LCEvent";

        /// <summary>
        /// X-Path string for lc-events node.
        /// </summary>
        public const string CONST_XPATH_ACTUAL_EVENTS = "LCEventActual";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_NAME = "name";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_DEVICE = "Device";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_DURATION = "Duration";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_HAS_DISCREET_STATES = "HasDiscreetStates";

        /// <summary>
        /// X-Path string for if a parameter is sample specific for an LC-Event XML Node.
        /// </summary>
        public const string CONST_IS_SAMPLE_SPECIFIC = "IsSampleSpecific";

        /// <summary>
        /// X-Path string for root lc-method node.
        /// </summary>
        public const string CONST_XPATH_METHOD = "LCMethod";

        /// <summary>
        /// X-Path string for root lc-method node.
        /// </summary>
        public const string CONST_XPATH_ACTUAL_METHOD = "MethodPerformanceData";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_METHOD_INFO = "MethodInfo";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_OPTIMIZE_WITH = "OptimizeWith";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_PARAMETERS = "Parameters";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_PARAMETER = "Parameter";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_START = "StartTime";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_TYPE = "type";

        /// <summary>
        /// X-Path string for lc-method name node.
        /// </summary>
        public const string CONST_XPATH_VALUE = "value";

        #endregion
    }
}