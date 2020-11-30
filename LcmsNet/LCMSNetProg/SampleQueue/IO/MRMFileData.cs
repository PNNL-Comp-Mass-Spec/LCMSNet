namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Holds MRM file data downloaded from DMS
    /// </summary>
    public class MRMFileData
    {
        #region "Properties"

        /// <summary>
        /// Name of MRM file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Contents of MRM file
        /// </summary>
        public string FileContents { get; set; }

        #endregion
    }
}