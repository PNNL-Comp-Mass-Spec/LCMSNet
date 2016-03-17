/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 * Last Modified 8/1/2014 By Christopher Walters
 *********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses;

namespace LcmsNet.Method
{
    public class classColumnArgs
    {
        public classColumnArgs(classSampleData data)
        {
            Sample = data;
        }

        public classSampleData Sample { get; private set; }
    }
}