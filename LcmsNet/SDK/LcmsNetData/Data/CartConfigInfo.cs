namespace LcmsNetData.Data
{
    /// <summary>
    /// Class to hold information about a single LC Cart Config
    /// </summary>
    public class CartConfigInfo
    {
        /// <summary>
        /// Name of LC Cart Config
        /// </summary>
        public string CartConfigName { get; set; }

        /// <summary>
        /// Name of Cart
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Constructor: Known values
        /// </summary>
        /// <param name="cartConfigName"></param>
        /// <param name="cartName"></param>
        public CartConfigInfo(string cartConfigName, string cartName)
        {
            CartConfigName = cartConfigName;
            CartName = cartName;
        }

        /// <summary>
        /// Constructor: for auto-population
        /// </summary>
        public CartConfigInfo()
        {
        }

        public override string ToString()
        {
            return CartConfigName;
        }
    }
}
