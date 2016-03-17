namespace LcmsNet.SampleQueue.Forms
{
    partial class controlColumnView
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
            this.mpanel_control = new System.Windows.Forms.Panel();
            this.mbutton_dmsEdit = new System.Windows.Forms.Button();
            this.mbutton_cartColumnDate = new System.Windows.Forms.Button();
            this.mbutton_moveColumns = new System.Windows.Forms.Button();
            this.mbutton_randomize = new System.Windows.Forms.Button();
            this.mbutton_trayVial = new System.Windows.Forms.Button();
            this.mbutton_fillDown = new System.Windows.Forms.Button();
            this.mbutton_addDMS = new System.Windows.Forms.Button();
            this.mbutton_addBlankAppend = new System.Windows.Forms.Button();
            this.mbutton_addBlank = new System.Windows.Forms.Button();
            this.mbutton_removeSelected = new System.Windows.Forms.Button();
            this.mbutton_deleteUnused = new System.Windows.Forms.Button();
            this.mbutton_down = new System.Windows.Forms.Button();
            this.mbutton_up = new System.Windows.Forms.Button();
            this.mbutton_expand = new System.Windows.Forms.Button();
            this.mlabel_columnNameHeader = new System.Windows.Forms.Label();
            this.mpanel_control.SuspendLayout();
            this.SuspendLayout();
            //
            // m_selector
            //
            this.m_selector.Location = new System.Drawing.Point(150, 150);
            //
            // mpanel_control
            //
            this.mpanel_control.Controls.Add(this.mbutton_dmsEdit);
            this.mpanel_control.Controls.Add(this.mbutton_cartColumnDate);
            this.mpanel_control.Controls.Add(this.mbutton_moveColumns);
            this.mpanel_control.Controls.Add(this.mbutton_randomize);
            this.mpanel_control.Controls.Add(this.mbutton_trayVial);
            this.mpanel_control.Controls.Add(this.mbutton_fillDown);
            this.mpanel_control.Controls.Add(this.mbutton_addDMS);
            this.mpanel_control.Controls.Add(this.mbutton_addBlankAppend);
            this.mpanel_control.Controls.Add(this.mbutton_addBlank);
            this.mpanel_control.Controls.Add(this.mbutton_removeSelected);
            this.mpanel_control.Controls.Add(this.mbutton_deleteUnused);
            this.mpanel_control.Controls.Add(this.mbutton_down);
            this.mpanel_control.Controls.Add(this.mbutton_up);
            this.mpanel_control.Controls.Add(this.mbutton_expand);
            this.mpanel_control.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_control.Location = new System.Drawing.Point(3, 471);
            this.mpanel_control.Name = "mpanel_control";
            this.mpanel_control.Padding = new System.Windows.Forms.Padding(1);
            this.mpanel_control.Size = new System.Drawing.Size(860, 104);
            this.mpanel_control.TabIndex = 20;
            //
            // mbutton_dmsEdit
            //
            this.mbutton_dmsEdit.BackColor = System.Drawing.Color.White;
            this.mbutton_dmsEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_dmsEdit.Image = global::LcmsNet.Properties.Resources.DMSEdit;
            this.mbutton_dmsEdit.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_dmsEdit.Location = new System.Drawing.Point(695, 3);
            this.mbutton_dmsEdit.Name = "mbutton_dmsEdit";
            this.mbutton_dmsEdit.Size = new System.Drawing.Size(57, 96);
            this.mbutton_dmsEdit.TabIndex = 60;
            this.mbutton_dmsEdit.Text = "DMS Edit";
            this.mbutton_dmsEdit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_dmsEdit.UseVisualStyleBackColor = false;
            this.mbutton_dmsEdit.Click += new System.EventHandler(this.mbutton_dmsEdit_Click);
            //
            // mbutton_cartColumnDate
            //
            this.mbutton_cartColumnDate.BackColor = System.Drawing.Color.White;
            this.mbutton_cartColumnDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_cartColumnDate.Image = global::LcmsNet.Properties.Resources.CartColumnName;
            this.mbutton_cartColumnDate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_cartColumnDate.Location = new System.Drawing.Point(629, 4);
            this.mbutton_cartColumnDate.Name = "mbutton_cartColumnDate";
            this.mbutton_cartColumnDate.Size = new System.Drawing.Size(57, 96);
            this.mbutton_cartColumnDate.TabIndex = 59;
            this.mbutton_cartColumnDate.Text = "Cart, Col, Date";
            this.mbutton_cartColumnDate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_cartColumnDate.UseVisualStyleBackColor = false;
            this.mbutton_cartColumnDate.Click += new System.EventHandler(this.mbutton_cartColumnDate_Click);
            //
            // mbutton_moveColumns
            //
            this.mbutton_moveColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_moveColumns.BackColor = System.Drawing.Color.White;
            this.mbutton_moveColumns.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_moveColumns.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_moveColumns.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_moveColumns.Image = global::LcmsNet.Properties.Resources.Column;
            this.mbutton_moveColumns.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_moveColumns.Location = new System.Drawing.Point(565, 4);
            this.mbutton_moveColumns.Name = "mbutton_moveColumns";
            this.mbutton_moveColumns.Size = new System.Drawing.Size(57, 96);
            this.mbutton_moveColumns.TabIndex = 55;
            this.mbutton_moveColumns.Text = "Move";
            this.mbutton_moveColumns.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_moveColumns.UseVisualStyleBackColor = false;
            this.mbutton_moveColumns.Click += new System.EventHandler(this.mbutton_moveColumns_Click);
            //
            // mbutton_randomize
            //
            this.mbutton_randomize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_randomize.BackColor = System.Drawing.Color.White;
            this.mbutton_randomize.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_randomize.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_randomize.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_randomize.Image = global::LcmsNet.Properties.Resources.Randomize;
            this.mbutton_randomize.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_randomize.Location = new System.Drawing.Point(502, 4);
            this.mbutton_randomize.Name = "mbutton_randomize";
            this.mbutton_randomize.Size = new System.Drawing.Size(57, 96);
            this.mbutton_randomize.TabIndex = 56;
            this.mbutton_randomize.Text = "Randomize";
            this.mbutton_randomize.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_randomize.UseVisualStyleBackColor = false;
            this.mbutton_randomize.Click += new System.EventHandler(this.mbutton_randomize_Click);
            //
            // mbutton_trayVial
            //
            this.mbutton_trayVial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_trayVial.BackColor = System.Drawing.Color.White;
            this.mbutton_trayVial.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_trayVial.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_trayVial.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_trayVial.Image = global::LcmsNet.Properties.Resources.testTube;
            this.mbutton_trayVial.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_trayVial.Location = new System.Drawing.Point(439, 4);
            this.mbutton_trayVial.Name = "mbutton_trayVial";
            this.mbutton_trayVial.Size = new System.Drawing.Size(57, 96);
            this.mbutton_trayVial.TabIndex = 58;
            this.mbutton_trayVial.Text = "Tray Vial";
            this.mbutton_trayVial.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_trayVial.UseVisualStyleBackColor = false;
            this.mbutton_trayVial.Click += new System.EventHandler(this.mbutton_trayVial_Click);
            //
            // mbutton_fillDown
            //
            this.mbutton_fillDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_fillDown.BackColor = System.Drawing.Color.White;
            this.mbutton_fillDown.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_fillDown.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_fillDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_fillDown.Image = global::LcmsNet.Properties.Resources.Filldown;
            this.mbutton_fillDown.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_fillDown.Location = new System.Drawing.Point(377, 4);
            this.mbutton_fillDown.Name = "mbutton_fillDown";
            this.mbutton_fillDown.Size = new System.Drawing.Size(57, 96);
            this.mbutton_fillDown.TabIndex = 57;
            this.mbutton_fillDown.Text = "Fill Down";
            this.mbutton_fillDown.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_fillDown.UseVisualStyleBackColor = false;
            this.mbutton_fillDown.Click += new System.EventHandler(this.mbutton_fillDown_Click);
            //
            // mbutton_addDMS
            //
            this.mbutton_addDMS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_addDMS.BackColor = System.Drawing.Color.White;
            this.mbutton_addDMS.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_addDMS.Image = global::LcmsNet.Properties.Resources.AddDMS;
            this.mbutton_addDMS.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_addDMS.Location = new System.Drawing.Point(116, 4);
            this.mbutton_addDMS.Name = "mbutton_addDMS";
            this.mbutton_addDMS.Size = new System.Drawing.Size(45, 96);
            this.mbutton_addDMS.TabIndex = 54;
            this.mbutton_addDMS.Text = "DMS";
            this.mbutton_addDMS.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_addDMS.UseVisualStyleBackColor = false;
            this.mbutton_addDMS.Click += new System.EventHandler(this.mbutton_addDMS_Click);
            //
            // mbutton_addBlankAppend
            //
            this.mbutton_addBlankAppend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_addBlankAppend.BackColor = System.Drawing.Color.White;
            this.mbutton_addBlankAppend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_addBlankAppend.ForeColor = System.Drawing.Color.Black;
            this.mbutton_addBlankAppend.Image = global::LcmsNet.Properties.Resources.AddAppendBlank;
            this.mbutton_addBlankAppend.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_addBlankAppend.Location = new System.Drawing.Point(58, 4);
            this.mbutton_addBlankAppend.Name = "mbutton_addBlankAppend";
            this.mbutton_addBlankAppend.Size = new System.Drawing.Size(52, 96);
            this.mbutton_addBlankAppend.TabIndex = 53;
            this.mbutton_addBlankAppend.Text = "Blank";
            this.mbutton_addBlankAppend.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_addBlankAppend.UseVisualStyleBackColor = false;
            this.mbutton_addBlankAppend.Click += new System.EventHandler(this.mbutton_addBlankAppend_Click);
            //
            // mbutton_addBlank
            //
            this.mbutton_addBlank.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_addBlank.BackColor = System.Drawing.Color.White;
            this.mbutton_addBlank.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_addBlank.ForeColor = System.Drawing.Color.Black;
            this.mbutton_addBlank.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_addBlank.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_addBlank.Location = new System.Drawing.Point(2, 4);
            this.mbutton_addBlank.Name = "mbutton_addBlank";
            this.mbutton_addBlank.Size = new System.Drawing.Size(50, 96);
            this.mbutton_addBlank.TabIndex = 52;
            this.mbutton_addBlank.Text = "Blank";
            this.mbutton_addBlank.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_addBlank.UseVisualStyleBackColor = false;
            this.mbutton_addBlank.Click += new System.EventHandler(this.mbutton_addBlank_Click);
            //
            // mbutton_removeSelected
            //
            this.mbutton_removeSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_removeSelected.BackColor = System.Drawing.Color.White;
            this.mbutton_removeSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_removeSelected.ForeColor = System.Drawing.Color.Black;
            this.mbutton_removeSelected.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_removeSelected.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_removeSelected.Location = new System.Drawing.Point(167, 4);
            this.mbutton_removeSelected.Name = "mbutton_removeSelected";
            this.mbutton_removeSelected.Size = new System.Drawing.Size(46, 96);
            this.mbutton_removeSelected.TabIndex = 51;
            this.mbutton_removeSelected.Text = "Selected";
            this.mbutton_removeSelected.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_removeSelected.UseVisualStyleBackColor = false;
            this.mbutton_removeSelected.Click += new System.EventHandler(this.mbutton_removeSelected_Click);
            //
            // mbutton_deleteUnused
            //
            this.mbutton_deleteUnused.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_deleteUnused.BackColor = System.Drawing.Color.White;
            this.mbutton_deleteUnused.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_deleteUnused.ForeColor = System.Drawing.Color.Black;
            this.mbutton_deleteUnused.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_deleteUnused.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_deleteUnused.Location = new System.Drawing.Point(219, 4);
            this.mbutton_deleteUnused.Name = "mbutton_deleteUnused";
            this.mbutton_deleteUnused.Size = new System.Drawing.Size(48, 96);
            this.mbutton_deleteUnused.TabIndex = 33;
            this.mbutton_deleteUnused.Text = "Unused";
            this.mbutton_deleteUnused.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_deleteUnused.UseVisualStyleBackColor = false;
            this.mbutton_deleteUnused.Click += new System.EventHandler(this.mbutton_removeUnused_Click);
            //
            // mbutton_down
            //
            this.mbutton_down.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_down.BackColor = System.Drawing.Color.White;
            this.mbutton_down.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_down.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.mbutton_down.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_down.Location = new System.Drawing.Point(326, 4);
            this.mbutton_down.Name = "mbutton_down";
            this.mbutton_down.Size = new System.Drawing.Size(47, 96);
            this.mbutton_down.TabIndex = 50;
            this.mbutton_down.UseVisualStyleBackColor = false;
            this.mbutton_down.Click += new System.EventHandler(this.mbutton_down_Click);
            //
            // mbutton_up
            //
            this.mbutton_up.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_up.BackColor = System.Drawing.Color.White;
            this.mbutton_up.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mbutton_up.Image = global::LcmsNet.Properties.Resources.Button_Up_16;
            this.mbutton_up.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mbutton_up.Location = new System.Drawing.Point(273, 4);
            this.mbutton_up.Name = "mbutton_up";
            this.mbutton_up.Size = new System.Drawing.Size(47, 96);
            this.mbutton_up.TabIndex = 49;
            this.mbutton_up.UseVisualStyleBackColor = false;
            this.mbutton_up.Click += new System.EventHandler(this.mbutton_up_Click);
            //
            // mbutton_expand
            //
            this.mbutton_expand.BackColor = System.Drawing.Color.Transparent;
            this.mbutton_expand.Dock = System.Windows.Forms.DockStyle.Right;
            this.mbutton_expand.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_expand.ForeColor = System.Drawing.Color.Black;
            this.mbutton_expand.Image = global::LcmsNet.Properties.Resources.Expand;
            this.mbutton_expand.Location = new System.Drawing.Point(824, 1);
            this.mbutton_expand.Name = "mbutton_expand";
            this.mbutton_expand.Size = new System.Drawing.Size(35, 102);
            this.mbutton_expand.TabIndex = 48;
            this.mbutton_expand.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mbutton_expand.UseVisualStyleBackColor = false;
            this.mbutton_expand.Click += new System.EventHandler(this.mbutton_expand_Click);
            this.mbutton_expand.MouseHover += new System.EventHandler(this.mbutton_expand_MouseHover);
            //
            // mlabel_columnNameHeader
            //
            this.mlabel_columnNameHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_columnNameHeader.Location = new System.Drawing.Point(3, 3);
            this.mlabel_columnNameHeader.Name = "mlabel_columnNameHeader";
            this.mlabel_columnNameHeader.Size = new System.Drawing.Size(860, 13);
            this.mlabel_columnNameHeader.TabIndex = 41;
            this.mlabel_columnNameHeader.Text = "Column:";
            this.mlabel_columnNameHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // controlColumnView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mpanel_control);
            this.Controls.Add(this.mlabel_columnNameHeader);
            this.Name = "controlColumnView";
            this.Size = new System.Drawing.Size(866, 578);
            this.Controls.SetChildIndex(this.mlabel_columnNameHeader, 0);
            this.Controls.SetChildIndex(this.mpanel_control, 0);
            this.mpanel_control.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel mpanel_control;
        private System.Windows.Forms.Label mlabel_columnNameHeader;
        private System.Windows.Forms.Button mbutton_deleteUnused;
        private System.Windows.Forms.Button mbutton_expand;
        private System.Windows.Forms.Button mbutton_down;
        private System.Windows.Forms.Button mbutton_up;
        private System.Windows.Forms.Button mbutton_addDMS;
        private System.Windows.Forms.Button mbutton_addBlankAppend;
        private System.Windows.Forms.Button mbutton_addBlank;
        private System.Windows.Forms.Button mbutton_removeSelected;
        private System.Windows.Forms.Button mbutton_moveColumns;
        private System.Windows.Forms.Button mbutton_randomize;
        private System.Windows.Forms.Button mbutton_trayVial;
        private System.Windows.Forms.Button mbutton_fillDown;
        private System.Windows.Forms.Button mbutton_dmsEdit;
        private System.Windows.Forms.Button mbutton_cartColumnDate;





    }
}
