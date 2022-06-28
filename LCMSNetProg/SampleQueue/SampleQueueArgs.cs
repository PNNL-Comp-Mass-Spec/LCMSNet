using System.Collections.Generic;
using LcmsNet.Data;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Sample queue arguments used by various delegates.
    /// </summary>
    public class SampleQueueArgs
    {
        /// <summary>
        /// Constructor that takes an array of samples that were operated upon.
        /// </summary>
        /// <param name="data">Samples of question.</param>
        public SampleQueueArgs(IEnumerable<SampleData> data)
        {
            Samples = data;
        }

        /// <summary>
        /// Constructor that takes an array of samples that were operated upon.
        /// </summary>
        /// <param name="data">Samples of question.</param>
        /// <param name="runningIndex"></param>
        /// <param name="running"></param>
        public SampleQueueArgs(
            IEnumerable<SampleData> data,
            int runningIndex,
            int running)
        {
            Samples = data;
            RunningSamplePosition = runningIndex;

            RunningQueueTotal = running;
        }

        /// <summary>
        /// Gets or sets the flag indicating that samples are running.
        /// </summary>
        public int RunningSamplePosition { get; set; }

        /// <summary>
        /// Gets the samples that had an action performed on them.
        /// </summary>
        public IEnumerable<SampleData> Samples { get; }

        /// <summary>
        ///
        /// </summary>
        public int RunningQueueTotal { get; }
    }
}
