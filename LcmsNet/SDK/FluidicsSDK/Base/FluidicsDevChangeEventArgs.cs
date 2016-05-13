using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FluidicsSDK.Base
{
    /// <summary>
    /// eventargs for a fluidics device change event
    /// </summary>
    public class FluidicsDevChangeEventArgs:EventArgs
    {        

        public FluidicsDevChangeEventArgs()
        {
            OldLocation = null;
            Message = null;
        }

        public FluidicsDevChangeEventArgs(Point oldLocation, string message = null)
        {
            OldLocation = oldLocation;
            Message = message;
        }

        public FluidicsDevChangeEventArgs(string message)
        {
            OldLocation = null;
            Message = message;
        }
        
        public Nullable<Point> OldLocation
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

    }
}
