//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/14/2009
//
//*********************************************************************************************************

using System.ComponentModel;
using System.Windows.Forms;
using LcmsNet.SampleQueue.Views;

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
            this.WpfControlHost = new System.Windows.Forms.Integration.ElementHost();
            this.sampleManagerView = new SampleManagerView();
            this.mmenuStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // mmenuStrip
            //
            this.mmenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mmenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mmenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mmenuStrip.MdiWindowListItem = this.fileToolStripMenuItem;
            this.mmenuStrip.Name = "mmenuStrip";
            this.mmenuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
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
            this.fileToolStripMenuItem.Text = "&File";
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
            // WpfControlHost
            //
            this.WpfControlHost.AutoSize = true;
            this.WpfControlHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WpfControlHost.Location = new System.Drawing.Point(0, 0);
            this.WpfControlHost.Name = "WpfControlHost";
            this.WpfControlHost.Size = new System.Drawing.Size(1158, 779);
            this.WpfControlHost.TabIndex = 5;
            this.WpfControlHost.TabStop = false;
            this.WpfControlHost.Text = "WpfControlHost";
            this.WpfControlHost.Child = this.sampleManagerView;
            //
            // formSampleManager2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1158, 779);
            this.Controls.Add(this.WpfControlHost);
            this.Controls.Add(this.mmenuStrip);
            this.MainMenuStrip = this.mmenuStrip;
            this.Name = "formSampleManager2";
            this.Text = "Sample Queue";
            this.mmenuStrip.ResumeLayout(false);
            this.mmenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.Integration.ElementHost WpfControlHost;
        private SampleManagerView sampleManagerView;
    }
}