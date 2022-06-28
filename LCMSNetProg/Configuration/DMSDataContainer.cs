using LcmsNet.IO.DMS;

namespace LcmsNet.Configuration
{
    public static class DMSDataContainer
    {
        public static DMSDBTools DBTools { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        static DMSDataContainer()
        {
            DBTools = new DMSDBTools();
        }
    }
}
