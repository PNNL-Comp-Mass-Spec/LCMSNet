namespace LcmsNet
{
    partial class ModelCheckListControl
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
            this.groupBoxModelChecks = new System.Windows.Forms.GroupBox();
            this.enableAllModelChecks = new System.Windows.Forms.CheckBox();
            this.panelEnableAll = new System.Windows.Forms.Panel();
            this.panelCheckBoxes = new System.Windows.Forms.Panel();
            this.groupBoxModelChecks.SuspendLayout();
            this.panelEnableAll.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxModelChecks
            // 
            this.groupBoxModelChecks.AutoSize = true;
            this.groupBoxModelChecks.Controls.Add(this.panelCheckBoxes);
            this.groupBoxModelChecks.Controls.Add(this.panelEnableAll);
            this.groupBoxModelChecks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxModelChecks.Location = new System.Drawing.Point(0, 0);
            this.groupBoxModelChecks.Name = "groupBoxModelChecks";
            this.groupBoxModelChecks.Size = new System.Drawing.Size(460, 245);
            this.groupBoxModelChecks.TabIndex = 0;
            this.groupBoxModelChecks.TabStop = false;
            this.groupBoxModelChecks.Text = "Model Checks";
            // 
            // enableAllModelChecks
            // 
            this.enableAllModelChecks.AutoSize = true;
            this.enableAllModelChecks.Dock = System.Windows.Forms.DockStyle.Left;
            this.enableAllModelChecks.Location = new System.Drawing.Point(0, 0);
            this.enableAllModelChecks.Name = "enableAllModelChecks";
            this.enableAllModelChecks.Size = new System.Drawing.Size(73, 32);
            this.enableAllModelChecks.TabIndex = 0;
            this.enableAllModelChecks.Text = "Enable All";
            this.enableAllModelChecks.UseVisualStyleBackColor = true;
            this.enableAllModelChecks.CheckedChanged += new System.EventHandler(this.EnableAllCheckHandler);
            // 
            // panelEnableAll
            // 
            this.panelEnableAll.Controls.Add(this.enableAllModelChecks);
            this.panelEnableAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEnableAll.Location = new System.Drawing.Point(3, 16);
            this.panelEnableAll.Name = "panelEnableAll";
            this.panelEnableAll.Size = new System.Drawing.Size(454, 32);
            this.panelEnableAll.TabIndex = 1;
            // 
            // panelCheckBoxes
            // 
            this.panelCheckBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCheckBoxes.Location = new System.Drawing.Point(3, 48);
            this.panelCheckBoxes.Name = "panelCheckBoxes";
            this.panelCheckBoxes.Size = new System.Drawing.Size(454, 194);
            this.panelCheckBoxes.TabIndex = 2;
            // 
            // ModelCheckListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.groupBoxModelChecks);
            this.Name = "ModelCheckListControl";
            this.Size = new System.Drawing.Size(460, 245);
            this.groupBoxModelChecks.ResumeLayout(false);
            this.panelEnableAll.ResumeLayout(false);
            this.panelEnableAll.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxModelChecks;
        private System.Windows.Forms.CheckBox enableAllModelChecks;
        private System.Windows.Forms.Panel panelEnableAll;
        private System.Windows.Forms.Panel panelCheckBoxes;
    }
}
