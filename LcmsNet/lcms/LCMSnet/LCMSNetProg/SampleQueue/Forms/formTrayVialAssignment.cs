//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/27/2010
//
// Last modified 07/27/2010
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class formTrayVialAssignment : Form
    {
        #region "Constructors"

        public formTrayVialAssignment()
        {
            InitializeComponent();

            InitForm();
        }

        #endregion

        #region "Properties"

        public List<classSampleData> SampleList { get; private set; }

        #endregion

        //*********************************************************************************************************
        // Form for making tray/vial assignments to samples
        //**********************************************************************************************************

        #region "Constants"

        const int TRAY_CONTROL_TOP = 10;
        const int TRAY_CONTROL_LEFT = 17;

        #endregion

        #region "Class variables"

        List<string> m_TrayNames; // List of tray names used by PAL on this cart

        DataTable m_DataList = new DataTable("SampleTable");
        // Table to hold data for samples in easy-to-handle format

        DataView m_DataView = new DataView();
        // Controls whether main form will show all samples or unassigned only

        controlTray[] m_Trays;
        // User control to be added to each tray tab; array is used to simplify later processing

        #endregion

        #region "Delegates"

        #endregion

        #region "Events"

        #endregion

        #region "Methods"

        /// <summary>
        /// Loads the samples into the form
        /// </summary>
        /// <param name="trayNames">List of PAL tray names</param>
        /// <param name="samples">List of samples</param>
        public void LoadSampleList(List<string> trayNames, List<classSampleData> samples)
        {
            m_TrayNames = trayNames;
            SampleList = samples;
            m_DataList.Clear();

            // Load sample information into data table
            foreach (var sample in SampleList)
            {
                AddSampleToDataTable(sample);
            }

            // set up the overall data view
            m_Trays[0].UpdateSampleList(ref m_DataList);
            SetDataView();

            // Update individual tray displays
            var captionStr = "";
            for (var trayIndx = 1; trayIndx < m_Trays.Length; trayIndx++)
            {
                m_Trays[trayIndx].UpdateSampleList(ref m_DataList);
                captionStr = "Tray " + (trayIndx).ToString() + " (" + m_Trays[trayIndx].SampleCount.ToString() +
                             ")";
                tabControlPlates.TabPages[trayIndx].Text = captionStr;
            }
        }

        /// <summary>
        /// Initializes the user controls and tabs on the form
        /// </summary>
        private void InitForm()
        {
            // Create tray display objects and assign them to tabs for each tray
            m_Trays = new controlTray[7];
            for (var trayIndx = 0; trayIndx < m_Trays.Length; trayIndx++)
            {
                m_Trays[trayIndx] = new controlTray();
                m_Trays[trayIndx].TrayNumber = trayIndx;
                m_Trays[trayIndx].Top = TRAY_CONTROL_TOP;
                m_Trays[trayIndx].Left = TRAY_CONTROL_LEFT;
                m_Trays[trayIndx].RowModified += UpdateTabDisplays;
                tabControlPlates.TabPages[trayIndx].Controls.Add(m_Trays[trayIndx]);
                m_Trays[trayIndx].Clear();
            }

            m_Trays[0].MasterView = true;

            // Create the data table to hold all of the sample data. We are using a data table because doing tray/vial assignments
            //      directly in the incoming sample list makes cancelling awkwars. Also, a data table is easier to display
            InitDataTable();
        }

        /// <summary>
        /// Gets the index into the trayNames collection for the specified input PAL tray name
        /// </summary>
        /// <param name="trayName">Name of PAL tray to search for</param>
        /// <returns>Index value if found; otherwise -1</returns>
        private int GetTrayIndexFromTrayName(string trayName)
        {
            // If trayName isn't contained in tray list, return -1
            if (!m_TrayNames.Contains(trayName)) return -1;

            // Iterate through tray names until matching name is found
            var retIndx = -1;
            for (var indx = 0; indx < m_TrayNames.Count; indx++)
            {
                if (trayName == m_TrayNames[indx])
                {
                    retIndx = indx;
                    break;
                }
            }
            return retIndx;
        }

        /// <summary>
        /// Initializes the data table that will hold all the sample information
        /// </summary>
        private void InitDataTable()
        {
            var typeInt = Type.GetType("System.Int32");
            var typeLong = Type.GetType("System.Int64");
            var typeString = Type.GetType("System.String");

            // Add the columns
            var columnArray = new DataColumn[9];
            columnArray[0] = CreateDataColumn("SampleName", typeString, "Sample Name", true);
            columnArray[1] = CreateDataColumn("SeqNum", typeLong, "Seq #", true);
            columnArray[2] = CreateDataColumn("ColNum", typeInt, "Col #", true);
            columnArray[3] = CreateDataColumn("Tray", typeInt, "Tray", false);
            columnArray[4] = CreateDataColumn("Vial", typeInt, "Vial", false);
            columnArray[5] = CreateDataColumn("Batch", typeInt, "Batch", true);
            columnArray[6] = CreateDataColumn("Block", typeInt, "Block", true);
            columnArray[7] = CreateDataColumn("RunOrder", typeInt, "Run Order (DMS)", true);
            columnArray[8] = CreateDataColumn("UniqueID", typeInt, "Unique ID", true);
            m_DataList.Columns.AddRange(columnArray);
        }

        /// <summary>
        /// Creates a data column
        /// </summary>
        /// <param name="colName">Name of column in table</param>
        /// <param name="colType">Type of column</param>
        /// <param name="caption">Caption to display in list view</param>
        /// <param name="makeReadOnly">TRUE to make column read-only</param>
        /// <returns>Column object</returns>
        private DataColumn CreateDataColumn(string colName, Type colType, string caption, bool makeReadOnly)
        {
            var newColumn = new DataColumn(colName, colType);
            newColumn.Caption = caption;
            newColumn.ReadOnly = makeReadOnly;
            return newColumn;
        }

        /// <summary>
        /// Adds data from a classSampleData object to the data table used for tray/vial assigments
        /// </summary>
        /// <param name="sample">Sample object</param>
        private void AddSampleToDataTable(classSampleData sample)
        {
            var newRow = m_DataList.NewRow();

            newRow["SampleName"] = sample.DmsData.DatasetName;
            newRow["SeqNum"] = sample.SequenceID;
            newRow["ColNum"] = sample.ColumnData.ID + 1; // Columns internally stored as 0-3
            newRow["Tray"] = GetTrayIndexFromTrayName(sample.PAL.PALTray) + 1;
            newRow["Vial"] = sample.PAL.Well;
            newRow["Batch"] = sample.DmsData.Batch;
            newRow["Block"] = sample.DmsData.Block;
            newRow["RunOrder"] = sample.DmsData.RunOrder;
            newRow["UniqueID"] = sample.UniqueID;

            m_DataList.Rows.Add(newRow);
        }

        /// <summary>
        /// Sets the data grid to display all samples or unassigned only
        /// </summary>
        private void SetDataView()
        {
            if (radbtnUnassigned.Checked)
            {
                m_Trays[0].SetDataView(true);
                tabControlPlates.TabPages[0].Text = "Unassigned (" + m_Trays[0].SampleCount.ToString() + ")";
            }
            else
            {
                m_Trays[0].SetDataView(false);
                tabControlPlates.TabPages[0].Text = "All (" + m_Trays[0].SampleCount.ToString() + ")";
            }
        }

        /// <summary>
        /// Updates the item counts on the tabs for each tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="trayNumber"></param>
        private void UpdateTabDisplays(object sender, int trayNumber)
        {
            string tmpStr;

            // Update main tab
            SetDataView();

            // Update individual tray tabs
            for (var indx = 1; indx < m_Trays.Length; indx++)
            {
                tmpStr = "Tray " + indx.ToString() + " (" + m_Trays[indx].SampleCount.ToString() + ")";
                tabControlPlates.TabPages[indx].Text = tmpStr;
            }
        }

        /// <summary>
        /// Gets the index of the sample in m_SampleList with specified unique ID
        /// </summary>
        /// <param name="sampleId">ID to search for</param>
        /// <returns>Index of matching sampe if found; -1 otherwise</returns>
        private int GetSampleIndex(int sampleId)
        {
            var foundIndx = -1;
            for (var indx = 0; indx < SampleList.Count; indx++)
            {
                if (SampleList[indx].UniqueID == sampleId)
                {
                    foundIndx = indx;
                    break;
                }
            }

            return foundIndx;
        }

        /// <summary>
        /// Updates the sample list tray/vial data from the data table
        /// </summary>
        private void UpdateSampleList()
        {
            var sampleIndx = -1;
            foreach (DataRow currRow in m_DataList.Rows)
            {
                // Get the index of the sample in m_SampleList that matches the current table row
                sampleIndx = GetSampleIndex((int) currRow["UniqueID"]);
                if (sampleIndx == -1)
                {
                    //TODO: Shouldn't happen. Must figure out how to let operator know if it does
                    continue;
                }

                SampleList[sampleIndx].PAL.PALTray = GetTrayNameFromNumber((int) currRow["Tray"]);
                SampleList[sampleIndx].PAL.Well = (int) currRow["Vial"];
            }
        }

        /// <summary>
        /// Gets the name of the tray corresponding to the tray number
        /// </summary>
        /// <param name="trayNumber">Number of the tray (0-6)</param>
        /// <returns>If non-zero, tray name; Otherwise empty string</returns>
        private string GetTrayNameFromNumber(int trayNumber)
        {
            if (trayNumber == 0) return string.Empty;

            return m_TrayNames[trayNumber - 1];
        }

        #endregion

        #region "Event handlers"

        /// <summary>
        /// Sets main grid to display unassigned samples only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radbtnUnassigned_Click(object sender, EventArgs e)
        {
            SetDataView();
        }

        /// <summary>
        /// Sets main grid to display all samples
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radbtnAll_Click(object sender, EventArgs e)
        {
            SetDataView();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            UpdateSampleList();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Cleans up when closing form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formTrayVialAssignment_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
            }
        }

        #endregion
    }
} // End namespace