namespace LcmsNetDataClasses.Devices.Pumps
{
    partial class controlPumpDisplay
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
            this.components = new System.ComponentModel.Container();
            this.mplot_monitoringDataPressure = new ZedGraph.ZedGraphControl();
            this.mplot_monitoringDataFlow = new ZedGraph.ZedGraphControl();
            this.mplot_monitoringDataB = new ZedGraph.ZedGraphControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.mlabel_pumpName = new System.Windows.Forms.Label();
            this.mbutton_expand = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mplot_monitoringDataPressure
            // 
            this.mplot_monitoringDataPressure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mplot_monitoringDataPressure.Location = new System.Drawing.Point(0, 24);
            this.mplot_monitoringDataPressure.Name = "mplot_monitoringDataPressure";
            this.mplot_monitoringDataPressure.ScrollGrace = 0;
            this.mplot_monitoringDataPressure.ScrollMaxX = 0;
            this.mplot_monitoringDataPressure.ScrollMaxY = 0;
            this.mplot_monitoringDataPressure.ScrollMaxY2 = 0;
            this.mplot_monitoringDataPressure.ScrollMinX = 0;
            this.mplot_monitoringDataPressure.ScrollMinY = 0;
            this.mplot_monitoringDataPressure.ScrollMinY2 = 0;
            this.mplot_monitoringDataPressure.Size = new System.Drawing.Size(682, 246);
            this.mplot_monitoringDataPressure.TabIndex = 12;
            // 
            // mplot_monitoringDataFlow
            // 
            this.mplot_monitoringDataFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mplot_monitoringDataFlow.Location = new System.Drawing.Point(0, 0);
            this.mplot_monitoringDataFlow.Name = "mplot_monitoringDataFlow";
            this.mplot_monitoringDataFlow.ScrollGrace = 0;
            this.mplot_monitoringDataFlow.ScrollMaxX = 0;
            this.mplot_monitoringDataFlow.ScrollMaxY = 0;
            this.mplot_monitoringDataFlow.ScrollMaxY2 = 0;
            this.mplot_monitoringDataFlow.ScrollMinX = 0;
            this.mplot_monitoringDataFlow.ScrollMinY = 0;
            this.mplot_monitoringDataFlow.ScrollMinY2 = 0;
            this.mplot_monitoringDataFlow.Size = new System.Drawing.Size(682, 311);
            this.mplot_monitoringDataFlow.TabIndex = 13;
            // 
            // mplot_monitoringDataB
            // 
            this.mplot_monitoringDataB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mplot_monitoringDataB.Location = new System.Drawing.Point(0, 0);
            this.mplot_monitoringDataB.Name = "mplot_monitoringDataB";
            this.mplot_monitoringDataB.ScrollGrace = 0;
            this.mplot_monitoringDataB.ScrollMaxX = 0;
            this.mplot_monitoringDataB.ScrollMaxY = 0;
            this.mplot_monitoringDataB.ScrollMaxY2 = 0;
            this.mplot_monitoringDataB.ScrollMinX = 0;
            this.mplot_monitoringDataB.ScrollMinY = 0;
            this.mplot_monitoringDataB.ScrollMinY2 = 0;
            this.mplot_monitoringDataB.Size = new System.Drawing.Size(682, 342);
            this.mplot_monitoringDataB.TabIndex = 14;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mplot_monitoringDataB);
            this.splitContainer1.Size = new System.Drawing.Size(682, 931);
            this.splitContainer1.SplitterDistance = 585;
            this.splitContainer1.TabIndex = 15;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.mbutton_expand);
            this.splitContainer2.Panel1.Controls.Add(this.mplot_monitoringDataPressure);
            this.splitContainer2.Panel1.Controls.Add(this.mlabel_pumpName);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.mplot_monitoringDataFlow);
            this.splitContainer2.Size = new System.Drawing.Size(682, 585);
            this.splitContainer2.SplitterDistance = 270;
            this.splitContainer2.TabIndex = 16;
            // 
            // mlabel_pumpName
            // 
            this.mlabel_pumpName.BackColor = System.Drawing.Color.White;
            this.mlabel_pumpName.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_pumpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_pumpName.Location = new System.Drawing.Point(0, 0);
            this.mlabel_pumpName.Name = "mlabel_pumpName";
            this.mlabel_pumpName.Size = new System.Drawing.Size(682, 24);
            this.mlabel_pumpName.TabIndex = 15;
            this.mlabel_pumpName.Text = "Pump";
            this.mlabel_pumpName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mbutton_expand
            // 
            this.mbutton_expand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            //this.mbutton_expand.Image = global::PNNLDevices.Properties.Resources.Expand;
            this.mbutton_expand.Location = new System.Drawing.Point(653, 27);
            this.mbutton_expand.Name = "mbutton_expand";
            this.mbutton_expand.Size = new System.Drawing.Size(26, 36);
            this.mbutton_expand.TabIndex = 16;
            this.mbutton_expand.UseVisualStyleBackColor = true;
            this.mbutton_expand.Click += new System.EventHandler(this.mbutton_expand_Click);
            // 
            // controlPumpDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "controlPumpDisplay";
            this.Size = new System.Drawing.Size(682, 931);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl mplot_monitoringDataPressure;
        private ZedGraph.ZedGraphControl mplot_monitoringDataFlow;
        private ZedGraph.ZedGraphControl mplot_monitoringDataB;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label mlabel_pumpName;
        private System.Windows.Forms.Button mbutton_expand;
    }
}
