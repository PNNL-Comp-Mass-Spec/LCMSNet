namespace LcmsNet.Devices.ContactClosure
{
    partial class controlContactClosure
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
            this.mcomboBox_Ports = new System.Windows.Forms.ComboBox();
            this.mtextbox_PulseLength = new System.Windows.Forms.TextBox();
            this.mbutton_SendPulse = new System.Windows.Forms.Button();
            this.mgroupBox_PulseControls = new System.Windows.Forms.GroupBox();
            this.mlabel_PulseLength = new System.Windows.Forms.Label();
            this.mlabel_Port = new System.Windows.Forms.Label();
            this.mtextBox_Voltage = new System.Windows.Forms.TextBox();
            this.mlabel_Voltage = new System.Windows.Forms.Label();
            this.mgroupBox_PulseControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // mcomboBox_Ports
            // 
            this.mcomboBox_Ports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_Ports.FormattingEnabled = true;
            this.mcomboBox_Ports.Items.AddRange(new object[] {
            enumLabjackU12OutputPorts.AO0,
            enumLabjackU12OutputPorts.AO0,
            enumLabjackU12OutputPorts.AO1,
            enumLabjackU12OutputPorts.D0,
            enumLabjackU12OutputPorts.D1,
            enumLabjackU12OutputPorts.D2,
            enumLabjackU12OutputPorts.D3,
            enumLabjackU12OutputPorts.D4,
            enumLabjackU12OutputPorts.D5,
            enumLabjackU12OutputPorts.D6,
            enumLabjackU12OutputPorts.D7,
            enumLabjackU12OutputPorts.D8,
            enumLabjackU12OutputPorts.D9,
            enumLabjackU12OutputPorts.D10,
            enumLabjackU12OutputPorts.D11,
            enumLabjackU12OutputPorts.D12,
            enumLabjackU12OutputPorts.D13,
            enumLabjackU12OutputPorts.D14,
            enumLabjackU12OutputPorts.D15,
            enumLabjackU12OutputPorts.IO0,
            enumLabjackU12OutputPorts.IO1,
            enumLabjackU12OutputPorts.IO2,
            enumLabjackU12OutputPorts.IO3});
            this.mcomboBox_Ports.Location = new System.Drawing.Point(12, 29);
            this.mcomboBox_Ports.Name = "mcomboBox_Ports";
            this.mcomboBox_Ports.Size = new System.Drawing.Size(81, 21);
            this.mcomboBox_Ports.TabIndex = 0;
            this.mcomboBox_Ports.SelectedValueChanged += new System.EventHandler(this.mcomboBox_Ports_SelectedValueChanged);
            // 
            // mtextbox_PulseLength
            // 
            this.mtextbox_PulseLength.Location = new System.Drawing.Point(98, 29);
            this.mtextbox_PulseLength.Name = "mtextbox_PulseLength";
            this.mtextbox_PulseLength.Size = new System.Drawing.Size(58, 20);
            this.mtextbox_PulseLength.TabIndex = 1;
            this.mtextbox_PulseLength.Text = "1000";
            // 
            // mbutton_SendPulse
            // 
            this.mbutton_SendPulse.Location = new System.Drawing.Point(222, 26);
            this.mbutton_SendPulse.Name = "mbutton_SendPulse";
            this.mbutton_SendPulse.Size = new System.Drawing.Size(54, 23);
            this.mbutton_SendPulse.TabIndex = 2;
            this.mbutton_SendPulse.Text = "Send";
            this.mbutton_SendPulse.UseVisualStyleBackColor = true;
            this.mbutton_SendPulse.Click += new System.EventHandler(this.mbutton_SendPulse_Click);
            // 
            // mgroupBox_PulseControls
            // 
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_Voltage);
            this.mgroupBox_PulseControls.Controls.Add(this.mtextBox_Voltage);
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_PulseLength);
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_Port);
            this.mgroupBox_PulseControls.Controls.Add(this.mcomboBox_Ports);
            this.mgroupBox_PulseControls.Controls.Add(this.mbutton_SendPulse);
            this.mgroupBox_PulseControls.Controls.Add(this.mtextbox_PulseLength);
            this.mgroupBox_PulseControls.Location = new System.Drawing.Point(3, 3);
            this.mgroupBox_PulseControls.Name = "mgroupBox_PulseControls";
            this.mgroupBox_PulseControls.Size = new System.Drawing.Size(285, 59);
            this.mgroupBox_PulseControls.TabIndex = 3;
            this.mgroupBox_PulseControls.TabStop = false;
            // 
            // mlabel_PulseLength
            // 
            this.mlabel_PulseLength.AutoSize = true;
            this.mlabel_PulseLength.Location = new System.Drawing.Point(94, 13);
            this.mlabel_PulseLength.Name = "mlabel_PulseLength";
            this.mlabel_PulseLength.Size = new System.Drawing.Size(62, 13);
            this.mlabel_PulseLength.TabIndex = 4;
            this.mlabel_PulseLength.Text = "Length (ms)";
            // 
            // mlabel_Port
            // 
            this.mlabel_Port.AutoSize = true;
            this.mlabel_Port.Location = new System.Drawing.Point(10, 13);
            this.mlabel_Port.Name = "mlabel_Port";
            this.mlabel_Port.Size = new System.Drawing.Size(61, 13);
            this.mlabel_Port.TabIndex = 3;
            this.mlabel_Port.Text = "Output Port";
            // 
            // mtextBox_Voltage
            // 
            this.mtextBox_Voltage.Location = new System.Drawing.Point(161, 29);
            this.mtextBox_Voltage.Name = "mtextBox_Voltage";
            this.mtextBox_Voltage.Size = new System.Drawing.Size(57, 20);
            this.mtextBox_Voltage.TabIndex = 5;
            this.mtextBox_Voltage.Text = "5";
            // 
            // mlabel_Voltage
            // 
            this.mlabel_Voltage.AutoSize = true;
            this.mlabel_Voltage.Location = new System.Drawing.Point(159, 13);
            this.mlabel_Voltage.Name = "mlabel_Voltage";
            this.mlabel_Voltage.Size = new System.Drawing.Size(59, 13);
            this.mlabel_Voltage.TabIndex = 6;
            this.mlabel_Voltage.Text = "Voltage (V)";
            // 
            // controlContactClosure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mgroupBox_PulseControls);
            this.Name = "controlContactClosure";
            this.Size = new System.Drawing.Size(293, 69);
            this.Load += new System.EventHandler(this.controlContactClosure_Load);
            this.mgroupBox_PulseControls.ResumeLayout(false);
            this.mgroupBox_PulseControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox mcomboBox_Ports;
        private System.Windows.Forms.TextBox mtextbox_PulseLength;
        private System.Windows.Forms.Button mbutton_SendPulse;
        private System.Windows.Forms.GroupBox mgroupBox_PulseControls;
        private System.Windows.Forms.Label mlabel_PulseLength;
        private System.Windows.Forms.Label mlabel_Port;
        private System.Windows.Forms.Label mlabel_Voltage;
        private System.Windows.Forms.TextBox mtextBox_Voltage;
    }
}
