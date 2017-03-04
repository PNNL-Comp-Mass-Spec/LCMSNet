//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 04/02/2009
//
//*********************************************************************************************************

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Holds MRM file data downloaded from DMS
    /// </summary>
    public class classMRMFileData
    {
        #region "Properties"

        /// <summary>
        /// Name of MRM file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Contents of MRM file
        /// </summary>
        public string FileContents { get; set; }

        #endregion
    }
}