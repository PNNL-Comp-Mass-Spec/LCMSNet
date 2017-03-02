namespace LcmsNet.Devices.BrukerStart
{
    partial class controlBrukerStartGlyph
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
            this.mpicture_device = new System.Windows.Forms.PictureBox();
            this.mlabel_name = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_device)).BeginInit();
            this.SuspendLayout();
            // 
            // mpicture_device
            // 
            this.mpicture_device.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpicture_device.Image = global::PNNLDevices.Properties.Resources.BrukerStart;
            this.mpicture_device.InitialImage = global::PNNLDevices.Properties.Resources.PAL;
            this.mpicture_device.Location = new System.Drawing.Point(0, 13);
            this.mpicture_device.Name = "mpicture_device";
            this.mpicture_device.Size = new System.Drawing.Size(108, 69);
            this.mpicture_device.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.mpicture_device.TabIndex = 3;
            this.mpicture_device.TabStop = false;
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(108, 13);
            this.mlabel_name.TabIndex = 4;
            this.mlabel_name.Text = "Bruker";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // controlBrukerStartGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mpicture_device);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlBrukerStartGlyph";
            this.Size = new System.Drawing.Size(108, 82);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_device)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mpicture_device;
        private System.Windows.Forms.Label mlabel_name;
    }
}
