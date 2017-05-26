using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Logging
{
    partial class formMessageWindow2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.messagesViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.messagesView = new LcmsNet.Logging.Views.MessagesView();
            this.SuspendLayout();
            // 
            // messagesViewHost
            // 
            this.messagesViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesViewHost.Location = new System.Drawing.Point(5, 5);
            this.messagesViewHost.Margin = new System.Windows.Forms.Padding(0);
            this.messagesViewHost.Name = "messagesViewHost";
            this.messagesViewHost.Size = new System.Drawing.Size(1579, 783);
            this.messagesViewHost.TabIndex = 0;
            this.messagesViewHost.Text = "messagesViewHost";
            this.messagesViewHost.Child = this.messagesView;
            // 
            // formMessageWindow2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1589, 793);
            this.Controls.Add(this.messagesViewHost);
            this.Name = "formMessageWindow2";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Messages";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost messagesViewHost;
        private Views.MessagesView messagesView;
    }
}