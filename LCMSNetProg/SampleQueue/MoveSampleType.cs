namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Enumeration for moving a sample data object in the sample queue.
    /// </summary>
    public enum MoveSampleType
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
