namespace DemoPluginLibrary
{
    partial class DemoPALAdvancedControl
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
            this.btnRunMethod = new System.Windows.Forms.Button();
            this.comboMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRunMethod
            // 
            this.btnRunMethod.Location = new System.Drawing.Point(365, 52);
            this.btnRunMethod.Name = "btnRunMethod";
            this.btnRunMethod.Size = new System.Drawing.Size(56, 23);
            this.btnRunMethod.TabIndex = 0;
            this.btnRunMethod.Text = "Run";
            this.btnRunMethod.UseVisualStyleBackColor = true;
            this.btnRunMethod.Click += new System.EventHandler(this.btnRunMethod_Click);
            // 
            // comboMethod
            // 
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Items.AddRange(new object[] {
            "\"Method1\"",
            "\"Method2\"",
            "\"Method3\""});
            this.comboMethod.Location = new System.Drawing.Point(67, 51);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(121, 21);
            this.comboMethod.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Method:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Timeout(seconds):";
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(294, 52);
            this.numTimeout.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(65, 20);
            this.numTimeout.TabIndex = 4;
            // 
            // TestPALAdvancedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboMethod);
            this.Controls.Add(this.btnRunMethod);
            this.Name = "TestPALAdvancedControl";
            this.Size = new System.Drawing.Size(436, 174);
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRunMethod;
        private System.Windows.Forms.ComboBox comboMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numTimeout;
    }
}
