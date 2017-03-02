namespace LcmsNet.Devices.Valves
{
    partial class controlValveVICIMultiPosGlyph
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
            this.mlabel_name = new System.Windows.Forms.Label();
            this.mbutton_SetPosition = new System.Windows.Forms.Button();
            this.mcomboBox_Position = new System.Windows.Forms.ComboBox();
            this.mtextbox_CurrentPos = new System.Windows.Forms.TextBox();
            this.mbutton_refreshPos = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mlabel_name
            // 
            this.mlabel_name.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_name.Location = new System.Drawing.Point(0, 0);
            this.mlabel_name.Name = "mlabel_name";
            this.mlabel_name.Size = new System.Drawing.Size(108, 25);
            this.mlabel_name.TabIndex = 6;
            this.mlabel_name.Text = "Multi Position Valve";
            this.mlabel_name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mbutton_SetPosition
            // 
            this.mbutton_SetPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_SetPosition.Location = new System.Drawing.Point(62, 72);
            this.mbutton_SetPosition.Name = "mbutton_SetPosition";
            this.mbutton_SetPosition.Size = new System.Drawing.Size(43, 47);
            this.mbutton_SetPosition.TabIndex = 1;
            this.mbutton_SetPosition.Text = "Set";
            this.mbutton_SetPosition.UseVisualStyleBackColor = true;
            this.mbutton_SetPosition.Click += new System.EventHandler(this.mbutton_SetPosition_Click);
            // 
            // mcomboBox_Position
            // 
            this.mcomboBox_Position.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_Position.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_Position.FormattingEnabled = true;
            this.mcomboBox_Position.Location = new System.Drawing.Point(3, 91);
            this.mcomboBox_Position.Name = "mcomboBox_Position";
            this.mcomboBox_Position.Size = new System.Drawing.Size(53, 21);
            this.mcomboBox_Position.TabIndex = 0;
            // 
            // mtextbox_CurrentPos
            // 
            this.mtextbox_CurrentPos.Dock = System.Windows.Forms.DockStyle.Top;
            this.mtextbox_CurrentPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextbox_CurrentPos.Location = new System.Drawing.Point(0, 0);
            this.mtextbox_CurrentPos.Name = "mtextbox_CurrentPos";
            this.mtextbox_CurrentPos.ReadOnly = true;
            this.mtextbox_CurrentPos.Size = new System.Drawing.Size(108, 31);
            this.mtextbox_CurrentPos.TabIndex = 5;
            this.mtextbox_CurrentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_refreshPos
            // 
            this.mbutton_refreshPos.Dock = System.Windows.Forms.DockStyle.Top;
            this.mbutton_refreshPos.Location = new System.Drawing.Point(0, 31);
            this.mbutton_refreshPos.Name = "mbutton_refreshPos";
            this.mbutton_refreshPos.Size = new System.Drawing.Size(108, 35);
            this.mbutton_refreshPos.TabIndex = 6;
            this.mbutton_refreshPos.Text = "Refresh";
            this.mbutton_refreshPos.UseVisualStyleBackColor = true;
            this.mbutton_refreshPos.Click += new System.EventHandler(this.mbutton_refreshPos_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mbutton_refreshPos);
            this.panel1.Controls.Add(this.mbutton_SetPosition);
            this.panel1.Controls.Add(this.mcomboBox_Position);
            this.panel1.Controls.Add(this.mtextbox_CurrentPos);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(108, 129);
            this.panel1.TabIndex = 16;
            // 
            // controlValveVICIMultiPosGlyph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mlabel_name);
            this.Name = "controlValveVICIMultiPosGlyph";
            this.Size = new System.Drawing.Size(108, 154);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label mlabel_name;
        private System.Windows.Forms.Button mbutton_SetPosition;
        private System.Windows.Forms.ComboBox mcomboBox_Position;
        private System.Windows.Forms.TextBox mtextbox_CurrentPos;
        private System.Windows.Forms.Button mbutton_refreshPos;
        private System.Windows.Forms.Panel panel1;
    }
}
