namespace LcmsNetSDK.Devices
{
    public interface IHasPerformanceData
    {
        /// <summary>
        /// Write the performance data and other required information associated with this device after a run.
        /// </summary>
        /// <param name="directoryPath">Path to write data to.</param>
        /// <param name="methodName">Name of method to gather performance data about.</param>
        /// <param name="parameters">Parameter data to use when writing output.</param>
        void WritePerformanceData(string directoryPath, string methodName, object[] parameters);

        /// <summary>
        /// Get the performance data and other required information associated with this device after a run.
        /// </summary>
        /// <param name="methodName">Name of method to gather performance data about.</param>
        /// <param name="parameters">Parameter data to use when writing output.</param>
        string GetPerformanceData(string methodName, object[] parameters);
    }
}
