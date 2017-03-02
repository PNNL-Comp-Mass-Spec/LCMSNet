namespace LcmsNet
{
    partial class ModelCheckControl
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
            this.chkboxModel = new System.Windows.Forms.CheckBox();
            this.comboCategories = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // chkboxModel
            //
            this.chkboxModel.AutoSize = true;
            this.chkboxModel.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkboxModel.Location = new System.Drawing.Point(0, 0);
            this.chkboxModel.Name = "chkboxModel";
            this.chkboxModel.Size = new System.Drawing.Size(80, 31);
            this.chkboxModel.TabIndex = 0;
            this.chkboxModel.Text = "checkBox1";
            this.chkboxModel.UseVisualStyleBackColor = true;
            this.chkboxModel.CheckedChanged += new System.EventHandler(this.chkboxModel_CheckedChanged);
            //
            // comboCategories
            //
            this.comboCategories.FormattingEnabled = true;
            this.comboCategories.Location = new System.Drawing.Point(332, 3);
            this.comboCategories.Name = "comboCategories";
            this.comboCategories.Size = new System.Drawing.Size(121, 21);
            this.comboCategories.TabIndex = 1;
            this.comboCategories.SelectedIndexChanged += new System.EventHandler(this.comboCategories_SelectedIndexChanged);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Display as:";
            //
            // ModelCheckControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboCategories);
            this.Controls.Add(this.chkboxModel);
            this.Name = "ModelCheckControl";
            this.Size = new System.Drawing.Size(465, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkboxModel;
        private System.Windows.Forms.ComboBox comboCategories;
        private System.Windows.Forms.Label label1;
    }
}
