using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Notification.Forms
{
    partial class formNotificationSystem2
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
            this.components = new System.ComponentModel.Container();
            this.notificationSystemViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.notificationSystemView = new LcmsNet.Notification.Views.NotificationSystemView();
            this.SuspendLayout();
            // 
            // notificationSystemViewHost
            // 
            this.notificationSystemViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notificationSystemViewHost.Location = new System.Drawing.Point(0, 0);
            this.notificationSystemViewHost.Name = "notificationSystemViewHost";
            this.notificationSystemViewHost.Size = new System.Drawing.Size(922, 750);
            this.notificationSystemViewHost.TabIndex = 0;
            this.notificationSystemViewHost.Text = "notificationSystemViewHost";
            this.notificationSystemViewHost.Child = this.notificationSystemView;
            // 
            // formNotificationSystem2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(922, 750);
            this.Controls.Add(this.notificationSystemViewHost);
            this.Name = "formNotificationSystem2";
            this.Text = "Notification System";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost notificationSystemViewHost;
        private Views.NotificationSystemView notificationSystemView;
    }
}