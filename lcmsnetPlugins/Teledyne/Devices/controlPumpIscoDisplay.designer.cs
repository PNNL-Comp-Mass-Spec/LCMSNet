namespace LcmsNet.Devices.Pumps
{
	partial class controlPumpIscoDisplay
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
            this.mlabel_PumpName = new System.Windows.Forms.Label();
            this.mbutton_Refill = new System.Windows.Forms.Button();
            this.mbutton_StopPump = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.mtextBox_ActualPressure = new System.Windows.Forms.TextBox();
            this.mlabel_SetpontUnits = new System.Windows.Forms.Label();
            this.mtextBox_ActualFlow = new System.Windows.Forms.TextBox();
            this.mbutton_StartPump = new System.Windows.Forms.Button();
            this.mbutton_ChangeSetpoint = new System.Windows.Forms.Button();
            this.mtextBox_Setpoint = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mtextBox_ActualVolume = new System.Windows.Forms.TextBox();
            this.mlabel_ProbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mlabel_PumpName
            // 
            this.mlabel_PumpName.AutoSize = true;
            this.mlabel_PumpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_PumpName.Location = new System.Drawing.Point(166, 5);
            this.mlabel_PumpName.Name = "mlabel_PumpName";
            this.mlabel_PumpName.Size = new System.Drawing.Size(50, 13);
            this.mlabel_PumpName.TabIndex = 0;
            this.mlabel_PumpName.Text = "Pump X";
            // 
            // mbutton_Refill
            // 
            this.mbutton_Refill.Location = new System.Drawing.Point(314, 72);
            this.mbutton_Refill.Name = "mbutton_Refill";
            this.mbutton_Refill.Size = new System.Drawing.Size(57, 23);
            this.mbutton_Refill.TabIndex = 25;
            this.mbutton_Refill.Text = "Refill";
            this.mbutton_Refill.UseVisualStyleBackColor = true;
            this.mbutton_Refill.Click += new System.EventHandler(this.Refill_Clicked);
            // 
            // mbutton_StopPump
            // 
            this.mbutton_StopPump.Location = new System.Drawing.Point(314, 43);
            this.mbutton_StopPump.Name = "mbutton_StopPump";
            this.mbutton_StopPump.Size = new System.Drawing.Size(57, 23);
            this.mbutton_StopPump.TabIndex = 24;
            this.mbutton_StopPump.Text = "Stop";
            this.mbutton_StopPump.UseVisualStyleBackColor = true;
            this.mbutton_StopPump.Click += new System.EventHandler(this.StopPump_Clicked);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Flow";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Press";
            // 
            // mtextBox_ActualPressure
            // 
            this.mtextBox_ActualPressure.Location = new System.Drawing.Point(49, 49);
            this.mtextBox_ActualPressure.Name = "mtextBox_ActualPressure";
            this.mtextBox_ActualPressure.ReadOnly = true;
            this.mtextBox_ActualPressure.Size = new System.Drawing.Size(65, 20);
            this.mtextBox_ActualPressure.TabIndex = 21;
            this.mtextBox_ActualPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mlabel_SetpontUnits
            // 
            this.mlabel_SetpontUnits.AutoSize = true;
            this.mlabel_SetpontUnits.Location = new System.Drawing.Point(242, 27);
            this.mlabel_SetpontUnits.Name = "mlabel_SetpontUnits";
            this.mlabel_SetpontUnits.Size = new System.Drawing.Size(24, 13);
            this.mlabel_SetpontUnits.TabIndex = 19;
            this.mlabel_SetpontUnits.Text = "PSI";
            // 
            // mtextBox_ActualFlow
            // 
            this.mtextBox_ActualFlow.Location = new System.Drawing.Point(49, 21);
            this.mtextBox_ActualFlow.Name = "mtextBox_ActualFlow";
            this.mtextBox_ActualFlow.ReadOnly = true;
            this.mtextBox_ActualFlow.Size = new System.Drawing.Size(65, 20);
            this.mtextBox_ActualFlow.TabIndex = 18;
            this.mtextBox_ActualFlow.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_StartPump
            // 
            this.mbutton_StartPump.Location = new System.Drawing.Point(314, 14);
            this.mbutton_StartPump.Name = "mbutton_StartPump";
            this.mbutton_StartPump.Size = new System.Drawing.Size(57, 23);
            this.mbutton_StartPump.TabIndex = 17;
            this.mbutton_StartPump.Text = "Start";
            this.mbutton_StartPump.UseVisualStyleBackColor = true;
            this.mbutton_StartPump.Click += new System.EventHandler(this.StartPump_Clicked);
            // 
            // mbutton_ChangeSetpoint
            // 
            this.mbutton_ChangeSetpoint.Location = new System.Drawing.Point(153, 51);
            this.mbutton_ChangeSetpoint.Name = "mbutton_ChangeSetpoint";
            this.mbutton_ChangeSetpoint.Size = new System.Drawing.Size(79, 23);
            this.mbutton_ChangeSetpoint.TabIndex = 16;
            this.mbutton_ChangeSetpoint.Text = "Set Press";
            this.mbutton_ChangeSetpoint.UseVisualStyleBackColor = true;
            this.mbutton_ChangeSetpoint.Click += new System.EventHandler(this.mbutton_ChangeSetpoint_Clicked);
            // 
            // mtextBox_Setpoint
            // 
            this.mtextBox_Setpoint.Location = new System.Drawing.Point(153, 24);
            this.mtextBox_Setpoint.Name = "mtextBox_Setpoint";
            this.mtextBox_Setpoint.Size = new System.Drawing.Size(79, 20);
            this.mtextBox_Setpoint.TabIndex = 26;
            this.mtextBox_Setpoint.Text = "0.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Vol";
            // 
            // mtextBox_ActualVolume
            // 
            this.mtextBox_ActualVolume.Location = new System.Drawing.Point(49, 77);
            this.mtextBox_ActualVolume.Name = "mtextBox_ActualVolume";
            this.mtextBox_ActualVolume.ReadOnly = true;
            this.mtextBox_ActualVolume.Size = new System.Drawing.Size(65, 20);
            this.mtextBox_ActualVolume.TabIndex = 27;
            this.mtextBox_ActualVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mlabel_ProbStatus
            // 
            this.mlabel_ProbStatus.ForeColor = System.Drawing.Color.Red;
            this.mlabel_ProbStatus.Location = new System.Drawing.Point(160, 80);
            this.mlabel_ProbStatus.Name = "mlabel_ProbStatus";
            this.mlabel_ProbStatus.Size = new System.Drawing.Size(65, 20);
            this.mlabel_ProbStatus.TabIndex = 29;
            this.mlabel_ProbStatus.Text = "NA";
            this.mlabel_ProbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPumpIscoDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mlabel_ProbStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mtextBox_ActualVolume);
            this.Controls.Add(this.mtextBox_Setpoint);
            this.Controls.Add(this.mbutton_Refill);
            this.Controls.Add(this.mbutton_StopPump);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.mtextBox_ActualPressure);
            this.Controls.Add(this.mlabel_SetpontUnits);
            this.Controls.Add(this.mtextBox_ActualFlow);
            this.Controls.Add(this.mbutton_StartPump);
            this.Controls.Add(this.mbutton_ChangeSetpoint);
            this.Controls.Add(this.mlabel_PumpName);
            this.Name = "controlPumpIscoDisplay";
            this.Size = new System.Drawing.Size(384, 102);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mlabel_PumpName;
		private System.Windows.Forms.Button mbutton_Refill;
		private System.Windows.Forms.Button mbutton_StopPump;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox mtextBox_ActualPressure;
		private System.Windows.Forms.Label mlabel_SetpontUnits;
		private System.Windows.Forms.TextBox mtextBox_ActualFlow;
		private System.Windows.Forms.Button mbutton_StartPump;
		private System.Windows.Forms.Button mbutton_ChangeSetpoint;
		private System.Windows.Forms.TextBox mtextBox_Setpoint;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox mtextBox_ActualVolume;
        private System.Windows.Forms.Label mlabel_ProbStatus;
	}
}
