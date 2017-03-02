namespace LcmsNet.Devices.Valves
{
    partial class controlValveVICIMultiPos
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
            this.mtextbox_CurrentPos = new System.Windows.Forms.TextBox();
            this.mbutton_refreshPos = new System.Windows.Forms.Button();
            this.m_tabControlValve = new System.Windows.Forms.TabControl();
            this.mtabPage_ValveControl = new System.Windows.Forms.TabPage();
            this.mgroupBox_currentPos = new System.Windows.Forms.GroupBox();
            this.mgroupBox_setPos = new System.Windows.Forms.GroupBox();
            this.mbutton_SetPosition = new System.Windows.Forms.Button();
            this.mcomboBox_Position = new System.Windows.Forms.ComboBox();
            this.mgroupbox_Version = new System.Windows.Forms.GroupBox();
            this.mtextbox_VersionInfo = new System.Windows.Forms.TextBox();
            this.mbutton_refreshVersion = new System.Windows.Forms.Button();
            this.mtabPage_SerialSettings = new System.Windows.Forms.TabPage();
            this.mbutton_initialize = new System.Windows.Forms.Button();
            this.mbutton_ClosePort = new System.Windows.Forms.Button();
            this.mbutton_OpenPort = new System.Windows.Forms.Button();
            this.mtabPage_ValveID = new System.Windows.Forms.TabPage();
            this.mgroupBox_Positions = new System.Windows.Forms.GroupBox();
            this.mbutton_GetNumPos = new System.Windows.Forms.Button();
            this.mbutton_SetNumPos = new System.Windows.Forms.Button();
            this.mtextBox_GetNumPos = new System.Windows.Forms.TextBox();
            this.mtextBox_SetNumPos = new System.Windows.Forms.TextBox();
            this.mgroupBox_valveID = new System.Windows.Forms.GroupBox();
            this.mlabel_SetID = new System.Windows.Forms.Label();
            this.mbutton_ClearID = new System.Windows.Forms.Button();
            this.mcomboBox_setID = new System.Windows.Forms.ComboBox();
            this.mlabel_GetID = new System.Windows.Forms.Label();
            this.mtextbox_currentID = new System.Windows.Forms.TextBox();
            this.mbutton_RefreshID = new System.Windows.Forms.Button();
            this.m_tabControlValve.SuspendLayout();
            this.mtabPage_ValveControl.SuspendLayout();
            this.mgroupBox_currentPos.SuspendLayout();
            this.mgroupBox_setPos.SuspendLayout();
            this.mgroupbox_Version.SuspendLayout();
            this.mtabPage_SerialSettings.SuspendLayout();
            this.mtabPage_ValveID.SuspendLayout();
            this.mgroupBox_Positions.SuspendLayout();
            this.mgroupBox_valveID.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpropertyGrid_Serial
            // 
            this.mpropertyGrid_Serial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpropertyGrid_Serial.Location = new System.Drawing.Point(-2, 0);
            this.mpropertyGrid_Serial.Name = "mpropertyGrid_Serial";
            this.mpropertyGrid_Serial.Size = new System.Drawing.Size(249, 320);
            this.mpropertyGrid_Serial.TabIndex = 0;
            this.mpropertyGrid_Serial.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.mpropertyGrid_Serial_PropertyValueChanged);
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
            this.m_tabControlValve.Controls.Add(this.mtabPage_ValveControl);
            this.m_tabControlValve.Controls.Add(this.mtabPage_SerialSettings);
            this.m_tabControlValve.Controls.Add(this.mtabPage_ValveID);
            this.m_tabControlValve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tabControlValve.Location = new System.Drawing.Point(0, 0);
            this.m_tabControlValve.MinimumSize = new System.Drawing.Size(247, 272);
            this.m_tabControlValve.Name = "m_tabControlValve";
            this.m_tabControlValve.SelectedIndex = 0;
            this.m_tabControlValve.Size = new System.Drawing.Size(267, 376);
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
            this.mtabPage_ValveControl.Size = new System.Drawing.Size(259, 350);
            this.mtabPage_ValveControl.TabIndex = 0;
            this.mtabPage_ValveControl.Text = "Valve Control";
            this.mtabPage_ValveControl.UseVisualStyleBackColor = true;
            this.mtabPage_ValveControl.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // mgroupBox_currentPos
            // 
            this.mgroupBox_currentPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_currentPos.Controls.Add(this.mtextbox_CurrentPos);
            this.mgroupBox_currentPos.Controls.Add(this.mbutton_refreshPos);
            this.mgroupBox_currentPos.Location = new System.Drawing.Point(145, 11);
            this.mgroupBox_currentPos.Name = "mgroupBox_currentPos";
            this.mgroupBox_currentPos.Size = new System.Drawing.Size(99, 85);
            this.mgroupBox_currentPos.TabIndex = 12;
            this.mgroupBox_currentPos.TabStop = false;
            this.mgroupBox_currentPos.Text = "Current Position";
            // 
            // mgroupBox_setPos
            // 
            this.mgroupBox_setPos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_setPos.Controls.Add(this.mbutton_SetPosition);
            this.mgroupBox_setPos.Controls.Add(this.mcomboBox_Position);
            this.mgroupBox_setPos.Location = new System.Drawing.Point(15, 11);
            this.mgroupBox_setPos.Name = "mgroupBox_setPos";
            this.mgroupBox_setPos.Size = new System.Drawing.Size(124, 85);
            this.mgroupBox_setPos.TabIndex = 11;
            this.mgroupBox_setPos.TabStop = false;
            this.mgroupBox_setPos.Text = "Set Position";
            // 
            // mbutton_SetPosition
            // 
            this.mbutton_SetPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_SetPosition.Location = new System.Drawing.Point(79, 19);
            this.mbutton_SetPosition.Name = "mbutton_SetPosition";
            this.mbutton_SetPosition.Size = new System.Drawing.Size(37, 23);
            this.mbutton_SetPosition.TabIndex = 1;
            this.mbutton_SetPosition.Text = "Set";
            this.mbutton_SetPosition.UseVisualStyleBackColor = true;
            this.mbutton_SetPosition.Click += new System.EventHandler(this.mbutton_SetPosition_Click);
            // 
            // mcomboBox_Position
            // 
            this.mcomboBox_Position.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_Position.FormattingEnabled = true;
            this.mcomboBox_Position.Location = new System.Drawing.Point(7, 19);
            this.mcomboBox_Position.Name = "mcomboBox_Position";
            this.mcomboBox_Position.Size = new System.Drawing.Size(66, 21);
            this.mcomboBox_Position.TabIndex = 0;
            // 
            // mgroupbox_Version
            // 
            this.mgroupbox_Version.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupbox_Version.Controls.Add(this.mtextbox_VersionInfo);
            this.mgroupbox_Version.Controls.Add(this.mbutton_refreshVersion);
            this.mgroupbox_Version.Location = new System.Drawing.Point(6, 102);
            this.mgroupbox_Version.Name = "mgroupbox_Version";
            this.mgroupbox_Version.Size = new System.Drawing.Size(247, 242);
            this.mgroupbox_Version.TabIndex = 10;
            this.mgroupbox_Version.TabStop = false;
            this.mgroupbox_Version.Text = "Version Info";
            // 
            // mtextbox_VersionInfo
            // 
            this.mtextbox_VersionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextbox_VersionInfo.Location = new System.Drawing.Point(6, 19);
            this.mtextbox_VersionInfo.Name = "mtextbox_VersionInfo";
            this.mtextbox_VersionInfo.ReadOnly = true;
            this.mtextbox_VersionInfo.Size = new System.Drawing.Size(235, 20);
            this.mtextbox_VersionInfo.TabIndex = 8;
            // 
            // mbutton_refreshVersion
            // 
            this.mbutton_refreshVersion.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mbutton_refreshVersion.Location = new System.Drawing.Point(63, 47);
            this.mbutton_refreshVersion.Name = "mbutton_refreshVersion";
            this.mbutton_refreshVersion.Size = new System.Drawing.Size(113, 23);
            this.mbutton_refreshVersion.TabIndex = 9;
            this.mbutton_refreshVersion.Text = "Refresh";
            this.mbutton_refreshVersion.UseVisualStyleBackColor = true;
            this.mbutton_refreshVersion.Click += new System.EventHandler(this.btnRefreshVer_Click);
            // 
            // mtabPage_SerialSettings
            // 
            this.mtabPage_SerialSettings.Controls.Add(this.mbutton_initialize);
            this.mtabPage_SerialSettings.Controls.Add(this.mbutton_ClosePort);
            this.mtabPage_SerialSettings.Controls.Add(this.mbutton_OpenPort);
            this.mtabPage_SerialSettings.Controls.Add(this.mpropertyGrid_Serial);
            this.mtabPage_SerialSettings.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_SerialSettings.Name = "mtabPage_SerialSettings";
            this.mtabPage_SerialSettings.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_SerialSettings.Size = new System.Drawing.Size(259, 350);
            this.mtabPage_SerialSettings.TabIndex = 1;
            this.mtabPage_SerialSettings.Text = "Serial Settings";
            this.mtabPage_SerialSettings.UseVisualStyleBackColor = true;
            // 
            // mbutton_initialize
            // 
            this.mbutton_initialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_initialize.Location = new System.Drawing.Point(184, 327);
            this.mbutton_initialize.Name = "mbutton_initialize";
            this.mbutton_initialize.Size = new System.Drawing.Size(63, 23);
            this.mbutton_initialize.TabIndex = 3;
            this.mbutton_initialize.Text = "Initialize";
            this.mbutton_initialize.UseVisualStyleBackColor = true;
            this.mbutton_initialize.Click += new System.EventHandler(this.mbutton_initialize_Click);
            // 
            // mbutton_ClosePort
            // 
            this.mbutton_ClosePort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_ClosePort.Location = new System.Drawing.Point(99, 327);
            this.mbutton_ClosePort.Name = "mbutton_ClosePort";
            this.mbutton_ClosePort.Size = new System.Drawing.Size(63, 23);
            this.mbutton_ClosePort.TabIndex = 2;
            this.mbutton_ClosePort.Text = "Close Port";
            this.mbutton_ClosePort.UseVisualStyleBackColor = true;
            this.mbutton_ClosePort.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mbutton_OpenPort
            // 
            this.mbutton_OpenPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_OpenPort.Location = new System.Drawing.Point(3, 327);
            this.mbutton_OpenPort.Name = "mbutton_OpenPort";
            this.mbutton_OpenPort.Size = new System.Drawing.Size(63, 23);
            this.mbutton_OpenPort.TabIndex = 1;
            this.mbutton_OpenPort.Text = "Open Port";
            this.mbutton_OpenPort.UseVisualStyleBackColor = true;
            this.mbutton_OpenPort.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // mtabPage_ValveID
            // 
            this.mtabPage_ValveID.Controls.Add(this.mgroupBox_Positions);
            this.mtabPage_ValveID.Controls.Add(this.mgroupBox_valveID);
            this.mtabPage_ValveID.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_ValveID.Name = "mtabPage_ValveID";
            this.mtabPage_ValveID.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_ValveID.Size = new System.Drawing.Size(259, 350);
            this.mtabPage_ValveID.TabIndex = 2;
            this.mtabPage_ValveID.Text = "Advanced";
            this.mtabPage_ValveID.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_Positions
            // 
            this.mgroupBox_Positions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_Positions.Controls.Add(this.mbutton_GetNumPos);
            this.mgroupBox_Positions.Controls.Add(this.mbutton_SetNumPos);
            this.mgroupBox_Positions.Controls.Add(this.mtextBox_GetNumPos);
            this.mgroupBox_Positions.Controls.Add(this.mtextBox_SetNumPos);
            this.mgroupBox_Positions.Location = new System.Drawing.Point(7, 121);
            this.mgroupBox_Positions.Name = "mgroupBox_Positions";
            this.mgroupBox_Positions.Size = new System.Drawing.Size(226, 94);
            this.mgroupBox_Positions.TabIndex = 7;
            this.mgroupBox_Positions.TabStop = false;
            this.mgroupBox_Positions.Text = "Positions";
            // 
            // mbutton_GetNumPos
            // 
            this.mbutton_GetNumPos.Location = new System.Drawing.Point(167, 20);
            this.mbutton_GetNumPos.Name = "mbutton_GetNumPos";
            this.mbutton_GetNumPos.Size = new System.Drawing.Size(53, 39);
            this.mbutton_GetNumPos.TabIndex = 3;
            this.mbutton_GetNumPos.Text = "Get";
            this.mbutton_GetNumPos.UseVisualStyleBackColor = true;
            this.mbutton_GetNumPos.Click += new System.EventHandler(this.mbutton_GetNumPos_Click);
            // 
            // mbutton_SetNumPos
            // 
            this.mbutton_SetNumPos.Location = new System.Drawing.Point(57, 20);
            this.mbutton_SetNumPos.Name = "mbutton_SetNumPos";
            this.mbutton_SetNumPos.Size = new System.Drawing.Size(54, 41);
            this.mbutton_SetNumPos.TabIndex = 2;
            this.mbutton_SetNumPos.Text = "Set";
            this.mbutton_SetNumPos.UseVisualStyleBackColor = true;
            this.mbutton_SetNumPos.Click += new System.EventHandler(this.mbutton_SetNumPos_Click);
            // 
            // mtextBox_GetNumPos
            // 
            this.mtextBox_GetNumPos.Location = new System.Drawing.Point(117, 20);
            this.mtextBox_GetNumPos.Name = "mtextBox_GetNumPos";
            this.mtextBox_GetNumPos.ReadOnly = true;
            this.mtextBox_GetNumPos.Size = new System.Drawing.Size(44, 20);
            this.mtextBox_GetNumPos.TabIndex = 1;
            // 
            // mtextBox_SetNumPos
            // 
            this.mtextBox_SetNumPos.Location = new System.Drawing.Point(7, 20);
            this.mtextBox_SetNumPos.Name = "mtextBox_SetNumPos";
            this.mtextBox_SetNumPos.Size = new System.Drawing.Size(44, 20);
            this.mtextBox_SetNumPos.TabIndex = 0;
            // 
            // mgroupBox_valveID
            // 
            this.mgroupBox_valveID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_valveID.Controls.Add(this.mlabel_SetID);
            this.mgroupBox_valveID.Controls.Add(this.mbutton_ClearID);
            this.mgroupBox_valveID.Controls.Add(this.mcomboBox_setID);
            this.mgroupBox_valveID.Controls.Add(this.mlabel_GetID);
            this.mgroupBox_valveID.Controls.Add(this.mtextbox_currentID);
            this.mgroupBox_valveID.Controls.Add(this.mbutton_RefreshID);
            this.mgroupBox_valveID.Location = new System.Drawing.Point(7, 6);
            this.mgroupBox_valveID.Name = "mgroupBox_valveID";
            this.mgroupBox_valveID.Size = new System.Drawing.Size(226, 109);
            this.mgroupBox_valveID.TabIndex = 6;
            this.mgroupBox_valveID.TabStop = false;
            this.mgroupBox_valveID.Text = "Valve ID";
            // 
            // mlabel_SetID
            // 
            this.mlabel_SetID.AutoSize = true;
            this.mlabel_SetID.Location = new System.Drawing.Point(6, 16);
            this.mlabel_SetID.Name = "mlabel_SetID";
            this.mlabel_SetID.Size = new System.Drawing.Size(37, 13);
            this.mlabel_SetID.TabIndex = 2;
            this.mlabel_SetID.Text = "Set ID";
            // 
            // mbutton_ClearID
            // 
            this.mbutton_ClearID.Location = new System.Drawing.Point(78, 30);
            this.mbutton_ClearID.Name = "mbutton_ClearID";
            this.mbutton_ClearID.Size = new System.Drawing.Size(55, 23);
            this.mbutton_ClearID.TabIndex = 5;
            this.mbutton_ClearID.Text = "Clear ID";
            this.mbutton_ClearID.UseVisualStyleBackColor = true;
            this.mbutton_ClearID.Click += new System.EventHandler(this.btnClearID_Click);
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
            this.mcomboBox_setID.Location = new System.Drawing.Point(9, 32);
            this.mcomboBox_setID.Name = "mcomboBox_setID";
            this.mcomboBox_setID.Size = new System.Drawing.Size(55, 21);
            this.mcomboBox_setID.TabIndex = 0;
            this.mcomboBox_setID.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // mlabel_GetID
            // 
            this.mlabel_GetID.AutoSize = true;
            this.mlabel_GetID.Location = new System.Drawing.Point(9, 56);
            this.mlabel_GetID.Name = "mlabel_GetID";
            this.mlabel_GetID.Size = new System.Drawing.Size(38, 13);
            this.mlabel_GetID.TabIndex = 4;
            this.mlabel_GetID.Text = "Get ID";
            // 
            // mtextbox_currentID
            // 
            this.mtextbox_currentID.Location = new System.Drawing.Point(13, 72);
            this.mtextbox_currentID.Name = "mtextbox_currentID";
            this.mtextbox_currentID.ReadOnly = true;
            this.mtextbox_currentID.Size = new System.Drawing.Size(51, 20);
            this.mtextbox_currentID.TabIndex = 1;
            // 
            // mbutton_RefreshID
            // 
            this.mbutton_RefreshID.Location = new System.Drawing.Point(78, 72);
            this.mbutton_RefreshID.Name = "mbutton_RefreshID";
            this.mbutton_RefreshID.Size = new System.Drawing.Size(52, 23);
            this.mbutton_RefreshID.TabIndex = 3;
            this.mbutton_RefreshID.Text = "Refresh";
            this.mbutton_RefreshID.UseVisualStyleBackColor = true;
            this.mbutton_RefreshID.Click += new System.EventHandler(this.btnRefreshID_Click);
            // 
            // controlValveVICIMultiPos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_tabControlValve);
            this.Name = "controlValveVICIMultiPos";
            this.Size = new System.Drawing.Size(267, 376);
            this.Load += new System.EventHandler(this.valvetest_Load);
            this.m_tabControlValve.ResumeLayout(false);
            this.mtabPage_ValveControl.ResumeLayout(false);
            this.mgroupBox_currentPos.ResumeLayout(false);
            this.mgroupBox_currentPos.PerformLayout();
            this.mgroupBox_setPos.ResumeLayout(false);
            this.mgroupbox_Version.ResumeLayout(false);
            this.mgroupbox_Version.PerformLayout();
            this.mtabPage_SerialSettings.ResumeLayout(false);
            this.mtabPage_ValveID.ResumeLayout(false);
            this.mgroupBox_Positions.ResumeLayout(false);
            this.mgroupBox_Positions.PerformLayout();
            this.mgroupBox_valveID.ResumeLayout(false);
            this.mgroupBox_valveID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.PropertyGrid mpropertyGrid_Serial;
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
        private System.Windows.Forms.Button mbutton_SetPosition;
        private System.Windows.Forms.ComboBox mcomboBox_Position;
        private System.Windows.Forms.GroupBox mgroupBox_Positions;
        private System.Windows.Forms.Button mbutton_GetNumPos;
        private System.Windows.Forms.Button mbutton_SetNumPos;
        private System.Windows.Forms.TextBox mtextBox_GetNumPos;
        private System.Windows.Forms.TextBox mtextBox_SetNumPos;
        private System.Windows.Forms.GroupBox mgroupBox_valveID;
        private System.Windows.Forms.Button mbutton_initialize;
    }
}
