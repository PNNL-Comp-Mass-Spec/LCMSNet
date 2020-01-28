namespace LcmsNetPlugins.Bruker
{
    /// <summary>
    /// Class for holding response data from sXc
    /// </summary>
    class FtmsResponse
    {
        #region "Properties"

        public short CommandCode { get; set; }
        public short ParamCode { get; set; }
        public string ResponseString { get; set; }
        public bool WaitingForParam { get; set; }

        #endregion

        #region "Constructors"

        public FtmsResponse()
        {
            CommandCode = -256;
            ParamCode = -256;
            ResponseString = "";
            WaitingForParam = false;
        }

        #endregion
    }
}
