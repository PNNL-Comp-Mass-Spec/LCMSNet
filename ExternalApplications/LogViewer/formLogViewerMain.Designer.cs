namespace LogViewer
{
    partial class formLogViewerMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formLogViewerMain));
            this.datagridLogContents = new System.Windows.Forms.DataGridView();
            this.bindingSource_LogData = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textLogFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLogFileSelect = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.textDevice = new System.Windows.Forms.TextBox();
            this.textColumn = new System.Windows.Forms.TextBox();
            this.textSampleName = new System.Windows.Forms.TextBox();
            this.textType = new System.Windows.Forms.TextBox();
            this.buttonFindLogEntries = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickStop = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.datagridLogContents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_LogData)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // datagridLogContents
            // 
            this.datagridLogContents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagridLogContents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridLogContents.Location = new System.Drawing.Point(12, 192);
            this.datagridLogContents.Name = "datagridLogContents";
            this.datagridLogContents.Size = new System.Drawing.Size(878, 406);
            this.datagridLogContents.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textLogFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonLogFileSelect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(878, 57);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // textLogFile
            // 
            this.textLogFile.Location = new System.Drawing.Point(65, 19);
            this.textLogFile.Name = "textLogFile";
            this.textLogFile.Size = new System.Drawing.Size(713, 20);
            this.textLogFile.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Log File:";
            // 
            // buttonLogFileSelect
            // 
            this.buttonLogFileSelect.Location = new System.Drawing.Point(784, 17);
            this.buttonLogFileSelect.Name = "buttonLogFileSelect";
            this.buttonLogFileSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonLogFileSelect.TabIndex = 4;
            this.buttonLogFileSelect.Text = "Select";
            this.buttonLogFileSelect.UseVisualStyleBackColor = true;
            this.buttonLogFileSelect.Click += new System.EventHandler(this.buttonLogFileSelect_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dateTimePickStop);
            this.groupBox2.Controls.Add(this.dateTimePickStart);
            this.groupBox2.Controls.Add(this.textMessage);
            this.groupBox2.Controls.Add(this.textDevice);
            this.groupBox2.Controls.Add(this.textColumn);
            this.groupBox2.Controls.Add(this.textSampleName);
            this.groupBox2.Controls.Add(this.textType);
            this.groupBox2.Controls.Add(this.buttonFindLogEntries);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(878, 93);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // textMessage
            // 
            this.textMessage.Location = new System.Drawing.Point(519, 53);
            this.textMessage.Name = "textMessage";
            this.textMessage.Size = new System.Drawing.Size(182, 20);
            this.textMessage.TabIndex = 20;
            // 
            // textDevice
            // 
            this.textDevice.Location = new System.Drawing.Point(759, 27);
            this.textDevice.Name = "textDevice";
            this.textDevice.Size = new System.Drawing.Size(100, 20);
            this.textDevice.TabIndex = 19;
            // 
            // textColumn
            // 
            this.textColumn.Location = new System.Drawing.Point(325, 54);
            this.textColumn.Name = "textColumn";
            this.textColumn.Size = new System.Drawing.Size(82, 20);
            this.textColumn.TabIndex = 18;
            // 
            // textSampleName
            // 
            this.textSampleName.Location = new System.Drawing.Point(519, 26);
            this.textSampleName.Name = "textSampleName";
            this.textSampleName.Size = new System.Drawing.Size(182, 20);
            this.textSampleName.TabIndex = 17;
            // 
            // textType
            // 
            this.textType.Location = new System.Drawing.Point(325, 24);
            this.textType.Name = "textType";
            this.textType.Size = new System.Drawing.Size(82, 20);
            this.textType.TabIndex = 16;
            // 
            // buttonFindLogEntries
            // 
            this.buttonFindLogEntries.Location = new System.Drawing.Point(784, 53);
            this.buttonFindLogEntries.Name = "buttonFindLogEntries";
            this.buttonFindLogEntries.Size = new System.Drawing.Size(75, 23);
            this.buttonFindLogEntries.TabIndex = 13;
            this.buttonFindLogEntries.Text = "Find";
            this.buttonFindLogEntries.UseVisualStyleBackColor = true;
            this.buttonFindLogEntries.Click += new System.EventHandler(this.buttonFindLogEntries_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(416, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Message Contains:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(709, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Device:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(274, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Column:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(424, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Sample Contains:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(258, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Entry Type:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Stop Date:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Start Date:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dateTimePickStart
            // 
            this.dateTimePickStart.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickStart.Location = new System.Drawing.Point(75, 24);
            this.dateTimePickStart.Name = "dateTimePickStart";
            this.dateTimePickStart.Size = new System.Drawing.Size(176, 20);
            this.dateTimePickStart.TabIndex = 21;
            // 
            // dateTimePickStop
            // 
            this.dateTimePickStop.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickStop.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickStop.Location = new System.Drawing.Point(75, 50);
            this.dateTimePickStop.Name = "dateTimePickStop";
            this.dateTimePickStop.Size = new System.Drawing.Size(176, 20);
            this.dateTimePickStop.TabIndex = 22;
            // 
            // formLogViewerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 610);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.datagridLogContents);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formLogViewerMain";
            this.Text = "LCMSNet Log Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.datagridLogContents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_LogData)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView datagridLogContents;
        private System.Windows.Forms.BindingSource bindingSource_LogData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textLogFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLogFileSelect;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textMessage;
        private System.Windows.Forms.TextBox textDevice;
        private System.Windows.Forms.TextBox textColumn;
        private System.Windows.Forms.TextBox textSampleName;
        private System.Windows.Forms.TextBox textType;
        private System.Windows.Forms.Button buttonFindLogEntries;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dateTimePickStop;
        private System.Windows.Forms.DateTimePicker dateTimePickStart;
    }
}

