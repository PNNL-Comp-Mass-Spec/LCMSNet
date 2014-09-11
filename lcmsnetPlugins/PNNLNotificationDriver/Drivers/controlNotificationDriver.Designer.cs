namespace FailureInjector.Drivers
{
    partial class controlNotificationDriver
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
            
            this.SuspendLayout();
            // 
            // mbutton_injectFailure
            // 
            this.mbutton_injectFailure.Location = new System.Drawing.Point(46, 36);
            this.mbutton_injectFailure.Name = "mbutton_injectFailure";
            this.mbutton_injectFailure.Size = new System.Drawing.Size(172, 58);
            this.mbutton_injectFailure.TabIndex = 0;
            this.mbutton_injectFailure.Text = "Inject Failure";
            this.mbutton_injectFailure.UseVisualStyleBackColor = true;
            this.mbutton_injectFailure.Click += new System.EventHandler(this.mbutton_injectFailure_Click);
            // 
            // controlNotificationDriver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mbutton_injectFailure);
            this.Name = "controlNotificationDriver";
            this.Size = new System.Drawing.Size(262, 225);
            this.Controls.SetChildIndex(this.mbutton_injectFailure, 0);
            
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_injectFailure;
    }
}
