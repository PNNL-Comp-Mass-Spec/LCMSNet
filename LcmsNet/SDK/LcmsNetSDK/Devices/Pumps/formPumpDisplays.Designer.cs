namespace LcmsNetDataClasses.Devices.Pumps
{
    partial class formPumpDisplays
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
            this.mpanel_control = new System.Windows.Forms.Panel();
            this.mbutton_expand = new System.Windows.Forms.Button();
            this.mpanel_pumps = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mobilePhaseTab = new System.Windows.Forms.TabPage();
            this.mbutton_right = new System.Windows.Forms.Button();
            this.mbutton_left = new System.Windows.Forms.Button();
            this.mpanel_mobilePhase = new System.Windows.Forms.Panel();
            this.mlabel_pump = new System.Windows.Forms.Label();
            this.mpanel_control.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.mobilePhaseTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpanel_control
            // 
            this.mpanel_control.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpanel_control.Controls.Add(this.mbutton_expand);
            this.mpanel_control.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_control.Location = new System.Drawing.Point(0, 522);
            this.mpanel_control.Name = "mpanel_control";
            this.mpanel_control.Size = new System.Drawing.Size(598, 24);
            this.mpanel_control.TabIndex = 0;
            // 
            // mbutton_expand
            // 
            this.mbutton_expand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_expand.Location = new System.Drawing.Point(553, 0);
            this.mbutton_expand.Name = "mbutton_expand";
            this.mbutton_expand.Size = new System.Drawing.Size(42, 21);
            this.mbutton_expand.TabIndex = 0;
            this.mbutton_expand.UseVisualStyleBackColor = true;
            this.mbutton_expand.Click += new System.EventHandler(this.mbutton_expand_Click);
            // 
            // mpanel_pumps
            // 
            this.mpanel_pumps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_pumps.Location = new System.Drawing.Point(3, 3);
            this.mpanel_pumps.Name = "mpanel_pumps";
            this.mpanel_pumps.Size = new System.Drawing.Size(584, 483);
            this.mpanel_pumps.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.mobilePhaseTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(598, 522);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.mpanel_pumps);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(590, 489);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Pump Status";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mobilePhaseTab
            // 
            this.mobilePhaseTab.AutoScroll = true;
            this.mobilePhaseTab.Controls.Add(this.mlabel_pump);
            this.mobilePhaseTab.Controls.Add(this.mbutton_right);
            this.mobilePhaseTab.Controls.Add(this.mbutton_left);
            this.mobilePhaseTab.Controls.Add(this.mpanel_mobilePhase);
            this.mobilePhaseTab.Location = new System.Drawing.Point(4, 29);
            this.mobilePhaseTab.Name = "mobilePhaseTab";
            this.mobilePhaseTab.Padding = new System.Windows.Forms.Padding(3);
            this.mobilePhaseTab.Size = new System.Drawing.Size(590, 489);
            this.mobilePhaseTab.TabIndex = 1;
            this.mobilePhaseTab.Text = "Mobile Phases";
            this.mobilePhaseTab.UseVisualStyleBackColor = true;
            // 
            // mbutton_right
            // 
            this.mbutton_right.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mbutton_right.FlatAppearance.BorderSize = 0;
            this.mbutton_right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mbutton_right.Image = global::LcmsNetSDK.Properties.Resources.next_32;
            this.mbutton_right.Location = new System.Drawing.Point(541, 241);
            this.mbutton_right.Name = "mbutton_right";
            this.mbutton_right.Size = new System.Drawing.Size(46, 58);
            this.mbutton_right.TabIndex = 3;
            this.mbutton_right.UseVisualStyleBackColor = true;
            this.mbutton_right.Click += new System.EventHandler(this.mbutton_right_Click);
            // 
            // mbutton_left
            // 
            this.mbutton_left.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mbutton_left.FlatAppearance.BorderSize = 0;
            this.mbutton_left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mbutton_left.Image = global::LcmsNetSDK.Properties.Resources.next_32_2;
            this.mbutton_left.Location = new System.Drawing.Point(8, 241);
            this.mbutton_left.Name = "mbutton_left";
            this.mbutton_left.Size = new System.Drawing.Size(46, 58);
            this.mbutton_left.TabIndex = 2;
            this.mbutton_left.UseVisualStyleBackColor = true;
            this.mbutton_left.Click += new System.EventHandler(this.mbutton_left_Click);
            // 
            // mpanel_mobilePhase
            // 
            this.mpanel_mobilePhase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_mobilePhase.AutoScroll = true;
            this.mpanel_mobilePhase.Location = new System.Drawing.Point(60, 50);
            this.mpanel_mobilePhase.Name = "mpanel_mobilePhase";
            this.mpanel_mobilePhase.Size = new System.Drawing.Size(475, 426);
            this.mpanel_mobilePhase.TabIndex = 0;
            // 
            // mlabel_pump
            // 
            this.mlabel_pump.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_pump.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_pump.ForeColor = System.Drawing.Color.DarkRed;
            this.mlabel_pump.Location = new System.Drawing.Point(60, 10);
            this.mlabel_pump.Name = "mlabel_pump";
            this.mlabel_pump.Size = new System.Drawing.Size(475, 37);
            this.mlabel_pump.TabIndex = 4;
            this.mlabel_pump.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // formPumpDisplays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(598, 546);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mpanel_control);
            this.Name = "formPumpDisplays";
            this.Text = "Pump Display";
            this.mpanel_control.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.mobilePhaseTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mpanel_control;
        private System.Windows.Forms.Button mbutton_expand;
        private System.Windows.Forms.Panel mpanel_pumps;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage mobilePhaseTab;
        private System.Windows.Forms.Panel mpanel_mobilePhase;
        private System.Windows.Forms.Button mbutton_left;
        private System.Windows.Forms.Button mbutton_right;
        private System.Windows.Forms.Label mlabel_pump;
    }
}