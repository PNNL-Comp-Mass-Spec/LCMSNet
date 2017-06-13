using System.ComponentModel;
using System.Windows.Forms;
using LcmsNet.Simulator.Views;

namespace LcmsNet.Simulator
{
    partial class formSimulatorCombined2
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
            this.simCombinedView = new SimulatorCombinedView();
            this.simCombinedViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            //
            // simCombinedViewHost
            //
            this.simCombinedViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simCombinedViewHost.Location = new System.Drawing.Point(0, 0);
            this.simCombinedViewHost.Name = "SimCombinedViewHost";
            this.simCombinedViewHost.Size = new System.Drawing.Size(286, 515);
            this.simCombinedViewHost.TabIndex = 0;
            this.simCombinedViewHost.Text = "SimCombinedViewHost";
            this.simCombinedViewHost.Child = simCombinedView;
            //
            // formSimulatorCombined2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 517);
            this.Controls.Add(this.simCombinedViewHost);
            this.Name = "formSimulatorCombined2";
            this.Text = "formSimulator";
            this.ResumeLayout(false);

        }

        #endregion

        private SimulatorCombinedView simCombinedView;
        private System.Windows.Forms.Integration.ElementHost simCombinedViewHost;
    }
}