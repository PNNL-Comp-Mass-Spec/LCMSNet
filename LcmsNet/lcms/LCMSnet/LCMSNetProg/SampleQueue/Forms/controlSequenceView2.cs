
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
    public sealed class controlSequenceView2 : UserControl
    {
        /// <summary>
        /// Delegate defining when status updates are available in batches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messages"></param>
        public delegate void DelegateStatusUpdates(object sender, List<string> messages);

        #region Members

        private Label mlabel_name;

        internal controlSampleView2 ControlSampleView => controlSampleView;

        #endregion

        /// <summary>
        /// Windows Forms Designer.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(controlSequenceView2));
            this.mlabel_name = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.mcheckbox_autoscroll = new System.Windows.Forms.CheckBox();
            this.mbutton_dmsEdit = new System.Windows.Forms.Button();
            this.mbutton_cartColumnDate = new System.Windows.Forms.Button();
            this.mbutton_down = new System.Windows.Forms.Button();
            this.mbutton_addBlank = new System.Windows.Forms.Button();
            this.mbutton_up = new System.Windows.Forms.Button();
            this.mbutton_addDMS = new System.Windows.Forms.Button();
            this.mbutton_removeSelected = new System.Windows.Forms.Button();
            this.mbutton_deleteUnused = new System.Windows.Forms.Button();
            this.mbutton_fillDown = new System.Windows.Forms.Button();
            this.mbutton_trayVial = new System.Windows.Forms.Button();
            this.mcheckbox_cycleColumns = new System.Windows.Forms.CheckBox();
            this.mToolTipManager = new System.Windows.Forms.ToolTip(this.components);
            this.controlSampleView = new LcmsNet.SampleQueue.Forms.controlSampleView2();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
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
            this.panel2.Location = new System.Drawing.Point(0, 555);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(866, 128);
            this.panel2.TabIndex = 20;
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
            // mcheckbox_autoscroll
            // 
            this.mcheckbox_autoscroll.AutoSize = true;
            this.mcheckbox_autoscroll.Checked = true;
            this.mcheckbox_autoscroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcheckbox_autoscroll.Location = new System.Drawing.Point(759, 96);
            this.mcheckbox_autoscroll.Margin = new System.Windows.Forms.Padding(4);
            this.mcheckbox_autoscroll.Name = "mcheckbox_autoscroll";
            this.mcheckbox_autoscroll.Size = new System.Drawing.Size(75, 17);
            this.mcheckbox_autoscroll.TabIndex = 42;
            this.mcheckbox_autoscroll.Text = "Auto-scroll";
            this.mcheckbox_autoscroll.UseVisualStyleBackColor = true;
            this.mcheckbox_autoscroll.CheckedChanged += new System.EventHandler(this.mcheckbox_autoscroll_CheckedChanged);
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
            // mbutton_down
            // 
            this.mbutton_down.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_down.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.mbutton_down.Location = new System.Drawing.Point(779, 5);
            this.mbutton_down.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_down.Name = "mbutton_down";
            this.mbutton_down.Size = new System.Drawing.Size(80, 118);
            this.mbutton_down.TabIndex = 31;
            this.mbutton_down.UseVisualStyleBackColor = true;
            this.mbutton_down.Click += new System.EventHandler(this.mbutton_down_Click);
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
            // mbutton_up
            // 
            this.mbutton_up.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_up.Image = global::LcmsNet.Properties.Resources.Button_Up_16;
            this.mbutton_up.Location = new System.Drawing.Point(691, 4);
            this.mbutton_up.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_up.Name = "mbutton_up";
            this.mbutton_up.Size = new System.Drawing.Size(80, 118);
            this.mbutton_up.TabIndex = 30;
            this.mbutton_up.UseVisualStyleBackColor = true;
            this.mbutton_up.Click += new System.EventHandler(this.mbutton_up_Click);
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
            // controlSampleView
            // 
            this.controlSampleView.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("controlSampleView.AutoSamplerMethods")));
            this.controlSampleView.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("controlSampleView.AutoSamplerTrays")));
            this.controlSampleView.AutoSize = true;
            this.controlSampleView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlSampleView.BackColor = System.Drawing.Color.White;
            this.controlSampleView.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.Resort;
            this.controlSampleView.DMSView = null;
            this.controlSampleView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlSampleView.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("controlSampleView.InstrumentMethods")));
            this.controlSampleView.Location = new System.Drawing.Point(0, 0);
            this.controlSampleView.Margin = new System.Windows.Forms.Padding(4);
            this.controlSampleView.Name = "controlSampleView";
            this.controlSampleView.Padding = new System.Windows.Forms.Padding(4);
            this.controlSampleView.SampleQueue = null;
            this.controlSampleView.Size = new System.Drawing.Size(866, 683);
            this.controlSampleView.TabIndex = 0;
            // 
            // controlSequenceView2
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.controlSampleView);
            this.Name = "controlSequenceView2";
            this.Size = new System.Drawing.Size(866, 683);
            this.Load += new System.EventHandler(this.controlSequenceView2_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void mbutton_addBlank_Click(object sender, EventArgs e)
        {
            // ToDo: AddNewSample(false);
        }

        private void mbutton_addDMS_Click(object sender, EventArgs e)
        {
            // ToDo: ShowDMSView();
        }

        private void mbutton_removeSelected_Click(object sender, EventArgs e)
        {
            // ToDo:  RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone);
        }

        private void mbutton_fillDown_Click(object sender, EventArgs e)
        {
            // ToDo: FillDown();
        }

        private void mbutton_trayVial_Click(object sender, EventArgs e)
        {
            // ToDo: EditTrayAndVial();
        }

        private void mbutton_randomize_Click(object sender, EventArgs e)
        {
            // ToDo: RandomizeSelectedSamples();
        }

        private void mbutton_down_Click(object sender, EventArgs e)
        {
            // ToDo: MoveSelectedSamples(1, enumMoveSampleType.Sequence);
        }

        private void mbutton_up_Click(object sender, EventArgs e)
        {
            // ToDo: MoveSelectedSamples(-1, enumMoveSampleType.Sequence);
        }

        private void mcheckbox_cycleColumns_CheckedChanged(object sender, EventArgs e)
        {
            var handling = enumColumnDataHandling.LeaveAlone;
            if (mcheckbox_cycleColumns.Checked)
            {
                handling = enumColumnDataHandling.Resort;
            }
            // ToDo: ColumnHandling = handling;

            // ToDo: IterateThroughColumns = mcheckbox_cycleColumns.Checked;
        }

        private void mcheckbox_autoscroll_CheckedChanged(object sender, EventArgs e)
        {
            // ToDo: m_autoscroll = mcheckbox_autoscroll.Checked;
        }

        private void mbutton_deleteUnused_Click(object sender, EventArgs e)
        {
            // ToDo: RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone);
        }

        private void mbutton_cartColumnDate_Click(object sender, EventArgs e)
        {
            // ToDo: AddDateCartnameColumnIDToDatasetName();
        }

        private void mbutton_refresh_Click(object sender, EventArgs e)
        {
            // ToDo: InvalidateGridView(true);
        }


        private void mbutton_dmsEdit_Click(object sender, EventArgs e)
        {
            // ToDo: EditDMSData();
        }

        private void m_selector_Load(object sender, EventArgs e)
        {
        }

        #region Constructors and Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public controlSequenceView2(formDMSView dmsView, classSampleQueue sampleQueue)           
        {
            InitializeComponent();

            UpdateToolTips();

            /*
             * DisplayColumn(CONST_COLUMN_PAL_TRAY, true);
            DisplayColumn(CONST_COLUMN_PAL_VIAL, true);
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);
            */
            // ToDo: EnableQueueing(true);

            foreach (var column in classCartConfiguration.Columns)
            {
                // ToDo: column.StatusChanged += column_StatusChanged;
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
        public controlSequenceView2()
        {
            InitializeComponent();
            // ToDo: DisplayColumn(CONST_COLUMN_PAL_TRAY, true);
            // ToDo: DisplayColumn(CONST_COLUMN_PAL_VIAL, true);
            // ToDo: DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
            // ToDo: DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);

            // ToDo: EnableQueueing(true);

            if (classCartConfiguration.Columns != null)
            {
                foreach (var column in classCartConfiguration.Columns)
                {
                    // ToDo: column.StatusChanged += column_StatusChanged;
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

      
        /// <summary>
        /// Flag indicating that the user is queuing samples.
        /// </summary>

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
        private ToolTip mToolTipManager;
        private controlSampleView2 controlSampleView;
        private System.ComponentModel.IContainer components;

        #endregion

        private void controlSequenceView2_Load(object sender, EventArgs e)
        {

        }
    }
}