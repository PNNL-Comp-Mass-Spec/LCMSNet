namespace LcmsNet.Devices.Valves
{
    partial class controlValveVICI2PosGlyph
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
            this.mbutton_setPosition = new System.Windows.Forms.Button();
            this.mbutton_setPositionB = new System.Windows.Forms.Button();
            this.mlabel_name = new System.Windows.Forms.Label();
            this.mtextbox_CurrentPos = new System.Windows.Forms.TextBox();
            this.mbutton_refreshPos = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbutton_setPosition
            // 
            this.mbutton_setPosition.Location = new System.Drawing.Point(4, 69);
            this.mbutton_setPosition.Name = "mbutton_setPosition";
            this.mbutton_setPosition.Size = new System.Drawing.Size(46, 54);
            this.mbutton_setPosition.TabIndex = 13;
            this.mbutton_setPosition.Text = "A";
            this.mbutton_setPosition.UseVisualStyleBackColor = true;
            this.mbutton_setPosition.Click += new System.EventHandler(this.mbutton_setPosition_Click);
            // 
            // mbutton_setPositionB
            // 
            this.mbutton_setPositionB.Location = new System.Drawing.Point(56, 69);
            this.mbutton_setPositionB.Name = "mbutton_setPositionB";
            this.mbutton_setPositionB.Size = new System.Drawing.Size(49, 54);
            this.mbutton_setPositionB.TabIndex = 12;
            this.mbutton_setPositionB.Text = "B";
            this.mbutton_setPositionB.UseVisualStyleBackColor = true;
            this.mbutton_setPositionB.Click += new System.EventHandler(this.mbutton_setPositionB_Click);
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(106, 25);
            this.mlabel_name.TabIndex = 11;
            this.mlabel_name.Text = "2-Position Valve";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mtextbox_CurrentPos
            // 
            this.mtextbox_CurrentPos.Dock = System.Windows.Forms.DockStyle.Top;
            this.mtextbox_CurrentPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextbox_CurrentPos.Location = new System.Drawing.Point(0, 0);
            this.mtextbox_CurrentPos.Name = "mtextbox_CurrentPos";
            this.mtextbox_CurrentPos.ReadOnly = true;
            this.mtextbox_CurrentPos.Size = new System.Drawing.Size(106, 31);
            this.mtextbox_CurrentPos.TabIndex = 5;
            this.mtextbox_CurrentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_refreshPos
            // 
            this.mbutton_refreshPos.Dock = System.Windows.Forms.DockStyle.Top;
            this.mbutton_refreshPos.Location = new System.Drawing.Point(0, 31);
            this.mbutton_refreshPos.Name = "mbutton_refreshPos";
            this.mbutton_refreshPos.Size = new System.Drawing.Size(106, 35);
            this.mbutton_refreshPos.TabIndex = 6;
            this.mbutton_refreshPos.Text = "Refresh";
            this.mbutton_refreshPos.UseVisualStyleBackColor = true;
            this.mbutton_refreshPos.Click += new System.EventHandler(this.mbutton_refreshPos_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mbutton_refreshPos);
            this.panel1.Controls.Add(this.mtextbox_CurrentPos);
            this.panel1.Controls.Add(this.mbutton_setPosition);
            this.panel1.Controls.Add(this.mbutton_setPositionB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(106, 127);
            this.panel1.TabIndex = 15;
            // 
            // controlValveVICI2PosGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlValveVICI2PosGlyph";
            this.Size = new System.Drawing.Size(106, 152);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_setPosition;
        private System.Windows.Forms.Button mbutton_setPositionB;
        private System.Windows.Forms.Label mlabel_name;
        private System.Windows.Forms.TextBox mtextbox_CurrentPos;
        private System.Windows.Forms.Button mbutton_refreshPos;
        private System.Windows.Forms.Panel panel1;

        /// <summary>
        /// Gets or sets the status display bar.
        /// </summary>
        public LcmsNetDataClasses.Devices.controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }
    }
}
