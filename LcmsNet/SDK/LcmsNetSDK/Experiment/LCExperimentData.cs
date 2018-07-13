//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/16/2009
//
//*********************************************************************************************************

using System;
using LcmsNetData.Data;

namespace LcmsNetSDK.Experiment
{
    /// <summary>
    /// Experiment data
    /// </summary>
    [Serializable]
    public class LCExperimentData : LcmsNetDataClassBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the experiment method to run.
        /// </summary>
        public string ExperimentName { get; set; }

        #endregion
    }
}
