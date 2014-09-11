namespace LcmsNet.Method.Forms
{
    partial class formThroughputPreview
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
            this.mcontrol_throughputTimeline = new LcmsNet.Method.Forms.controlLCMethodTimeline();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mcontrol_throughputTimeline
            // 
            this.mcontrol_throughputTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_throughputTimeline.AutoScroll = true;
            this.mcontrol_throughputTimeline.BackColor = System.Drawing.Color.White;
            this.mcontrol_throughputTimeline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mcontrol_throughputTimeline.Location = new System.Drawing.Point(12, 12);
            this.mcontrol_throughputTimeline.Name = "mcontrol_throughputTimeline";
            this.mcontrol_throughputTimeline.Size = new System.Drawing.Size(567, 248);
            this.mcontrol_throughputTimeline.TabIndex = 0;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.Location = new System.Drawing.Point(508, 269);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(71, 28);
            this.mbutton_ok.TabIndex = 1;
            this.mbutton_ok.Text = "Ok";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // formThroughputPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(591, 300);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mcontrol_throughputTimeline);
            this.Name = "formThroughputPreview";
            this.Text = "Projected Separation Throughput";
            this.ResumeLayout(false);

        }

        #endregion

        private controlLCMethodTimeline mcontrol_throughputTimeline;
        private System.Windows.Forms.Button mbutton_ok;
    }
}