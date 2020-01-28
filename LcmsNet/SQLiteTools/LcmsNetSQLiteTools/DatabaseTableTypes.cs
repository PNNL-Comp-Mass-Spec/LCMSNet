namespace LcmsNetSQLiteTools
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
        ColumnList, // Active LC Columns
        ExperimentList,
        PUserList, // A User that's in a Proposal
        PReferenceList, // A cross reference of the PUser and a Proposal
        DatasetList,
        CartConfigNameList,
        CartConfigNameSelected,
        WorkPackages
    }
}
