//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/28/2009
//
// Last modified 01/28/2009
//						- 03/04/2009 (DAC) - Moved to LcmsNetDataClasses namespace
//
//*********************************************************************************************************

using System.Collections.Generic;

namespace LcmsNetDataClasses
{
    public interface IRandomizerInterface
    {
        //*********************************************************************************************************
        // Inteface for sample run order randomizer
        //**********************************************************************************************************

        #region "Methods"

        List<classSampleData> RandomizeSamples(List<classSampleData> InputSampleList);

        #endregion
    } // End interface
} // End namespace