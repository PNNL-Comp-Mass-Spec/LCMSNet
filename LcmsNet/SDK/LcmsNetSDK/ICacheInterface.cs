//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/21/2009
//
// Updates:
// - 02/23/2009 (DAC) - Corrected bug in interface syntax
//
//*********************************************************************************************************

using System.Collections.Generic;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Interface for data classes that use the cache database
    /// </summary>
    public interface ICacheInterface
    {

        #region "Methods"

        Dictionary<string, string> GetPropertyValues();

        void LoadPropertyValues(Dictionary<string,string> propValues);

        #endregion
    }
}
