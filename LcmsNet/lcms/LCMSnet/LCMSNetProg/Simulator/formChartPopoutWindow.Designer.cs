namespace LcmsNet.Simulator
{
    partial class formChartPopoutWindow
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(formChartPopoutWindow));
            this.panelControl = new System.Windows.Forms.Panel();
            this.btnTack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // panelControl
            //
            this.panelControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl.Location = new System.Drawing.Point(1, -1);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(458, 288);
            this.panelControl.TabIndex = 0;
            //
            // btnTack
            //
            this.btnTack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTack.Location = new System.Drawing.Point(423, 293);
            this.btnTack.Name = "btnTack";
            this.btnTack.Size = new System.Drawing.Size(36, 23);
            this.btnTack.TabIndex = 1;
            this.btnTack.UseVisualStyleBackColor = true;
            this.btnTack.Click += new System.EventHandler(this.btnTack_OnClick);
            //
            // formChartPopoutWindow
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 318);
            this.ControlBox = false;
            this.Controls.Add(this.btnTack);
            this.Controls.Add(this.panelControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formChartPopoutWindow";
            this.Text = "formChartPopoutWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Button btnTack;
    }
}