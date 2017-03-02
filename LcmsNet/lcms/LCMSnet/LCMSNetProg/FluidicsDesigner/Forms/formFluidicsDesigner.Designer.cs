namespace LcmsNet.FluidicsDesigner
{
    partial class formFluidicsDesigner
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formFluidicsDesigner));
            this.menuStripMainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveAsDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.initializeDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddAgilentPump = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddDetector = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddBrukerStart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddDetContactClosure = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddDetNetStart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddPAL = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddPort = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddValve = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddValve4Port = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddValve6Port = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddValve9Port = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddValve10Port = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLink = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orhtogonalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagramFluidiDesign = new Syncfusion.Windows.Forms.Diagram.Controls.Diagram(this.components);
            this.modelFluidDesign = new Syncfusion.Windows.Forms.Diagram.Model(this.components);
            this.pumpDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diagramFluidiDesign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelFluidDesign)).BeginInit();
            this.SuspendLayout();
            //
            // menuStripMainMenu
            //
            this.menuStripMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.configToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.menuItemAddDevice,
            this.menuItemDelete,
            this.menuItemLink});
            this.menuStripMainMenu.Location = new System.Drawing.Point(0, 0);
            this.menuStripMainMenu.Name = "menuStripMainMenu";
            this.menuStripMainMenu.Size = new System.Drawing.Size(902, 24);
            this.menuStripMainMenu.TabIndex = 0;
            this.menuStripMainMenu.Text = "menuStrip1";
            //
            // menuItemFile
            //
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileLoad,
            this.menuItemFileSave,
            this.menuItemSaveAsDefault});
            this.menuItemFile.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.menuItemFile.MergeIndex = 0;
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(35, 20);
            this.menuItemFile.Text = "File";
            this.menuItemFile.Visible = false;
            //
            // menuItemFileLoad
            //
            this.menuItemFileLoad.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemFileLoad.MergeIndex = 0;
            this.menuItemFileLoad.Name = "menuItemFileLoad";
            this.menuItemFileLoad.Size = new System.Drawing.Size(162, 22);
            this.menuItemFileLoad.Text = "Load";
            this.menuItemFileLoad.Click += new System.EventHandler(this.menuItemFileLoad_Click);
            //
            // menuItemFileSave
            //
            this.menuItemFileSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemFileSave.MergeIndex = 1;
            this.menuItemFileSave.Name = "menuItemFileSave";
            this.menuItemFileSave.Size = new System.Drawing.Size(162, 22);
            this.menuItemFileSave.Text = "Save As";
            this.menuItemFileSave.Click += new System.EventHandler(this.menuItemFileSave_Click);
            //
            // menuItemSaveAsDefault
            //
            this.menuItemSaveAsDefault.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemSaveAsDefault.MergeIndex = 2;
            this.menuItemSaveAsDefault.Name = "menuItemSaveAsDefault";
            this.menuItemSaveAsDefault.Size = new System.Drawing.Size(162, 22);
            this.menuItemSaveAsDefault.Text = "Save As Default";
            this.menuItemSaveAsDefault.Click += new System.EventHandler(this.menuItemSaveAsDefault_Click);
            //
            // configToolStripMenuItem
            //
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.initializeDevicesToolStripMenuItem});
            this.configToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.configToolStripMenuItem.MergeIndex = 1;
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.configToolStripMenuItem.Text = "Config";
            this.configToolStripMenuItem.Visible = false;
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            //
            // initializeDevicesToolStripMenuItem
            //
            this.initializeDevicesToolStripMenuItem.Name = "initializeDevicesToolStripMenuItem";
            this.initializeDevicesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.initializeDevicesToolStripMenuItem.Text = "Initialize Devices";
            this.initializeDevicesToolStripMenuItem.Click += new System.EventHandler(this.initializeDevicesToolStripMenuItem_Click);
            //
            // viewToolStripMenuItem
            //
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devicesToolStripMenuItem,
            this.pumpDataToolStripMenuItem});
            this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.viewToolStripMenuItem.MergeIndex = 2;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            //
            // devicesToolStripMenuItem
            //
            this.devicesToolStripMenuItem.Name = "devicesToolStripMenuItem";
            this.devicesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.devicesToolStripMenuItem.Text = "Devices";
            this.devicesToolStripMenuItem.Click += new System.EventHandler(this.devicesToolStripMenuItem_Click);
            //
            // menuItemAddDevice
            //
            this.menuItemAddDevice.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddAgilentPump,
            this.menuItemAddDetector,
            this.menuItemAddPAL,
            this.menuItemAddPort,
            this.menuItemAddValve});
            this.menuItemAddDevice.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemAddDevice.MergeIndex = 3;
            this.menuItemAddDevice.Name = "menuItemAddDevice";
            this.menuItemAddDevice.Size = new System.Drawing.Size(73, 20);
            this.menuItemAddDevice.Text = "Add Device";
            //
            // menuItemAddAgilentPump
            //
            this.menuItemAddAgilentPump.Name = "menuItemAddAgilentPump";
            this.menuItemAddAgilentPump.Size = new System.Drawing.Size(147, 22);
            this.menuItemAddAgilentPump.Text = "Agilent Pump";
            this.menuItemAddAgilentPump.Click += new System.EventHandler(this.menuItemAddAgilentPump_Click);
            //
            // menuItemAddDetector
            //
            this.menuItemAddDetector.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddBrukerStart,
            this.menuItemAddDetContactClosure,
            this.menuItemAddDetNetStart});
            this.menuItemAddDetector.Name = "menuItemAddDetector";
            this.menuItemAddDetector.Size = new System.Drawing.Size(147, 22);
            this.menuItemAddDetector.Text = "Detector";
            //
            // menuItemAddBrukerStart
            //
            this.menuItemAddBrukerStart.Name = "menuItemAddBrukerStart";
            this.menuItemAddBrukerStart.Size = new System.Drawing.Size(162, 22);
            this.menuItemAddBrukerStart.Text = "Bruker";
            this.menuItemAddBrukerStart.Click += new System.EventHandler(this.menuItemAddBrukerStart_Click);
            //
            // menuItemAddDetContactClosure
            //
            this.menuItemAddDetContactClosure.Name = "menuItemAddDetContactClosure";
            this.menuItemAddDetContactClosure.Size = new System.Drawing.Size(162, 22);
            this.menuItemAddDetContactClosure.Text = "Contact Closure";
            this.menuItemAddDetContactClosure.Click += new System.EventHandler(this.menuItemAddDetContactClosure_Click);
            //
            // menuItemAddDetNetStart
            //
            this.menuItemAddDetNetStart.Name = "menuItemAddDetNetStart";
            this.menuItemAddDetNetStart.Size = new System.Drawing.Size(162, 22);
            this.menuItemAddDetNetStart.Text = "Network Start";
            this.menuItemAddDetNetStart.Click += new System.EventHandler(this.menuItemAddDetNetStart_Click);
            //
            // menuItemAddPAL
            //
            this.menuItemAddPAL.Name = "menuItemAddPAL";
            this.menuItemAddPAL.Size = new System.Drawing.Size(147, 22);
            this.menuItemAddPAL.Text = "PAL";
            this.menuItemAddPAL.Click += new System.EventHandler(this.menuItemAddPAL_Click);
            //
            // menuItemAddPort
            //
            this.menuItemAddPort.Enabled = false;
            this.menuItemAddPort.Name = "menuItemAddPort";
            this.menuItemAddPort.Size = new System.Drawing.Size(147, 22);
            this.menuItemAddPort.Text = "Port";
            this.menuItemAddPort.Click += new System.EventHandler(this.menuItemAddPort_Click);
            //
            // menuItemAddValve
            //
            this.menuItemAddValve.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddValve4Port,
            this.menuItemAddValve6Port,
            this.menuItemAddValve9Port,
            this.menuItemAddValve10Port});
            this.menuItemAddValve.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemAddValve.MergeIndex = 3;
            this.menuItemAddValve.Name = "menuItemAddValve";
            this.menuItemAddValve.Size = new System.Drawing.Size(147, 22);
            this.menuItemAddValve.Text = "Valve";
            //
            // menuItemAddValve4Port
            //
            this.menuItemAddValve4Port.Name = "menuItemAddValve4Port";
            this.menuItemAddValve4Port.Size = new System.Drawing.Size(121, 22);
            this.menuItemAddValve4Port.Text = "4-Port";
            this.menuItemAddValve4Port.Click += new System.EventHandler(this.menuItemAddValve4Port_Click);
            //
            // menuItemAddValve6Port
            //
            this.menuItemAddValve6Port.Name = "menuItemAddValve6Port";
            this.menuItemAddValve6Port.Size = new System.Drawing.Size(121, 22);
            this.menuItemAddValve6Port.Text = "6-Port";
            this.menuItemAddValve6Port.Click += new System.EventHandler(this.menuItemAddValve6Port_Click);
            //
            // menuItemAddValve9Port
            //
            this.menuItemAddValve9Port.Name = "menuItemAddValve9Port";
            this.menuItemAddValve9Port.Size = new System.Drawing.Size(121, 22);
            this.menuItemAddValve9Port.Text = "9-Port";
            this.menuItemAddValve9Port.Click += new System.EventHandler(this.menuItemAddValve9Port_Click);
            //
            // menuItemAddValve10Port
            //
            this.menuItemAddValve10Port.Name = "menuItemAddValve10Port";
            this.menuItemAddValve10Port.Size = new System.Drawing.Size(121, 22);
            this.menuItemAddValve10Port.Text = "10-Port";
            this.menuItemAddValve10Port.Click += new System.EventHandler(this.menuItemAddValve10Port_Click);
            //
            // menuItemDelete
            //
            this.menuItemDelete.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemDelete.MergeIndex = 4;
            this.menuItemDelete.Name = "menuItemDelete";
            this.menuItemDelete.Size = new System.Drawing.Size(50, 20);
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            //
            // menuItemLink
            //
            this.menuItemLink.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineToolStripMenuItem,
            this.orhtogonalToolStripMenuItem});
            this.menuItemLink.Enabled = false;
            this.menuItemLink.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItemLink.MergeIndex = 5;
            this.menuItemLink.Name = "menuItemLink";
            this.menuItemLink.Size = new System.Drawing.Size(37, 20);
            this.menuItemLink.Text = "Link";
            //
            // lineToolStripMenuItem
            //
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.lineToolStripMenuItem.Text = "Line";
            this.lineToolStripMenuItem.Click += new System.EventHandler(this.menuItemLinkLine_Click);
            //
            // orhtogonalToolStripMenuItem
            //
            this.orhtogonalToolStripMenuItem.Name = "orhtogonalToolStripMenuItem";
            this.orhtogonalToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.orhtogonalToolStripMenuItem.Text = "Orhtogonal";
            this.orhtogonalToolStripMenuItem.Click += new System.EventHandler(this.menuItemLinkOrtho_Click);
            //
            // diagramFluidiDesign
            //
            this.diagramFluidiDesign.AllowDrop = true;
            this.diagramFluidiDesign.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diagramFluidiDesign.Controller.PasteOffset = new System.Drawing.SizeF(10F, 10F);
            this.diagramFluidiDesign.HScroll = true;
            this.diagramFluidiDesign.LayoutManager = null;
            this.diagramFluidiDesign.Location = new System.Drawing.Point(15, 35);
            this.diagramFluidiDesign.Model = this.modelFluidDesign;
            this.diagramFluidiDesign.Name = "diagramFluidiDesign";
            this.diagramFluidiDesign.ScrollVirtualBounds = ((System.Drawing.RectangleF)(resources.GetObject("diagramFluidiDesign.ScrollVirtualBounds")));
            this.diagramFluidiDesign.ShowRulers = false;
            this.diagramFluidiDesign.Size = new System.Drawing.Size(866, 559);
            this.diagramFluidiDesign.SmartSizeBox = false;
            this.diagramFluidiDesign.TabIndex = 1;
            this.diagramFluidiDesign.Text = "diagramFluidDesign";
            //
            //
            //
            this.diagramFluidiDesign.View.Grid.HorizontalSpacing = 10F;
            this.diagramFluidiDesign.View.Grid.VerticalSpacing = 10F;
            this.diagramFluidiDesign.View.MouseTrackingEnabled = false;
            this.diagramFluidiDesign.View.ScrollVirtualBounds = ((System.Drawing.RectangleF)(resources.GetObject("diagramFluidiDesign.View.ScrollVirtualBounds")));
            this.diagramFluidiDesign.VScroll = true;
            //
            // modelFluidDesign
            //
            this.modelFluidDesign.BackgroundStyle.Color = System.Drawing.Color.White;
            this.modelFluidDesign.BackgroundStyle.PathBrushStyle = Syncfusion.Windows.Forms.Diagram.PathGradientBrushStyle.RectangleCenter;
            this.modelFluidDesign.CanUngroup = false;
            this.modelFluidDesign.DocumentScale.DisplayName = "No Scale";
            this.modelFluidDesign.DocumentScale.Height = 1F;
            this.modelFluidDesign.DocumentScale.Width = 1F;
            this.modelFluidDesign.DocumentSize.DisplayName = "A4: 210 mm x 297 mm";
            this.modelFluidDesign.DocumentSize.Height = 1500F;
            this.modelFluidDesign.DocumentSize.Width = 4500F;
            this.modelFluidDesign.LineBridgingEnabled = true;
            this.modelFluidDesign.LineRoutingEnabled = true;
            this.modelFluidDesign.LineStyle.DashPattern = null;
            this.modelFluidDesign.LineStyle.LineColor = System.Drawing.Color.Black;
            this.modelFluidDesign.LogicalSize = new System.Drawing.SizeF(4500F, 1500F);
            this.modelFluidDesign.OptimizeLineBridging = false;
            this.modelFluidDesign.ShadowStyle.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.modelFluidDesign.ShadowStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            //
            // pumpDataToolStripMenuItem
            //
            this.pumpDataToolStripMenuItem.Name = "pumpDataToolStripMenuItem";
            this.pumpDataToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pumpDataToolStripMenuItem.Text = "Pump Data";
            this.pumpDataToolStripMenuItem.Click += new System.EventHandler(this.pumpDataToolStripMenuItem_Click);
            //
            // formFluidicsDesigner
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 610);
            this.Controls.Add(this.diagramFluidiDesign);
            this.Controls.Add(this.menuStripMainMenu);
            this.Name = "formFluidicsDesigner";
            this.Text = "Fluidics Designer";
            this.menuStripMainMenu.ResumeLayout(false);
            this.menuStripMainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diagramFluidiDesign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelFluidDesign)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileLoad;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddDevice;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddValve;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddValve4Port;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddPAL;
        private System.Windows.Forms.ToolStripMenuItem menuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem menuItemLink;
        private Syncfusion.Windows.Forms.Diagram.Controls.Diagram diagramFluidiDesign;
        private Syncfusion.Windows.Forms.Diagram.Model modelFluidDesign;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddPort;
        private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orhtogonalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddValve6Port;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddValve10Port;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddAgilentPump;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddDetector;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddDetContactClosure;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddDetNetStart;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddValve9Port;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveAsDefault;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
          private System.Windows.Forms.ToolStripMenuItem menuItemAddBrukerStart;
          private System.Windows.Forms.ToolStripMenuItem pumpDataToolStripMenuItem;

    }
}