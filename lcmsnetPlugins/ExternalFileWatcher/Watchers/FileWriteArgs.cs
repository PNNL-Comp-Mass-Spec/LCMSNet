using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExternalFileWatcher.Watchers
{
    /// <summary>
    /// Class for event arguments when an externale file has been written.
    /// </summary>
    public class FileWriteArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lastWrite"></param>
        public FileWriteArgs(string path,
                                DateTime lastWrite,
                                DateTime untilTrigger)
        {
            Path          = path;
            LastWrite     = lastWrite;
            TimeUntilDone = untilTrigger;
        }

        /// <summary>
        /// Gets the path of the file.
        /// </summary>
        public string Path
        {
            private set;
            get;
        }
        /// <summary>
        /// Gets the last time written to.
        /// </summary>
        public DateTime LastWrite
        {
            private set;
            get;
        }
        public DateTime TimeUntilDone
        {
            private set;
            get;
        }
    }
}
