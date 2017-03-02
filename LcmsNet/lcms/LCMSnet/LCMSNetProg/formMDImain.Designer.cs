namespace LcmsNet
{
    partial class formMDImain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                Shutdown();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMDImain));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.mtoolButton_showSampleQueue = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFludics = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSimulate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparatorSimulator = new System.Windows.Forms.ToolStripSeparator();
            this.mbutton_sampleProgress = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mbutton_MethodEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolButton_notificationSystem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.mtoolButton_showMessages = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mbutton_cartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.mbutton_reportError = new System.Windows.Forms.ToolStripButton();
            this.mstatusStrip_status = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.mstatusStrip_status.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // toolStrip
            //
            this.toolStrip.BackColor = System.Drawing.Color.White;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mtoolButton_showSampleQueue,
            this.toolStripSeparator5,
            this.toolStripButtonFludics,
            this.toolStripSeparator7,
            this.toolStripButtonSimulate,
            this.toolStripSeparatorSimulator,
            this.mbutton_sampleProgress,
            this.toolStripSeparator2,
            this.mbutton_MethodEditor,
            this.toolStripSeparator4,
            this.toolStripButton2,
            this.toolStripSeparator8,
            this.toolButton_notificationSystem,
            this.toolStripSeparator10,
            this.mtoolButton_showMessages,
            this.toolStripSeparator1,
            this.mbutton_cartButton,
            this.toolStripButtonAbout,
            this.mbutton_reportError});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1070, 32);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "ToolStrip";
            //
            // mtoolButton_showSampleQueue
            //
            this.mtoolButton_showSampleQueue.Image = global::LcmsNet.Properties.Resources.vialReal;
            this.mtoolButton_showSampleQueue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mtoolButton_showSampleQueue.Name = "mtoolButton_showSampleQueue";
            this.mtoolButton_showSampleQueue.Size = new System.Drawing.Size(74, 29);
            this.mtoolButton_showSampleQueue.Text = " Queue";
            this.mtoolButton_showSampleQueue.Click += new System.EventHandler(this.mtoolButton_showSampleQueue_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 32);
            //
            // toolStripButtonFludics
            //
            this.toolStripButtonFludics.Image = global::LcmsNet.Properties.Resources.water;
            this.toolStripButtonFludics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFludics.Name = "toolStripButtonFludics";
            this.toolStripButtonFludics.Size = new System.Drawing.Size(72, 29);
            this.toolStripButtonFludics.Text = "Design";
            this.toolStripButtonFludics.ToolTipText = "Cart / Instrument Setup";
            this.toolStripButtonFludics.Click += new System.EventHandler(this.toolStripButtonFludics_Click);
            //
            // toolStripSeparator7
            //
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 32);
            //
            // toolStripButtonSimulate
            //
            this.toolStripButtonSimulate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSimulate.Image")));
            this.toolStripButtonSimulate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSimulate.Name = "toolStripButtonSimulate";
            this.toolStripButtonSimulate.Size = new System.Drawing.Size(82, 29);
            this.toolStripButtonSimulate.Text = "Simulate";
            this.toolStripButtonSimulate.Click += new System.EventHandler(this.toolStripButtonSimulate_Click);
            //
            // toolStripSeparatorSimulator
            //
            this.toolStripSeparatorSimulator.Name = "toolStripSeparatorSimulator";
            this.toolStripSeparatorSimulator.Size = new System.Drawing.Size(6, 32);
            //
            // mbutton_sampleProgress
            //
            this.mbutton_sampleProgress.Image = global::LcmsNet.Properties.Resources.progress;
            this.mbutton_sampleProgress.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbutton_sampleProgress.Name = "mbutton_sampleProgress";
            this.mbutton_sampleProgress.Size = new System.Drawing.Size(81, 29);
            this.mbutton_sampleProgress.Text = "Progress";
            this.mbutton_sampleProgress.Click += new System.EventHandler(this.mbutton_sampleProgress_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
            //
            // mbutton_MethodEditor
            //
            this.mbutton_MethodEditor.Image = global::LcmsNet.Properties.Resources.MethodEditor;
            this.mbutton_MethodEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbutton_MethodEditor.Name = "mbutton_MethodEditor";
            this.mbutton_MethodEditor.Size = new System.Drawing.Size(83, 29);
            this.mbutton_MethodEditor.Text = "Methods";
            this.mbutton_MethodEditor.Click += new System.EventHandler(this.mbutton_MethodEditor_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 32);
            //
            // toolStripButton2
            //
            this.toolStripButton2.Image = global::LcmsNet.Properties.Resources.pumps2;
            this.toolStripButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(73, 29);
            this.toolStripButton2.Text = "Pumps";
            this.toolStripButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            //
            // toolStripSeparator8
            //
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 32);
            //
            // toolButton_notificationSystem
            //
            this.toolButton_notificationSystem.Image = global::LcmsNet.Properties.Resources.notify;
            this.toolButton_notificationSystem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolButton_notificationSystem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolButton_notificationSystem.Name = "toolButton_notificationSystem";
            this.toolButton_notificationSystem.Size = new System.Drawing.Size(104, 29);
            this.toolButton_notificationSystem.Text = "Notifications";
            this.toolButton_notificationSystem.Click += new System.EventHandler(this.toolButton_notificationSystem_Click);
            //
            // toolStripSeparator10
            //
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 32);
            //
            // mtoolButton_showMessages
            //
            this.mtoolButton_showMessages.Image = global::LcmsNet.Properties.Resources.StatusMessages;
            this.mtoolButton_showMessages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mtoolButton_showMessages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mtoolButton_showMessages.Name = "mtoolButton_showMessages";
            this.mtoolButton_showMessages.Size = new System.Drawing.Size(87, 29);
            this.mtoolButton_showMessages.Text = "Messages";
            this.mtoolButton_showMessages.Click += new System.EventHandler(this.mtoolButton_showMessages_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            //
            // mbutton_cartButton
            //
            this.mbutton_cartButton.Image = global::LcmsNet.Properties.Resources.gears;
            this.mbutton_cartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbutton_cartButton.Name = "mbutton_cartButton";
            this.mbutton_cartButton.Size = new System.Drawing.Size(72, 29);
            this.mbutton_cartButton.Text = "Config";
            this.mbutton_cartButton.ToolTipText = "Cart / Instrument Setup";
            this.mbutton_cartButton.Click += new System.EventHandler(this.mbutton_cartButton_Click);
            //
            // toolStripButtonAbout
            //
            this.toolStripButtonAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonAbout.Image = global::LcmsNet.Properties.Resources.About;
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(29, 29);
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            //
            // mbutton_reportError
            //
            this.mbutton_reportError.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mbutton_reportError.Image = global::LcmsNet.Properties.Resources.Append;
            this.mbutton_reportError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbutton_reportError.Name = "mbutton_reportError";
            this.mbutton_reportError.Size = new System.Drawing.Size(29, 29);
            this.mbutton_reportError.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mbutton_reportError.ToolTipText = "Create report for error reporting";
            this.mbutton_reportError.Click += new System.EventHandler(this.mbutton_reportError_Click);
            //
            // mstatusStrip_status
            //
            this.mstatusStrip_status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mstatusStrip_status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.mstatusStrip_status.Location = new System.Drawing.Point(0, 663);
            this.mstatusStrip_status.Name = "mstatusStrip_status";
            this.mstatusStrip_status.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.mstatusStrip_status.Size = new System.Drawing.Size(1070, 22);
            this.mstatusStrip_status.TabIndex = 2;
            this.mstatusStrip_status.Text = "StatusStrip";
            //
            // toolStripStatusLabel
            //
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Status";
            //
            // menuStrip
            //
            this.menuStrip.BackColor = System.Drawing.Color.White;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1070, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            //
            // fileMenu
            //
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.fileMenu.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileMenu.MergeIndex = 0;
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "&File";
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator3.MergeIndex = 100;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(89, 6);
            //
            // exitToolStripMenuItem
            //
            this.exitToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.exitToolStripMenuItem.MergeIndex = 101;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
            //
            // formMDImain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1070, 685);
            this.Controls.Add(this.mstatusStrip_status);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "formMDImain";
            this.Text = "LcmsNet";
            this.toolTip.SetToolTip(this, "Separation Status ");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMDImain_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mstatusStrip_status.ResumeLayout(false);
            this.mstatusStrip_status.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

            private System.Windows.Forms.ToolStrip toolStrip;
            private System.Windows.Forms.StatusStrip mstatusStrip_status;
            private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
            private System.Windows.Forms.ToolTip toolTip;
              private System.Windows.Forms.ToolStripButton mtoolButton_showMessages;
              private System.Windows.Forms.ToolStripButton mtoolButton_showSampleQueue;
              private System.Windows.Forms.ToolStripButton mbutton_MethodEditor;
              private System.Windows.Forms.ToolStripButton mbutton_sampleProgress;
              private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
              private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
              private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
              private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
             private System.Windows.Forms.MenuStrip menuStrip;
             private System.Windows.Forms.ToolStripMenuItem fileMenu;
             private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
             private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
             private System.Windows.Forms.ToolStripButton mbutton_reportError;
             private System.Windows.Forms.ToolStripButton toolStripButton2;
             private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
             private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
             private System.Windows.Forms.ToolStripButton toolButton_notificationSystem;
             private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
             private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
             private System.Windows.Forms.ToolStripButton toolStripButtonFludics;
             private System.Windows.Forms.ToolStripButton mbutton_cartButton;
             private System.Windows.Forms.ToolStripButton toolStripButtonSimulate;
             private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSimulator;
    }
}



