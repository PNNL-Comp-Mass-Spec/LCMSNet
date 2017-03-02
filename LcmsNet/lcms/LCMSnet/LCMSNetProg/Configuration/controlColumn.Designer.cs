using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Configuration
{
    partial class controlColumn
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mcheckBox_enabled = new System.Windows.Forms.CheckBox();
            this.mcomboBox_names = new System.Windows.Forms.ComboBox();
            this.mlabel_status = new System.Windows.Forms.Label();
            this.mbutton_color = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // mcheckBox_enabled
            //
            this.mcheckBox_enabled.AutoSize = true;
            this.mcheckBox_enabled.Location = new System.Drawing.Point(49, 32);
            this.mcheckBox_enabled.Name = "mcheckBox_enabled";
            this.mcheckBox_enabled.Size = new System.Drawing.Size(65, 17);
            this.mcheckBox_enabled.TabIndex = 0;
            this.mcheckBox_enabled.Text = "Enabled";
            this.mcheckBox_enabled.UseVisualStyleBackColor = true;
            this.mcheckBox_enabled.CheckedChanged += new System.EventHandler(this.mcheckBox_enabled_CheckedChanged);
            //
            // mcomboBox_names
            //
            this.mcomboBox_names.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_names.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_names.FormattingEnabled = true;
            this.mcomboBox_names.Location = new System.Drawing.Point(49, 5);
            this.mcomboBox_names.Name = "mcomboBox_names";
            this.mcomboBox_names.Size = new System.Drawing.Size(178, 21);
            this.mcomboBox_names.TabIndex = 1;
            this.mcomboBox_names.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_names_SelectedIndexChanged);
            this.mcomboBox_names.Enter += new System.EventHandler(this.mcomboBox_names_Enter);
            this.mcomboBox_names.Leave += new System.EventHandler(this.mcomboBox_names_Leave);
            //
            // mlabel_status
            //
            this.mlabel_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_status.AutoEllipsis = true;
            this.mlabel_status.Location = new System.Drawing.Point(120, 31);
            this.mlabel_status.Name = "mlabel_status";
            this.mlabel_status.Size = new System.Drawing.Size(133, 18);
            this.mlabel_status.TabIndex = 4;
            this.mlabel_status.Text = "Status:";
            this.mlabel_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // mbutton_color
            //
            this.mbutton_color.BackColor = System.Drawing.Color.Red;
            this.mbutton_color.Location = new System.Drawing.Point(7, 5);
            this.mbutton_color.Name = "mbutton_color";
            this.mbutton_color.Size = new System.Drawing.Size(36, 29);
            this.mbutton_color.TabIndex = 5;
            this.mbutton_color.UseVisualStyleBackColor = false;
            this.mbutton_color.Click += new System.EventHandler(this.mbutton_color_Click);
            //
            // controlColumn
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mbutton_color);
            this.Controls.Add(this.mlabel_status);
            this.Controls.Add(this.mcomboBox_names);
            this.Controls.Add(this.mcheckBox_enabled);
            this.Name = "controlColumn";
            this.Size = new System.Drawing.Size(266, 57);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox mcheckBox_enabled;
        private ComboBox mcomboBox_names;
        private Label mlabel_status;
        private Button mbutton_color;
    }
}
