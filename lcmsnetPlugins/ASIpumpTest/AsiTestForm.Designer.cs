namespace ASIpumpTest
{
    partial class AsiTestForm
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
            ASIpump.AsiPump asiPump1 = new ASIpump.AsiPump();
            this.asiUI1 = new ASIpump.AsiUI();
            this.SuspendLayout();
            // 
            // asiUI1
            // 
            this.asiUI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.asiUI1.Location = new System.Drawing.Point(0, 0);
            this.asiUI1.Name = "asiUI1";
            asiPump1.AbortEvent = null;
            asiPump1.BaudRate = 9600;
            asiPump1.DataBits = 8;
            asiPump1.DtrEnable = false;
            asiPump1.Emulation = false;
            asiPump1.ErrorType = LcmsNetDataClasses.Devices.enumDeviceErrorStatus.ErrorSampleOnly;
            asiPump1.FinalIsoTime = 0D;
            asiPump1.GradientTime = 200D;
            asiPump1.Handshake = System.IO.Ports.Handshake.None;
            asiPump1.InitialIsoTime = 30D;
            asiPump1.MobilePhases = null;
            asiPump1.Name = null;
            asiPump1.Parity = System.IO.Ports.Parity.None;
            asiPump1.PortName = "COM1";
            asiPump1.ReplyDelimeter = "\r\n";
            asiPump1.RtsEnable = false;
            asiPump1.Running = false;
            asiPump1.SendDelimeter = "\r\n";
            asiPump1.StartPercentA = 5D;
            asiPump1.StartPercentB = 95D;
            asiPump1.Status = LcmsNetDataClasses.Devices.enumDeviceStatus.NotInitialized;
            asiPump1.StopBits = System.IO.Ports.StopBits.One;
            asiPump1.Timeout = 1D;
            asiPump1.TotalFlow = 20D;
            asiPump1.TotalMonitoringMinutesDataToKeep = 0;
            asiPump1.TotalMonitoringSecondElapsed = 0;
            asiPump1.Version = null;
            this.asiUI1.Pump = asiPump1;
            this.asiUI1.Size = new System.Drawing.Size(656, 788);
            this.asiUI1.TabIndex = 0;
            this.asiUI1.Load += new System.EventHandler(this.asiUI1_Load);
            // 
            // AsiUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 788);
            this.Controls.Add(this.asiUI1);
            this.Name = "AsiUI";
            this.Text = "ASI 576";
            this.ResumeLayout(false);

        }

        #endregion

        private ASIpump.AsiUI asiUI1;
    }
}

