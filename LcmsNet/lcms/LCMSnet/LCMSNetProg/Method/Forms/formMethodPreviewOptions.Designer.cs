namespace LcmsNet.Method.Forms
{
    partial class formMethodPreviewOptions
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
            this.mcheckBox_animate = new System.Windows.Forms.CheckBox();
            this.mnum_delay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mnum_frameCount = new System.Windows.Forms.NumericUpDown();
            this.mgroupBox_update = new System.Windows.Forms.GroupBox();
            this.mlabel_frames = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_frameCount)).BeginInit();
            this.mgroupBox_update.SuspendLayout();
            this.SuspendLayout();
            // 
            // mcheckBox_animate
            // 
            this.mcheckBox_animate.AutoSize = true;
            this.mcheckBox_animate.Location = new System.Drawing.Point(12, 23);
            this.mcheckBox_animate.Name = "mcheckBox_animate";
            this.mcheckBox_animate.Size = new System.Drawing.Size(64, 17);
            this.mcheckBox_animate.TabIndex = 0;
            this.mcheckBox_animate.Text = "Animate";
            this.mcheckBox_animate.UseVisualStyleBackColor = true;
            this.mcheckBox_animate.CheckedChanged += new System.EventHandler(this.mcheckBox_animate_CheckedChanged);
            // 
            // mnum_delay
            // 
            this.mnum_delay.Location = new System.Drawing.Point(133, 44);
            this.mnum_delay.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.mnum_delay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mnum_delay.Name = "mnum_delay";
            this.mnum_delay.Size = new System.Drawing.Size(90, 20);
            this.mnum_delay.TabIndex = 1;
            this.mnum_delay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Delay animation for";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "milliseconds";
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(159, 152);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(64, 31);
            this.mbutton_ok.TabIndex = 4;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.Location = new System.Drawing.Point(232, 152);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(60, 31);
            this.mbutton_cancel.TabIndex = 5;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            this.mbutton_cancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Update every";
            // 
            // mnum_frameCount
            // 
            this.mnum_frameCount.Location = new System.Drawing.Point(121, 23);
            this.mnum_frameCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mnum_frameCount.Name = "mnum_frameCount";
            this.mnum_frameCount.Size = new System.Drawing.Size(90, 20);
            this.mnum_frameCount.TabIndex = 6;
            this.mnum_frameCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mgroupBox_update
            // 
            this.mgroupBox_update.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_update.Controls.Add(this.mlabel_frames);
            this.mgroupBox_update.Controls.Add(this.mnum_frameCount);
            this.mgroupBox_update.Controls.Add(this.label3);
            this.mgroupBox_update.Location = new System.Drawing.Point(12, 81);
            this.mgroupBox_update.Name = "mgroupBox_update";
            this.mgroupBox_update.Size = new System.Drawing.Size(280, 65);
            this.mgroupBox_update.TabIndex = 8;
            this.mgroupBox_update.TabStop = false;
            this.mgroupBox_update.Text = "Update Data";
            // 
            // mlabel_frames
            // 
            this.mlabel_frames.AutoSize = true;
            this.mlabel_frames.Location = new System.Drawing.Point(217, 25);
            this.mlabel_frames.Name = "mlabel_frames";
            this.mlabel_frames.Size = new System.Drawing.Size(38, 13);
            this.mlabel_frames.TabIndex = 8;
            this.mlabel_frames.Text = "frames";
            // 
            // formMethodPreviewOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 186);
            this.Controls.Add(this.mgroupBox_update);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mnum_delay);
            this.Controls.Add(this.mcheckBox_animate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formMethodPreviewOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Method Preview Options";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_frameCount)).EndInit();
            this.mgroupBox_update.ResumeLayout(false);
            this.mgroupBox_update.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox mcheckBox_animate;
        private System.Windows.Forms.NumericUpDown mnum_delay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.Button mbutton_cancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown mnum_frameCount;
        private System.Windows.Forms.GroupBox mgroupBox_update;
        private System.Windows.Forms.Label mlabel_frames;
    }
}