namespace LcmsNet.SampleQueue.Forms
{
    partial class formInsertOntoUnusedDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formInsertOntoUnusedDialog));
            this.mlabel_insertDescription = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // mlabel_insertDescription
            //
            this.mlabel_insertDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_insertDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_insertDescription.Location = new System.Drawing.Point(7, 9);
            this.mlabel_insertDescription.Name = "mlabel_insertDescription";
            this.mlabel_insertDescription.Size = new System.Drawing.Size(362, 42);
            this.mlabel_insertDescription.TabIndex = 2;
            this.mlabel_insertDescription.Text = "We noticed there are some unused samples.  What do you want to do?";
            this.mlabel_insertDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // button2
            //
            this.button2.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button2.Image = global::LcmsNet.Properties.Resources.Append;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(195, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(170, 100);
            this.button2.TabIndex = 1;
            this.button2.Text = "Append the samples";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            //
            // button1
            //
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(11, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(170, 100);
            this.button1.TabIndex = 0;
            this.button1.Text = "Insert onto Unused";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            //
            // formInsertOntoUnusedDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 158);
            this.ControlBox = false;
            this.Controls.Add(this.mlabel_insertDescription);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formInsertOntoUnusedDialog";
            this.Text = "Insert Into Unused";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label mlabel_insertDescription;
    }
}