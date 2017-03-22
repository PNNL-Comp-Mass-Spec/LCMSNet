//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Updates:
// - 1/20/2009 (DAC) Modified listview item addition to reflect data input from DMS view
// - 8/31/2010 (DAC) Changes resulting from moving part of config to LcmsNet namespace
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNet.Properties;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Class that displays sample data in as a sequence.
    /// </summary>
    public sealed class controlSequenceView : controlSampleView
    {
        /// <summary>
        /// Delegate defining when status updates are available in batches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messages"></param>
        public delegate void DelegateStatusUpdates(object sender, List<string> messages);

        #region Members

        private Label mlabel_name;

        #endregion

        #region Column Events

        /// <summary>
        /// Handles when a property about a column changes and rebuilds the column ordering list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        void column_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        {
            //
            // Make sure we have at least one column that is enabled
            //
            var enabled = false;
            foreach (var column in classCartConfiguration.Columns)
            {
                if (column.Status != enumColumnStatus.Disabled)
                {
                    enabled = true;
                    break;
                }
            }

            //
            // If at least one column is not enabled, then we disable the sample queue
            //
            if (enabled == false)
            {
                Enabled = false;
                BackColor = Color.LightGray;
            }
            else
            {
                Enabled = true;
                BackColor = Color.White;
            }

            m_sampleQueue.UpdateAllSamples();
        }

        #endregion

        #region Sample Queue - Addition Virtual Method Overrides

        /// <summary>
        /// Adds the samples to the manager appropriately.
        /// </summary>
        /// <param name="samples"></param>
        protected override void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            var data = m_sampleQueue.NextColumnData;

            // Make sure that we have a column data,
            // and that not all of the columns are
            // disabled for some odd (User) reason.
            //if (data == null)
            //{
            //    return;
            //}

            var index = m_sampleQueue.ColumnOrder.IndexOf(data);

            foreach (var sample in samples)
            {
                // Get the column data iterating through the enabled column list.
                if (IterateThroughColumns)
                {
                    sample.ColumnData = data;
                    sample.LCMethod = null;
                }
                // We still iterate here, in case the user wants to move the samples that were
                // on other columns already.
                data = m_sampleQueue.ColumnOrder[(++index) % m_sampleQueue.ColumnOrder.Count];
            }

            if (insertIntoUnused == false)
            {
                m_sampleQueue.QueueSamples(samples, ColumnHandling);
            }
            else
            {
                m_sampleQueue.InsertIntoUnusedSamples(samples, ColumnHandling);
            }
        }

        #endregion

        /// <summary>
        /// Windows Forms Designer.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mlabel_name = new System.Windows.Forms.Label();
            this.mToolTipManager = new System.Windows.Forms.ToolTip(this.components);
            this.mcheckbox_cycleColumns = new System.Windows.Forms.CheckBox();
            this.mbutton_trayVial = new System.Windows.Forms.Button();
            this.mbutton_fillDown = new System.Windows.Forms.Button();
            this.mbutton_deleteUnused = new System.Windows.Forms.Button();
            this.mbutton_removeSelected = new System.Windows.Forms.Button();
            this.mbutton_addDMS = new System.Windows.Forms.Button();
            this.mbutton_up = new System.Windows.Forms.Button();
            this.mbutton_addBlank = new System.Windows.Forms.Button();
            this.mbutton_down = new System.Windows.Forms.Button();
            this.mbutton_cartColumnDate = new System.Windows.Forms.Button();
            this.mbutton_dmsEdit = new System.Windows.Forms.Button();
            this.mcheckbox_autoscroll = new System.Windows.Forms.CheckBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_selector
            // 
            this.m_selector.ClientSize = new System.Drawing.Size(0, 0);
            this.m_selector.Location = new System.Drawing.Point(-32000, -32000);
            this.m_selector.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            // 
            // mlabel_name
            // 
            this.mlabel_name.AutoSize = true;
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(0, 13);
            this.mlabel_name.TabIndex = 0;
            // 
            // mcheckbox_cycleColumns
            // 
            this.mcheckbox_cycleColumns.Location = new System.Drawing.Point(52, 95);
            this.mcheckbox_cycleColumns.Margin = new System.Windows.Forms.Padding(4);
            this.mcheckbox_cycleColumns.Name = "mcheckbox_cycleColumns";
            this.mcheckbox_cycleColumns.Size = new System.Drawing.Size(145, 22);
            this.mcheckbox_cycleColumns.TabIndex = 35;
            this.mcheckbox_cycleColumns.Text = "Cycle Columns";
            this.mcheckbox_cycleColumns.UseVisualStyleBackColor = true;
            this.mcheckbox_cycleColumns.CheckedChanged += new System.EventHandler(this.mcheckbox_cycleColumns_CheckedChanged);
            // 
            // mbutton_trayVial
            // 
            this.mbutton_trayVial.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_trayVial.Image = global::LcmsNet.Properties.Resources.testTube;
            this.mbutton_trayVial.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_trayVial.Location = new System.Drawing.Point(495, 6);
            this.mbutton_trayVial.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_trayVial.Name = "mbutton_trayVial";
            this.mbutton_trayVial.Size = new System.Drawing.Size(80, 118);
            this.mbutton_trayVial.TabIndex = 39;
            this.mbutton_trayVial.Text = "Tray Vial";
            this.mbutton_trayVial.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_trayVial.UseVisualStyleBackColor = true;
            this.mbutton_trayVial.Click += new System.EventHandler(this.mbutton_trayVial_Click);
            // 
            // mbutton_fillDown
            // 
            this.mbutton_fillDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_fillDown.Image = global::LcmsNet.Properties.Resources.Filldown;
            this.mbutton_fillDown.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_fillDown.Location = new System.Drawing.Point(407, 6);
            this.mbutton_fillDown.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_fillDown.Name = "mbutton_fillDown";
            this.mbutton_fillDown.Size = new System.Drawing.Size(80, 118);
            this.mbutton_fillDown.TabIndex = 39;
            this.mbutton_fillDown.Text = "Fill Down";
            this.mbutton_fillDown.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_fillDown.UseVisualStyleBackColor = true;
            this.mbutton_fillDown.Click += new System.EventHandler(this.mbutton_fillDown_Click);
            // 
            // mbutton_deleteUnused
            // 
            this.mbutton_deleteUnused.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_deleteUnused.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_deleteUnused.ForeColor = System.Drawing.Color.Black;
            this.mbutton_deleteUnused.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_deleteUnused.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_deleteUnused.Location = new System.Drawing.Point(231, 6);
            this.mbutton_deleteUnused.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_deleteUnused.Name = "mbutton_deleteUnused";
            this.mbutton_deleteUnused.Size = new System.Drawing.Size(80, 118);
            this.mbutton_deleteUnused.TabIndex = 33;
            this.mbutton_deleteUnused.Text = "Unused";
            this.mbutton_deleteUnused.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_deleteUnused.UseVisualStyleBackColor = false;
            this.mbutton_deleteUnused.Click += new System.EventHandler(this.mbutton_deleteUnused_Click);
            // 
            // mbutton_removeSelected
            // 
            this.mbutton_removeSelected.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_removeSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_removeSelected.ForeColor = System.Drawing.Color.Black;
            this.mbutton_removeSelected.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_removeSelected.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_removeSelected.Location = new System.Drawing.Point(319, 6);
            this.mbutton_removeSelected.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_removeSelected.Name = "mbutton_removeSelected";
            this.mbutton_removeSelected.Size = new System.Drawing.Size(80, 118);
            this.mbutton_removeSelected.TabIndex = 32;
            this.mbutton_removeSelected.Text = "Selected";
            this.mbutton_removeSelected.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_removeSelected.UseVisualStyleBackColor = false;
            this.mbutton_removeSelected.Click += new System.EventHandler(this.mbutton_removeSelected_Click);
            // 
            // mbutton_addDMS
            // 
            this.mbutton_addDMS.Image = global::LcmsNet.Properties.Resources.AddDMS;
            this.mbutton_addDMS.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_addDMS.Location = new System.Drawing.Point(140, 6);
            this.mbutton_addDMS.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_addDMS.Name = "mbutton_addDMS";
            this.mbutton_addDMS.Size = new System.Drawing.Size(80, 81);
            this.mbutton_addDMS.TabIndex = 34;
            this.mbutton_addDMS.Text = "DMS";
            this.mbutton_addDMS.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_addDMS.UseVisualStyleBackColor = true;
            this.mbutton_addDMS.Click += new System.EventHandler(this.mbutton_addDMS_Click);
            // 
            // mbutton_up
            // 
            this.mbutton_up.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_up.Image = global::LcmsNet.Properties.Resources.Button_Up_16;
            this.mbutton_up.Location = new System.Drawing.Point(856, 4);
            this.mbutton_up.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_up.Name = "mbutton_up";
            this.mbutton_up.Size = new System.Drawing.Size(80, 118);
            this.mbutton_up.TabIndex = 30;
            this.mbutton_up.UseVisualStyleBackColor = true;
            this.mbutton_up.Click += new System.EventHandler(this.mbutton_up_Click);
            // 
            // mbutton_addBlank
            // 
            this.mbutton_addBlank.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_addBlank.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_addBlank.ForeColor = System.Drawing.Color.Black;
            this.mbutton_addBlank.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_addBlank.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_addBlank.Location = new System.Drawing.Point(52, 6);
            this.mbutton_addBlank.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_addBlank.Name = "mbutton_addBlank";
            this.mbutton_addBlank.Size = new System.Drawing.Size(80, 81);
            this.mbutton_addBlank.TabIndex = 31;
            this.mbutton_addBlank.Text = "Blank";
            this.mbutton_addBlank.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_addBlank.UseVisualStyleBackColor = false;
            this.mbutton_addBlank.Click += new System.EventHandler(this.mbutton_addBlank_Click);
            // 
            // mbutton_down
            // 
            this.mbutton_down.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_down.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.mbutton_down.Location = new System.Drawing.Point(944, 5);
            this.mbutton_down.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_down.Name = "mbutton_down";
            this.mbutton_down.Size = new System.Drawing.Size(80, 118);
            this.mbutton_down.TabIndex = 31;
            this.mbutton_down.UseVisualStyleBackColor = true;
            this.mbutton_down.Click += new System.EventHandler(this.mbutton_down_Click);
            // 
            // mbutton_cartColumnDate
            // 
            this.mbutton_cartColumnDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_cartColumnDate.Image = global::LcmsNet.Properties.Resources.CartColumnName;
            this.mbutton_cartColumnDate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_cartColumnDate.Location = new System.Drawing.Point(583, 6);
            this.mbutton_cartColumnDate.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_cartColumnDate.Name = "mbutton_cartColumnDate";
            this.mbutton_cartColumnDate.Size = new System.Drawing.Size(80, 118);
            this.mbutton_cartColumnDate.TabIndex = 40;
            this.mbutton_cartColumnDate.Text = "Cart, Col, Date";
            this.mbutton_cartColumnDate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_cartColumnDate.UseVisualStyleBackColor = true;
            this.mbutton_cartColumnDate.Click += new System.EventHandler(this.mbutton_cartColumnDate_Click);
            // 
            // mbutton_dmsEdit
            // 
            this.mbutton_dmsEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_dmsEdit.Image = global::LcmsNet.Properties.Resources.DMSEdit;
            this.mbutton_dmsEdit.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_dmsEdit.Location = new System.Drawing.Point(671, 6);
            this.mbutton_dmsEdit.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_dmsEdit.Name = "mbutton_dmsEdit";
            this.mbutton_dmsEdit.Size = new System.Drawing.Size(80, 118);
            this.mbutton_dmsEdit.TabIndex = 41;
            this.mbutton_dmsEdit.Text = "DMS Edit";
            this.mbutton_dmsEdit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_dmsEdit.UseVisualStyleBackColor = true;
            this.mbutton_dmsEdit.Click += new System.EventHandler(this.mbutton_dmsEdit_Click);
            // 
            // mcheckbox_autoscroll
            // 
            this.mcheckbox_autoscroll.AutoSize = true;
            this.mcheckbox_autoscroll.Checked = true;
            this.mcheckbox_autoscroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcheckbox_autoscroll.Location = new System.Drawing.Point(759, 96);
            this.mcheckbox_autoscroll.Margin = new System.Windows.Forms.Padding(4);
            this.mcheckbox_autoscroll.Name = "mcheckbox_autoscroll";
            this.mcheckbox_autoscroll.Size = new System.Drawing.Size(97, 21);
            this.mcheckbox_autoscroll.TabIndex = 42;
            this.mcheckbox_autoscroll.Text = "Auto-scroll";
            this.mcheckbox_autoscroll.UseVisualStyleBackColor = true;
            this.mcheckbox_autoscroll.CheckedChanged += new System.EventHandler(this.mcheckbox_autoscroll_CheckedChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackColor = System.Drawing.Color.Transparent;
            this.buttonRefresh.CausesValidation = false;
            this.buttonRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.ForeColor = System.Drawing.Color.Black;
            this.buttonRefresh.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonRefresh.Location = new System.Drawing.Point(759, 7);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(80, 81);
            this.buttonRefresh.TabIndex = 43;
            this.buttonRefresh.Text = "Refresh\r\nList";
            this.buttonRefresh.UseVisualStyleBackColor = false;
            this.buttonRefresh.Click += new System.EventHandler(this.mbutton_refresh_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonRefresh);
            this.panel2.Controls.Add(this.mcheckbox_autoscroll);
            this.panel2.Controls.Add(this.mbutton_dmsEdit);
            this.panel2.Controls.Add(this.mbutton_cartColumnDate);
            this.panel2.Controls.Add(this.mbutton_down);
            this.panel2.Controls.Add(this.mbutton_addBlank);
            this.panel2.Controls.Add(this.mbutton_up);
            this.panel2.Controls.Add(this.mbutton_addDMS);
            this.panel2.Controls.Add(this.mbutton_removeSelected);
            this.panel2.Controls.Add(this.mbutton_deleteUnused);
            this.panel2.Controls.Add(this.mbutton_fillDown);
            this.panel2.Controls.Add(this.mbutton_trayVial);
            this.panel2.Controls.Add(this.mcheckbox_cycleColumns);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 708);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1031, 128);
            this.panel2.TabIndex = 20;
            // 
            // controlSequenceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "controlSequenceView";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(1041, 841);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void mbutton_addBlank_Click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void mbutton_addDMS_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

        private void mbutton_removeSelected_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone);
        }

        // Deprecated, unused
        //private void mbutton_ccd_Click(object sender, EventArgs e)
        //{
        //    AddDateCartnameColumnIDToDatasetName();
        //}

        private void mbutton_fillDown_Click(object sender, EventArgs e)
        {
            FillDown();
        }

        private void mbutton_trayVial_Click(object sender, EventArgs e)
        {
            EditTrayAndVial();
        }

        private void mbutton_randomize_Click(object sender, EventArgs e)
        {
            RandomizeSelectedSamples();
        }

        private void mbutton_down_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(1, enumMoveSampleType.Sequence);
        }

        private void mbutton_up_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(-1, enumMoveSampleType.Sequence);
        }

        private void mcheckbox_cycleColumns_CheckedChanged(object sender, EventArgs e)
        {
            var handling = enumColumnDataHandling.LeaveAlone;
            if (mcheckbox_cycleColumns.Checked)
            {
                handling = enumColumnDataHandling.Resort;
            }
            ColumnHandling = handling;

            IterateThroughColumns = mcheckbox_cycleColumns.Checked;
        }

        private void mcheckbox_autoscroll_CheckedChanged(object sender, EventArgs e)
        {
            m_autoscroll = mcheckbox_autoscroll.Checked;
        }

        private void mbutton_deleteUnused_Click(object sender, EventArgs e)
        {
            RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone);
        }

        private void mbutton_cartColumnDate_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

        private void mbutton_refresh_Click(object sender, EventArgs e)
        {
            InvalidateGridView(true);
        }


        private void mbutton_dmsEdit_Click(object sender, EventArgs e)
        {
            EditDMSData();
        }

        private void m_selector_Load(object sender, EventArgs e)
        {
        }

        #region Constructors and Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public controlSequenceView(formDMSView dmsView, classSampleQueue sampleQueue) :
            base(dmsView, sampleQueue)
        {
            InitializeComponent();

            UpdateToolTips();

            DisplayColumn(CONST_COLUMN_PAL_TRAY, true);
            DisplayColumn(CONST_COLUMN_PAL_VIAL, true);
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);

            EnableQueueing(true);

            foreach (var column in classCartConfiguration.Columns)
            {
                column.StatusChanged += column_StatusChanged;
            }

            panel2.SendToBack();
            if (string.IsNullOrWhiteSpace(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL)))
            {
                mbutton_addDMS.Visible = false;
                mbutton_dmsEdit.Visible = false;
            }
        }

        private void UpdateToolTips()
        {
            this.mToolTipManager.SetToolTip(mbutton_cartColumnDate, "Add date, cart name, and columnID to the dataset name");
        }

        /// <summary>
        /// Default constructor.  To use this constructor one must supply the dms view and
        /// sample queue to the respectively named properties.
        /// </summary>
        public controlSequenceView()
        {
            InitializeComponent();
            DisplayColumn(CONST_COLUMN_PAL_TRAY, true);
            DisplayColumn(CONST_COLUMN_PAL_VIAL, true);
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);

            EnableQueueing(true);

            if (classCartConfiguration.Columns != null)
            {
                foreach (var column in classCartConfiguration.Columns)
                {
                    column.StatusChanged += column_StatusChanged;
                }
            }

            panel2.SendToBack();
            if (string.IsNullOrWhiteSpace(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL)))
            {
                mbutton_addDMS.Visible = false;
                mbutton_dmsEdit.Visible = false;
            }
            classLCMSSettings.SettingChanged += classLCMSSettings_SettingChanged;
        }

        void classLCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName == "DMSTool" && e.SettingValue != string.Empty)
            {
                mbutton_addDMS.Visible = true;
                mbutton_dmsEdit.Visible = true;
            }
        }

        #endregion

        #region Queue Region Moving
        private ToolTip mToolTipManager;
        private CheckBox mcheckbox_cycleColumns;
        private Button mbutton_trayVial;
        private Button mbutton_fillDown;
        private Button mbutton_deleteUnused;
        private Button mbutton_removeSelected;
        private Button mbutton_addDMS;
        private Button mbutton_up;
        private Button mbutton_addBlank;
        private Button mbutton_down;
        private Button mbutton_cartColumnDate;
        private Button mbutton_dmsEdit;
        private CheckBox mcheckbox_autoscroll;
        private Button buttonRefresh;
        private Panel panel2;
        private System.ComponentModel.IContainer components;
      
        #endregion

    }
}