//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/16/2009
//
// Last modified 03/16/2009
//                      - 03/16/2009: (BLL) - Created file.
//
//*********************************************************************************************************

using System;

namespace LcmsNetDataClasses.Experiment
{
    /// <summary>
    /// Experiment data
    /// </summary>
    [Serializable]
    public class classLCExperimentData : classDataClassBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the experiment method to run.
        /// </summary>
        public string ExperimentName { get; set; }

        #endregion
    }
}
