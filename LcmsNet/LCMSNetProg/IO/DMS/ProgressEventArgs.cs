using System;

namespace LcmsNet.IO.DMS
{
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Description of the current task
        /// </summary>
        public readonly string CurrentTask;

        /// <summary>
        /// Value between 0 and 100
        /// </summary>
        public readonly double PercentComplete;

        public ProgressEventArgs(string currentTask, double percentComplete)
        {
            CurrentTask = currentTask;
            PercentComplete = percentComplete;
        }
    }
}
