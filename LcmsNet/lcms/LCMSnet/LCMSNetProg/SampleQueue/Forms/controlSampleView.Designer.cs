using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    partial class controlSampleView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mcontextMenu_options = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addBlankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBlankIntoUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromDMSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addDateCartNameColumnIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pALTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pALVialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lCMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instrumentMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datasetTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewThroughputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.randomizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_sampleContainer = new System.Windows.Forms.Panel();
            this.mdataGrid_samples = new LcmsNet.SampleQueue.Forms.classDataGrid();
            this.sampleToRowTranslatorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mcolumn_checkbox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_sequenceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_columnNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_uniqueID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_blockNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_runOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_requestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_PalTray = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mcolumn_palVial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_PALVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcolumn_LCMethod = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mcolumn_instrumentMethod = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mcolumn_datasetType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mcolumn_batchID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sampleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sequenceNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.specialColumnNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uniqueIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkboxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkboxTagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusToolTipTextDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.blockNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.runOrderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requestNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pALTrayDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pALVialDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pALVolumeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lCMethodDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.instrumentMethodDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.datasetTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.batchIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mcontextMenu_options.SuspendLayout();
            this.m_sampleContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mdataGrid_samples)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleToRowTranslatorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mcontextMenu_options
            // 
            this.mcontextMenu_options.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mcontextMenu_options.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator2,
            this.addBlankToolStripMenuItem,
            this.insertBlankIntoUnusedToolStripMenuItem,
            this.importFromDMSToolStripMenuItem,
            this.toolStripSeparator3,
            this.addDateCartNameColumnIDToolStripMenuItem,
            this.toolStripSeparator5,
            this.deleteSelectedToolStripMenuItem,
            this.deleteUnusedToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator1,
            this.showToolStripMenuItem,
            this.previewThroughputToolStripMenuItem,
            this.toolStripSeparator4,
            this.randomizeToolStripMenuItem});
            this.mcontextMenu_options.Name = "mcontextMenu_options";
            this.mcontextMenu_options.Size = new System.Drawing.Size(304, 346);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.undo_16;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(300, 6);
            // 
            // addBlankToolStripMenuItem
            // 
            this.addBlankToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.add;
            this.addBlankToolStripMenuItem.Name = "addBlankToolStripMenuItem";
            this.addBlankToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.addBlankToolStripMenuItem.Text = "Add Blank";
            this.addBlankToolStripMenuItem.Click += new System.EventHandler(this.addBlankToolStripMenuItem_Click);
            // 
            // insertBlankIntoUnusedToolStripMenuItem
            // 
            this.insertBlankIntoUnusedToolStripMenuItem.Name = "insertBlankIntoUnusedToolStripMenuItem";
            this.insertBlankIntoUnusedToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.insertBlankIntoUnusedToolStripMenuItem.Text = "Insert Blank Into Unused";
            this.insertBlankIntoUnusedToolStripMenuItem.Click += new System.EventHandler(this.insertBlankIntoUnusedToolStripMenuItem_Click);
            // 
            // importFromDMSToolStripMenuItem
            // 
            this.importFromDMSToolStripMenuItem.Name = "importFromDMSToolStripMenuItem";
            this.importFromDMSToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.importFromDMSToolStripMenuItem.Text = "Import from DMS";
            this.importFromDMSToolStripMenuItem.Click += new System.EventHandler(this.importFromDMSToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(300, 6);
            // 
            // addDateCartNameColumnIDToolStripMenuItem
            // 
            this.addDateCartNameColumnIDToolStripMenuItem.Name = "addDateCartNameColumnIDToolStripMenuItem";
            this.addDateCartNameColumnIDToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.addDateCartNameColumnIDToolStripMenuItem.Text = "Add Date, Cart Name, Column ID";
            this.addDateCartNameColumnIDToolStripMenuItem.Click += new System.EventHandler(this.addDateCartNameColumnIDToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(300, 6);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.deleteSelectedToolStripMenuItem.Text = "Delete Selected";
            this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
            // 
            // deleteUnusedToolStripMenuItem
            // 
            this.deleteUnusedToolStripMenuItem.Name = "deleteUnusedToolStripMenuItem";
            this.deleteUnusedToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.deleteUnusedToolStripMenuItem.Text = "Delete Unused";
            this.deleteUnusedToolStripMenuItem.Click += new System.EventHandler(this.deleteUnusedToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(300, 6);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pALTrayToolStripMenuItem,
            this.pALVialToolStripMenuItem,
            this.volumeToolStripMenuItem,
            this.lCMethodToolStripMenuItem,
            this.instrumentMethodToolStripMenuItem,
            this.datasetTypeToolStripMenuItem,
            this.batchIDToolStripMenuItem,
            this.blockToolStripMenuItem,
            this.runOrderToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // pALTrayToolStripMenuItem
            // 
            this.pALTrayToolStripMenuItem.Name = "pALTrayToolStripMenuItem";
            this.pALTrayToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.pALTrayToolStripMenuItem.Text = "PAL Tray";
            this.pALTrayToolStripMenuItem.Click += new System.EventHandler(this.pALTrayToolStripMenuItem_Click);
            // 
            // pALVialToolStripMenuItem
            // 
            this.pALVialToolStripMenuItem.Name = "pALVialToolStripMenuItem";
            this.pALVialToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.pALVialToolStripMenuItem.Text = "PAL Vial";
            this.pALVialToolStripMenuItem.Click += new System.EventHandler(this.pALVialToolStripMenuItem_Click);
            // 
            // volumeToolStripMenuItem
            // 
            this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            this.volumeToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.volumeToolStripMenuItem.Text = "Volume";
            this.volumeToolStripMenuItem.Click += new System.EventHandler(this.volumeToolStripMenuItem_Click);
            // 
            // lCMethodToolStripMenuItem
            // 
            this.lCMethodToolStripMenuItem.Name = "lCMethodToolStripMenuItem";
            this.lCMethodToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.lCMethodToolStripMenuItem.Text = "LC Method";
            this.lCMethodToolStripMenuItem.Click += new System.EventHandler(this.lCMethodToolStripMenuItem_Click);
            // 
            // instrumentMethodToolStripMenuItem
            // 
            this.instrumentMethodToolStripMenuItem.Name = "instrumentMethodToolStripMenuItem";
            this.instrumentMethodToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.instrumentMethodToolStripMenuItem.Text = "Instrument Method";
            this.instrumentMethodToolStripMenuItem.Click += new System.EventHandler(this.instrumentMethodToolStripMenuItem_Click);
            // 
            // datasetTypeToolStripMenuItem
            // 
            this.datasetTypeToolStripMenuItem.Name = "datasetTypeToolStripMenuItem";
            this.datasetTypeToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.datasetTypeToolStripMenuItem.Text = "Dataset Type";
            this.datasetTypeToolStripMenuItem.Click += new System.EventHandler(this.datasetTypeToolStripMenuItem_Click);
            // 
            // batchIDToolStripMenuItem
            // 
            this.batchIDToolStripMenuItem.Name = "batchIDToolStripMenuItem";
            this.batchIDToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.batchIDToolStripMenuItem.Text = "Batch ID";
            this.batchIDToolStripMenuItem.Click += new System.EventHandler(this.batchIDToolStripMenuItem_Click);
            // 
            // blockToolStripMenuItem
            // 
            this.blockToolStripMenuItem.Name = "blockToolStripMenuItem";
            this.blockToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.blockToolStripMenuItem.Text = "Block";
            this.blockToolStripMenuItem.Click += new System.EventHandler(this.blockToolStripMenuItem_Click);
            // 
            // runOrderToolStripMenuItem
            // 
            this.runOrderToolStripMenuItem.Name = "runOrderToolStripMenuItem";
            this.runOrderToolStripMenuItem.Size = new System.Drawing.Size(210, 26);
            this.runOrderToolStripMenuItem.Text = "Run Order";
            this.runOrderToolStripMenuItem.Click += new System.EventHandler(this.runOrderToolStripMenuItem_Click);
            // 
            // previewThroughputToolStripMenuItem
            // 
            this.previewThroughputToolStripMenuItem.Name = "previewThroughputToolStripMenuItem";
            this.previewThroughputToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.previewThroughputToolStripMenuItem.Text = "Preview Throughput";
            this.previewThroughputToolStripMenuItem.Click += new System.EventHandler(this.previewThroughputToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(300, 6);
            // 
            // randomizeToolStripMenuItem
            // 
            this.randomizeToolStripMenuItem.Name = "randomizeToolStripMenuItem";
            this.randomizeToolStripMenuItem.Size = new System.Drawing.Size(303, 26);
            this.randomizeToolStripMenuItem.Text = "Randomize";
            this.randomizeToolStripMenuItem.Click += new System.EventHandler(this.randomizeToolStripMenuItem_Click);
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.HeaderText = "PAL Method";
            this.dataGridViewComboBoxColumn1.MaxDropDownItems = 100;
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Width = 73;
            // 
            // dataGridViewComboBoxColumn2
            // 
            this.dataGridViewComboBoxColumn2.HeaderText = "PAL Tray";
            this.dataGridViewComboBoxColumn2.MaxDropDownItems = 100;
            this.dataGridViewComboBoxColumn2.Name = "dataGridViewComboBoxColumn2";
            this.dataGridViewComboBoxColumn2.Width = 72;
            // 
            // dataGridViewComboBoxColumn3
            // 
            this.dataGridViewComboBoxColumn3.HeaderText = "LC Method";
            this.dataGridViewComboBoxColumn3.MaxDropDownItems = 100;
            this.dataGridViewComboBoxColumn3.Name = "dataGridViewComboBoxColumn3";
            this.dataGridViewComboBoxColumn3.Width = 72;
            // 
            // dataGridViewComboBoxColumn4
            // 
            this.dataGridViewComboBoxColumn4.HeaderText = "Instrument Method";
            this.dataGridViewComboBoxColumn4.MaxDropDownItems = 100;
            this.dataGridViewComboBoxColumn4.Name = "dataGridViewComboBoxColumn4";
            this.dataGridViewComboBoxColumn4.Width = 73;
            // 
            // dataGridViewComboBoxColumn5
            // 
            this.dataGridViewComboBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridViewComboBoxColumn5.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewComboBoxColumn5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dataGridViewComboBoxColumn5.HeaderText = "Dataset Type";
            this.dataGridViewComboBoxColumn5.Items.AddRange(new object[] {
            "Test"});
            this.dataGridViewComboBoxColumn5.MaxDropDownItems = 100;
            this.dataGridViewComboBoxColumn5.Name = "dataGridViewComboBoxColumn5";
            this.dataGridViewComboBoxColumn5.Sorted = true;
            this.dataGridViewComboBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // m_sampleContainer
            // 
            this.m_sampleContainer.AutoSize = true;
            this.m_sampleContainer.Controls.Add(this.mdataGrid_samples);
            this.m_sampleContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_sampleContainer.Location = new System.Drawing.Point(4, 4);
            this.m_sampleContainer.Margin = new System.Windows.Forms.Padding(4);
            this.m_sampleContainer.Name = "m_sampleContainer";
            this.m_sampleContainer.Size = new System.Drawing.Size(1235, 806);
            this.m_sampleContainer.TabIndex = 19;
            // 
            // mdataGrid_samples
            // 
            this.mdataGrid_samples.AllowDrop = true;
            this.mdataGrid_samples.AllowUserToAddRows = false;
            this.mdataGrid_samples.AllowUserToDeleteRows = false;
            this.mdataGrid_samples.AllowUserToOrderColumns = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mdataGrid_samples.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.mdataGrid_samples.AutoGenerateColumns = false;
            this.mdataGrid_samples.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.mdataGrid_samples.BackgroundColor = System.Drawing.Color.White;
            this.mdataGrid_samples.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mdataGrid_samples.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mcolumn_checkbox,
            this.Status,
            this.mcolumn_sequenceNumber,
            this.mcolumn_columnNumber,
            this.mcolumn_uniqueID,
            this.mcolumn_blockNumber,
            this.mcolumn_runOrder,
            this.mcolumn_requestName,
            this.mcolumn_PalTray,
            this.mcolumn_palVial,
            this.mcolumn_PALVolume,
            this.mcolumn_LCMethod,
            this.mcolumn_instrumentMethod,
            this.mcolumn_datasetType,
            this.mcolumn_batchID,
            this.sampleDataGridViewTextBoxColumn,
            this.sequenceNumberDataGridViewTextBoxColumn,
            this.columnNumberDataGridViewTextBoxColumn,
            this.specialColumnNumberDataGridViewTextBoxColumn,
            this.uniqueIDDataGridViewTextBoxColumn,
            this.checkboxDataGridViewTextBoxColumn,
            this.checkboxTagDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.statusToolTipTextDataGridViewTextBoxColumn,
            this.blockNumberDataGridViewTextBoxColumn,
            this.runOrderDataGridViewTextBoxColumn,
            this.requestNameDataGridViewTextBoxColumn,
            this.pALTrayDataGridViewTextBoxColumn,
            this.pALVialDataGridViewTextBoxColumn,
            this.pALVolumeDataGridViewTextBoxColumn,
            this.lCMethodDataGridViewTextBoxColumn,
            this.instrumentMethodDataGridViewTextBoxColumn,
            this.datasetTypeDataGridViewTextBoxColumn,
            this.batchIDDataGridViewTextBoxColumn});
            this.mdataGrid_samples.DataSource = this.sampleToRowTranslatorBindingSource;
            this.mdataGrid_samples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mdataGrid_samples.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.mdataGrid_samples.Location = new System.Drawing.Point(0, 0);
            this.mdataGrid_samples.Margin = new System.Windows.Forms.Padding(4);
            this.mdataGrid_samples.Name = "mdataGrid_samples";
            this.mdataGrid_samples.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mdataGrid_samples.Size = new System.Drawing.Size(1235, 806);
            this.mdataGrid_samples.TabIndex = 7;
            // 
            // sampleToRowTranslatorBindingSource
            // 
            this.sampleToRowTranslatorBindingSource.DataSource = typeof(LcmsNet.SampleQueue.Forms.SampleToRowTranslator);
            // 
            // mcolumn_checkbox
            // 
            this.mcolumn_checkbox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.mcolumn_checkbox.DataPropertyName = "Checkbox";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.NullValue = false;
            this.mcolumn_checkbox.DefaultCellStyle = dataGridViewCellStyle3;
            this.mcolumn_checkbox.FalseValue = "enumCheckboxStatus.Unchecked";
            this.mcolumn_checkbox.HeaderText = "";
            this.mcolumn_checkbox.MinimumWidth = 25;
            this.mcolumn_checkbox.Name = "mcolumn_checkbox";
            this.mcolumn_checkbox.ReadOnly = true;
            this.mcolumn_checkbox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.mcolumn_checkbox.TrueValue = "enumCheckboxStatus.Checked";
            this.mcolumn_checkbox.Width = 25;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Status.DataPropertyName = "Status";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            this.Status.DefaultCellStyle = dataGridViewCellStyle4;
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Status.Width = 54;
            // 
            // mcolumn_sequenceNumber
            // 
            this.mcolumn_sequenceNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_sequenceNumber.DataPropertyName = "SequenceNumber";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Format = "0000";
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_sequenceNumber.DefaultCellStyle = dataGridViewCellStyle5;
            this.mcolumn_sequenceNumber.HeaderText = "Seq #";
            this.mcolumn_sequenceNumber.MinimumWidth = 10;
            this.mcolumn_sequenceNumber.Name = "mcolumn_sequenceNumber";
            this.mcolumn_sequenceNumber.ReadOnly = true;
            this.mcolumn_sequenceNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_sequenceNumber.Width = 51;
            // 
            // mcolumn_columnNumber
            // 
            this.mcolumn_columnNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_columnNumber.DataPropertyName = "ColumnNumber";
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_columnNumber.DefaultCellStyle = dataGridViewCellStyle6;
            this.mcolumn_columnNumber.HeaderText = "Col #";
            this.mcolumn_columnNumber.Name = "mcolumn_columnNumber";
            this.mcolumn_columnNumber.ReadOnly = true;
            this.mcolumn_columnNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_columnNumber.Width = 46;
            // 
            // mcolumn_uniqueID
            // 
            this.mcolumn_uniqueID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_uniqueID.DataPropertyName = "UniqueID";
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_uniqueID.DefaultCellStyle = dataGridViewCellStyle7;
            this.mcolumn_uniqueID.HeaderText = "UID";
            this.mcolumn_uniqueID.Name = "mcolumn_uniqueID";
            this.mcolumn_uniqueID.ReadOnly = true;
            this.mcolumn_uniqueID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_uniqueID.Width = 37;
            // 
            // mcolumn_blockNumber
            // 
            this.mcolumn_blockNumber.DataPropertyName = "BlockNumber";
            this.mcolumn_blockNumber.HeaderText = "Block";
            this.mcolumn_blockNumber.Name = "mcolumn_blockNumber";
            this.mcolumn_blockNumber.ReadOnly = true;
            this.mcolumn_blockNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_blockNumber.Width = 60;
            // 
            // mcolumn_runOrder
            // 
            this.mcolumn_runOrder.DataPropertyName = "RunOrder";
            this.mcolumn_runOrder.HeaderText = "Run Order";
            this.mcolumn_runOrder.Name = "mcolumn_runOrder";
            this.mcolumn_runOrder.ReadOnly = true;
            this.mcolumn_runOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_runOrder.Width = 30;
            // 
            // mcolumn_requestName
            // 
            this.mcolumn_requestName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.mcolumn_requestName.DataPropertyName = "RequestName";
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_requestName.DefaultCellStyle = dataGridViewCellStyle8;
            this.mcolumn_requestName.HeaderText = "Dataset Name";
            this.mcolumn_requestName.MinimumWidth = 10;
            this.mcolumn_requestName.Name = "mcolumn_requestName";
            this.mcolumn_requestName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // mcolumn_PalTray
            // 
            this.mcolumn_PalTray.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_PalTray.DataPropertyName = "PALTray";
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_PalTray.DefaultCellStyle = dataGridViewCellStyle9;
            this.mcolumn_PalTray.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcolumn_PalTray.HeaderText = "PAL Tray";
            this.mcolumn_PalTray.MaxDropDownItems = 100;
            this.mcolumn_PalTray.MinimumWidth = 10;
            this.mcolumn_PalTray.Name = "mcolumn_PalTray";
            this.mcolumn_PalTray.Sorted = true;
            this.mcolumn_PalTray.Width = 66;
            // 
            // mcolumn_palVial
            // 
            this.mcolumn_palVial.DataPropertyName = "PALVial";
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_palVial.DefaultCellStyle = dataGridViewCellStyle10;
            this.mcolumn_palVial.HeaderText = "PAL Vial";
            this.mcolumn_palVial.Name = "mcolumn_palVial";
            this.mcolumn_palVial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_palVial.Width = 50;
            // 
            // mcolumn_PALVolume
            // 
            this.mcolumn_PALVolume.DataPropertyName = "PALVolume";
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_PALVolume.DefaultCellStyle = dataGridViewCellStyle11;
            this.mcolumn_PALVolume.HeaderText = "Volume";
            this.mcolumn_PALVolume.Name = "mcolumn_PALVolume";
            this.mcolumn_PALVolume.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_PALVolume.Width = 50;
            // 
            // mcolumn_LCMethod
            // 
            this.mcolumn_LCMethod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_LCMethod.DataPropertyName = "LCMethod";
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_LCMethod.DefaultCellStyle = dataGridViewCellStyle12;
            this.mcolumn_LCMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcolumn_LCMethod.HeaderText = "LC Method";
            this.mcolumn_LCMethod.MaxDropDownItems = 100;
            this.mcolumn_LCMethod.Name = "mcolumn_LCMethod";
            this.mcolumn_LCMethod.Sorted = true;
            this.mcolumn_LCMethod.Width = 74;
            // 
            // mcolumn_instrumentMethod
            // 
            this.mcolumn_instrumentMethod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_instrumentMethod.DataPropertyName = "InstrumentMethod";
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_instrumentMethod.DefaultCellStyle = dataGridViewCellStyle13;
            this.mcolumn_instrumentMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcolumn_instrumentMethod.HeaderText = "Instrument Method";
            this.mcolumn_instrumentMethod.MaxDropDownItems = 100;
            this.mcolumn_instrumentMethod.Name = "mcolumn_instrumentMethod";
            this.mcolumn_instrumentMethod.Sorted = true;
            this.mcolumn_instrumentMethod.Width = 118;
            // 
            // mcolumn_datasetType
            // 
            this.mcolumn_datasetType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mcolumn_datasetType.DataPropertyName = "DatasetType";
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.MediumBlue;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.White;
            this.mcolumn_datasetType.DefaultCellStyle = dataGridViewCellStyle14;
            this.mcolumn_datasetType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcolumn_datasetType.HeaderText = "Dataset Type";
            this.mcolumn_datasetType.Items.AddRange(new object[] {
            "Test"});
            this.mcolumn_datasetType.MaxDropDownItems = 100;
            this.mcolumn_datasetType.Name = "mcolumn_datasetType";
            this.mcolumn_datasetType.Sorted = true;
            this.mcolumn_datasetType.Width = 89;
            // 
            // mcolumn_batchID
            // 
            this.mcolumn_batchID.DataPropertyName = "BatchID";
            this.mcolumn_batchID.HeaderText = "Batch ID";
            this.mcolumn_batchID.Name = "mcolumn_batchID";
            this.mcolumn_batchID.ReadOnly = true;
            this.mcolumn_batchID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mcolumn_batchID.Width = 68;
            // 
            // sampleDataGridViewTextBoxColumn
            // 
            this.sampleDataGridViewTextBoxColumn.DataPropertyName = "Sample";
            this.sampleDataGridViewTextBoxColumn.HeaderText = "Sample";
            this.sampleDataGridViewTextBoxColumn.Name = "sampleDataGridViewTextBoxColumn";
            this.sampleDataGridViewTextBoxColumn.Visible = false;
            // 
            // sequenceNumberDataGridViewTextBoxColumn
            // 
            this.sequenceNumberDataGridViewTextBoxColumn.DataPropertyName = "SequenceNumber";
            this.sequenceNumberDataGridViewTextBoxColumn.HeaderText = "SequenceNumber";
            this.sequenceNumberDataGridViewTextBoxColumn.Name = "sequenceNumberDataGridViewTextBoxColumn";
            this.sequenceNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.sequenceNumberDataGridViewTextBoxColumn.Visible = false;
            // 
            // columnNumberDataGridViewTextBoxColumn
            // 
            this.columnNumberDataGridViewTextBoxColumn.DataPropertyName = "ColumnNumber";
            this.columnNumberDataGridViewTextBoxColumn.HeaderText = "ColumnNumber";
            this.columnNumberDataGridViewTextBoxColumn.Name = "columnNumberDataGridViewTextBoxColumn";
            this.columnNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.columnNumberDataGridViewTextBoxColumn.Visible = false;
            // 
            // specialColumnNumberDataGridViewTextBoxColumn
            // 
            this.specialColumnNumberDataGridViewTextBoxColumn.DataPropertyName = "SpecialColumnNumber";
            this.specialColumnNumberDataGridViewTextBoxColumn.HeaderText = "SpecialColumnNumber";
            this.specialColumnNumberDataGridViewTextBoxColumn.Name = "specialColumnNumberDataGridViewTextBoxColumn";
            this.specialColumnNumberDataGridViewTextBoxColumn.Visible = false;
            // 
            // uniqueIDDataGridViewTextBoxColumn
            // 
            this.uniqueIDDataGridViewTextBoxColumn.DataPropertyName = "UniqueID";
            this.uniqueIDDataGridViewTextBoxColumn.HeaderText = "UniqueID";
            this.uniqueIDDataGridViewTextBoxColumn.Name = "uniqueIDDataGridViewTextBoxColumn";
            this.uniqueIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.uniqueIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // checkboxDataGridViewTextBoxColumn
            // 
            this.checkboxDataGridViewTextBoxColumn.DataPropertyName = "Checkbox";
            this.checkboxDataGridViewTextBoxColumn.HeaderText = "Checkbox";
            this.checkboxDataGridViewTextBoxColumn.Name = "checkboxDataGridViewTextBoxColumn";
            this.checkboxDataGridViewTextBoxColumn.ReadOnly = true;
            this.checkboxDataGridViewTextBoxColumn.Visible = false;
            // 
            // checkboxTagDataGridViewTextBoxColumn
            // 
            this.checkboxTagDataGridViewTextBoxColumn.DataPropertyName = "CheckboxTag";
            this.checkboxTagDataGridViewTextBoxColumn.HeaderText = "CheckboxTag";
            this.checkboxTagDataGridViewTextBoxColumn.Name = "checkboxTagDataGridViewTextBoxColumn";
            this.checkboxTagDataGridViewTextBoxColumn.ReadOnly = true;
            this.checkboxTagDataGridViewTextBoxColumn.Visible = false;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Visible = false;
            // 
            // statusToolTipTextDataGridViewTextBoxColumn
            // 
            this.statusToolTipTextDataGridViewTextBoxColumn.DataPropertyName = "StatusToolTipText";
            this.statusToolTipTextDataGridViewTextBoxColumn.HeaderText = "StatusToolTipText";
            this.statusToolTipTextDataGridViewTextBoxColumn.Name = "statusToolTipTextDataGridViewTextBoxColumn";
            this.statusToolTipTextDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusToolTipTextDataGridViewTextBoxColumn.Visible = false;
            // 
            // blockNumberDataGridViewTextBoxColumn
            // 
            this.blockNumberDataGridViewTextBoxColumn.DataPropertyName = "BlockNumber";
            this.blockNumberDataGridViewTextBoxColumn.HeaderText = "BlockNumber";
            this.blockNumberDataGridViewTextBoxColumn.Name = "blockNumberDataGridViewTextBoxColumn";
            this.blockNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.blockNumberDataGridViewTextBoxColumn.Visible = false;
            // 
            // runOrderDataGridViewTextBoxColumn
            // 
            this.runOrderDataGridViewTextBoxColumn.DataPropertyName = "RunOrder";
            this.runOrderDataGridViewTextBoxColumn.HeaderText = "RunOrder";
            this.runOrderDataGridViewTextBoxColumn.Name = "runOrderDataGridViewTextBoxColumn";
            this.runOrderDataGridViewTextBoxColumn.ReadOnly = true;
            this.runOrderDataGridViewTextBoxColumn.Visible = false;
            // 
            // requestNameDataGridViewTextBoxColumn
            // 
            this.requestNameDataGridViewTextBoxColumn.DataPropertyName = "RequestName";
            this.requestNameDataGridViewTextBoxColumn.HeaderText = "RequestName";
            this.requestNameDataGridViewTextBoxColumn.Name = "requestNameDataGridViewTextBoxColumn";
            this.requestNameDataGridViewTextBoxColumn.Visible = false;
            // 
            // pALTrayDataGridViewTextBoxColumn
            // 
            this.pALTrayDataGridViewTextBoxColumn.DataPropertyName = "PALTray";
            this.pALTrayDataGridViewTextBoxColumn.HeaderText = "PALTray";
            this.pALTrayDataGridViewTextBoxColumn.Name = "pALTrayDataGridViewTextBoxColumn";
            this.pALTrayDataGridViewTextBoxColumn.Visible = false;
            // 
            // pALVialDataGridViewTextBoxColumn
            // 
            this.pALVialDataGridViewTextBoxColumn.DataPropertyName = "PALVial";
            this.pALVialDataGridViewTextBoxColumn.HeaderText = "PALVial";
            this.pALVialDataGridViewTextBoxColumn.Name = "pALVialDataGridViewTextBoxColumn";
            this.pALVialDataGridViewTextBoxColumn.Visible = false;
            // 
            // pALVolumeDataGridViewTextBoxColumn
            // 
            this.pALVolumeDataGridViewTextBoxColumn.DataPropertyName = "PALVolume";
            this.pALVolumeDataGridViewTextBoxColumn.HeaderText = "PALVolume";
            this.pALVolumeDataGridViewTextBoxColumn.Name = "pALVolumeDataGridViewTextBoxColumn";
            this.pALVolumeDataGridViewTextBoxColumn.Visible = false;
            // 
            // lCMethodDataGridViewTextBoxColumn
            // 
            this.lCMethodDataGridViewTextBoxColumn.DataPropertyName = "LCMethod";
            this.lCMethodDataGridViewTextBoxColumn.HeaderText = "LCMethod";
            this.lCMethodDataGridViewTextBoxColumn.Name = "lCMethodDataGridViewTextBoxColumn";
            this.lCMethodDataGridViewTextBoxColumn.Visible = false;
            // 
            // instrumentMethodDataGridViewTextBoxColumn
            // 
            this.instrumentMethodDataGridViewTextBoxColumn.DataPropertyName = "InstrumentMethod";
            this.instrumentMethodDataGridViewTextBoxColumn.HeaderText = "InstrumentMethod";
            this.instrumentMethodDataGridViewTextBoxColumn.Name = "instrumentMethodDataGridViewTextBoxColumn";
            this.instrumentMethodDataGridViewTextBoxColumn.Visible = false;
            // 
            // datasetTypeDataGridViewTextBoxColumn
            // 
            this.datasetTypeDataGridViewTextBoxColumn.DataPropertyName = "DatasetType";
            this.datasetTypeDataGridViewTextBoxColumn.HeaderText = "DatasetType";
            this.datasetTypeDataGridViewTextBoxColumn.Name = "datasetTypeDataGridViewTextBoxColumn";
            this.datasetTypeDataGridViewTextBoxColumn.Visible = false;
            // 
            // batchIDDataGridViewTextBoxColumn
            // 
            this.batchIDDataGridViewTextBoxColumn.DataPropertyName = "BatchID";
            this.batchIDDataGridViewTextBoxColumn.HeaderText = "BatchID";
            this.batchIDDataGridViewTextBoxColumn.Name = "batchIDDataGridViewTextBoxColumn";
            this.batchIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.batchIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // controlSampleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.m_sampleContainer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "controlSampleView";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(1243, 814);
            this.mcontextMenu_options.ResumeLayout(false);
            this.m_sampleContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mdataGrid_samples)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleToRowTranslatorBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ContextMenuStrip mcontextMenu_options;
        private ToolStripMenuItem randomizeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn2;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn3;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn4;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn5;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripMenuItem pALTrayToolStripMenuItem;
        private ToolStripMenuItem pALVialToolStripMenuItem;
        private ToolStripMenuItem volumeToolStripMenuItem;
        private ToolStripMenuItem lCMethodToolStripMenuItem;
        private ToolStripMenuItem instrumentMethodToolStripMenuItem;
        private ToolStripMenuItem datasetTypeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem addBlankToolStripMenuItem;
        private ToolStripMenuItem importFromDMSToolStripMenuItem;
        private ToolStripMenuItem deleteSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem deleteUnusedToolStripMenuItem;
        private ToolStripMenuItem insertBlankIntoUnusedToolStripMenuItem;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem batchIDToolStripMenuItem;
        private ToolStripMenuItem previewThroughputToolStripMenuItem;
        public classDataGrid mdataGrid_samples;
        private ToolStripMenuItem blockToolStripMenuItem;
        private ToolStripMenuItem runOrderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem addDateCartNameColumnIDToolStripMenuItem;
        private Panel m_sampleContainer;
        private BindingSource sampleToRowTranslatorBindingSource;
        private DataGridViewCheckBoxColumn mcolumn_checkbox;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewTextBoxColumn mcolumn_sequenceNumber;
        private DataGridViewTextBoxColumn mcolumn_columnNumber;
        private DataGridViewTextBoxColumn mcolumn_uniqueID;
        private DataGridViewTextBoxColumn mcolumn_blockNumber;
        private DataGridViewTextBoxColumn mcolumn_runOrder;
        private DataGridViewTextBoxColumn mcolumn_requestName;
        private DataGridViewComboBoxColumn mcolumn_PalTray;
        private DataGridViewTextBoxColumn mcolumn_palVial;
        private DataGridViewTextBoxColumn mcolumn_PALVolume;
        private DataGridViewComboBoxColumn mcolumn_LCMethod;
        private DataGridViewComboBoxColumn mcolumn_instrumentMethod;
        private DataGridViewComboBoxColumn mcolumn_datasetType;
        private DataGridViewTextBoxColumn mcolumn_batchID;
        private DataGridViewTextBoxColumn sampleDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn sequenceNumberDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn columnNumberDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn specialColumnNumberDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn uniqueIDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn checkboxDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn checkboxTagDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusToolTipTextDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn blockNumberDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn runOrderDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn requestNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pALTrayDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pALVialDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pALVolumeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn lCMethodDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn instrumentMethodDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn datasetTypeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn batchIDDataGridViewTextBoxColumn;
    }
}
