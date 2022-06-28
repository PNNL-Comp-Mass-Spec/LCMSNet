namespace LcmsNet.Method
{
    public class LCMethodEventParameter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Parameter value to add.</param>
        /// <param name="viewModel">Edit ViewModel to use.</param>
        /// <param name="name"></param>
        /// <param name="dataProviderName">Name of the data provider to use.</param>
        public LCMethodEventParameter(string name, object value, ILCEventParameter viewModel, string dataProviderName)
        {
            Name = name;
            Value = value;
            ViewModel = viewModel;
            DataProviderName = dataProviderName;
        }

        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parameter value to use with the event method
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// ViewModel for editing the value
        /// </summary>
        public ILCEventParameter ViewModel { get; }

        /// <summary>
        /// Data provider name associated with the value options
        /// </summary>
        public string DataProviderName { get; }

        public override string ToString()
        {
            if (Value == null)
            {
                return "- Not Set -";
            }

            return Value.ToString();
        }
    }
}
