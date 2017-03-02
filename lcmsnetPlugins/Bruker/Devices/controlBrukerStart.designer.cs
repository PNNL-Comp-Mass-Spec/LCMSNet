namespace LcmsNet.Devices.BrukerStart
{
    partial class controlBrukerStart
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
            this.mtabControl_methods = new System.Windows.Forms.TabControl();
            this.mtabPage_methods = new System.Windows.Forms.TabPage();
            this.mlabel_sampleName = new System.Windows.Forms.Label();
            this.mtextbox_sampleName = new System.Windows.Forms.TextBox();
            this.mlabel_status = new System.Windows.Forms.Label();
            this.mbutton_startAcquisition = new System.Windows.Forms.Button();
            this.mbutton_stopAcquisition = new System.Windows.Forms.Button();
            this.mbutton_getMethods = new System.Windows.Forms.Button();
            this.mcomboBox_methods = new System.Windows.Forms.ComboBox();
            this.mlabel_methods = new System.Windows.Forms.Label();
            this.mtabPage_advanced = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.mtextbox_ipAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mnum_port = new System.Windows.Forms.NumericUpDown();
            this.mtabControl_methods.SuspendLayout();
            this.mtabPage_methods.SuspendLayout();
            this.mtabPage_advanced.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_port)).BeginInit();
            // 
            // mtabControl_methods
            // 
            this.mtabControl_methods.Controls.Add(this.mtabPage_methods);
            this.mtabControl_methods.Controls.Add(this.mtabPage_advanced);
            this.mtabControl_methods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mtabControl_methods.Location = new System.Drawing.Point(0, 0);
            this.mtabControl_methods.Name = "mtabControl_methods";
            this.mtabControl_methods.SelectedIndex = 0;
            this.mtabControl_methods.Size = new System.Drawing.Size(262, 277);
            this.mtabControl_methods.TabIndex = 7;
            // 
            // mtabPage_methods
            // 
            this.mtabPage_methods.Controls.Add(this.mlabel_sampleName);
            this.mtabPage_methods.Controls.Add(this.mtextbox_sampleName);
            this.mtabPage_methods.Controls.Add(this.mlabel_status);
            this.mtabPage_methods.Controls.Add(this.mbutton_startAcquisition);
            this.mtabPage_methods.Controls.Add(this.mbutton_stopAcquisition);
            this.mtabPage_methods.Controls.Add(this.mbutton_getMethods);
            this.mtabPage_methods.Controls.Add(this.mcomboBox_methods);
            this.mtabPage_methods.Controls.Add(this.mlabel_methods);
            this.mtabPage_methods.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_methods.Name = "mtabPage_methods";
            this.mtabPage_methods.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_methods.Size = new System.Drawing.Size(254, 251);
            this.mtabPage_methods.TabIndex = 0;
            this.mtabPage_methods.Text = "Control";
            this.mtabPage_methods.UseVisualStyleBackColor = true;
            // 
            // mlabel_sampleName
            // 
            this.mlabel_sampleName.AutoSize = true;
            this.mlabel_sampleName.Location = new System.Drawing.Point(3, 87);
            this.mlabel_sampleName.Name = "mlabel_sampleName";
            this.mlabel_sampleName.Size = new System.Drawing.Size(73, 13);
            this.mlabel_sampleName.TabIndex = 10;
            this.mlabel_sampleName.Text = "Sample Name";
            // 
            // mtextbox_sampleName
            // 
            this.mtextbox_sampleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextbox_sampleName.Location = new System.Drawing.Point(6, 103);
            this.mtextbox_sampleName.Name = "mtextbox_sampleName";
            this.mtextbox_sampleName.Size = new System.Drawing.Size(239, 20);
            this.mtextbox_sampleName.TabIndex = 9;
            this.mtextbox_sampleName.Text = "ManualSample";
            // 
            // mlabel_status
            // 
            this.mlabel_status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_status.Location = new System.Drawing.Point(6, 173);
            this.mlabel_status.Name = "mlabel_status";
            this.mlabel_status.Size = new System.Drawing.Size(239, 66);
            this.mlabel_status.TabIndex = 8;
            this.mlabel_status.Text = "Status:";
            // 
            // mbutton_startAcquisition
            // 
            this.mbutton_startAcquisition.BackColor = System.Drawing.Color.Green;
            this.mbutton_startAcquisition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_startAcquisition.ForeColor = System.Drawing.Color.White;
            this.mbutton_startAcquisition.Location = new System.Drawing.Point(6, 131);
            this.mbutton_startAcquisition.Name = "mbutton_startAcquisition";
            this.mbutton_startAcquisition.Size = new System.Drawing.Size(104, 27);
            this.mbutton_startAcquisition.TabIndex = 7;
            this.mbutton_startAcquisition.Text = "Start";
            this.mbutton_startAcquisition.UseVisualStyleBackColor = false;
            this.mbutton_startAcquisition.Click += new System.EventHandler(this.mbutton_startAcquisition_Click);
            // 
            // mbutton_stopAcquisition
            // 
            this.mbutton_stopAcquisition.BackColor = System.Drawing.Color.DarkRed;
            this.mbutton_stopAcquisition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_stopAcquisition.ForeColor = System.Drawing.Color.White;
            this.mbutton_stopAcquisition.Location = new System.Drawing.Point(124, 131);
            this.mbutton_stopAcquisition.Name = "mbutton_stopAcquisition";
            this.mbutton_stopAcquisition.Size = new System.Drawing.Size(104, 27);
            this.mbutton_stopAcquisition.TabIndex = 6;
            this.mbutton_stopAcquisition.Text = "Stop";
            this.mbutton_stopAcquisition.UseVisualStyleBackColor = false;
            this.mbutton_stopAcquisition.Click += new System.EventHandler(this.mbutton_stopAcquisition_Click);
            // 
            // mbutton_getMethods
            // 
            this.mbutton_getMethods.Location = new System.Drawing.Point(132, 57);
            this.mbutton_getMethods.Name = "mbutton_getMethods";
            this.mbutton_getMethods.Size = new System.Drawing.Size(93, 22);
            this.mbutton_getMethods.TabIndex = 5;
            this.mbutton_getMethods.Text = "Refresh";
            this.mbutton_getMethods.UseVisualStyleBackColor = true;
            this.mbutton_getMethods.Click += new System.EventHandler(this.mbutton_getMethods_Click);
            // 
            // mcomboBox_methods
            // 
            this.mcomboBox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_methods.FormattingEnabled = true;
            this.mcomboBox_methods.Location = new System.Drawing.Point(6, 30);
            this.mcomboBox_methods.Name = "mcomboBox_methods";
            this.mcomboBox_methods.Size = new System.Drawing.Size(239, 21);
            this.mcomboBox_methods.TabIndex = 3;
            // 
            // mlabel_methods
            // 
            this.mlabel_methods.AutoSize = true;
            this.mlabel_methods.Location = new System.Drawing.Point(3, 14);
            this.mlabel_methods.Name = "mlabel_methods";
            this.mlabel_methods.Size = new System.Drawing.Size(48, 13);
            this.mlabel_methods.TabIndex = 4;
            this.mlabel_methods.Text = "Methods";
            // 
            // mtabPage_advanced
            // 
            this.mtabPage_advanced.Controls.Add(this.label2);
            this.mtabPage_advanced.Controls.Add(this.mtextbox_ipAddress);
            this.mtabPage_advanced.Controls.Add(this.label1);
            this.mtabPage_advanced.Controls.Add(this.mnum_port);
            this.mtabPage_advanced.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_advanced.Name = "mtabPage_advanced";
            this.mtabPage_advanced.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_advanced.Size = new System.Drawing.Size(234, 261);
            this.mtabPage_advanced.TabIndex = 1;
            this.mtabPage_advanced.Text = "Advanced";
            this.mtabPage_advanced.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "IP Address";
            // 
            // mtextbox_ipAddress
            // 
            this.mtextbox_ipAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextbox_ipAddress.Location = new System.Drawing.Point(84, 20);
            this.mtextbox_ipAddress.Name = "mtextbox_ipAddress";
            this.mtextbox_ipAddress.Size = new System.Drawing.Size(144, 20);
            this.mtextbox_ipAddress.TabIndex = 2;
            this.mtextbox_ipAddress.Text = "localhost";
            this.mtextbox_ipAddress.TextChanged += new System.EventHandler(this.mtextbox_ipAddress_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port";
            // 
            // mnum_port
            // 
            this.mnum_port.Location = new System.Drawing.Point(84, 46);
            this.mnum_port.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.mnum_port.Name = "mnum_port";
            this.mnum_port.Size = new System.Drawing.Size(63, 20);
            this.mnum_port.TabIndex = 0;
            this.mnum_port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_port.Value = new decimal(new int[] {
            4771,
            0,
            0,
            0});
            this.mnum_port.ValueChanged += new System.EventHandler(this.mnum_port_ValueChanged);
            // 
            // controlBrukerStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mtabControl_methods);
            this.Name = "controlBrukerStart";
            this.Size = new System.Drawing.Size(262, 365);
            this.Controls.SetChildIndex(this.mtabControl_methods, 0);
            
            this.mtabControl_methods.ResumeLayout(false);
            this.mtabPage_methods.ResumeLayout(false);
            this.mtabPage_methods.PerformLayout();
            this.mtabPage_advanced.ResumeLayout(false);
            this.mtabPage_advanced.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mtabControl_methods;
        private System.Windows.Forms.TabPage mtabPage_methods;
        private System.Windows.Forms.Label mlabel_sampleName;
        private System.Windows.Forms.TextBox mtextbox_sampleName;
        private System.Windows.Forms.Label mlabel_status;
        private System.Windows.Forms.Button mbutton_startAcquisition;
        private System.Windows.Forms.Button mbutton_stopAcquisition;
        private System.Windows.Forms.Button mbutton_getMethods;
        private System.Windows.Forms.ComboBox mcomboBox_methods;
        private System.Windows.Forms.Label mlabel_methods;
        private System.Windows.Forms.TabPage mtabPage_advanced;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mtextbox_ipAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown mnum_port;
    }
}
