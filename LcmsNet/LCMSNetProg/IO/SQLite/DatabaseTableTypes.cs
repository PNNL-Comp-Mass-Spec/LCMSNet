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
        UserList,
        CartList,
        SeparationTypeList,
        SeparationTypeSelected,
        DatasetTypeList,
        InstrumentList,
        /// <summary>
        /// Active LC Columns
        /// </summary>
        ColumnList,
        ExperimentList,
        /// <summary>
        /// A User that's associated with a Proposal
        /// </summary>
        PUserList,
        /// <summary>
        /// A cross reference of the PUser and a Proposal
        /// </summary>
        PReferenceList,
        DatasetList,
        CartConfigNameList,
        CartConfigNameSelected,
        WorkPackages,
        InstrumentGroupList,
    }
}
