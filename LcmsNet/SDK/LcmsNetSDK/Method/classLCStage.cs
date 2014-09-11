using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Method
{
    /// <summary>
    /// A stage is a block of atomic operations whose generic operation is protected by the length of time for this stage.
    /// </summary>
    public class classLCStage
    {
        /// <summary>
        /// Gets or sets the actions to be performed by this method.
        /// </summary>
        public List<classLCAction> Actions { get; set; }
        /// <summary>
        /// Gets or sets whether to optimize this stage.
        /// </summary>
        public bool Optimize { get; set; }
        /// <summary>
        /// Gets or sets the start time of this stage.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Gets or sets the duration for this stage.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
