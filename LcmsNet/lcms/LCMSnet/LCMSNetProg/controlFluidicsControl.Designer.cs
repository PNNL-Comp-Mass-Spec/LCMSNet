namespace LcmsNet
{
    partial class controlFluidicsControl
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trackBarScale = new System.Windows.Forms.TrackBar();
            this.textBoxZoom = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarDeviceTransparency = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarPortTransparency = new System.Windows.Forms.TrackBar();
            this.trackBarConnectionTransparency = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelFluidicsDesign = new LcmsNet.controlBufferedPanel();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScale)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDeviceTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPortTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarConnectionTransparency)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBox2
            //
            this.groupBox2.Controls.Add(this.trackBarScale);
            this.groupBox2.Controls.Add(this.textBoxZoom);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 82);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Zoom";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            //
            // trackBarScale
            //
            this.trackBarScale.Location = new System.Drawing.Point(73, 19);
            this.trackBarScale.Maximum = 200;
            this.trackBarScale.Minimum = 10;
            this.trackBarScale.Name = "trackBarScale";
            this.trackBarScale.Size = new System.Drawing.Size(100, 45);
            this.trackBarScale.TabIndex = 4;
            this.trackBarScale.TickFrequency = 190;
            this.trackBarScale.Value = 100;
            this.trackBarScale.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            //
            // textBoxZoom
            //
            this.textBoxZoom.Location = new System.Drawing.Point(13, 19);
            this.textBoxZoom.MaxLength = 3;
            this.textBoxZoom.Name = "textBoxZoom";
            this.textBoxZoom.Size = new System.Drawing.Size(33, 20);
            this.textBoxZoom.TabIndex = 3;
            this.textBoxZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxZoom.Enter += new System.EventHandler(this.textBoxZoom_Enter);
            this.textBoxZoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxZoom_KeyPress);
            this.textBoxZoom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBoxZoom_PreviewKeyDown);
            this.textBoxZoom.Validated += new System.EventHandler(this.textboxZoom_Validated);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "%";
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackBarDeviceTransparency);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.trackBarPortTransparency);
            this.groupBox1.Controls.Add(this.trackBarConnectionTransparency);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(228, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(472, 82);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transparency";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(328, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Device";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port";
            //
            // trackBarDeviceTransparency
            //
            this.trackBarDeviceTransparency.Location = new System.Drawing.Point(375, 19);
            this.trackBarDeviceTransparency.Maximum = 255;
            this.trackBarDeviceTransparency.Name = "trackBarDeviceTransparency";
            this.trackBarDeviceTransparency.Size = new System.Drawing.Size(80, 45);
            this.trackBarDeviceTransparency.TabIndex = 7;
            this.trackBarDeviceTransparency.TickFrequency = 255;
            this.trackBarDeviceTransparency.Value = 255;
            this.trackBarDeviceTransparency.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Connection";
            //
            // trackBarPortTransparency
            //
            this.trackBarPortTransparency.Location = new System.Drawing.Point(226, 19);
            this.trackBarPortTransparency.Maximum = 255;
            this.trackBarPortTransparency.Name = "trackBarPortTransparency";
            this.trackBarPortTransparency.Size = new System.Drawing.Size(76, 45);
            this.trackBarPortTransparency.TabIndex = 6;
            this.trackBarPortTransparency.TickFrequency = 255;
            this.trackBarPortTransparency.Value = 255;
            this.trackBarPortTransparency.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            //
            // trackBarConnectionTransparency
            //
            this.trackBarConnectionTransparency.Location = new System.Drawing.Point(80, 19);
            this.trackBarConnectionTransparency.Maximum = 255;
            this.trackBarConnectionTransparency.Name = "trackBarConnectionTransparency";
            this.trackBarConnectionTransparency.Size = new System.Drawing.Size(73, 45);
            this.trackBarConnectionTransparency.TabIndex = 5;
            this.trackBarConnectionTransparency.TickFrequency = 255;
            this.trackBarConnectionTransparency.Value = 255;
            this.trackBarConnectionTransparency.Scroll += new System.EventHandler(this.trackBarConnectionTransparency_Scroll);
            this.trackBarConnectionTransparency.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 82);
            this.panel1.TabIndex = 1;
            //
            // panelFluidicsDesign
            //
            this.panelFluidicsDesign.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFluidicsDesign.AutoScroll = true;
            this.panelFluidicsDesign.AutoScrollMinSize = new System.Drawing.Size(695, 341);
            this.panelFluidicsDesign.BackColor = System.Drawing.Color.White;
            this.panelFluidicsDesign.Location = new System.Drawing.Point(0, 0);
            this.panelFluidicsDesign.Name = "panelFluidicsDesign";
            this.panelFluidicsDesign.Size = new System.Drawing.Size(697, 340);
            this.panelFluidicsDesign.TabIndex = 0;
            this.panelFluidicsDesign.Paint += new System.Windows.Forms.PaintEventHandler(this.panelFluidicsDesign_Paint);
            //
            // controlFluidicsControl
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelFluidicsDesign);
            this.DoubleBuffered = true;
            this.Name = "controlFluidicsControl";
            this.Size = new System.Drawing.Size(700, 428);
            this.SizeChanged += new System.EventHandler(this.controlFluidicsControl_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.controlFluidicsControl_Paint);
            this.ParentChanged += new System.EventHandler(this.controlFluidicsControl_ParentChanged);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScale)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDeviceTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPortTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarConnectionTransparency)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private controlBufferedPanel panelFluidicsDesign;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar trackBarScale;
        private System.Windows.Forms.TextBox textBoxZoom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarDeviceTransparency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarPortTransparency;
        private System.Windows.Forms.TrackBar trackBarConnectionTransparency;
        private System.Windows.Forms.Panel panel1;
    }
}
