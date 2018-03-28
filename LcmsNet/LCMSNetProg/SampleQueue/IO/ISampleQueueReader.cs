using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Abstract base class for reading sample data from a file path.
    /// </summary>
    public interface ISampleQueueReader
    {
        /// <summary>
        /// Reads a file containing sample data.
        /// </summary>
        /// <param name="path">Path to read data from.</param>
        /// <returns>List of sample data objects.  Empty list if no data was found.</returns>
        List<SampleData> ReadSamples(string path);
    }
}