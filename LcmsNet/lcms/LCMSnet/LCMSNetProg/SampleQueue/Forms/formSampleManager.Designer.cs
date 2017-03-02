//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/14/2009
//
/* Last modified 01/14/2009
 *
 *
 *
 */
//*********************************************************************************************************

using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    partial class formSampleManager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(formSampleManager));
            this.mtabControl_sampleViews = new System.Windows.Forms.TabControl();
            this.mtabPage_sequenceView = new System.Windows.Forms.TabPage();
            this.mcontrol_sequenceView = new LcmsNet.SampleQueue.Forms.controlSequenceView();
            this.mtabPage_columnView = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.mcontrol_column1 = new LcmsNet.SampleQueue.Forms.controlColumnView();
            this.mcontrol_column2 = new LcmsNet.SampleQueue.Forms.controlColumnView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mcontrol_column3 = new LcmsNet.SampleQueue.Forms.controlColumnView();
            this.mcontrol_column4 = new LcmsNet.SampleQueue.Forms.controlColumnView();
            this.mmenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importQueueLcmsNetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queueToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queueToExcaliburToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mpicture_preview = new System.Windows.Forms.PictureBox();
            this.mbutton_stop = new System.Windows.Forms.Button();
            this.mbutton_run = new System.Windows.Forms.Button();
            this.mbutton_undo = new System.Windows.Forms.Button();
            this.mbutton_redo = new System.Windows.Forms.Button();
            this.mtabControl_sampleViews.SuspendLayout();
            this.mtabPage_sequenceView.SuspendLayout();
            this.mtabPage_columnView.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mmenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).BeginInit();
            this.SuspendLayout();
            //
            // mtabControl_sampleViews
            //
            this.mtabControl_sampleViews.Controls.Add(this.mtabPage_sequenceView);
            this.mtabControl_sampleViews.Controls.Add(this.mtabPage_columnView);
            this.mtabControl_sampleViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mtabControl_sampleViews.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtabControl_sampleViews.Location = new System.Drawing.Point(0, 0);
            this.mtabControl_sampleViews.Name = "mtabControl_sampleViews";
            this.mtabControl_sampleViews.SelectedIndex = 0;
            this.mtabControl_sampleViews.Size = new System.Drawing.Size(1158, 688);
            this.mtabControl_sampleViews.TabIndex = 1;
            //
            // mtabPage_sequenceView
            //
            this.mtabPage_sequenceView.Controls.Add(this.mcontrol_sequenceView);
            this.mtabPage_sequenceView.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_sequenceView.Name = "mtabPage_sequenceView";
            this.mtabPage_sequenceView.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_sequenceView.Size = new System.Drawing.Size(1150, 662);
            this.mtabPage_sequenceView.TabIndex = 0;
            this.mtabPage_sequenceView.Text = "Sequence View";
            this.mtabPage_sequenceView.UseVisualStyleBackColor = true;
            //
            // mcontrol_sequenceView
            //
            //this.mcontrol_sequenceView.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.AutoSamplerMethods")));
            //this.mcontrol_sequenceView.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.AutoSamplerTrays")));
            this.mcontrol_sequenceView.BackColor = System.Drawing.Color.White;
            this.mcontrol_sequenceView.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.Resort;
            this.mcontrol_sequenceView.DMSView = null;
            this.mcontrol_sequenceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_sequenceView.Enabled = false;
            this.mcontrol_sequenceView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          //  this.mcontrol_sequenceView.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.InstrumentMethods")));
            this.mcontrol_sequenceView.Location = new System.Drawing.Point(3, 3);
            this.mcontrol_sequenceView.Name = "mcontrol_sequenceView";
            this.mcontrol_sequenceView.Padding = new System.Windows.Forms.Padding(3);
            this.mcontrol_sequenceView.SampleQueue = null;
            this.mcontrol_sequenceView.Size = new System.Drawing.Size(1144, 656);
            this.mcontrol_sequenceView.TabIndex = 2;
            this.mcontrol_sequenceView.Load += new System.EventHandler(this.mcontrol_sequenceView_Load);
            //
            // mtabPage_columnView
            //
            this.mtabPage_columnView.Controls.Add(this.splitContainer3);
            this.mtabPage_columnView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtabPage_columnView.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_columnView.Name = "mtabPage_columnView";
            this.mtabPage_columnView.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_columnView.Size = new System.Drawing.Size(1150, 662);
            this.mtabPage_columnView.TabIndex = 1;
            this.mtabPage_columnView.Text = "Column View";
            this.mtabPage_columnView.UseVisualStyleBackColor = true;
            //
            // splitContainer3
            //
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            //
            // splitContainer3.Panel1
            //
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            //
            // splitContainer3.Panel2
            //
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer3.Size = new System.Drawing.Size(1144, 656);
            this.splitContainer3.SplitterDistance = 568;
            this.splitContainer3.TabIndex = 9;
            //
            // splitContainer2
            //
            this.splitContainer2.BackColor = System.Drawing.Color.Silver;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            //
            // splitContainer2.Panel1
            //
            this.splitContainer2.Panel1.Controls.Add(this.mcontrol_column1);
            //
            // splitContainer2.Panel2
            //
            this.splitContainer2.Panel2.Controls.Add(this.mcontrol_column2);
            this.splitContainer2.Size = new System.Drawing.Size(568, 656);
            this.splitContainer2.SplitterDistance = 281;
            this.splitContainer2.TabIndex = 8;
            //
            // mcontrol_column1
            //
            //this.mcontrol_column1.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column1.AutoSamplerMethods")));
            //this.mcontrol_column1.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column1.AutoSamplerTrays")));
            this.mcontrol_column1.BackColor = System.Drawing.Color.White;
            //this.mcontrol_column1.Column = ((LcmsNetDataClasses.Configuration.classColumnData)(resources.GetObject("mcontrol_column1.Column")));
            this.mcontrol_column1.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.CreateUnused;
            this.mcontrol_column1.DMSView = null;
            this.mcontrol_column1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_column1.Enabled = false;
            //this.mcontrol_column1.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column1.InstrumentMethods")));
            this.mcontrol_column1.Location = new System.Drawing.Point(0, 0);
            this.mcontrol_column1.Name = "mcontrol_column1";
            this.mcontrol_column1.Padding = new System.Windows.Forms.Padding(3);
            this.mcontrol_column1.SampleQueue = null;
            this.mcontrol_column1.Size = new System.Drawing.Size(281, 656);
            this.mcontrol_column1.TabIndex = 1;
            //
            // mcontrol_column2
            //
            //this.mcontrol_column2.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column2.AutoSamplerMethods")));
            //this.mcontrol_column2.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column2.AutoSamplerTrays")));
            this.mcontrol_column2.BackColor = System.Drawing.Color.White;
            //this.mcontrol_column2.Column = ((LcmsNetDataClasses.Configuration.classColumnData)(resources.GetObject("mcontrol_column2.Column")));
            this.mcontrol_column2.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.CreateUnused;
            this.mcontrol_column2.DMSView = null;
            this.mcontrol_column2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_column2.Enabled = false;
            //this.mcontrol_column2.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column2.InstrumentMethods")));
            this.mcontrol_column2.Location = new System.Drawing.Point(0, 0);
            this.mcontrol_column2.Name = "mcontrol_column2";
            this.mcontrol_column2.Padding = new System.Windows.Forms.Padding(3);
            this.mcontrol_column2.SampleQueue = null;
            this.mcontrol_column2.Size = new System.Drawing.Size(283, 656);
            this.mcontrol_column2.TabIndex = 2;
            //
            // splitContainer1
            //
            this.splitContainer1.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.mcontrol_column3);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.mcontrol_column4);
            this.splitContainer1.Size = new System.Drawing.Size(572, 656);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 7;
            //
            // mcontrol_column3
            //
            //this.mcontrol_column3.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column3.AutoSamplerMethods")));
            //this.mcontrol_column3.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column3.AutoSamplerTrays")));
            this.mcontrol_column3.BackColor = System.Drawing.Color.White;
            //this.mcontrol_column3.Column = ((LcmsNetDataClasses.Configuration.classColumnData)(resources.GetObject("mcontrol_column3.Column")));
            this.mcontrol_column3.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.CreateUnused;
            this.mcontrol_column3.DMSView = null;
            this.mcontrol_column3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_column3.Enabled = false;
            //this.mcontrol_column3.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column3.InstrumentMethods")));
            this.mcontrol_column3.Location = new System.Drawing.Point(0, 0);
            this.mcontrol_column3.Name = "mcontrol_column3";
            this.mcontrol_column3.Padding = new System.Windows.Forms.Padding(3);
            this.mcontrol_column3.SampleQueue = null;
            this.mcontrol_column3.Size = new System.Drawing.Size(275, 656);
            this.mcontrol_column3.TabIndex = 5;
            //
            // mcontrol_column4
            //
            //this.mcontrol_column4.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column4.AutoSamplerMethods")));
            //this.mcontrol_column4.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column4.AutoSamplerTrays")));
            this.mcontrol_column4.BackColor = System.Drawing.Color.White;
            //this.mcontrol_column4.Column = ((LcmsNetDataClasses.Configuration.classColumnData)(resources.GetObject("mcontrol_column4.Column")));
            this.mcontrol_column4.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.CreateUnused;
            this.mcontrol_column4.DMSView = null;
            this.mcontrol_column4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_column4.Enabled = false;
            //this.mcontrol_column4.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_column4.InstrumentMethods")));
            this.mcontrol_column4.Location = new System.Drawing.Point(0, 0);
            this.mcontrol_column4.Name = "mcontrol_column4";
            this.mcontrol_column4.Padding = new System.Windows.Forms.Padding(3);
            this.mcontrol_column4.SampleQueue = null;
            this.mcontrol_column4.Size = new System.Drawing.Size(293, 656);
            this.mcontrol_column4.TabIndex = 2;
            //
            // mmenuStrip
            //
            this.mmenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mmenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mmenuStrip.MdiWindowListItem = this.fileToolStripMenuItem;
            this.mmenuStrip.Name = "mmenuStrip";
            this.mmenuStrip.Size = new System.Drawing.Size(1158, 24);
            this.mmenuStrip.TabIndex = 4;
            this.mmenuStrip.Text = "menuStrip1";
            this.mmenuStrip.Visible = false;
            //
            // fileToolStripMenuItem
            //
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importQueueLcmsNetToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.MergeIndex = 0;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            //
            // importQueueLcmsNetToolStripMenuItem
            //
            this.importQueueLcmsNetToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.importQueueLcmsNetToolStripMenuItem.MergeIndex = 0;
            this.importQueueLcmsNetToolStripMenuItem.Name = "importQueueLcmsNetToolStripMenuItem";
            this.importQueueLcmsNetToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.importQueueLcmsNetToolStripMenuItem.Text = "Open";
            this.importQueueLcmsNetToolStripMenuItem.Click += new System.EventHandler(this.ImportQueue);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator3.MergeIndex = 1;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(111, 6);
            //
            // saveToolStripMenuItem1
            //
            this.saveToolStripMenuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem1.MergeIndex = 2;
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            //
            // saveAsToolStripMenuItem
            //
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 3;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator1.MergeIndex = 4;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(111, 6);
            //
            // exportToolStripMenuItem
            //
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queueToXMLToolStripMenuItem,
            this.toCSVToolStripMenuItem,
            this.queueToExcaliburToolStripMenuItem,
            this.toolStripSeparator4,
            this.tToolStripMenuItem});
            this.exportToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.exportToolStripMenuItem.MergeIndex = 5;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exportToolStripMenuItem.Text = "Export";
            //
            // queueToXMLToolStripMenuItem
            //
            this.queueToXMLToolStripMenuItem.Name = "queueToXMLToolStripMenuItem";
            this.queueToXMLToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.queueToXMLToolStripMenuItem.Text = "to LCMS VB6 (XML)";
            this.queueToXMLToolStripMenuItem.Click += new System.EventHandler(this.queueToXMLToolStripMenuItem_Click);
            //
            // toCSVToolStripMenuItem
            //
            this.toCSVToolStripMenuItem.Name = "toCSVToolStripMenuItem";
            this.toCSVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toCSVToolStripMenuItem.Text = "to CSV";
            this.toCSVToolStripMenuItem.Click += new System.EventHandler(this.queueAsCSVToolStripMenuItem_Click);
            //
            // queueToExcaliburToolStripMenuItem
            //
            this.queueToExcaliburToolStripMenuItem.Name = "queueToExcaliburToolStripMenuItem";
            this.queueToExcaliburToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.queueToExcaliburToolStripMenuItem.Text = "to XCalibur";
            this.queueToExcaliburToolStripMenuItem.Click += new System.EventHandler(this.queueAsCSVToolStripMenuItem_Click_1);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            //
            // tToolStripMenuItem
            //
            this.tToolStripMenuItem.Name = "tToolStripMenuItem";
            this.tToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.tToolStripMenuItem.Text = "MRM Files";
            this.tToolStripMenuItem.Click += new System.EventHandler(this.exportMRMFilesToolStripMenuItem_Click);
            //
            // groupBox1
            //
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.mpicture_preview);
            this.groupBox1.Controls.Add(this.mbutton_stop);
            this.groupBox1.Controls.Add(this.mbutton_run);
            this.groupBox1.Controls.Add(this.mbutton_undo);
            this.groupBox1.Controls.Add(this.mbutton_redo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 688);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1158, 91);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            //
            // mpicture_preview
            //
            this.mpicture_preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mpicture_preview.Location = new System.Drawing.Point(210, 12);
            this.mpicture_preview.Name = "mpicture_preview";
            this.mpicture_preview.Size = new System.Drawing.Size(700, 73);
            this.mpicture_preview.TabIndex = 23;
            this.mpicture_preview.TabStop = false;
            //
            // mbutton_stop
            //
            this.mbutton_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_stop.BackColor = System.Drawing.Color.White;
            this.mbutton_stop.Enabled = false;
            this.mbutton_stop.Image = global::LcmsNet.Properties.Resources.notOK;
            this.mbutton_stop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_stop.Location = new System.Drawing.Point(1034, 19);
            this.mbutton_stop.Name = "mbutton_stop";
            this.mbutton_stop.Size = new System.Drawing.Size(112, 66);
            this.mbutton_stop.TabIndex = 18;
            this.mbutton_stop.Text = "Stop";
            this.mbutton_stop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_stop.UseVisualStyleBackColor = false;
            this.mbutton_stop.Click += new System.EventHandler(this.mbutton_stop_Click);
            //
            // mbutton_run
            //
            this.mbutton_run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_run.BackColor = System.Drawing.Color.White;
            this.mbutton_run.Enabled = false;
            this.mbutton_run.Image = global::LcmsNet.Properties.Resources.ok;
            this.mbutton_run.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_run.Location = new System.Drawing.Point(916, 19);
            this.mbutton_run.Name = "mbutton_run";
            this.mbutton_run.Size = new System.Drawing.Size(112, 66);
            this.mbutton_run.TabIndex = 17;
            this.mbutton_run.Text = "Run";
            this.mbutton_run.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_run.UseVisualStyleBackColor = false;
            this.mbutton_run.Click += new System.EventHandler(this.mbutton_run_Click);
            //
            // mbutton_undo
            //
            this.mbutton_undo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_undo.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_undo.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_undo.Image = global::LcmsNet.Properties.Resources.undo_16;
            this.mbutton_undo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_undo.Location = new System.Drawing.Point(6, 19);
            this.mbutton_undo.Name = "mbutton_undo";
            this.mbutton_undo.Size = new System.Drawing.Size(83, 66);
            this.mbutton_undo.TabIndex = 15;
            this.mbutton_undo.Text = "Undo";
            this.mbutton_undo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_undo.UseVisualStyleBackColor = false;
            this.mbutton_undo.Click += new System.EventHandler(this.mbutton_undo_Click);
            //
            // mbutton_redo
            //
            this.mbutton_redo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_redo.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_redo.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_redo.Image = global::LcmsNet.Properties.Resources.redo_16;
            this.mbutton_redo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_redo.Location = new System.Drawing.Point(95, 19);
            this.mbutton_redo.Name = "mbutton_redo";
            this.mbutton_redo.Size = new System.Drawing.Size(83, 66);
            this.mbutton_redo.TabIndex = 16;
            this.mbutton_redo.Text = "redo";
            this.mbutton_redo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_redo.UseVisualStyleBackColor = false;
            this.mbutton_redo.Click += new System.EventHandler(this.mbutton_redo_Click);
            //
            // formSampleManager
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1158, 779);
            this.Controls.Add(this.mtabControl_sampleViews);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mmenuStrip);
            this.MainMenuStrip = this.mmenuStrip;
            this.Name = "formSampleManager";
            this.Text = "Sample Queue";
            this.mtabControl_sampleViews.ResumeLayout(false);
            this.mtabPage_sequenceView.ResumeLayout(false);
            this.mtabPage_columnView.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.mmenuStrip.ResumeLayout(false);
            this.mmenuStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TabControl mtabControl_sampleViews;
        private TabPage mtabPage_sequenceView;
        private TabPage mtabPage_columnView;
        private controlSequenceView mcontrol_sequenceView;
          private MenuStrip mmenuStrip;
          private ToolStripMenuItem fileToolStripMenuItem;
                 private ToolStripMenuItem importQueueLcmsNetToolStripMenuItem;
                      private ToolStripMenuItem exportToolStripMenuItem;
                      private ToolStripMenuItem tToolStripMenuItem;
                      private ToolStripSeparator toolStripSeparator4;
                      private ToolStripMenuItem queueToXMLToolStripMenuItem;
                      private ToolStripSeparator toolStripSeparator3;
                      private ToolStripMenuItem saveAsToolStripMenuItem;
                      private ToolStripMenuItem queueToExcaliburToolStripMenuItem;
                      private ToolStripSeparator toolStripSeparator1;
                      private ToolStripMenuItem toCSVToolStripMenuItem;
                      private ToolStripMenuItem saveToolStripMenuItem1;
                      private GroupBox groupBox1;
                      private Button mbutton_undo;
                      private Button mbutton_redo;
                      private Button mbutton_stop;
                      private Button mbutton_run;
                      private SplitContainer splitContainer1;
                      private controlColumnView mcontrol_column3;
                      private controlColumnView mcontrol_column4;
                      private SplitContainer splitContainer2;
                      private controlColumnView mcontrol_column1;
                      private controlColumnView mcontrol_column2;
                      private SplitContainer splitContainer3;
                      private PictureBox mpicture_preview;

    }
}