namespace WindowsFormsApplication2
{
    partial class controlValveVICI2Pos : controlBaseDevice
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
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.mpropertyGrid_Serial = new System.Windows.Forms.PropertyGrid();
            this.mbutton_SetPosA = new System.Windows.Forms.Button();
            this.mbutton_setPosB = new System.Windows.Forms.Button();
            this.mtextbox_CurrentPos = new System.Windows.Forms.TextBox();
            this.mbutton_refreshPos = new System.Windows.Forms.Button();
            this.m_tabControlValve = new System.Windows.Forms.TabControl();
            this.mtabPage_ValveControl = new System.Windows.Forms.TabPage();
            this.mgroupBox_currentPos = new System.Windows.Forms.GroupBox();
            this.mgroupBox_setPos = new System.Windows.Forms.GroupBox();
            this.mgroupbox_Version = new System.Windows.Forms.GroupBox();
            this.mtextbox_VersionInfo = new System.Windows.Forms.TextBox();
            this.mbutton_refreshVersion = new System.Windows.Forms.Button();
            this.mtabPage_SerialSettings = new System.Windows.Forms.TabPage();
            this.mbutton_ClosePort = new System.Windows.Forms.Button();
            this.mbutton_OpenPort = new System.Windows.Forms.Button();
            this.mtabPage_ValveID = new System.Windows.Forms.TabPage();
            this.mbutton_ClearID = new System.Windows.Forms.Button();
            this.mlabel_GetID = new System.Windows.Forms.Label();
            this.mbutton_RefreshID = new System.Windows.Forms.Button();
            this.mlabel_SetID = new System.Windows.Forms.Label();
            this.mtextbox_currentID = new System.Windows.Forms.TextBox();
            this.mcomboBox_setID = new System.Windows.Forms.ComboBox();
            this.m_tabControlValve.SuspendLayout();
            this.mtabPage_ValveControl.SuspendLayout();
            this.mgroupBox_currentPos.SuspendLayout();
            this.mgroupBox_setPos.SuspendLayout();
            this.mgroupbox_Version.SuspendLayout();
            this.mtabPage_SerialSettings.SuspendLayout();
            this.mtabPage_ValveID.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpropertyGrid_Serial
            // 
            this.mpropertyGrid_Serial.Location = new System.Drawing.Point(-2, 0);
            this.mpropertyGrid_Serial.Name = "mpropertyGrid_Serial";
            this.mpropertyGrid_Serial.Size = new System.Drawing.Size(246, 188);
            this.mpropertyGrid_Serial.TabIndex = 0;
            this.mpropertyGrid_Serial.Click += new System.EventHandler(this.propertyGrid1_Click);
            // 
            // mbutton_SetPosA
            // 
            this.mbutton_SetPosA.Location = new System.Drawing.Point(6, 19);
            this.mbutton_SetPosA.Name = "mbutton_SetPosA";
            this.mbutton_SetPosA.Size = new System.Drawing.Size(40, 40);
            this.mbutton_SetPosA.TabIndex = 1;
            this.mbutton_SetPosA.Text = "A";
            this.mbutton_SetPosA.UseVisualStyleBackColor = true;
            this.mbutton_SetPosA.Click += new System.EventHandler(this.btnA_Click);
            // 
            // mbutton_setPosB
            // 
            this.mbutton_setPosB.Location = new System.Drawing.Point(52, 19);
            this.mbutton_setPosB.Name = "mbutton_setPosB";
            this.mbutton_setPosB.Size = new System.Drawing.Size(40, 40);
            this.mbutton_setPosB.TabIndex = 2;
            this.mbutton_setPosB.Text = "B";
            this.mbutton_setPosB.UseVisualStyleBackColor = true;
            this.mbutton_setPosB.Click += new System.EventHandler(this.btnB_Click);
            // 
            // mtextbox_CurrentPos
            // 
            this.mtextbox_CurrentPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextbox_CurrentPos.Location = new System.Drawing.Point(12, 19);
            this.mtextbox_CurrentPos.Name = "mtextbox_CurrentPos";
            this.mtextbox_CurrentPos.ReadOnly = true;
            this.mtextbox_CurrentPos.Size = new System.Drawing.Size(75, 31);
            this.mtextbox_CurrentPos.TabIndex = 5;
            this.mtextbox_CurrentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_refreshPos
            // 
            this.mbutton_refreshPos.Location = new System.Drawing.Point(12, 56);
            this.mbutton_refreshPos.Name = "mbutton_refreshPos";
            this.mbutton_refreshPos.Size = new System.Drawing.Size(75, 23);
            this.mbutton_refreshPos.TabIndex = 6;
            this.mbutton_refreshPos.Text = "Refresh";
            this.mbutton_refreshPos.UseVisualStyleBackColor = true;
            this.mbutton_refreshPos.Click += new System.EventHandler(this.btnRefreshPos_Click);
            // 
            // m_tabControlValve
            // 
            this.m_tabControlValve.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabControlValve.Controls.Add(this.mtabPage_ValveControl);
            this.m_tabControlValve.Controls.Add(this.mtabPage_SerialSettings);
            this.m_tabControlValve.Controls.Add(this.mtabPage_ValveID);
            this.m_tabControlValve.Location = new System.Drawing.Point(3, 3);
            this.m_tabControlValve.Name = "m_tabControlValve";
            this.m_tabControlValve.SelectedIndex = 0;
            this.m_tabControlValve.Size = new System.Drawing.Size(247, 226);
            this.m_tabControlValve.TabIndex = 8;
            this.m_tabControlValve.Validated += new System.EventHandler(this.tabControl1_Validated);
            // 
            // mtabPage_ValveControl
            // 
            this.mtabPage_ValveControl.Controls.Add(this.mgroupBox_currentPos);
            this.mtabPage_ValveControl.Controls.Add(this.mgroupBox_setPos);
            this.mtabPage_ValveControl.Controls.Add(this.mgroupbox_Version);
            this.mtabPage_ValveControl.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_ValveControl.Name = "mtabPage_ValveControl";
            this.mtabPage_ValveControl.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_ValveControl.Size = new System.Drawing.Size(239, 200);
            this.mtabPage_ValveControl.TabIndex = 0;
            this.mtabPage_ValveControl.Text = "Valve Control";
            this.mtabPage_ValveControl.UseVisualStyleBackColor = true;
            this.mtabPage_ValveControl.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // mgroupBox_currentPos
            // 
            this.mgroupBox_currentPos.Controls.Add(this.mtextbox_CurrentPos);
            this.mgroupBox_currentPos.Controls.Add(this.mbutton_refreshPos);
            this.mgroupBox_currentPos.Location = new System.Drawing.Point(133, 11);
            this.mgroupBox_currentPos.Name = "mgroupBox_currentPos";
            this.mgroupBox_currentPos.Size = new System.Drawing.Size(99, 85);
            this.mgroupBox_currentPos.TabIndex = 12;
            this.mgroupBox_currentPos.TabStop = false;
            this.mgroupBox_currentPos.Text = "Current Position";
            // 
            // mgroupBox_setPos
            // 
            this.mgroupBox_setPos.Controls.Add(this.mbutton_SetPosA);
            this.mgroupBox_setPos.Controls.Add(this.mbutton_setPosB);
            this.mgroupBox_setPos.Location = new System.Drawing.Point(15, 11);
            this.mgroupBox_setPos.Name = "mgroupBox_setPos";
            this.mgroupBox_setPos.Size = new System.Drawing.Size(99, 69);
            this.mgroupBox_setPos.TabIndex = 11;
            this.mgroupBox_setPos.TabStop = false;
            this.mgroupBox_setPos.Text = "Set Position";
            // 
            // mgroupbox_Version
            // 
            this.mgroupbox_Version.Controls.Add(this.mtextbox_VersionInfo);
            this.mgroupbox_Version.Controls.Add(this.mbutton_refreshVersion);
            this.mgroupbox_Version.Location = new System.Drawing.Point(6, 102);
            this.mgroupbox_Version.Name = "mgroupbox_Version";
            this.mgroupbox_Version.Size = new System.Drawing.Size(229, 73);
            this.mgroupbox_Version.TabIndex = 10;
            this.mgroupbox_Version.TabStop = false;
            this.mgroupbox_Version.Text = "Version Info";
            // 
            // mtextbox_VersionInfo
            // 
            this.mtextbox_VersionInfo.Location = new System.Drawing.Point(6, 19);
            this.mtextbox_VersionInfo.Name = "mtextbox_VersionInfo";
            this.mtextbox_VersionInfo.ReadOnly = true;
            this.mtextbox_VersionInfo.Size = new System.Drawing.Size(217, 20);
            this.mtextbox_VersionInfo.TabIndex = 8;
            // 
            // mbutton_refreshVersion
            // 
            this.mbutton_refreshVersion.Location = new System.Drawing.Point(58, 45);
            this.mbutton_refreshVersion.Name = "mbutton_refreshVersion";
            this.mbutton_refreshVersion.Size = new System.Drawing.Size(113, 23);
            this.mbutton_refreshVersion.TabIndex = 9;
            this.mbutton_refreshVersion.Text = "Refresh";
            this.mbutton_refreshVersion.UseVisualStyleBackColor = true;
            this.mbutton_refreshVersion.Click += new System.EventHandler(this.btnRefreshVer_Click);
            // 
            // mtabPage_SerialSettings
            // 
            this.mtabPage_SerialSettings.Controls.Add(this.mbutton_ClosePort);
            this.mtabPage_SerialSettings.Controls.Add(this.mbutton_OpenPort);
            this.mtabPage_SerialSettings.Controls.Add(this.mpropertyGrid_Serial);
            this.mtabPage_SerialSettings.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_SerialSettings.Name = "mtabPage_SerialSettings";
            this.mtabPage_SerialSettings.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_SerialSettings.Size = new System.Drawing.Size(253, 206);
            this.mtabPage_SerialSettings.TabIndex = 1;
            this.mtabPage_SerialSettings.Text = "Serial Settings";
            this.mtabPage_SerialSettings.UseVisualStyleBackColor = true;
            // 
            // mbutton_ClosePort
            // 
            this.mbutton_ClosePort.Location = new System.Drawing.Point(170, 0);
            this.mbutton_ClosePort.Name = "mbutton_ClosePort";
            this.mbutton_ClosePort.Size = new System.Drawing.Size(63, 23);
            this.mbutton_ClosePort.TabIndex = 2;
            this.mbutton_ClosePort.Text = "Close Port";
            this.mbutton_ClosePort.UseVisualStyleBackColor = true;
            this.mbutton_ClosePort.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mbutton_OpenPort
            // 
            this.mbutton_OpenPort.Location = new System.Drawing.Point(101, 0);
            this.mbutton_OpenPort.Name = "mbutton_OpenPort";
            this.mbutton_OpenPort.Size = new System.Drawing.Size(63, 23);
            this.mbutton_OpenPort.TabIndex = 1;
            this.mbutton_OpenPort.Text = "Open Port";
            this.mbutton_OpenPort.UseVisualStyleBackColor = true;
            this.mbutton_OpenPort.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // mtabPage_ValveID
            // 
            this.mtabPage_ValveID.Controls.Add(this.mbutton_ClearID);
            this.mtabPage_ValveID.Controls.Add(this.mlabel_GetID);
            this.mtabPage_ValveID.Controls.Add(this.mbutton_RefreshID);
            this.mtabPage_ValveID.Controls.Add(this.mlabel_SetID);
            this.mtabPage_ValveID.Controls.Add(this.mtextbox_currentID);
            this.mtabPage_ValveID.Controls.Add(this.mcomboBox_setID);
            this.mtabPage_ValveID.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_ValveID.Name = "mtabPage_ValveID";
            this.mtabPage_ValveID.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_ValveID.Size = new System.Drawing.Size(253, 206);
            this.mtabPage_ValveID.TabIndex = 2;
            this.mtabPage_ValveID.Text = "Valve ID";
            this.mtabPage_ValveID.UseVisualStyleBackColor = true;
            // 
            // mbutton_ClearID
            // 
            this.mbutton_ClearID.Location = new System.Drawing.Point(70, 98);
            this.mbutton_ClearID.Name = "mbutton_ClearID";
            this.mbutton_ClearID.Size = new System.Drawing.Size(55, 23);
            this.mbutton_ClearID.TabIndex = 5;
            this.mbutton_ClearID.Text = "Clear ID";
            this.mbutton_ClearID.UseVisualStyleBackColor = true;
            this.mbutton_ClearID.Click += new System.EventHandler(this.btnClearID_Click);
            // 
            // mlabel_GetID
            // 
            this.mlabel_GetID.AutoSize = true;
            this.mlabel_GetID.Location = new System.Drawing.Point(129, 54);
            this.mlabel_GetID.Name = "mlabel_GetID";
            this.mlabel_GetID.Size = new System.Drawing.Size(38, 13);
            this.mlabel_GetID.TabIndex = 4;
            this.mlabel_GetID.Text = "Get ID";
            // 
            // mbutton_RefreshID
            // 
            this.mbutton_RefreshID.Location = new System.Drawing.Point(132, 98);
            this.mbutton_RefreshID.Name = "mbutton_RefreshID";
            this.mbutton_RefreshID.Size = new System.Drawing.Size(52, 23);
            this.mbutton_RefreshID.TabIndex = 3;
            this.mbutton_RefreshID.Text = "Refresh";
            this.mbutton_RefreshID.UseVisualStyleBackColor = true;
            this.mbutton_RefreshID.Click += new System.EventHandler(this.btnRefreshID_Click);
            // 
            // mlabel_SetID
            // 
            this.mlabel_SetID.AutoSize = true;
            this.mlabel_SetID.Location = new System.Drawing.Point(67, 54);
            this.mlabel_SetID.Name = "mlabel_SetID";
            this.mlabel_SetID.Size = new System.Drawing.Size(37, 13);
            this.mlabel_SetID.TabIndex = 2;
            this.mlabel_SetID.Text = "Set ID";
            // 
            // mtextbox_currentID
            // 
            this.mtextbox_currentID.Location = new System.Drawing.Point(132, 71);
            this.mtextbox_currentID.Name = "mtextbox_currentID";
            this.mtextbox_currentID.ReadOnly = true;
            this.mtextbox_currentID.Size = new System.Drawing.Size(22, 20);
            this.mtextbox_currentID.TabIndex = 1;
            // 
            // mcomboBox_setID
            // 
            this.mcomboBox_setID.FormattingEnabled = true;
            this.mcomboBox_setID.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            " "});
            this.mcomboBox_setID.Location = new System.Drawing.Point(70, 70);
            this.mcomboBox_setID.Name = "mcomboBox_setID";
            this.mcomboBox_setID.Size = new System.Drawing.Size(41, 21);
            this.mcomboBox_setID.TabIndex = 0;
            this.mcomboBox_setID.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // controlValveVICI2Pos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_tabControlValve);
            this.Name = "controlValveVICI2Pos";
            this.Size = new System.Drawing.Size(252, 290);
            this.Load += new System.EventHandler(this.valvetest_Load);
            this.Controls.SetChildIndex(this.m_tabControlValve, 0);
            this.m_tabControlValve.ResumeLayout(false);
            this.mtabPage_ValveControl.ResumeLayout(false);
            this.mgroupBox_currentPos.ResumeLayout(false);
            this.mgroupBox_currentPos.PerformLayout();
            this.mgroupBox_setPos.ResumeLayout(false);
            this.mgroupbox_Version.ResumeLayout(false);
            this.mgroupbox_Version.PerformLayout();
            this.mtabPage_SerialSettings.ResumeLayout(false);
            this.mtabPage_ValveID.ResumeLayout(false);
            this.mtabPage_ValveID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.PropertyGrid mpropertyGrid_Serial;
        private System.Windows.Forms.Button mbutton_SetPosA;
        private System.Windows.Forms.Button mbutton_setPosB;
        private System.Windows.Forms.TextBox mtextbox_CurrentPos;
        private System.Windows.Forms.Button mbutton_refreshPos;
        private System.Windows.Forms.TabControl m_tabControlValve;
        private System.Windows.Forms.TabPage mtabPage_ValveControl;
        private System.Windows.Forms.TabPage mtabPage_SerialSettings;
        private System.Windows.Forms.TextBox mtextbox_VersionInfo;
        private System.Windows.Forms.Button mbutton_ClosePort;
        private System.Windows.Forms.Button mbutton_OpenPort;
        private System.Windows.Forms.GroupBox mgroupBox_currentPos;
        private System.Windows.Forms.GroupBox mgroupBox_setPos;
        private System.Windows.Forms.GroupBox mgroupbox_Version;
        private System.Windows.Forms.Button mbutton_refreshVersion;
        private System.Windows.Forms.TabPage mtabPage_ValveID;
        private System.Windows.Forms.ComboBox mcomboBox_setID;
        private System.Windows.Forms.Label mlabel_SetID;
        private System.Windows.Forms.TextBox mtextbox_currentID;
        private System.Windows.Forms.Button mbutton_RefreshID;
        private System.Windows.Forms.Label mlabel_GetID;
        private System.Windows.Forms.Button mbutton_ClearID;
    }
}
