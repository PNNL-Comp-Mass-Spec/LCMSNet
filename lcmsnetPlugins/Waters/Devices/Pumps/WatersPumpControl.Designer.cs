namespace Waters.Devices.Pumps
{
    partial class WatersPumpControl
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
            this.m_buttonLaunchConsole = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.m_statusLabel = new System.Windows.Forms.Label();
            this.mcontrol_pumpDisplay = new LcmsNetDataClasses.Devices.Pumps.controlPumpDisplay();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mbutton_stopMethod = new System.Windows.Forms.Button();
            this.mbutton_getListOfMethods = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mcomboBox_methods = new System.Windows.Forms.ComboBox();
            this.mbutton_startMethod = new System.Windows.Forms.Button();
            this.mnum_methodLength = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mbutton_setSystemName = new System.Windows.Forms.Button();
            this.mbutton_setInstrument = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mtext_systemName = new System.Windows.Forms.TextBox();
            this.mbutton_scanInstruments = new System.Windows.Forms.Button();
            this.mtext_computerName = new System.Windows.Forms.TextBox();
            this.mbutton_setComputerName = new System.Windows.Forms.Button();
            this.mlist_instruments = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_methodLength)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_buttonLaunchConsole
            // 
            this.m_buttonLaunchConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonLaunchConsole.Location = new System.Drawing.Point(13, 19);
            this.m_buttonLaunchConsole.Name = "m_buttonLaunchConsole";
            this.m_buttonLaunchConsole.Size = new System.Drawing.Size(299, 32);
            this.m_buttonLaunchConsole.TabIndex = 1;
            this.m_buttonLaunchConsole.Text = "Waters Console";
            this.m_buttonLaunchConsole.UseVisualStyleBackColor = true;
            this.m_buttonLaunchConsole.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 265);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.m_statusLabel);
            this.tabPage3.Controls.Add(this.mcontrol_pumpDisplay);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(328, 239);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Status";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // m_statusLabel
            // 
            this.m_statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_statusLabel.Location = new System.Drawing.Point(3, 10);
            this.m_statusLabel.Name = "m_statusLabel";
            this.m_statusLabel.Size = new System.Drawing.Size(322, 23);
            this.m_statusLabel.TabIndex = 11;
            // 
            // mcontrol_pumpDisplay
            // 
            this.mcontrol_pumpDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_pumpDisplay.Location = new System.Drawing.Point(0, 40);
            this.mcontrol_pumpDisplay.Name = "mcontrol_pumpDisplay";
            this.mcontrol_pumpDisplay.Size = new System.Drawing.Size(328, 199);
            this.mcontrol_pumpDisplay.TabIndex = 10;
            this.mcontrol_pumpDisplay.Tacked = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.m_buttonLaunchConsole);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(328, 239);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Control";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.mbutton_stopMethod);
            this.groupBox1.Controls.Add(this.mbutton_getListOfMethods);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.mcomboBox_methods);
            this.groupBox1.Controls.Add(this.mbutton_startMethod);
            this.groupBox1.Controls.Add(this.mnum_methodLength);
            this.groupBox1.Location = new System.Drawing.Point(13, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 165);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Methods";
            // 
            // mbutton_stopMethod
            // 
            this.mbutton_stopMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_stopMethod.Location = new System.Drawing.Point(161, 91);
            this.mbutton_stopMethod.Name = "mbutton_stopMethod";
            this.mbutton_stopMethod.Size = new System.Drawing.Size(132, 32);
            this.mbutton_stopMethod.TabIndex = 11;
            this.mbutton_stopMethod.Text = "Stop Method";
            this.mbutton_stopMethod.UseVisualStyleBackColor = true;
            this.mbutton_stopMethod.Click += new System.EventHandler(this.mbutton_stopMethod_Click);
            // 
            // mbutton_getListOfMethods
            // 
            this.mbutton_getListOfMethods.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_getListOfMethods.Location = new System.Drawing.Point(161, 12);
            this.mbutton_getListOfMethods.Name = "mbutton_getListOfMethods";
            this.mbutton_getListOfMethods.Size = new System.Drawing.Size(132, 32);
            this.mbutton_getListOfMethods.TabIndex = 10;
            this.mbutton_getListOfMethods.Text = "Get List Of Methods";
            this.mbutton_getListOfMethods.UseVisualStyleBackColor = true;
            this.mbutton_getListOfMethods.Click += new System.EventHandler(this.mbutton_getListOfMethods_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(123, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Mins.";
            // 
            // mcomboBox_methods
            // 
            this.mcomboBox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_methods.FormattingEnabled = true;
            this.mcomboBox_methods.Location = new System.Drawing.Point(6, 19);
            this.mcomboBox_methods.Name = "mcomboBox_methods";
            this.mcomboBox_methods.Size = new System.Drawing.Size(149, 21);
            this.mcomboBox_methods.TabIndex = 8;
            // 
            // mbutton_startMethod
            // 
            this.mbutton_startMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_startMethod.Location = new System.Drawing.Point(161, 53);
            this.mbutton_startMethod.Name = "mbutton_startMethod";
            this.mbutton_startMethod.Size = new System.Drawing.Size(132, 32);
            this.mbutton_startMethod.TabIndex = 4;
            this.mbutton_startMethod.Text = "Start Method";
            this.mbutton_startMethod.UseVisualStyleBackColor = true;
            this.mbutton_startMethod.Click += new System.EventHandler(this.m_buttonTestMethod_Click_1);
            // 
            // mnum_methodLength
            // 
            this.mnum_methodLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_methodLength.Location = new System.Drawing.Point(6, 56);
            this.mnum_methodLength.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.mnum_methodLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_methodLength.Name = "mnum_methodLength";
            this.mnum_methodLength.Size = new System.Drawing.Size(111, 20);
            this.mnum_methodLength.TabIndex = 6;
            this.mnum_methodLength.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mbutton_setSystemName);
            this.tabPage2.Controls.Add(this.mbutton_setInstrument);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.mtext_systemName);
            this.tabPage2.Controls.Add(this.mbutton_scanInstruments);
            this.tabPage2.Controls.Add(this.mtext_computerName);
            this.tabPage2.Controls.Add(this.mbutton_setComputerName);
            this.tabPage2.Controls.Add(this.mlist_instruments);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(328, 239);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // mbutton_setSystemName
            // 
            this.mbutton_setSystemName.Location = new System.Drawing.Point(164, 71);
            this.mbutton_setSystemName.Name = "mbutton_setSystemName";
            this.mbutton_setSystemName.Size = new System.Drawing.Size(76, 25);
            this.mbutton_setSystemName.TabIndex = 8;
            this.mbutton_setSystemName.Text = "Set";
            this.mbutton_setSystemName.UseVisualStyleBackColor = true;
            this.mbutton_setSystemName.Click += new System.EventHandler(this.mbutton_setSystemName_Click);
            // 
            // mbutton_setInstrument
            // 
            this.mbutton_setInstrument.Location = new System.Drawing.Point(164, 126);
            this.mbutton_setInstrument.Name = "mbutton_setInstrument";
            this.mbutton_setInstrument.Size = new System.Drawing.Size(76, 25);
            this.mbutton_setInstrument.TabIndex = 7;
            this.mbutton_setInstrument.Text = "Set";
            this.mbutton_setInstrument.UseVisualStyleBackColor = true;
            this.mbutton_setInstrument.Click += new System.EventHandler(this.mbutton_setInstrument_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "System Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Computer Name";
            // 
            // mtext_systemName
            // 
            this.mtext_systemName.Location = new System.Drawing.Point(15, 74);
            this.mtext_systemName.Name = "mtext_systemName";
            this.mtext_systemName.Size = new System.Drawing.Size(143, 20);
            this.mtext_systemName.TabIndex = 4;
            // 
            // mbutton_scanInstruments
            // 
            this.mbutton_scanInstruments.Location = new System.Drawing.Point(164, 157);
            this.mbutton_scanInstruments.Name = "mbutton_scanInstruments";
            this.mbutton_scanInstruments.Size = new System.Drawing.Size(76, 25);
            this.mbutton_scanInstruments.TabIndex = 3;
            this.mbutton_scanInstruments.Text = "Scan";
            this.mbutton_scanInstruments.UseVisualStyleBackColor = true;
            this.mbutton_scanInstruments.Click += new System.EventHandler(this.mbutton_scanInstruments_Click);
            // 
            // mtext_computerName
            // 
            this.mtext_computerName.Location = new System.Drawing.Point(15, 28);
            this.mtext_computerName.Name = "mtext_computerName";
            this.mtext_computerName.Size = new System.Drawing.Size(143, 20);
            this.mtext_computerName.TabIndex = 2;
            // 
            // mbutton_setComputerName
            // 
            this.mbutton_setComputerName.Location = new System.Drawing.Point(164, 25);
            this.mbutton_setComputerName.Name = "mbutton_setComputerName";
            this.mbutton_setComputerName.Size = new System.Drawing.Size(76, 25);
            this.mbutton_setComputerName.TabIndex = 1;
            this.mbutton_setComputerName.Text = "Set";
            this.mbutton_setComputerName.UseVisualStyleBackColor = true;
            this.mbutton_setComputerName.Click += new System.EventHandler(this.mbutton_setComputerName_Click);
            // 
            // mlist_instruments
            // 
            this.mlist_instruments.FormattingEnabled = true;
            this.mlist_instruments.Location = new System.Drawing.Point(15, 124);
            this.mlist_instruments.Name = "mlist_instruments";
            this.mlist_instruments.Size = new System.Drawing.Size(143, 82);
            this.mlist_instruments.TabIndex = 0;
            // 
            // WatersPumpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "WatersPumpControl";
            this.Size = new System.Drawing.Size(336, 359);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_methodLength)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_buttonLaunchConsole;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button mbutton_startMethod;
        private System.Windows.Forms.TextBox mtext_computerName;
        private System.Windows.Forms.Button mbutton_setComputerName;
        private System.Windows.Forms.ListBox mlist_instruments;
        private System.Windows.Forms.Button mbutton_scanInstruments;
        private System.Windows.Forms.TextBox mtext_systemName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown mnum_methodLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mcomboBox_methods;
        private System.Windows.Forms.Button mbutton_getListOfMethods;
        private System.Windows.Forms.Button mbutton_stopMethod;
        private System.Windows.Forms.Button mbutton_setInstrument;
        private System.Windows.Forms.Button mbutton_setSystemName;
        private System.Windows.Forms.TabPage tabPage3;
        private LcmsNetDataClasses.Devices.Pumps.controlPumpDisplay mcontrol_pumpDisplay;
        private System.Windows.Forms.Label m_statusLabel;

    }
}
