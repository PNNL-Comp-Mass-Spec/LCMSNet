namespace LcmsNet.SampleQueue.Forms
{
    partial class formSampleBadBlockDisplay
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
            this.mlistview_samples = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(645, 49);
            this.label1.TabIndex = 0;
            this.label1.Text = "These samples were blocked but selected to run on different columns or use differ" +
                "ent LC-Methods";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mlistview_samples
            // 
            this.mlistview_samples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistview_samples.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader3,
            this.columnHeader4});
            this.mlistview_samples.FullRowSelect = true;
            this.mlistview_samples.GridLines = true;
            this.mlistview_samples.Location = new System.Drawing.Point(-3, 70);
            this.mlistview_samples.Name = "mlistview_samples";
            this.mlistview_samples.Size = new System.Drawing.Size(660, 290);
            this.mlistview_samples.TabIndex = 1;
            this.mlistview_samples.UseCompatibleStateImageBehavior = false;
            this.mlistview_samples.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Batch";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Block";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Column #";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Dataset Name";
            this.columnHeader3.Width = 203;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "LC-Method";
            this.columnHeader4.Width = 225;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(117, 366);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(199, 39);
            this.mbutton_ok.TabIndex = 2;
            this.mbutton_ok.Text = "OK, I want to run in this order";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(331, 366);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(195, 39);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel, do not run!";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // formSampleBadBlockDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(657, 408);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mlistview_samples);
            this.Controls.Add(this.label1);
            this.Name = "formSampleBadBlockDisplay";
            this.Text = "Blocked Sample Errors";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView mlistview_samples;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button button1;
    }
}