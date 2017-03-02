namespace LcmsNet.Devices.Dashboard
{
    partial class formDeviceAddForm
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
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mlistbox_devices = new System.Windows.Forms.ListBox();
            this.mbutton_add = new System.Windows.Forms.Button();
            this.mbutton_remove = new System.Windows.Forms.Button();
            this.mtree_availableDevices = new System.Windows.Forms.TreeView();
            this.mcheckBox_initializeOnAdd = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // mbutton_ok
            //
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_ok.Location = new System.Drawing.Point(413, 471);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(90, 33);
            this.mbutton_ok.TabIndex = 1;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            //
            // mbutton_cancel
            //
            this.mbutton_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_cancel.Location = new System.Drawing.Point(513, 471);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(90, 33);
            this.mbutton_cancel.TabIndex = 2;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            this.mbutton_cancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            //
            // mlistbox_devices
            //
            this.mlistbox_devices.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlistbox_devices.FormattingEnabled = true;
            this.mlistbox_devices.ItemHeight = 20;
            this.mlistbox_devices.Location = new System.Drawing.Point(343, 12);
            this.mlistbox_devices.Name = "mlistbox_devices";
            this.mlistbox_devices.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.mlistbox_devices.Size = new System.Drawing.Size(260, 424);
            this.mlistbox_devices.Sorted = true;
            this.mlistbox_devices.TabIndex = 4;
            //
            // mbutton_add
            //
            this.mbutton_add.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_add.Location = new System.Drawing.Point(275, 114);
            this.mbutton_add.Name = "mbutton_add";
            this.mbutton_add.Size = new System.Drawing.Size(48, 33);
            this.mbutton_add.TabIndex = 6;
            this.mbutton_add.Text = ">>";
            this.mbutton_add.UseVisualStyleBackColor = true;
            this.mbutton_add.Click += new System.EventHandler(this.mbutton_add_Click);
            //
            // mbutton_remove
            //
            this.mbutton_remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_remove.Location = new System.Drawing.Point(275, 153);
            this.mbutton_remove.Name = "mbutton_remove";
            this.mbutton_remove.Size = new System.Drawing.Size(48, 33);
            this.mbutton_remove.TabIndex = 7;
            this.mbutton_remove.Text = "<<";
            this.mbutton_remove.UseVisualStyleBackColor = true;
            this.mbutton_remove.Click += new System.EventHandler(this.mbutton_remove_Click);
            //
            // mtree_availableDevices
            //
            this.mtree_availableDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtree_availableDevices.Location = new System.Drawing.Point(12, 10);
            this.mtree_availableDevices.Name = "mtree_availableDevices";
            this.mtree_availableDevices.Size = new System.Drawing.Size(257, 454);
            this.mtree_availableDevices.TabIndex = 8;
            //
            // mcheckBox_initializeOnAdd
            //
            this.mcheckBox_initializeOnAdd.AutoSize = true;
            this.mcheckBox_initializeOnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcheckBox_initializeOnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcheckBox_initializeOnAdd.Location = new System.Drawing.Point(343, 439);
            this.mcheckBox_initializeOnAdd.Name = "mcheckBox_initializeOnAdd";
            this.mcheckBox_initializeOnAdd.Size = new System.Drawing.Size(149, 25);
            this.mcheckBox_initializeOnAdd.TabIndex = 9;
            this.mcheckBox_initializeOnAdd.Text = "Initialize On Add";
            this.mcheckBox_initializeOnAdd.UseVisualStyleBackColor = true;
            //
            // formDeviceAddForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(615, 516);
            this.Controls.Add(this.mcheckBox_initializeOnAdd);
            this.Controls.Add(this.mtree_availableDevices);
            this.Controls.Add(this.mbutton_remove);
            this.Controls.Add(this.mbutton_add);
            this.Controls.Add(this.mlistbox_devices);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mbutton_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formDeviceAddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Devices";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.Button mbutton_cancel;
        private System.Windows.Forms.ListBox mlistbox_devices;
        private System.Windows.Forms.Button mbutton_add;
        private System.Windows.Forms.Button mbutton_remove;
        private System.Windows.Forms.TreeView mtree_availableDevices;
        private System.Windows.Forms.CheckBox mcheckBox_initializeOnAdd;
    }
}