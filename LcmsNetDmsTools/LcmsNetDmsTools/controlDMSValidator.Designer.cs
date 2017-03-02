namespace LcmsNetDmsTools
{
    partial class controlDMSValidator
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
            this.mlabel_sampleName = new System.Windows.Forms.Label();
            this.mtextbox_proposalID = new System.Windows.Forms.TextBox();
            this.mtextbox_user = new System.Windows.Forms.TextBox();
            this.mnum_requestNumber = new System.Windows.Forms.NumericUpDown();
            this.mtextBox_experimentName = new System.Windows.Forms.TextBox();
            this.mpictureBox_glyph = new System.Windows.Forms.PictureBox();
            this.mcomboBox_usageType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_requestNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_glyph)).BeginInit();
            this.SuspendLayout();
            // 
            // mlabel_sampleName
            // 
            this.mlabel_sampleName.AutoSize = true;
            this.mlabel_sampleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_sampleName.Location = new System.Drawing.Point(26, 2);
            this.mlabel_sampleName.Name = "mlabel_sampleName";
            this.mlabel_sampleName.Size = new System.Drawing.Size(70, 13);
            this.mlabel_sampleName.TabIndex = 0;
            this.mlabel_sampleName.Text = "SampleName";
            // 
            // mtextbox_proposalID
            // 
            this.mtextbox_proposalID.Location = new System.Drawing.Point(384, -1);
            this.mtextbox_proposalID.Name = "mtextbox_proposalID";
            this.mtextbox_proposalID.Size = new System.Drawing.Size(101, 20);
            this.mtextbox_proposalID.TabIndex = 2;
            this.mtextbox_proposalID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mtextbox_user
            // 
            this.mtextbox_user.BackColor = System.Drawing.Color.White;
            this.mtextbox_user.Location = new System.Drawing.Point(491, 0);
            this.mtextbox_user.Name = "mtextbox_user";
            this.mtextbox_user.Size = new System.Drawing.Size(120, 20);
            this.mtextbox_user.TabIndex = 3;
            this.mtextbox_user.TextChanged += new System.EventHandler(this.mtextbox_user_TextChanged);
            // 
            // mnum_requestNumber
            // 
            this.mnum_requestNumber.Location = new System.Drawing.Point(165, 0);
            this.mnum_requestNumber.Maximum = new decimal(new int[] {
            1569325056,
            23283064,
            0,
            0});
            this.mnum_requestNumber.Name = "mnum_requestNumber";
            this.mnum_requestNumber.Size = new System.Drawing.Size(119, 20);
            this.mnum_requestNumber.TabIndex = 0;
            this.mnum_requestNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_requestNumber.ValueChanged += new System.EventHandler(this.mnum_requestNumber_ValueChanged);
            // 
            // mtextBox_experimentName
            // 
            this.mtextBox_experimentName.Location = new System.Drawing.Point(617, 0);
            this.mtextBox_experimentName.Name = "mtextBox_experimentName";
            this.mtextBox_experimentName.Size = new System.Drawing.Size(88, 20);
            this.mtextBox_experimentName.TabIndex = 4;
            // 
            // mpictureBox_glyph
            // 
            this.mpictureBox_glyph.Location = new System.Drawing.Point(4, 0);
            this.mpictureBox_glyph.Name = "mpictureBox_glyph";
            this.mpictureBox_glyph.Size = new System.Drawing.Size(20, 20);
            this.mpictureBox_glyph.TabIndex = 11;
            this.mpictureBox_glyph.TabStop = false;
            // 
            // mcomboBox_usageType
            // 
            this.mcomboBox_usageType.FormattingEnabled = true;
            this.mcomboBox_usageType.Items.AddRange(new object[] {
            "Broken",
            "Cap_Dev",
            "Maintenance",
            "User",
            "User_Unknown"});
            this.mcomboBox_usageType.Location = new System.Drawing.Point(290, 0);
            this.mcomboBox_usageType.Name = "mcomboBox_usageType";
            this.mcomboBox_usageType.Size = new System.Drawing.Size(88, 21);
            this.mcomboBox_usageType.Sorted = true;
            this.mcomboBox_usageType.TabIndex = 1;
            // 
            // controlDMSValidator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mcomboBox_usageType);
            this.Controls.Add(this.mpictureBox_glyph);
            this.Controls.Add(this.mtextBox_experimentName);
            this.Controls.Add(this.mnum_requestNumber);
            this.Controls.Add(this.mtextbox_user);
            this.Controls.Add(this.mtextbox_proposalID);
            this.Controls.Add(this.mlabel_sampleName);
            this.Name = "controlDMSValidator";
            this.Size = new System.Drawing.Size(708, 20);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_requestNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_glyph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mlabel_sampleName;
        private System.Windows.Forms.TextBox mtextbox_proposalID;
        private System.Windows.Forms.TextBox mtextbox_user;
        private System.Windows.Forms.NumericUpDown mnum_requestNumber;
        private System.Windows.Forms.TextBox mtextBox_experimentName;
        private System.Windows.Forms.PictureBox mpictureBox_glyph;
        private System.Windows.Forms.ComboBox mcomboBox_usageType;
    }
}
