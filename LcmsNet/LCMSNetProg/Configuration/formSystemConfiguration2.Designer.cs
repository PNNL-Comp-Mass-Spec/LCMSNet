using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Configuration
{
    partial class formSystemConfiguration2
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
            this.systemConfigurationViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.systemConfigurationView = new LcmsNet.Configuration.Views.SystemConfigurationView();
            this.SuspendLayout();
            // 
            // systemConfigurationViewHost
            // 
            this.systemConfigurationViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.systemConfigurationViewHost.Location = new System.Drawing.Point(0, 0);
            this.systemConfigurationViewHost.Name = "systemConfigurationViewHost";
            this.systemConfigurationViewHost.Size = new System.Drawing.Size(871, 604);
            this.systemConfigurationViewHost.TabIndex = 0;
            this.systemConfigurationViewHost.Text = "systemConfigurationViewHost";
            this.systemConfigurationViewHost.Child = this.systemConfigurationView;
            // 
            // formSystemConfiguration2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(871, 604);
            this.Controls.Add(this.systemConfigurationViewHost);
            this.MinimumSize = new System.Drawing.Size(521, 257);
            this.Name = "formSystemConfiguration2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cart Configuration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost systemConfigurationViewHost;
        private Views.SystemConfigurationView systemConfigurationView;
    }
}