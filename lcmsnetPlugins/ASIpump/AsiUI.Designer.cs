namespace ASIpump
{
    partial class AsiUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsiUI));
            this.grpASI = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnConnect = new System.Windows.Forms.ToolStripButton();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.btnAbort = new System.Windows.Forms.ToolStripButton();
            this.txtPump = new System.Windows.Forms.TextBox();
            this.btnGetPos = new System.Windows.Forms.ToolStripButton();
            this.gridASI = new ASIpump.PropertyGridPersist();
            this.btnGetPositionB = new System.Windows.Forms.ToolStripButton();
            this.grpASI.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpASI
            // 
            this.grpASI.Controls.Add(this.txtPump);
            this.grpASI.Controls.Add(this.gridASI);
            this.grpASI.Controls.Add(this.toolStrip1);
            this.grpASI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpASI.Location = new System.Drawing.Point(0, 0);
            this.grpASI.Name = "grpASI";
            this.grpASI.Size = new System.Drawing.Size(651, 296);
            this.grpASI.TabIndex = 0;
            this.grpASI.TabStop = false;
            this.grpASI.Text = "ASI 576";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConnect,
            this.btnRun,
            this.btnAbort,
            this.btnGetPos,
            this.btnGetPositionB});
            this.toolStrip1.Location = new System.Drawing.Point(3, 268);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(645, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnConnect
            // 
            this.btnConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(56, 22);
            this.btnConnect.Text = "Connect";
            this.btnConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnRun
            // 
            this.btnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRun.Image = ((System.Drawing.Image)(resources.GetObject("btnRun.Image")));
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(32, 22);
            this.btnRun.Text = "Run";
            this.btnRun.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAbort.Image = ((System.Drawing.Image)(resources.GetObject("btnAbort.Image")));
            this.btnAbort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(41, 22);
            this.btnAbort.Text = "Abort";
            this.btnAbort.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // txtPump
            // 
            this.txtPump.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPump.Location = new System.Drawing.Point(277, 16);
            this.txtPump.Multiline = true;
            this.txtPump.Name = "txtPump";
            this.txtPump.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPump.Size = new System.Drawing.Size(371, 252);
            this.txtPump.TabIndex = 2;
            // 
            // btnGetPos
            // 
            this.btnGetPos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGetPos.Image = ((System.Drawing.Image)(resources.GetObject("btnGetPos.Image")));
            this.btnGetPos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGetPos.Name = "btnGetPos";
            this.btnGetPos.Size = new System.Drawing.Size(86, 22);
            this.btnGetPos.Text = "Get Position A";
            this.btnGetPos.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnGetPos.Click += new System.EventHandler(this.btnGetPos_Click);
            // 
            // gridASI
            // 
            this.gridASI.DisplayedObject = null;
            this.gridASI.Dock = System.Windows.Forms.DockStyle.Left;
            this.gridASI.Location = new System.Drawing.Point(3, 16);
            this.gridASI.Name = "gridASI";
            this.gridASI.Size = new System.Drawing.Size(274, 252);
            this.gridASI.TabIndex = 0;
            this.gridASI.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridASI_PropertyValueChanged);
            // 
            // btnGetPositionB
            // 
            this.btnGetPositionB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGetPositionB.Image = ((System.Drawing.Image)(resources.GetObject("btnGetPositionB.Image")));
            this.btnGetPositionB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGetPositionB.Name = "btnGetPositionB";
            this.btnGetPositionB.Size = new System.Drawing.Size(85, 22);
            this.btnGetPositionB.Text = "Get Position B";
            this.btnGetPositionB.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnGetPositionB.Click += new System.EventHandler(this.btnGetPositionB_Click);
            // 
            // AsiUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpASI);
            this.Name = "AsiUI";
            this.Size = new System.Drawing.Size(651, 296);
            this.grpASI.ResumeLayout(false);
            this.grpASI.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpASI;
        private PropertyGridPersist gridASI;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnConnect;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.ToolStripButton btnAbort;
        private System.Windows.Forms.TextBox txtPump;
        private System.Windows.Forms.ToolStripButton btnGetPos;
        private System.Windows.Forms.ToolStripButton btnGetPositionB;
    }
}
