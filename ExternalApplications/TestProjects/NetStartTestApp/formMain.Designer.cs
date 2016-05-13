namespace NetStartTestApp
{
	partial class formMain
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbAckFail = new System.Windows.Forms.RadioButton();
			this.rbAckNormal = new System.Windows.Forms.RadioButton();
			this.txtReceivedMsg = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbStartSendImm = new System.Windows.Forms.CheckBox();
			this.btnStartSendNow = new System.Windows.Forms.Button();
			this.txtStartMsg = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rbStartError = new System.Windows.Forms.RadioButton();
			this.rbStartStarting = new System.Windows.Forms.RadioButton();
			this.rbStartStarted = new System.Windows.Forms.RadioButton();
			this.rbStartNoResponse = new System.Windows.Forms.RadioButton();
			this.label13 = new System.Windows.Forms.Label();
			this.txtInstName = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.cbStopSendImm = new System.Windows.Forms.CheckBox();
			this.btnStopSendNow = new System.Windows.Forms.Button();
			this.txtStopMsg = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.rbStopError = new System.Windows.Forms.RadioButton();
			this.rbStopStopping = new System.Windows.Forms.RadioButton();
			this.rbStopStopped = new System.Windows.Forms.RadioButton();
			this.rbStopNoResp = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.lbMethods = new System.Windows.Forms.ListBox();
			this.label9 = new System.Windows.Forms.Label();
			this.rbMethodRequestNormal = new System.Windows.Forms.RadioButton();
			this.rbMethodReqNoResponse = new System.Windows.Forms.RadioButton();
			this.cbMethReqSendImm = new System.Windows.Forms.CheckBox();
			this.btnMethReqSendNow = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.lblTopicName = new System.Windows.Forms.Label();
			this.lblBrokerUri = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnClear = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbAckFail);
			this.groupBox1.Controls.Add(this.rbAckNormal);
			this.groupBox1.Location = new System.Drawing.Point(27, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(165, 72);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Message Acknowledge";
			// 
			// rbAckFail
			// 
			this.rbAckFail.AutoSize = true;
			this.rbAckFail.Location = new System.Drawing.Point(12, 42);
			this.rbAckFail.Name = "rbAckFail";
			this.rbAckFail.Size = new System.Drawing.Size(63, 17);
			this.rbAckFail.TabIndex = 2;
			this.rbAckFail.Text = "Fail Ack";
			this.rbAckFail.UseVisualStyleBackColor = true;
			// 
			// rbAckNormal
			// 
			this.rbAckNormal.AutoSize = true;
			this.rbAckNormal.Checked = true;
			this.rbAckNormal.Location = new System.Drawing.Point(12, 19);
			this.rbAckNormal.Name = "rbAckNormal";
			this.rbAckNormal.Size = new System.Drawing.Size(80, 17);
			this.rbAckNormal.TabIndex = 1;
			this.rbAckNormal.TabStop = true;
			this.rbAckNormal.Text = "Ack Normal";
			this.rbAckNormal.UseVisualStyleBackColor = true;
			// 
			// txtReceivedMsg
			// 
			this.txtReceivedMsg.Location = new System.Drawing.Point(623, 34);
			this.txtReceivedMsg.Multiline = true;
			this.txtReceivedMsg.Name = "txtReceivedMsg";
			this.txtReceivedMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtReceivedMsg.Size = new System.Drawing.Size(267, 391);
			this.txtReceivedMsg.TabIndex = 0;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(620, 18);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(99, 13);
			this.label12.TabIndex = 12;
			this.label12.Text = "Message Received";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbStartSendImm);
			this.groupBox2.Controls.Add(this.btnStartSendNow);
			this.groupBox2.Controls.Add(this.txtStartMsg);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.rbStartError);
			this.groupBox2.Controls.Add(this.rbStartStarting);
			this.groupBox2.Controls.Add(this.rbStartStarted);
			this.groupBox2.Controls.Add(this.rbStartNoResponse);
			this.groupBox2.Location = new System.Drawing.Point(27, 108);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(266, 184);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Start Command";
			// 
			// cbStartSendImm
			// 
			this.cbStartSendImm.AutoSize = true;
			this.cbStartSendImm.Checked = true;
			this.cbStartSendImm.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbStartSendImm.Location = new System.Drawing.Point(149, 29);
			this.cbStartSendImm.Name = "cbStartSendImm";
			this.cbStartSendImm.Size = new System.Drawing.Size(109, 17);
			this.cbStartSendImm.TabIndex = 21;
			this.cbStartSendImm.Text = "Send Immediately";
			this.cbStartSendImm.UseVisualStyleBackColor = true;
			this.cbStartSendImm.CheckedChanged += new System.EventHandler(this.cbStartSendImm_CheckedChanged);
			// 
			// btnStartSendNow
			// 
			this.btnStartSendNow.Enabled = false;
			this.btnStartSendNow.Location = new System.Drawing.Point(149, 52);
			this.btnStartSendNow.Name = "btnStartSendNow";
			this.btnStartSendNow.Size = new System.Drawing.Size(75, 23);
			this.btnStartSendNow.TabIndex = 20;
			this.btnStartSendNow.Text = "Send Now";
			this.btnStartSendNow.UseVisualStyleBackColor = true;
			this.btnStartSendNow.Click += new System.EventHandler(this.btnStartSendNow_Click);
			// 
			// txtStartMsg
			// 
			this.txtStartMsg.Location = new System.Drawing.Point(12, 143);
			this.txtStartMsg.Name = "txtStartMsg";
			this.txtStartMsg.Size = new System.Drawing.Size(212, 20);
			this.txtStartMsg.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 127);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "Message:";
			// 
			// rbStartError
			// 
			this.rbStartError.AutoSize = true;
			this.rbStartError.Location = new System.Drawing.Point(12, 98);
			this.rbStartError.Name = "rbStartError";
			this.rbStartError.Size = new System.Drawing.Size(47, 17);
			this.rbStartError.TabIndex = 6;
			this.rbStartError.Text = "Error";
			this.rbStartError.UseVisualStyleBackColor = true;
			// 
			// rbStartStarting
			// 
			this.rbStartStarting.AutoSize = true;
			this.rbStartStarting.Location = new System.Drawing.Point(12, 75);
			this.rbStartStarting.Name = "rbStartStarting";
			this.rbStartStarting.Size = new System.Drawing.Size(61, 17);
			this.rbStartStarting.TabIndex = 5;
			this.rbStartStarting.Text = "Starting";
			this.rbStartStarting.UseVisualStyleBackColor = true;
			// 
			// rbStartStarted
			// 
			this.rbStartStarted.AutoSize = true;
			this.rbStartStarted.Checked = true;
			this.rbStartStarted.Location = new System.Drawing.Point(12, 52);
			this.rbStartStarted.Name = "rbStartStarted";
			this.rbStartStarted.Size = new System.Drawing.Size(59, 17);
			this.rbStartStarted.TabIndex = 4;
			this.rbStartStarted.TabStop = true;
			this.rbStartStarted.Text = "Started";
			this.rbStartStarted.UseVisualStyleBackColor = true;
			// 
			// rbStartNoResponse
			// 
			this.rbStartNoResponse.AutoSize = true;
			this.rbStartNoResponse.Location = new System.Drawing.Point(12, 29);
			this.rbStartNoResponse.Name = "rbStartNoResponse";
			this.rbStartNoResponse.Size = new System.Drawing.Size(90, 17);
			this.rbStartNoResponse.TabIndex = 3;
			this.rbStartNoResponse.Text = "No Response";
			this.rbStartNoResponse.UseVisualStyleBackColor = true;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(313, 13);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(59, 13);
			this.label13.TabIndex = 14;
			this.label13.Text = "Instrument:";
			// 
			// txtInstName
			// 
			this.txtInstName.Location = new System.Drawing.Point(316, 30);
			this.txtInstName.Name = "txtInstName";
			this.txtInstName.Size = new System.Drawing.Size(169, 20);
			this.txtInstName.TabIndex = 15;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.cbStopSendImm);
			this.groupBox3.Controls.Add(this.btnStopSendNow);
			this.groupBox3.Controls.Add(this.txtStopMsg);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.rbStopError);
			this.groupBox3.Controls.Add(this.rbStopStopping);
			this.groupBox3.Controls.Add(this.rbStopStopped);
			this.groupBox3.Controls.Add(this.rbStopNoResp);
			this.groupBox3.Location = new System.Drawing.Point(27, 321);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(266, 184);
			this.groupBox3.TabIndex = 16;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Stop Command";
			// 
			// cbStopSendImm
			// 
			this.cbStopSendImm.AutoSize = true;
			this.cbStopSendImm.Checked = true;
			this.cbStopSendImm.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbStopSendImm.Location = new System.Drawing.Point(149, 29);
			this.cbStopSendImm.Name = "cbStopSendImm";
			this.cbStopSendImm.Size = new System.Drawing.Size(109, 17);
			this.cbStopSendImm.TabIndex = 23;
			this.cbStopSendImm.Text = "Send Immediately";
			this.cbStopSendImm.UseVisualStyleBackColor = true;
			this.cbStopSendImm.CheckedChanged += new System.EventHandler(this.cbStopSendImm_CheckedChanged);
			// 
			// btnStopSendNow
			// 
			this.btnStopSendNow.Enabled = false;
			this.btnStopSendNow.Location = new System.Drawing.Point(149, 52);
			this.btnStopSendNow.Name = "btnStopSendNow";
			this.btnStopSendNow.Size = new System.Drawing.Size(75, 23);
			this.btnStopSendNow.TabIndex = 22;
			this.btnStopSendNow.Text = "Send Now";
			this.btnStopSendNow.UseVisualStyleBackColor = true;
			this.btnStopSendNow.Click += new System.EventHandler(this.btnStopSendNow_Click);
			// 
			// txtStopMsg
			// 
			this.txtStopMsg.Location = new System.Drawing.Point(12, 143);
			this.txtStopMsg.Name = "txtStopMsg";
			this.txtStopMsg.Size = new System.Drawing.Size(212, 20);
			this.txtStopMsg.TabIndex = 16;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 127);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "Message:";
			// 
			// rbStopError
			// 
			this.rbStopError.AutoSize = true;
			this.rbStopError.Location = new System.Drawing.Point(12, 98);
			this.rbStopError.Name = "rbStopError";
			this.rbStopError.Size = new System.Drawing.Size(47, 17);
			this.rbStopError.TabIndex = 6;
			this.rbStopError.Text = "Error";
			this.rbStopError.UseVisualStyleBackColor = true;
			// 
			// rbStopStopping
			// 
			this.rbStopStopping.AutoSize = true;
			this.rbStopStopping.Location = new System.Drawing.Point(12, 75);
			this.rbStopStopping.Name = "rbStopStopping";
			this.rbStopStopping.Size = new System.Drawing.Size(67, 17);
			this.rbStopStopping.TabIndex = 5;
			this.rbStopStopping.Text = "Stopping";
			this.rbStopStopping.UseVisualStyleBackColor = true;
			// 
			// rbStopStopped
			// 
			this.rbStopStopped.AutoSize = true;
			this.rbStopStopped.Checked = true;
			this.rbStopStopped.Location = new System.Drawing.Point(12, 52);
			this.rbStopStopped.Name = "rbStopStopped";
			this.rbStopStopped.Size = new System.Drawing.Size(65, 17);
			this.rbStopStopped.TabIndex = 4;
			this.rbStopStopped.TabStop = true;
			this.rbStopStopped.Text = "Stopped";
			this.rbStopStopped.UseVisualStyleBackColor = true;
			// 
			// rbStopNoResp
			// 
			this.rbStopNoResp.AutoSize = true;
			this.rbStopNoResp.Location = new System.Drawing.Point(12, 29);
			this.rbStopNoResp.Name = "rbStopNoResp";
			this.rbStopNoResp.Size = new System.Drawing.Size(90, 17);
			this.rbStopNoResp.TabIndex = 3;
			this.rbStopNoResp.Text = "No Response";
			this.rbStopNoResp.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.lbMethods);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.rbMethodRequestNormal);
			this.groupBox4.Controls.Add(this.rbMethodReqNoResponse);
			this.groupBox4.Controls.Add(this.cbMethReqSendImm);
			this.groupBox4.Controls.Add(this.btnMethReqSendNow);
			this.groupBox4.Location = new System.Drawing.Point(316, 120);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(284, 276);
			this.groupBox4.TabIndex = 17;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Method Request";
			// 
			// lbMethods
			// 
			this.lbMethods.FormattingEnabled = true;
			this.lbMethods.Items.AddRange(new object[] {
            "GoodMethod1",
            "BadMethod2",
            "WhoCaresAboutThisMedhod",
            "WhatsAMethod"});
			this.lbMethods.Location = new System.Drawing.Point(17, 102);
			this.lbMethods.Name = "lbMethods";
			this.lbMethods.Size = new System.Drawing.Size(185, 160);
			this.lbMethods.TabIndex = 27;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(14, 86);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 13);
			this.label9.TabIndex = 26;
			this.label9.Text = "Method List:";
			// 
			// rbMethodRequestNormal
			// 
			this.rbMethodRequestNormal.AutoSize = true;
			this.rbMethodRequestNormal.Checked = true;
			this.rbMethodRequestNormal.Location = new System.Drawing.Point(17, 40);
			this.rbMethodRequestNormal.Name = "rbMethodRequestNormal";
			this.rbMethodRequestNormal.Size = new System.Drawing.Size(109, 17);
			this.rbMethodRequestNormal.TabIndex = 25;
			this.rbMethodRequestNormal.TabStop = true;
			this.rbMethodRequestNormal.Text = "Normal Response";
			this.rbMethodRequestNormal.UseVisualStyleBackColor = true;
			// 
			// rbMethodReqNoResponse
			// 
			this.rbMethodReqNoResponse.AutoSize = true;
			this.rbMethodReqNoResponse.Location = new System.Drawing.Point(17, 19);
			this.rbMethodReqNoResponse.Name = "rbMethodReqNoResponse";
			this.rbMethodReqNoResponse.Size = new System.Drawing.Size(90, 17);
			this.rbMethodReqNoResponse.TabIndex = 24;
			this.rbMethodReqNoResponse.Text = "No Response";
			this.rbMethodReqNoResponse.UseVisualStyleBackColor = true;
			// 
			// cbMethReqSendImm
			// 
			this.cbMethReqSendImm.AutoSize = true;
			this.cbMethReqSendImm.Checked = true;
			this.cbMethReqSendImm.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbMethReqSendImm.Location = new System.Drawing.Point(169, 19);
			this.cbMethReqSendImm.Name = "cbMethReqSendImm";
			this.cbMethReqSendImm.Size = new System.Drawing.Size(109, 17);
			this.cbMethReqSendImm.TabIndex = 23;
			this.cbMethReqSendImm.Text = "Send Immediately";
			this.cbMethReqSendImm.UseVisualStyleBackColor = true;
			this.cbMethReqSendImm.CheckedChanged += new System.EventHandler(this.cbMethReqSendImm_CheckedChanged);
			// 
			// btnMethReqSendNow
			// 
			this.btnMethReqSendNow.Enabled = false;
			this.btnMethReqSendNow.Location = new System.Drawing.Point(169, 42);
			this.btnMethReqSendNow.Name = "btnMethReqSendNow";
			this.btnMethReqSendNow.Size = new System.Drawing.Size(75, 23);
			this.btnMethReqSendNow.TabIndex = 22;
			this.btnMethReqSendNow.Text = "Send Now";
			this.btnMethReqSendNow.UseVisualStyleBackColor = true;
			this.btnMethReqSendNow.Click += new System.EventHandler(this.btnMethReqSendNow_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.lblTopicName);
			this.groupBox5.Controls.Add(this.lblBrokerUri);
			this.groupBox5.Controls.Add(this.label5);
			this.groupBox5.Controls.Add(this.label4);
			this.groupBox5.Location = new System.Drawing.Point(318, 405);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(282, 147);
			this.groupBox5.TabIndex = 19;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Broker Data";
			// 
			// lblTopicName
			// 
			this.lblTopicName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTopicName.Location = new System.Drawing.Point(15, 89);
			this.lblTopicName.Name = "lblTopicName";
			this.lblTopicName.Size = new System.Drawing.Size(185, 18);
			this.lblTopicName.TabIndex = 22;
			// 
			// lblBrokerUri
			// 
			this.lblBrokerUri.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblBrokerUri.Location = new System.Drawing.Point(15, 45);
			this.lblBrokerUri.Name = "lblBrokerUri";
			this.lblBrokerUri.Size = new System.Drawing.Size(185, 18);
			this.lblBrokerUri.TabIndex = 21;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 76);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 20;
			this.label5.Text = "Topic:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 19;
			this.label4.Text = "Broker URI:";
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(623, 443);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(75, 23);
			this.btnClear.TabIndex = 20;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// formMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(902, 583);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.txtInstName);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.txtReceivedMsg);
			this.Controls.Add(this.groupBox1);
			this.Name = "formMain";
			this.Text = "Net Start Test";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbAckNormal;
		private System.Windows.Forms.TextBox txtReceivedMsg;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.RadioButton rbAckFail;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.RadioButton rbStartError;
		private System.Windows.Forms.RadioButton rbStartStarting;
		private System.Windows.Forms.RadioButton rbStartStarted;
		private System.Windows.Forms.RadioButton rbStartNoResponse;
		private System.Windows.Forms.TextBox txtInstName;
		private System.Windows.Forms.TextBox txtStartMsg;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtStopMsg;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton rbStopError;
		private System.Windows.Forms.RadioButton rbStopStopping;
		private System.Windows.Forms.RadioButton rbStopStopped;
		private System.Windows.Forms.RadioButton rbStopNoResp;
		private System.Windows.Forms.CheckBox cbStartSendImm;
		private System.Windows.Forms.Button btnStartSendNow;
		private System.Windows.Forms.CheckBox cbStopSendImm;
		private System.Windows.Forms.Button btnStopSendNow;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton rbMethodRequestNormal;
		private System.Windows.Forms.RadioButton rbMethodReqNoResponse;
		private System.Windows.Forms.CheckBox cbMethReqSendImm;
		private System.Windows.Forms.Button btnMethReqSendNow;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ListBox lbMethods;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label lblBrokerUri;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblTopicName;
		private System.Windows.Forms.Button btnClear;
	}
}

