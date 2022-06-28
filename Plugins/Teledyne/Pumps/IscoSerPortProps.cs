using System.IO.Ports;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Class to hold settings for the pump serial port
    /// </summary>
    class IscoSerPortProps
    {
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

        public IscoSerPortProps()
        {
            PortName = CONST_DFLT_COMM_PORT;
            BaudRate = CONST_DFLT_BAUD_RATE;
            UnitAddress = CONST_DFLT_UNIT_ADDR;
            ReadTimeout = CONST_DFLT_READTIMEOUT;
            WriteTimeout = CONST_DFLT_WRITETIMEOUT;
            StopBits = CONST_DFLT_STOPBITS;
            DataBits = CONST_DFLT_DATABITS;
            HandShake = CONST_DFLT_HANDSHAKE;
            Parity = CONST_DFLT_PARITY;
            NewLine = CONST_DFLT_NEWLINE;
        }
    }
}
