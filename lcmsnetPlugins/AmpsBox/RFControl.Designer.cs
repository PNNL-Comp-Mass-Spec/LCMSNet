namespace AmpsBox
{
    partial class RFControl
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
            this.m_driveLevelControl = new AmpsBox.SingleElementControl();
            this.m_outputVoltageControl = new AmpsBox.SingleElementControl();
            this.m_rfControl = new AmpsBox.SingleElementControl();
            this.SuspendLayout();
            // 
            // m_driveLevelControl
            // 
            this.m_driveLevelControl.BackColor = System.Drawing.Color.White;
            this.m_driveLevelControl.Data = null;
            this.m_driveLevelControl.DisplayName = "Drive Level";
            this.m_driveLevelControl.Location = new System.Drawing.Point(3, 119);
            this.m_driveLevelControl.Name = "m_driveLevelControl";
            this.m_driveLevelControl.Size = new System.Drawing.Size(232, 118);
            this.m_driveLevelControl.TabIndex = 2;
            // 
            // m_outputVoltageControl
            // 
            this.m_outputVoltageControl.BackColor = System.Drawing.Color.White;
            this.m_outputVoltageControl.Data = null;
            this.m_outputVoltageControl.DisplayName = "Output Voltage";
            this.m_outputVoltageControl.Location = new System.Drawing.Point(3, 233);
            this.m_outputVoltageControl.Name = "m_outputVoltageControl";
            this.m_outputVoltageControl.Size = new System.Drawing.Size(232, 118);
            this.m_outputVoltageControl.TabIndex = 1;
            // 
            // m_rfControl
            // 
            this.m_rfControl.BackColor = System.Drawing.Color.White;
            this.m_rfControl.Data = null;
            this.m_rfControl.DisplayName = "RF Frequency";
            this.m_rfControl.Location = new System.Drawing.Point(0, 3);
            this.m_rfControl.Name = "m_rfControl";
            this.m_rfControl.Size = new System.Drawing.Size(235, 121);
            this.m_rfControl.TabIndex = 0;
            // 
            // RFControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.m_driveLevelControl);
            this.Controls.Add(this.m_outputVoltageControl);
            this.Controls.Add(this.m_rfControl);
            this.Name = "RFControl";
            this.Size = new System.Drawing.Size(242, 354);
            this.ResumeLayout(false);

        }

        #endregion

        private SingleElementControl m_rfControl;
        private SingleElementControl m_outputVoltageControl;
        private SingleElementControl m_driveLevelControl;

    }
}
