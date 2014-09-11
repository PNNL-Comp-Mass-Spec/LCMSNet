namespace LcmsNet.Devices.Pumps
{
	partial class controlPumpIsco
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
            this.mTabControl = new System.Windows.Forms.TabControl();
            this.mtabPage_PumpControl = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mcontrol_PumpA = new LcmsNet.Devices.Pumps.controlPumpIscoDisplay();
            this.mcontrol_PumpC = new LcmsNet.Devices.Pumps.controlPumpIscoDisplay();
            this.mcontrol_PumpB = new LcmsNet.Devices.Pumps.controlPumpIscoDisplay();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.mbutton_SetControlMode = new System.Windows.Forms.Button();
            this.mcomboBox_ControlMode = new System.Windows.Forms.ComboBox();
            this.mbuttonSetAllPress = new System.Windows.Forms.Button();
            this.mbutton_Refresh = new System.Windows.Forms.Button();
            this.mbuttonStopAll = new System.Windows.Forms.Button();
            this.mbutton_RefillAll = new System.Windows.Forms.Button();
            this.mbutton_SetAllFlow = new System.Windows.Forms.Button();
            this.mbutton_StartAll = new System.Windows.Forms.Button();
            this.mtabPage_Limits = new System.Windows.Forms.TabPage();
            this.mtextBox_Notes = new System.Windows.Forms.TextBox();
            this.mlistView_Limits = new System.Windows.Forms.ListView();
            this.mColHdr_Params_Parameter = new System.Windows.Forms.ColumnHeader();
            this.mColHdr_Params_PumpA = new System.Windows.Forms.ColumnHeader();
            this.mColHdr_Params_PumpB = new System.Windows.Forms.ColumnHeader();
            this.mColHdr_Params_PumpC = new System.Windows.Forms.ColumnHeader();
            this.mtabPage_Advanced = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mbutton_SetOpMode = new System.Windows.Forms.Button();
            this.mcomboBox_OperationMode = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mcomboBox_PumpCount = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.mbutton_SetRefillRate = new System.Windows.Forms.Button();
            this.mtextBox_RefillSpC = new System.Windows.Forms.TextBox();
            this.mtextBox_RefillSpB = new System.Windows.Forms.TextBox();
            this.mtextBox_RefillSpA = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mbutton_SetPortProperties = new System.Windows.Forms.Button();
            this.mtextBox_WriteTimeout = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mtextBox_ReadTimeout = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mcomboBox_UnitAddress = new System.Windows.Forms.ComboBox();
            this.mcomboBox_Ports = new System.Windows.Forms.ComboBox();
            this.mtextBox_BaudRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mcontrol_IscoGraphs = new LcmsNet.Devices.Pumps.controlPumpIscoGraphs();
            this.mgroupBox_Rename.SuspendLayout();
            this.mTabControl.SuspendLayout();
            this.mtabPage_PumpControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.mtabPage_Limits.SuspendLayout();
            this.mtabPage_Advanced.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtextBox_NewDeviceName
            // 
            this.mtextBox_NewDeviceName.Size = new System.Drawing.Size(717, 20);
            // 
            // mbutton_RenameDevice
            // 
            this.mbutton_RenameDevice.Location = new System.Drawing.Point(729, 13);
            // 
            // mgroupBox_Rename
            // 
            this.mgroupBox_Rename.Location = new System.Drawing.Point(0, 491);
            this.mgroupBox_Rename.Size = new System.Drawing.Size(822, 89);
            // 
            // mbutton_initializeDevice
            // 
            this.mbutton_initializeDevice.Size = new System.Drawing.Size(798, 21);
            // 
            // mTabControl
            // 
            this.mTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mTabControl.Controls.Add(this.mtabPage_PumpControl);
            this.mTabControl.Controls.Add(this.mtabPage_Limits);
            this.mTabControl.Controls.Add(this.mtabPage_Advanced);
            this.mTabControl.Location = new System.Drawing.Point(6, 3);
            this.mTabControl.Name = "mTabControl";
            this.mTabControl.SelectedIndex = 0;
            this.mTabControl.Size = new System.Drawing.Size(422, 476);
            this.mTabControl.TabIndex = 3;
            // 
            // mtabPage_PumpControl
            // 
            this.mtabPage_PumpControl.Controls.Add(this.tableLayoutPanel1);
            this.mtabPage_PumpControl.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_PumpControl.Name = "mtabPage_PumpControl";
            this.mtabPage_PumpControl.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_PumpControl.Size = new System.Drawing.Size(414, 450);
            this.mtabPage_PumpControl.TabIndex = 0;
            this.mtabPage_PumpControl.Text = "Pump Controls";
            this.mtabPage_PumpControl.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.mcontrol_PumpA, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.mcontrol_PumpC, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.mcontrol_PumpB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 444);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // mcontrol_PumpA
            // 
            this.mcontrol_PumpA.BackColor = System.Drawing.Color.Transparent;
            this.mcontrol_PumpA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_PumpA.Location = new System.Drawing.Point(4, 4);
            this.mcontrol_PumpA.MaxFlowLimit = 50;
            this.mcontrol_PumpA.MaxFlowSp = 25;
            this.mcontrol_PumpA.MaxPressSp = 10000;
            this.mcontrol_PumpA.MinFlowSp = 0.001;
            this.mcontrol_PumpA.MinPressSp = 10;
            this.mcontrol_PumpA.Name = "mcontrol_PumpA";
            this.mcontrol_PumpA.Setpoint = 0;
            this.mcontrol_PumpA.Size = new System.Drawing.Size(400, 103);
            this.mcontrol_PumpA.TabIndex = 6;
            // 
            // mcontrol_PumpC
            // 
            this.mcontrol_PumpC.BackColor = System.Drawing.Color.Transparent;
            this.mcontrol_PumpC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_PumpC.Location = new System.Drawing.Point(4, 224);
            this.mcontrol_PumpC.MaxFlowLimit = 50;
            this.mcontrol_PumpC.MaxFlowSp = 25;
            this.mcontrol_PumpC.MaxPressSp = 10000;
            this.mcontrol_PumpC.MinFlowSp = 0.001;
            this.mcontrol_PumpC.MinPressSp = 10;
            this.mcontrol_PumpC.Name = "mcontrol_PumpC";
            this.mcontrol_PumpC.Setpoint = 0;
            this.mcontrol_PumpC.Size = new System.Drawing.Size(400, 103);
            this.mcontrol_PumpC.TabIndex = 5;
            this.mcontrol_PumpC.Load += new System.EventHandler(this.mcontrol_PumpC_Load);
            // 
            // mcontrol_PumpB
            // 
            this.mcontrol_PumpB.BackColor = System.Drawing.Color.Transparent;
            this.mcontrol_PumpB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_PumpB.Location = new System.Drawing.Point(4, 114);
            this.mcontrol_PumpB.MaxFlowLimit = 50;
            this.mcontrol_PumpB.MaxFlowSp = 25;
            this.mcontrol_PumpB.MaxPressSp = 10000;
            this.mcontrol_PumpB.MinFlowSp = 0.001;
            this.mcontrol_PumpB.MinPressSp = 10;
            this.mcontrol_PumpB.Name = "mcontrol_PumpB";
            this.mcontrol_PumpB.Setpoint = 0;
            this.mcontrol_PumpB.Size = new System.Drawing.Size(400, 103);
            this.mcontrol_PumpB.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.mbuttonSetAllPress);
            this.panel1.Controls.Add(this.mbutton_Refresh);
            this.panel1.Controls.Add(this.mbuttonStopAll);
            this.panel1.Controls.Add(this.mbutton_RefillAll);
            this.panel1.Controls.Add(this.mbutton_SetAllFlow);
            this.panel1.Controls.Add(this.mbutton_StartAll);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 334);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 106);
            this.panel1.TabIndex = 7;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.mbutton_SetControlMode);
            this.groupBox4.Controls.Add(this.mcomboBox_ControlMode);
            this.groupBox4.Location = new System.Drawing.Point(18, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(88, 83);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Control Mode";
            // 
            // mbutton_SetControlMode
            // 
            this.mbutton_SetControlMode.Location = new System.Drawing.Point(6, 50);
            this.mbutton_SetControlMode.Name = "mbutton_SetControlMode";
            this.mbutton_SetControlMode.Size = new System.Drawing.Size(75, 23);
            this.mbutton_SetControlMode.TabIndex = 9;
            this.mbutton_SetControlMode.Text = "Set";
            this.mbutton_SetControlMode.UseVisualStyleBackColor = true;
            this.mbutton_SetControlMode.Click += new System.EventHandler(this.mbutton_SetControlMode_Click);
            // 
            // mcomboBox_ControlMode
            // 
            this.mcomboBox_ControlMode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.mcomboBox_ControlMode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.mcomboBox_ControlMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_ControlMode.FormattingEnabled = true;
            this.mcomboBox_ControlMode.Items.AddRange(new object[] {
            "Local",
            "Remote"});
            this.mcomboBox_ControlMode.Location = new System.Drawing.Point(6, 19);
            this.mcomboBox_ControlMode.Name = "mcomboBox_ControlMode";
            this.mcomboBox_ControlMode.Size = new System.Drawing.Size(75, 21);
            this.mcomboBox_ControlMode.TabIndex = 6;
            // 
            // mbuttonSetAllPress
            // 
            this.mbuttonSetAllPress.Location = new System.Drawing.Point(213, 57);
            this.mbuttonSetAllPress.Name = "mbuttonSetAllPress";
            this.mbuttonSetAllPress.Size = new System.Drawing.Size(76, 23);
            this.mbuttonSetAllPress.TabIndex = 25;
            this.mbuttonSetAllPress.Text = "Set All Press";
            this.mbuttonSetAllPress.UseVisualStyleBackColor = true;
            this.mbuttonSetAllPress.Click += new System.EventHandler(this.mbuttonSetAllPress_Click);
            // 
            // mbutton_Refresh
            // 
            this.mbutton_Refresh.Location = new System.Drawing.Point(309, 57);
            this.mbutton_Refresh.Name = "mbutton_Refresh";
            this.mbutton_Refresh.Size = new System.Drawing.Size(79, 23);
            this.mbutton_Refresh.TabIndex = 24;
            this.mbutton_Refresh.Text = "Update Disp";
            this.mbutton_Refresh.UseVisualStyleBackColor = true;
            this.mbutton_Refresh.Click += new System.EventHandler(this.mbutton_Refresh_Click);
            // 
            // mbuttonStopAll
            // 
            this.mbuttonStopAll.Location = new System.Drawing.Point(127, 57);
            this.mbuttonStopAll.Name = "mbuttonStopAll";
            this.mbuttonStopAll.Size = new System.Drawing.Size(57, 23);
            this.mbuttonStopAll.TabIndex = 23;
            this.mbuttonStopAll.Text = "Stop All";
            this.mbuttonStopAll.UseVisualStyleBackColor = true;
            this.mbuttonStopAll.Click += new System.EventHandler(this.mbuttonStopAll_Click);
            // 
            // mbutton_RefillAll
            // 
            this.mbutton_RefillAll.Location = new System.Drawing.Point(320, 11);
            this.mbutton_RefillAll.Name = "mbutton_RefillAll";
            this.mbutton_RefillAll.Size = new System.Drawing.Size(57, 23);
            this.mbutton_RefillAll.TabIndex = 22;
            this.mbutton_RefillAll.Text = "Refill All";
            this.mbutton_RefillAll.UseVisualStyleBackColor = true;
            this.mbutton_RefillAll.Click += new System.EventHandler(this.mbutton_RefillAll_Click);
            // 
            // mbutton_SetAllFlow
            // 
            this.mbutton_SetAllFlow.Location = new System.Drawing.Point(213, 11);
            this.mbutton_SetAllFlow.Name = "mbutton_SetAllFlow";
            this.mbutton_SetAllFlow.Size = new System.Drawing.Size(76, 23);
            this.mbutton_SetAllFlow.TabIndex = 21;
            this.mbutton_SetAllFlow.Text = "Set All Flow";
            this.mbutton_SetAllFlow.UseVisualStyleBackColor = true;
            this.mbutton_SetAllFlow.Click += new System.EventHandler(this.mbutton_SetAllFlow_Click);
            // 
            // mbutton_StartAll
            // 
            this.mbutton_StartAll.Location = new System.Drawing.Point(127, 11);
            this.mbutton_StartAll.Name = "mbutton_StartAll";
            this.mbutton_StartAll.Size = new System.Drawing.Size(57, 23);
            this.mbutton_StartAll.TabIndex = 19;
            this.mbutton_StartAll.Text = "Start All";
            this.mbutton_StartAll.UseVisualStyleBackColor = true;
            this.mbutton_StartAll.Click += new System.EventHandler(this.mbutton_StartAll_Click);
            // 
            // mtabPage_Limits
            // 
            this.mtabPage_Limits.Controls.Add(this.mtextBox_Notes);
            this.mtabPage_Limits.Controls.Add(this.mlistView_Limits);
            this.mtabPage_Limits.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_Limits.Name = "mtabPage_Limits";
            this.mtabPage_Limits.Size = new System.Drawing.Size(414, 450);
            this.mtabPage_Limits.TabIndex = 2;
            this.mtabPage_Limits.Text = "Limits";
            this.mtabPage_Limits.UseVisualStyleBackColor = true;
            // 
            // mtextBox_Notes
            // 
            this.mtextBox_Notes.Location = new System.Drawing.Point(13, 258);
            this.mtextBox_Notes.Multiline = true;
            this.mtextBox_Notes.Name = "mtextBox_Notes";
            this.mtextBox_Notes.Size = new System.Drawing.Size(377, 162);
            this.mtextBox_Notes.TabIndex = 1;
            // 
            // mlistView_Limits
            // 
            this.mlistView_Limits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mColHdr_Params_Parameter,
            this.mColHdr_Params_PumpA,
            this.mColHdr_Params_PumpB,
            this.mColHdr_Params_PumpC});
            this.mlistView_Limits.GridLines = true;
            this.mlistView_Limits.Location = new System.Drawing.Point(13, 15);
            this.mlistView_Limits.Name = "mlistView_Limits";
            this.mlistView_Limits.Size = new System.Drawing.Size(377, 219);
            this.mlistView_Limits.TabIndex = 0;
            this.mlistView_Limits.UseCompatibleStateImageBehavior = false;
            this.mlistView_Limits.View = System.Windows.Forms.View.Details;
            // 
            // mColHdr_Params_Parameter
            // 
            this.mColHdr_Params_Parameter.Text = "Parameter";
            this.mColHdr_Params_Parameter.Width = 160;
            // 
            // mColHdr_Params_PumpA
            // 
            this.mColHdr_Params_PumpA.Text = "Pump A";
            this.mColHdr_Params_PumpA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mColHdr_Params_PumpA.Width = 70;
            // 
            // mColHdr_Params_PumpB
            // 
            this.mColHdr_Params_PumpB.Text = "Pump B";
            this.mColHdr_Params_PumpB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mColHdr_Params_PumpB.Width = 70;
            // 
            // mColHdr_Params_PumpC
            // 
            this.mColHdr_Params_PumpC.Text = "Pump C";
            this.mColHdr_Params_PumpC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mColHdr_Params_PumpC.Width = 70;
            // 
            // mtabPage_Advanced
            // 
            this.mtabPage_Advanced.Controls.Add(this.groupBox2);
            this.mtabPage_Advanced.Controls.Add(this.groupBox3);
            this.mtabPage_Advanced.Controls.Add(this.groupBox5);
            this.mtabPage_Advanced.Controls.Add(this.groupBox1);
            this.mtabPage_Advanced.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_Advanced.Name = "mtabPage_Advanced";
            this.mtabPage_Advanced.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_Advanced.Size = new System.Drawing.Size(414, 450);
            this.mtabPage_Advanced.TabIndex = 1;
            this.mtabPage_Advanced.Text = "Advanced";
            this.mtabPage_Advanced.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mbutton_SetOpMode);
            this.groupBox2.Controls.Add(this.mcomboBox_OperationMode);
            this.groupBox2.Location = new System.Drawing.Point(227, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(99, 140);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operation Mode";
            // 
            // mbutton_SetOpMode
            // 
            this.mbutton_SetOpMode.Location = new System.Drawing.Point(6, 76);
            this.mbutton_SetOpMode.Name = "mbutton_SetOpMode";
            this.mbutton_SetOpMode.Size = new System.Drawing.Size(79, 23);
            this.mbutton_SetOpMode.TabIndex = 25;
            this.mbutton_SetOpMode.Text = "Set";
            this.mbutton_SetOpMode.UseVisualStyleBackColor = true;
            this.mbutton_SetOpMode.Click += new System.EventHandler(this.mbutton_SetOpMode_Click);
            // 
            // mcomboBox_OperationMode
            // 
            this.mcomboBox_OperationMode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.mcomboBox_OperationMode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.mcomboBox_OperationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_OperationMode.FormattingEnabled = true;
            this.mcomboBox_OperationMode.Items.AddRange(new object[] {
            "Const Flow",
            "Const Press"});
            this.mcomboBox_OperationMode.Location = new System.Drawing.Point(6, 33);
            this.mcomboBox_OperationMode.Name = "mcomboBox_OperationMode";
            this.mcomboBox_OperationMode.Size = new System.Drawing.Size(79, 21);
            this.mcomboBox_OperationMode.TabIndex = 20;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.mcomboBox_PumpCount);
            this.groupBox3.Location = new System.Drawing.Point(21, 357);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(167, 72);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Pump Count:";
            // 
            // mcomboBox_PumpCount
            // 
            this.mcomboBox_PumpCount.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.mcomboBox_PumpCount.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.mcomboBox_PumpCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_PumpCount.FormattingEnabled = true;
            this.mcomboBox_PumpCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.mcomboBox_PumpCount.Location = new System.Drawing.Point(84, 25);
            this.mcomboBox_PumpCount.Name = "mcomboBox_PumpCount";
            this.mcomboBox_PumpCount.Size = new System.Drawing.Size(50, 21);
            this.mcomboBox_PumpCount.TabIndex = 5;
            this.mcomboBox_PumpCount.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_PumpCount_SelectedIndexChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.mbutton_SetRefillRate);
            this.groupBox5.Controls.Add(this.mtextBox_RefillSpC);
            this.groupBox5.Controls.Add(this.mtextBox_RefillSpB);
            this.groupBox5.Controls.Add(this.mtextBox_RefillSpA);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Location = new System.Drawing.Point(21, 201);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 150);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Refill Rate";
            // 
            // mbutton_SetRefillRate
            // 
            this.mbutton_SetRefillRate.Location = new System.Drawing.Point(70, 109);
            this.mbutton_SetRefillRate.Name = "mbutton_SetRefillRate";
            this.mbutton_SetRefillRate.Size = new System.Drawing.Size(75, 23);
            this.mbutton_SetRefillRate.TabIndex = 30;
            this.mbutton_SetRefillRate.Text = "Set";
            this.mbutton_SetRefillRate.UseVisualStyleBackColor = true;
            this.mbutton_SetRefillRate.Click += new System.EventHandler(this.mbutton_SetRefillRate_Click);
            // 
            // mtextBox_RefillSpC
            // 
            this.mtextBox_RefillSpC.Location = new System.Drawing.Point(70, 83);
            this.mtextBox_RefillSpC.Name = "mtextBox_RefillSpC";
            this.mtextBox_RefillSpC.Size = new System.Drawing.Size(79, 20);
            this.mtextBox_RefillSpC.TabIndex = 29;
            this.mtextBox_RefillSpC.Text = "0.0";
            // 
            // mtextBox_RefillSpB
            // 
            this.mtextBox_RefillSpB.Location = new System.Drawing.Point(70, 57);
            this.mtextBox_RefillSpB.Name = "mtextBox_RefillSpB";
            this.mtextBox_RefillSpB.Size = new System.Drawing.Size(79, 20);
            this.mtextBox_RefillSpB.TabIndex = 28;
            this.mtextBox_RefillSpB.Text = "0.0";
            // 
            // mtextBox_RefillSpA
            // 
            this.mtextBox_RefillSpA.Location = new System.Drawing.Point(70, 31);
            this.mtextBox_RefillSpA.Name = "mtextBox_RefillSpA";
            this.mtextBox_RefillSpA.Size = new System.Drawing.Size(79, 20);
            this.mtextBox_RefillSpA.TabIndex = 27;
            this.mtextBox_RefillSpA.Text = "0.0";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 86);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 13);
            this.label18.TabIndex = 12;
            this.label18.Text = "Pump C:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 60);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(47, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Pump B:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 34);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(47, 13);
            this.label16.TabIndex = 8;
            this.label16.Text = "Pump A:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mbutton_SetPortProperties);
            this.groupBox1.Controls.Add(this.mtextBox_WriteTimeout);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.mtextBox_ReadTimeout);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.mcomboBox_UnitAddress);
            this.groupBox1.Controls.Add(this.mcomboBox_Ports);
            this.groupBox1.Controls.Add(this.mtextBox_BaudRate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(21, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 165);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Port Settings";
            // 
            // mbutton_SetPortProperties
            // 
            this.mbutton_SetPortProperties.Location = new System.Drawing.Point(146, 121);
            this.mbutton_SetPortProperties.Name = "mbutton_SetPortProperties";
            this.mbutton_SetPortProperties.Size = new System.Drawing.Size(75, 23);
            this.mbutton_SetPortProperties.TabIndex = 15;
            this.mbutton_SetPortProperties.Text = "Set";
            this.mbutton_SetPortProperties.UseVisualStyleBackColor = true;
            this.mbutton_SetPortProperties.Click += new System.EventHandler(this.mbutton_SetPortProperties_Click);
            // 
            // mtextBox_WriteTimeout
            // 
            this.mtextBox_WriteTimeout.Location = new System.Drawing.Point(290, 42);
            this.mtextBox_WriteTimeout.Name = "mtextBox_WriteTimeout";
            this.mtextBox_WriteTimeout.Size = new System.Drawing.Size(59, 20);
            this.mtextBox_WriteTimeout.TabIndex = 14;
            this.mtextBox_WriteTimeout.Text = "500";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(187, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Write Timeout (ms):";
            // 
            // mtextBox_ReadTimeout
            // 
            this.mtextBox_ReadTimeout.Location = new System.Drawing.Point(291, 13);
            this.mtextBox_ReadTimeout.Name = "mtextBox_ReadTimeout";
            this.mtextBox_ReadTimeout.Size = new System.Drawing.Size(58, 20);
            this.mtextBox_ReadTimeout.TabIndex = 12;
            this.mtextBox_ReadTimeout.Text = "500";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(186, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Read Timeout (ms):";
            // 
            // mcomboBox_UnitAddress
            // 
            this.mcomboBox_UnitAddress.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.mcomboBox_UnitAddress.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.mcomboBox_UnitAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_UnitAddress.FormattingEnabled = true;
            this.mcomboBox_UnitAddress.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.mcomboBox_UnitAddress.Location = new System.Drawing.Point(70, 29);
            this.mcomboBox_UnitAddress.Name = "mcomboBox_UnitAddress";
            this.mcomboBox_UnitAddress.Size = new System.Drawing.Size(79, 21);
            this.mcomboBox_UnitAddress.TabIndex = 10;
            // 
            // mcomboBox_Ports
            // 
            this.mcomboBox_Ports.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.mcomboBox_Ports.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.mcomboBox_Ports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_Ports.FormattingEnabled = true;
            this.mcomboBox_Ports.Location = new System.Drawing.Point(70, 66);
            this.mcomboBox_Ports.Name = "mcomboBox_Ports";
            this.mcomboBox_Ports.Size = new System.Drawing.Size(79, 21);
            this.mcomboBox_Ports.TabIndex = 9;
            // 
            // mtextBox_BaudRate
            // 
            this.mtextBox_BaudRate.Location = new System.Drawing.Point(290, 71);
            this.mtextBox_BaudRate.Name = "mtextBox_BaudRate";
            this.mtextBox_BaudRate.Size = new System.Drawing.Size(59, 20);
            this.mtextBox_BaudRate.TabIndex = 8;
            this.mtextBox_BaudRate.Text = "9600";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(187, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Baud Rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Unit Addr:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Serial Port:";
            // 
            // mcontrol_IscoGraphs
            // 
            this.mcontrol_IscoGraphs.Location = new System.Drawing.Point(430, 25);
            this.mcontrol_IscoGraphs.Name = "mcontrol_IscoGraphs";
            this.mcontrol_IscoGraphs.Size = new System.Drawing.Size(384, 453);
            this.mcontrol_IscoGraphs.TabIndex = 4;
            // 
            // controlPumpIsco
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mTabControl);
            this.Controls.Add(this.mcontrol_IscoGraphs);
            this.Name = "controlPumpIsco";
            this.Size = new System.Drawing.Size(822, 580);
            this.Controls.SetChildIndex(this.mcontrol_IscoGraphs, 0);
            this.Controls.SetChildIndex(this.mTabControl, 0);
            this.Controls.SetChildIndex(this.mgroupBox_Rename, 0);
            this.mgroupBox_Rename.ResumeLayout(false);
            this.mgroupBox_Rename.PerformLayout();
            this.mTabControl.ResumeLayout(false);
            this.mtabPage_PumpControl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.mtabPage_Limits.ResumeLayout(false);
            this.mtabPage_Limits.PerformLayout();
            this.mtabPage_Advanced.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl mTabControl;
		private System.Windows.Forms.TabPage mtabPage_PumpControl;
		private System.Windows.Forms.TabPage mtabPage_Advanced;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox mtextBox_BaudRate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox mcomboBox_PumpCount;
		private System.Windows.Forms.TextBox mtextBox_WriteTimeout;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox mtextBox_ReadTimeout;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox mcomboBox_UnitAddress;
		private System.Windows.Forms.ComboBox mcomboBox_Ports;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label16;
		private controlPumpIscoDisplay mcontrol_PumpB;
		private controlPumpIscoDisplay mcontrol_PumpC;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private controlPumpIscoDisplay mcontrol_PumpA;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button mbuttonSetAllPress;
		private System.Windows.Forms.Button mbutton_Refresh;
		private System.Windows.Forms.Button mbuttonStopAll;
		private System.Windows.Forms.Button mbutton_RefillAll;
		private System.Windows.Forms.Button mbutton_SetAllFlow;
		private System.Windows.Forms.Button mbutton_StartAll;
		private System.Windows.Forms.Button mbutton_SetPortProperties;
		private System.Windows.Forms.TextBox mtextBox_RefillSpA;
		private System.Windows.Forms.TextBox mtextBox_RefillSpC;
		private System.Windows.Forms.TextBox mtextBox_RefillSpB;
		private System.Windows.Forms.Button mbutton_SetRefillRate;
		private System.Windows.Forms.TabPage mtabPage_Limits;
		private System.Windows.Forms.ListView mlistView_Limits;
		private System.Windows.Forms.ColumnHeader mColHdr_Params_Parameter;
		private System.Windows.Forms.ColumnHeader mColHdr_Params_PumpA;
		private System.Windows.Forms.ColumnHeader mColHdr_Params_PumpB;
		private System.Windows.Forms.ColumnHeader mColHdr_Params_PumpC;
		private controlPumpIscoGraphs mcontrol_IscoGraphs;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button mbutton_SetControlMode;
		private System.Windows.Forms.ComboBox mcomboBox_ControlMode;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button mbutton_SetOpMode;
		private System.Windows.Forms.ComboBox mcomboBox_OperationMode;
		private System.Windows.Forms.TextBox mtextBox_Notes;
	}
}
