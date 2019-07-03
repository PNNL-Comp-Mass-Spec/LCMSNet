namespace LcmsNetData.Data
{
    /// <summary>
    /// Class to hold information about a work package
    /// </summary>
    public class WorkPackageInfo : LcmsNetDataClassBase
    {
        /// <summary>
        /// Constructor: for auto-population
        /// </summary>
        public WorkPackageInfo()
        {
        }

        /// <summary>
        /// Constructor: Known Values
        /// </summary>
        /// <param name="chargeCode"></param>
        /// <param name="state"></param>
        /// <param name="subAccount"></param>
        /// <param name="wbs"></param>
        /// <param name="title"></param>
        /// <param name="ownerUserName"></param>
        /// <param name="ownerName"></param>
        public WorkPackageInfo(string chargeCode, string state, string subAccount, string wbs, string title,
            string ownerUserName, string ownerName)
        {
            ChargeCode = chargeCode;
            State = state;
            SubAccount = subAccount;
            WorkBreakdownStructure = wbs;
            Title = title;
            OwnerUserName = ownerUserName;
            OwnerName = ownerName;
        }

        /// <summary>
        /// Charge code for this work package
        /// </summary>
        public string ChargeCode { get; set; }

        /// <summary>
        /// Valid content: (Tuple, separated by a comma and space) "(Active|Inactive)(, (used|unused|old))?"
        /// 'Active' can be by itself, 'used' is only paired with 'Inactive'
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Work package subaccount name
        /// </summary>
        public string SubAccount { get; set; }

        /// <summary>
        /// Work package group (WBS) title
        /// </summary>
        public string WorkBreakdownStructure { get; set; }

        /// <summary>
        /// Work package title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Username of work package owner
        /// </summary>
        public string OwnerUserName { get; set; }

        /// <summary>
        /// Name of work package owner
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// ToString override for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ChargeCode}: {Title}";
        }
    }
}
