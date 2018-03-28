//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 02/22/2011
//
//*********************************************************************************************************

using System.IO.Ports;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Class to hold settings for the pump serial port
    /// </summary>
    class IscoSerPortProps
    {
        #region "Constants"
            const string CONST_DFLT_COMM_PORT = "COM1";
            const int CONST_DFLT_BAUD_RATE = 9600;
            const int CONST_DFLT_UNIT_ADDR = 6;
            const int CONST_DFLT_READTIMEOUT = 500;          //milliseconds
            const int CONST_DFLT_WRITETIMEOUT = 500;         //milliseconds
            const StopBits CONST_DFLT_STOPBITS = StopBits.One;
            const int CONST_DFLT_DATABITS = 8;
            const Handshake CONST_DFLT_HANDSHAKE = Handshake.None;
            const Parity CONST_DFLT_PARITY = Parity.None;
            const string CONST_DFLT_NEWLINE = "\r";
            const int CONST_MIN_UNIT_ADDR = 1;
            const int CONST_MAX_UNIT_ADDR = 7;
        #endregion

        #region "Properties"
            public string PortName { get; set; }
            public int BaudRate { get; set; }
            public int UnitAddress { get; set; }
            public int ReadTimeout { get; set; }
            public int WriteTimeout { get; set; }
            public StopBits StopBits { get; set; }
            public int DataBits { get; set; }
            public Handshake HandShake { get; set; }
            public Parity Parity { get; set; }
            public string NewLine { get; set; }
            public int UnitAddressMin => CONST_MIN_UNIT_ADDR;
        public int UnitAddressMax => CONST_MAX_UNIT_ADDR;

        #endregion

        #region "Constructors"
            public IscoSerPortProps()
            {
                this.PortName = CONST_DFLT_COMM_PORT;
                this.BaudRate = CONST_DFLT_BAUD_RATE;
                this.UnitAddress = CONST_DFLT_UNIT_ADDR;
                this.ReadTimeout = CONST_DFLT_READTIMEOUT;
                this.WriteTimeout = CONST_DFLT_WRITETIMEOUT;
                this.StopBits = CONST_DFLT_STOPBITS;
                this.DataBits = CONST_DFLT_DATABITS;
                this.HandShake = CONST_DFLT_HANDSHAKE;
                this.Parity = CONST_DFLT_PARITY;
                this.NewLine = CONST_DFLT_NEWLINE;
            }
        #endregion
    }
}
