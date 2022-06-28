namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Holds data from a the response to a RANGE command
    /// </summary>
    public class IscoPumpRangeData
    {
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

        /// <summary>
        /// Default constructor
        /// </summary>
        public IscoPumpRangeData()
        {
            MaxPressure = 10000D;
            MaxFlowRate = 25D;
            MaxRefillRate = 30D;
            MaxVolume = 102.96D;
        }
    }
}
