namespace LcmsNet.Devices.Pumps
{
	partial class controlPumpIscoGlyph
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.mpicture_pumps = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_pumps)).BeginInit();
            this.SuspendLayout();
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(72, 25);
            this.mlabel_name.TabIndex = 5;
            this.mlabel_name.Text = "ISCO Pumps";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mpicture_pumps);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(72, 69);
            this.panel1.TabIndex = 9;
            // 
            // mpicture_pumps
            // 
            this.mpicture_pumps.Image = global::PNNLDevices.Properties.Resources.ISCO;
            this.mpicture_pumps.InitialImage = null;
            this.mpicture_pumps.Location = new System.Drawing.Point(0, 0);
            this.mpicture_pumps.Name = "mpicture_pumps";
            this.mpicture_pumps.Size = new System.Drawing.Size(70, 67);
            this.mpicture_pumps.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mpicture_pumps.TabIndex = 11;
            this.mpicture_pumps.TabStop = false;
            // 
            // controlPumpIscoGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlPumpIscoGlyph";
            this.Size = new System.Drawing.Size(72, 94);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_pumps)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label mlabel_name;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox mpicture_pumps;
	}
}
