namespace LcmsNet.SampleQueue.Forms
{
    partial class formSampleQueueOptions
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
            this.mcheckbox_displayFullName = new System.Windows.Forms.CheckBox();
            this.mgroupBox_triggerFileOptions = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mgroupBox_triggerFileOptions.SuspendLayout();
            this.SuspendLayout();
            //
            // mcheckbox_displayFullName
            //
            this.mcheckbox_displayFullName.AutoSize = true;
            this.mcheckbox_displayFullName.Location = new System.Drawing.Point(27, 12);
            this.mcheckbox_displayFullName.Name = "mcheckbox_displayFullName";
            this.mcheckbox_displayFullName.Size = new System.Drawing.Size(208, 17);
            this.mcheckbox_displayFullName.TabIndex = 0;
            this.mcheckbox_displayFullName.Text = "Display Full Name (Column, Date, Cart)";
            this.mcheckbox_displayFullName.UseVisualStyleBackColor = true;
            //
            // mgroupBox_triggerFileOptions
            //
            this.mgroupBox_triggerFileOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_triggerFileOptions.Controls.Add(this.checkBox3);
            this.mgroupBox_triggerFileOptions.Controls.Add(this.checkBox2);
            this.mgroupBox_triggerFileOptions.Controls.Add(this.checkBox1);
            this.mgroupBox_triggerFileOptions.Location = new System.Drawing.Point(12, 49);
            this.mgroupBox_triggerFileOptions.Name = "mgroupBox_triggerFileOptions";
            this.mgroupBox_triggerFileOptions.Size = new System.Drawing.Size(391, 137);
            this.mgroupBox_triggerFileOptions.TabIndex = 1;
            this.mgroupBox_triggerFileOptions.TabStop = false;
            this.mgroupBox_triggerFileOptions.Text = "Trigger File Options";
            //
            // checkBox1
            //
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 31);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(285, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Use Full Name (Column, Date, Cart) in trigger file output";
            this.checkBox1.UseVisualStyleBackColor = true;
            //
            // checkBox2
            //
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(15, 66);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(149, 17);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "Copy Trigger Files to DMS";
            this.checkBox2.UseVisualStyleBackColor = true;
            //
            // checkBox3
            //
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(15, 105);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(117, 17);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "Create Trigger Files";
            this.checkBox3.ThreeState = true;
            this.checkBox3.UseVisualStyleBackColor = true;
            //
            // mbutton_ok
            //
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(170, 195);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(82, 42);
            this.mbutton_ok.TabIndex = 2;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            //
            // formSampleQueueOptions
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 242);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mgroupBox_triggerFileOptions);
            this.Controls.Add(this.mcheckbox_displayFullName);
            this.Name = "formSampleQueueOptions";
            this.Text = "Options";
            this.mgroupBox_triggerFileOptions.ResumeLayout(false);
            this.mgroupBox_triggerFileOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox mcheckbox_displayFullName;
        private System.Windows.Forms.GroupBox mgroupBox_triggerFileOptions;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button mbutton_ok;
    }
}