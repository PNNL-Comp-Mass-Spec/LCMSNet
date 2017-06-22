using System.ComponentModel;
using LcmsNet.Devices.Fluidics.Views;

namespace LcmsNet.Devices.Fluidics
{
    partial class FluidicsDesign2
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
            if (disposing)
            {
                fluidicsVm.Dispose();
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
            this.fluidicsDesignViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.fluidicsDesignView = new FluidicsDesignView();
            this.SuspendLayout();
            //
            // fluidicsDesignViewHost
            //
            this.fluidicsDesignViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fluidicsDesignViewHost.Location = new System.Drawing.Point(0, 0);
            this.fluidicsDesignViewHost.Name = "fluidicsDesignViewHost";
            this.fluidicsDesignViewHost.Size = new System.Drawing.Size(1006, 591);
            this.fluidicsDesignViewHost.TabIndex = 0;
            this.fluidicsDesignViewHost.Text = "fluidicsDesignViewHost";
            this.fluidicsDesignViewHost.Child = fluidicsDesignView;
            //
            // FluidicsDesign2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1006, 591);
            this.Controls.Add(this.fluidicsDesignViewHost);
            this.Name = "FluidicsDesign2";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Integration.ElementHost fluidicsDesignViewHost;
        private FluidicsDesignView fluidicsDesignView;
    }
}