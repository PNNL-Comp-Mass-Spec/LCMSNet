namespace ContactClosureAutomation
{
    partial class contactClosureForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_timer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.m_totalTriggers = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_cycleTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_stopButton = new System.Windows.Forms.Button();
            this.m_runButton = new System.Windows.Forms.Button();
            this.m_delayTime = new System.Windows.Forms.NumericUpDown();
            this.m_holdOpenTime = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.m_contactControl = new LcmsNet.Devices.ContactClosure.controlContactClosureU3();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_totalTriggers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_delayTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_holdOpenTime)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(364, 279);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.m_totalTriggers);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.m_cycleTime);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.m_stopButton);
            this.tabPage1.Controls.Add(this.m_runButton);
            this.tabPage1.Controls.Add(this.m_delayTime);
            this.tabPage1.Controls.Add(this.m_holdOpenTime);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(356, 253);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Run";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(264, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "triggers";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(141, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Make";
            // 
            // m_totalTriggers
            // 
            this.m_totalTriggers.Location = new System.Drawing.Point(190, 113);
            this.m_totalTriggers.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.m_totalTriggers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_totalTriggers.Name = "m_totalTriggers";
            this.m_totalTriggers.Size = new System.Drawing.Size(68, 20);
            this.m_totalTriggers.TabIndex = 10;
            this.m_totalTriggers.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_totalTriggers.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(264, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "seconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(264, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "seconds";
            // 
            // m_cycleTime
            // 
            this.m_cycleTime.AutoSize = true;
            this.m_cycleTime.Location = new System.Drawing.Point(190, 75);
            this.m_cycleTime.Name = "m_cycleTime";
            this.m_cycleTime.Size = new System.Drawing.Size(18, 13);
            this.m_cycleTime.TabIndex = 7;
            this.m_cycleTime.Text = "(s)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(74, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Total Time For Cycle: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Wait time between contact closures";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Hold Contact Closure For ";
            // 
            // m_stopButton
            // 
            this.m_stopButton.Enabled = false;
            this.m_stopButton.Location = new System.Drawing.Point(190, 153);
            this.m_stopButton.Name = "m_stopButton";
            this.m_stopButton.Size = new System.Drawing.Size(140, 76);
            this.m_stopButton.TabIndex = 3;
            this.m_stopButton.Text = "Stop";
            this.m_stopButton.UseVisualStyleBackColor = true;
            this.m_stopButton.Click += new System.EventHandler(this.m_stopButton_Click);
            // 
            // m_runButton
            // 
            this.m_runButton.Location = new System.Drawing.Point(35, 153);
            this.m_runButton.Name = "m_runButton";
            this.m_runButton.Size = new System.Drawing.Size(140, 76);
            this.m_runButton.TabIndex = 2;
            this.m_runButton.Text = "Run";
            this.m_runButton.UseVisualStyleBackColor = true;
            this.m_runButton.Click += new System.EventHandler(this.m_runButton_Click);
            // 
            // m_delayTime
            // 
            this.m_delayTime.Location = new System.Drawing.Point(190, 44);
            this.m_delayTime.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.m_delayTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_delayTime.Name = "m_delayTime";
            this.m_delayTime.Size = new System.Drawing.Size(68, 20);
            this.m_delayTime.TabIndex = 1;
            this.m_delayTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_delayTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.m_delayTime.ValueChanged += new System.EventHandler(this.m_delayTime_ValueChanged);
            // 
            // m_holdOpenTime
            // 
            this.m_holdOpenTime.Location = new System.Drawing.Point(190, 18);
            this.m_holdOpenTime.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.m_holdOpenTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_holdOpenTime.Name = "m_holdOpenTime";
            this.m_holdOpenTime.Size = new System.Drawing.Size(68, 20);
            this.m_holdOpenTime.TabIndex = 0;
            this.m_holdOpenTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_holdOpenTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.m_holdOpenTime.ValueChanged += new System.EventHandler(this.m_holdOpenTime_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.m_contactControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(356, 253);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lab Jack Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // m_contactControl
            // 
            this.m_contactControl.Device = null;
            this.m_contactControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_contactControl.Emulation = true;
            this.m_contactControl.Location = new System.Drawing.Point(3, 3);
            this.m_contactControl.Name = "m_contactControl";
            this.m_contactControl.Running = false;
            this.m_contactControl.Size = new System.Drawing.Size(350, 247);
            this.m_contactControl.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 279);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(364, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_statusLabel
            // 
            this.m_statusLabel.Name = "m_statusLabel";
            this.m_statusLabel.Size = new System.Drawing.Size(42, 17);
            this.m_statusLabel.Text = "Ready.";
            // 
            // contactClosureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 301);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "contactClosureForm";
            this.Text = "Contact Closure Automation";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_totalTriggers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_delayTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_holdOpenTime)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer m_timer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private LcmsNet.Devices.ContactClosure.controlContactClosureU3 m_contactControl;
        private System.Windows.Forms.Button m_stopButton;
        private System.Windows.Forms.Button m_runButton;
        private System.Windows.Forms.NumericUpDown m_delayTime;
        private System.Windows.Forms.NumericUpDown m_holdOpenTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label m_cycleTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel m_statusLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown m_totalTriggers;
    }
}

