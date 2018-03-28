/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/

using LcmsNetSDK.Data;

namespace LcmsNet.Method
{
    public class classColumnArgs
    {
        public classColumnArgs(SampleData data)
        {
            Sample = data;
        }

        public SampleData Sample { get; private set; }
    }
}