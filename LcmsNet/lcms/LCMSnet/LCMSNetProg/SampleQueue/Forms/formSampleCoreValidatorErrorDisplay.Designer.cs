namespace LcmsNet.SampleQueue
{
    partial class formSampleValidatorErrorDisplay
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
            this.mpanel_errors = new System.Windows.Forms.Panel();
            this.mpanel_buttons = new System.Windows.Forms.Panel();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mlistview_errors = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.mpanel_errors.SuspendLayout();
            this.mpanel_buttons.SuspendLayout();
            this.SuspendLayout();
            //
            // mpanel_errors
            //
            this.mpanel_errors.AutoScroll = true;
            this.mpanel_errors.BackColor = System.Drawing.Color.White;
            this.mpanel_errors.Controls.Add(this.mlistview_errors);
            this.mpanel_errors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_errors.Location = new System.Drawing.Point(0, 0);
            this.mpanel_errors.Name = "mpanel_errors";
            this.mpanel_errors.Size = new System.Drawing.Size(510, 423);
            this.mpanel_errors.TabIndex = 1;
            //
            // mpanel_buttons
            //
            this.mpanel_buttons.Controls.Add(this.mbutton_ok);
            this.mpanel_buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_buttons.Location = new System.Drawing.Point(0, 423);
            this.mpanel_buttons.Name = "mpanel_buttons";
            this.mpanel_buttons.Size = new System.Drawing.Size(510, 37);
            this.mpanel_buttons.TabIndex = 2;
            //
            // mbutton_ok
            //
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_ok.Location = new System.Drawing.Point(217, 3);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(71, 27);
            this.mbutton_ok.TabIndex = 0;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            //
            // mlistview_errors
            //
            this.mlistview_errors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistview_errors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.mlistview_errors.FullRowSelect = true;
            this.mlistview_errors.GridLines = true;
            this.mlistview_errors.Location = new System.Drawing.Point(0, 3);
            this.mlistview_errors.Name = "mlistview_errors";
            this.mlistview_errors.Size = new System.Drawing.Size(507, 420);
            this.mlistview_errors.TabIndex = 0;
            this.mlistview_errors.UseCompatibleStateImageBehavior = false;
            this.mlistview_errors.View = System.Windows.Forms.View.Details;
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "Sample";
            this.columnHeader1.Width = 130;
            //
            // columnHeader2
            //
            this.columnHeader2.Text = "Error ";
            this.columnHeader2.Width = 372;
            //
            // formSampleValidatorErrorDisplay
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(510, 460);
            this.Controls.Add(this.mpanel_errors);
            this.Controls.Add(this.mpanel_buttons);
            this.Name = "formSampleValidatorErrorDisplay";
            this.Text = "Sample Errors";
            this.mpanel_errors.ResumeLayout(false);
            this.mpanel_buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mpanel_errors;
        private System.Windows.Forms.Panel mpanel_buttons;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.ListView mlistview_errors;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}