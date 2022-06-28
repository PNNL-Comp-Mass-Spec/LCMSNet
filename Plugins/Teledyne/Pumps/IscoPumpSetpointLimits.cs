namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Holds instrument setpoint ranges
    /// </summary>
    public class IscoPumpSetpointLimits
    {
        /// <summary>
        /// Min flow SP
        /// </summary>
        public double MinFlowSp { get; set; }

        /// <summary>
        /// Max flow SP
        /// </summary>
        public double MaxFlowSp { get; set; }

        /// <summary>
        /// Maximum flow limit
        /// </summary>
        public double MaxFlowLimit { get; set; }

        /// <summary>
        /// Min pressure SP
        /// </summary>
        public double MinPressSp { get; set; }

        /// <summary>
        /// Max pressure SP
        /// </summary>
        public double MaxPressSp { get; set; }

        /// <summary>
        /// Max refill rate SP
        /// </summary>
        public double MaxRefillRateSp { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public IscoPumpSetpointLimits()
        {
            MinFlowSp = 0.0010D;
            MaxFlowSp = 25D;
            MinPressSp = 10D;
            MaxPressSp = 10000D;
            MaxRefillRateSp = 30D;
        }
    }
}
