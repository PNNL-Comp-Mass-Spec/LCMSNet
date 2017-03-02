namespace LcmsNet.SampleQueue.Forms
{
    partial class formSampleRandomizer
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
            this.buttonRandomize = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewInput = new System.Windows.Forms.ListView();
            this.lvInputColSeq = new System.Windows.Forms.ColumnHeader();
            this.lvInputColName = new System.Windows.Forms.ColumnHeader();
            this.listViewOutput = new System.Windows.Forms.ListView();
            this.lvOutColSeq = new System.Windows.Forms.ColumnHeader();
            this.lvOutColName = new System.Windows.Forms.ColumnHeader();
            this.comboBoxRandomizers = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            //
            // buttonRandomize
            //
            this.buttonRandomize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRandomize.Location = new System.Drawing.Point(6, 159);
            this.buttonRandomize.Name = "buttonRandomize";
            this.buttonRandomize.Size = new System.Drawing.Size(121, 38);
            this.buttonRandomize.TabIndex = 0;
            this.buttonRandomize.Text = "Randomize";
            this.buttonRandomize.UseVisualStyleBackColor = true;
            this.buttonRandomize.Click += new System.EventHandler(this.buttonRandomize_Click);
            //
            // buttonOK
            //
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonOK.Location = new System.Drawing.Point(241, 440);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 39);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.Location = new System.Drawing.Point(347, 440);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(98, 39);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            //
            // listViewInput
            //
            this.listViewInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewInput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvInputColSeq,
            this.lvInputColName});
            this.listViewInput.GridLines = true;
            this.listViewInput.Location = new System.Drawing.Point(12, 12);
            this.listViewInput.Name = "listViewInput";
            this.listViewInput.Size = new System.Drawing.Size(300, 423);
            this.listViewInput.TabIndex = 3;
            this.listViewInput.UseCompatibleStateImageBehavior = false;
            this.listViewInput.View = System.Windows.Forms.View.Details;
            //
            // lvInputColSeq
            //
            this.lvInputColSeq.Text = "Seq #";
            this.lvInputColSeq.Width = 49;
            //
            // lvInputColName
            //
            this.lvInputColName.Text = "Name";
            this.lvInputColName.Width = 167;
            //
            // listViewOutput
            //
            this.listViewOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvOutColSeq,
            this.lvOutColName});
            this.listViewOutput.GridLines = true;
            this.listViewOutput.Location = new System.Drawing.Point(133, 12);
            this.listViewOutput.Name = "listViewOutput";
            this.listViewOutput.Size = new System.Drawing.Size(319, 422);
            this.listViewOutput.TabIndex = 4;
            this.listViewOutput.UseCompatibleStateImageBehavior = false;
            this.listViewOutput.View = System.Windows.Forms.View.Details;
            //
            // lvOutColSeq
            //
            this.lvOutColSeq.Text = "Seq #";
            this.lvOutColSeq.Width = 49;
            //
            // lvOutColName
            //
            this.lvOutColName.Text = "Name";
            this.lvOutColName.Width = 167;
            //
            // comboBoxRandomizers
            //
            this.comboBoxRandomizers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRandomizers.FormattingEnabled = true;
            this.comboBoxRandomizers.Location = new System.Drawing.Point(6, 132);
            this.comboBoxRandomizers.Name = "comboBoxRandomizers";
            this.comboBoxRandomizers.Size = new System.Drawing.Size(121, 21);
            this.comboBoxRandomizers.TabIndex = 5;
            //
            // label1
            //
            this.label1.Location = new System.Drawing.Point(6, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Randomization Type";
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 482);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(773, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 17);
            this.statusLabel.Text = "Ready.";
            //
            // panel1
            //
            this.panel1.Controls.Add(this.listViewInput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(318, 482);
            this.panel1.TabIndex = 8;
            //
            // panel2
            //
            this.panel2.Controls.Add(this.buttonRandomize);
            this.panel2.Controls.Add(this.comboBoxRandomizers);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.listViewOutput);
            this.panel2.Controls.Add(this.buttonOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(318, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(455, 482);
            this.panel2.TabIndex = 9;
            //
            // formSampleRandomizer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 504);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "formSampleRandomizer";
            this.Text = "Randomize Samples";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRandomize;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewInput;
        private System.Windows.Forms.ColumnHeader lvInputColSeq;
        private System.Windows.Forms.ColumnHeader lvInputColName;
        private System.Windows.Forms.ListView listViewOutput;
        private System.Windows.Forms.ColumnHeader lvOutColSeq;
        private System.Windows.Forms.ColumnHeader lvOutColName;
          private System.Windows.Forms.ComboBox comboBoxRandomizers;
          private System.Windows.Forms.Label label1;
          private System.Windows.Forms.StatusStrip statusStrip1;
          private System.Windows.Forms.ToolStripStatusLabel statusLabel;
          private System.Windows.Forms.Panel panel1;
          private System.Windows.Forms.Panel panel2;
    }
}