namespace LcmsNet.IO.DMS.Data
{
    /// <summary>
    /// Class to hold data about LcmsNet users
    /// </summary>
    public class UserInfo
    {
        #region "Properties"

        /// <summary>
        /// Name of user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Payroll number (network login) of user
        /// </summary>
        public string PayrollNum { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            var name = string.IsNullOrWhiteSpace(UserName) ? "Undefined user" : UserName;

            if (string.IsNullOrWhiteSpace(PayrollNum))
            {
                return name;
            }

            return PayrollNum + ": " + name;
        }

        #endregion
    }
}
