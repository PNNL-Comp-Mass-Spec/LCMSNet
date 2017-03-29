//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/14/2009
//
//*********************************************************************************************************

using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    partial class formSampleManager2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formSampleManager2));
            this.mtabControl_sampleViews = new System.Windows.Forms.TabControl();
            this.mtabPage_sequenceView = new System.Windows.Forms.TabPage();
            this.mcontrol_sequenceView = new LcmsNet.SampleQueue.Forms.controlSequenceView2();
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
            this.mmenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // mtabControl_sampleViews
            // 
            this.mtabControl_sampleViews.Controls.Add(this.mtabPage_sequenceView);
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
            this.mtabPage_sequenceView.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.mtabPage_sequenceView.Size = new System.Drawing.Size(1150, 662);
            this.mtabPage_sequenceView.TabIndex = 0;
            this.mtabPage_sequenceView.Text = "Sequence View";
            this.mtabPage_sequenceView.UseVisualStyleBackColor = true;
            // 
            // mcontrol_sequenceView
            // 
            //ToDo: this.mcontrol_sequenceView.AutoSamplerMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.AutoSamplerMethods")));
            //ToDo: this.mcontrol_sequenceView.AutoSamplerTrays = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.AutoSamplerTrays")));
            this.mcontrol_sequenceView.BackColor = System.Drawing.Color.White;
            //ToDo: this.mcontrol_sequenceView.ColumnHandling = LcmsNet.SampleQueue.enumColumnDataHandling.Resort;
            this.mcontrol_sequenceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_sequenceView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //ToDo: this.mcontrol_sequenceView.InstrumentMethods = ((System.Collections.Generic.List<string>)(resources.GetObject("mcontrol_sequenceView.InstrumentMethods")));
            this.mcontrol_sequenceView.Location = new System.Drawing.Point(3, 3);
            this.mcontrol_sequenceView.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.mcontrol_sequenceView.Name = "mcontrol_sequenceView";
            this.mcontrol_sequenceView.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.mcontrol_sequenceView.Size = new System.Drawing.Size(1144, 656);
            this.mcontrol_sequenceView.TabIndex = 2;
            this.mcontrol_sequenceView.Load += new System.EventHandler(this.mcontrol_sequenceView_Load);
            // 
            // mmenuStrip
            // 
            this.mmenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            // formSampleManager2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1158, 779);
            this.Controls.Add(this.mtabControl_sampleViews);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mmenuStrip);
            this.MainMenuStrip = this.mmenuStrip;
            this.Name = "formSampleManager2";
            this.Text = "Sample Queue";
            this.mtabControl_sampleViews.ResumeLayout(false);
            this.mtabPage_sequenceView.ResumeLayout(false);
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
        private controlSequenceView2 mcontrol_sequenceView;
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
        private PictureBox mpicture_preview;
    }
}