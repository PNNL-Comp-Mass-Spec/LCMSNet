using System;
using System.Collections.Generic;

using LcmsNetDataClasses;


namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Abstract base class for writing sample data to a file path.
    /// </summary>
    public interface ISampleQueueWriter
    {
        /// <summary>
        /// Writes sample data to the specified path.
        /// </summary>
        /// <param name="path">Path to write data to.</param>
        /// <param name="data">Data to write.</param>
        void WriteSamples(string path, List<classSampleData> data);
    }
}
