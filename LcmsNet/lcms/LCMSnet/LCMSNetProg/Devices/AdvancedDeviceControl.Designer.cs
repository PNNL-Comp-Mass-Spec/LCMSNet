using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Devices
{
    partial class AdvancedDeviceControl
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
            this.m_advancedTabControl = new System.Windows.Forms.TabControl();
            this.button1 = new System.Windows.Forms.Button();
            this.mbutton_addDevice = new System.Windows.Forms.Button();
            this.mbutton_fluidicsComponent = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // m_advancedTabControl
            //
            this.m_advancedTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_advancedTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_advancedTabControl.Location = new System.Drawing.Point(116, 12);
            this.m_advancedTabControl.Name = "m_advancedTabControl";
            this.m_advancedTabControl.SelectedIndex = 0;
            this.m_advancedTabControl.Size = new System.Drawing.Size(780, 704);
            this.m_advancedTabControl.TabIndex = 0;
            //
            // button1
            //
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(808, 722);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 41);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // mbutton_addDevice
            //
            this.mbutton_addDevice.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_addDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_addDevice.Location = new System.Drawing.Point(12, 12);
            this.mbutton_addDevice.Name = "mbutton_addDevice";
            this.mbutton_addDevice.Size = new System.Drawing.Size(98, 66);
            this.mbutton_addDevice.TabIndex = 2;
            this.mbutton_addDevice.Text = "Device";
            this.mbutton_addDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_addDevice.UseVisualStyleBackColor = true;
            this.mbutton_addDevice.Click += new System.EventHandler(this.mbutton_addDevice_Click);
            //
            // mbutton_fluidicsComponent
            //
            this.mbutton_fluidicsComponent.Enabled = false;
            this.mbutton_fluidicsComponent.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_fluidicsComponent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_fluidicsComponent.Location = new System.Drawing.Point(12, 84);
            this.mbutton_fluidicsComponent.Name = "mbutton_fluidicsComponent";
            this.mbutton_fluidicsComponent.Size = new System.Drawing.Size(98, 66);
            this.mbutton_fluidicsComponent.TabIndex = 3;
            this.mbutton_fluidicsComponent.Text = "Fluidics Component";
            this.mbutton_fluidicsComponent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_fluidicsComponent.UseVisualStyleBackColor = true;
            //
            // AdvancedDeviceControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(908, 775);
            this.Controls.Add(this.mbutton_fluidicsComponent);
            this.Controls.Add(this.mbutton_addDevice);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_advancedTabControl);
            this.Name = "AdvancedDeviceControl";
            this.Text = "Device Control Panel";
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl m_advancedTabControl;
        private Button button1;
        private Button mbutton_addDevice;
        private Button mbutton_fluidicsComponent;
    }
}