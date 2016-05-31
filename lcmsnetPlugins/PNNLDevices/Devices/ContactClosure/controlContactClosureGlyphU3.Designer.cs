namespace LcmsNet.Devices.ContactClosure
{
    partial class controlContactClosureGlyphU3
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
            this.mlabel_name = new System.Windows.Forms.Label();
            this.mpicture_device = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_device)).BeginInit();
            this.SuspendLayout();
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(84, 13);
            this.mlabel_name.TabIndex = 8;
            this.mlabel_name.Text = "Contact Closure";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mpicture_device
            // 
            this.mpicture_device.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpicture_device.Image = global::PNNLDevices.Properties.Resources.labjack;
            this.mpicture_device.InitialImage = global::PNNLDevices.Properties.Resources.labjack;
            this.mpicture_device.Location = new System.Drawing.Point(0, 13);
            this.mpicture_device.Name = "mpicture_device";
            this.mpicture_device.Size = new System.Drawing.Size(84, 66);
            this.mpicture_device.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.mpicture_device.TabIndex = 7;
            this.mpicture_device.TabStop = false;
            // 
            // controlContactClosureGlyphU3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mpicture_device);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlContactClosureGlyphU3";
            this.Size = new System.Drawing.Size(84, 79);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_device)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mpicture_device;
        private System.Windows.Forms.Label mlabel_name;
    }
}
