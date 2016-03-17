namespace LcmsNet.Simulator
{
    partial class SimConfigControl
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
            this.btnTack = new System.Windows.Forms.Button();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.controlConfig = new LcmsNet.controlFluidicsControl();
            this.SuspendLayout();
            //
            // btnTack
            //
            this.btnTack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTack.Location = new System.Drawing.Point(668, 384);
            this.btnTack.Name = "btnTack";
            this.btnTack.Size = new System.Drawing.Size(36, 23);
            this.btnTack.TabIndex = 1;
            this.btnTack.UseVisualStyleBackColor = true;
            this.btnTack.Click += new System.EventHandler(this.btnTack_Click);
            //
            // lblElapsed
            //
            this.lblElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblElapsed.AutoSize = true;
            this.lblElapsed.BackColor = System.Drawing.Color.Transparent;
            this.lblElapsed.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblElapsed.ForeColor = System.Drawing.Color.Maroon;
            this.lblElapsed.Location = new System.Drawing.Point(3, 282);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(171, 39);
            this.lblElapsed.TabIndex = 3;
            this.lblElapsed.Text = "+00:00:00";
            //
            // controlConfig
            //
            this.controlConfig.BackColor = System.Drawing.Color.Transparent;
            this.controlConfig.DevicesLocked = false;
            this.controlConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlConfig.Location = new System.Drawing.Point(0, 0);
            this.controlConfig.Name = "controlConfig";
            this.controlConfig.Size = new System.Drawing.Size(707, 419);
            this.controlConfig.TabIndex = 2;
            //
            // SimConfigControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblElapsed);
            this.Controls.Add(this.btnTack);
            this.Controls.Add(this.controlConfig);
            this.Name = "SimConfigControl";
            this.Size = new System.Drawing.Size(707, 419);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTack;
        private controlFluidicsControl controlConfig;
        private System.Windows.Forms.Label lblElapsed;
    }
}
