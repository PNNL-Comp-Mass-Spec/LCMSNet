using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Devices.Fluidics
{
    partial class FluidicsDesign
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.mbutton_removeDevice = new System.Windows.Forms.Button();
            this.mbutton_loadHardware = new System.Windows.Forms.Button();
            this.mbutton_save = new System.Windows.Forms.Button();
            this.mbutton_saveAs = new System.Windows.Forms.Button();
            this.mbutton_lock = new System.Windows.Forms.Button();
            this.mbutton_unlock = new System.Windows.Forms.Button();
            this.mbutton_initialize = new System.Windows.Forms.Button();
            this.mbutton_addDevice = new System.Windows.Forms.Button();
            this.panelDevices = new System.Windows.Forms.Panel();
            this.panelDeviceQuickboard = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDesign = new System.Windows.Forms.TabPage();
            this.controlFluidicsControlDesigner = new LcmsNet.controlFluidicsControl();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.advancedDeviceControlPanel2 = new LcmsNet.Devices.AdvancedDeviceControlPanel();
            this.tabPageModelStatus = new System.Windows.Forms.TabPage();
            this.tabPageModel = new System.Windows.Forms.TabPage();
            this.panelDevices.SuspendLayout();
            this.panelDeviceQuickboard.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageDesign.SuspendLayout();
            this.tabPageConfiguration.SuspendLayout();
            this.SuspendLayout();
            //
            // mbutton_removeDevice
            //
            this.mbutton_removeDevice.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_removeDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_removeDevice.Location = new System.Drawing.Point(3, 314);
            this.mbutton_removeDevice.Name = "mbutton_removeDevice";
            this.mbutton_removeDevice.Size = new System.Drawing.Size(75, 71);
            this.mbutton_removeDevice.TabIndex = 2;
            this.mbutton_removeDevice.Text = "Remove";
            this.mbutton_removeDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_removeDevice.UseVisualStyleBackColor = true;
            this.mbutton_removeDevice.Click += new System.EventHandler(this.btnRemove_Click);
            //
            // mbutton_loadHardware
            //
            this.mbutton_loadHardware.Image = global::LcmsNet.Properties.Resources.Open;
            this.mbutton_loadHardware.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_loadHardware.Location = new System.Drawing.Point(789, 6);
            this.mbutton_loadHardware.Name = "mbutton_loadHardware";
            this.mbutton_loadHardware.Size = new System.Drawing.Size(58, 32);
            this.mbutton_loadHardware.TabIndex = 19;
            this.mbutton_loadHardware.Text = "Load";
            this.mbutton_loadHardware.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_loadHardware.UseVisualStyleBackColor = true;
            this.mbutton_loadHardware.Click += new System.EventHandler(this.mbutton_loadHardware_Click);
            //
            // mbutton_save
            //
            this.mbutton_save.Image = global::LcmsNet.Properties.Resources.Save;
            this.mbutton_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_save.Location = new System.Drawing.Point(853, 6);
            this.mbutton_save.Name = "mbutton_save";
            this.mbutton_save.Size = new System.Drawing.Size(58, 32);
            this.mbutton_save.TabIndex = 20;
            this.mbutton_save.Text = "Save";
            this.mbutton_save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_save.UseVisualStyleBackColor = true;
            this.mbutton_save.Click += new System.EventHandler(this.mbutton_save_Click);
            //
            // mbutton_saveAs
            //
            this.mbutton_saveAs.Image = global::LcmsNet.Properties.Resources.Save;
            this.mbutton_saveAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_saveAs.Location = new System.Drawing.Point(917, 6);
            this.mbutton_saveAs.Name = "mbutton_saveAs";
            this.mbutton_saveAs.Size = new System.Drawing.Size(58, 32);
            this.mbutton_saveAs.TabIndex = 21;
            this.mbutton_saveAs.Text = "Export";
            this.mbutton_saveAs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_saveAs.UseVisualStyleBackColor = true;
            this.mbutton_saveAs.Click += new System.EventHandler(this.mbutton_saveAs_Click);
            //
            // mbutton_lock
            //
            this.mbutton_lock.Image = global::LcmsNet.Properties.Resources._lock;
            this.mbutton_lock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_lock.Location = new System.Drawing.Point(3, 6);
            this.mbutton_lock.Name = "mbutton_lock";
            this.mbutton_lock.Size = new System.Drawing.Size(75, 71);
            this.mbutton_lock.TabIndex = 22;
            this.mbutton_lock.Text = "Lock";
            this.mbutton_lock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_lock.UseVisualStyleBackColor = true;
            this.mbutton_lock.Click += new System.EventHandler(this.mbutton_lock_Click);
            //
            // mbutton_unlock
            //
            this.mbutton_unlock.Enabled = false;
            this.mbutton_unlock.Image = global::LcmsNet.Properties.Resources.unlock;
            this.mbutton_unlock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_unlock.Location = new System.Drawing.Point(3, 83);
            this.mbutton_unlock.Name = "mbutton_unlock";
            this.mbutton_unlock.Size = new System.Drawing.Size(75, 71);
            this.mbutton_unlock.TabIndex = 23;
            this.mbutton_unlock.Text = "Unlock";
            this.mbutton_unlock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_unlock.UseVisualStyleBackColor = true;
            this.mbutton_unlock.Click += new System.EventHandler(this.mbutton_unlock_Click);
            //
            // mbutton_initialize
            //
            this.mbutton_initialize.Image = global::LcmsNet.Properties.Resources.Cycle_16_Yellow;
            this.mbutton_initialize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_initialize.Location = new System.Drawing.Point(3, 160);
            this.mbutton_initialize.Name = "mbutton_initialize";
            this.mbutton_initialize.Size = new System.Drawing.Size(75, 71);
            this.mbutton_initialize.TabIndex = 24;
            this.mbutton_initialize.Text = "Initialize";
            this.mbutton_initialize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_initialize.UseVisualStyleBackColor = true;
            this.mbutton_initialize.Click += new System.EventHandler(this.mbutton_initialize_Click);
            //
            // mbutton_addDevice
            //
            this.mbutton_addDevice.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_addDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_addDevice.Location = new System.Drawing.Point(3, 237);
            this.mbutton_addDevice.Name = "mbutton_addDevice";
            this.mbutton_addDevice.Size = new System.Drawing.Size(75, 71);
            this.mbutton_addDevice.TabIndex = 25;
            this.mbutton_addDevice.Text = "Add";
            this.mbutton_addDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_addDevice.UseVisualStyleBackColor = true;
            this.mbutton_addDevice.Click += new System.EventHandler(this.btnAddDevice_Click);
            //
            // panelDevices
            //
            this.panelDevices.Controls.Add(this.mbutton_loadHardware);
            this.panelDevices.Controls.Add(this.mbutton_saveAs);
            this.panelDevices.Controls.Add(this.mbutton_save);
            this.panelDevices.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDevices.Location = new System.Drawing.Point(0, 0);
            this.panelDevices.Name = "panelDevices";
            this.panelDevices.Size = new System.Drawing.Size(920, 40);
            this.panelDevices.TabIndex = 1;
            //
            // panelDeviceQuickboard
            //
            this.panelDeviceQuickboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDeviceQuickboard.Controls.Add(this.btnConnect);
            this.panelDeviceQuickboard.Controls.Add(this.mbutton_lock);
            this.panelDeviceQuickboard.Controls.Add(this.mbutton_addDevice);
            this.panelDeviceQuickboard.Controls.Add(this.mbutton_unlock);
            this.panelDeviceQuickboard.Controls.Add(this.mbutton_initialize);
            this.panelDeviceQuickboard.Controls.Add(this.mbutton_removeDevice);
            this.panelDeviceQuickboard.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDeviceQuickboard.Location = new System.Drawing.Point(920, 0);
            this.panelDeviceQuickboard.Name = "panelDeviceQuickboard";
            this.panelDeviceQuickboard.Size = new System.Drawing.Size(86, 591);
            this.panelDeviceQuickboard.TabIndex = 27;
            //
            // btnConnect
            //
            this.btnConnect.Location = new System.Drawing.Point(5, 391);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 71);
            this.btnConnect.TabIndex = 26;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // tabControl1
            //
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageDesign);
            this.tabControl1.Controls.Add(this.tabPageConfiguration);
            this.tabControl1.Controls.Add(this.tabPageModelStatus);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(914, 539);
            this.tabControl1.TabIndex = 28;
            //
            // tabPageDesign
            //
            this.tabPageDesign.Controls.Add(this.controlFluidicsControlDesigner);
            this.tabPageDesign.Location = new System.Drawing.Point(4, 29);
            this.tabPageDesign.Name = "tabPageDesign";
            this.tabPageDesign.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDesign.Size = new System.Drawing.Size(906, 506);
            this.tabPageDesign.TabIndex = 0;
            this.tabPageDesign.Text = "Design";
            this.tabPageDesign.UseVisualStyleBackColor = true;
            //
            // controlFluidicsControlDesigner
            //
            this.controlFluidicsControlDesigner.DevicesLocked = false;
            this.controlFluidicsControlDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlFluidicsControlDesigner.Location = new System.Drawing.Point(3, 3);
            this.controlFluidicsControlDesigner.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.controlFluidicsControlDesigner.Name = "controlFluidicsControlDesigner";
            this.controlFluidicsControlDesigner.Size = new System.Drawing.Size(900, 500);
            this.controlFluidicsControlDesigner.TabIndex = 0;
            //
            // tabPageConfiguration
            //
            this.tabPageConfiguration.Controls.Add(this.advancedDeviceControlPanel2);
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 29);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfiguration.Size = new System.Drawing.Size(906, 506);
            this.tabPageConfiguration.TabIndex = 1;
            this.tabPageConfiguration.Text = "Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            //
            // advancedDeviceControlPanel2
            //
            this.advancedDeviceControlPanel2.AutoScroll = true;
            this.advancedDeviceControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedDeviceControlPanel2.Location = new System.Drawing.Point(3, 3);
            this.advancedDeviceControlPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.advancedDeviceControlPanel2.Name = "advancedDeviceControlPanel2";
            this.advancedDeviceControlPanel2.Size = new System.Drawing.Size(900, 500);
            this.advancedDeviceControlPanel2.TabIndex = 0;
            //
            // tabPageModelStatus
            //
            this.tabPageModelStatus.Location = new System.Drawing.Point(4, 29);
            this.tabPageModelStatus.Name = "tabPageModelStatus";
            this.tabPageModelStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModelStatus.Size = new System.Drawing.Size(906, 506);
            this.tabPageModelStatus.TabIndex = 2;
            this.tabPageModelStatus.Text = "Model Status";
            this.tabPageModelStatus.UseVisualStyleBackColor = true;
            //
            // tabPageModel
            //
            this.tabPageModel.Location = new System.Drawing.Point(4, 29);
            this.tabPageModel.Name = "tabPageModel";
            this.tabPageModel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModel.Size = new System.Drawing.Size(1084, 839);
            this.tabPageModel.TabIndex = 2;
            this.tabPageModel.Text = "Model Status";
            this.tabPageModel.UseVisualStyleBackColor = true;
            //
            // FluidicsDesign
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1006, 591);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelDevices);
            this.Controls.Add(this.panelDeviceQuickboard);
            this.Name = "FluidicsDesign";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FluidicsDesign_Paint);
            this.panelDevices.ResumeLayout(false);
            this.panelDeviceQuickboard.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageDesign.ResumeLayout(false);
            this.tabPageConfiguration.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Button mbutton_removeDevice;
        private Button mbutton_loadHardware;
        private Button mbutton_save;
        private Button mbutton_saveAs;
        private Button mbutton_lock;
        private Button mbutton_unlock;
        private Button mbutton_initialize;
        private Button mbutton_addDevice;
        private Panel panelDevices;
        private Panel panelDeviceQuickboard;
        private TabControl tabControl1;
        private Button btnConnect;
        private TabPage tabPageModel;
        private TabPage tabPageDesign;
        private controlFluidicsControl controlFluidicsControlDesigner;
        private TabPage tabPageConfiguration;
        private AdvancedDeviceControlPanel advancedDeviceControlPanel2;
        private TabPage tabPageModelStatus;

    }
}