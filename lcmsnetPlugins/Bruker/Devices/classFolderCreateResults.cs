
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
//*********************************************************************************************************
using System;

namespace LcmsNet.Devices.BrukerStart
{
    /// <summary>
    /// Class used for transferring results of output folder creation
    /// </summary>
    class classFolderCreateResults
    {
        #region "Properties"
            public bool Success { get; set; }
            public string DirectoryName { get; set; }
            public string Message { get; set; }
            public Exception CreationException { get; set; }
        #endregion

        #region "Constructors"
            public classFolderCreateResults()
            {
                Success = false;
                DirectoryName = "";
                Message = "";
                CreationException = null;
            }
        #endregion
    }   
}
