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
using LcmsNetSDK.Data;

namespace LcmsNetSDK
{
    /// <summary>
    /// Inteface for sample run order randomizer
    /// </summary>
    public interface IRandomizerInterface
    {
        #region "Methods"

        List<SampleData> RandomizeSamples(List<SampleData> InputSampleList);

        #endregion
    }
}
