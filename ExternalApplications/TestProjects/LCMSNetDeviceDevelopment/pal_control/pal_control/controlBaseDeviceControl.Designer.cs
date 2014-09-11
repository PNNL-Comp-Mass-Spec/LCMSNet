namespace pal_control
{
    partial class controlBaseDeviceControl
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
            this.mgroupBox_Rename = new System.Windows.Forms.GroupBox();
            this.mgroupBox_Rename.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtextBox_NewDeviceName
            // 
            this.mtextBox_NewDeviceName.Location = new System.Drawing.Point(6, 14);
            this.mtextBox_NewDeviceName.Name = "mtextBox_NewDeviceName";
            this.mtextBox_NewDeviceName.Size = new System.Drawing.Size(152, 20);
            this.mtextBox_NewDeviceName.TabIndex = 0;
            // 
            // mbutton_RenameDevice
            // 
            this.mbutton_RenameDevice.Location = new System.Drawing.Point(164, 13);
            this.mbutton_RenameDevice.Name = "mbutton_RenameDevice";
            this.mbutton_RenameDevice.Size = new System.Drawing.Size(75, 21);
            this.mbutton_RenameDevice.TabIndex = 1;
            this.mbutton_RenameDevice.Text = "Rename";
            this.mbutton_RenameDevice.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_Rename
            // 
            this.mgroupBox_Rename.Controls.Add(this.mtextBox_NewDeviceName);
            this.mgroupBox_Rename.Controls.Add(this.mbutton_RenameDevice);
            this.mgroupBox_Rename.Location = new System.Drawing.Point(3, 248);
            this.mgroupBox_Rename.Name = "mgroupBox_Rename";
            this.mgroupBox_Rename.Size = new System.Drawing.Size(249, 40);
            this.mgroupBox_Rename.TabIndex = 2;
            this.mgroupBox_Rename.TabStop = false;
            // 
            // controlBaseDeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mgroupBox_Rename);
            this.Name = "controlBaseDeviceControl";
            this.Size = new System.Drawing.Size(257, 294);
            this.mgroupBox_Rename.ResumeLayout(false);
            this.mgroupBox_Rename.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox mtextBox_NewDeviceName;
        private System.Windows.Forms.Button mbutton_RenameDevice;
        private System.Windows.Forms.GroupBox mgroupBox_Rename;
    }
}
