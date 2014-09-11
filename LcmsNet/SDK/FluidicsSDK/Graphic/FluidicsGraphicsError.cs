/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/19/2013
 * 
 * Last Modified 8/19/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
