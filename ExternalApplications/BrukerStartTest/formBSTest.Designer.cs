namespace BrukerStartTest
{
	partial class formBSTest
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
			this.btnGetMethods = new System.Windows.Forms.Button();
			this.cboMethodName = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtDSName = new System.Windows.Forms.TextBox();
			this.btnStartAcq = new System.Windows.Forms.Button();
			this.btnEndAcq = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnGetMethods
			// 
			this.btnGetMethods.Location = new System.Drawing.Point(372, 63);
			this.btnGetMethods.Name = "btnGetMethods";
			this.btnGetMethods.Size = new System.Drawing.Size(75, 23);
			this.btnGetMethods.TabIndex = 0;
			this.btnGetMethods.Text = "Get Methods";
			this.btnGetMethods.UseVisualStyleBackColor = true;
			this.btnGetMethods.Click += new System.EventHandler(this.btnGetMethods_Click);
			// 
			// cboMethodName
			// 
			this.cboMethodName.FormattingEnabled = true;
			this.cboMethodName.Items.AddRange(new object[] {
            "-- None --"});
			this.cboMethodName.Location = new System.Drawing.Point(105, 63);
			this.cboMethodName.Name = "cboMethodName";
			this.cboMethodName.Size = new System.Drawing.Size(242, 21);
			this.cboMethodName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "DS Name";
			// 
			// txtDSName
			// 
			this.txtDSName.Location = new System.Drawing.Point(105, 25);
			this.txtDSName.Name = "txtDSName";
			this.txtDSName.Size = new System.Drawing.Size(342, 20);
			this.txtDSName.TabIndex = 3;
			// 
			// btnStartAcq
			// 
			this.btnStartAcq.Location = new System.Drawing.Point(105, 116);
			this.btnStartAcq.Name = "btnStartAcq";
			this.btnStartAcq.Size = new System.Drawing.Size(75, 23);
			this.btnStartAcq.TabIndex = 4;
			this.btnStartAcq.Text = "Start Acq";
			this.btnStartAcq.UseVisualStyleBackColor = true;
			this.btnStartAcq.Click += new System.EventHandler(this.btnStartAcq_Click);
			// 
			// btnEndAcq
			// 
			this.btnEndAcq.Location = new System.Drawing.Point(272, 116);
			this.btnEndAcq.Name = "btnEndAcq";
			this.btnEndAcq.Size = new System.Drawing.Size(75, 23);
			this.btnEndAcq.TabIndex = 5;
			this.btnEndAcq.Text = "End Acq";
			this.btnEndAcq.UseVisualStyleBackColor = true;
			this.btnEndAcq.Click += new System.EventHandler(this.btnEndAcq_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(25, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Method Name";
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(31, 186);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(416, 20);
			this.lblStatus.TabIndex = 7;
			// 
			// formBSTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(526, 242);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnEndAcq);
			this.Controls.Add(this.btnStartAcq);
			this.Controls.Add(this.txtDSName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboMethodName);
			this.Controls.Add(this.btnGetMethods);
			this.Name = "formBSTest";
			this.Text = "BS Test Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formBSTest_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGetMethods;
		private System.Windows.Forms.ComboBox cboMethodName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtDSName;
		private System.Windows.Forms.Button btnStartAcq;
		private System.Windows.Forms.Button btnEndAcq;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblStatus;
	}
}

