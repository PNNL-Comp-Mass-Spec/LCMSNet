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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(716, 163);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            //
            // labelStatus
            //
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStatus.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(0, 426);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(716, 67);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Sample Text";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // mlabel_version
            //
            this.mlabel_version.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_version.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_version.Location = new System.Drawing.Point(0, 163);
            this.mlabel_version.Name = "mlabel_version";
            this.mlabel_version.Size = new System.Drawing.Size(716, 70);
            this.mlabel_version.TabIndex = 3;
            this.mlabel_version.Text = "Version: ";
            this.mlabel_version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.mlabel_emulated);
            this.panel1.Controls.Add(this.mlabel_version);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(716, 493);
            this.panel1.TabIndex = 4;
            //
            // label2
            //
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 374);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(716, 32);
            this.label2.TabIndex = 5;
            this.label2.Text = "Copyright Battelle Memorial Institute, 2011";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // label1
            //
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(716, 65);
            this.label1.TabIndex = 4;
            this.label1.Text = "Brian LaMarche, Christopher Walters, David Clark, Danny Orton, John Ryan\r\nRon Moo" +
    "re, Derek Hopkins, Gordon Anderson, Richard Smith";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // mlabel_emulated
            //
            this.mlabel_emulated.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_emulated.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_emulated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.mlabel_emulated.Location = new System.Drawing.Point(0, 233);
            this.mlabel_emulated.Name = "mlabel_emulated";
            this.mlabel_emulated.Size = new System.Drawing.Size(716, 76);
            this.mlabel_emulated.TabIndex = 6;
            this.mlabel_emulated.Text = "Cart";
            this.mlabel_emulated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // formSplashScreen
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(53)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(736, 513);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formSplashScreen";
            this.Padding = new System.Windows.Forms.Padding(10);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mlabel_emulated;
    }
}