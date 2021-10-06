namespace LcmsNetSDK.Data
{
    /// <summary>
    /// A bridge between our DLL and our concrete classes for writing LC methods.
    /// </summary>
    public interface IMethodWriter
    {
        void WriteMethodFiles(ISampleInfo sample);

        /// <summary>
        /// Test for presence of completed sample method folders that need to be moved to DMS
        /// </summary>
        /// <returns>TRUE if files found; FALSE otherwise</returns>
        bool CheckLocalMethodFolders();

        /// <summary>
        /// Moves local sample method files to the DMS transfer folder
        /// </summary>
        void MoveLocalMethodFiles();

        /// <summary>
        /// Creates the remote system path.
        /// </summary>
        /// <returns></returns>
        string CreateRemoteFolderPath();
    }
}