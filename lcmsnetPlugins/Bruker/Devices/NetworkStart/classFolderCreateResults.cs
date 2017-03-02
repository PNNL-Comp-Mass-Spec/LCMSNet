
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
// Last modified 06/29/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.BrukerStart
{
    class classFolderCreateResults
    {
        //*********************************************************************************************************
        // Class used for transferring results of output folder creation
        //**********************************************************************************************************

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
}   // End namespace
