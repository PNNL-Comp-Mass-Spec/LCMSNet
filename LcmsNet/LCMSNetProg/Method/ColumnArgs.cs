/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 *********************************************************************************************************/

using LcmsNetSDK.Data;

namespace LcmsNet.Method
{
    public class ColumnArgs
    {
        public ColumnArgs(SampleData data)
        {
            Sample = data;
        }

        public SampleData Sample { get; }
    }
}