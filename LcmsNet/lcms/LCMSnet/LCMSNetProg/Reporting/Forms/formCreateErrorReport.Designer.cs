namespace LcmsNet.Reporting.Forms
{
    partial class formCreateErrorReport
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
            this.mbutton_create = new System.Windows.Forms.Button();
            this.mlistbox_methods = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mbutton_create
            // 
            this.mbutton_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_create.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_create.Location = new System.Drawing.Point(43, 305);
            this.mbutton_create.Name = "mbutton_create";
            this.mbutton_create.Size = new System.Drawing.Size(101, 38);
            this.mbutton_create.TabIndex = 0;
            this.mbutton_create.Text = "Create";
            this.mbutton_create.UseVisualStyleBackColor = true;
            this.mbutton_create.Click += new System.EventHandler(this.mbutton_create_Click);
            // 
            // mlistbox_methods
            // 
            this.mlistbox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistbox_methods.FormattingEnabled = true;
            this.mlistbox_methods.Location = new System.Drawing.Point(12, 35);
            this.mlistbox_methods.Name = "mlistbox_methods";
            this.mlistbox_methods.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.mlistbox_methods.Size = new System.Drawing.Size(238, 264);
            this.mlistbox_methods.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select LC Methods To Add to Report";
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.Location = new System.Drawing.Point(149, 305);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(101, 38);
            this.mbutton_cancel.TabIndex = 3;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            // 
            // formCreateErrorReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 355);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mlistbox_methods);
            this.Controls.Add(this.mbutton_create);
            this.Name = "formCreateErrorReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error Report Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_create;
        private System.Windows.Forms.ListBox mlistbox_methods;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mbutton_cancel;
    }
}