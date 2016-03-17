namespace LcmsNet.SampleQueue.Forms
{
    partial class formTrayVialAssignment
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
            if (mobject_Trays != null)
            {
                foreach (controlTray currControl in mobject_Trays)
                {
                    currControl.RowModified -= UpdateTabDisplays;
                    currControl.Dispose();
                }
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.tabControlPlates = new System.Windows.Forms.TabControl();
            this.tabPageOverall = new System.Windows.Forms.TabPage();
            this.radbtnUnassigned = new System.Windows.Forms.RadioButton();
            this.radbtnAll = new System.Windows.Forms.RadioButton();
            this.tabPagePlate1 = new System.Windows.Forms.TabPage();
            this.tabPagePlate2 = new System.Windows.Forms.TabPage();
            this.tabPagePlate3 = new System.Windows.Forms.TabPage();
            this.tabPagePlate4 = new System.Windows.Forms.TabPage();
            this.tabPagePlate5 = new System.Windows.Forms.TabPage();
            this.tabPagePlate6 = new System.Windows.Forms.TabPage();
            this.tabControlPlates.SuspendLayout();
            this.tabPageOverall.SuspendLayout();
            this.SuspendLayout();
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(829, 457);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(79, 35);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            //
            // buttonApply
            //
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(728, 457);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(95, 35);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            //
            // tabControlPlates
            //
            this.tabControlPlates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlPlates.Controls.Add(this.tabPageOverall);
            this.tabControlPlates.Controls.Add(this.tabPagePlate1);
            this.tabControlPlates.Controls.Add(this.tabPagePlate2);
            this.tabControlPlates.Controls.Add(this.tabPagePlate3);
            this.tabControlPlates.Controls.Add(this.tabPagePlate4);
            this.tabControlPlates.Controls.Add(this.tabPagePlate5);
            this.tabControlPlates.Controls.Add(this.tabPagePlate6);
            this.tabControlPlates.Location = new System.Drawing.Point(12, 12);
            this.tabControlPlates.Name = "tabControlPlates";
            this.tabControlPlates.SelectedIndex = 0;
            this.tabControlPlates.Size = new System.Drawing.Size(896, 439);
            this.tabControlPlates.TabIndex = 2;
            //
            // tabPageOverall
            //
            this.tabPageOverall.Controls.Add(this.radbtnUnassigned);
            this.tabPageOverall.Controls.Add(this.radbtnAll);
            this.tabPageOverall.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverall.Name = "tabPageOverall";
            this.tabPageOverall.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOverall.Size = new System.Drawing.Size(888, 413);
            this.tabPageOverall.TabIndex = 0;
            this.tabPageOverall.Text = "Unassigned (0)";
            this.tabPageOverall.UseVisualStyleBackColor = true;
            //
            // radbtnUnassigned
            //
            this.radbtnUnassigned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radbtnUnassigned.AutoSize = true;
            this.radbtnUnassigned.Checked = true;
            this.radbtnUnassigned.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radbtnUnassigned.Location = new System.Drawing.Point(97, 379);
            this.radbtnUnassigned.Name = "radbtnUnassigned";
            this.radbtnUnassigned.Size = new System.Drawing.Size(105, 17);
            this.radbtnUnassigned.TabIndex = 4;
            this.radbtnUnassigned.TabStop = true;
            this.radbtnUnassigned.Text = "Unassigned Only";
            this.radbtnUnassigned.UseVisualStyleBackColor = true;
            this.radbtnUnassigned.Click += new System.EventHandler(this.radbtnUnassigned_Click);
            //
            // radbtnAll
            //
            this.radbtnAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radbtnAll.AutoSize = true;
            this.radbtnAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radbtnAll.Location = new System.Drawing.Point(16, 379);
            this.radbtnAll.Name = "radbtnAll";
            this.radbtnAll.Size = new System.Drawing.Size(66, 17);
            this.radbtnAll.TabIndex = 3;
            this.radbtnAll.Text = "Show All";
            this.radbtnAll.UseVisualStyleBackColor = true;
            this.radbtnAll.Click += new System.EventHandler(this.radbtnAll_Click);
            //
            // tabPagePlate1
            //
            this.tabPagePlate1.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate1.Name = "tabPagePlate1";
            this.tabPagePlate1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlate1.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate1.TabIndex = 1;
            this.tabPagePlate1.Text = "Tray 1 (0)";
            this.tabPagePlate1.UseVisualStyleBackColor = true;
            //
            // tabPagePlate2
            //
            this.tabPagePlate2.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate2.Name = "tabPagePlate2";
            this.tabPagePlate2.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate2.TabIndex = 2;
            this.tabPagePlate2.Text = "Tray 2 (0)";
            this.tabPagePlate2.UseVisualStyleBackColor = true;
            //
            // tabPagePlate3
            //
            this.tabPagePlate3.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate3.Name = "tabPagePlate3";
            this.tabPagePlate3.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate3.TabIndex = 3;
            this.tabPagePlate3.Text = "Tray 3 (0)";
            this.tabPagePlate3.UseVisualStyleBackColor = true;
            //
            // tabPagePlate4
            //
            this.tabPagePlate4.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate4.Name = "tabPagePlate4";
            this.tabPagePlate4.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate4.TabIndex = 4;
            this.tabPagePlate4.Text = "Tray 4 (0)";
            this.tabPagePlate4.UseVisualStyleBackColor = true;
            //
            // tabPagePlate5
            //
            this.tabPagePlate5.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate5.Name = "tabPagePlate5";
            this.tabPagePlate5.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate5.TabIndex = 5;
            this.tabPagePlate5.Text = "Tray 5 (0)";
            this.tabPagePlate5.UseVisualStyleBackColor = true;
            //
            // tabPagePlate6
            //
            this.tabPagePlate6.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlate6.Name = "tabPagePlate6";
            this.tabPagePlate6.Size = new System.Drawing.Size(888, 413);
            this.tabPagePlate6.TabIndex = 6;
            this.tabPagePlate6.Text = "Tray 6 (0)";
            this.tabPagePlate6.UseVisualStyleBackColor = true;
            //
            // formTrayVialAssignment
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(920, 494);
            this.Controls.Add(this.tabControlPlates);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Name = "formTrayVialAssignment";
            this.Text = "Tray/Vial Assignment";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formTrayVialAssignment_FormClosing);
            this.tabControlPlates.ResumeLayout(false);
            this.tabPageOverall.ResumeLayout(false);
            this.tabPageOverall.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.TabControl tabControlPlates;
        private System.Windows.Forms.TabPage tabPageOverall;
        private System.Windows.Forms.TabPage tabPagePlate1;
        private System.Windows.Forms.TabPage tabPagePlate2;
        private System.Windows.Forms.TabPage tabPagePlate3;
        private System.Windows.Forms.TabPage tabPagePlate4;
        private System.Windows.Forms.TabPage tabPagePlate5;
        private System.Windows.Forms.TabPage tabPagePlate6;
        private System.Windows.Forms.RadioButton radbtnUnassigned;
        private System.Windows.Forms.RadioButton radbtnAll;
    }
}