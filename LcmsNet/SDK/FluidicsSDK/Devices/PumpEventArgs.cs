/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 10/7/2013
 *
 * Last Modified 10/7/2013 By Christopher Walters
 ********************************************************************************************************/
using System;

namespace FluidicsSDK.Devices
{
    public class PumpEventArgs<T>:EventArgs
    {
        public PumpEventArgs( T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}