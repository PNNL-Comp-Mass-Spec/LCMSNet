namespace LcmsNet.Devices.Pumps
{
	partial class controlPumpIscoGraphs
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.mplot_PumpA = new ZedGraph.ZedGraphControl();
			this.mplot_PumpB = new ZedGraph.ZedGraphControl();
			this.mplot_PumpC = new ZedGraph.ZedGraphControl();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.mplot_PumpC, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.mplot_PumpB, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.mplot_PumpA, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(682, 931);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// mplot_PumpA
			// 
			this.mplot_PumpA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mplot_PumpA.Location = new System.Drawing.Point(6, 6);
			this.mplot_PumpA.Name = "mplot_PumpA";
			this.mplot_PumpA.ScrollGrace = 0;
			this.mplot_PumpA.ScrollMaxX = 0;
			this.mplot_PumpA.ScrollMaxY = 0;
			this.mplot_PumpA.ScrollMaxY2 = 0;
			this.mplot_PumpA.ScrollMinX = 0;
			this.mplot_PumpA.ScrollMinY = 0;
			this.mplot_PumpA.ScrollMinY2 = 0;
			this.mplot_PumpA.Size = new System.Drawing.Size(670, 300);
			this.mplot_PumpA.TabIndex = 0;
			// 
			// mplot_PumpB
			// 
			this.mplot_PumpB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mplot_PumpB.Location = new System.Drawing.Point(6, 315);
			this.mplot_PumpB.Name = "mplot_PumpB";
			this.mplot_PumpB.ScrollGrace = 0;
			this.mplot_PumpB.ScrollMaxX = 0;
			this.mplot_PumpB.ScrollMaxY = 0;
			this.mplot_PumpB.ScrollMaxY2 = 0;
			this.mplot_PumpB.ScrollMinX = 0;
			this.mplot_PumpB.ScrollMinY = 0;
			this.mplot_PumpB.ScrollMinY2 = 0;
			this.mplot_PumpB.Size = new System.Drawing.Size(670, 300);
			this.mplot_PumpB.TabIndex = 1;
			// 
			// mplot_PumpC
			// 
			this.mplot_PumpC.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mplot_PumpC.Location = new System.Drawing.Point(6, 624);
			this.mplot_PumpC.Name = "mplot_PumpC";
			this.mplot_PumpC.ScrollGrace = 0;
			this.mplot_PumpC.ScrollMaxX = 0;
			this.mplot_PumpC.ScrollMaxY = 0;
			this.mplot_PumpC.ScrollMaxY2 = 0;
			this.mplot_PumpC.ScrollMinX = 0;
			this.mplot_PumpC.ScrollMinY = 0;
			this.mplot_PumpC.ScrollMinY2 = 0;
			this.mplot_PumpC.Size = new System.Drawing.Size(670, 301);
			this.mplot_PumpC.TabIndex = 2;
			// 
			// controlPumpIscoGraphs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "controlPumpIscoGraphs";
			this.Size = new System.Drawing.Size(682, 931);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ZedGraph.ZedGraphControl mplot_PumpA;
		private ZedGraph.ZedGraphControl mplot_PumpC;
		private ZedGraph.ZedGraphControl mplot_PumpB;
	}
}
