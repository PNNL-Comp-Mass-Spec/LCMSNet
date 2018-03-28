//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/24/2011
//
//*********************************************************************************************************

namespace LcmsNetPlugins.Teledyne.Devices
{
    /// <summary>
    /// Holds data from a the response to a RANGE command
    /// </summary>
    public class classPumpIscoRangeData
    {
        #region "Properties"
            /// <summary>
            /// Max pressure (PSI)
            /// </summary>
            public double MaxPressure { get; set; }

            /// <summary>
            /// Max flow rate (ml/min)
            /// </summary>
            public double MaxFlowRate { get; set; }

            /// <summary>
            /// Max refill rate (ml/min)
            /// </summary>
            public double MaxRefillRate { get; set; }

            /// <summary>
            /// Max volume (ml)
            /// </summary>
            public double MaxVolume { get; set; }
        #endregion

        #region "Constructors"
            /// <summary>
            /// Default constructor
            /// </summary>
            public classPumpIscoRangeData()
            {
                MaxPressure = 10000D;
                MaxFlowRate = 25D;
                MaxRefillRate = 30D;
                MaxVolume = 102.96D;
            }
        #endregion
    }   
}
