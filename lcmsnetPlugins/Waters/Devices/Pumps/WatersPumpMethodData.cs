using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waters.Devices.Pumps
{
    /// <summary>
    /// Class that holds information about a waters pump.
    /// </summary>
    public class WatersPumpMethodData
    {
        /// <summary>
        /// Gets or sets the path associated with the method.
        /// </summary>
        public string Path
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the method data associated with the pump
        /// </summary>
        public string MethodData
        {
            get;
            set;
        }
    }
}
