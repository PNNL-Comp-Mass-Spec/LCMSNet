using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using LcmsNet.Configuration;
using LcmsNet.Data;
using LcmsNet.IO.DMS;
using LcmsNet.IO.SQLite;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    /// <summary>
    /// Form for retrieval of run requests from DMS
    /// </summary>
    public class DMSDownloadViewModel : ReactiveObject
    {
        #region "Properties"

        /// <summary>
        /// Connection string for DMS SQL Server connection
        /// </summary>
        public string DMSConnStr { get; set; }

        /// <summary>
        /// Command for FIND button to load available request list from DMS
        /// </summary>
        public ReactiveCommand<Unit, Unit> FindCommand { get; }

        /// <summary>
        /// Command for MoveDown button to move requests from Available Requests to Requests To Run list
        /// </summary>
        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; }

        /// <summary>
        /// Command for MoveUp button to move requests from Requests To Run to Available Requests list
        /// </summary>
        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }

        /// <summary>
        /// Command to sort available requests by batch, block, and run order
        /// </summary>
        public ReactiveCommand<Unit, Unit> SortByBatchBlockRunOrderCommand { get; }

        #endregion

        #region "Class variables"

        private readonly List<DmsDownloadData> dmsRequestList = new List<DmsDownloadData>();
        private string matchString;

        private string requestName = string.Empty;
        private string requestIdStart = string.Empty;
        private string requestIdEnd = string.Empty;
        private string cart = string.Empty;
        private string wellplate = string.Empty;
        private string batchId = string.Empty;
        private string block = string.Empty;
        private bool unassignedRequestsOnly;
        private readonly SourceList<string> lcCartSearchComboBoxOptions = new SourceList<string>();
        private DMSDownloadDataViewModel availableRequestData = new DMSDownloadDataViewModel(true);
        private bool loadingData;
        private string requestsFoundString = string.Empty;
        private DMSDownloadDataViewModel selectedRequestData = new DMSDownloadDataViewModel(false);
        private bool blockingEnabled = false;

        public string WindowTitle { get; }

        public string RequestName
        {
            get => requestName;
            set => this.RaiseAndSetIfChanged(ref requestName, value);
        }

        public string RequestIdStart
        {
            get => requestIdStart;
            set => this.RaiseAndSetIfChanged(ref requestIdStart, value);
        }

        public string RequestIdEnd
        {
            get => requestIdEnd;
            set => this.RaiseAndSetIfChanged(ref requestIdEnd, value);
        }

        public string Cart
        {
            get => cart;
            set => this.RaiseAndSetIfChanged(ref cart, value);
        }

        public string Wellplate
        {
            get => wellplate;
            set => this.RaiseAndSetIfChanged(ref wellplate, value);
        }

        public string BatchId
        {
            get => batchId;
            set => this.RaiseAndSetIfChanged(ref batchId, value);
        }

        public string Block
        {
            get => block;
            set => this.RaiseAndSetIfChanged(ref block, value);
        }

        public bool UnassignedRequestsOnly
        {
            get => unassignedRequestsOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref unassignedRequestsOnly, value);
                this.RaisePropertyChanged(nameof(AssignedRequestsOnly));
            }
        }

        public bool AssignedRequestsOnly => !unassignedRequestsOnly;

        public ReadOnlyObservableCollection<string> LcCartSearchComboBoxOptions { get; }

        public DMSDownloadDataViewModel AvailableRequestData
        {
            get => availableRequestData;
            private set => this.RaiseAndSetIfChanged(ref availableRequestData, value);
        }

        public bool LoadingData
        {
            get => loadingData;
            private set => this.RaiseAndSetIfChanged(ref loadingData, value);
        }

        public string RequestsFoundString
        {
            get => requestsFoundString;
            private set => this.RaiseAndSetIfChanged(ref requestsFoundString, value);
        }

        public DMSDownloadDataViewModel SelectedRequestData
        {
            get => selectedRequestData;
            private set => this.RaiseAndSetIfChanged(ref selectedRequestData, value);
        }

        private bool BlockingEnabled
        {
            get => blockingEnabled;
            set => this.RaiseAndSetIfChanged(ref blockingEnabled, value);
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public DMSDownloadViewModel()
        {
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            lcCartSearchComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcCartSearchComboBoxOptionsBound).Subscribe();
            LcCartSearchComboBoxOptions = lcCartSearchComboBoxOptionsBound;

            // Initialize form controls
            // Form caption
            string dbInUse;
            try
            {
                if (LcmsNet.Configuration.DMSDataContainer.DBTools.DMSVersion.Contains("_T3"))
                {
                    dbInUse = " (Using Development Database)";
                }
                else
                {
                    dbInUse = " (Using Production Database)";
                }
            }
            catch (Exception ex)
            {
                dbInUse = " (Using ?? database: " + ex.Message + ")";
            }

            WindowTitle = "LcmsNet V" + Assembly.GetEntryAssembly().GetName().Version + dbInUse;

            // Load the LC cart lists
            RefreshCartList();

            FindCommand = ReactiveCommand.CreateFromTask(FindDmsRequests);
            MoveDownCommand = ReactiveCommand.Create(MoveRequestsToRunList);
            MoveUpCommand = ReactiveCommand.Create(RemoveRequestsFromRunList);
            SortByBatchBlockRunOrderCommand = ReactiveCommand.Create(SortByBatchBlockRunOrder, this.WhenAnyValue(x => x.BlockingEnabled));
        }

        /// <summary>
        /// Loads the LC cart dropdowns with data from cache
        /// </summary>
        private void RefreshCartList()
        {
            List<string> cartList;
            lcCartSearchComboBoxOptions.Clear();

            // Get the list of carts from DMS
            try
            {
                cartList = DMSDataContainer.DBTools.GetCartListFromDMS();
            }
            catch (DatabaseConnectionStringException ex)
            {
                // The SQLite connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart listing.\r\n" +
                    "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return;
            }
            catch (DatabaseDataException ex)
            {
                // There was a problem getting the list of LC carts from the cache db
                var innerException = string.Empty;
                if (ex.InnerException != null)
                    innerException = ex.InnerException.Message;
                var errMsg = "Exception getting LC cart list from DMS: " + innerException + "\r\n" +
                    "As a workaround, you may manually type the cart name when needed.\r\n" +
                    "You may retry retrieving the cart list later, if desired.";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return;
            }

            if (cartList.Any())
            {
                lcCartSearchComboBoxOptions.Edit(list =>
                {
                    // Add a blank for "No cart specified"
                    list.Add("");
                    list.AddRange(cartList);
                });
            }
        }

        /// <summary>
        /// Loads listViewAvailableRequests with all requests in DMS matching specified criteria
        /// </summary>
        private async Task FindDmsRequests()
        {
            List<DmsDownloadData> tempRequestList;

            // Fill an object with the data from the UI, then pass to DMSTools class to run the query
            var queryData = new SampleQueryData {
                RequestName = this.RequestName
            };

            // If min request number input is not specified, set it to 0
            if (string.IsNullOrEmpty(RequestIdStart))
            {
                queryData.MinRequestNum = "0";
            }
            else
            {
                queryData.MinRequestNum = RequestIdStart;
            }

            //If max request number input is not specified, set it to 1,000,000,000
            if (string.IsNullOrEmpty(RequestIdEnd))
            {
                queryData.MaxRequestNum = "1000000000";
            }
            else
            {
                queryData.MaxRequestNum = RequestIdEnd;
            }

            // If "unassigned only" is checked, override cart selection combo box
            //string cartToFind;
            if (UnassignedRequestsOnly)
            {
                queryData.Cart = "unknown";
            }
            else
            {
                queryData.Cart = Cart;
            }

            queryData.BatchID = BatchId;
            queryData.Block = Block;
            queryData.Wellplate = Wellplate;

            // Blank listview and display wait message
            LoadingData = true;

            // Clear the available datasets listview
            AvailableRequestData.ClearSamples();

            tempRequestList = await Task.Run(() => GetDMSData(queryData));

            if (tempRequestList == null)
            {
                return;
            }

            // Check to see if any items were found
            LoadingData = false;
            if (!tempRequestList.Any())
            {
                MessageBox.Show("No requests found in DMS");
                RequestsFoundString = "0 requests found";
                return;
            }
            RequestsFoundString = tempRequestList.Count + " requests found";

            // Add the requests to the listview
            var availReqList = new List<DmsDownloadData>();
            foreach (var request in tempRequestList)
            {
                // Determine if already in list of requests
                matchString = request.RequestName;
                var foundIndx = dmsRequestList.FindIndex(PredContainsRequestName);
                if (foundIndx == -1)
                {
                    // Request not found, so add to list of all requests
                    dmsRequestList.Add(request);
                    // Add request data to the listview
                    availReqList.Add(request);
                }
                else
                {
                    if (dmsRequestList[foundIndx].SelectedToRun)
                    {
                        // Request was found and is already in Requests To Run list, so do nothing
                    }
                    else
                    {
                        availReqList.Add(request);
                    }
                }
            }

            AvailableRequestData.AddSamples(availReqList);

            // Hide the wait message and display the listview again
            LoadingData = false;
            //classStatusTools.SendStatusMsg("Found " + listviewAvailableRequests.Items.Count.ToString() + " requests in DMS");

            // Check to see if any items have blocking enabled
            BlockingEnabled = IsBlockingEnabled(tempRequestList);
            if (BlockingEnabled)
            {
                var msg =
                    "You have downloaded samples that have blocking enabled. Please be sure you have downloaded the " +
                    "entire block of samples needed";
                MessageBox.Show(msg, "Blocked Samples Downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private List<DmsDownloadData> GetDMSData(SampleQueryData queryData)
        {
            var tempRequestList = new List<DmsDownloadData>();

            // Get a list of requests from DMS
            try
            {
                var dmsTools = LcmsNet.Configuration.DMSDataContainer.DBTools;
                tempRequestList = dmsTools.GetRequestedRunsFromDMS(queryData).ToList();
            }
            catch (DatabaseConnectionStringException ex)
            {
                // The DMS connection string wasn't found
                var errMsg = ex.Message + " while getting request listing\r\n";
                errMsg = errMsg + "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return null;
            }
            catch (DatabaseDataException ex)
            {
                var errMsg = ex.Message;
                if (ex.InnerException != null)
                    errMsg += ": " + ex.InnerException.Message;

                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
                return null;
            }
            finally
            {
                LoadingData = false;
            }

            return tempRequestList;
        }

        /// <summary>
        /// Tests downloaded samples to determine if any have blocking enabled
        /// </summary>
        /// <param name="inputData">List containing downloaded samples</param>
        /// <returns>TRUE if any samples have blocking enabled; otherwise FALSE</returns>
        private bool IsBlockingEnabled(List<DmsDownloadData> inputData)
        {
            foreach (var testSample in inputData)
            {
                if (testSample.Block > 0)
                {
                    // Blocking is enabled for this sample, no further test required
                    return true;
                }
            }
            // If we got to here, no samples have blocking enabled
            return false;
        }

        /// <summary>
        /// Sorts the available requests by batch, block, and run order
        /// </summary>
        private void SortByBatchBlockRunOrder()
        {
            AvailableRequestData.SortByBatchBlockRunOrder();
        }

        /// <summary>
        /// Moves items from the Available Requests list to the Requests To Run list
        /// </summary>
        private void MoveRequestsToRunList()
        {
            // Copy selected items and update main list of DMS items
            var data = AvailableRequestData.SelectedData.ToList();
            // Remove the selected items from the Available Requests listview
            AvailableRequestData.RemoveSamples(data);
            // Add the selected items to the selected list
            SelectedRequestData.AddSamples(data);
            foreach (var tempItem in data)
            {
                // Update main list of DMS items
                tempItem.SelectedToRun = true;
            }
        }

        /// <summary>
        /// Removes requests from the "Requests to run" list
        /// </summary>
        private void RemoveRequestsFromRunList()
        {
            var data = SelectedRequestData.SelectedData.ToList();
            AvailableRequestData.AddSamples(data);
            SelectedRequestData.RemoveSamples(data);
            foreach (var tempItem in SelectedRequestData.SelectedData)
            {
                // Update main list of DMS items
                if (!dmsRequestList.Remove(tempItem))
                {
                    var tempStr = "Request " + tempItem.RequestName + " not found in requested run collection";
                    MessageBox.Show(tempStr);
                    return;
                }
            }
        }

        /// <summary>
        /// Predicate function to find index of request matching specified request number
        /// Used for m_DmsRequestList FindIndex method
        /// </summary>
        /// <param name="request">classDMSData object passed in from FindIndex method</param>
        /// <returns>True if match is made; otherwise False</returns>
        private bool PredContainsRequestName(DmsDownloadData request)
        {
            if (string.Equals(request.RequestName, matchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all data on the DMS form
        /// </summary>
        public void ClearForm()
        {
            AvailableRequestData.ClearSamples();
            SelectedRequestData.ClearSamples();
            dmsRequestList.Clear();
        }

        /// <summary>
        /// Transfers the current list of classDMSData objects that have been selected
        /// for running to the calling program as a list of SampleData objects
        /// </summary>
        /// <returns>List of SampleData objects</returns>
        public List<SampleData> GetNewSamplesDMSView()
        {
            var retList = new List<SampleData>();

            foreach (var dmsData in SelectedRequestData.Data)
            {
                var tempSampleData = new SampleData(true, new DMSData(dmsData))
                {
                    PAL =
                    {
                        Well = dmsData.PalWell,
                        WellPlate = dmsData.PalWellPlate
                    }
                };

                retList.Add(tempSampleData);
            }

//            classStatusTools.SendStatusMsg("Adding " + retList.Count.ToString() + " samples from DMS");
            return retList;
        }

        #endregion

    }
}
