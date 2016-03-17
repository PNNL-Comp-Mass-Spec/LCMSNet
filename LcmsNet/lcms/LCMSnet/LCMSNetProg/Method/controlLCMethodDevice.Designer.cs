namespace LcmsNet.Method
{
    partial class controlLCMethodDevice
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
            this.mcomboBox_parameters = new System.Windows.Forms.ComboBox();
            this.mcomboBox_method = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            //
            // mcomboBox_parameters
            //
            this.mcomboBox_parameters.FormattingEnabled = true;
            this.mcomboBox_parameters.Location = new System.Drawing.Point(156, 3);
            this.mcomboBox_parameters.Name = "mcomboBox_parameters";
            this.mcomboBox_parameters.Size = new System.Drawing.Size(147, 21);
            this.mcomboBox_parameters.TabIndex = 6;
            //
            // mcomboBox_method
            //
            this.mcomboBox_method.FormattingEnabled = true;
            this.mcomboBox_method.Location = new System.Drawing.Point(3, 3);
            this.mcomboBox_method.Name = "mcomboBox_method";
            this.mcomboBox_method.Size = new System.Drawing.Size(147, 21);
            this.mcomboBox_method.TabIndex = 5;
            //
            // controlLCMethodDevice
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mcomboBox_parameters);
            this.Controls.Add(this.mcomboBox_method);
            this.Name = "controlLCMethodDevice";
            this.Size = new System.Drawing.Size(310, 29);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox mcomboBox_parameters;
        private System.Windows.Forms.ComboBox mcomboBox_method;
    }
}
