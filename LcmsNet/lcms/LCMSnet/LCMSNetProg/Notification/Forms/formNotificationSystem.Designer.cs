using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Notification.Forms
{
    partial class formNotificationSystem
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(formNotificationSystem));
            this.mnotify_icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mlistview_devices = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mcomboBox_actions = new System.Windows.Forms.ComboBox();
            this.mlabel_action = new System.Windows.Forms.Label();
            this.mnum_minimum = new System.Windows.Forms.NumericUpDown();
            this.mnum_maximum = new System.Windows.Forms.NumericUpDown();
            this.mgroupBox_number = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mgroupBox_text = new System.Windows.Forms.GroupBox();
            this.mtextBox_statusText = new System.Windows.Forms.TextBox();
            this.mradioButton_text = new System.Windows.Forms.RadioButton();
            this.mradioButton_number = new System.Windows.Forms.RadioButton();
            this.mradioButton_happens = new System.Windows.Forms.RadioButton();
            this.mcomboBox_methods = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mgroupBox_actions = new System.Windows.Forms.GroupBox();
            this.mlabel_status = new System.Windows.Forms.Label();
            this.mtimer_notifier = new System.Windows.Forms.Timer(this.components);
            this.mlistBox_events = new System.Windows.Forms.ListBox();
            this.mlistbox_assignedEvents = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mbutton_ignoreAll = new System.Windows.Forms.Button();
            this.mbutton_save = new System.Windows.Forms.Button();
            this.mgroupBox_conditions = new System.Windows.Forms.GroupBox();
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.mlabel_enabled = new System.Windows.Forms.Label();
            this.mlabel_setting = new System.Windows.Forms.Label();
            this.mbutton_ignoreThisSetting = new System.Windows.Forms.Button();
            this.mlabel_device = new System.Windows.Forms.Label();
            this.mbutton_disable = new System.Windows.Forms.Button();
            this.mbutton_enable = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mgroupBox_status = new System.Windows.Forms.GroupBox();
            this.mtextBox_path = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mnum_statusMinutes = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_writeStatus = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maximum)).BeginInit();
            this.mgroupBox_number.SuspendLayout();
            this.mgroupBox_text.SuspendLayout();
            this.mgroupBox_actions.SuspendLayout();
            this.mgroupBox_conditions.SuspendLayout();
            this.settingsPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.mgroupBox_status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_statusMinutes)).BeginInit();
            this.SuspendLayout();
            //
            // mlistview_devices
            //
            this.mlistview_devices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mlistview_devices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.mlistview_devices.FullRowSelect = true;
            this.mlistview_devices.GridLines = true;
            this.mlistview_devices.Location = new System.Drawing.Point(6, 4);
            this.mlistview_devices.Name = "mlistview_devices";
            this.mlistview_devices.Size = new System.Drawing.Size(209, 639);
            this.mlistview_devices.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mlistview_devices.TabIndex = 5;
            this.mlistview_devices.UseCompatibleStateImageBehavior = false;
            this.mlistview_devices.View = System.Windows.Forms.View.Details;
            this.mlistview_devices.SelectedIndexChanged += new System.EventHandler(this.mlistview_devices_SelectedIndexChanged);
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "Notifiers";
            this.columnHeader1.Width = 205;
            //
            // mcomboBox_actions
            //
            this.mcomboBox_actions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_actions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_actions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_actions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcomboBox_actions.FormattingEnabled = true;
            this.mcomboBox_actions.Location = new System.Drawing.Point(186, 15);
            this.mcomboBox_actions.Name = "mcomboBox_actions";
            this.mcomboBox_actions.Size = new System.Drawing.Size(207, 21);
            this.mcomboBox_actions.Sorted = true;
            this.mcomboBox_actions.TabIndex = 6;
            this.mcomboBox_actions.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_actions_SelectedIndexChanged);
            //
            // mlabel_action
            //
            this.mlabel_action.AutoSize = true;
            this.mlabel_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_action.Location = new System.Drawing.Point(24, 18);
            this.mlabel_action.Name = "mlabel_action";
            this.mlabel_action.Size = new System.Drawing.Size(156, 13);
            this.mlabel_action.TabIndex = 7;
            this.mlabel_action.Text = "Then the system should do this:";
            //
            // mnum_minimum
            //
            this.mnum_minimum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_minimum.DecimalPlaces = 2;
            this.mnum_minimum.Location = new System.Drawing.Point(96, 32);
            this.mnum_minimum.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.mnum_minimum.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.mnum_minimum.Name = "mnum_minimum";
            this.mnum_minimum.Size = new System.Drawing.Size(183, 20);
            this.mnum_minimum.TabIndex = 9;
            this.mnum_minimum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_minimum.ValueChanged += new System.EventHandler(this.mnum_minimum_ValueChanged);
            //
            // mnum_maximum
            //
            this.mnum_maximum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_maximum.DecimalPlaces = 2;
            this.mnum_maximum.Location = new System.Drawing.Point(96, 58);
            this.mnum_maximum.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.mnum_maximum.Name = "mnum_maximum";
            this.mnum_maximum.Size = new System.Drawing.Size(183, 20);
            this.mnum_maximum.TabIndex = 10;
            this.mnum_maximum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_maximum.ValueChanged += new System.EventHandler(this.mnum_maximum_ValueChanged);
            //
            // mgroupBox_number
            //
            this.mgroupBox_number.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_number.Controls.Add(this.label2);
            this.mgroupBox_number.Controls.Add(this.label1);
            this.mgroupBox_number.Controls.Add(this.mnum_minimum);
            this.mgroupBox_number.Controls.Add(this.mnum_maximum);
            this.mgroupBox_number.Enabled = false;
            this.mgroupBox_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_number.Location = new System.Drawing.Point(94, 21);
            this.mgroupBox_number.Name = "mgroupBox_number";
            this.mgroupBox_number.Size = new System.Drawing.Size(279, 87);
            this.mgroupBox_number.TabIndex = 11;
            this.mgroupBox_number.TabStop = false;
            this.mgroupBox_number.Text = "Goes outside";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Maximum (<)";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Minimum (>)";
            //
            // mgroupBox_text
            //
            this.mgroupBox_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_text.Controls.Add(this.mtextBox_statusText);
            this.mgroupBox_text.Enabled = false;
            this.mgroupBox_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_text.Location = new System.Drawing.Point(94, 138);
            this.mgroupBox_text.Name = "mgroupBox_text";
            this.mgroupBox_text.Size = new System.Drawing.Size(297, 62);
            this.mgroupBox_text.TabIndex = 12;
            this.mgroupBox_text.TabStop = false;
            this.mgroupBox_text.Text = "Equal To";
            //
            // mtextBox_statusText
            //
            this.mtextBox_statusText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_statusText.Location = new System.Drawing.Point(6, 28);
            this.mtextBox_statusText.Name = "mtextBox_statusText";
            this.mtextBox_statusText.Size = new System.Drawing.Size(285, 20);
            this.mtextBox_statusText.TabIndex = 0;
            this.mtextBox_statusText.TextChanged += new System.EventHandler(this.mtextBox_statusText_TextChanged);
            //
            // mradioButton_text
            //
            this.mradioButton_text.AutoSize = true;
            this.mradioButton_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_text.Location = new System.Drawing.Point(17, 138);
            this.mradioButton_text.Name = "mradioButton_text";
            this.mradioButton_text.Size = new System.Drawing.Size(55, 17);
            this.mradioButton_text.TabIndex = 14;
            this.mradioButton_text.Text = "If Text";
            this.mradioButton_text.UseVisualStyleBackColor = true;
            this.mradioButton_text.CheckedChanged += new System.EventHandler(this.mradioButton_text_CheckedChanged);
            //
            // mradioButton_number
            //
            this.mradioButton_number.AutoSize = true;
            this.mradioButton_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_number.Location = new System.Drawing.Point(17, 19);
            this.mradioButton_number.Name = "mradioButton_number";
            this.mradioButton_number.Size = new System.Drawing.Size(71, 17);
            this.mradioButton_number.TabIndex = 15;
            this.mradioButton_number.Text = "If Number";
            this.mradioButton_number.UseVisualStyleBackColor = true;
            this.mradioButton_number.CheckedChanged += new System.EventHandler(this.mradioButton_number_CheckedChanged);
            //
            // mradioButton_happens
            //
            this.mradioButton_happens.AutoSize = true;
            this.mradioButton_happens.Checked = true;
            this.mradioButton_happens.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_happens.Location = new System.Drawing.Point(17, 229);
            this.mradioButton_happens.Name = "mradioButton_happens";
            this.mradioButton_happens.Size = new System.Drawing.Size(173, 17);
            this.mradioButton_happens.TabIndex = 16;
            this.mradioButton_happens.TabStop = true;
            this.mradioButton_happens.Text = "If the event simply just happens";
            this.mradioButton_happens.UseVisualStyleBackColor = true;
            this.mradioButton_happens.CheckedChanged += new System.EventHandler(this.mradioButton_happens_CheckedChanged);
            //
            // mcomboBox_methods
            //
            this.mcomboBox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_methods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_methods.Enabled = false;
            this.mcomboBox_methods.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_methods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcomboBox_methods.FormattingEnabled = true;
            this.mcomboBox_methods.Location = new System.Drawing.Point(186, 42);
            this.mcomboBox_methods.Name = "mcomboBox_methods";
            this.mcomboBox_methods.Size = new System.Drawing.Size(210, 21);
            this.mcomboBox_methods.Sorted = true;
            this.mcomboBox_methods.TabIndex = 17;
            this.mcomboBox_methods.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_methods_SelectedIndexChanged);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(121, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "LC-Method";
            //
            // mgroupBox_actions
            //
            this.mgroupBox_actions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_actions.Controls.Add(this.mradioButton_number);
            this.mgroupBox_actions.Controls.Add(this.mradioButton_happens);
            this.mgroupBox_actions.Controls.Add(this.mgroupBox_number);
            this.mgroupBox_actions.Controls.Add(this.mgroupBox_text);
            this.mgroupBox_actions.Controls.Add(this.mradioButton_text);
            this.mgroupBox_actions.Enabled = false;
            this.mgroupBox_actions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_actions.Location = new System.Drawing.Point(473, 112);
            this.mgroupBox_actions.Name = "mgroupBox_actions";
            this.mgroupBox_actions.Size = new System.Drawing.Size(400, 259);
            this.mgroupBox_actions.TabIndex = 19;
            this.mgroupBox_actions.TabStop = false;
            this.mgroupBox_actions.Text = "Conditions ";
            //
            // mlabel_status
            //
            this.mlabel_status.AutoSize = true;
            this.mlabel_status.Location = new System.Drawing.Point(218, 43);
            this.mlabel_status.Name = "mlabel_status";
            this.mlabel_status.Size = new System.Drawing.Size(99, 13);
            this.mlabel_status.TabIndex = 20;
            this.mlabel_status.Text = "Unassigned Events";
            //
            // mtimer_notifier
            //
            this.mtimer_notifier.Enabled = true;
            this.mtimer_notifier.Interval = 20000;
            this.mtimer_notifier.Tick += new System.EventHandler(this.mtimer_notifier_Tick);
            //
            // mlistBox_events
            //
            this.mlistBox_events.FormattingEnabled = true;
            this.mlistBox_events.HorizontalScrollbar = true;
            this.mlistBox_events.Location = new System.Drawing.Point(221, 59);
            this.mlistBox_events.Name = "mlistBox_events";
            this.mlistBox_events.Size = new System.Drawing.Size(246, 290);
            this.mlistBox_events.Sorted = true;
            this.mlistBox_events.TabIndex = 22;
            this.mlistBox_events.Click += new System.EventHandler(this.mlistBox_events_Click);
            this.mlistBox_events.SelectedIndexChanged += new System.EventHandler(this.mlistBox_events_SelectedIndexChanged);
            //
            // mlistbox_assignedEvents
            //
            this.mlistbox_assignedEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mlistbox_assignedEvents.FormattingEnabled = true;
            this.mlistbox_assignedEvents.HorizontalScrollbar = true;
            this.mlistbox_assignedEvents.Location = new System.Drawing.Point(221, 394);
            this.mlistbox_assignedEvents.Name = "mlistbox_assignedEvents";
            this.mlistbox_assignedEvents.Size = new System.Drawing.Size(246, 212);
            this.mlistbox_assignedEvents.Sorted = true;
            this.mlistbox_assignedEvents.TabIndex = 23;
            this.mlistbox_assignedEvents.SelectedIndexChanged += new System.EventHandler(this.mlistbox_assignedEvents_SelectedIndexChanged);
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(221, 371);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Events with Actions";
            //
            // mbutton_ignoreAll
            //
            this.mbutton_ignoreAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_ignoreAll.Location = new System.Drawing.Point(221, 608);
            this.mbutton_ignoreAll.Name = "mbutton_ignoreAll";
            this.mbutton_ignoreAll.Size = new System.Drawing.Size(246, 35);
            this.mbutton_ignoreAll.TabIndex = 25;
            this.mbutton_ignoreAll.Text = "Ignore All";
            this.mbutton_ignoreAll.UseVisualStyleBackColor = true;
            this.mbutton_ignoreAll.Click += new System.EventHandler(this.mbutton_ignoreAll_Click);
            //
            // mbutton_save
            //
            this.mbutton_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_save.Image = global::LcmsNet.Properties.Resources.Save;
            this.mbutton_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_save.Location = new System.Drawing.Point(837, 714);
            this.mbutton_save.Name = "mbutton_save";
            this.mbutton_save.Size = new System.Drawing.Size(79, 35);
            this.mbutton_save.TabIndex = 26;
            this.mbutton_save.Text = "Save";
            this.mbutton_save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_save.UseVisualStyleBackColor = true;
            this.mbutton_save.Click += new System.EventHandler(this.mbutton_save_Click);
            //
            // mgroupBox_conditions
            //
            this.mgroupBox_conditions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_conditions.Controls.Add(this.mcomboBox_methods);
            this.mgroupBox_conditions.Controls.Add(this.mcomboBox_actions);
            this.mgroupBox_conditions.Controls.Add(this.mlabel_action);
            this.mgroupBox_conditions.Controls.Add(this.label4);
            this.mgroupBox_conditions.Enabled = false;
            this.mgroupBox_conditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_conditions.Location = new System.Drawing.Point(473, 377);
            this.mgroupBox_conditions.Name = "mgroupBox_conditions";
            this.mgroupBox_conditions.Size = new System.Drawing.Size(400, 81);
            this.mgroupBox_conditions.TabIndex = 27;
            this.mgroupBox_conditions.TabStop = false;
            this.mgroupBox_conditions.Text = "Actions To Take";
            //
            // settingsPanel
            //
            this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanel.Controls.Add(this.mlabel_enabled);
            this.settingsPanel.Controls.Add(this.mlabel_setting);
            this.settingsPanel.Controls.Add(this.mbutton_ignoreThisSetting);
            this.settingsPanel.Controls.Add(this.mlabel_device);
            this.settingsPanel.Controls.Add(this.mgroupBox_conditions);
            this.settingsPanel.Controls.Add(this.mlistview_devices);
            this.settingsPanel.Controls.Add(this.mgroupBox_actions);
            this.settingsPanel.Controls.Add(this.mbutton_ignoreAll);
            this.settingsPanel.Controls.Add(this.mlabel_status);
            this.settingsPanel.Controls.Add(this.label3);
            this.settingsPanel.Controls.Add(this.mlistBox_events);
            this.settingsPanel.Controls.Add(this.mlistbox_assignedEvents);
            this.settingsPanel.Location = new System.Drawing.Point(6, 6);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(888, 652);
            this.settingsPanel.TabIndex = 28;
            //
            // mlabel_enabled
            //
            this.mlabel_enabled.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mlabel_enabled.BackColor = System.Drawing.Color.Transparent;
            this.mlabel_enabled.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlabel_enabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_enabled.Location = new System.Drawing.Point(267, 250);
            this.mlabel_enabled.Name = "mlabel_enabled";
            this.mlabel_enabled.Size = new System.Drawing.Size(365, 156);
            this.mlabel_enabled.TabIndex = 28;
            this.mlabel_enabled.Text = "Disabled";
            this.mlabel_enabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // mlabel_setting
            //
            this.mlabel_setting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_setting.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_setting.Location = new System.Drawing.Point(473, 59);
            this.mlabel_setting.Name = "mlabel_setting";
            this.mlabel_setting.Size = new System.Drawing.Size(400, 31);
            this.mlabel_setting.TabIndex = 31;
            this.mlabel_setting.Text = "Setting:";
            this.mlabel_setting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // mbutton_ignoreThisSetting
            //
            this.mbutton_ignoreThisSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ignoreThisSetting.Location = new System.Drawing.Point(727, 464);
            this.mbutton_ignoreThisSetting.Name = "mbutton_ignoreThisSetting";
            this.mbutton_ignoreThisSetting.Size = new System.Drawing.Size(146, 35);
            this.mbutton_ignoreThisSetting.TabIndex = 30;
            this.mbutton_ignoreThisSetting.Text = "Ignore This Setting";
            this.mbutton_ignoreThisSetting.UseVisualStyleBackColor = true;
            this.mbutton_ignoreThisSetting.Click += new System.EventHandler(this.mbutton_ignoreThisSetting_Click);
            //
            // mlabel_device
            //
            this.mlabel_device.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_device.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_device.Location = new System.Drawing.Point(219, 4);
            this.mlabel_device.Name = "mlabel_device";
            this.mlabel_device.Size = new System.Drawing.Size(654, 29);
            this.mlabel_device.TabIndex = 29;
            this.mlabel_device.Text = "Device:";
            //
            // mbutton_disable
            //
            this.mbutton_disable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_disable.Image = ((System.Drawing.Image)(resources.GetObject("mbutton_disable.Image")));
            this.mbutton_disable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_disable.Location = new System.Drawing.Point(9, 714);
            this.mbutton_disable.Name = "mbutton_disable";
            this.mbutton_disable.Size = new System.Drawing.Size(79, 35);
            this.mbutton_disable.TabIndex = 29;
            this.mbutton_disable.Text = "Disable";
            this.mbutton_disable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_disable.UseVisualStyleBackColor = true;
            this.mbutton_disable.Click += new System.EventHandler(this.mbutton_disable_Click);
            //
            // mbutton_enable
            //
            this.mbutton_enable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_enable.Image = global::LcmsNet.Properties.Resources.unlock;
            this.mbutton_enable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_enable.Location = new System.Drawing.Point(94, 714);
            this.mbutton_enable.Name = "mbutton_enable";
            this.mbutton_enable.Size = new System.Drawing.Size(79, 35);
            this.mbutton_enable.TabIndex = 30;
            this.mbutton_enable.Text = "Enable";
            this.mbutton_enable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_enable.UseVisualStyleBackColor = true;
            this.mbutton_enable.Click += new System.EventHandler(this.mbutton_enable_Click);
            //
            // tabControl1
            //
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(9, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(908, 690);
            this.tabControl1.TabIndex = 31;
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.settingsPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(900, 664);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Notification Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            //
            // tabPage2
            //
            this.tabPage2.Controls.Add(this.mgroupBox_status);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(900, 664);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Remote Monitoring";
            this.tabPage2.UseVisualStyleBackColor = true;
            //
            // mgroupBox_status
            //
            this.mgroupBox_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_status.Controls.Add(this.mtextBox_path);
            this.mgroupBox_status.Controls.Add(this.label5);
            this.mgroupBox_status.Controls.Add(this.mnum_statusMinutes);
            this.mgroupBox_status.Controls.Add(this.mcheckBox_writeStatus);
            this.mgroupBox_status.Location = new System.Drawing.Point(3, 6);
            this.mgroupBox_status.Name = "mgroupBox_status";
            this.mgroupBox_status.Size = new System.Drawing.Size(891, 81);
            this.mgroupBox_status.TabIndex = 22;
            this.mgroupBox_status.TabStop = false;
            this.mgroupBox_status.Text = "Remote Status Monitoring";
            //
            // mtextBox_path
            //
            this.mtextBox_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_path.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.mtextBox_path.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.mtextBox_path.Location = new System.Drawing.Point(49, 50);
            this.mtextBox_path.Name = "mtextBox_path";
            this.mtextBox_path.Size = new System.Drawing.Size(830, 20);
            this.mtextBox_path.TabIndex = 9;
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(301, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "minutes to the directory:";
            //
            // mnum_statusMinutes
            //
            this.mnum_statusMinutes.Location = new System.Drawing.Point(229, 24);
            this.mnum_statusMinutes.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.mnum_statusMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_statusMinutes.Name = "mnum_statusMinutes";
            this.mnum_statusMinutes.Size = new System.Drawing.Size(66, 20);
            this.mnum_statusMinutes.TabIndex = 1;
            this.mnum_statusMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_statusMinutes.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mnum_statusMinutes.ValueChanged += new System.EventHandler(this.mnum_statusMinutes_ValueChanged);
            //
            // mcheckBox_writeStatus
            //
            this.mcheckBox_writeStatus.AutoSize = true;
            this.mcheckBox_writeStatus.Location = new System.Drawing.Point(24, 25);
            this.mcheckBox_writeStatus.Name = "mcheckBox_writeStatus";
            this.mcheckBox_writeStatus.Size = new System.Drawing.Size(199, 17);
            this.mcheckBox_writeStatus.TabIndex = 0;
            this.mcheckBox_writeStatus.Text = "Write Instrument Health Status every";
            this.mcheckBox_writeStatus.UseVisualStyleBackColor = true;
            this.mcheckBox_writeStatus.CheckedChanged += new System.EventHandler(this.mcheckBox_writeStatus_CheckedChanged);
            //
            // formNotificationSystem
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(922, 750);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mbutton_enable);
            this.Controls.Add(this.mbutton_disable);
            this.Controls.Add(this.mbutton_save);
            this.Name = "formNotificationSystem";
            this.Text = "Notification System";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maximum)).EndInit();
            this.mgroupBox_number.ResumeLayout(false);
            this.mgroupBox_number.PerformLayout();
            this.mgroupBox_text.ResumeLayout(false);
            this.mgroupBox_text.PerformLayout();
            this.mgroupBox_actions.ResumeLayout(false);
            this.mgroupBox_actions.PerformLayout();
            this.mgroupBox_conditions.ResumeLayout(false);
            this.mgroupBox_conditions.PerformLayout();
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.mgroupBox_status.ResumeLayout(false);
            this.mgroupBox_status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_statusMinutes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NotifyIcon mnotify_icon;
        private ListView mlistview_devices;
        private ComboBox mcomboBox_actions;
        private Label mlabel_action;
        private NumericUpDown mnum_minimum;
        private NumericUpDown mnum_maximum;
        private GroupBox mgroupBox_number;
        private GroupBox mgroupBox_text;
        private Label label2;
        private Label label1;
        private TextBox mtextBox_statusText;
        private RadioButton mradioButton_text;
        private RadioButton mradioButton_number;
        private RadioButton mradioButton_happens;
        private ComboBox mcomboBox_methods;
        private Label label4;
        private GroupBox mgroupBox_actions;
        private Label mlabel_status;
        private Timer mtimer_notifier;
        private ColumnHeader columnHeader1;
        private ListBox mlistBox_events;
        private ListBox mlistbox_assignedEvents;
        private Label label3;
        private Button mbutton_ignoreAll;
        private Button mbutton_save;
        private GroupBox mgroupBox_conditions;
        private Panel settingsPanel;
        private Button mbutton_disable;
        private Button mbutton_enable;
        private Label mlabel_enabled;
        private Label mlabel_device;
        private Button mbutton_ignoreThisSetting;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox mgroupBox_status;
        private TextBox mtextBox_path;
        private Label label5;
        private NumericUpDown mnum_statusMinutes;
        private CheckBox mcheckBox_writeStatus;
        private Label mlabel_setting;
    }
}