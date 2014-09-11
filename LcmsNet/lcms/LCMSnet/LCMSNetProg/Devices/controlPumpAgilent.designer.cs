namespace LcmsNet.Devices.Pumps
{
    partial class controlPumpAgilent
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
            this.mbutton_SetFlowRate = new System.Windows.Forms.Button();
            this.mgroupBox_Flow = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mtextBox_ActualFlowRate = new System.Windows.Forms.TextBox();
            this.mbutton_GetFlowRate = new System.Windows.Forms.Button();
            this.mlabel_ulmin = new System.Windows.Forms.Label();
            this.mtextBox_setFlow = new System.Windows.Forms.TextBox();
            this.mgroupbox_Pressure = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextBox_Pressure = new System.Windows.Forms.TextBox();
            this.mbutton_GetPressure = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mtextBox_GetMixerVol = new System.Windows.Forms.TextBox();
            this.mbutton_GetMixerVol = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.mtextBox_SetMixerVol = new System.Windows.Forms.TextBox();
            this.mbutton_SetMixerVol = new System.Windows.Forms.Button();
            this.mGroupBox_Mode = new System.Windows.Forms.GroupBox();
            this.mcomboBox_Mode = new System.Windows.Forms.ComboBox();
            this.mbutton_SetMode = new System.Windows.Forms.Button();
            this.mtabControl = new System.Windows.Forms.TabControl();
            this.mtabPage_PumpControls = new System.Windows.Forms.TabPage();
            this.mtabPage_SerialProps = new System.Windows.Forms.TabPage();
            this.mpropertyGrid_SerialSettings = new System.Windows.Forms.PropertyGrid();
            this.mgroupBox_Flow.SuspendLayout();
            this.mgroupbox_Pressure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mGroupBox_Mode.SuspendLayout();
            this.mtabControl.SuspendLayout();
            this.mtabPage_PumpControls.SuspendLayout();
            this.mtabPage_SerialProps.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbutton_SetFlowRate
            // 
            this.mbutton_SetFlowRate.Location = new System.Drawing.Point(87, 19);
            this.mbutton_SetFlowRate.Name = "mbutton_SetFlowRate";
            this.mbutton_SetFlowRate.Size = new System.Drawing.Size(33, 23);
            this.mbutton_SetFlowRate.TabIndex = 0;
            this.mbutton_SetFlowRate.Text = "Set";
            this.mbutton_SetFlowRate.UseVisualStyleBackColor = true;
            this.mbutton_SetFlowRate.Click += new System.EventHandler(this.mbutton_SetFlowRate_Click);
            // 
            // mgroupBox_Flow
            // 
            this.mgroupBox_Flow.Controls.Add(this.label1);
            this.mgroupBox_Flow.Controls.Add(this.mtextBox_ActualFlowRate);
            this.mgroupBox_Flow.Controls.Add(this.mbutton_GetFlowRate);
            this.mgroupBox_Flow.Controls.Add(this.mlabel_ulmin);
            this.mgroupBox_Flow.Controls.Add(this.mtextBox_setFlow);
            this.mgroupBox_Flow.Controls.Add(this.mbutton_SetFlowRate);
            this.mgroupBox_Flow.Location = new System.Drawing.Point(13, 12);
            this.mgroupBox_Flow.Name = "mgroupBox_Flow";
            this.mgroupBox_Flow.Size = new System.Drawing.Size(128, 76);
            this.mgroupBox_Flow.TabIndex = 1;
            this.mgroupBox_Flow.TabStop = false;
            this.mgroupBox_Flow.Text = "Flow Rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "uL/min";
            // 
            // mtextBox_ActualFlowRate
            // 
            this.mtextBox_ActualFlowRate.Location = new System.Drawing.Point(7, 46);
            this.mtextBox_ActualFlowRate.Name = "mtextBox_ActualFlowRate";
            this.mtextBox_ActualFlowRate.ReadOnly = true;
            this.mtextBox_ActualFlowRate.Size = new System.Drawing.Size(39, 20);
            this.mtextBox_ActualFlowRate.TabIndex = 4;
            // 
            // mbutton_GetFlowRate
            // 
            this.mbutton_GetFlowRate.Location = new System.Drawing.Point(87, 45);
            this.mbutton_GetFlowRate.Name = "mbutton_GetFlowRate";
            this.mbutton_GetFlowRate.Size = new System.Drawing.Size(33, 23);
            this.mbutton_GetFlowRate.TabIndex = 3;
            this.mbutton_GetFlowRate.Text = "Get";
            this.mbutton_GetFlowRate.UseVisualStyleBackColor = true;
            this.mbutton_GetFlowRate.Click += new System.EventHandler(this.mbutton_GetFlowRate_Click);
            // 
            // mlabel_ulmin
            // 
            this.mlabel_ulmin.AutoSize = true;
            this.mlabel_ulmin.Location = new System.Drawing.Point(46, 24);
            this.mlabel_ulmin.Name = "mlabel_ulmin";
            this.mlabel_ulmin.Size = new System.Drawing.Size(40, 13);
            this.mlabel_ulmin.TabIndex = 2;
            this.mlabel_ulmin.Text = "uL/min";
            // 
            // mtextBox_setFlow
            // 
            this.mtextBox_setFlow.Location = new System.Drawing.Point(7, 20);
            this.mtextBox_setFlow.Name = "mtextBox_setFlow";
            this.mtextBox_setFlow.Size = new System.Drawing.Size(39, 20);
            this.mtextBox_setFlow.TabIndex = 1;
            // 
            // mgroupbox_Pressure
            // 
            this.mgroupbox_Pressure.Controls.Add(this.label3);
            this.mgroupbox_Pressure.Controls.Add(this.mtextBox_Pressure);
            this.mgroupbox_Pressure.Controls.Add(this.mbutton_GetPressure);
            this.mgroupbox_Pressure.Location = new System.Drawing.Point(13, 94);
            this.mgroupbox_Pressure.Name = "mgroupbox_Pressure";
            this.mgroupbox_Pressure.Size = new System.Drawing.Size(128, 52);
            this.mgroupbox_Pressure.TabIndex = 6;
            this.mgroupbox_Pressure.TabStop = false;
            this.mgroupbox_Pressure.Text = "Pressure";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "psi";
            // 
            // mtextBox_Pressure
            // 
            this.mtextBox_Pressure.Location = new System.Drawing.Point(7, 20);
            this.mtextBox_Pressure.Name = "mtextBox_Pressure";
            this.mtextBox_Pressure.ReadOnly = true;
            this.mtextBox_Pressure.Size = new System.Drawing.Size(39, 20);
            this.mtextBox_Pressure.TabIndex = 1;
            // 
            // mbutton_GetPressure
            // 
            this.mbutton_GetPressure.Location = new System.Drawing.Point(87, 19);
            this.mbutton_GetPressure.Name = "mbutton_GetPressure";
            this.mbutton_GetPressure.Size = new System.Drawing.Size(33, 23);
            this.mbutton_GetPressure.TabIndex = 0;
            this.mbutton_GetPressure.Text = "Get";
            this.mbutton_GetPressure.UseVisualStyleBackColor = true;
            this.mbutton_GetPressure.Click += new System.EventHandler(this.mbutton_GetPressure_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.mtextBox_GetMixerVol);
            this.groupBox1.Controls.Add(this.mbutton_GetMixerVol);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.mtextBox_SetMixerVol);
            this.groupBox1.Controls.Add(this.mbutton_SetMixerVol);
            this.groupBox1.Location = new System.Drawing.Point(149, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(128, 76);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mixer Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "uL";
            // 
            // mtextBox_GetMixerVol
            // 
            this.mtextBox_GetMixerVol.Location = new System.Drawing.Point(7, 46);
            this.mtextBox_GetMixerVol.Name = "mtextBox_GetMixerVol";
            this.mtextBox_GetMixerVol.ReadOnly = true;
            this.mtextBox_GetMixerVol.Size = new System.Drawing.Size(39, 20);
            this.mtextBox_GetMixerVol.TabIndex = 4;
            // 
            // mbutton_GetMixerVol
            // 
            this.mbutton_GetMixerVol.Location = new System.Drawing.Point(87, 45);
            this.mbutton_GetMixerVol.Name = "mbutton_GetMixerVol";
            this.mbutton_GetMixerVol.Size = new System.Drawing.Size(33, 23);
            this.mbutton_GetMixerVol.TabIndex = 3;
            this.mbutton_GetMixerVol.Text = "Get";
            this.mbutton_GetMixerVol.UseVisualStyleBackColor = true;
            this.mbutton_GetMixerVol.Click += new System.EventHandler(this.mbutton_GetMixerVol_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "uL";
            // 
            // mtextBox_SetMixerVol
            // 
            this.mtextBox_SetMixerVol.Location = new System.Drawing.Point(7, 20);
            this.mtextBox_SetMixerVol.Name = "mtextBox_SetMixerVol";
            this.mtextBox_SetMixerVol.Size = new System.Drawing.Size(39, 20);
            this.mtextBox_SetMixerVol.TabIndex = 1;
            // 
            // mbutton_SetMixerVol
            // 
            this.mbutton_SetMixerVol.Location = new System.Drawing.Point(87, 19);
            this.mbutton_SetMixerVol.Name = "mbutton_SetMixerVol";
            this.mbutton_SetMixerVol.Size = new System.Drawing.Size(33, 23);
            this.mbutton_SetMixerVol.TabIndex = 0;
            this.mbutton_SetMixerVol.Text = "Set";
            this.mbutton_SetMixerVol.UseVisualStyleBackColor = true;
            this.mbutton_SetMixerVol.Click += new System.EventHandler(this.mbutton_SetMixerVol_Click);
            // 
            // mGroupBox_Mode
            // 
            this.mGroupBox_Mode.Controls.Add(this.mcomboBox_Mode);
            this.mGroupBox_Mode.Controls.Add(this.mbutton_SetMode);
            this.mGroupBox_Mode.Location = new System.Drawing.Point(149, 94);
            this.mGroupBox_Mode.Name = "mGroupBox_Mode";
            this.mGroupBox_Mode.Size = new System.Drawing.Size(128, 52);
            this.mGroupBox_Mode.TabIndex = 7;
            this.mGroupBox_Mode.TabStop = false;
            this.mGroupBox_Mode.Text = "Mode";
            // 
            // mcomboBox_Mode
            // 
            this.mcomboBox_Mode.FormattingEnabled = true;
            this.mcomboBox_Mode.Location = new System.Drawing.Point(7, 20);
            this.mcomboBox_Mode.Name = "mcomboBox_Mode";
            this.mcomboBox_Mode.Size = new System.Drawing.Size(74, 21);
            this.mcomboBox_Mode.TabIndex = 1;
            // 
            // mbutton_SetMode
            // 
            this.mbutton_SetMode.Location = new System.Drawing.Point(87, 19);
            this.mbutton_SetMode.Name = "mbutton_SetMode";
            this.mbutton_SetMode.Size = new System.Drawing.Size(33, 23);
            this.mbutton_SetMode.TabIndex = 0;
            this.mbutton_SetMode.Text = "Set";
            this.mbutton_SetMode.UseVisualStyleBackColor = true;
            this.mbutton_SetMode.Click += new System.EventHandler(this.mbutton_SetMode_Click);
            // 
            // mtabControl
            // 
            this.mtabControl.Controls.Add(this.mtabPage_PumpControls);
            this.mtabControl.Controls.Add(this.mtabPage_SerialProps);
            this.mtabControl.Location = new System.Drawing.Point(3, 2);
            this.mtabControl.Name = "mtabControl";
            this.mtabControl.SelectedIndex = 0;
            this.mtabControl.Size = new System.Drawing.Size(292, 240);
            this.mtabControl.TabIndex = 8;
            // 
            // mtabPage_PumpControls
            // 
            this.mtabPage_PumpControls.Controls.Add(this.mgroupbox_Pressure);
            this.mtabPage_PumpControls.Controls.Add(this.groupBox1);
            this.mtabPage_PumpControls.Controls.Add(this.mgroupBox_Flow);
            this.mtabPage_PumpControls.Controls.Add(this.mGroupBox_Mode);
            this.mtabPage_PumpControls.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_PumpControls.Name = "mtabPage_PumpControls";
            this.mtabPage_PumpControls.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_PumpControls.Size = new System.Drawing.Size(284, 214);
            this.mtabPage_PumpControls.TabIndex = 0;
            this.mtabPage_PumpControls.Text = "Pump Controls";
            this.mtabPage_PumpControls.UseVisualStyleBackColor = true;
            // 
            // mtabPage_SerialProps
            // 
            this.mtabPage_SerialProps.Controls.Add(this.mpropertyGrid_SerialSettings);
            this.mtabPage_SerialProps.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_SerialProps.Name = "mtabPage_SerialProps";
            this.mtabPage_SerialProps.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_SerialProps.Size = new System.Drawing.Size(284, 214);
            this.mtabPage_SerialProps.TabIndex = 1;
            this.mtabPage_SerialProps.Text = "Serial Settings";
            this.mtabPage_SerialProps.UseVisualStyleBackColor = true;
            // 
            // mpropertyGrid_SerialSettings
            // 
            this.mpropertyGrid_SerialSettings.Location = new System.Drawing.Point(4, 4);
            this.mpropertyGrid_SerialSettings.Name = "mpropertyGrid_SerialSettings";
            this.mpropertyGrid_SerialSettings.Size = new System.Drawing.Size(276, 204);
            this.mpropertyGrid_SerialSettings.TabIndex = 0;
            // 
            // controlPumpAgilent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mtabControl);
            this.Name = "controlPumpAgilent";
            this.Size = new System.Drawing.Size(298, 307);
            this.Load += new System.EventHandler(this.controlPumpAgilent_Load);
            this.Controls.SetChildIndex(this.mtabControl, 0);
            this.mgroupBox_Flow.ResumeLayout(false);
            this.mgroupBox_Flow.PerformLayout();
            this.mgroupbox_Pressure.ResumeLayout(false);
            this.mgroupbox_Pressure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mGroupBox_Mode.ResumeLayout(false);
            this.mtabControl.ResumeLayout(false);
            this.mtabPage_PumpControls.ResumeLayout(false);
            this.mtabPage_SerialProps.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_SetFlowRate;
        private System.Windows.Forms.GroupBox mgroupBox_Flow;
        private System.Windows.Forms.TextBox mtextBox_setFlow;
        private System.Windows.Forms.Label mlabel_ulmin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mtextBox_ActualFlowRate;
        private System.Windows.Forms.Button mbutton_GetFlowRate;
        private System.Windows.Forms.GroupBox mgroupbox_Pressure;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mtextBox_Pressure;
        private System.Windows.Forms.Button mbutton_GetPressure;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mtextBox_GetMixerVol;
        private System.Windows.Forms.Button mbutton_GetMixerVol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox mtextBox_SetMixerVol;
        private System.Windows.Forms.Button mbutton_SetMixerVol;
        private System.Windows.Forms.GroupBox mGroupBox_Mode;
        private System.Windows.Forms.Button mbutton_SetMode;
        private System.Windows.Forms.ComboBox mcomboBox_Mode;
        private System.Windows.Forms.TabControl mtabControl;
        private System.Windows.Forms.TabPage mtabPage_PumpControls;
        private System.Windows.Forms.TabPage mtabPage_SerialProps;
        private System.Windows.Forms.PropertyGrid mpropertyGrid_SerialSettings;
    }
}

