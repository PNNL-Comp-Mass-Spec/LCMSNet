namespace Agilent.Devices.Pumps
{
    sealed partial class formAgilentPumpPurge
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
            this.mbutton_purgeA1 = new System.Windows.Forms.Button();
            this.mbutton_purgeA2 = new System.Windows.Forms.Button();
            this.mbutton_purgeB1 = new System.Windows.Forms.Button();
            this.mbutton_purgeB2 = new System.Windows.Forms.Button();
            this.mnum_timeA1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.mnum_timeA2 = new System.Windows.Forms.NumericUpDown();
            this.mnum_timeB1 = new System.Windows.Forms.NumericUpDown();
            this.mnum_timeB2 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_flowA1 = new System.Windows.Forms.NumericUpDown();
            this.mnum_flowA2 = new System.Windows.Forms.NumericUpDown();
            this.mnum_flowB1 = new System.Windows.Forms.NumericUpDown();
            this.mnum_flowB2 = new System.Windows.Forms.NumericUpDown();
            this.mbutton_abortPurges = new System.Windows.Forms.Button();
            this.mbutton_ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeA1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeA2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeB1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeB2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowA1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowA2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowB1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowB2)).BeginInit();
            this.SuspendLayout();
            // 
            // mbutton_purgeA1
            // 
            this.mbutton_purgeA1.Location = new System.Drawing.Point(12, 12);
            this.mbutton_purgeA1.Name = "mbutton_purgeA1";
            this.mbutton_purgeA1.Size = new System.Drawing.Size(134, 45);
            this.mbutton_purgeA1.TabIndex = 14;
            this.mbutton_purgeA1.Text = "PURGE Channel A1";
            this.mbutton_purgeA1.UseVisualStyleBackColor = true;
            this.mbutton_purgeA1.Click += new System.EventHandler(this.mbutton_purgeA1_Click);
            // 
            // mbutton_purgeA2
            // 
            this.mbutton_purgeA2.Location = new System.Drawing.Point(12, 63);
            this.mbutton_purgeA2.Name = "mbutton_purgeA2";
            this.mbutton_purgeA2.Size = new System.Drawing.Size(134, 45);
            this.mbutton_purgeA2.TabIndex = 15;
            this.mbutton_purgeA2.Text = "PURGE Channel A2";
            this.mbutton_purgeA2.UseVisualStyleBackColor = true;
            this.mbutton_purgeA2.Click += new System.EventHandler(this.mbutton_purgeA2_Click);
            // 
            // mbutton_purgeB1
            // 
            this.mbutton_purgeB1.Location = new System.Drawing.Point(12, 114);
            this.mbutton_purgeB1.Name = "mbutton_purgeB1";
            this.mbutton_purgeB1.Size = new System.Drawing.Size(134, 45);
            this.mbutton_purgeB1.TabIndex = 16;
            this.mbutton_purgeB1.Text = "PURGE Channel B1";
            this.mbutton_purgeB1.UseVisualStyleBackColor = true;
            this.mbutton_purgeB1.Click += new System.EventHandler(this.mbutton_purgeB1_Click);
            // 
            // mbutton_purgeB2
            // 
            this.mbutton_purgeB2.Location = new System.Drawing.Point(12, 165);
            this.mbutton_purgeB2.Name = "mbutton_purgeB2";
            this.mbutton_purgeB2.Size = new System.Drawing.Size(134, 45);
            this.mbutton_purgeB2.TabIndex = 17;
            this.mbutton_purgeB2.Text = "PURGE Channel B2";
            this.mbutton_purgeB2.UseVisualStyleBackColor = true;
            this.mbutton_purgeB2.Click += new System.EventHandler(this.mbutton_purgeB2_Click);
            // 
            // mnum_timeA1
            // 
            this.mnum_timeA1.DecimalPlaces = 2;
            this.mnum_timeA1.Location = new System.Drawing.Point(166, 29);
            this.mnum_timeA1.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.mnum_timeA1.Name = "mnum_timeA1";
            this.mnum_timeA1.Size = new System.Drawing.Size(123, 20);
            this.mnum_timeA1.TabIndex = 18;
            this.mnum_timeA1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_timeA1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(329, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Flow Rate (ul/min)";
            // 
            // mnum_timeA2
            // 
            this.mnum_timeA2.DecimalPlaces = 2;
            this.mnum_timeA2.Location = new System.Drawing.Point(166, 80);
            this.mnum_timeA2.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.mnum_timeA2.Name = "mnum_timeA2";
            this.mnum_timeA2.Size = new System.Drawing.Size(123, 20);
            this.mnum_timeA2.TabIndex = 20;
            this.mnum_timeA2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_timeA2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // mnum_timeB1
            // 
            this.mnum_timeB1.DecimalPlaces = 2;
            this.mnum_timeB1.Location = new System.Drawing.Point(166, 131);
            this.mnum_timeB1.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.mnum_timeB1.Name = "mnum_timeB1";
            this.mnum_timeB1.Size = new System.Drawing.Size(123, 20);
            this.mnum_timeB1.TabIndex = 21;
            this.mnum_timeB1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_timeB1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // mnum_timeB2
            // 
            this.mnum_timeB2.DecimalPlaces = 2;
            this.mnum_timeB2.Location = new System.Drawing.Point(166, 182);
            this.mnum_timeB2.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.mnum_timeB2.Name = "mnum_timeB2";
            this.mnum_timeB2.Size = new System.Drawing.Size(123, 20);
            this.mnum_timeB2.TabIndex = 22;
            this.mnum_timeB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_timeB2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Duration (mins)";
            // 
            // mnum_flowA1
            // 
            this.mnum_flowA1.DecimalPlaces = 2;
            this.mnum_flowA1.Location = new System.Drawing.Point(314, 29);
            this.mnum_flowA1.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.mnum_flowA1.Name = "mnum_flowA1";
            this.mnum_flowA1.Size = new System.Drawing.Size(123, 20);
            this.mnum_flowA1.TabIndex = 24;
            this.mnum_flowA1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_flowA1.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // mnum_flowA2
            // 
            this.mnum_flowA2.DecimalPlaces = 2;
            this.mnum_flowA2.Location = new System.Drawing.Point(314, 80);
            this.mnum_flowA2.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.mnum_flowA2.Name = "mnum_flowA2";
            this.mnum_flowA2.Size = new System.Drawing.Size(123, 20);
            this.mnum_flowA2.TabIndex = 25;
            this.mnum_flowA2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_flowA2.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // mnum_flowB1
            // 
            this.mnum_flowB1.DecimalPlaces = 2;
            this.mnum_flowB1.Location = new System.Drawing.Point(314, 131);
            this.mnum_flowB1.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.mnum_flowB1.Name = "mnum_flowB1";
            this.mnum_flowB1.Size = new System.Drawing.Size(123, 20);
            this.mnum_flowB1.TabIndex = 26;
            this.mnum_flowB1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_flowB1.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // mnum_flowB2
            // 
            this.mnum_flowB2.DecimalPlaces = 2;
            this.mnum_flowB2.Location = new System.Drawing.Point(314, 182);
            this.mnum_flowB2.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.mnum_flowB2.Name = "mnum_flowB2";
            this.mnum_flowB2.Size = new System.Drawing.Size(123, 20);
            this.mnum_flowB2.TabIndex = 27;
            this.mnum_flowB2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_flowB2.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // mbutton_abortPurges
            // 
            this.mbutton_abortPurges.Location = new System.Drawing.Point(12, 241);
            this.mbutton_abortPurges.Name = "mbutton_abortPurges";
            this.mbutton_abortPurges.Size = new System.Drawing.Size(134, 45);
            this.mbutton_abortPurges.TabIndex = 28;
            this.mbutton_abortPurges.Text = "Abort Purges";
            this.mbutton_abortPurges.UseVisualStyleBackColor = true;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_ok.Location = new System.Drawing.Point(314, 241);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(123, 45);
            this.mbutton_ok.TabIndex = 29;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            // 
            // formAgilentPumpPurge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(452, 298);
            this.ControlBox = false;
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mbutton_abortPurges);
            this.Controls.Add(this.mnum_flowB2);
            this.Controls.Add(this.mnum_flowB1);
            this.Controls.Add(this.mnum_flowA2);
            this.Controls.Add(this.mnum_flowA1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mnum_timeB2);
            this.Controls.Add(this.mnum_timeB1);
            this.Controls.Add(this.mnum_timeA2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mnum_timeA1);
            this.Controls.Add(this.mbutton_purgeB2);
            this.Controls.Add(this.mbutton_purgeB1);
            this.Controls.Add(this.mbutton_purgeA2);
            this.Controls.Add(this.mbutton_purgeA1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formAgilentPumpPurge";
            this.Text = "Purge Pumps";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeA1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeA2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeB1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_timeB2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowA1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowA2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowB1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowB2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_purgeA1;
        private System.Windows.Forms.Button mbutton_purgeA2;
        private System.Windows.Forms.Button mbutton_purgeB1;
        private System.Windows.Forms.Button mbutton_purgeB2;
        private System.Windows.Forms.NumericUpDown mnum_timeA1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown mnum_timeA2;
        private System.Windows.Forms.NumericUpDown mnum_timeB1;
        private System.Windows.Forms.NumericUpDown mnum_timeB2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_flowA1;
        private System.Windows.Forms.NumericUpDown mnum_flowA2;
        private System.Windows.Forms.NumericUpDown mnum_flowB1;
        private System.Windows.Forms.NumericUpDown mnum_flowB2;
        private System.Windows.Forms.Button mbutton_abortPurges;
        private System.Windows.Forms.Button mbutton_ok;
    }
}