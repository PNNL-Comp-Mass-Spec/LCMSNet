namespace LcmsNet
{
    partial class formSplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formSplashScreen));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.mlabel_version = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblDevelopers = new System.Windows.Forms.Label();
            this.mlabel_emulated = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::LcmsNet.Properties.Resources.LcmsNet;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(955, 201);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStatus.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(0, 525);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(955, 82);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Status";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_version
            // 
            this.mlabel_version.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_version.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_version.Location = new System.Drawing.Point(0, 201);
            this.mlabel_version.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mlabel_version.Name = "mlabel_version";
            this.mlabel_version.Size = new System.Drawing.Size(955, 86);
            this.mlabel_version.TabIndex = 3;
            this.mlabel_version.Text = "Version: ";
            this.mlabel_version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblCopyright);
            this.panel1.Controls.Add(this.lblDevelopers);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.mlabel_emulated);
            this.panel1.Controls.Add(this.mlabel_version);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(13, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(955, 607);
            this.panel1.TabIndex = 4;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCopyright.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(0, 381);
            this.lblCopyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(955, 39);
            this.lblCopyright.TabIndex = 5;
            this.lblCopyright.Text = "Copyright Battelle Memorial Institute, 2017";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDevelopers
            // 
            this.lblDevelopers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDevelopers.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDevelopers.Location = new System.Drawing.Point(62, 417);
            this.lblDevelopers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDevelopers.Name = "lblDevelopers";
            this.lblDevelopers.Size = new System.Drawing.Size(839, 108);
            this.lblDevelopers.TabIndex = 4;
            this.lblDevelopers.Text = "Developers: populated at runtimne";
            this.lblDevelopers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_emulated
            // 
            this.mlabel_emulated.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_emulated.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_emulated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.mlabel_emulated.Location = new System.Drawing.Point(0, 287);
            this.mlabel_emulated.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mlabel_emulated.Name = "mlabel_emulated";
            this.mlabel_emulated.Size = new System.Drawing.Size(955, 94);
            this.mlabel_emulated.TabIndex = 6;
            this.mlabel_emulated.Text = "Cart";
            this.mlabel_emulated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // formSplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(53)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(981, 631);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "formSplashScreen";
            this.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LcmsNet V";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label mlabel_version;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblDevelopers;
        private System.Windows.Forms.Label mlabel_emulated;
    }
}