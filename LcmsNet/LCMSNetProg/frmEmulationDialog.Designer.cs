using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet
{
    partial class frmEmulationDialog
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
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnEnableEmulation = new System.Windows.Forms.Button();
            this.btnRunSimulationWithoutEmulation = new System.Windows.Forms.Button();
            this.btnCancelSimulationRun = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblWarning
            //
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(184, 9);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(163, 33);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "WARNING!";
            //
            // btnEnableEmulation
            //
            this.btnEnableEmulation.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnEnableEmulation.Location = new System.Drawing.Point(23, 167);
            this.btnEnableEmulation.Name = "btnEnableEmulation";
            this.btnEnableEmulation.Size = new System.Drawing.Size(94, 83);
            this.btnEnableEmulation.TabIndex = 1;
            this.btnEnableEmulation.Text = "Enable Emulation Mode and Run Simulation";
            this.btnEnableEmulation.UseVisualStyleBackColor = true;
            //
            // btnRunSimulationWithoutEmulation
            //
            this.btnRunSimulationWithoutEmulation.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnRunSimulationWithoutEmulation.Location = new System.Drawing.Point(190, 167);
            this.btnRunSimulationWithoutEmulation.Name = "btnRunSimulationWithoutEmulation";
            this.btnRunSimulationWithoutEmulation.Size = new System.Drawing.Size(94, 83);
            this.btnRunSimulationWithoutEmulation.TabIndex = 2;
            this.btnRunSimulationWithoutEmulation.Text = "Do Not Enable Emulation Mode and Run Simulation";
            this.btnRunSimulationWithoutEmulation.UseVisualStyleBackColor = true;
            //
            // btnCancelSimulationRun
            //
            this.btnCancelSimulationRun.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelSimulationRun.Location = new System.Drawing.Point(353, 167);
            this.btnCancelSimulationRun.Name = "btnCancelSimulationRun";
            this.btnCancelSimulationRun.Size = new System.Drawing.Size(94, 83);
            this.btnCancelSimulationRun.TabIndex = 3;
            this.btnCancelSimulationRun.Text = "Cancel Simulation Run";
            this.btnCancelSimulationRun.UseVisualStyleBackColor = true;
            //
            // lblMessage
            //
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(12, 51);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(105, 24);
            this.lblMessage.TabIndex = 4;
            this.lblMessage.Text = "WARNING!";
            //
            // frmEmulationDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 262);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnCancelSimulationRun);
            this.Controls.Add(this.btnRunSimulationWithoutEmulation);
            this.Controls.Add(this.btnEnableEmulation);
            this.Controls.Add(this.lblWarning);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEmulationDialog";
            this.Text = "WARNING!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblWarning;
        private Button btnEnableEmulation;
        private Button btnRunSimulationWithoutEmulation;
        private Button btnCancelSimulationRun;
        private Label lblMessage;
    }
}