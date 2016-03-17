namespace LcmsNet.Configuration
{
    partial class controlSystem
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
            this.mlabel_system = new System.Windows.Forms.Label();
            this.mbutton_color = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // mlabel_system
            //
            this.mlabel_system.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_system.AutoSize = true;
            this.mlabel_system.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_system.Location = new System.Drawing.Point(3, 0);
            this.mlabel_system.Name = "mlabel_system";
            this.mlabel_system.Size = new System.Drawing.Size(86, 24);
            this.mlabel_system.TabIndex = 21;
            this.mlabel_system.Text = "System 1";
            //
            // mbutton_color
            //
            this.mbutton_color.BackColor = System.Drawing.Color.Red;
            this.mbutton_color.Location = new System.Drawing.Point(95, 0);
            this.mbutton_color.Name = "mbutton_color";
            this.mbutton_color.Size = new System.Drawing.Size(36, 29);
            this.mbutton_color.TabIndex = 20;
            this.mbutton_color.UseVisualStyleBackColor = false;
            this.mbutton_color.Click += new System.EventHandler(this.mbutton_color_Click);
            //
            // controlSystem
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mlabel_system);
            this.Controls.Add(this.mbutton_color);
            this.Name = "controlSystem";
            this.Size = new System.Drawing.Size(139, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mlabel_system;
        private System.Windows.Forms.Button mbutton_color;
    }
}
