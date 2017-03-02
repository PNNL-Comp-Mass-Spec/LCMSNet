namespace LcmsNet.Method.Forms
{
    partial class formConvertToSeconds
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
            this.mbutton_conversionOk = new System.Windows.Forms.Button();
            this.mnum_minutes = new System.Windows.Forms.NumericUpDown();
            this.mnum_seconds = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_decimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_seconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_decimalPlaces)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            //
            // mbutton_conversionOk
            //
            this.mbutton_conversionOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_conversionOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_conversionOk.Location = new System.Drawing.Point(194, 219);
            this.mbutton_conversionOk.Name = "mbutton_conversionOk";
            this.mbutton_conversionOk.Size = new System.Drawing.Size(57, 42);
            this.mbutton_conversionOk.TabIndex = 0;
            this.mbutton_conversionOk.Text = "Ok";
            this.mbutton_conversionOk.UseVisualStyleBackColor = true;
            //
            // mnum_minutes
            //
            this.mnum_minutes.Location = new System.Drawing.Point(19, 31);
            this.mnum_minutes.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_minutes.Name = "mnum_minutes";
            this.mnum_minutes.Size = new System.Drawing.Size(98, 20);
            this.mnum_minutes.TabIndex = 1;
            this.mnum_minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // mnum_seconds
            //
            this.mnum_seconds.Location = new System.Drawing.Point(123, 31);
            this.mnum_seconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.mnum_seconds.Name = "mnum_seconds";
            this.mnum_seconds.Size = new System.Drawing.Size(46, 20);
            this.mnum_seconds.TabIndex = 2;
            this.mnum_seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_seconds.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            //
            // button1
            //
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(119, 219);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 42);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            //
            // label1
            //
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Minutes";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // label2
            //
            this.label2.Location = new System.Drawing.Point(123, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "Seconds";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // mnum_decimalPlaces
            //
            this.mnum_decimalPlaces.Location = new System.Drawing.Point(12, 38);
            this.mnum_decimalPlaces.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_decimalPlaces.Name = "mnum_decimalPlaces";
            this.mnum_decimalPlaces.Size = new System.Drawing.Size(98, 20);
            this.mnum_decimalPlaces.TabIndex = 6;
            this.mnum_decimalPlaces.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Decimal Places";
            //
            // radioButton1
            //
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(12, 8);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(143, 17);
            this.radioButton1.TabIndex = 8;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Perform Time Conversion";
            this.radioButton1.UseVisualStyleBackColor = true;
            //
            // radioButton2
            //
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(12, 119);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(139, 17);
            this.radioButton2.TabIndex = 9;
            this.radioButton2.Text = "Modify Display Precision";
            this.radioButton2.UseVisualStyleBackColor = true;
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.mnum_decimalPlaces);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(36, 142);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 70);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Precision";
            //
            // groupBox2
            //
            this.groupBox2.Controls.Add(this.mnum_seconds);
            this.groupBox2.Controls.Add(this.mnum_minutes);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(36, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 82);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Time Conversion";
            //
            // formConvertToSeconds
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 273);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mbutton_conversionOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formConvertToSeconds";
            this.Text = "Conversion Tool";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_seconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_decimalPlaces)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_conversionOk;
        private System.Windows.Forms.NumericUpDown mnum_minutes;
        private System.Windows.Forms.NumericUpDown mnum_seconds;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_decimalPlaces;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}