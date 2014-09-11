namespace LcmsNet.Method.Forms
{
    partial class controlLCMethodEditor
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
            this.msplitter_one = new System.Windows.Forms.Splitter();
            this.mpanel_controlPanel = new System.Windows.Forms.Panel();
            this.mtabPages_methods = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mlabel_previewMode = new System.Windows.Forms.Label();
            this.mcomboBox_previewMode = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mgroupBox_update = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mnum_delay = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.mnum_frameCount = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_animate = new System.Windows.Forms.CheckBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mcontrol_methodTimelineThroughput = new LcmsNet.Method.Forms.controlLCMethodTimeline();
            this.mcontrol_selectedMethods = new LcmsNet.Method.Forms.controlLCMethodSelection();
            this.mcontrol_acquisitionStage = new LcmsNet.Method.Forms.controlLCMethodStage();
            this.mpanel_controlPanel.SuspendLayout();
            this.mtabPages_methods.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.mgroupBox_update.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_frameCount)).BeginInit();
            this.SuspendLayout();
            // 
            // msplitter_one
            // 
            this.msplitter_one.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.msplitter_one.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.msplitter_one.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.msplitter_one.Location = new System.Drawing.Point(5, 345);
            this.msplitter_one.Name = "msplitter_one";
            this.msplitter_one.Size = new System.Drawing.Size(816, 5);
            this.msplitter_one.TabIndex = 4;
            this.msplitter_one.TabStop = false;
            // 
            // mpanel_controlPanel
            // 
            this.mpanel_controlPanel.BackColor = System.Drawing.Color.White;
            this.mpanel_controlPanel.Controls.Add(this.mcontrol_acquisitionStage);
            this.mpanel_controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_controlPanel.Location = new System.Drawing.Point(5, 350);
            this.mpanel_controlPanel.Name = "mpanel_controlPanel";
            this.mpanel_controlPanel.Size = new System.Drawing.Size(816, 365);
            this.mpanel_controlPanel.TabIndex = 10;
            // 
            // mtabPages_methods
            // 
            this.mtabPages_methods.Controls.Add(this.tabPage1);
            this.mtabPages_methods.Controls.Add(this.tabPage2);
            this.mtabPages_methods.Dock = System.Windows.Forms.DockStyle.Right;
            this.mtabPages_methods.Location = new System.Drawing.Point(573, 5);
            this.mtabPages_methods.Multiline = true;
            this.mtabPages_methods.Name = "mtabPages_methods";
            this.mtabPages_methods.SelectedIndex = 0;
            this.mtabPages_methods.Size = new System.Drawing.Size(248, 340);
            this.mtabPages_methods.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.mlabel_previewMode);
            this.tabPage1.Controls.Add(this.mcomboBox_previewMode);
            this.tabPage1.Controls.Add(this.mcontrol_selectedMethods);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(240, 314);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Method Preview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mlabel_previewMode
            // 
            this.mlabel_previewMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_previewMode.AutoSize = true;
            this.mlabel_previewMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_previewMode.Location = new System.Drawing.Point(6, 287);
            this.mlabel_previewMode.Name = "mlabel_previewMode";
            this.mlabel_previewMode.Size = new System.Drawing.Size(75, 13);
            this.mlabel_previewMode.TabIndex = 13;
            this.mlabel_previewMode.Text = "Preview Mode";
            // 
            // mcomboBox_previewMode
            // 
            this.mcomboBox_previewMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_previewMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcomboBox_previewMode.FormattingEnabled = true;
            this.mcomboBox_previewMode.Location = new System.Drawing.Point(93, 287);
            this.mcomboBox_previewMode.Name = "mcomboBox_previewMode";
            this.mcomboBox_previewMode.Size = new System.Drawing.Size(141, 21);
            this.mcomboBox_previewMode.TabIndex = 11;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mgroupBox_update);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(240, 314);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Preview Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_update
            // 
            this.mgroupBox_update.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_update.Controls.Add(this.label3);
            this.mgroupBox_update.Controls.Add(this.mnum_delay);
            this.mgroupBox_update.Controls.Add(this.label4);
            this.mgroupBox_update.Controls.Add(this.mnum_frameCount);
            this.mgroupBox_update.Controls.Add(this.mcheckBox_animate);
            this.mgroupBox_update.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_update.Location = new System.Drawing.Point(6, 6);
            this.mgroupBox_update.Name = "mgroupBox_update";
            this.mgroupBox_update.Size = new System.Drawing.Size(228, 259);
            this.mgroupBox_update.TabIndex = 13;
            this.mgroupBox_update.TabStop = false;
            this.mgroupBox_update.Text = "Preview Options";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Frame Update";
            // 
            // mnum_delay
            // 
            this.mnum_delay.Location = new System.Drawing.Point(92, 37);
            this.mnum_delay.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.mnum_delay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mnum_delay.Name = "mnum_delay";
            this.mnum_delay.Size = new System.Drawing.Size(60, 20);
            this.mnum_delay.TabIndex = 13;
            this.mnum_delay.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.mnum_delay.ValueChanged += new System.EventHandler(this.mnum_delay_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Delay (ms)";
            // 
            // mnum_frameCount
            // 
            this.mnum_frameCount.Location = new System.Drawing.Point(92, 67);
            this.mnum_frameCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mnum_frameCount.Name = "mnum_frameCount";
            this.mnum_frameCount.Size = new System.Drawing.Size(61, 20);
            this.mnum_frameCount.TabIndex = 14;
            this.mnum_frameCount.ValueChanged += new System.EventHandler(this.mnum_frameCount_ValueChanged);
            // 
            // mcheckBox_animate
            // 
            this.mcheckBox_animate.AutoSize = true;
            this.mcheckBox_animate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcheckBox_animate.Location = new System.Drawing.Point(17, 19);
            this.mcheckBox_animate.Name = "mcheckBox_animate";
            this.mcheckBox_animate.Size = new System.Drawing.Size(64, 17);
            this.mcheckBox_animate.TabIndex = 12;
            this.mcheckBox_animate.Text = "Animate";
            this.mcheckBox_animate.UseVisualStyleBackColor = true;
            this.mcheckBox_animate.CheckedChanged += new System.EventHandler(this.mcheckBox_animate_CheckedChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.Gray;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(568, 5);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 340);
            this.splitter1.TabIndex = 16;
            this.splitter1.TabStop = false;
            // 
            // mcontrol_methodTimelineThroughput
            // 
            this.mcontrol_methodTimelineThroughput.AutoScroll = true;
            this.mcontrol_methodTimelineThroughput.BackColor = System.Drawing.Color.White;
            this.mcontrol_methodTimelineThroughput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mcontrol_methodTimelineThroughput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_methodTimelineThroughput.Location = new System.Drawing.Point(5, 5);
            this.mcontrol_methodTimelineThroughput.Name = "mcontrol_methodTimelineThroughput";
            this.mcontrol_methodTimelineThroughput.RenderMode = LcmsNet.Method.enumLCMethodRenderMode.Column;
            this.mcontrol_methodTimelineThroughput.Size = new System.Drawing.Size(568, 340);
            this.mcontrol_methodTimelineThroughput.TabIndex = 5;
            // 
            // mcontrol_selectedMethods
            // 
            this.mcontrol_selectedMethods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_selectedMethods.AutoScroll = true;
            this.mcontrol_selectedMethods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcontrol_selectedMethods.Location = new System.Drawing.Point(6, 6);
            this.mcontrol_selectedMethods.Name = "mcontrol_selectedMethods";
            this.mcontrol_selectedMethods.Size = new System.Drawing.Size(228, 278);
            this.mcontrol_selectedMethods.TabIndex = 10;
            // 
            // mcontrol_acquisitionStage
            // 
            this.mcontrol_acquisitionStage.AllowPostOverlap = false;
            this.mcontrol_acquisitionStage.AllowPreOverlap = false;
            this.mcontrol_acquisitionStage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_acquisitionStage.BackColor = System.Drawing.Color.White;
            this.mcontrol_acquisitionStage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mcontrol_acquisitionStage.Location = new System.Drawing.Point(0, -1);
            this.mcontrol_acquisitionStage.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_acquisitionStage.MethodFolderPath = "LCMethods";
            this.mcontrol_acquisitionStage.Name = "mcontrol_acquisitionStage";
            this.mcontrol_acquisitionStage.Size = new System.Drawing.Size(816, 366);
            this.mcontrol_acquisitionStage.TabIndex = 5;
            // 
            // controlLCMethodEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.mcontrol_methodTimelineThroughput);
            this.Controls.Add(this.mtabPages_methods);
            this.Controls.Add(this.msplitter_one);
            this.Controls.Add(this.mpanel_controlPanel);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "controlLCMethodEditor";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(826, 720);
            this.mpanel_controlPanel.ResumeLayout(false);
            this.mtabPages_methods.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.mgroupBox_update.ResumeLayout(false);
            this.mgroupBox_update.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_frameCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Splitter msplitter_one;
        private controlLCMethodStage mcontrol_acquisitionStage;
        private controlLCMethodTimeline mcontrol_methodTimelineThroughput;
        private System.Windows.Forms.Panel mpanel_controlPanel;
        private controlLCMethodSelection mcontrol_selectedMethods;
        private System.Windows.Forms.TabControl mtabPages_methods;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label mlabel_previewMode;
        private System.Windows.Forms.ComboBox mcomboBox_previewMode;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox mgroupBox_update;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown mnum_delay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown mnum_frameCount;
        private System.Windows.Forms.CheckBox mcheckBox_animate;
        private System.Windows.Forms.Splitter splitter1;

    }
}
