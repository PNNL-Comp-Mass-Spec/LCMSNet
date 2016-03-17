using System;

namespace LcmsNetDataClasses
{
    public interface IDmsTools
    {
        string ErrMsg { get; set; }
        string DMSVersion { get; }
        bool ForceValidation { get; }
        // determines if lcmsnet should attempt to force dmsvalidation, and refuse to queue samples if validator not found.
        void GetCartListFromDMS();
        void GetColumnListFromDMS();
        void GetDatasetTypeListFromDMS();
        void GetEntireColumnListListFromDMS();
        void GetExperimentListFromDMS();
        void GetInstrumentListFromDMS();
        System.Collections.Generic.Dictionary<int, int> GetMRMFileListFromDMS(int MinID, int MaxID);

        void GetMRMFilesFromDMS(string FileIndxList,
            ref System.Collections.Generic.List<LcmsNetDataClasses.classMRMFileData> fileData);

        void GetProposalUsers();

        System.Collections.Generic.List<LcmsNetDataClasses.classSampleData> GetSamplesFromDMS(
            classSampleQueryData queryData);

        void GetSepTypeListFromDMS();
        void GetUserListFromDMS();
        void LoadCacheFromDMS();
        void LoadCacheFromDMS(bool shouldLoadExperiment);
        bool UpdateDMSCartAssignment(string requestList, string cartName, bool updateMode);
    }
}