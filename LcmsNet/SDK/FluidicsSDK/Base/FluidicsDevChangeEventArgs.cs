using System;
using System.Windows;

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

        public Point? OldLocation { get; }

        public string Message { get; }
    }
}
