﻿using System.Collections.Generic;
using LcmsNet.Data;

namespace LcmsNet.IO.Sequence
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
        void WriteSamples(string path, List<SampleData> data);
    }
}
