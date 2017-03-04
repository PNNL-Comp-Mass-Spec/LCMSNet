//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
//*********************************************************************************************************

using LcmsNet.Devices.ContactClosure;

namespace LcmsNet.Devices.ContactClosure
{
    partial class controlContactClosureU3
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
            this.mbutton_SendPulse = new System.Windows.Forms.Button();
            this.mgroupBox_PulseControls = new System.Windows.Forms.GroupBox();
            this.mnum_voltage = new System.Windows.Forms.NumericUpDown();
            this.mnum_pulseLength = new System.Windows.Forms.NumericUpDown();
            this.mlabel_Voltage = new System.Windows.Forms.Label();
            this.mlabel_PulseLength = new System.Windows.Forms.Label();
            this.mlabel_Port = new System.Windows.Forms.Label();
            this.mgroupBox_PulseControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_voltage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pulseLength)).BeginInit();
            this.SuspendLayout();
            // 
            // mcomboBox_Ports
            // 
            this.mcomboBox_Ports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_Ports.FormattingEnabled = true;
            this.mcomboBox_Ports.Items.AddRange(new object[] {
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.DAC0Analog,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.DAC1Analog,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO0Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO1Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO2Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO3Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO4Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO5Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO6Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.FIO7Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO0Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO1Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO2Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO3Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO4Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO5Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO6Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.EIO7Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.CIO0Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.CIO1Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.CIO2Digital,
            LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts.CIO3Digital});
            this.mcomboBox_Ports.Location = new System.Drawing.Point(12, 29);
            this.mcomboBox_Ports.Name = "mcomboBox_Ports";
            this.mcomboBox_Ports.Size = new System.Drawing.Size(81, 21);
            this.mcomboBox_Ports.TabIndex = 0;
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
            this.mgroupBox_PulseControls.Controls.Add(this.mnum_voltage);
            this.mgroupBox_PulseControls.Controls.Add(this.mnum_pulseLength);
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_Voltage);
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_PulseLength);
            this.mgroupBox_PulseControls.Controls.Add(this.mlabel_Port);
            this.mgroupBox_PulseControls.Controls.Add(this.mcomboBox_Ports);
            this.mgroupBox_PulseControls.Controls.Add(this.mbutton_SendPulse);
            this.mgroupBox_PulseControls.Location = new System.Drawing.Point(3, 3);
            this.mgroupBox_PulseControls.Name = "mgroupBox_PulseControls";
            this.mgroupBox_PulseControls.Size = new System.Drawing.Size(285, 59);
            this.mgroupBox_PulseControls.TabIndex = 3;
            this.mgroupBox_PulseControls.TabStop = false;
            // 
            // mnum_voltage
            // 
            this.mnum_voltage.DecimalPlaces = 1;
            this.mnum_voltage.Location = new System.Drawing.Point(160, 29);
            this.mnum_voltage.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mnum_voltage.Name = "mnum_voltage";
            this.mnum_voltage.Size = new System.Drawing.Size(56, 20);
            this.mnum_voltage.TabIndex = 8;
            // 
            // mnum_pulseLength
            // 
            this.mnum_pulseLength.Location = new System.Drawing.Point(99, 29);
            this.mnum_pulseLength.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.mnum_pulseLength.Name = "mnum_pulseLength";
            this.mnum_pulseLength.Size = new System.Drawing.Size(56, 20);
            this.mnum_pulseLength.TabIndex = 7;
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
            // mlabel_PulseLength
            // 
            this.mlabel_PulseLength.AutoSize = true;
            this.mlabel_PulseLength.Location = new System.Drawing.Point(94, 13);
            this.mlabel_PulseLength.Name = "mlabel_PulseLength";
            this.mlabel_PulseLength.Size = new System.Drawing.Size(54, 13);
            this.mlabel_PulseLength.TabIndex = 4;
            this.mlabel_PulseLength.Text = "Length (s)";
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
            // controlContactClosureU3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mgroupBox_PulseControls);
            this.Name = "controlContactClosureU3";
            this.Size = new System.Drawing.Size(305, 77);
            this.mgroupBox_PulseControls.ResumeLayout(false);
            this.mgroupBox_PulseControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_voltage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pulseLength)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox mcomboBox_Ports;
        private System.Windows.Forms.Button mbutton_SendPulse;
        private System.Windows.Forms.GroupBox mgroupBox_PulseControls;
        private System.Windows.Forms.Label mlabel_PulseLength;
        private System.Windows.Forms.Label mlabel_Port;
        private System.Windows.Forms.Label mlabel_Voltage;
        private System.Windows.Forms.NumericUpDown mnum_pulseLength;
        private System.Windows.Forms.NumericUpDown mnum_voltage;
    }
}
