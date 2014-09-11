namespace LcmsNet.SampleQueue.Forms
{
    partial class formMoveToColumnSelector
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
            this.mbutton_column1 = new System.Windows.Forms.Button();
            this.mbutton_column2 = new System.Windows.Forms.Button();
            this.mbutton_column3 = new System.Windows.Forms.Button();
            this.mbutton_column4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mcheckbox_fillIn = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mbutton_column1
            // 
            this.mbutton_column1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_column1.Location = new System.Drawing.Point(21, 31);
            this.mbutton_column1.Name = "mbutton_column1";
            this.mbutton_column1.Size = new System.Drawing.Size(109, 81);
            this.mbutton_column1.TabIndex = 0;
            this.mbutton_column1.Text = "1";
            this.mbutton_column1.UseVisualStyleBackColor = true;
            this.mbutton_column1.Click += new System.EventHandler(this.mbutton_column1_Click);
            // 
            // mbutton_column2
            // 
            this.mbutton_column2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_column2.Location = new System.Drawing.Point(136, 31);
            this.mbutton_column2.Name = "mbutton_column2";
            this.mbutton_column2.Size = new System.Drawing.Size(109, 81);
            this.mbutton_column2.TabIndex = 1;
            this.mbutton_column2.Text = "2";
            this.mbutton_column2.UseVisualStyleBackColor = true;
            this.mbutton_column2.Click += new System.EventHandler(this.mbutton_column2_Click);
            // 
            // mbutton_column3
            // 
            this.mbutton_column3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_column3.Location = new System.Drawing.Point(251, 31);
            this.mbutton_column3.Name = "mbutton_column3";
            this.mbutton_column3.Size = new System.Drawing.Size(109, 81);
            this.mbutton_column3.TabIndex = 2;
            this.mbutton_column3.Text = "3";
            this.mbutton_column3.UseVisualStyleBackColor = true;
            this.mbutton_column3.Click += new System.EventHandler(this.mbutton_column3_Click);
            // 
            // mbutton_column4
            // 
            this.mbutton_column4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_column4.Location = new System.Drawing.Point(366, 31);
            this.mbutton_column4.Name = "mbutton_column4";
            this.mbutton_column4.Size = new System.Drawing.Size(109, 81);
            this.mbutton_column4.TabIndex = 3;
            this.mbutton_column4.Text = "4";
            this.mbutton_column4.UseVisualStyleBackColor = true;
            this.mbutton_column4.Click += new System.EventHandler(this.mbutton_column4_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(133, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Move Samples To Column?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Location = new System.Drawing.Point(136, 167);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(224, 27);
            this.mbutton_cancel.TabIndex = 5;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            this.mbutton_cancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // mcheckbox_fillIn
            // 
            this.mcheckbox_fillIn.AutoSize = true;
            this.mcheckbox_fillIn.Checked = true;
            this.mcheckbox_fillIn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcheckbox_fillIn.Location = new System.Drawing.Point(21, 133);
            this.mcheckbox_fillIn.Name = "mcheckbox_fillIn";
            this.mcheckbox_fillIn.Size = new System.Drawing.Size(110, 17);
            this.mcheckbox_fillIn.TabIndex = 6;
            this.mcheckbox_fillIn.Text = "Fill in any un-used";
            this.mcheckbox_fillIn.UseVisualStyleBackColor = true;
            // 
            // formMoveToColumnSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 206);
            this.Controls.Add(this.mcheckbox_fillIn);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mbutton_column4);
            this.Controls.Add(this.mbutton_column3);
            this.Controls.Add(this.mbutton_column2);
            this.Controls.Add(this.mbutton_column1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formMoveToColumnSelector";
            this.Text = "Select Column";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_column1;
        private System.Windows.Forms.Button mbutton_column2;
        private System.Windows.Forms.Button mbutton_column3;
        private System.Windows.Forms.Button mbutton_column4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mbutton_cancel;
        private System.Windows.Forms.CheckBox mcheckbox_fillIn;
    }
}