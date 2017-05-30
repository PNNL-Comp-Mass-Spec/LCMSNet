using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Devices.Pumps
{
    partial class formPumpDisplays2
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
            this.pumpDisplaysViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.pumpDisplaysView = new LcmsNet.Devices.Pumps.Views.PumpDisplaysView();
            this.SuspendLayout();
            // 
            // pumpDisplaysViewHost
            // 
            this.pumpDisplaysViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pumpDisplaysViewHost.Location = new System.Drawing.Point(0, 0);
            this.pumpDisplaysViewHost.Name = "pumpDisplaysViewHost";
            this.pumpDisplaysViewHost.Size = new System.Drawing.Size(598, 546);
            this.pumpDisplaysViewHost.TabIndex = 1;
            this.pumpDisplaysViewHost.Text = "pumpDisplaysViewHost";
            this.pumpDisplaysViewHost.Child = this.pumpDisplaysView;
            // 
            // formPumpDisplays2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(598, 546);
            this.Controls.Add(this.pumpDisplaysViewHost);
            this.Name = "formPumpDisplays2";
            this.Text = "Pump Display";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost pumpDisplaysViewHost;
        private Views.PumpDisplaysView pumpDisplaysView;
    }
}