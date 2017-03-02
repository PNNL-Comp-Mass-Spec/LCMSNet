namespace ActiveMQModuleTest
{
    partial class Form1
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
            this.txtStatusTopic = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtQueueName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBrokerURI = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSendWithReply = new System.Windows.Forms.Button();
            this.txtQueueReply = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQueueMsg = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSendStatus = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtCmdReceived = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtStatusTopic);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtQueueName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtBrokerURI);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(29, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 100);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config";
            // 
            // txtStatusTopic
            // 
            this.txtStatusTopic.Location = new System.Drawing.Point(95, 71);
            this.txtStatusTopic.Name = "txtStatusTopic";
            this.txtStatusTopic.Size = new System.Drawing.Size(210, 20);
            this.txtStatusTopic.TabIndex = 7;
            this.txtStatusTopic.Text = "DAC.Status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Status Topic";
            // 
            // txtQueueName
            // 
            this.txtQueueName.Location = new System.Drawing.Point(95, 45);
            this.txtQueueName.Name = "txtQueueName";
            this.txtQueueName.Size = new System.Drawing.Size(210, 20);
            this.txtQueueName.TabIndex = 5;
            this.txtQueueName.Text = "DAC.Control";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Control Queue";
            // 
            // txtBrokerURI
            // 
            this.txtBrokerURI.Location = new System.Drawing.Point(95, 19);
            this.txtBrokerURI.Name = "txtBrokerURI";
            this.txtBrokerURI.Size = new System.Drawing.Size(210, 20);
            this.txtBrokerURI.TabIndex = 3;
            this.txtBrokerURI.Text = "tcp://prismdevii.pnl.gov:61616";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bruker URI";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSendWithReply);
            this.groupBox2.Controls.Add(this.txtQueueReply);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtQueueMsg);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(29, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 120);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Send With Reply";
            // 
            // btnSendWithReply
            // 
            this.btnSendWithReply.Location = new System.Drawing.Point(152, 91);
            this.btnSendWithReply.Name = "btnSendWithReply";
            this.btnSendWithReply.Size = new System.Drawing.Size(75, 23);
            this.btnSendWithReply.TabIndex = 8;
            this.btnSendWithReply.Text = "Send";
            this.btnSendWithReply.UseVisualStyleBackColor = true;
            this.btnSendWithReply.Click += new System.EventHandler(this.btnSendWithReply_Click);
            // 
            // txtQueueReply
            // 
            this.txtQueueReply.Location = new System.Drawing.Point(95, 55);
            this.txtQueueReply.Name = "txtQueueReply";
            this.txtQueueReply.Size = new System.Drawing.Size(210, 20);
            this.txtQueueReply.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Reply";
            // 
            // txtQueueMsg
            // 
            this.txtQueueMsg.Location = new System.Drawing.Point(95, 19);
            this.txtQueueMsg.Name = "txtQueueMsg";
            this.txtQueueMsg.Size = new System.Drawing.Size(210, 20);
            this.txtQueueMsg.TabIndex = 3;
            this.txtQueueMsg.Text = "Test Message";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Message";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSendStatus);
            this.groupBox3.Controls.Add(this.txtStatus);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(29, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(363, 94);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Send Status";
            // 
            // btnSendStatus
            // 
            this.btnSendStatus.Location = new System.Drawing.Point(152, 57);
            this.btnSendStatus.Name = "btnSendStatus";
            this.btnSendStatus.Size = new System.Drawing.Size(75, 23);
            this.btnSendStatus.TabIndex = 8;
            this.btnSendStatus.Text = "Send";
            this.btnSendStatus.UseVisualStyleBackColor = true;
            this.btnSendStatus.Click += new System.EventHandler(this.btnSendStatus_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(95, 19);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(210, 20);
            this.txtStatus.TabIndex = 3;
            this.txtStatus.Text = "My Status";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Message";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtCmdReceived);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(29, 344);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(363, 60);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Send Status";
            // 
            // txtCmdReceived
            // 
            this.txtCmdReceived.Location = new System.Drawing.Point(95, 19);
            this.txtCmdReceived.Name = "txtCmdReceived";
            this.txtCmdReceived.Size = new System.Drawing.Size(210, 20);
            this.txtCmdReceived.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Cmd Received";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 447);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtStatusTopic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtQueueName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBrokerURI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSendWithReply;
        private System.Windows.Forms.TextBox txtQueueReply;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQueueMsg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSendStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtCmdReceived;
        private System.Windows.Forms.Label label5;
    }
}

