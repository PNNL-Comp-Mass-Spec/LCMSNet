namespace ASUTGen.Devices.Valves
{
    partial class IDEXValveControl
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
            this.mbutton_injectFailure = new System.Windows.Forms.Button();
            this.mbutton_injectStatus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mbutton_injectFailure
            // 
            this.mbutton_injectFailure.Location = new System.Drawing.Point(34, 20);
            this.mbutton_injectFailure.Name = "mbutton_injectFailure";
            this.mbutton_injectFailure.Size = new System.Drawing.Size(172, 58);
            this.mbutton_injectFailure.TabIndex = 0;
            this.mbutton_injectFailure.Text = "Inject Failure";
            this.mbutton_injectFailure.UseVisualStyleBackColor = true;
            this.mbutton_injectFailure.Click += new System.EventHandler(this.mbutton_injectFailure_Click);
            // 
            // mbutton_injectStatus
            // 
            this.mbutton_injectStatus.Location = new System.Drawing.Point(34, 93);
            this.mbutton_injectStatus.Name = "mbutton_injectStatus";
            this.mbutton_injectStatus.Size = new System.Drawing.Size(172, 58);
            this.mbutton_injectStatus.TabIndex = 1;
            this.mbutton_injectStatus.Text = "Inject Status";
            this.mbutton_injectStatus.UseVisualStyleBackColor = true;
            // 
            // controlNotificationDriver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mbutton_injectStatus);
            this.Controls.Add(this.mbutton_injectFailure);
            this.Name = "controlNotificationDriver";
            this.Size = new System.Drawing.Size(241, 172);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_injectFailure;
        private System.Windows.Forms.Button mbutton_injectStatus;
    }
}
