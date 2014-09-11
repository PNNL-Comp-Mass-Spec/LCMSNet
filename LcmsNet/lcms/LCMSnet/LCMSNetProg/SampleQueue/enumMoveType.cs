using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Enumeration for moving a sample data object in the sample queue.  
    /// </summary>
    public enum enumMoveSampleType
    {
        /// <summary>
        /// Moves the samples up and down in sequence.
        /// </summary>
        Sequence,
        /// <summary>
        /// Swaps the samples on the column by the offset specified.
        /// </summary>
        Column
    }
}
