namespace LcmsNet.Devices
{
    partial class AdvancedDeviceGroupControl
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
            this.m_selectedDevicePanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.m_deviceButtonPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearError = new System.Windows.Forms.Button();
            this.mlabel_status = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_RenameDevice = new System.Windows.Forms.Button();
            this.mtextBox_NewDeviceName = new System.Windows.Forms.TextBox();
            this.mbutton_initialize = new System.Windows.Forms.Button();
            this.m_selectedDevicePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            //
            // m_selectedDevicePanel
            //
            this.m_selectedDevicePanel.AutoScroll = true;
            this.m_selectedDevicePanel.BackColor = System.Drawing.Color.White;
            this.m_selectedDevicePanel.Controls.Add(this.splitter1);
            this.m_selectedDevicePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_selectedDevicePanel.Location = new System.Drawing.Point(156, 0);
            this.m_selectedDevicePanel.Name = "m_selectedDevicePanel";
            this.m_selectedDevicePanel.Size = new System.Drawing.Size(503, 522);
            this.m_selectedDevicePanel.TabIndex = 0;
            //
            // splitter1
            //
            this.splitter1.BackColor = System.Drawing.Color.Silver;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 522);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            //
            // m_deviceButtonPanel
            //
            this.m_deviceButtonPanel.AutoScroll = true;
            this.m_deviceButtonPanel.BackColor = System.Drawing.Color.White;
            this.m_deviceButtonPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_deviceButtonPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.m_deviceButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.m_deviceButtonPanel.Name = "m_deviceButtonPanel";
            this.m_deviceButtonPanel.Size = new System.Drawing.Size(156, 670);
            this.m_deviceButtonPanel.TabIndex = 1;
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.clearError);
            this.panel1.Controls.Add(this.mlabel_status);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.mbutton_initialize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(156, 522);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(503, 148);
            this.panel1.TabIndex = 1;
            //
            // clearError
            //
            this.clearError.Image = global::LcmsNet.Properties.Resources.ButtonDeleteRed;
            this.clearError.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearError.Location = new System.Drawing.Point(98, 110);
            this.clearError.Name = "clearError";
            this.clearError.Size = new System.Drawing.Size(86, 34);
            this.clearError.TabIndex = 6;
            this.clearError.Text = "Clear Error";
            this.clearError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.clearError.UseVisualStyleBackColor = true;
            this.clearError.Click += new System.EventHandler(this.clearError_Click);
            //
            // mlabel_status
            //
            this.mlabel_status.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_status.Location = new System.Drawing.Point(0, 0);
            this.mlabel_status.Name = "mlabel_status";
            this.mlabel_status.Size = new System.Drawing.Size(503, 30);
            this.mlabel_status.TabIndex = 5;
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mbutton_RenameDevice);
            this.groupBox1.Controls.Add(this.mtextBox_NewDeviceName);
            this.groupBox1.Location = new System.Drawing.Point(6, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(494, 71);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            //
            // mbutton_RenameDevice
            //
            this.mbutton_RenameDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_RenameDevice.Location = new System.Drawing.Point(407, 32);
            this.mbutton_RenameDevice.Name = "mbutton_RenameDevice";
            this.mbutton_RenameDevice.Size = new System.Drawing.Size(81, 33);
            this.mbutton_RenameDevice.TabIndex = 3;
            this.mbutton_RenameDevice.Text = "Rename";
            this.mbutton_RenameDevice.UseVisualStyleBackColor = true;
            this.mbutton_RenameDevice.Click += new System.EventHandler(this.mbutton_RenameDevice_Click);
            //
            // mtextBox_NewDeviceName
            //
            this.mtextBox_NewDeviceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_NewDeviceName.Location = new System.Drawing.Point(19, 38);
            this.mtextBox_NewDeviceName.Name = "mtextBox_NewDeviceName";
            this.mtextBox_NewDeviceName.Size = new System.Drawing.Size(374, 20);
            this.mtextBox_NewDeviceName.TabIndex = 2;
            //
            // mbutton_initialize
            //
            this.mbutton_initialize.Image = global::LcmsNet.Properties.Resources.Cycle_16_Yellow;
            this.mbutton_initialize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_initialize.Location = new System.Drawing.Point(6, 110);
            this.mbutton_initialize.Name = "mbutton_initialize";
            this.mbutton_initialize.Size = new System.Drawing.Size(86, 34);
            this.mbutton_initialize.TabIndex = 0;
            this.mbutton_initialize.Text = "Initialize";
            this.mbutton_initialize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_initialize.UseVisualStyleBackColor = true;
            this.mbutton_initialize.Click += new System.EventHandler(this.mbutton_initialize_Click);
            //
            // AdvancedDeviceGroupControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.m_selectedDevicePanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_deviceButtonPanel);
            this.Name = "AdvancedDeviceGroupControl";
            this.Size = new System.Drawing.Size(659, 670);
            this.m_selectedDevicePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel m_selectedDevicePanel;
        private System.Windows.Forms.Panel m_deviceButtonPanel;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button mbutton_initialize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Button mbutton_RenameDevice;
        protected System.Windows.Forms.TextBox mtextBox_NewDeviceName;
        private System.Windows.Forms.Label mlabel_status;
        private System.Windows.Forms.Button clearError;

    }
}
