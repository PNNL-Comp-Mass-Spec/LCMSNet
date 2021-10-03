namespace LcmsNet.IO.SQLite
{
    /// <summary>
    /// Describes the available data table names
    /// </summary>
    public enum DatabaseTableTypes
    {
        WaitingQueue,
        RunningQueue,
        CompletedQueue,
        CartList,
        /// <summary>
        /// Active LC Columns
        /// </summary>
        ColumnList,
        CartConfigNameList,
        CartConfigNameSelected,
    }
}
