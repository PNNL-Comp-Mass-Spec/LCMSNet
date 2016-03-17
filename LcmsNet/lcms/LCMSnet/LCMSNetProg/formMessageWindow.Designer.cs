namespace LcmsNet
{
    partial class formMessageWindow
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
            this.mlabel_errors = new System.Windows.Forms.Label();
            this.mlabel_messages = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.mpanel_messageIndicator = new System.Windows.Forms.Panel();
            this.mpanel_errorIndicator = new System.Windows.Forms.Panel();
            this.customTabControl1 = new LcmsNet.CustomTabControl();
            this.mtab_messages = new System.Windows.Forms.TabPage();
            this.mpanel_messages = new System.Windows.Forms.Panel();
            this.mlistBox_messages = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.mtab_errors = new System.Windows.Forms.TabPage();
            this.mpanel_errors = new System.Windows.Forms.Panel();
            this.m_errorMessages = new System.Windows.Forms.RichTextBox();
            this.mbutton_acknowledgeErrors = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.customTabControl1.SuspendLayout();
            this.mtab_messages.SuspendLayout();
            this.mpanel_messages.SuspendLayout();
            this.mtab_errors.SuspendLayout();
            this.mpanel_errors.SuspendLayout();
            this.SuspendLayout();
            //
            // mlabel_errors
            //
            this.mlabel_errors.BackColor = System.Drawing.Color.White;
            this.mlabel_errors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlabel_errors.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_errors.Image = global::LcmsNet.Properties.Resources.Errors;
            this.mlabel_errors.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.mlabel_errors.Location = new System.Drawing.Point(4, 45);
            this.mlabel_errors.Name = "mlabel_errors";
            this.mlabel_errors.Size = new System.Drawing.Size(161, 42);
            this.mlabel_errors.TabIndex = 4;
            this.mlabel_errors.Text = "Errors";
            this.mlabel_errors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mlabel_errors.MouseLeave += new System.EventHandler(this.mlabel_errors_MouseLeave);
            this.mlabel_errors.Click += new System.EventHandler(this.mlabel_errors_Click);
            this.mlabel_errors.MouseEnter += new System.EventHandler(this.mlabel_errors_MouseEnter);
            //
            // mlabel_messages
            //
            this.mlabel_messages.BackColor = System.Drawing.Color.White;
            this.mlabel_messages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlabel_messages.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_messages.Image = global::LcmsNet.Properties.Resources.MethodEditor;
            this.mlabel_messages.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.mlabel_messages.Location = new System.Drawing.Point(4, 0);
            this.mlabel_messages.Name = "mlabel_messages";
            this.mlabel_messages.Size = new System.Drawing.Size(161, 42);
            this.mlabel_messages.TabIndex = 11;
            this.mlabel_messages.Text = "Messages";
            this.mlabel_messages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mlabel_messages.MouseLeave += new System.EventHandler(this.mlabel_messages_MouseLeave);
            this.mlabel_messages.Click += new System.EventHandler(this.mlabel_messages_Click);
            this.mlabel_messages.MouseEnter += new System.EventHandler(this.mlabel_messages_MouseEnter);
            //
            // panel3
            //
            this.panel3.Controls.Add(this.mpanel_messageIndicator);
            this.panel3.Controls.Add(this.mpanel_errorIndicator);
            this.panel3.Controls.Add(this.mlabel_errors);
            this.panel3.Controls.Add(this.mlabel_messages);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(5, 5);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.panel3.Size = new System.Drawing.Size(168, 783);
            this.panel3.TabIndex = 11;
            //
            // mpanel_messageIndicator
            //
            this.mpanel_messageIndicator.Location = new System.Drawing.Point(0, 0);
            this.mpanel_messageIndicator.Name = "mpanel_messageIndicator";
            this.mpanel_messageIndicator.Size = new System.Drawing.Size(4, 42);
            this.mpanel_messageIndicator.TabIndex = 13;
            //
            // mpanel_errorIndicator
            //
            this.mpanel_errorIndicator.Location = new System.Drawing.Point(0, 45);
            this.mpanel_errorIndicator.Name = "mpanel_errorIndicator";
            this.mpanel_errorIndicator.Size = new System.Drawing.Size(4, 42);
            this.mpanel_errorIndicator.TabIndex = 12;
            //
            // customTabControl1
            //
            this.customTabControl1.Controls.Add(this.mtab_messages);
            this.customTabControl1.Controls.Add(this.mtab_errors);
            this.customTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customTabControl1.Location = new System.Drawing.Point(173, 5);
            this.customTabControl1.Name = "customTabControl1";
            this.customTabControl1.SelectedIndex = 0;
            this.customTabControl1.Size = new System.Drawing.Size(1411, 783);
            this.customTabControl1.TabIndex = 1;
            //
            // mtab_messages
            //
            this.mtab_messages.Controls.Add(this.mpanel_messages);
            this.mtab_messages.Location = new System.Drawing.Point(4, 22);
            this.mtab_messages.Name = "mtab_messages";
            this.mtab_messages.Padding = new System.Windows.Forms.Padding(3);
            this.mtab_messages.Size = new System.Drawing.Size(1403, 757);
            this.mtab_messages.TabIndex = 0;
            this.mtab_messages.Text = "tabPage3";
            this.mtab_messages.UseVisualStyleBackColor = true;
            //
            // mpanel_messages
            //
            this.mpanel_messages.BackColor = System.Drawing.Color.White;
            this.mpanel_messages.Controls.Add(this.mlistBox_messages);
            this.mpanel_messages.Controls.Add(this.button1);
            this.mpanel_messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_messages.Location = new System.Drawing.Point(3, 3);
            this.mpanel_messages.Name = "mpanel_messages";
            this.mpanel_messages.Padding = new System.Windows.Forms.Padding(5);
            this.mpanel_messages.Size = new System.Drawing.Size(1397, 751);
            this.mpanel_messages.TabIndex = 10;
            //
            // mlistBox_messages
            //
            this.mlistBox_messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlistBox_messages.FormattingEnabled = true;
            this.mlistBox_messages.HorizontalScrollbar = true;
            this.mlistBox_messages.Location = new System.Drawing.Point(5, 5);
            this.mlistBox_messages.Name = "mlistBox_messages";
            this.mlistBox_messages.ScrollAlwaysVisible = true;
            this.mlistBox_messages.Size = new System.Drawing.Size(1387, 706);
            this.mlistBox_messages.TabIndex = 10;
            //
            // button1
            //
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(5, 714);
            this.button1.Name = "button1";
            this.button1.Padding = new System.Windows.Forms.Padding(5);
            this.button1.Size = new System.Drawing.Size(1387, 32);
            this.button1.TabIndex = 12;
            this.button1.Text = "Clear Messages";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // mtab_errors
            //
            this.mtab_errors.Controls.Add(this.mpanel_errors);
            this.mtab_errors.Location = new System.Drawing.Point(4, 22);
            this.mtab_errors.Name = "mtab_errors";
            this.mtab_errors.Padding = new System.Windows.Forms.Padding(3);
            this.mtab_errors.Size = new System.Drawing.Size(1403, 757);
            this.mtab_errors.TabIndex = 1;
            this.mtab_errors.Text = "tabPage4";
            this.mtab_errors.UseVisualStyleBackColor = true;
            //
            // mpanel_errors
            //
            this.mpanel_errors.BackColor = System.Drawing.Color.White;
            this.mpanel_errors.Controls.Add(this.m_errorMessages);
            this.mpanel_errors.Controls.Add(this.mbutton_acknowledgeErrors);
            this.mpanel_errors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_errors.Location = new System.Drawing.Point(3, 3);
            this.mpanel_errors.Name = "mpanel_errors";
            this.mpanel_errors.Padding = new System.Windows.Forms.Padding(5);
            this.mpanel_errors.Size = new System.Drawing.Size(1397, 751);
            this.mpanel_errors.TabIndex = 8;
            //
            // m_errorMessages
            //
            this.m_errorMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_errorMessages.Location = new System.Drawing.Point(5, 5);
            this.m_errorMessages.Name = "m_errorMessages";
            this.m_errorMessages.ReadOnly = true;
            this.m_errorMessages.Size = new System.Drawing.Size(1387, 709);
            this.m_errorMessages.TabIndex = 6;
            this.m_errorMessages.Text = "";
            this.m_errorMessages.WordWrap = false;
            //
            // mbutton_acknowledgeErrors
            //
            this.mbutton_acknowledgeErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mbutton_acknowledgeErrors.Location = new System.Drawing.Point(5, 714);
            this.mbutton_acknowledgeErrors.Name = "mbutton_acknowledgeErrors";
            this.mbutton_acknowledgeErrors.Padding = new System.Windows.Forms.Padding(5);
            this.mbutton_acknowledgeErrors.Size = new System.Drawing.Size(1387, 32);
            this.mbutton_acknowledgeErrors.TabIndex = 5;
            this.mbutton_acknowledgeErrors.Text = "Clear Error Notification";
            this.mbutton_acknowledgeErrors.UseVisualStyleBackColor = true;
            this.mbutton_acknowledgeErrors.Click += new System.EventHandler(this.mbutton_acknowledgeErrors_Click);
            //
            // formMessageWindow
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1589, 793);
            this.Controls.Add(this.customTabControl1);
            this.Controls.Add(this.panel3);
            this.Name = "formMessageWindow";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Messages";
            this.panel3.ResumeLayout(false);
            this.customTabControl1.ResumeLayout(false);
            this.mtab_messages.ResumeLayout(false);
            this.mpanel_messages.ResumeLayout(false);
            this.mtab_errors.ResumeLayout(false);
            this.mpanel_errors.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mpanel_errors;
        private System.Windows.Forms.Label mlabel_errors;
        private System.Windows.Forms.Button mbutton_acknowledgeErrors;
        private System.Windows.Forms.Panel mpanel_messages;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label mlabel_messages;
        private System.Windows.Forms.Panel panel3;
        private CustomTabControl customTabControl1;
        private System.Windows.Forms.TabPage mtab_messages;
        private System.Windows.Forms.TabPage mtab_errors;
        private System.Windows.Forms.Panel mpanel_messageIndicator;
        private System.Windows.Forms.Panel mpanel_errorIndicator;
        private System.Windows.Forms.ListBox mlistBox_messages;
        private System.Windows.Forms.RichTextBox m_errorMessages;

    }
}