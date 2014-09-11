namespace LcmsNet.Simulator
{
    partial class formSimulatorCombined
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
            this.splitFluidicsAndControls = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitFluidicsAndControls)).BeginInit();
            this.splitFluidicsAndControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitFluidicsAndControls
            // 
            this.splitFluidicsAndControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitFluidicsAndControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitFluidicsAndControls.Location = new System.Drawing.Point(0, 0);
            this.splitFluidicsAndControls.Name = "splitFluidicsAndControls";
            // 
            // splitFluidicsAndControls.Panel1
            // 
            this.splitFluidicsAndControls.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitFluidicsAndControls.Panel1MinSize = 0;
            this.splitFluidicsAndControls.Panel2MinSize = 0;
            this.splitFluidicsAndControls.Size = new System.Drawing.Size(957, 517);
            this.splitFluidicsAndControls.SplitterDistance = 288;
            this.splitFluidicsAndControls.SplitterWidth = 5;
            this.splitFluidicsAndControls.TabIndex = 0;
            // 
            // formSimulatorCombined
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 517);
            this.Controls.Add(this.splitFluidicsAndControls);
            this.Name = "formSimulatorCombined";
            this.Text = "formSimulator";
            ((System.ComponentModel.ISupportInitialize)(this.splitFluidicsAndControls)).EndInit();
            this.splitFluidicsAndControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitFluidicsAndControls;
    }
}