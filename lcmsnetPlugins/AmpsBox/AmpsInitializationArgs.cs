using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmpsBox
{
    public class AmpsInitializationArgs: EventArgs
    {
        /// <summary>
        /// Gets how many HV power supplies are available.
        /// </summary>
        public int HvCount { get; set; }
        /// <summary>
        /// Gets how many RF channels are available.
        /// </summary>
        public int RfCount { get; set; }
    }
}
