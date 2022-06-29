namespace LcmsNet.IO.DMS
{
    public static class DMSConnectionManager
    {
        public static DMSDBTools DBTools { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        static DMSConnectionManager()
        {
            DBTools = new DMSDBTools();
        }
    }
}
