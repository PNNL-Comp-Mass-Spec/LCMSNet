namespace LCMSNetPlugins
{
    partial class PluginTestObjectGlyph
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
            this.mItemNameLabel = new System.Windows.Forms.Label();
            this.mDeviceToStringDisplay = new System.Windows.Forms.TextBox();
            this.mDeviceNameLabel = new System.Windows.Forms.Label();
            this.mDeviceNameDisplay = new System.Windows.Forms.TextBox();
            this.mRefreshButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mItemNameLabel
            // 
            this.mItemNameLabel.AutoSize = true;
            this.mItemNameLabel.Location = new System.Drawing.Point(88, 12);
            this.mItemNameLabel.Name = "mItemNameLabel";
            this.mItemNameLabel.Size = new System.Drawing.Size(58, 13);
            this.mItemNameLabel.TabIndex = 5;
            this.mItemNameLabel.Text = "Item Name";
            // 
            // mDeviceToStringDisplay
            // 
            this.mDeviceToStringDisplay.Location = new System.Drawing.Point(16, 76);
            this.mDeviceToStringDisplay.Multiline = true;
            this.mDeviceToStringDisplay.Name = "mDeviceToStringDisplay";
            this.mDeviceToStringDisplay.ReadOnly = true;
            this.mDeviceToStringDisplay.Size = new System.Drawing.Size(213, 74);
            this.mDeviceToStringDisplay.TabIndex = 16;
            // 
            // mDeviceNameLabel
            // 
            this.mDeviceNameLabel.AutoSize = true;
            this.mDeviceNameLabel.Location = new System.Drawing.Point(81, 32);
            this.mDeviceNameLabel.Name = "mDeviceNameLabel";
            this.mDeviceNameLabel.Size = new System.Drawing.Size(72, 13);
            this.mDeviceNameLabel.TabIndex = 15;
            this.mDeviceNameLabel.Text = "Device Name";
            // 
            // mDeviceNameDisplay
            // 
            this.mDeviceNameDisplay.Location = new System.Drawing.Point(16, 48);
            this.mDeviceNameDisplay.Name = "mDeviceNameDisplay";
            this.mDeviceNameDisplay.ReadOnly = true;
            this.mDeviceNameDisplay.Size = new System.Drawing.Size(213, 20);
            this.mDeviceNameDisplay.TabIndex = 14;
            // 
            // mRefreshButton
            // 
            this.mRefreshButton.Location = new System.Drawing.Point(16, 156);
            this.mRefreshButton.Name = "mRefreshButton";
            this.mRefreshButton.Size = new System.Drawing.Size(213, 27);
            this.mRefreshButton.TabIndex = 13;
            this.mRefreshButton.Text = "Refresh";
            this.mRefreshButton.UseVisualStyleBackColor = true;
            this.mRefreshButton.Click += new System.EventHandler(this.mRefreshButton_Click);
            // 
            // ItemGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mDeviceToStringDisplay);
            this.Controls.Add(this.mDeviceNameLabel);
            this.Controls.Add(this.mDeviceNameDisplay);
            this.Controls.Add(this.mRefreshButton);
            this.Controls.Add(this.mItemNameLabel);
            this.Name = "ItemGlyph";
            this.Size = new System.Drawing.Size(244, 197);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mItemNameLabel;
        private System.Windows.Forms.TextBox mDeviceToStringDisplay;
        private System.Windows.Forms.Label mDeviceNameLabel;
        private System.Windows.Forms.TextBox mDeviceNameDisplay;
        private System.Windows.Forms.Button mRefreshButton;


    }
}
