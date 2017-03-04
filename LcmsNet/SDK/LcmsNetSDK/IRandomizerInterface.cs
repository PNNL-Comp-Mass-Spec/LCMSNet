//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/28/2009
//
// Updates
// - 03/04/2009 (DAC) - Moved to LcmsNetDataClasses namespace
//
//*********************************************************************************************************

using System.Collections.Generic;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Inteface for sample run order randomizer
    /// </summary>
    public interface IRandomizerInterface
    {

        #region "Methods"

        List<classSampleData> RandomizeSamples(List<classSampleData> InputSampleList);

        #endregion
    }
}
