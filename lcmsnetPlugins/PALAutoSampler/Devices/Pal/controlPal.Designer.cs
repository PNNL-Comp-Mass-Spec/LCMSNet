//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//*********************************************************************************************************

namespace LcmsNet.Devices.Pal
{
    partial class controlPal
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
            this.mgroupBox_Methods = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mcomboBox_tray = new System.Windows.Forms.ComboBox();
            this.mbutton_stopMethod = new System.Windows.Forms.Button();
            this.mnum_volume = new System.Windows.Forms.NumericUpDown();
            this.mnum_vial = new System.Windows.Forms.NumericUpDown();
            this.mLabel_MicroLiters = new System.Windows.Forms.Label();
            this.mLabel_Volume = new System.Windows.Forms.Label();
            this.mLabel_Vial = new System.Windows.Forms.Label();
            this.mLabel_Tray = new System.Windows.Forms.Label();
            this.mButton_RefreshMethods = new System.Windows.Forms.Button();
            this.mButton_RunMethod = new System.Windows.Forms.Button();
            this.mcomboBox_MethodList = new System.Windows.Forms.ComboBox();
            this.mTextBox_Status = new System.Windows.Forms.TextBox();
            this.mButton_StatusRefresh = new System.Windows.Forms.Button();
            this.mGroupBox_Status = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mTabPage_Methods_Status = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mbutton_apply = new System.Windows.Forms.Button();
            this.mlabel_portName = new System.Windows.Forms.Label();
            this.mcombo_portNames = new System.Windows.Forms.ComboBox();
            this.mlabel_VialRange = new System.Windows.Forms.Label();
            this.mcomboBox_VialRange = new System.Windows.Forms.ComboBox();
            this.mgroupBox_Methods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_volume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_vial)).BeginInit();
            this.mGroupBox_Status.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.mTabPage_Methods_Status.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mgroupBox_Methods
            // 
            this.mgroupBox_Methods.Controls.Add(this.label1);
            this.mgroupBox_Methods.Controls.Add(this.mcomboBox_tray);
            this.mgroupBox_Methods.Controls.Add(this.mbutton_stopMethod);
            this.mgroupBox_Methods.Controls.Add(this.mnum_volume);
            this.mgroupBox_Methods.Controls.Add(this.mnum_vial);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_MicroLiters);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Volume);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Vial);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Tray);
            this.mgroupBox_Methods.Controls.Add(this.mButton_RefreshMethods);
            this.mgroupBox_Methods.Controls.Add(this.mButton_RunMethod);
            this.mgroupBox_Methods.Controls.Add(this.mcomboBox_MethodList);
            this.mgroupBox_Methods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mgroupBox_Methods.Location = new System.Drawing.Point(3, 3);
            this.mgroupBox_Methods.Name = "mgroupBox_Methods";
            this.mgroupBox_Methods.Size = new System.Drawing.Size(289, 267);
            this.mgroupBox_Methods.TabIndex = 4;
            this.mgroupBox_Methods.TabStop = false;
            this.mgroupBox_Methods.Text = "Methods";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Method";
            // 
            // mcomboBox_tray
            // 
            this.mcomboBox_tray.FormattingEnabled = true;
            this.mcomboBox_tray.Location = new System.Drawing.Point(51, 44);
            this.mcomboBox_tray.Name = "mcomboBox_tray";
            this.mcomboBox_tray.Size = new System.Drawing.Size(135, 21);
            this.mcomboBox_tray.TabIndex = 13;
            // 
            // mbutton_stopMethod
            // 
            this.mbutton_stopMethod.Location = new System.Drawing.Point(146, 139);
            this.mbutton_stopMethod.Name = "mbutton_stopMethod";
            this.mbutton_stopMethod.Size = new System.Drawing.Size(79, 23);
            this.mbutton_stopMethod.TabIndex = 12;
            this.mbutton_stopMethod.Text = "STOP!";
            this.mbutton_stopMethod.UseVisualStyleBackColor = true;
            this.mbutton_stopMethod.Click += new System.EventHandler(this.mbutton_stopMethod_Click);
            // 
            // mnum_volume
            // 
            this.mnum_volume.Location = new System.Drawing.Point(128, 102);
            this.mnum_volume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mnum_volume.Name = "mnum_volume";
            this.mnum_volume.Size = new System.Drawing.Size(33, 20);
            this.mnum_volume.TabIndex = 11;
            // 
            // mnum_vial
            // 
            this.mnum_vial.Location = new System.Drawing.Point(51, 102);
            this.mnum_vial.Maximum = new decimal(new int[] {
            96,
            0,
            0,
            0});
            this.mnum_vial.Name = "mnum_vial";
            this.mnum_vial.Size = new System.Drawing.Size(33, 20);
            this.mnum_vial.TabIndex = 10;
            // 
            // mLabel_MicroLiters
            // 
            this.mLabel_MicroLiters.AutoSize = true;
            this.mLabel_MicroLiters.Location = new System.Drawing.Point(167, 104);
            this.mLabel_MicroLiters.Name = "mLabel_MicroLiters";
            this.mLabel_MicroLiters.Size = new System.Drawing.Size(19, 13);
            this.mLabel_MicroLiters.TabIndex = 9;
            this.mLabel_MicroLiters.Text = "uL";
            // 
            // mLabel_Volume
            // 
            this.mLabel_Volume.AutoSize = true;
            this.mLabel_Volume.Location = new System.Drawing.Point(88, 104);
            this.mLabel_Volume.Name = "mLabel_Volume";
            this.mLabel_Volume.Size = new System.Drawing.Size(42, 13);
            this.mLabel_Volume.TabIndex = 8;
            this.mLabel_Volume.Text = "Volume";
            // 
            // mLabel_Vial
            // 
            this.mLabel_Vial.AutoSize = true;
            this.mLabel_Vial.Location = new System.Drawing.Point(6, 104);
            this.mLabel_Vial.Name = "mLabel_Vial";
            this.mLabel_Vial.Size = new System.Drawing.Size(24, 13);
            this.mLabel_Vial.TabIndex = 7;
            this.mLabel_Vial.Text = "Vial";
            // 
            // mLabel_Tray
            // 
            this.mLabel_Tray.AutoSize = true;
            this.mLabel_Tray.Location = new System.Drawing.Point(6, 47);
            this.mLabel_Tray.Name = "mLabel_Tray";
            this.mLabel_Tray.Size = new System.Drawing.Size(28, 13);
            this.mLabel_Tray.TabIndex = 6;
            this.mLabel_Tray.Text = "Tray";
            // 
            // mButton_RefreshMethods
            // 
            this.mButton_RefreshMethods.Location = new System.Drawing.Point(107, 71);
            this.mButton_RefreshMethods.Name = "mButton_RefreshMethods";
            this.mButton_RefreshMethods.Size = new System.Drawing.Size(79, 23);
            this.mButton_RefreshMethods.TabIndex = 2;
            this.mButton_RefreshMethods.Text = "Refresh";
            this.mButton_RefreshMethods.UseVisualStyleBackColor = true;
            this.mButton_RefreshMethods.Click += new System.EventHandler(this.mButton_RefreshMethods_Click);
            // 
            // mButton_RunMethod
            // 
            this.mButton_RunMethod.Location = new System.Drawing.Point(11, 139);
            this.mButton_RunMethod.Name = "mButton_RunMethod";
            this.mButton_RunMethod.Size = new System.Drawing.Size(129, 23);
            this.mButton_RunMethod.TabIndex = 1;
            this.mButton_RunMethod.Text = "Run/Continue Method";
            this.mButton_RunMethod.UseVisualStyleBackColor = true;
            this.mButton_RunMethod.Click += new System.EventHandler(this.mButton_RunMethod_Click);
            // 
            // mcomboBox_MethodList
            // 
            this.mcomboBox_MethodList.FormattingEnabled = true;
            this.mcomboBox_MethodList.Location = new System.Drawing.Point(51, 20);
            this.mcomboBox_MethodList.Name = "mcomboBox_MethodList";
            this.mcomboBox_MethodList.Size = new System.Drawing.Size(135, 21);
            this.mcomboBox_MethodList.TabIndex = 0;
            // 
            // mTextBox_Status
            // 
            this.mTextBox_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTextBox_Status.Location = new System.Drawing.Point(3, 16);
            this.mTextBox_Status.Name = "mTextBox_Status";
            this.mTextBox_Status.ReadOnly = true;
            this.mTextBox_Status.Size = new System.Drawing.Size(283, 20);
            this.mTextBox_Status.TabIndex = 5;
            // 
            // mButton_StatusRefresh
            // 
            this.mButton_StatusRefresh.Location = new System.Drawing.Point(91, 39);
            this.mButton_StatusRefresh.Name = "mButton_StatusRefresh";
            this.mButton_StatusRefresh.Size = new System.Drawing.Size(81, 23);
            this.mButton_StatusRefresh.TabIndex = 6;
            this.mButton_StatusRefresh.Text = "Refresh";
            this.mButton_StatusRefresh.UseVisualStyleBackColor = true;
            this.mButton_StatusRefresh.Click += new System.EventHandler(this.mButton_StatusRefresh_Click);
            // 
            // mGroupBox_Status
            // 
            this.mGroupBox_Status.Controls.Add(this.mButton_StatusRefresh);
            this.mGroupBox_Status.Controls.Add(this.mTextBox_Status);
            this.mGroupBox_Status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mGroupBox_Status.Location = new System.Drawing.Point(3, 270);
            this.mGroupBox_Status.Name = "mGroupBox_Status";
            this.mGroupBox_Status.Size = new System.Drawing.Size(289, 68);
            this.mGroupBox_Status.TabIndex = 7;
            this.mGroupBox_Status.TabStop = false;
            this.mGroupBox_Status.Text = "Status";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.mTabPage_Methods_Status);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(303, 367);
            this.tabControl1.TabIndex = 8;
            // 
            // mTabPage_Methods_Status
            // 
            this.mTabPage_Methods_Status.Controls.Add(this.mgroupBox_Methods);
            this.mTabPage_Methods_Status.Controls.Add(this.mGroupBox_Status);
            this.mTabPage_Methods_Status.Location = new System.Drawing.Point(4, 22);
            this.mTabPage_Methods_Status.Name = "mTabPage_Methods_Status";
            this.mTabPage_Methods_Status.Padding = new System.Windows.Forms.Padding(3);
            this.mTabPage_Methods_Status.Size = new System.Drawing.Size(295, 341);
            this.mTabPage_Methods_Status.TabIndex = 0;
            this.mTabPage_Methods_Status.Text = "Methods/Status";
            this.mTabPage_Methods_Status.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.mbutton_apply);
            this.tabPage1.Controls.Add(this.mlabel_portName);
            this.tabPage1.Controls.Add(this.mcombo_portNames);
            this.tabPage1.Controls.Add(this.mlabel_VialRange);
            this.tabPage1.Controls.Add(this.mcomboBox_VialRange);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(295, 261);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Advanced";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mbutton_apply
            // 
            this.mbutton_apply.Location = new System.Drawing.Point(214, 44);
            this.mbutton_apply.Name = "mbutton_apply";
            this.mbutton_apply.Size = new System.Drawing.Size(75, 21);
            this.mbutton_apply.TabIndex = 4;
            this.mbutton_apply.Text = "Apply";
            this.mbutton_apply.UseVisualStyleBackColor = true;
            this.mbutton_apply.Click += new System.EventHandler(this.mbutton_apply_Click);
            // 
            // mlabel_portName
            // 
            this.mlabel_portName.AutoSize = true;
            this.mlabel_portName.Location = new System.Drawing.Point(13, 47);
            this.mlabel_portName.Name = "mlabel_portName";
            this.mlabel_portName.Size = new System.Drawing.Size(57, 13);
            this.mlabel_portName.TabIndex = 3;
            this.mlabel_portName.Text = "Port Name";
            // 
            // mcombo_portNames
            // 
            this.mcombo_portNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcombo_portNames.FormattingEnabled = true;
            this.mcombo_portNames.Location = new System.Drawing.Point(88, 44);
            this.mcombo_portNames.Name = "mcombo_portNames";
            this.mcombo_portNames.Size = new System.Drawing.Size(121, 21);
            this.mcombo_portNames.TabIndex = 2;
            this.mcombo_portNames.SelectedIndexChanged += new System.EventHandler(this.mcombo_portNames_SelectedIndexChanged);
            // 
            // mlabel_VialRange
            // 
            this.mlabel_VialRange.AutoSize = true;
            this.mlabel_VialRange.Location = new System.Drawing.Point(13, 9);
            this.mlabel_VialRange.Name = "mlabel_VialRange";
            this.mlabel_VialRange.Size = new System.Drawing.Size(59, 13);
            this.mlabel_VialRange.TabIndex = 1;
            this.mlabel_VialRange.Text = "Vial Range";
            // 
            // mcomboBox_VialRange
            // 
            this.mcomboBox_VialRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_VialRange.FormattingEnabled = true;
            this.mcomboBox_VialRange.Location = new System.Drawing.Point(88, 6);
            this.mcomboBox_VialRange.Name = "mcomboBox_VialRange";
            this.mcomboBox_VialRange.Size = new System.Drawing.Size(121, 21);
            this.mcomboBox_VialRange.TabIndex = 0;
            this.mcomboBox_VialRange.SelectionChangeCommitted += new System.EventHandler(this.mcomboBox_VialRange_SelectionChangeCommitted);
            // 
            // controlPal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "controlPal";
            this.Size = new System.Drawing.Size(303, 367);
            this.mgroupBox_Methods.ResumeLayout(false);
            this.mgroupBox_Methods.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_volume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_vial)).EndInit();
            this.mGroupBox_Status.ResumeLayout(false);
            this.mGroupBox_Status.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.mTabPage_Methods_Status.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox mgroupBox_Methods;
        private System.Windows.Forms.Button mButton_RunMethod;
        private System.Windows.Forms.ComboBox mcomboBox_MethodList;
        private System.Windows.Forms.Button mButton_RefreshMethods;
        private System.Windows.Forms.Label mLabel_Volume;
        private System.Windows.Forms.Label mLabel_Vial;
        private System.Windows.Forms.Label mLabel_Tray;
        private System.Windows.Forms.Label mLabel_MicroLiters;
        private System.Windows.Forms.TextBox mTextBox_Status;
        private System.Windows.Forms.Button mButton_StatusRefresh;
        private System.Windows.Forms.GroupBox mGroupBox_Status;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage mTabPage_Methods_Status;
        private System.Windows.Forms.NumericUpDown mnum_vial;
        private System.Windows.Forms.NumericUpDown mnum_volume;
        private System.Windows.Forms.Button mbutton_stopMethod;
        private System.Windows.Forms.ComboBox mcomboBox_tray;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox mcomboBox_VialRange;
        private System.Windows.Forms.Label mlabel_VialRange;
        private System.Windows.Forms.Label mlabel_portName;
        private System.Windows.Forms.ComboBox mcombo_portNames;
        private System.Windows.Forms.Button mbutton_apply;
    }
}
