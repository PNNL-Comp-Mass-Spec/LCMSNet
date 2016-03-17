namespace LcmsNet.Method.Forms
{
    partial class formFailedMethodLoadDisplay
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
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.m_tree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(763, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "These methods had issues you should review.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // mbutton_ok
            //
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(355, 458);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(66, 30);
            this.mbutton_ok.TabIndex = 2;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            //
            // m_tree
            //
            this.m_tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tree.Location = new System.Drawing.Point(16, 38);
            this.m_tree.Name = "m_tree";
            this.m_tree.Size = new System.Drawing.Size(751, 415);
            this.m_tree.TabIndex = 3;
            //
            // formFailedMethodLoadDisplay
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 491);
            this.Controls.Add(this.m_tree);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.label1);
            this.Name = "formFailedMethodLoadDisplay";
            this.Text = "Method Load Issues";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.TreeView m_tree;
    }
}