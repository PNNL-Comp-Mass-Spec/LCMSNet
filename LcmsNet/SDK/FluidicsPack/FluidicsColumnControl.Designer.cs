namespace FluidicsPack
{
    partial class FluidicsColumnControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_length = new System.Windows.Forms.NumericUpDown();
            this.mnum_innerDiameter = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextBox_packingMaterial = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_length)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_innerDiameter)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inner Diameter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Length";
            // 
            // mnum_length
            // 
            this.mnum_length.Location = new System.Drawing.Point(120, 51);
            this.mnum_length.Name = "mnum_length";
            this.mnum_length.Size = new System.Drawing.Size(104, 20);
            this.mnum_length.TabIndex = 2;
            this.mnum_length.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mnum_innerDiameter
            // 
            this.mnum_innerDiameter.Location = new System.Drawing.Point(120, 10);
            this.mnum_innerDiameter.Name = "mnum_innerDiameter";
            this.mnum_innerDiameter.Size = new System.Drawing.Size(104, 20);
            this.mnum_innerDiameter.TabIndex = 3;
            this.mnum_innerDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Packing Material";
            // 
            // mtextBox_packingMaterial
            // 
            this.mtextBox_packingMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_packingMaterial.Location = new System.Drawing.Point(120, 88);
            this.mtextBox_packingMaterial.Multiline = true;
            this.mtextBox_packingMaterial.Name = "mtextBox_packingMaterial";
            this.mtextBox_packingMaterial.Size = new System.Drawing.Size(256, 111);
            this.mtextBox_packingMaterial.TabIndex = 5;
            // 
            // FluidicsColumnControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mtextBox_packingMaterial);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mnum_innerDiameter);
            this.Controls.Add(this.mnum_length);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FluidicsColumnControl";
            this.Size = new System.Drawing.Size(390, 218);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_length)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_innerDiameter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_length;
        private System.Windows.Forms.NumericUpDown mnum_innerDiameter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mtextBox_packingMaterial;
    }
}
