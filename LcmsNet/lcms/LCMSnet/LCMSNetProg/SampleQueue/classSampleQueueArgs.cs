using System.Collections.Generic;
using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Sample queue arguments used by various delegates.
    /// </summary>
    public class classSampleQueueArgs
    {
        #region Members

        /// <summary>
        /// Array of samples that have been operated on.
        /// </summary>
        private readonly IEnumerable<classSampleData> marray_data;

        #endregion

        /// <summary>
        /// Constructor that takes an array of samples that were operated upon.
        /// </summary>
        /// <param name="data">Samples of question.</param>
        public classSampleQueueArgs(IEnumerable<classSampleData> data)
        {
            marray_data = data;
        }

        /// <summary>
        /// Constructor that takes an array of samples that were operated upon.
        /// </summary>
        /// <param name="data">Samples of question.</param>
        public classSampleQueueArgs(IEnumerable<classSampleData> data, int runningIndex, int running, int complete,
            int waiting)
        {
            marray_data = data;
            RunningSamplePosition = runningIndex;

            WaitingQueueTotal = waiting;
            RunningQueueTotal = running;
            CompleteQueueTotal = complete;
        }

        /// <summary>
        /// Gets or sets the flag indicating that samples are running.
        /// </summary>
        public int RunningSamplePosition { get; set; }

        /// <summary>
        /// Gets the samples that had an action performed on them.
        /// </summary>
        public IEnumerable<classSampleData> Samples => marray_data;

        /// <summary>
        ///
        /// </summary>
        public int RunningQueueTotal { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int CompleteQueueTotal { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int WaitingQueueTotal { get; private set; }
    }
}