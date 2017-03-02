//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
// Last modified 06/29/2010
//*********************************************************************************************************

namespace LcmsNet.Devices.BrukerStart
{
    class classFtmsResponse
    {
        //*********************************************************************************************************
        // Class for holding response data from sXc
        //**********************************************************************************************************

        #region "Properties"
            public short CommandCode { get; set; }
            public short ParamCode { get; set; }
            public string ResponseString { get; set; }
            public bool WaitingForParam { get; set; }
        #endregion

        #region "Constructors"
            public classFtmsResponse()
            {
                CommandCode = -256;
                ParamCode = -256;
                ResponseString = "";
                WaitingForParam = false;
            }
        #endregion
    }   
}   // End namespace
