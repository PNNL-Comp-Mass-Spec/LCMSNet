namespace LcmsNet.Devices.Pumps
{
    partial class controlPumpAgilentGlyph
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mlabel_flowrate = new System.Windows.Forms.Label();
            this.mlabel_composition = new System.Windows.Forms.Label();
            this.mlabel_pressure = new System.Windows.Forms.Label();
            this.mbutton_purge = new System.Windows.Forms.Button();
            this.mpicture_pumps = new System.Windows.Forms.PictureBox();
            this.mlabel_name = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_pumps)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.mbutton_purge);
            this.panel1.Controls.Add(this.mpicture_pumps);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(228, 108);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.mlabel_flowrate);
            this.panel2.Controls.Add(this.mlabel_composition);
            this.panel2.Controls.Add(this.mlabel_pressure);
            this.panel2.Location = new System.Drawing.Point(77, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(148, 95);
            this.panel2.TabIndex = 13;
            // 
            // mlabel_flowrate
            // 
            this.mlabel_flowrate.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_flowrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_flowrate.Location = new System.Drawing.Point(0, 52);
            this.mlabel_flowrate.Name = "mlabel_flowrate";
            this.mlabel_flowrate.Size = new System.Drawing.Size(148, 28);
            this.mlabel_flowrate.TabIndex = 12;
            this.mlabel_flowrate.Text = "Flow: ---";
            // 
            // mlabel_composition
            // 
            this.mlabel_composition.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_composition.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_composition.Location = new System.Drawing.Point(0, 27);
            this.mlabel_composition.Name = "mlabel_composition";
            this.mlabel_composition.Size = new System.Drawing.Size(148, 25);
            this.mlabel_composition.TabIndex = 11;
            this.mlabel_composition.Text = "Comp B: ---";
            // 
            // mlabel_pressure
            // 
            this.mlabel_pressure.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_pressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_pressure.Location = new System.Drawing.Point(0, 0);
            this.mlabel_pressure.Name = "mlabel_pressure";
            this.mlabel_pressure.Size = new System.Drawing.Size(148, 27);
            this.mlabel_pressure.TabIndex = 10;
            this.mlabel_pressure.Text = "Pressure: ---";
            // 
            // mbutton_purge
            // 
            this.mbutton_purge.Location = new System.Drawing.Point(1, 72);
            this.mbutton_purge.Name = "mbutton_purge";
            this.mbutton_purge.Size = new System.Drawing.Size(75, 32);
            this.mbutton_purge.TabIndex = 12;
            this.mbutton_purge.Text = "PURGE";
            this.mbutton_purge.UseVisualStyleBackColor = true;
            this.mbutton_purge.Click += new System.EventHandler(this.mbutton_purge_Click);
            // 
            // mpicture_pumps
            // 
            this.mpicture_pumps.Image = global::PNNLDevices.Properties.Resources.Agilent1200Pump;
            this.mpicture_pumps.InitialImage = null;
            this.mpicture_pumps.Location = new System.Drawing.Point(3, 0);
            this.mpicture_pumps.Name = "mpicture_pumps";
            this.mpicture_pumps.Size = new System.Drawing.Size(68, 66);
            this.mpicture_pumps.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mpicture_pumps.TabIndex = 11;
            this.mpicture_pumps.TabStop = false;
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(228, 23);
            this.mlabel_name.TabIndex = 11;
            this.mlabel_name.Text = "Agilent Pumps";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // controlPumpAgilentGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlPumpAgilentGlyph";
            this.Size = new System.Drawing.Size(228, 131);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_pumps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox mpicture_pumps;
        private System.Windows.Forms.Button mbutton_purge;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label mlabel_flowrate;
        private System.Windows.Forms.Label mlabel_composition;
        private System.Windows.Forms.Label mlabel_pressure;
        private System.Windows.Forms.Label mlabel_name;
    }
}
