//*********************************************************************************************************
// Written by Dave Clark, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/08/2009
//
// Last modified 09/30/2014
//                      - 02/04/2009 (DAC) - Changed to use List<classSampleData> instead of classDMSData
//                      - 05/12/2009 (DAC) - Added handling of blocking and run order values
//                      - 08/11/2009 (DAC) - Added batch value
//                      - 02/18/2010 (DAC) - Added block value to sample query; restructured DMS sample query
//                      - 12/08/2010 (DAC) - Modified form caption to reflect DMS version in use
//                      - 09/11/2014 (CJW) - Modified to use new classDMSToolsManager
//                      - 09/30/2014 (CJW) - bug fixes
//*********************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetSDK;
using LcmsNetSQLiteTools;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Form for retrieval of run requests from DMS
    /// </summary>
    public partial class formDMSView : Form
    {
        #region "Properties"

        /// <summary>
        /// Connection string for DMS SQL Server connection
        /// </summary>
        public string DMSConnStr { get; set; }

        #endregion

        #region "Class variables"

        List<classSampleData> m_DmsRequestList;
        string m_MatchString;
        string m_CartName;
        string m_CartConfigName;

        // These two string dictionaries hold selected column and sort orders for the listview
        // Using string dictionaries allows me to make the event handler for the column click event common
        //   to both listviews
        readonly Dictionary<string, string> m_ListViewColumns = new Dictionary<string, string>();
        readonly Dictionary<string, string> m_ListViewSortOrder = new Dictionary<string, string>();

        #endregion

        #region "Event Handlers"

        /// <summary>
        /// Event handler for FIND button to load available request list from DMS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFind_Click(object sender, EventArgs e)
        {
            FindDmsRequests();
        }

        /// <summary>
        /// Event handler for MoveDown button to move requests from Available Requests to Requests To Run list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            MoveRequestsToRunList();
        }

        /// <summary>
        /// Event handler for MoveUp button to move requests from Requests To Run to Available Requests list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            RemoveRequestsFromRunList();
        }

        /// <summary>
        /// Event handler OK button to tell calling form that new DMS data is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Update cart assignments in DMS
            if (UpdateDMSCartAssignment())
            {
                // Hide the form if update was successful
                Hide();
                DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Event handler for selection of cart name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxSelectCart_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_CartName = comboBoxSelectCart.Text;
            labelLCCart.Text = "LC Cart: " + m_CartName;
        }

        /// <summary>
        /// Event handler for selection of cart config name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxSelectCartConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_CartConfigName = comboBoxSelectCartConfig.Text;
        }

        /// <summary>
        /// Event handler for change to "Unassigned only" checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUnAssignedOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnAssignedOnly.Checked)
            {
                comboBoxCarts.Enabled = false;
            }
            else
            {
                comboBoxCarts.Enabled = true;
            }
        }

        /// <summary>
        /// Event handler for Available Requests listview column click event (sorts by column)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listviewAvailableRequests_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortListview(sender as ListView, e);
        }

        /// <summary>
        /// Event handler for Requests To Run listview column click event (sorts by column)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewRequestsToRun_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortListview(sender as ListView, e);
        }

        /// <summary>
        /// Causes list of carts in combo boxes to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdateCartList_Click(object sender, EventArgs e)
        {
            UpdateCartList();
            UpdateCartConfigList();
        }

        /// <summary>
        ///  Handle when the user wants to close the window, but dont close it just allow it to hide
        ///  and the caller the ability to clean up the resources as they need.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formDMSView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public formDMSView()
        {
            InitializeComponent();

            // Initialize form controls
            InitFormControls();
        }

        /// <summary>
        /// Loads form controls with initial data, where applicable
        /// </summary>
        private void InitFormControls()
        {
            FormClosing += formDMSView_FormClosing;

            // Form caption
            var dbInUse = string.Empty;
            try
            {
                if (classDMSToolsManager.Instance.SelectedTool.DMSVersion.Contains("_T3"))
                {
                    dbInUse = " (Using Development Database)";
                }
                else
                {
                    dbInUse = " (Using Production Database)";
                }
            }
            catch (Exception)
            {
            }
            Text = "LcmsNet V" + Application.ProductVersion + dbInUse;

            //Listview information
            m_ListViewColumns.Add("listviewAvailableRequests", "0");
            m_ListViewColumns.Add("listViewRequestsToRun", "0");
            m_ListViewSortOrder.Add("listviewAvailableRequests", "true");
            m_ListViewSortOrder.Add("listViewRequestsToRun", "true");

            //Section removed because moving listviews to aplit display changed how Controls collection works
            //foreach (object tempControl in this.Controls)
            //{
            //   if (tempControl is System.Windows.Forms.ListView)
            //   {
            //      ListView tempListView = tempControl as ListView;
            //      m_ListViewColumns.Add(tempListView.Name,"0"); // Initializes all listviews to column 0
            //      m_ListViewSortOrder.Add(tempListView.Name, "true");   // Initializes all listviews to asc sort order
            //   }
            //}

            // Load the LC cart lists
            UpdateCartList();
            UpdateCartConfigList();

            // Cart name
            m_CartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
            m_CartConfigName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME);

            if (m_CartName.ToLower() == classLCMSSettings.CONST_UNASSIGNED_CART_NAME)
            {
                // No cart name is assigned, user will need to select one
            }
            else
            {
                labelLCCart.Text = "LC Cart: " + m_CartName;
            }
        }

        /// <summary>
        /// Loads the LC cart dropdowns with data from cache
        /// </summary>
        private void UpdateCartList()
        {
            List<string> cartList;

            comboBoxCarts.Items.Clear();
            comboBoxSelectCart.Items.Clear();

            // Get the list of carts from DMS
            try
            {
                cartList = classSQLiteTools.GetCartNameList();
            }
            catch (classDatabaseConnectionStringException ex)
            {
                // The SQLite connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart listing.\r\n" +
                    "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return;
            }
            catch (classDatabaseDataException ex)
            {
                // There was a problem getting the list of LC carts from the cache db
                var innerException = string.Empty;
                if (ex.InnerException != null)
                    innerException = ex.InnerException.Message;
                var errMsg = "Exception getting LC cart list from DMS: " + innerException + "\r\n" +
                    "As a workaround, you may manually type the cart name when needed.\r\n" +
                    "You may retry retrieving the cart list later, if desired.";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return;
            }

            if (cartList.Any())
            {
                foreach (var cart in cartList)
                {
                    comboBoxCarts.Items.Add(cart);
                    comboBoxSelectCart.Items.Add(cart);
                }
            }
        }

        /// <summary>
        /// Loads the LC cart config dropdown with data from cache
        /// </summary>
        private void UpdateCartConfigList()
        {
            List<string> cartConfigList;

            comboBoxSelectCartConfig.Items.Clear();

            // Get the list of cart configuration names from DMS
            try
            {
                cartConfigList = classSQLiteTools.GetCartConfigNameList(false);
            }
            catch (classDatabaseConnectionStringException ex)
            {
                // The SQLite connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart config name listing.\r\n" +
                    "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
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
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return;
            }

            if (cartConfigList.Any())
            {
                foreach (var cartConfig in cartConfigList)
                {
                    comboBoxSelectCartConfig.Items.Add(cartConfig);
                }
            }
        }
        /// <summary>
        /// Loads listViewAvailableRequests with all requests in DMS matching specified criteria
        /// </summary>
        private void FindDmsRequests()
        {
            List<classSampleData> tempRequestList;

            // Fill an object with the data from the UI, then pass to DMSTools class to run the query
            var queryData = new classSampleQueryData {
                RequestName = textRequestName.Text
            };


            // If min request number input is not specified, set it to 0
            if (string.IsNullOrEmpty(textRequestNumMin.Text))
            {
                queryData.MinRequestNum = "0";
            }
            else
            {
                queryData.MinRequestNum = textRequestNumMin.Text;
            }

            //If max request number input is not specified, set it to 1,000,000,000
            if (string.IsNullOrEmpty(textRequestNumMax.Text))
            {
                queryData.MaxRequestNum = "1000000000";
            }
            else
            {
                queryData.MaxRequestNum = textRequestNumMax.Text;
            }

            // If "unassigned only" is checked, override cart selection combo box
            //string cartToFind;
            if (checkBoxUnAssignedOnly.Checked)
            {
                queryData.Cart = "unknown";
            }
            else
            {
                queryData.Cart = comboBoxCarts.Text;
            }

            queryData.BatchID = textBatchID.Text;
            queryData.Block = textBlock.Text;
            queryData.Wellplate = textWellplate.Text;
            queryData.UnassignedOnly = checkBoxUnAssignedOnly.Checked;

            // Blank listview and display wait message
            labelPleaseWait.Visible = true;
            Refresh();

            // Clear the available datasets listview
            listviewAvailableRequests.Items.Clear();

            // Get a list of requests from DMS
            try
            {
                tempRequestList = classDMSToolsManager.Instance.SelectedTool.GetSamplesFromDMS(queryData);
            }
            catch (classDatabaseConnectionStringException ex)
            {
                // The DMS connection string wasn't found
                var errMsg = ex.Message + " while getting request listing\r\n";
                errMsg = errMsg + "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return;
            }
            catch (classDatabaseDataException ex)
            {
                var errMsg = ex.Message;
                if (ex.InnerException != null)
                    errMsg += ": " + ex.InnerException.Message;

                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return;
            }
            finally
            {
                labelPleaseWait.Visible = false;
            }

            // Check to see if any items were found
            labelPleaseWait.Visible = false;
            if (!tempRequestList.Any())
            {
                MessageBox.Show("No requests found in DMS");
                labelRequestCount.Text = "0 requests found";
                return;
            }
            labelRequestCount.Text = tempRequestList.Count + " requests found";

            // Add the requests to the listview
//              listviewAvailableRequests.BeginUpdate();
            if (m_DmsRequestList == null)
            {
                m_DmsRequestList = new List<classSampleData>();
            }

            var availReqList = new ArrayList();

            foreach (var request in tempRequestList)
            {
                // Determine if already in list of requests
                m_MatchString = request.DmsData.RequestName;
                var foundIndx = m_DmsRequestList.FindIndex(PredContainsRequestName);
                if (foundIndx == -1)
                {
                    // Request not found, so add to list of all requests
                    m_DmsRequestList.Add(request);
                    // Add request data to the listview
                    AddNewItemToRunListView(request, ref availReqList);
                }
                else
                {
                    if (m_DmsRequestList[foundIndx].DmsData.SelectedToRun)
                    {
                        // Request was found and is already in Requests To Run list, so do nothing
                    }
                    else
                    {
                        AddNewItemToRunListView(request, ref availReqList);
                    }
                }
            }

            var itemsToAdd = (ListViewItem[]) availReqList.ToArray(typeof (ListViewItem));
            listviewAvailableRequests.Items.AddRange(itemsToAdd);

            // Hide the wait message and display the listview again
//              listviewAvailableRequests.EndUpdate();
            labelPleaseWait.Visible = false;
//              classStatusTools.SendStatusMsg("Found " + listviewAvailableRequests.Items.Count.ToString() + " requests in DMS");

            // Check to see if any items have blocking enabled
            if (IsBlockingEnabled(tempRequestList))
            {
                var msg =
                    "You have downloaded samples that have blocking enabled. Please be sure you have downloaded the " +
                    "entire block of samples needed";
                MessageBox.Show(msg, "Blocked Samples Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Tests downloaded samples to determine if any have blocking enabled
        /// </summary>
        /// <param name="InpData">List containing downloaded samples</param>
        /// <returns>TRUE if any samples have blcoking enabled; otherwise FALSE</returns>
        private bool IsBlockingEnabled(List<classSampleData> InpData)
        {
            foreach (var testSample in InpData)
            {
                if (testSample.DmsData.Block > 0)
                {
                    // Blocking is enabled for this sample, no furhter test required
                    return true;
                }
            }
            // If we got to here, no samples have blocking enabled
            return false;
        }

        /// <summary>
        /// Moves items from the Available Requests list to the Requests To Run list
        /// </summary>
        private void MoveRequestsToRunList()
        {
            // Copy selected items and update main list of DMS items
            listviewAvailableRequests.BeginUpdate();
            listViewRequestsToRun.BeginUpdate();

            foreach (ListViewItem tempItem in listviewAvailableRequests.SelectedItems)
            {
                // Move the selected items
                var tempItemToMove = new ListViewItem(tempItem.Text);
                var numSubItems = tempItem.SubItems.Count;
                for (var cnt = 1; cnt < numSubItems; cnt++)
                {
                    tempItemToMove.SubItems.Add(tempItem.SubItems[cnt].Text);
                }
                listViewRequestsToRun.Items.Add(tempItemToMove);
                // Update main list of DMS items
                m_MatchString = tempItem.Text;
                var foundIndx = m_DmsRequestList.FindIndex(PredContainsRequestName);
                if (foundIndx != -1)
                {
                    m_DmsRequestList[foundIndx].DmsData.SelectedToRun = true;
                }
                else
                {
                    var tempStr = "Request " + tempItem.SubItems[2].Text + " not found in requested run collection";
                    MessageBox.Show(tempStr);
                    return;
                }
                // Remove the selected items from the Available Requests listview
                listviewAvailableRequests.Items.Remove(tempItem);
            }


            listviewAvailableRequests.EndUpdate();
            listViewRequestsToRun.EndUpdate();
        }

        /// <summary>
        /// Removes requests from the "Requests to run" list
        /// </summary>
        private void RemoveRequestsFromRunList()
        {
            foreach (ListViewItem tempItem in listViewRequestsToRun.SelectedItems)
            {
                // Update main list of DMS items
                m_MatchString = tempItem.Text;
                var foundIndx = m_DmsRequestList.FindIndex(PredContainsRequestName);
                if (foundIndx != -1)
                {
                    m_DmsRequestList.RemoveAt(foundIndx);
                }
                else
                {
                    var tempStr = "Request " + tempItem.SubItems[2].Text + " not found in requested run collection";
                    MessageBox.Show(tempStr);
                    return;
                }
                // Remove the selected items from the Requests To Run listview
                listViewRequestsToRun.Items.Remove(tempItem);
            }
        }

        /// <summary>
        /// Predicate function to find index of request matching specified request number
        /// Used for m_DmsRequestList FindIndex method
        /// </summary>
        /// <param name="request">classDMSData object passed in from FindIndex method</param>
        /// <returns>True if match is made; otherwise False</returns>
        private bool PredContainsRequestName(classSampleData request)
        {
            if (string.Equals(request.DmsData.RequestName, m_MatchString, StringComparison.CurrentCultureIgnoreCase))
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
            listviewAvailableRequests.Items.Clear();
            listViewRequestsToRun.Items.Clear();
            m_DmsRequestList.Clear();
        }

        /// <summary>
        /// Transfers the current list of classDMSData objects that have been selected
        /// for running to the calling program as a list of classSampleData objects
        /// </summary>
        /// <returns>List of classSampleData objects</returns>
        public List<classSampleData> GetNewSamplesDMSView()
        {
            var retList = new List<classSampleData>();

            foreach (ListViewItem tempItem in listViewRequestsToRun.Items)
            {
                // Find index of item in master request list
                m_MatchString = tempItem.Text;
                var foundIndx = m_DmsRequestList.FindIndex(PredContainsRequestName);
                if (foundIndx == -1)
                {
                    // Request has disappeared from master list (shouldn't ever happen, but.....)
                    var tempStr = "Request " + tempItem.Text + " not found in requested run collection";
                    MessageBox.Show(tempStr);
                    retList.Clear();
                    return retList;
                }
                var tempSampleData = (classSampleData) m_DmsRequestList[foundIndx].Clone();
                tempSampleData.DmsData.CartName = m_CartName;
                tempSampleData.DmsData.CartConfigName = m_CartConfigName;

                //                  classSampleData tempSampleData = CopyDMSDataObj(tempDMSData);
                retList.Add(tempSampleData);
            }
//              classStatusTools.SendStatusMsg("Adding " + retList.Count.ToString() + " samples from DMS");
            return retList;
        }

        /// <summary>
        /// Updates selected requests in DMS to show new cart assignment
        /// </summary>
        /// <returns></returns>
        bool UpdateDMSCartAssignment()
        {
            // Verify a cart is specified
            if (m_CartName.ToLower() == classLCMSSettings.CONST_UNASSIGNED_CART_NAME)
            {
                MessageBox.Show("Cart name must be specified", "CART NAME NOT SPECIFIED");
                return false;
            }

            // Update the cart assignments in DMS
            var reqIDs = "";

            // Build a string of request IDs for stored procedure call
            foreach (var tempDMSData in m_DmsRequestList)
            {
                if (tempDMSData.DmsData.SelectedToRun)
                {
                    if (reqIDs.Length != 0)
                    {
                        reqIDs = reqIDs + "," + tempDMSData.DmsData.RequestID;
                    }
                    else
                    {
                        reqIDs = tempDMSData.DmsData.RequestID.ToString();
                    }
                }
            }
            // Call the DMS stored procedure to update the cart assignments
            bool success;
            try
            {
                success = classDMSToolsManager.Instance.SelectedTool.UpdateDMSCartAssignment(reqIDs, m_CartName, m_CartConfigName, true);
            }
            catch (classDatabaseConnectionStringException ex)
            {
                // The DMS connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart listing\r\n";
                errMsg = errMsg + "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return false;
            }
            catch (classDatabaseDataException ex)
            {
                var errMsg = ex.Message;
                if (ex.InnerException != null)
                    errMsg += ": " + ex.InnerException.Message;

                errMsg = errMsg + "\r\n\r\n Requests in DMS may not show correct cart assignments";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return true;
            }
            catch (classDatabaseStoredProcException ex)
            {
                var errMsg = "Error " + ex.ReturnCode + " while executing stored procedure ";
                errMsg = errMsg + ex.ProcName + ": " + ex.ErrMessage;
                errMsg = errMsg + "\r\n\r\nRequests in DMS may not show correct cart assignments";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButtons.OK);
                return true;
            }

            if (!success)
            {
                MessageBox.Show(classDMSToolsManager.Instance.SelectedTool.ErrMsg, "LcmsNet",
                    MessageBoxButtons.AbortRetryIgnore);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a new line to the the RequestsToRun listview (overload for adding directly to listview)
        /// </summary>
        /// <param name="requestData">classDMSData object containing request data to add</param>
        private void AddNewItemToRunListView(classSampleData requestData)
        {
            // Add to listview
            var newItem = new ListViewItem(requestData.DmsData.RequestName);
            newItem.SubItems.Add(requestData.DmsData.RequestID.ToString());
            newItem.SubItems.Add(requestData.DmsData.CartName);
            newItem.SubItems.Add(requestData.DmsData.UserList);
            newItem.SubItems.Add(requestData.DmsData.UsageType);
            newItem.SubItems.Add(requestData.DmsData.Batch.ToString());
            newItem.SubItems.Add(requestData.DmsData.Block.ToString());
            newItem.SubItems.Add(requestData.DmsData.RunOrder.ToString());
            // Set color if cart is assigned
            if (requestData.DmsData.CartName != "unknown")
            {
                newItem.BackColor = Color.Yellow;
            }
            listviewAvailableRequests.Items.Add(newItem);
        }

        /// <summary>
        /// Adds a new line to the the RequestsToRun listview (overload for using ArrayList
        /// </summary>
        /// <param name="requestData">classDMSData object containing request data to add</param>
        /// <param name="lvItemList">ArrayList containing listview items to add</param>
        private void AddNewItemToRunListView(classSampleData requestData, ref ArrayList lvItemList)
        {
            var newItem = new ListViewItem(requestData.DmsData.RequestName);
            newItem.SubItems.Add(requestData.DmsData.RequestID.ToString());
            newItem.SubItems.Add(requestData.DmsData.CartName);
            newItem.SubItems.Add(requestData.DmsData.UserList);
            newItem.SubItems.Add(requestData.DmsData.UsageType);
            newItem.SubItems.Add(requestData.DmsData.Batch.ToString());
            newItem.SubItems.Add(requestData.DmsData.Block.ToString());
            newItem.SubItems.Add(requestData.DmsData.RunOrder.ToString());
            // Set color if cart is assigned
            if (requestData.DmsData.CartName != "unknown")
            {
                newItem.BackColor = Color.Yellow;
            }
            lvItemList.Add(newItem);
        }

        /// <summary>
        /// Sorts selected column in specified listview
        /// </summary>
        /// <param name="selectedView">ListView containing column to sort   </param>
        /// <param name="e">Arguments for listview column click event</param>
        private void SortListview(ListView selectedView, ColumnClickEventArgs e)
        {
            var selectedColumn = int.Parse(m_ListViewColumns[selectedView.Name]);
            var sortOrderAscending = bool.Parse(m_ListViewSortOrder[selectedView.Name]);

            // selected column is same as previously selected column, then reverse sort order.
            // Otherwise, sort newly selected column in ascending order
            if (e.Column == selectedColumn)
            {
                sortOrderAscending = !sortOrderAscending;
            }
            else
            {
                sortOrderAscending = true;
                selectedColumn = e.Column;
            }

            // Update the stored column index and sort order for this ListView
            m_ListViewColumns[selectedView.Name] = selectedColumn.ToString();
            m_ListViewSortOrder[selectedView.Name] = sortOrderAscending.ToString();

            // Perform sort. For DMS View listviews, columns 1, 6, and 7 are numeric, all other columns are string
            if ((selectedColumn == 1) || (selectedColumn == 6) || (selectedColumn == 7))
            {
                // Use numeric sort
                selectedView.ListViewItemSorter = new classListViewItemComparer(e.Column, sortOrderAscending,
                    enumListViewComparerMode.SortModeConstants.numeric);
            }
            else
            {
                // Use string sort
                selectedView.ListViewItemSorter = new classListViewItemComparer(e.Column, sortOrderAscending,
                    enumListViewComparerMode.SortModeConstants.text);
            }
        }

        #endregion
        
    }
}