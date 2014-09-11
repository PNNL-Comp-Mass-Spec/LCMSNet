namespace LcmsNetDataClasses.Devices
{
    partial class formFailedDevicesDisplay
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
            this.mlistview_failedDevices = new System.Windows.Forms.ListView();
            this.mcolumn_device = new System.Windows.Forms.ColumnHeader();
            this.mcolumn_error = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mlistview_failedDevices
            // 
            this.mlistview_failedDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistview_failedDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mcolumn_device,
            this.mcolumn_error});
            this.mlistview_failedDevices.FullRowSelect = true;
            this.mlistview_failedDevices.GridLines = true;
            this.mlistview_failedDevices.Location = new System.Drawing.Point(12, 42);
            this.mlistview_failedDevices.Name = "mlistview_failedDevices";
            this.mlistview_failedDevices.Size = new System.Drawing.Size(765, 405);
            this.mlistview_failedDevices.TabIndex = 0;
            this.mlistview_failedDevices.UseCompatibleStateImageBehavior = false;
            this.mlistview_failedDevices.View = System.Windows.Forms.View.Details;
            // 
            // mcolumn_device
            // 
            this.mcolumn_device.Text = "Device Name";
            this.mcolumn_device.Width = 87;
            // 
            // mcolumn_error
            // 
            this.mcolumn_error.Text = "Error";
            this.mcolumn_error.Width = 100;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(774, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "These devices failed to initialize properly.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(360, 453);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(66, 30);
            this.mbutton_ok.TabIndex = 2;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // formFailedDevicesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(795, 491);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mlistview_failedDevices);
            this.Name = "formFailedDevicesDisplay";
            this.Text = "Devices Initialization Failures";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mlistview_failedDevices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.ColumnHeader mcolumn_device;
        private System.Windows.Forms.ColumnHeader mcolumn_error;
    }
}