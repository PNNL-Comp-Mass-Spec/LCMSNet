namespace LcmsNet.Devices
{
    partial class formDeviceManager
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
            this.mlistview_devices = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            //
            // mlistview_devices
            //
            this.mlistview_devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlistview_devices.GridLines = true;
            this.mlistview_devices.Location = new System.Drawing.Point(0, 0);
            this.mlistview_devices.Name = "mlistview_devices";
            this.mlistview_devices.Size = new System.Drawing.Size(211, 226);
            this.mlistview_devices.TabIndex = 0;
            this.mlistview_devices.UseCompatibleStateImageBehavior = false;
            this.mlistview_devices.View = System.Windows.Forms.View.Details;
            //
            // formDeviceManager
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 226);
            this.Controls.Add(this.mlistview_devices);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formDeviceManager";
            this.ShowInTaskbar = false;
            this.Text = "Device Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mlistview_devices;

    }
}