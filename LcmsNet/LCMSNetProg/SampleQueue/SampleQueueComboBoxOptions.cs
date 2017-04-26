using System;
using System.Windows;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSQLiteTools;
using ReactiveUI;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// A class to statically hold the options allowed for the combo boxes displayed in the sample queue
    /// Using a static class since this data is the same for all data in an instance of LCMSNet, and any changes should appear everywhere they are used.
    /// </summary>
    public class SampleQueueComboBoxOptions
    {
        #region Static data

        public static ReactiveList<classLCMethod> LcMethodOptions { get; private set; }
        public static ReactiveList<string> InstrumentMethodOptions { get; private set; }
        public static ReactiveList<string> DatasetTypeOptions { get; private set; }
        public static ReactiveList<string> CartConfigOptions { get; private set; }
        public static ReactiveList<string> PalTrayOptions { get; private set; }

        // If null, no error retrieving the cart config names from the database; otherwise, the error that occurred
        public static string CartConfigOptionsError { get; private set; }

        static SampleQueueComboBoxOptions()
        {
            LcMethodOptions = new ReactiveList<classLCMethod>();
            DatasetTypeOptions = new ReactiveList<string>();
            PalTrayOptions = new ReactiveList<string>();
            InstrumentMethodOptions = new ReactiveList<string>();
            CartConfigOptions = new ReactiveList<string>();
            CartConfigOptionsError = null;

#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                CartConfigOptionsError = "Values not read, because we are running in Visual Studio's Design Mode";
                return;
            }
#endif

            // Add the dataset type items to the data grid
            try
            {
                var datasetTypes = classSQLiteTools.GetDatasetTypeList(false);
                DatasetTypeOptions.Clear();
                DatasetTypeOptions.AddRange(datasetTypes);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(1, "The sample queue could not load the dataset type list.", ex);
            }

            // Get the list of cart configuration names from DMS
            try
            {
                var totalConfigCount = classSQLiteTools.GetCartConfigNameList(false).Count;
                var cartName = classCartConfiguration.CartName;
                var cartConfigList = classSQLiteTools.GetCartConfigNameList(cartName, false);
                if (cartConfigList.Count > 0)
                {
                    CartConfigOptions.Clear();
                    CartConfigOptions.AddRange(cartConfigList);
                }
                else
                {
                    if (totalConfigCount > 0)
                    {
                        CartConfigOptionsError = "No cart configurations found that match the supplied cart name: \"" + cartName + "\".\n" +
                                                 "Fix: close, fix the cart name, and restart.";
                    }
                    else
                    {
                        CartConfigOptionsError = "No cart configurations found. Can this computer communicate with DMS? Fix and restart LCMSNet";
                    }
                }
            }
            catch (classDatabaseConnectionStringException ex)
            {
                // The SQLite connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart config name listing.\r\n" +
                             "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return;
            }
            catch (classDatabaseDataException ex)
            {
                // There was a problem getting the list of LC carts from the cache db
                var innerException = string.Empty;
                if (ex.InnerException != null)
                    innerException = ex.InnerException.Message;
                var errMsg = "Exception getting LC cart config name list from DMS: " + innerException + "\r\n" +
                             "As a workaround, you may manually type the cart config name when needed.\r\n" +
                             "You may retry retrieving the cart list later, if desired.";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return;
            }
        }

        #endregion
    }
}
