using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Devices.Dashboard
{
    partial class formResolveDeviceStatus
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
            this.mbutton_leaveError = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mbutton_clearErrors = new System.Windows.Forms.Button();
            this.mbutton_notInitialized = new System.Windows.Forms.Button();
            this.mbutton_doNothing = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // mbutton_leaveError
            //
            this.mbutton_leaveError.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_leaveError.Image = global::LcmsNet.Properties.Resources.ButtonDeleteRed;
            this.mbutton_leaveError.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_leaveError.Location = new System.Drawing.Point(12, 46);
            this.mbutton_leaveError.Name = "mbutton_leaveError";
            this.mbutton_leaveError.Size = new System.Drawing.Size(106, 66);
            this.mbutton_leaveError.TabIndex = 0;
            this.mbutton_leaveError.Text = "Leave Error Indicator";
            this.mbutton_leaveError.UseVisualStyleBackColor = true;
            this.mbutton_leaveError.Click += new System.EventHandler(this.mbutton_leaveError_Click);
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "How do you want to resolve this device status?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // mbutton_clearErrors
            //
            this.mbutton_clearErrors.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_clearErrors.Image = global::LcmsNet.Properties.Resources.AllIsGood;
            this.mbutton_clearErrors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_clearErrors.Location = new System.Drawing.Point(124, 46);
            this.mbutton_clearErrors.Name = "mbutton_clearErrors";
            this.mbutton_clearErrors.Size = new System.Drawing.Size(106, 66);
            this.mbutton_clearErrors.TabIndex = 2;
            this.mbutton_clearErrors.Text = "Clear Error Indicator as Initialized";
            this.mbutton_clearErrors.UseVisualStyleBackColor = true;
            this.mbutton_clearErrors.Click += new System.EventHandler(this.mbutton_clearErrors_Click);
            //
            // mbutton_notInitialized
            //
            this.mbutton_notInitialized.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_notInitialized.Image = global::LcmsNet.Properties.Resources.breakpointDisabled;
            this.mbutton_notInitialized.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_notInitialized.Location = new System.Drawing.Point(236, 46);
            this.mbutton_notInitialized.Name = "mbutton_notInitialized";
            this.mbutton_notInitialized.Size = new System.Drawing.Size(106, 66);
            this.mbutton_notInitialized.TabIndex = 3;
            this.mbutton_notInitialized.Text = "Set as not Initialized ";
            this.mbutton_notInitialized.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_notInitialized.UseVisualStyleBackColor = true;
            this.mbutton_notInitialized.Click += new System.EventHandler(this.mbutton_notInitialized_Click);
            //
            // mbutton_doNothing
            //
            this.mbutton_doNothing.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_doNothing.Location = new System.Drawing.Point(348, 46);
            this.mbutton_doNothing.Name = "mbutton_doNothing";
            this.mbutton_doNothing.Size = new System.Drawing.Size(106, 66);
            this.mbutton_doNothing.TabIndex = 4;
            this.mbutton_doNothing.Text = "Cancel";
            this.mbutton_doNothing.UseVisualStyleBackColor = true;
            //
            // formResolveDeviceStatus
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 116);
            this.Controls.Add(this.mbutton_doNothing);
            this.Controls.Add(this.mbutton_notInitialized);
            this.Controls.Add(this.mbutton_clearErrors);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mbutton_leaveError);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formResolveDeviceStatus";
            this.Text = "Resolve Device Status";
            this.ResumeLayout(false);

        }

        #endregion

        private Button mbutton_leaveError;
        private Label label1;
        private Button mbutton_clearErrors;
        private Button mbutton_notInitialized;
        private Button mbutton_doNothing;
    }
}