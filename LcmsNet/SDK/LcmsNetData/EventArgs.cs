﻿using System;

namespace LcmsNetData
{
    public class MessageEventArgs : EventArgs
    {
        public readonly string Message;

        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }

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
