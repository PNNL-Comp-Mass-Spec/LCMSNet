//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
//*********************************************************************************************************
using LcmsNetDataClasses.Devices.Pumps;

namespace Agilent.Devices.Pumps
{
    partial class controlPumpAgilent
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
            this.mbutton_SetFlowRate = new System.Windows.Forms.Button();
            this.mgroupBox_Flow = new System.Windows.Forms.GroupBox();
            this.mnum_flowRate = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.mtextBox_ActualFlowRate = new System.Windows.Forms.TextBox();
            this.mbutton_GetFlowRate = new System.Windows.Forms.Button();
            this.mlabel_ulmin = new System.Windows.Forms.Label();
            this.mgroupbox_Pressure = new System.Windows.Forms.GroupBox();
            this.mbutton_GetPressure = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextBox_Pressure = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mnum_mixerVolume = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.mtextBox_GetMixerVol = new System.Windows.Forms.TextBox();
            this.mbutton_GetMixerVol = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.mbutton_SetMixerVol = new System.Windows.Forms.Button();
            this.mGroupBox_Mode = new System.Windows.Forms.GroupBox();
            this.mcomboBox_Mode = new System.Windows.Forms.ComboBox();
            this.mbutton_SetMode = new System.Windows.Forms.Button();
            this.mtabControl = new System.Windows.Forms.TabControl();
            this.mtabPage_PumpControls = new System.Windows.Forms.TabPage();
            this.mbutton_purge = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mnum_percentB = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.mtextbox_percentB = new System.Windows.Forms.TextBox();
            this.mbutton_getPercentB = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.mbutton_setPercentB = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mcomboBox_methods = new System.Windows.Forms.ComboBox();
            this.mbutton_stop = new System.Windows.Forms.Button();
            this.mbutton_start = new System.Windows.Forms.Button();
            this.mgroupBox_pumpsState = new System.Windows.Forms.GroupBox();
            this.mbutton_off = new System.Windows.Forms.Button();
            this.mbutton_on = new System.Windows.Forms.Button();
            this.mtabPage_SerialProps = new System.Windows.Forms.TabPage();
            this.mbutton_setPortName = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.mbutton_loadMethods = new System.Windows.Forms.Button();
            this.mbutton_saveMethod = new System.Windows.Forms.Button();
            this.mtextbox_method = new System.Windows.Forms.RichTextBox();
            this.mbutton_retrieve = new System.Windows.Forms.Button();
            this.mlabel_communication = new System.Windows.Forms.Label();
            this.mcomboBox_comPort = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mcontrol_pumpDisplay = new LcmsNetDataClasses.Devices.Pumps.controlPumpDisplay();
            this.mgroupBox_Flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowRate)).BeginInit();
            this.mgroupbox_Pressure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_mixerVolume)).BeginInit();
            this.mGroupBox_Mode.SuspendLayout();
            this.mtabControl.SuspendLayout();
            this.mtabPage_PumpControls.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_percentB)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.mgroupBox_pumpsState.SuspendLayout();
            this.mtabPage_SerialProps.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbutton_SetFlowRate
            // 
            this.mbutton_SetFlowRate.Location = new System.Drawing.Point(133, 19);
            this.mbutton_SetFlowRate.Name = "mbutton_SetFlowRate";
            this.mbutton_SetFlowRate.Size = new System.Drawing.Size(56, 23);
            this.mbutton_SetFlowRate.TabIndex = 0;
            this.mbutton_SetFlowRate.Text = "Set";
            this.mbutton_SetFlowRate.UseVisualStyleBackColor = true;
            this.mbutton_SetFlowRate.Click += new System.EventHandler(this.mbutton_SetFlowRate_Click);
            // 
            // mgroupBox_Flow
            // 
            this.mgroupBox_Flow.Controls.Add(this.mnum_flowRate);
            this.mgroupBox_Flow.Controls.Add(this.label1);
            this.mgroupBox_Flow.Controls.Add(this.mtextBox_ActualFlowRate);
            this.mgroupBox_Flow.Controls.Add(this.mbutton_GetFlowRate);
            this.mgroupBox_Flow.Controls.Add(this.mlabel_ulmin);
            this.mgroupBox_Flow.Controls.Add(this.mbutton_SetFlowRate);
            this.mgroupBox_Flow.Location = new System.Drawing.Point(13, 12);
            this.mgroupBox_Flow.Name = "mgroupBox_Flow";
            this.mgroupBox_Flow.Size = new System.Drawing.Size(357, 51);
            this.mgroupBox_Flow.TabIndex = 1;
            this.mgroupBox_Flow.TabStop = false;
            this.mgroupBox_Flow.Text = "Flow Rate";
            // 
            // mnum_flowRate
            // 
            this.mnum_flowRate.DecimalPlaces = 4;
            this.mnum_flowRate.Location = new System.Drawing.Point(7, 22);
            this.mnum_flowRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mnum_flowRate.Name = "mnum_flowRate";
            this.mnum_flowRate.Size = new System.Drawing.Size(79, 20);
            this.mnum_flowRate.TabIndex = 6;
            this.mnum_flowRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(248, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "uL/min";
            // 
            // mtextBox_ActualFlowRate
            // 
            this.mtextBox_ActualFlowRate.Location = new System.Drawing.Point(199, 22);
            this.mtextBox_ActualFlowRate.Name = "mtextBox_ActualFlowRate";
            this.mtextBox_ActualFlowRate.ReadOnly = true;
            this.mtextBox_ActualFlowRate.Size = new System.Drawing.Size(46, 20);
            this.mtextBox_ActualFlowRate.TabIndex = 4;
            this.mtextBox_ActualFlowRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_GetFlowRate
            // 
            this.mbutton_GetFlowRate.Location = new System.Drawing.Point(294, 20);
            this.mbutton_GetFlowRate.Name = "mbutton_GetFlowRate";
            this.mbutton_GetFlowRate.Size = new System.Drawing.Size(57, 23);
            this.mbutton_GetFlowRate.TabIndex = 3;
            this.mbutton_GetFlowRate.Text = "Retrieve";
            this.mbutton_GetFlowRate.UseVisualStyleBackColor = true;
            this.mbutton_GetFlowRate.Click += new System.EventHandler(this.mbutton_GetFlowRate_Click);
            // 
            // mlabel_ulmin
            // 
            this.mlabel_ulmin.AutoSize = true;
            this.mlabel_ulmin.Location = new System.Drawing.Point(91, 24);
            this.mlabel_ulmin.Name = "mlabel_ulmin";
            this.mlabel_ulmin.Size = new System.Drawing.Size(40, 13);
            this.mlabel_ulmin.TabIndex = 2;
            this.mlabel_ulmin.Text = "uL/min";
            // 
            // mgroupbox_Pressure
            // 
            this.mgroupbox_Pressure.Controls.Add(this.mbutton_GetPressure);
            this.mgroupbox_Pressure.Controls.Add(this.label3);
            this.mgroupbox_Pressure.Controls.Add(this.mtextBox_Pressure);
            this.mgroupbox_Pressure.Location = new System.Drawing.Point(212, 189);
            this.mgroupbox_Pressure.Name = "mgroupbox_Pressure";
            this.mgroupbox_Pressure.Size = new System.Drawing.Size(156, 52);
            this.mgroupbox_Pressure.TabIndex = 6;
            this.mgroupbox_Pressure.TabStop = false;
            this.mgroupbox_Pressure.Text = "Pressure";
            // 
            // mbutton_GetPressure
            // 
            this.mbutton_GetPressure.Location = new System.Drawing.Point(95, 14);
            this.mbutton_GetPressure.Name = "mbutton_GetPressure";
            this.mbutton_GetPressure.Size = new System.Drawing.Size(57, 26);
            this.mbutton_GetPressure.TabIndex = 0;
            this.mbutton_GetPressure.Text = "Retrieve";
            this.mbutton_GetPressure.UseVisualStyleBackColor = true;
            this.mbutton_GetPressure.Click += new System.EventHandler(this.mbutton_GetPressure_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(77, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "psi";
            // 
            // mtextBox_Pressure
            // 
            this.mtextBox_Pressure.Location = new System.Drawing.Point(6, 20);
            this.mtextBox_Pressure.Name = "mtextBox_Pressure";
            this.mtextBox_Pressure.ReadOnly = true;
            this.mtextBox_Pressure.Size = new System.Drawing.Size(65, 20);
            this.mtextBox_Pressure.TabIndex = 1;
            this.mtextBox_Pressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mnum_mixerVolume);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.mtextBox_GetMixerVol);
            this.groupBox1.Controls.Add(this.mbutton_GetMixerVol);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.mbutton_SetMixerVol);
            this.groupBox1.Location = new System.Drawing.Point(13, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 54);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mixer Volume";
            // 
            // mnum_mixerVolume
            // 
            this.mnum_mixerVolume.DecimalPlaces = 4;
            this.mnum_mixerVolume.Location = new System.Drawing.Point(7, 22);
            this.mnum_mixerVolume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mnum_mixerVolume.Name = "mnum_mixerVolume";
            this.mnum_mixerVolume.Size = new System.Drawing.Size(79, 20);
            this.mnum_mixerVolume.TabIndex = 6;
            this.mnum_mixerVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "uL";
            // 
            // mtextBox_GetMixerVol
            // 
            this.mtextBox_GetMixerVol.Location = new System.Drawing.Point(199, 22);
            this.mtextBox_GetMixerVol.Name = "mtextBox_GetMixerVol";
            this.mtextBox_GetMixerVol.ReadOnly = true;
            this.mtextBox_GetMixerVol.Size = new System.Drawing.Size(48, 20);
            this.mtextBox_GetMixerVol.TabIndex = 4;
            this.mtextBox_GetMixerVol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_GetMixerVol
            // 
            this.mbutton_GetMixerVol.Location = new System.Drawing.Point(294, 19);
            this.mbutton_GetMixerVol.Name = "mbutton_GetMixerVol";
            this.mbutton_GetMixerVol.Size = new System.Drawing.Size(57, 23);
            this.mbutton_GetMixerVol.TabIndex = 3;
            this.mbutton_GetMixerVol.Text = "Retrieve";
            this.mbutton_GetMixerVol.UseVisualStyleBackColor = true;
            this.mbutton_GetMixerVol.Click += new System.EventHandler(this.mbutton_GetMixerVol_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "uL";
            // 
            // mbutton_SetMixerVol
            // 
            this.mbutton_SetMixerVol.Location = new System.Drawing.Point(133, 19);
            this.mbutton_SetMixerVol.Name = "mbutton_SetMixerVol";
            this.mbutton_SetMixerVol.Size = new System.Drawing.Size(56, 23);
            this.mbutton_SetMixerVol.TabIndex = 0;
            this.mbutton_SetMixerVol.Text = "Set";
            this.mbutton_SetMixerVol.UseVisualStyleBackColor = true;
            this.mbutton_SetMixerVol.Click += new System.EventHandler(this.mbutton_SetMixerVol_Click);
            // 
            // mGroupBox_Mode
            // 
            this.mGroupBox_Mode.Controls.Add(this.mcomboBox_Mode);
            this.mGroupBox_Mode.Controls.Add(this.mbutton_SetMode);
            this.mGroupBox_Mode.Location = new System.Drawing.Point(13, 189);
            this.mGroupBox_Mode.Name = "mGroupBox_Mode";
            this.mGroupBox_Mode.Size = new System.Drawing.Size(193, 52);
            this.mGroupBox_Mode.TabIndex = 7;
            this.mGroupBox_Mode.TabStop = false;
            this.mGroupBox_Mode.Text = "Mode";
            // 
            // mcomboBox_Mode
            // 
            this.mcomboBox_Mode.FormattingEnabled = true;
            this.mcomboBox_Mode.Location = new System.Drawing.Point(7, 20);
            this.mcomboBox_Mode.Name = "mcomboBox_Mode";
            this.mcomboBox_Mode.Size = new System.Drawing.Size(79, 21);
            this.mcomboBox_Mode.TabIndex = 1;
            // 
            // mbutton_SetMode
            // 
            this.mbutton_SetMode.Location = new System.Drawing.Point(133, 19);
            this.mbutton_SetMode.Name = "mbutton_SetMode";
            this.mbutton_SetMode.Size = new System.Drawing.Size(56, 23);
            this.mbutton_SetMode.TabIndex = 0;
            this.mbutton_SetMode.Text = "Set";
            this.mbutton_SetMode.UseVisualStyleBackColor = true;
            this.mbutton_SetMode.Click += new System.EventHandler(this.mbutton_SetMode_Click);
            // 
            // mtabControl
            // 
            this.mtabControl.Controls.Add(this.mtabPage_PumpControls);
            this.mtabControl.Controls.Add(this.mtabPage_SerialProps);
            this.mtabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.mtabControl.Location = new System.Drawing.Point(0, 0);
            this.mtabControl.Name = "mtabControl";
            this.mtabControl.SelectedIndex = 0;
            this.mtabControl.Size = new System.Drawing.Size(422, 577);
            this.mtabControl.TabIndex = 8;
            // 
            // mtabPage_PumpControls
            // 
            this.mtabPage_PumpControls.AutoScroll = true;
            this.mtabPage_PumpControls.Controls.Add(this.mbutton_purge);
            this.mtabPage_PumpControls.Controls.Add(this.groupBox3);
            this.mtabPage_PumpControls.Controls.Add(this.groupBox2);
            this.mtabPage_PumpControls.Controls.Add(this.mgroupBox_pumpsState);
            this.mtabPage_PumpControls.Controls.Add(this.mgroupbox_Pressure);
            this.mtabPage_PumpControls.Controls.Add(this.groupBox1);
            this.mtabPage_PumpControls.Controls.Add(this.mgroupBox_Flow);
            this.mtabPage_PumpControls.Controls.Add(this.mGroupBox_Mode);
            this.mtabPage_PumpControls.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_PumpControls.Name = "mtabPage_PumpControls";
            this.mtabPage_PumpControls.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_PumpControls.Size = new System.Drawing.Size(414, 551);
            this.mtabPage_PumpControls.TabIndex = 0;
            this.mtabPage_PumpControls.Text = "Pump Controls";
            this.mtabPage_PumpControls.UseVisualStyleBackColor = true;
            // 
            // mbutton_purge
            // 
            this.mbutton_purge.Location = new System.Drawing.Point(236, 254);
            this.mbutton_purge.Name = "mbutton_purge";
            this.mbutton_purge.Size = new System.Drawing.Size(134, 45);
            this.mbutton_purge.TabIndex = 13;
            this.mbutton_purge.Text = "PURGE";
            this.mbutton_purge.UseVisualStyleBackColor = true;
            this.mbutton_purge.Click += new System.EventHandler(this.mbutton_purge_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mnum_percentB);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.mtextbox_percentB);
            this.groupBox3.Controls.Add(this.mbutton_getPercentB);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.mbutton_setPercentB);
            this.groupBox3.Location = new System.Drawing.Point(13, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 54);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "%B";
            // 
            // mnum_percentB
            // 
            this.mnum_percentB.Location = new System.Drawing.Point(7, 17);
            this.mnum_percentB.Name = "mnum_percentB";
            this.mnum_percentB.Size = new System.Drawing.Size(79, 20);
            this.mnum_percentB.TabIndex = 6;
            this.mnum_percentB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(257, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "%";
            // 
            // mtextbox_percentB
            // 
            this.mtextbox_percentB.Location = new System.Drawing.Point(199, 17);
            this.mtextbox_percentB.Name = "mtextbox_percentB";
            this.mtextbox_percentB.ReadOnly = true;
            this.mtextbox_percentB.Size = new System.Drawing.Size(53, 20);
            this.mtextbox_percentB.TabIndex = 4;
            this.mtextbox_percentB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_getPercentB
            // 
            this.mbutton_getPercentB.Location = new System.Drawing.Point(294, 15);
            this.mbutton_getPercentB.Name = "mbutton_getPercentB";
            this.mbutton_getPercentB.Size = new System.Drawing.Size(57, 23);
            this.mbutton_getPercentB.TabIndex = 3;
            this.mbutton_getPercentB.Text = "Retrieve";
            this.mbutton_getPercentB.UseVisualStyleBackColor = true;
            this.mbutton_getPercentB.Click += new System.EventHandler(this.mbutton_getPercentB_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(92, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "%";
            // 
            // mbutton_setPercentB
            // 
            this.mbutton_setPercentB.Location = new System.Drawing.Point(133, 15);
            this.mbutton_setPercentB.Name = "mbutton_setPercentB";
            this.mbutton_setPercentB.Size = new System.Drawing.Size(56, 23);
            this.mbutton_setPercentB.TabIndex = 0;
            this.mbutton_setPercentB.Text = "Set";
            this.mbutton_setPercentB.UseVisualStyleBackColor = true;
            this.mbutton_setPercentB.Click += new System.EventHandler(this.mbutton_setPercentB_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.mcomboBox_methods);
            this.groupBox2.Controls.Add(this.mbutton_stop);
            this.groupBox2.Controls.Add(this.mbutton_start);
            this.groupBox2.Location = new System.Drawing.Point(13, 305);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(357, 240);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operation";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // mcomboBox_methods
            // 
            this.mcomboBox_methods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_methods.FormattingEnabled = true;
            this.mcomboBox_methods.Location = new System.Drawing.Point(20, 19);
            this.mcomboBox_methods.Name = "mcomboBox_methods";
            this.mcomboBox_methods.Size = new System.Drawing.Size(321, 21);
            this.mcomboBox_methods.TabIndex = 2;
            // 
            // mbutton_stop
            // 
            this.mbutton_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_stop.BackColor = System.Drawing.Color.DarkRed;
            this.mbutton_stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_stop.ForeColor = System.Drawing.Color.White;
            this.mbutton_stop.Location = new System.Drawing.Point(223, 46);
            this.mbutton_stop.Name = "mbutton_stop";
            this.mbutton_stop.Size = new System.Drawing.Size(118, 38);
            this.mbutton_stop.TabIndex = 1;
            this.mbutton_stop.Text = "Stop";
            this.mbutton_stop.UseVisualStyleBackColor = false;
            this.mbutton_stop.Click += new System.EventHandler(this.mbutton_stop_Click);
            // 
            // mbutton_start
            // 
            this.mbutton_start.BackColor = System.Drawing.Color.Green;
            this.mbutton_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_start.ForeColor = System.Drawing.Color.White;
            this.mbutton_start.Location = new System.Drawing.Point(20, 46);
            this.mbutton_start.Name = "mbutton_start";
            this.mbutton_start.Size = new System.Drawing.Size(108, 38);
            this.mbutton_start.TabIndex = 0;
            this.mbutton_start.Text = "Start";
            this.mbutton_start.UseVisualStyleBackColor = false;
            this.mbutton_start.Click += new System.EventHandler(this.mbutton_start_Click);
            // 
            // mgroupBox_pumpsState
            // 
            this.mgroupBox_pumpsState.Controls.Add(this.mbutton_off);
            this.mgroupBox_pumpsState.Controls.Add(this.mbutton_on);
            this.mgroupBox_pumpsState.Location = new System.Drawing.Point(13, 247);
            this.mgroupBox_pumpsState.Name = "mgroupBox_pumpsState";
            this.mgroupBox_pumpsState.Size = new System.Drawing.Size(131, 52);
            this.mgroupBox_pumpsState.TabIndex = 8;
            this.mgroupBox_pumpsState.TabStop = false;
            this.mgroupBox_pumpsState.Text = "Pumps On/Off";
            // 
            // mbutton_off
            // 
            this.mbutton_off.BackColor = System.Drawing.Color.Gray;
            this.mbutton_off.ForeColor = System.Drawing.Color.White;
            this.mbutton_off.Location = new System.Drawing.Point(48, 19);
            this.mbutton_off.Name = "mbutton_off";
            this.mbutton_off.Size = new System.Drawing.Size(38, 26);
            this.mbutton_off.TabIndex = 1;
            this.mbutton_off.Text = "OFF";
            this.mbutton_off.UseVisualStyleBackColor = false;
            this.mbutton_off.Click += new System.EventHandler(this.mbutton_off_Click);
            // 
            // mbutton_on
            // 
            this.mbutton_on.BackColor = System.Drawing.Color.LightGray;
            this.mbutton_on.Location = new System.Drawing.Point(7, 19);
            this.mbutton_on.Name = "mbutton_on";
            this.mbutton_on.Size = new System.Drawing.Size(38, 26);
            this.mbutton_on.TabIndex = 0;
            this.mbutton_on.Text = "ON";
            this.mbutton_on.UseVisualStyleBackColor = false;
            this.mbutton_on.Click += new System.EventHandler(this.mbutton_on_Click);
            // 
            // mtabPage_SerialProps
            // 
            this.mtabPage_SerialProps.AutoScroll = true;
            this.mtabPage_SerialProps.Controls.Add(this.mbutton_setPortName);
            this.mtabPage_SerialProps.Controls.Add(this.label7);
            this.mtabPage_SerialProps.Controls.Add(this.mbutton_loadMethods);
            this.mtabPage_SerialProps.Controls.Add(this.mbutton_saveMethod);
            this.mtabPage_SerialProps.Controls.Add(this.mtextbox_method);
            this.mtabPage_SerialProps.Controls.Add(this.mbutton_retrieve);
            this.mtabPage_SerialProps.Controls.Add(this.mlabel_communication);
            this.mtabPage_SerialProps.Controls.Add(this.mcomboBox_comPort);
            this.mtabPage_SerialProps.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_SerialProps.Name = "mtabPage_SerialProps";
            this.mtabPage_SerialProps.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_SerialProps.Size = new System.Drawing.Size(414, 551);
            this.mtabPage_SerialProps.TabIndex = 1;
            this.mtabPage_SerialProps.Text = "Advanced";
            this.mtabPage_SerialProps.UseVisualStyleBackColor = true;
            // 
            // mbutton_setPortName
            // 
            this.mbutton_setPortName.Location = new System.Drawing.Point(180, 14);
            this.mbutton_setPortName.Name = "mbutton_setPortName";
            this.mbutton_setPortName.Size = new System.Drawing.Size(69, 25);
            this.mbutton_setPortName.TabIndex = 9;
            this.mbutton_setPortName.Text = "Set";
            this.mbutton_setPortName.UseVisualStyleBackColor = true;
            this.mbutton_setPortName.Click += new System.EventHandler(this.mbutton_setPortName_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Method Editor";
            // 
            // mbutton_loadMethods
            // 
            this.mbutton_loadMethods.Location = new System.Drawing.Point(175, 45);
            this.mbutton_loadMethods.Name = "mbutton_loadMethods";
            this.mbutton_loadMethods.Size = new System.Drawing.Size(138, 27);
            this.mbutton_loadMethods.TabIndex = 7;
            this.mbutton_loadMethods.Text = "Load Methods From File";
            this.mbutton_loadMethods.UseVisualStyleBackColor = true;
            this.mbutton_loadMethods.Click += new System.EventHandler(this.mbutton_loadMethods_Click);
            // 
            // mbutton_saveMethod
            // 
            this.mbutton_saveMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_saveMethod.Location = new System.Drawing.Point(316, 489);
            this.mbutton_saveMethod.Name = "mbutton_saveMethod";
            this.mbutton_saveMethod.Size = new System.Drawing.Size(92, 27);
            this.mbutton_saveMethod.TabIndex = 6;
            this.mbutton_saveMethod.Text = "Save Method";
            this.mbutton_saveMethod.UseVisualStyleBackColor = true;
            this.mbutton_saveMethod.Click += new System.EventHandler(this.mbutton_saveMethod_Click);
            // 
            // mtextbox_method
            // 
            this.mtextbox_method.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextbox_method.Location = new System.Drawing.Point(10, 99);
            this.mtextbox_method.Name = "mtextbox_method";
            this.mtextbox_method.Size = new System.Drawing.Size(398, 384);
            this.mtextbox_method.TabIndex = 5;
            this.mtextbox_method.Text = "";
            // 
            // mbutton_retrieve
            // 
            this.mbutton_retrieve.Location = new System.Drawing.Point(9, 45);
            this.mbutton_retrieve.Name = "mbutton_retrieve";
            this.mbutton_retrieve.Size = new System.Drawing.Size(159, 27);
            this.mbutton_retrieve.TabIndex = 4;
            this.mbutton_retrieve.Text = "Retrieve Method From Pump";
            this.mbutton_retrieve.UseVisualStyleBackColor = true;
            this.mbutton_retrieve.Click += new System.EventHandler(this.mbutton_retrieve_Click);
            // 
            // mlabel_communication
            // 
            this.mlabel_communication.AutoSize = true;
            this.mlabel_communication.Location = new System.Drawing.Point(6, 16);
            this.mlabel_communication.Name = "mlabel_communication";
            this.mlabel_communication.Size = new System.Drawing.Size(53, 13);
            this.mlabel_communication.TabIndex = 3;
            this.mlabel_communication.Text = "COM Port";
            // 
            // mcomboBox_comPort
            // 
            this.mcomboBox_comPort.FormattingEnabled = true;
            this.mcomboBox_comPort.Location = new System.Drawing.Point(65, 13);
            this.mcomboBox_comPort.Name = "mcomboBox_comPort";
            this.mcomboBox_comPort.Size = new System.Drawing.Size(101, 21);
            this.mcomboBox_comPort.TabIndex = 2;
            this.mcomboBox_comPort.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_comPort_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mcontrol_pumpDisplay
            // 
            this.mcontrol_pumpDisplay.AutoScroll = true;
            this.mcontrol_pumpDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_pumpDisplay.Location = new System.Drawing.Point(422, 0);
            this.mcontrol_pumpDisplay.Name = "mcontrol_pumpDisplay";
            this.mcontrol_pumpDisplay.Size = new System.Drawing.Size(400, 577);
            this.mcontrol_pumpDisplay.TabIndex = 9;
            this.mcontrol_pumpDisplay.Tacked = false;
            // 
            // controlPumpAgilent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(822, 577);
            this.Controls.Add(this.mcontrol_pumpDisplay);
            this.Controls.Add(this.mtabControl);
            this.Name = "controlPumpAgilent";
            this.Size = new System.Drawing.Size(822, 577);
            this.mgroupBox_Flow.ResumeLayout(false);
            this.mgroupBox_Flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_flowRate)).EndInit();
            this.mgroupbox_Pressure.ResumeLayout(false);
            this.mgroupbox_Pressure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_mixerVolume)).EndInit();
            this.mGroupBox_Mode.ResumeLayout(false);
            this.mtabControl.ResumeLayout(false);
            this.mtabPage_PumpControls.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_percentB)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.mgroupBox_pumpsState.ResumeLayout(false);
            this.mtabPage_SerialProps.ResumeLayout(false);
            this.mtabPage_SerialProps.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_SetFlowRate;
        private System.Windows.Forms.GroupBox mgroupBox_Flow;
        private System.Windows.Forms.Label mlabel_ulmin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mtextBox_ActualFlowRate;
        private System.Windows.Forms.Button mbutton_GetFlowRate;
        private System.Windows.Forms.GroupBox mgroupbox_Pressure;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mtextBox_Pressure;
        private System.Windows.Forms.Button mbutton_GetPressure;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mtextBox_GetMixerVol;
        private System.Windows.Forms.Button mbutton_GetMixerVol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button mbutton_SetMixerVol;
        private System.Windows.Forms.GroupBox mGroupBox_Mode;
        private System.Windows.Forms.Button mbutton_SetMode;
        private System.Windows.Forms.ComboBox mcomboBox_Mode;
        private System.Windows.Forms.TabControl mtabControl;
        private System.Windows.Forms.TabPage mtabPage_PumpControls;
        private System.Windows.Forms.TabPage mtabPage_SerialProps;
        private System.Windows.Forms.GroupBox mgroupBox_pumpsState;
        private System.Windows.Forms.Button mbutton_on;
        private System.Windows.Forms.Button mbutton_off;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button mbutton_stop;
        private System.Windows.Forms.Button mbutton_start;
        private System.Windows.Forms.ComboBox mcomboBox_methods;
        private System.Windows.Forms.Label mlabel_communication;
        private System.Windows.Forms.ComboBox mcomboBox_comPort;
        private System.Windows.Forms.RichTextBox mtextbox_method;
        private System.Windows.Forms.Button mbutton_retrieve;
        private System.Windows.Forms.Button mbutton_saveMethod;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox mtextbox_percentB;
        private System.Windows.Forms.Button mbutton_getPercentB;
        private System.Windows.Forms.Button mbutton_setPercentB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown mnum_mixerVolume;
        private System.Windows.Forms.NumericUpDown mnum_flowRate;
        private System.Windows.Forms.NumericUpDown mnum_percentB;
        private System.Windows.Forms.Button mbutton_loadMethods;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timer1;
        private controlPumpDisplay mcontrol_pumpDisplay;
        private System.Windows.Forms.Button mbutton_purge;
        private System.Windows.Forms.Button mbutton_setPortName;
    }
}

