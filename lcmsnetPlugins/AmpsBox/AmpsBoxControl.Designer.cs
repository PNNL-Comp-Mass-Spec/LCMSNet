namespace AmpsBox
{
    partial class AmpsBoxControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.mnum_channelNumberRF = new System.Windows.Forms.NumericUpDown();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_channelHV = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mlabel_version = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mbutton_closePort = new System.Windows.Forms.Button();
            this.mpropertyGrid_serialPort = new System.Windows.Forms.PropertyGrid();
            this.mbutton_openPort = new System.Windows.Forms.Button();
            this.mbutton_saveParameters = new System.Windows.Forms.Button();
            this.mbutton_getVersion = new System.Windows.Forms.Button();
            this.mbuton_emulate = new System.Windows.Forms.Button();
            this.m_rfControl = new AmpsBox.RFControl();
            this.m_hvControl = new AmpsBox.SingleElementControl();
            this.m_rfPanel = new System.Windows.Forms.Panel();
            this.mgroupBox_Rename.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_channelNumberRF)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_channelHV)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtextBox_NewDeviceName
            // 
            this.mtextBox_NewDeviceName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            // 
            // mgroupBox_Rename
            // 
            this.mgroupBox_Rename.Location = new System.Drawing.Point(0, 482);
            this.mgroupBox_Rename.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mgroupBox_Rename.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mgroupBox_Rename.Size = new System.Drawing.Size(271, 94);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(271, 482);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.mnum_channelNumberRF);
            this.tabPage1.Controls.Add(this.m_rfControl);
            this.tabPage1.Controls.Add(this.m_rfPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(263, 456);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RF";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Channel";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mnum_channelNumberRF
            // 
            this.mnum_channelNumberRF.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnum_channelNumberRF.Location = new System.Drawing.Point(9, 24);
            this.mnum_channelNumberRF.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.mnum_channelNumberRF.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_channelNumberRF.Name = "mnum_channelNumberRF";
            this.mnum_channelNumberRF.Size = new System.Drawing.Size(225, 47);
            this.mnum_channelNumberRF.TabIndex = 1;
            this.mnum_channelNumberRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_channelNumberRF.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.m_hvControl);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.mnum_channelHV);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(263, 456);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "High Voltage DC";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Channel";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mnum_channelHV
            // 
            this.mnum_channelHV.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnum_channelHV.Location = new System.Drawing.Point(5, 27);
            this.mnum_channelHV.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.mnum_channelHV.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_channelHV.Name = "mnum_channelHV";
            this.mnum_channelHV.Size = new System.Drawing.Size(234, 47);
            this.mnum_channelHV.TabIndex = 3;
            this.mnum_channelHV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_channelHV.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_channelHV.ValueChanged += new System.EventHandler(this.mnum_channelHV_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mbuton_emulate);
            this.tabPage2.Controls.Add(this.mlabel_version);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.mbutton_saveParameters);
            this.tabPage2.Controls.Add(this.mbutton_getVersion);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(263, 456);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // mlabel_version
            // 
            this.mlabel_version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_version.Location = new System.Drawing.Point(6, 386);
            this.mlabel_version.Name = "mlabel_version";
            this.mlabel_version.Size = new System.Drawing.Size(244, 29);
            this.mlabel_version.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.mbutton_closePort);
            this.groupBox1.Controls.Add(this.mpropertyGrid_serialPort);
            this.groupBox1.Controls.Add(this.mbutton_openPort);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 288);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication";
            // 
            // mbutton_closePort
            // 
            this.mbutton_closePort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_closePort.Location = new System.Drawing.Point(25, 258);
            this.mbutton_closePort.Name = "mbutton_closePort";
            this.mbutton_closePort.Size = new System.Drawing.Size(101, 24);
            this.mbutton_closePort.TabIndex = 14;
            this.mbutton_closePort.Text = "Close";
            this.mbutton_closePort.UseVisualStyleBackColor = true;
            this.mbutton_closePort.Click += new System.EventHandler(this.mbutton_closePort_Click);
            // 
            // mpropertyGrid_serialPort
            // 
            this.mpropertyGrid_serialPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpropertyGrid_serialPort.Location = new System.Drawing.Point(6, 19);
            this.mpropertyGrid_serialPort.Name = "mpropertyGrid_serialPort";
            this.mpropertyGrid_serialPort.Size = new System.Drawing.Size(237, 233);
            this.mpropertyGrid_serialPort.TabIndex = 13;
            // 
            // mbutton_openPort
            // 
            this.mbutton_openPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_openPort.Location = new System.Drawing.Point(132, 258);
            this.mbutton_openPort.Name = "mbutton_openPort";
            this.mbutton_openPort.Size = new System.Drawing.Size(101, 24);
            this.mbutton_openPort.TabIndex = 12;
            this.mbutton_openPort.Text = "Open";
            this.mbutton_openPort.UseVisualStyleBackColor = true;
            this.mbutton_openPort.Click += new System.EventHandler(this.mbutton_openPort_Click);
            // 
            // mbutton_saveParameters
            // 
            this.mbutton_saveParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_saveParameters.Location = new System.Drawing.Point(12, 309);
            this.mbutton_saveParameters.Name = "mbutton_saveParameters";
            this.mbutton_saveParameters.Size = new System.Drawing.Size(231, 34);
            this.mbutton_saveParameters.TabIndex = 14;
            this.mbutton_saveParameters.Text = "Save Parameters To Device";
            this.mbutton_saveParameters.UseVisualStyleBackColor = true;
            this.mbutton_saveParameters.Click += new System.EventHandler(this.mbutton_saveParameters_Click);
            // 
            // mbutton_getVersion
            // 
            this.mbutton_getVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_getVersion.Location = new System.Drawing.Point(12, 349);
            this.mbutton_getVersion.Name = "mbutton_getVersion";
            this.mbutton_getVersion.Size = new System.Drawing.Size(231, 34);
            this.mbutton_getVersion.TabIndex = 13;
            this.mbutton_getVersion.Text = "Get Version";
            this.mbutton_getVersion.UseVisualStyleBackColor = true;
            this.mbutton_getVersion.Click += new System.EventHandler(this.mbutton_getVersion_Click);
            // 
            // mbuton_emulate
            // 
            this.mbuton_emulate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbuton_emulate.Location = new System.Drawing.Point(12, 418);
            this.mbuton_emulate.Name = "mbuton_emulate";
            this.mbuton_emulate.Size = new System.Drawing.Size(231, 34);
            this.mbuton_emulate.TabIndex = 17;
            this.mbuton_emulate.Text = "Emulate";
            this.mbuton_emulate.UseVisualStyleBackColor = true;
            this.mbuton_emulate.Click += new System.EventHandler(this.mbuton_emulate_Click);
            // 
            // m_rfControl
            // 
            this.m_rfControl.BackColor = System.Drawing.Color.White;
            this.m_rfControl.Enabled = false;
            this.m_rfControl.Location = new System.Drawing.Point(6, 77);
            this.m_rfControl.Margin = new System.Windows.Forms.Padding(4);
            this.m_rfControl.Name = "m_rfControl";
            this.m_rfControl.Size = new System.Drawing.Size(243, 354);
            this.m_rfControl.TabIndex = 0;
            // 
            // m_hvControl
            // 
            this.m_hvControl.BackColor = System.Drawing.Color.White;
            this.m_hvControl.Data = null;
            this.m_hvControl.DisplayName = "High Voltage";
            this.m_hvControl.Enabled = false;
            this.m_hvControl.Location = new System.Drawing.Point(5, 80);
            this.m_hvControl.Margin = new System.Windows.Forms.Padding(4);
            this.m_hvControl.Name = "m_hvControl";
            this.m_hvControl.Size = new System.Drawing.Size(234, 120);
            this.m_hvControl.TabIndex = 5;
            // 
            // m_rfPanel
            // 
            this.m_rfPanel.Location = new System.Drawing.Point(9, 77);
            this.m_rfPanel.Name = "m_rfPanel";
            this.m_rfPanel.Size = new System.Drawing.Size(248, 354);
            this.m_rfPanel.TabIndex = 3;
            // 
            // AmpsBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AmpsBoxControl";
            this.Size = new System.Drawing.Size(271, 576);
            this.Controls.SetChildIndex(this.mgroupBox_Rename, 0);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.mgroupBox_Rename.ResumeLayout(false);
            this.mgroupBox_Rename.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_channelNumberRF)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_channelHV)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button mbutton_openPort;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button mbutton_getVersion;
        private System.Windows.Forms.Button mbutton_saveParameters;
        private RFControl m_rfControl;
        private System.Windows.Forms.NumericUpDown mnum_channelNumberRF;
        private System.Windows.Forms.Label label1;
        private SingleElementControl m_hvControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_channelHV;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid mpropertyGrid_serialPort;
        private System.Windows.Forms.Button mbutton_closePort;
        private System.Windows.Forms.Label mlabel_version;
        private System.Windows.Forms.Button mbuton_emulate;
        private System.Windows.Forms.Panel m_rfPanel;
    }
}
