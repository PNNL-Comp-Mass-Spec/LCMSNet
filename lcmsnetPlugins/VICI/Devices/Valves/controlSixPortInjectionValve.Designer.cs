namespace LcmsNet.Devices.Valves
{
    partial class controlSixPortInjectionValve
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
            this.mgroupBox_Inj_Volume = new System.Windows.Forms.GroupBox();
            this.btnSetInjectionVolume = new System.Windows.Forms.Button();
            this.txtInjectionVolume = new System.Windows.Forms.TextBox();
            this.mtab_ControlValve.SuspendLayout();
            this.mtabPage_ValveControl.SuspendLayout();
            this.mgroupBox_currentPos.SuspendLayout();
            this.mgroupBox_setPos.SuspendLayout();
            this.mgroupbox_Version.SuspendLayout();
            this.mgroupBox_Inj_Volume.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtabPage_ValveControl
            // 
            this.mtabPage_ValveControl.Controls.Add(this.mgroupBox_Inj_Volume);
            this.mtabPage_ValveControl.Controls.SetChildIndex(this.mgroupBox_Inj_Volume, 0);
            this.mtabPage_ValveControl.Controls.SetChildIndex(this.mgroupbox_Version, 0);
            this.mtabPage_ValveControl.Controls.SetChildIndex(this.mgroupBox_setPos, 0);
            this.mtabPage_ValveControl.Controls.SetChildIndex(this.mgroupBox_currentPos, 0);
            // 
            // mgroupbox_Version
            // 
            this.mgroupbox_Version.Location = new System.Drawing.Point(6, 186);
            this.mgroupbox_Version.Size = new System.Drawing.Size(251, 169);
            // 
            // mgroupBox_Inj_Volume
            // 
            this.mgroupBox_Inj_Volume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_Inj_Volume.Controls.Add(this.btnSetInjectionVolume);
            this.mgroupBox_Inj_Volume.Controls.Add(this.txtInjectionVolume);
            this.mgroupBox_Inj_Volume.Location = new System.Drawing.Point(155, 118);
            this.mgroupBox_Inj_Volume.Name = "mgroupBox_Inj_Volume";
            this.mgroupBox_Inj_Volume.Size = new System.Drawing.Size(99, 72);
            this.mgroupBox_Inj_Volume.TabIndex = 13;
            this.mgroupBox_Inj_Volume.TabStop = false;
            this.mgroupBox_Inj_Volume.Text = "Injection Volume";
            // 
            // btnSetInjectionVolume
            // 
            this.btnSetInjectionVolume.Location = new System.Drawing.Point(15, 45);
            this.btnSetInjectionVolume.Name = "btnSetInjectionVolume";
            this.btnSetInjectionVolume.Size = new System.Drawing.Size(75, 23);
            this.btnSetInjectionVolume.TabIndex = 1;
            this.btnSetInjectionVolume.Text = "Set";
            this.btnSetInjectionVolume.UseVisualStyleBackColor = true;
            this.btnSetInjectionVolume.Click += new System.EventHandler(this.btnSetInjectionVolume_Click);
            // 
            // txtInjectionVolume
            // 
            this.txtInjectionVolume.Location = new System.Drawing.Point(6, 19);
            this.txtInjectionVolume.Name = "txtInjectionVolume";
            this.txtInjectionVolume.Size = new System.Drawing.Size(87, 20);
            this.txtInjectionVolume.TabIndex = 0;
            // 
            // controlSixPortInjectionValve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "controlSixPortInjectionValve";
            this.mtab_ControlValve.ResumeLayout(false);
            this.mtabPage_ValveControl.ResumeLayout(false);
            this.mgroupBox_currentPos.ResumeLayout(false);
            this.mgroupBox_currentPos.PerformLayout();
            this.mgroupBox_setPos.ResumeLayout(false);
            this.mgroupbox_Version.ResumeLayout(false);
            this.mgroupbox_Version.PerformLayout();
            this.mgroupBox_Inj_Volume.ResumeLayout(false);
            this.mgroupBox_Inj_Volume.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox mgroupBox_Inj_Volume;
        private System.Windows.Forms.Button btnSetInjectionVolume;
        private System.Windows.Forms.TextBox txtInjectionVolume;

    }
}
