namespace LcmsNet.Devices
{
    partial class AdvancedDeviceControlPanel
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
            this.m_advancedTabControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // m_advancedTabControl
            // 
            this.m_advancedTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_advancedTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_advancedTabControl.Location = new System.Drawing.Point(0, 0);
            this.m_advancedTabControl.Name = "m_advancedTabControl";
            this.m_advancedTabControl.SelectedIndex = 0;
            this.m_advancedTabControl.Size = new System.Drawing.Size(708, 846);
            this.m_advancedTabControl.TabIndex = 1;
            // 
            // AdvancedDeviceControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.m_advancedTabControl);
            this.Name = "AdvancedDeviceControlPanel";
            this.Size = new System.Drawing.Size(708, 846);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_advancedTabControl;
    }
}
