//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/27/2010
//
// Last modified 07/27/2010
//*********************************************************************************************************

using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class controlTray : UserControl
    {
        #region "Constructors"

        public controlTray()
        {
            InitializeComponent();

            InitControl();
        }

        #endregion

        #region "Events"

        public event DelegateRowModified RowModified;

        #endregion

        private void mbutton_assignToVial_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count < 1) return;

            //TODO: May want to add a warning and allow specifying a start vial number? Possibly allow selection of sample subset?
            int vialCounter = Convert.ToInt32(mnum_specificVial.Value);

            foreach (DataGridViewRow currRow in dataGridView1.Rows)
            {
                currRow.Cells[4].Value = vialCounter;
            }
        }

        //*********************************************************************************************************
        // User control for tray/vial assignment form
        //**********************************************************************************************************

        #region "Constants"

        #endregion

        #region "Class variables"

        int mint_TrayNumber = 0;
        DataTable mobject_DataTable = new DataTable("DummyTable");
        DataView mobject_DataView = new DataView();
        bool mbool_MasterView = false;
        bool mbool_ShowUnassigned = true;

        #endregion

        #region "Delegates"

        #endregion

        #region "Properties"

        public int TrayNumber
        {
            get { return mint_TrayNumber; }
            set { mint_TrayNumber = value; }
        }

        public int SampleCount
        {
            get { return dataGridView1.Rows.Count; }
        }

        public bool MasterView
        {
            get { return mbool_MasterView; }
            set { mbool_MasterView = value; }
        }

        #endregion

        #region "Methods"

        public void Clear()
        {
            dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// Sets up the display grid columns
        /// </summary>
        private void InitControl()
        {
            dataGridView1.AutoGenerateColumns = false;

            // Define columns
            DataGridViewTextBoxColumn dsNameColumn = new DataGridViewTextBoxColumn();
            dsNameColumn.DataPropertyName = "SampleName";
            dsNameColumn.HeaderText = "Sample";
            dsNameColumn.Width = 290;
            dsNameColumn.ReadOnly = true;

            DataGridViewTextBoxColumn seqNumColumn = new DataGridViewTextBoxColumn();
            seqNumColumn.DataPropertyName = "SeqNum";
            seqNumColumn.HeaderText = "Seq #";
            seqNumColumn.Width = 64;
            seqNumColumn.ReadOnly = true;

            DataGridViewTextBoxColumn colNumColumn = new DataGridViewTextBoxColumn();
            colNumColumn.DataPropertyName = "ColNum";
            colNumColumn.HeaderText = "Column #";
            colNumColumn.Width = 60;
            colNumColumn.ReadOnly = true;

            DataGridViewTextBoxColumn trayColumn = new DataGridViewTextBoxColumn();
            trayColumn.DataPropertyName = "Tray";
            trayColumn.HeaderText = "Tray";
            trayColumn.Width = 50;
            trayColumn.ReadOnly = true;

            DataGridViewTextBoxColumn vialColumn = new DataGridViewTextBoxColumn();
            vialColumn.DataPropertyName = "Vial";
            vialColumn.HeaderText = "Vial";
            vialColumn.Width = 50;
            vialColumn.ReadOnly = false;

            DataGridViewTextBoxColumn batchColumn = new DataGridViewTextBoxColumn();
            batchColumn.DataPropertyName = "Batch";
            batchColumn.HeaderText = "Batch";
            batchColumn.Width = 50;
            batchColumn.ReadOnly = true;

            DataGridViewTextBoxColumn blockColumn = new DataGridViewTextBoxColumn();
            blockColumn.DataPropertyName = "Block";
            blockColumn.HeaderText = "Block";
            blockColumn.Width = 50;
            blockColumn.ReadOnly = true;

            DataGridViewTextBoxColumn runOrderColumn = new DataGridViewTextBoxColumn();
            runOrderColumn.DataPropertyName = "RunOrder";
            runOrderColumn.HeaderText = "Run Order (DMS)";
            runOrderColumn.Width = 86;
            runOrderColumn.ReadOnly = true;

            // Add columns to data view control
            dataGridView1.Columns.Add(dsNameColumn);
            dataGridView1.Columns.Add(seqNumColumn);
            dataGridView1.Columns.Add(colNumColumn);
            dataGridView1.Columns.Add(trayColumn);
            dataGridView1.Columns.Add(vialColumn);
            dataGridView1.Columns.Add(batchColumn);
            dataGridView1.Columns.Add(blockColumn);
            dataGridView1.Columns.Add(runOrderColumn);

            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        /// <summary>
        /// Displays the sample list for this tray
        /// </summary>
        /// <param name="samples">List of samples to display and edit</param>
        public void UpdateSampleList(ref DataTable samples)
        {
            mobject_DataTable = samples;

            mobject_DataView.Table = mobject_DataTable;
            dataGridView1.DataSource = mobject_DataView;
            if (!mbool_MasterView)
            {
                mobject_DataView.RowFilter = "Tray = " + mint_TrayNumber.ToString();
            }
            else SetDataView();
        }

        /// <summary>
        /// Sets the data view depending on the row filter
        /// </summary>
        private void SetDataView()
        {
            if (mbool_MasterView)
            {
                if (mbool_ShowUnassigned)
                {
                    mobject_DataView.RowFilter = "Tray = 0";
                }
                else mobject_DataView.RowFilter = "";
            }
            else mobject_DataView.RowFilter = "Tray = " + mint_TrayNumber.ToString();
        }

        /// <summary>
        /// Overload allowing external call to set view
        /// </summary>
        /// <param name="unassigned">TRUE for showing unallsigned only; FALSE otherwise</param>
        public void SetDataView(bool unassignedOnly)
        {
            mbool_ShowUnassigned = unassignedOnly;
            SetDataView();
        }

        /// <summary>
        /// Updates tray assignments for selected samples
        /// </summary>
        /// <param name="newTrayNum">New tray number</param>
        private void UpdateTrayAssignment(int newTrayNum)
        {
            if (dataGridView1.Rows.Count < 1) return;

            if (dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("At least one sample must be selected");
                return;
            }

            foreach (DataGridViewRow currRow in dataGridView1.SelectedRows)
            {
                try
                {
                    currRow.Cells[3].Value = newTrayNum;
                    dataGridView1.NotifyCurrentCellDirty(true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            // Crude but effective way to update the displays
            dataGridView1.DataSource = null;
//              mobject_DataView.RowStateFilter = DataViewRowState.Added | DataViewRowState.Unchanged;
            SetDataView();
            dataGridView1.DataSource = mobject_DataView;
            if (RowModified != null) RowModified(this, mint_TrayNumber);
        }

        // Debug code
        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Cell Value Changed: Row " + e.RowIndex.ToString() + ", Column " +
                                               e.ColumnIndex.ToString());
        }

        #endregion

        #region "Event handlers"

        /// <summary>
        /// Automatically numbers all vials in the list for this tray, starting at 1. It's assumed there will never be
        /// more than 96 samples in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAutoAssignVials_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count < 1) return;

            //TODO: May want to add a warning and allow specifying a start vial number? Possibly allow selection of sample subset?
            int vialCounter = 0;

            int mod = Convert.ToInt32(mnum_maxVials.Value);
            foreach (DataGridViewRow currRow in dataGridView1.Rows)
            {
                currRow.Cells[4].Value = (vialCounter % mod) + 1;
                vialCounter++;
            }
        }

        private void buttonMoveToTray1_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(1);
        }

        private void buttonMoveToTray2_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(2);
        }

        private void buttonMoveToTray3_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(3);
        }

        private void buttonMoveToTray4_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(4);
        }

        private void buttonMoveToTray5_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(5);
        }

        private void buttonMoveToTray6_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(6);
        }

        private void buttonUnassignTray_Click(object sender, EventArgs e)
        {
            UpdateTrayAssignment(0);
        }

        #endregion
    }
} // End namespace