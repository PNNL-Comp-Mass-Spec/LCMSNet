using System;

namespace FluidicsSDK.Graphic
{
    [Serializable()]
    public class FluidicsGraphicsError:Exception
    {
        //constructors
        public FluidicsGraphicsError() : base() { }
        public FluidicsGraphicsError(string message):base(message) { }
        public FluidicsGraphicsError(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected FluidicsGraphicsError(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
