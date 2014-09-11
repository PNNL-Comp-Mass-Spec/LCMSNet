namespace Newport.ESP300
{
    partial class controlNewportStage
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefreshPosition = new System.Windows.Forms.Button();
            this.lblAxis3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAxis2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAxis1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAxis3Fwd = new System.Windows.Forms.Button();
            this.btnAxis3Back = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAxis2Fwd = new System.Windows.Forms.Button();
            this.btnAxis2Back = new System.Windows.Forms.Button();
            this.btnAxis1Back = new System.Windows.Forms.Button();
            this.btnAxis1Fwd = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnRemovePosition = new System.Windows.Forms.Button();
            this.lblCurrentPos = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPosName = new System.Windows.Forms.TextBox();
            this.lstPositions = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSetPos = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnClearErrors = new System.Windows.Forms.Button();
            this.btnGetErrors = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblAxis3MotorStatus = new System.Windows.Forms.Label();
            this.lblAxis2MotorStatus = new System.Windows.Forms.Label();
            this.lblAxis1MotorStatus = new System.Windows.Forms.Label();
            this.btnAxis3Motor = new System.Windows.Forms.Button();
            this.btnAxis2Motor = new System.Windows.Forms.Button();
            this.btnAxis1Motor = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnClosePort = new System.Windows.Forms.Button();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.m_serialPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRefreshPosition);
            this.groupBox1.Controls.Add(this.lblAxis3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblAxis2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblAxis1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(124, 146);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Position";
            // 
            // btnRefreshPosition
            // 
            this.btnRefreshPosition.Location = new System.Drawing.Point(9, 106);
            this.btnRefreshPosition.Name = "btnRefreshPosition";
            this.btnRefreshPosition.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshPosition.TabIndex = 6;
            this.btnRefreshPosition.Text = "Refresh";
            this.btnRefreshPosition.UseVisualStyleBackColor = true;
            this.btnRefreshPosition.Click += new System.EventHandler(this.btnRefreshPosition_Click);
            // 
            // lblAxis3
            // 
            this.lblAxis3.AutoSize = true;
            this.lblAxis3.Location = new System.Drawing.Point(48, 72);
            this.lblAxis3.Name = "lblAxis3";
            this.lblAxis3.Size = new System.Drawing.Size(29, 13);
            this.lblAxis3.TabIndex = 5;
            this.lblAxis3.Text = "0mm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Axis-3";
            // 
            // lblAxis2
            // 
            this.lblAxis2.AutoSize = true;
            this.lblAxis2.Location = new System.Drawing.Point(48, 50);
            this.lblAxis2.Name = "lblAxis2";
            this.lblAxis2.Size = new System.Drawing.Size(29, 13);
            this.lblAxis2.TabIndex = 3;
            this.lblAxis2.Text = "0mm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Axis-2";
            // 
            // lblAxis1
            // 
            this.lblAxis1.AutoSize = true;
            this.lblAxis1.Location = new System.Drawing.Point(48, 28);
            this.lblAxis1.Name = "lblAxis1";
            this.lblAxis1.Size = new System.Drawing.Size(29, 13);
            this.lblAxis1.TabIndex = 1;
            this.lblAxis1.Text = "0mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Axis-1";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(67, 106);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Home";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnAxis3Fwd);
            this.groupBox2.Controls.Add(this.btnAxis3Back);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnAxis2Fwd);
            this.groupBox2.Controls.Add(this.btnReset);
            this.groupBox2.Controls.Add(this.btnAxis2Back);
            this.groupBox2.Controls.Add(this.btnAxis1Back);
            this.groupBox2.Controls.Add(this.btnAxis1Fwd);
            this.groupBox2.Location = new System.Drawing.Point(152, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 146);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Manual Movement";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(133, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Axis3";
            // 
            // btnAxis3Fwd
            // 
            this.btnAxis3Fwd.Location = new System.Drawing.Point(136, 46);
            this.btnAxis3Fwd.Name = "btnAxis3Fwd";
            this.btnAxis3Fwd.Size = new System.Drawing.Size(58, 23);
            this.btnAxis3Fwd.TabIndex = 8;
            this.btnAxis3Fwd.Text = "Forward";
            this.btnAxis3Fwd.UseVisualStyleBackColor = true;
            this.btnAxis3Fwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis3Fwd_MouseDown);
            this.btnAxis3Fwd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis3Fwd_MouseUp);
            // 
            // btnAxis3Back
            // 
            this.btnAxis3Back.Location = new System.Drawing.Point(136, 75);
            this.btnAxis3Back.Name = "btnAxis3Back";
            this.btnAxis3Back.Size = new System.Drawing.Size(58, 23);
            this.btnAxis3Back.TabIndex = 7;
            this.btnAxis3Back.Text = "Back";
            this.btnAxis3Back.UseVisualStyleBackColor = true;
            this.btnAxis3Back.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis3Back_MouseDown);
            this.btnAxis3Back.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis3Back_MouseUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Axis1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Axis2";
            // 
            // btnAxis2Fwd
            // 
            this.btnAxis2Fwd.Location = new System.Drawing.Point(69, 45);
            this.btnAxis2Fwd.Name = "btnAxis2Fwd";
            this.btnAxis2Fwd.Size = new System.Drawing.Size(58, 23);
            this.btnAxis2Fwd.TabIndex = 3;
            this.btnAxis2Fwd.Text = "Forward";
            this.btnAxis2Fwd.UseVisualStyleBackColor = true;
            this.btnAxis2Fwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis2Fwd_MouseDown);
            this.btnAxis2Fwd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis2Fwd_MouseUp);
            // 
            // btnAxis2Back
            // 
            this.btnAxis2Back.Location = new System.Drawing.Point(69, 74);
            this.btnAxis2Back.Name = "btnAxis2Back";
            this.btnAxis2Back.Size = new System.Drawing.Size(58, 23);
            this.btnAxis2Back.TabIndex = 2;
            this.btnAxis2Back.Text = "Back";
            this.btnAxis2Back.UseVisualStyleBackColor = true;
            this.btnAxis2Back.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis2Back_MouseDown);
            this.btnAxis2Back.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis2Back_MouseUp);
            // 
            // btnAxis1Back
            // 
            this.btnAxis1Back.Location = new System.Drawing.Point(6, 74);
            this.btnAxis1Back.Name = "btnAxis1Back";
            this.btnAxis1Back.Size = new System.Drawing.Size(58, 23);
            this.btnAxis1Back.TabIndex = 1;
            this.btnAxis1Back.Text = "Back";
            this.btnAxis1Back.UseVisualStyleBackColor = true;
            this.btnAxis1Back.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis1Back_MouseDown);
            this.btnAxis1Back.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis1Back_MouseUp);
            // 
            // btnAxis1Fwd
            // 
            this.btnAxis1Fwd.Location = new System.Drawing.Point(6, 45);
            this.btnAxis1Fwd.Name = "btnAxis1Fwd";
            this.btnAxis1Fwd.Size = new System.Drawing.Size(58, 23);
            this.btnAxis1Fwd.TabIndex = 0;
            this.btnAxis1Fwd.Text = "Forward";
            this.btnAxis1Fwd.UseVisualStyleBackColor = true;
            this.btnAxis1Fwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnAxis1Fwd_MouseDown);
            this.btnAxis1Fwd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnAxis1Fwd_MouseUp);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnRemovePosition);
            this.groupBox3.Controls.Add(this.lblCurrentPos);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtPosName);
            this.groupBox3.Controls.Add(this.lstPositions);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnSetPos);
            this.groupBox3.Controls.Add(this.btnGo);
            this.groupBox3.Location = new System.Drawing.Point(378, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(381, 173);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Auto-Positioning";
            // 
            // btnRemovePosition
            // 
            this.btnRemovePosition.Location = new System.Drawing.Point(207, 92);
            this.btnRemovePosition.Name = "btnRemovePosition";
            this.btnRemovePosition.Size = new System.Drawing.Size(168, 23);
            this.btnRemovePosition.TabIndex = 14;
            this.btnRemovePosition.Text = "Delete Position";
            this.btnRemovePosition.UseVisualStyleBackColor = true;
            this.btnRemovePosition.Click += new System.EventHandler(this.btnRemovePosition_Click);
            // 
            // lblCurrentPos
            // 
            this.lblCurrentPos.Location = new System.Drawing.Point(204, 32);
            this.lblCurrentPos.Name = "lblCurrentPos";
            this.lblCurrentPos.Size = new System.Drawing.Size(171, 28);
            this.lblCurrentPos.TabIndex = 13;
            this.lblCurrentPos.Text = "NoPosition";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(204, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Current Position:";
            // 
            // txtPosName
            // 
            this.txtPosName.Location = new System.Drawing.Point(129, 141);
            this.txtPosName.Name = "txtPosName";
            this.txtPosName.Size = new System.Drawing.Size(100, 20);
            this.txtPosName.TabIndex = 11;
            // 
            // lstPositions
            // 
            this.lstPositions.FormattingEnabled = true;
            this.lstPositions.Location = new System.Drawing.Point(23, 19);
            this.lstPositions.Name = "lstPositions";
            this.lstPositions.Size = new System.Drawing.Size(169, 95);
            this.lstPositions.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Set Current Position as:";
            // 
            // btnSetPos
            // 
            this.btnSetPos.Location = new System.Drawing.Point(236, 138);
            this.btnSetPos.Name = "btnSetPos";
            this.btnSetPos.Size = new System.Drawing.Size(75, 23);
            this.btnSetPos.TabIndex = 8;
            this.btnSetPos.Text = "Set Position";
            this.btnSetPos.UseVisualStyleBackColor = true;
            this.btnSetPos.Click += new System.EventHandler(this.btnSetPos_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(207, 63);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(168, 23);
            this.btnGo.TabIndex = 8;
            this.btnGo.Text = "Go to Selected Position";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnClearErrors);
            this.groupBox4.Controls.Add(this.btnGetErrors);
            this.groupBox4.Location = new System.Drawing.Point(378, 194);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(381, 50);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Errors";
            // 
            // btnClearErrors
            // 
            this.btnClearErrors.Location = new System.Drawing.Point(169, 19);
            this.btnClearErrors.Name = "btnClearErrors";
            this.btnClearErrors.Size = new System.Drawing.Size(134, 23);
            this.btnClearErrors.TabIndex = 1;
            this.btnClearErrors.Text = "Clear Errors";
            this.btnClearErrors.UseVisualStyleBackColor = true;
            this.btnClearErrors.Click += new System.EventHandler(this.btnClearErrors_Click);
            // 
            // btnGetErrors
            // 
            this.btnGetErrors.Location = new System.Drawing.Point(9, 19);
            this.btnGetErrors.Name = "btnGetErrors";
            this.btnGetErrors.Size = new System.Drawing.Size(134, 23);
            this.btnGetErrors.TabIndex = 0;
            this.btnGetErrors.Text = "Get Error Messages";
            this.btnGetErrors.UseVisualStyleBackColor = true;
            this.btnGetErrors.Click += new System.EventHandler(this.btnGetErrors_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(889, 574);
            this.tabControl1.TabIndex = 9;
            this.tabControl1.VisibleChanged += new System.EventHandler(this.tabControl1_VisibleChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(881, 548);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Controls";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblAxis3MotorStatus);
            this.groupBox5.Controls.Add(this.lblAxis2MotorStatus);
            this.groupBox5.Controls.Add(this.lblAxis1MotorStatus);
            this.groupBox5.Controls.Add(this.btnAxis3Motor);
            this.groupBox5.Controls.Add(this.btnAxis2Motor);
            this.groupBox5.Controls.Add(this.btnAxis1Motor);
            this.groupBox5.Location = new System.Drawing.Point(6, 167);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(366, 94);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Motor Control";
            // 
            // lblAxis3MotorStatus
            // 
            this.lblAxis3MotorStatus.AutoSize = true;
            this.lblAxis3MotorStatus.Location = new System.Drawing.Point(196, 64);
            this.lblAxis3MotorStatus.Name = "lblAxis3MotorStatus";
            this.lblAxis3MotorStatus.Size = new System.Drawing.Size(70, 13);
            this.lblAxis3MotorStatus.TabIndex = 15;
            this.lblAxis3MotorStatus.Text = "Motor3Status";
            // 
            // lblAxis2MotorStatus
            // 
            this.lblAxis2MotorStatus.AutoSize = true;
            this.lblAxis2MotorStatus.Location = new System.Drawing.Point(101, 64);
            this.lblAxis2MotorStatus.Name = "lblAxis2MotorStatus";
            this.lblAxis2MotorStatus.Size = new System.Drawing.Size(70, 13);
            this.lblAxis2MotorStatus.TabIndex = 14;
            this.lblAxis2MotorStatus.Text = "Motor2Status";
            // 
            // lblAxis1MotorStatus
            // 
            this.lblAxis1MotorStatus.AutoSize = true;
            this.lblAxis1MotorStatus.Location = new System.Drawing.Point(6, 64);
            this.lblAxis1MotorStatus.Name = "lblAxis1MotorStatus";
            this.lblAxis1MotorStatus.Size = new System.Drawing.Size(70, 13);
            this.lblAxis1MotorStatus.TabIndex = 13;
            this.lblAxis1MotorStatus.Text = "Motor1Status";
            // 
            // btnAxis3Motor
            // 
            this.btnAxis3Motor.Location = new System.Drawing.Point(199, 29);
            this.btnAxis3Motor.Name = "btnAxis3Motor";
            this.btnAxis3Motor.Size = new System.Drawing.Size(89, 23);
            this.btnAxis3Motor.TabIndex = 12;
            this.btnAxis3Motor.Text = "Motor 3";
            this.btnAxis3Motor.UseVisualStyleBackColor = true;
            this.btnAxis3Motor.Click += new System.EventHandler(this.btnAxis3Motor_Click);
            // 
            // btnAxis2Motor
            // 
            this.btnAxis2Motor.Location = new System.Drawing.Point(104, 29);
            this.btnAxis2Motor.Name = "btnAxis2Motor";
            this.btnAxis2Motor.Size = new System.Drawing.Size(89, 23);
            this.btnAxis2Motor.TabIndex = 11;
            this.btnAxis2Motor.Text = "Motor 2";
            this.btnAxis2Motor.UseVisualStyleBackColor = true;
            this.btnAxis2Motor.Click += new System.EventHandler(this.btnAxis2Motor_Click);
            // 
            // btnAxis1Motor
            // 
            this.btnAxis1Motor.Location = new System.Drawing.Point(9, 29);
            this.btnAxis1Motor.Name = "btnAxis1Motor";
            this.btnAxis1Motor.Size = new System.Drawing.Size(89, 23);
            this.btnAxis1Motor.TabIndex = 10;
            this.btnAxis1Motor.Text = "Motor 1";
            this.btnAxis1Motor.UseVisualStyleBackColor = true;
            this.btnAxis1Motor.Click += new System.EventHandler(this.btnAxis1Motor_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnClosePort);
            this.tabPage2.Controls.Add(this.btnOpenPort);
            this.tabPage2.Controls.Add(this.m_serialPropertyGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(881, 548);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Serial";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnClosePort
            // 
            this.btnClosePort.Location = new System.Drawing.Point(87, 293);
            this.btnClosePort.Name = "btnClosePort";
            this.btnClosePort.Size = new System.Drawing.Size(75, 23);
            this.btnClosePort.TabIndex = 14;
            this.btnClosePort.Text = "Close Port";
            this.btnClosePort.UseVisualStyleBackColor = true;
            this.btnClosePort.Click += new System.EventHandler(this.btnClosePort_Click);
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(6, 293);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(75, 23);
            this.btnOpenPort.TabIndex = 13;
            this.btnOpenPort.Text = "Open Port";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // m_serialPropertyGrid
            // 
            this.m_serialPropertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_serialPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.m_serialPropertyGrid.Name = "m_serialPropertyGrid";
            this.m_serialPropertyGrid.Size = new System.Drawing.Size(875, 284);
            this.m_serialPropertyGrid.TabIndex = 12;
            this.m_serialPropertyGrid.Click += new System.EventHandler(this.m_serialPropertyGrid_Click);
            // 
            // controlNewportStage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "controlNewportStage";
            this.Size = new System.Drawing.Size(889, 574);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRefreshPosition;
        private System.Windows.Forms.Label lblAxis3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAxis2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblAxis1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAxis2Fwd;
        private System.Windows.Forms.Button btnAxis2Back;
        private System.Windows.Forms.Button btnAxis1Back;
        private System.Windows.Forms.Button btnAxis1Fwd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSetPos;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnGetErrors;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAxis3Fwd;
        private System.Windows.Forms.Button btnAxis3Back;
        private System.Windows.Forms.ListBox lstPositions;
        private System.Windows.Forms.TextBox txtPosName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid m_serialPropertyGrid;
        private System.Windows.Forms.Label lblCurrentPos;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnRemovePosition;
        private System.Windows.Forms.Button btnClearErrors;
        private System.Windows.Forms.Button btnClosePort;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblAxis3MotorStatus;
        private System.Windows.Forms.Label lblAxis2MotorStatus;
        private System.Windows.Forms.Label lblAxis1MotorStatus;
        private System.Windows.Forms.Button btnAxis3Motor;
        private System.Windows.Forms.Button btnAxis2Motor;
        private System.Windows.Forms.Button btnAxis1Motor;
    }
}
