namespace LcmsNet.Method.Forms
{
    partial class controlLCMethodEvent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(controlLCMethodEvent));
            this.mcomboBox_method = new System.Windows.Forms.ComboBox();
            this.mcomboBox_devices = new System.Windows.Forms.ComboBox();
            this.mcheckBox_selected = new System.Windows.Forms.CheckBox();
            this.mpanel_parameters = new System.Windows.Forms.TableLayoutPanel();
            this.mpanel_extras = new System.Windows.Forms.Panel();
            this.mcheckBox_optimizeFor = new System.Windows.Forms.CheckBox();
            this.labelEventNumber = new System.Windows.Forms.Label();
            this.controlBreakpoint1 = new LcmsNet.Method.Forms.controlBreakpoint();
            this.mpanel_extras.SuspendLayout();
            this.SuspendLayout();
            //
            // mcomboBox_method
            //
            this.mcomboBox_method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_method.FormattingEnabled = true;
            this.mcomboBox_method.Location = new System.Drawing.Point(243, 5);
            this.mcomboBox_method.Name = "mcomboBox_method";
            this.mcomboBox_method.Size = new System.Drawing.Size(147, 21);
            this.mcomboBox_method.TabIndex = 2;
            //
            // mcomboBox_devices
            //
            this.mcomboBox_devices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_devices.FormattingEnabled = true;
            this.mcomboBox_devices.Location = new System.Drawing.Point(90, 5);
            this.mcomboBox_devices.Name = "mcomboBox_devices";
            this.mcomboBox_devices.Size = new System.Drawing.Size(147, 21);
            this.mcomboBox_devices.Sorted = true;
            this.mcomboBox_devices.TabIndex = 1;
            this.mcomboBox_devices.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_devices_SelectedIndexChanged);
            //
            // mcheckBox_selected
            //
            this.mcheckBox_selected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mcheckBox_selected.BackColor = System.Drawing.Color.Silver;
            this.mcheckBox_selected.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mcheckBox_selected.Location = new System.Drawing.Point(64, -1);
            this.mcheckBox_selected.Name = "mcheckBox_selected";
            this.mcheckBox_selected.Size = new System.Drawing.Size(20, 32);
            this.mcheckBox_selected.TabIndex = 0;
            this.mcheckBox_selected.UseVisualStyleBackColor = false;
            //
            // mpanel_parameters
            //
            this.mpanel_parameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_parameters.BackColor = System.Drawing.Color.White;
            this.mpanel_parameters.ColumnCount = 1;
            this.mpanel_parameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mpanel_parameters.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.mpanel_parameters.Location = new System.Drawing.Point(396, -1);
            this.mpanel_parameters.Name = "mpanel_parameters";
            this.mpanel_parameters.RowCount = 1;
            this.mpanel_parameters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mpanel_parameters.Size = new System.Drawing.Size(542, 33);
            this.mpanel_parameters.TabIndex = 3;
            this.mpanel_parameters.Paint += new System.Windows.Forms.PaintEventHandler(this.mpanel_parameters_Paint);
            //
            // mpanel_extras
            //
            this.mpanel_extras.Controls.Add(this.mcheckBox_optimizeFor);
            this.mpanel_extras.Dock = System.Windows.Forms.DockStyle.Right;
            this.mpanel_extras.Location = new System.Drawing.Point(941, 0);
            this.mpanel_extras.Name = "mpanel_extras";
            this.mpanel_extras.Size = new System.Drawing.Size(34, 31);
            this.mpanel_extras.TabIndex = 10;
            //
            // mcheckBox_optimizeFor
            //
            this.mcheckBox_optimizeFor.AutoSize = true;
            this.mcheckBox_optimizeFor.Image = global::LcmsNet.Properties.Resources.highlighter;
            this.mcheckBox_optimizeFor.Location = new System.Drawing.Point(3, 8);
            this.mcheckBox_optimizeFor.Name = "mcheckBox_optimizeFor";
            this.mcheckBox_optimizeFor.Size = new System.Drawing.Size(31, 16);
            this.mcheckBox_optimizeFor.TabIndex = 4;
            this.mcheckBox_optimizeFor.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mcheckBox_optimizeFor.UseVisualStyleBackColor = true;
            this.mcheckBox_optimizeFor.CheckedChanged += new System.EventHandler(this.mcheckBox_optimizeFor_CheckedChanged);
            //
            // labelEventNumber
            //
            this.labelEventNumber.Location = new System.Drawing.Point(4, 10);
            this.labelEventNumber.Name = "labelEventNumber";
            this.labelEventNumber.Size = new System.Drawing.Size(32, 16);
            this.labelEventNumber.TabIndex = 12;
            this.labelEventNumber.Text = "100";
            //
            // controlBreakpoint1
            //
            this.controlBreakpoint1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("controlBreakpoint1.BackgroundImage")));
            this.controlBreakpoint1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.controlBreakpoint1.IsSet = false;
            this.controlBreakpoint1.Location = new System.Drawing.Point(42, 10);
            this.controlBreakpoint1.Name = "controlBreakpoint1";
            this.controlBreakpoint1.Size = new System.Drawing.Size(16, 16);
            this.controlBreakpoint1.TabIndex = 11;
            //
            // controlLCMethodEvent
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelEventNumber);
            this.Controls.Add(this.controlBreakpoint1);
            this.Controls.Add(this.mpanel_extras);
            this.Controls.Add(this.mpanel_parameters);
            this.Controls.Add(this.mcheckBox_selected);
            this.Controls.Add(this.mcomboBox_devices);
            this.Controls.Add(this.mcomboBox_method);
            this.Name = "controlLCMethodEvent";
            this.Size = new System.Drawing.Size(975, 31);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.controlLCMethodEvent_Paint);
            this.mpanel_extras.ResumeLayout(false);
            this.mpanel_extras.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox mcomboBox_method;
        private System.Windows.Forms.ComboBox mcomboBox_devices;
        private System.Windows.Forms.CheckBox mcheckBox_selected;
        private System.Windows.Forms.TableLayoutPanel mpanel_parameters;
        private System.Windows.Forms.Panel mpanel_extras;
        private System.Windows.Forms.CheckBox mcheckBox_optimizeFor;
        private controlBreakpoint controlBreakpoint1;
        private System.Windows.Forms.Label labelEventNumber;
    }
}
