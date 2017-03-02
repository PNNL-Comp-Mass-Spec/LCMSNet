namespace DemoPluginLibrary
{
    partial class DemoClosureAdvanceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.numPulseLength = new System.Windows.Forms.NumericUpDown();
            this.numVoltage = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pulse Length(seconds):";
            // 
            // numPulseLength
            // 
            this.numPulseLength.Location = new System.Drawing.Point(127, 19);
            this.numPulseLength.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numPulseLength.Name = "numPulseLength";
            this.numPulseLength.Size = new System.Drawing.Size(58, 20);
            this.numPulseLength.TabIndex = 1;
            // 
            // numVoltage
            // 
            this.numVoltage.DecimalPlaces = 1;
            this.numVoltage.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVoltage.Location = new System.Drawing.Point(243, 21);
            this.numVoltage.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numVoltage.Name = "numVoltage";
            this.numVoltage.Size = new System.Drawing.Size(58, 20);
            this.numVoltage.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Voltage:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(307, 21);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // TestClosureAdvanceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.numVoltage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numPulseLength);
            this.Controls.Add(this.label1);
            this.Name = "TestClosureAdvanceControl";
            this.Size = new System.Drawing.Size(417, 73);
            ((System.ComponentModel.ISupportInitialize)(this.numPulseLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numPulseLength;
        private System.Windows.Forms.NumericUpDown numVoltage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSend;
    }
}
