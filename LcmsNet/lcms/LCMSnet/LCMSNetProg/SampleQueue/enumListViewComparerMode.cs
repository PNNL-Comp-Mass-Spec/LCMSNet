//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/22/2009
//
// Last modified 01/2/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.SampleQueue
{
    class enumListViewComparerMode
    {
        //*********************************************************************************************************
        // Enums for use by Listview classListViewItemComparer
        //**********************************************************************************************************

        #region "Enums"

        /// <summary>
        /// Used for determining if column is numeric or string
        /// </summary>
        public enum SortModeConstants
        {
            text,
            numeric
        }

        #endregion
    }
} // End namespace