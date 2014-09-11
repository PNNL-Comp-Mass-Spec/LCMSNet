namespace WindowsFormsApplication2
{
    partial class controlBaseDevice
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
            this.mtextBox_NewDeviceName = new System.Windows.Forms.TextBox();
            this.mbutton_RenameDevice = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtextBox_NewDeviceName
            // 
            this.mtextBox_NewDeviceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_NewDeviceName.Location = new System.Drawing.Point(3, 4);
            this.mtextBox_NewDeviceName.Name = "mtextBox_NewDeviceName";
            this.mtextBox_NewDeviceName.Size = new System.Drawing.Size(155, 20);
            this.mtextBox_NewDeviceName.TabIndex = 0;
            this.mtextBox_NewDeviceName.TextChanged += new System.EventHandler(this.mtextBox_NewDeviceName_TextChanged);
            // 
            // mbutton_RenameDevice
            // 
            this.mbutton_RenameDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_RenameDevice.Location = new System.Drawing.Point(83, 28);
            this.mbutton_RenameDevice.Name = "mbutton_RenameDevice";
            this.mbutton_RenameDevice.Size = new System.Drawing.Size(75, 23);
            this.mbutton_RenameDevice.TabIndex = 1;
            this.mbutton_RenameDevice.Text = "Rename";
            this.mbutton_RenameDevice.UseVisualStyleBackColor = true;
            this.mbutton_RenameDevice.Click += new System.EventHandler(this.mbutton_RenameDevice_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mbutton_RenameDevice);
            this.panel1.Controls.Add(this.mtextBox_NewDeviceName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 298);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(161, 54);
            this.panel1.TabIndex = 2;
            // 
            // controlBaseDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "controlBaseDevice";
            this.Size = new System.Drawing.Size(161, 352);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox mtextBox_NewDeviceName;
        private System.Windows.Forms.Button mbutton_RenameDevice;
        private System.Windows.Forms.Panel panel1;
    }
}
