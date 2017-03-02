//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
/* Last modified 01/16/2009
 *
 *      1/16/2009:  Brian LaMarche
 *      1/20/2009:  Dave Clark
 *            - Modified listview item addition to reflect data input from DMS view
 *        8/31/2010:  Dave Clark
 *                - Changes resulting from moving part of config to LcmsNet namespace
*/
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Class that displays sample data in as a sequence.
    /// </summary>
    public sealed partial class controlSequenceView : controlSampleView
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
        /// Adds the samples to the manager appropiately.
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
            mlabel_name = new Label();
            panel2 = new Panel();
            buttonRefresh = new Button();
            mcheckbox_autoscroll = new CheckBox();
            mbutton_dmsEdit = new Button();
            mbutton_cartColumnDate = new Button();
            mbutton_down = new Button();
            mbutton_addBlank = new Button();
            mbutton_up = new Button();
            mbutton_addDMS = new Button();
            mbutton_removeSelected = new Button();
            mbutton_deleteUnused = new Button();
            mbutton_fillDown = new Button();
            mbutton_trayVial = new Button();
            mcheckbox_cycleColumns = new CheckBox();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // m_selector
            // 
            m_selector.Location = new Point(150, 150);
            m_selector.Load += m_selector_Load;
            // 
            // mlabel_name
            // 
            mlabel_name.AutoSize = true;
            mlabel_name.Dock = DockStyle.Fill;
            mlabel_name.Location = new Point(0, 0);
            mlabel_name.Name = "mlabel_name";
            mlabel_name.Size = new Size(0, 13);
            mlabel_name.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(buttonRefresh);
            panel2.Controls.Add(mcheckbox_autoscroll);
            panel2.Controls.Add(mbutton_dmsEdit);
            panel2.Controls.Add(mbutton_cartColumnDate);
            panel2.Controls.Add(mbutton_down);
            panel2.Controls.Add(mbutton_addBlank);
            panel2.Controls.Add(mbutton_up);
            panel2.Controls.Add(mbutton_addDMS);
            panel2.Controls.Add(mbutton_removeSelected);
            panel2.Controls.Add(mbutton_deleteUnused);
            panel2.Controls.Add(mbutton_fillDown);
            panel2.Controls.Add(mbutton_trayVial);
            panel2.Controls.Add(mcheckbox_cycleColumns);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(3, 576);
            panel2.Name = "panel2";
            panel2.Size = new Size(860, 104);
            panel2.TabIndex = 20;
            // 
            // buttonRefresh
            // 
            buttonRefresh.BackColor = Color.Transparent;
            buttonRefresh.CausesValidation = false;
            buttonRefresh.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            buttonRefresh.ForeColor = Color.Black;
            buttonRefresh.ImageAlign = ContentAlignment.TopCenter;
            buttonRefresh.Location = new Point(569, 6);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(60, 66);
            buttonRefresh.TabIndex = 43;
            buttonRefresh.Text = "Refresh\r\nList";
            buttonRefresh.UseVisualStyleBackColor = false;
            buttonRefresh.Click += mbutton_refresh_Click;
            // 
            // mcheckbox_autoscroll
            // 
            mcheckbox_autoscroll.AutoSize = true;
            mcheckbox_autoscroll.Checked = true;
            mcheckbox_autoscroll.CheckState = CheckState.Checked;
            mcheckbox_autoscroll.Location = new Point(569, 78);
            mcheckbox_autoscroll.Name = "mcheckbox_autoscroll";
            mcheckbox_autoscroll.Size = new Size(75, 17);
            mcheckbox_autoscroll.TabIndex = 42;
            mcheckbox_autoscroll.Text = "Auto-scroll";
            mcheckbox_autoscroll.UseVisualStyleBackColor = true;
            mcheckbox_autoscroll.CheckedChanged += mcheckbox_autoscroll_CheckedChanged;
            // 
            // mbutton_dmsEdit
            // 
            mbutton_dmsEdit.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_dmsEdit.Image = Properties.Resources.DMSEdit;
            mbutton_dmsEdit.ImageAlign = ContentAlignment.TopCenter;
            mbutton_dmsEdit.Location = new Point(503, 5);
            mbutton_dmsEdit.Name = "mbutton_dmsEdit";
            mbutton_dmsEdit.Size = new Size(60, 96);
            mbutton_dmsEdit.TabIndex = 41;
            mbutton_dmsEdit.Text = "DMS Edit";
            mbutton_dmsEdit.TextAlign = ContentAlignment.BottomCenter;
            mbutton_dmsEdit.UseVisualStyleBackColor = true;
            mbutton_dmsEdit.Click += mbutton_dmsEdit_Click;
            // 
            // mbutton_cartColumnDate
            // 
            mbutton_cartColumnDate.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_cartColumnDate.Image = Properties.Resources.CartColumnName;
            mbutton_cartColumnDate.ImageAlign = ContentAlignment.TopCenter;
            mbutton_cartColumnDate.Location = new Point(437, 5);
            mbutton_cartColumnDate.Name = "mbutton_cartColumnDate";
            mbutton_cartColumnDate.Size = new Size(60, 96);
            mbutton_cartColumnDate.TabIndex = 40;
            mbutton_cartColumnDate.Text = "Cart, Col, Date";
            mbutton_cartColumnDate.TextAlign = ContentAlignment.BottomCenter;
            mbutton_cartColumnDate.UseVisualStyleBackColor = true;
            mbutton_cartColumnDate.Click += mbutton_cartColumnDate_Click;
            // 
            // mbutton_down
            // 
            mbutton_down.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Right)));
            mbutton_down.Image = Properties.Resources.Button_Down_16;
            mbutton_down.Location = new Point(795, 4);
            mbutton_down.Name = "mbutton_down";
            mbutton_down.Size = new Size(60, 96);
            mbutton_down.TabIndex = 31;
            mbutton_down.UseVisualStyleBackColor = true;
            mbutton_down.Click += mbutton_down_Click;
            // 
            // mbutton_addBlank
            // 
            mbutton_addBlank.BackColor = Color.Transparent;
            mbutton_addBlank.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_addBlank.ForeColor = Color.Black;
            mbutton_addBlank.Image = Properties.Resources.add;
            mbutton_addBlank.ImageAlign = ContentAlignment.TopCenter;
            mbutton_addBlank.Location = new Point(39, 5);
            mbutton_addBlank.Name = "mbutton_addBlank";
            mbutton_addBlank.Size = new Size(60, 66);
            mbutton_addBlank.TabIndex = 31;
            mbutton_addBlank.Text = "Blank";
            mbutton_addBlank.TextAlign = ContentAlignment.BottomCenter;
            mbutton_addBlank.UseVisualStyleBackColor = false;
            mbutton_addBlank.Click += mbutton_addBlank_Click;
            // 
            // mbutton_up
            // 
            mbutton_up.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Right)));
            mbutton_up.Image = Properties.Resources.Button_Up_16;
            mbutton_up.Location = new Point(729, 3);
            mbutton_up.Name = "mbutton_up";
            mbutton_up.Size = new Size(60, 96);
            mbutton_up.TabIndex = 30;
            mbutton_up.UseVisualStyleBackColor = true;
            mbutton_up.Click += mbutton_up_Click;
            // 
            // mbutton_addDMS
            // 
            mbutton_addDMS.Image = Properties.Resources.AddDMS;
            mbutton_addDMS.ImageAlign = ContentAlignment.TopCenter;
            mbutton_addDMS.Location = new Point(105, 5);
            mbutton_addDMS.Name = "mbutton_addDMS";
            mbutton_addDMS.Size = new Size(60, 66);
            mbutton_addDMS.TabIndex = 34;
            mbutton_addDMS.Text = "DMS";
            mbutton_addDMS.TextAlign = ContentAlignment.BottomCenter;
            mbutton_addDMS.UseVisualStyleBackColor = true;
            mbutton_addDMS.Click += mbutton_addDMS_Click;
            // 
            // mbutton_removeSelected
            // 
            mbutton_removeSelected.BackColor = Color.Transparent;
            mbutton_removeSelected.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_removeSelected.ForeColor = Color.Black;
            mbutton_removeSelected.Image = Properties.Resources.Button_Delete_16;
            mbutton_removeSelected.ImageAlign = ContentAlignment.TopCenter;
            mbutton_removeSelected.Location = new Point(239, 5);
            mbutton_removeSelected.Name = "mbutton_removeSelected";
            mbutton_removeSelected.Size = new Size(60, 96);
            mbutton_removeSelected.TabIndex = 32;
            mbutton_removeSelected.Text = "Selected";
            mbutton_removeSelected.TextAlign = ContentAlignment.BottomCenter;
            mbutton_removeSelected.UseVisualStyleBackColor = false;
            mbutton_removeSelected.Click += mbutton_removeSelected_Click;
            // 
            // mbutton_deleteUnused
            // 
            mbutton_deleteUnused.BackColor = Color.Transparent;
            mbutton_deleteUnused.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_deleteUnused.ForeColor = Color.Black;
            mbutton_deleteUnused.Image = Properties.Resources.Button_Delete_16;
            mbutton_deleteUnused.ImageAlign = ContentAlignment.TopCenter;
            mbutton_deleteUnused.Location = new Point(173, 5);
            mbutton_deleteUnused.Name = "mbutton_deleteUnused";
            mbutton_deleteUnused.Size = new Size(60, 96);
            mbutton_deleteUnused.TabIndex = 33;
            mbutton_deleteUnused.Text = "Unused";
            mbutton_deleteUnused.TextAlign = ContentAlignment.BottomCenter;
            mbutton_deleteUnused.UseVisualStyleBackColor = false;
            mbutton_deleteUnused.Click += mbutton_deleteUnused_Click;
            // 
            // mbutton_fillDown
            // 
            mbutton_fillDown.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_fillDown.Image = Properties.Resources.Filldown;
            mbutton_fillDown.ImageAlign = ContentAlignment.TopCenter;
            mbutton_fillDown.Location = new Point(305, 5);
            mbutton_fillDown.Name = "mbutton_fillDown";
            mbutton_fillDown.Size = new Size(60, 96);
            mbutton_fillDown.TabIndex = 39;
            mbutton_fillDown.Text = "Fill Down";
            mbutton_fillDown.TextAlign = ContentAlignment.BottomCenter;
            mbutton_fillDown.UseVisualStyleBackColor = true;
            mbutton_fillDown.Click += mbutton_fillDown_Click;
            // 
            // mbutton_trayVial
            // 
            mbutton_trayVial.Font = new Font("Microsoft Sans Serif", 6.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            mbutton_trayVial.Image = Properties.Resources.testTube;
            mbutton_trayVial.ImageAlign = ContentAlignment.TopCenter;
            mbutton_trayVial.Location = new Point(371, 5);
            mbutton_trayVial.Name = "mbutton_trayVial";
            mbutton_trayVial.Size = new Size(60, 96);
            mbutton_trayVial.TabIndex = 39;
            mbutton_trayVial.Text = "Tray Vial";
            mbutton_trayVial.TextAlign = ContentAlignment.BottomCenter;
            mbutton_trayVial.UseVisualStyleBackColor = true;
            mbutton_trayVial.Click += mbutton_trayVial_Click;
            // 
            // mcheckbox_cycleColumns
            // 
            mcheckbox_cycleColumns.Location = new Point(39, 77);
            mcheckbox_cycleColumns.Name = "mcheckbox_cycleColumns";
            mcheckbox_cycleColumns.Size = new Size(109, 18);
            mcheckbox_cycleColumns.TabIndex = 35;
            mcheckbox_cycleColumns.Text = "Cycle Columns";
            mcheckbox_cycleColumns.UseVisualStyleBackColor = true;
            mcheckbox_cycleColumns.CheckedChanged += mcheckbox_cycleColumns_CheckedChanged;
            // 
            // controlSequenceView
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            Controls.Add(panel2);
            Name = "controlSequenceView";
            Size = new Size(866, 683);
            Controls.SetChildIndex(panel2, 0);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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

        private void mbutton_ccd_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

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
            InvalidateGridView();
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

        /// <summary>
        /// Default constructor.  To use this constructor one must supply the dms view and
        /// sample queue to the respectively named properties.
        /// </summary>
        public controlSequenceView() :
            base()
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

        void mpanel_queue_MouseUp(object sender, MouseEventArgs e)
        {
            m_movingQueueDown = false;
            m_movingQueueUp = false;
        }

        void mpanel_queue_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("MOVING--rect!");
            if (m_movingQueueDown == false && m_movingQueueUp == false)
                return;

            if (mdataGrid_samples.Rows.Count <= 0)
                return;

            var rowHeight = mdataGrid_samples.Rows[0].Height;
            if (m_movingQueueDown)
            {
                //
                // If the mouse has been clicked and moving
                //
                // Size newSize = new Size(mrect_queueRegion.Size.Width, mrect_queueRegion.Size.Height + 1);
                // mrect_queueRegion.Size = newSize;
            }
        }

        /// <summary>
        /// Flag indicating that the user is queuing samples.
        /// </summary>
        private bool m_movingQueueDown;

        private Panel panel2;
        private CheckBox mcheckbox_cycleColumns;
        private Button mbutton_down;
        private Button mbutton_addBlank;
        private Button mbutton_up;
        private Button mbutton_addDMS;
        private Button mbutton_removeSelected;
        private Button mbutton_deleteUnused;
        private Button mbutton_fillDown;
        private Button mbutton_trayVial;
        private Button mbutton_cartColumnDate;
        private Button mbutton_dmsEdit;
        private CheckBox mcheckbox_autoscroll;
        private Button buttonRefresh;
        private bool m_movingQueueUp;

        private bool CanDeQueueSample(int rowID)
        {
            var canMove = false;

            return canMove;
        }

        private bool CanQueueSample(int rowID)
        {
            var canMove = false;

            return canMove;
        }

        void mdataGrid_samples_MouseMove(object sender, MouseEventArgs e)
        {
        }

        void mrect_queueRegion_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("MOVING--rect!");
            if (m_movingQueueDown == false && m_movingQueueUp == false)
                return;

            if (mdataGrid_samples.Rows.Count <= 0)
                return;

            var rowHeight = mdataGrid_samples.Rows[0].Height;
            if (m_movingQueueDown)
            {
                //
                // If the mouse has been clicked and moving
                //
                //Size newSize = new Size(mrect_queueRegion.Size.Width, e.Y + 1);
                // mrect_queueRegion.Size = newSize;
            }
        }

        void mdataGrid_samples_MouseUp(object sender, MouseEventArgs e)
        {
        }

        void mrect_queueRegion_MouseUp(object sender, MouseEventArgs e)
        {
        }

        void mrect_queueRegion_MouseDown(object sender, MouseEventArgs e)
        {
            //
            // See if we selected the queue region
            //
            var point = e.Location;
            var diffYTop = Math.Abs(point.Y);
            var diffYBottom = 0; // Math.Abs(mrect_queueRegion.Size.Height - point.Y);
            var eps = 2;

            if (diffYTop <= eps)
            {
                m_movingQueueUp = true;
            }
            else if (diffYBottom <= eps)
            {
                m_movingQueueDown = true;
            }
            else
            {
                //
                // Nothing was selected
                //
                m_movingQueueUp = false;
                m_movingQueueDown = false;
            }
        }

        #endregion
    }
}