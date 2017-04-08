using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    partial class controlSampleView2
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
            this.m_sampleContainer = new System.Windows.Forms.Panel();
            this.WpfControlHost = new System.Windows.Forms.Integration.ElementHost();
            this.mdataGrid_samples = new LcmsNet.sampleView();
            this.mcontextMenu_options.SuspendLayout();
            this.m_sampleContainer.SuspendLayout();
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
            this.mcontextMenu_options.Size = new System.Drawing.Size(254, 346);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.undo_16;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(250, 6);
            // 
            // addBlankToolStripMenuItem
            // 
            this.addBlankToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.add;
            this.addBlankToolStripMenuItem.Name = "addBlankToolStripMenuItem";
            this.addBlankToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.addBlankToolStripMenuItem.Text = "Add Blank";
            // 
            // insertBlankIntoUnusedToolStripMenuItem
            // 
            this.insertBlankIntoUnusedToolStripMenuItem.Name = "insertBlankIntoUnusedToolStripMenuItem";
            this.insertBlankIntoUnusedToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.insertBlankIntoUnusedToolStripMenuItem.Text = "Insert Blank Into Unused";
            // 
            // importFromDMSToolStripMenuItem
            // 
            this.importFromDMSToolStripMenuItem.Name = "importFromDMSToolStripMenuItem";
            this.importFromDMSToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.importFromDMSToolStripMenuItem.Text = "Import from DMS";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(250, 6);
            // 
            // addDateCartNameColumnIDToolStripMenuItem
            // 
            this.addDateCartNameColumnIDToolStripMenuItem.Name = "addDateCartNameColumnIDToolStripMenuItem";
            this.addDateCartNameColumnIDToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.addDateCartNameColumnIDToolStripMenuItem.Text = "Add Date, Cart Name, Column ID";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(250, 6);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.deleteSelectedToolStripMenuItem.Text = "Delete Selected";
            // 
            // deleteUnusedToolStripMenuItem
            // 
            this.deleteUnusedToolStripMenuItem.Name = "deleteUnusedToolStripMenuItem";
            this.deleteUnusedToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.deleteUnusedToolStripMenuItem.Text = "Delete Unused";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.clearToolStripMenuItem.Text = "Clear";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(250, 6);
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
            this.showToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // pALTrayToolStripMenuItem
            // 
            this.pALTrayToolStripMenuItem.Name = "pALTrayToolStripMenuItem";
            this.pALTrayToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.pALTrayToolStripMenuItem.Text = "PAL Tray";
            // 
            // pALVialToolStripMenuItem
            // 
            this.pALVialToolStripMenuItem.Name = "pALVialToolStripMenuItem";
            this.pALVialToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.pALVialToolStripMenuItem.Text = "PAL Vial";
            // 
            // volumeToolStripMenuItem
            // 
            this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            this.volumeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.volumeToolStripMenuItem.Text = "Volume";
            // 
            // lCMethodToolStripMenuItem
            // 
            this.lCMethodToolStripMenuItem.Name = "lCMethodToolStripMenuItem";
            this.lCMethodToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.lCMethodToolStripMenuItem.Text = "LC Method";
            // 
            // instrumentMethodToolStripMenuItem
            // 
            this.instrumentMethodToolStripMenuItem.Name = "instrumentMethodToolStripMenuItem";
            this.instrumentMethodToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.instrumentMethodToolStripMenuItem.Text = "Instrument Method";
            // 
            // datasetTypeToolStripMenuItem
            // 
            this.datasetTypeToolStripMenuItem.Name = "datasetTypeToolStripMenuItem";
            this.datasetTypeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.datasetTypeToolStripMenuItem.Text = "Dataset Type";
            // 
            // batchIDToolStripMenuItem
            // 
            this.batchIDToolStripMenuItem.Name = "batchIDToolStripMenuItem";
            this.batchIDToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.batchIDToolStripMenuItem.Text = "Batch ID";
            // 
            // blockToolStripMenuItem
            // 
            this.blockToolStripMenuItem.Name = "blockToolStripMenuItem";
            this.blockToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.blockToolStripMenuItem.Text = "Block";
            // 
            // runOrderToolStripMenuItem
            // 
            this.runOrderToolStripMenuItem.Name = "runOrderToolStripMenuItem";
            this.runOrderToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.runOrderToolStripMenuItem.Text = "Run Order";
            // 
            // previewThroughputToolStripMenuItem
            // 
            this.previewThroughputToolStripMenuItem.Name = "previewThroughputToolStripMenuItem";
            this.previewThroughputToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.previewThroughputToolStripMenuItem.Text = "Preview Throughput";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
            // 
            // randomizeToolStripMenuItem
            // 
            this.randomizeToolStripMenuItem.Name = "randomizeToolStripMenuItem";
            this.randomizeToolStripMenuItem.Size = new System.Drawing.Size(253, 26);
            this.randomizeToolStripMenuItem.Text = "Randomize";
            // 
            // m_sampleContainer
            // 
            this.m_sampleContainer.AutoSize = true;
            this.m_sampleContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_sampleContainer.Controls.Add(this.WpfControlHost);
            this.m_sampleContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_sampleContainer.Location = new System.Drawing.Point(3, 3);
            this.m_sampleContainer.Name = "m_sampleContainer";
            this.m_sampleContainer.Size = new System.Drawing.Size(2161, 988);
            this.m_sampleContainer.TabIndex = 19;
            // 
            // WpfControlHost
            // 
            this.WpfControlHost.AutoSize = true;
            this.WpfControlHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WpfControlHost.Location = new System.Drawing.Point(0, 0);
            this.WpfControlHost.Margin = new System.Windows.Forms.Padding(2);
            this.WpfControlHost.Name = "WpfControlHost";
            this.WpfControlHost.Size = new System.Drawing.Size(2161, 988);
            this.WpfControlHost.TabIndex = 0;
            this.WpfControlHost.TabStop = false;
            this.WpfControlHost.Text = "WPFControlHost";
            this.WpfControlHost.Child = this.mdataGrid_samples;
            this.mdataGrid_samples.TabIndex = 0;
            this.mdataGrid_samples.IsTabStop = true;
            // 
            // controlSampleView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.m_sampleContainer);
            this.Name = "controlSampleView2";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(2167, 994);
            this.mcontextMenu_options.ResumeLayout(false);
            this.m_sampleContainer.ResumeLayout(false);
            this.m_sampleContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ContextMenuStrip mcontextMenu_options;
        private ToolStripMenuItem randomizeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
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
        private ToolStripMenuItem blockToolStripMenuItem;
        private ToolStripMenuItem runOrderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem addDateCartNameColumnIDToolStripMenuItem;
        private Panel m_sampleContainer;
        private System.Windows.Forms.Integration.ElementHost WpfControlHost;
        private sampleView mdataGrid_samples;
    }
}
